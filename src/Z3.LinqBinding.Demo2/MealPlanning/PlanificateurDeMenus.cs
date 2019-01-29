using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Z3.LinqBinding;

namespace Z3.LinqBinding.Demo
{
    public class PlanificateurDeMenus
    {

	    public Dietetique Dietetique;

        public int NombreMenus { get; set; } = 7;
        public int NombrePlatsParMenu { get; set; } = 5;

        public List<Menu> Menus { get; set; }

        public PlanificateurDeMenus()
        {
            
        }

        public PlanificateurDeMenus(Dietetique dietetique)
        {
            Dietetique = dietetique;
        }

        public Theorem<PlanificateurDeMenus> Create(Z3Context context, List<Patient> patients)
        {
            var nbConstituants = Dietetique.Constituants.Count;
            var theorem = context.NewTheorem<PlanificateurDeMenus>();

            var indicesMenus = Enumerable.Range(0, NombreMenus).ToArray();
            var indicesPlats = Enumerable.Range(0, NombrePlatsParMenu).ToArray();

            //Variables globales
            theorem.Assert(t => t.NombreMenus == NombreMenus);
            theorem.Assert(t => t.NombrePlatsParMenu == NombrePlatsParMenu);

            // Pas deux fois le même plat
            theorem.Assert(t => Z3Methods.Distinct(indicesMenus
                    .Select(m => indicesPlats
                        .Select(p => t.Menus[m].Plats[p].PlatId).ToArray())
                    .ToArray()));

            // Plats parmi la collection proposée
            for (int m = 0; m < NombreMenus; m++)
            {
                var m1 = m;
                for (int p = 0; p < NombrePlatsParMenu; p++)
                {
                    // Les plats sont dans la collection
                    
                    var p1 = p;
                    theorem.Assert(t =>
                        t.Menus[m1].Plats[p1].PlatId >= 0 && t.Menus[m1].Plats[p1].PlatId < Dietetique.Plats.Count);

                    // Les compositions sont celles des plats et l'ordre des plats est respecté
                    for (int recetteId = 0; recetteId < Dietetique.Plats.Count; recetteId++)
                    {
                        var recetteId1 = recetteId;
                        
                        for (int c = 0; c < nbConstituants; c++)
                        {
                            var c1 = c;
                            theorem.Assert(t => t.Menus[m1].Plats[p1].PlatId != recetteId1
                                                         || (t.Menus[m1].Plats[p1].Compositions[c1] == Dietetique.Plats[recetteId1].Compositions[c1] // Bonne composition
                                                             && Dietetique.Plats[recetteId1].Ordre == p1 + 1)); //Ordre respecté
                        }

                    }

                }

                // La composition des menu est la somme de celle des plats
                for (int c = 0; c < nbConstituants; c++)
                {
                    var c1 = c;
                    theorem.Assert(t => t.Menus[m1].Compositions[c1] == indicesPlats
                                                .Select(p=>t.Menus[m1].Plats[p].Compositions[c1]).Sum());
                }
                

                // Les menus respectent les restrictions des patients.
                for (int p = 0; p < patients.Count; p++)
                {
                    var p1 = p;
                    for (int c = 0; c < nbConstituants; c++)
                    {
                        var c1 = c;
                        var restriction = patients[p1].Restrictions[c1];
                        theorem.Assert(t => (restriction.Min == -1 || t.Menus[m1].Compositions[c1] > restriction.Min)
                                                      && (restriction.Max == -1 || t.Menus[m1].Compositions[c1] < restriction.Max));
                    }
                }
            }

            return theorem;

        }


        public List<MenuReadable> ToReadable()
        {
            return Menus.Select(m => new MenuReadable()
            {
                Plats = m.Plats.Select(p => new PlatReadable()
                {
                    Nom = Dietetique.Plats[p.PlatId].Nom,
					Ingredients = p.Ingredients.Select(i => new IngredientReadable()
					{
						Denree = new DenreeReadable()
						{
							Nom = Dietetique.Denrees[i.Denree].Nom,
							Compositions = Dietetique.Denrees[i.Denree].Compositions.Select((c, cIndex) => new CompositionReadable()
							{
								Constituant = Dietetique.Constituants[cIndex],
								Teneur = c
							}).ToList()
						},
						Quantite = i.Quantite
					}).ToList(),
					//Compositions = p.Compositions.Select((c, cIndex) => new CompositionReadable()
					//{
					//	Constituant = Dietetique.Constituants[cIndex],
					//	Teneur = c
					//}).ToList()
				}).ToList(),
				//Compositions = m.Compositions
			}).ToList();
        }

        public override string ToString()
        {
            var readable = ToReadable();
            return JsonConvert.SerializeObject(readable, Formatting.Indented);
        }
    }


    public class Patient
    {
        public Restriction[] Restrictions { get; set; }

	    public void AddRestriction(Dietetique dietetique, RestrictionReadable restriction)
	    {
		    if (Restrictions == null || Restrictions.Length ==0)
		    {
			    Restrictions = dietetique.Constituants.Select(c => new Restriction()).ToArray();
		    }

		    var idxConstituant = dietetique.Constituants.FindIndex(c => c.Nom == restriction.Constituant);
		    if (restriction.Min != -1)
		    {
			    Restrictions[idxConstituant].Min = restriction.Min;
		    }
		    if (restriction.Max != -1)
		    {
			    Restrictions[idxConstituant].Max = restriction.Max;
		    }

		}


    }


    public class Restriction
    {


        public Decimal Min { get; set; } = -1;

        public Decimal Max { get; set; } = -1;

    }

    public class Dietetique
    {

        public List<Constituant> Constituants { get; set; }

        public List<DenreeImport> Denrees { get; set; }

        public List<PlatImport> Plats { get; set; }

        //public List<Menu> Menus { get; set; }

        public static Dietetique Load(string basePath)
        {
            Dietetique toReturn;
            var savePath = Path.Combine(basePath, @"dietetique.json");
            if (File.Exists(savePath))
            {
                var strDietetique = File.ReadAllText(savePath);
                toReturn = JsonConvert.DeserializeObject<Dietetique>(strDietetique);
            }
            else
            {
                toReturn = new Dietetique();
                var ciqualPath = Path.Combine(basePath, @"Ciqual\");
                var ciqual = Ciqual.Load(ciqualPath);
                var recettesPath = Path.Combine(basePath, @"Recettes\");
                var recettes = Recettes.Load(recettesPath);

                // On charge la liste des constituants depuis celle du fichier correspondant, en ne conservant que le nom
                toReturn.Constituants = ciqual.Constituants.CONST
                    .Select(c => new Constituant() { Nom = c.const_nom_fr }).ToList();
                // On obtient les constituant des aliments en construisant un tableau de la taill de notre liste de constituant, et en croisant 3 fichiers (soit 2 jointures): les constituants, les compositions (constituant+aliments = teneur), et les aliments
                var constituantsAliments = toReturn.Constituants
                    .Select(c => ciqual.Constituants.CONST
                        .Where(ciqualConst => ciqualConst.const_nom_fr == c.Nom).ToArray()
                        .Join(ciqual.Compositions.COMPO, ciqualConst => ciqualConst.const_code,
                            compo => compo.const_code,
                            (constCiqual, compoCiqual) => new
                            {
                                Teneur = Decimal.Parse(compoCiqual.teneur.Replace("traces", "0").Replace('<', ' ').Replace('-', '0').Trim(), new CultureInfo("fr")),
                                CodeAlim = compoCiqual.alim_code
                            }).ToArray()
                        .Join(ciqual.Aliments.ALIM, compo => compo.CodeAlim,
                            alim => alim.alim_code,
                            (compo, alim) => new
                            {
                                NomAliment = alim.alim_nom_fr,
                                compo.Teneur
                            })
		                .Where(compoAlim=> compoAlim.Teneur>0)
		                .ToArray())
                    .ToArray();
                // On filtre les denrées (= aliments dans le fichier de recette) pour garder celles qui ont un nom
                var denreesFiltered = recettes.Denrees
                    .Where(d => !string.IsNullOrEmpty(d.fields.libelle_denree))
                    .GroupBy(d => d.fields.libelle_denree)
                    .Select(g => g.First()).ToArray();
				// On essaie de matcher les denrées des recettes aux aliments de la base ciqual par proximité sémantique
				var matchingMots = denreesFiltered
                    .Select(d => new
                    {
                        NomDenree = d.fields.libelle_denree,
                        constituantsAliments[0]
                            .OrderByDescending(a => CalculeProximiteLexicale(a.NomAliment, d.fields.libelle_denree))
                            .First().NomAliment,
                        ProximiteLexicale = constituantsAliments[0]
                            .Max(a => CalculeProximiteLexicale(a.NomAliment, d.fields.libelle_denree))
                    })
                    .Where(match => match.NomAliment != ciqual.Aliments.ALIM[0].alim_nom_fr)
                    .ToDictionary(a => a.NomDenree, a => new { Nom = a.NomAliment, Proximite = a.ProximiteLexicale });

                // On charge les denrées à partir des noms de celles qu'on a réussi à matcher, et en retrouvant la composition par notre précédente jointure
                toReturn.Denrees = matchingMots
                    .Select(d => new DenreeImport()
                    {
                        Nom = d.Key,
                        Compositions = constituantsAliments
                        .Select(compAliments => compAliments.FirstOrDefault(a => a.NomAliment == matchingMots[d.Key].Nom)?.Teneur ?? 0).ToArray()
                    }).ToList();
	            //On remet les doublons pour les recettes
				var denreesFilteredAvecDoublons = recettes.Denrees.Where(d =>
		            denreesFiltered.Any(dFiltered => dFiltered.fields.libelle_denree == d.fields.libelle_denree)).ToArray();
				//On croise le fichier des denrées et des plats pour avoir les ingrédients des plats
				var denreesPlats = recettes.Plats
	                .Join(recettes.Menus,p=> p.fields.code_plat, m => m.fields.code_plat, 
		                (p, m) => new
		                {
			                Plat = p,
			                Ordre = m.fields.ordre_plat
		                })
					.GroupBy(p=>p.Plat.fields.libelle_plat).Select(g=>g.First()) //On enlève les plats en doublons
                    .GroupJoin(denreesFilteredAvecDoublons, p => p.Plat.fields.code_recette, d => d.fields.code_recette,
                        (plat, denreesPlat) => new
                        {
	                        NomPlat = plat.Plat.fields.libelle_plat,
							Ordre = plat.Ordre,
	                        DenreesPlat = denreesPlat.ToArray()
                        })
					.Where(p=>p.DenreesPlat.Length>0) //On ne veut pas les plats sans denrées
					.ToArray();
                //On charge les plats à partir des ingrédients qu'on vient de constituer, et pour la composition du croisement précédément utilisé 
                toReturn.Plats = denreesPlats.Select(platEtDenrees => new PlatImport()
                {
                    Nom = platEtDenrees.NomPlat,
					Ordre = platEtDenrees.Ordre,
					Ingredients = platEtDenrees.DenreesPlat.Select(denreePlat => new Ingredient()
                    {
                        Denree = toReturn.Denrees.FindIndex(d => d.Nom == denreePlat.fields.libelle_denree),
                        Quantite = 1
                    }).Where(p => p.Denree >= 0).ToList(),
                    Compositions = toReturn.Denrees
                                .Where(denree => platEtDenrees.DenreesPlat
                                    .Any(denreePlat =>
                                        denreePlat.fields.libelle_denree ==
                                        denree.Nom))
                                .Select(compAliment => compAliment.Compositions)
                                .Aggregate(new decimal[toReturn.Constituants.Count], (composition1, composition2) => composition1.Zip(composition2, (a, b) => (a + b)).ToArray())

                }).ToList();
                var strDietetique = JsonConvert.SerializeObject(toReturn, Formatting.Indented);
                File.WriteAllText(savePath, strDietetique);
            }
            return toReturn;
        }

        private static double CalculeProximiteLexicale(string s1, string s2)
        {

            var bag1 = CreateSimpleList(s1);
            var bag2 = CreateSimpleList(s2);

            var toReturn = 0d;
            for (var wordIndex = 0; wordIndex < bag2.Count; wordIndex++)
            {
                var word = bag2[wordIndex];
                var idx = bag1.IndexOf(word);
                if (idx > -1)
                {
                    toReturn = toReturn + (1.0 / (2.0 * (idx+wordIndex) + 1.0));
                }
            }


            toReturn -= (bag1.Count + bag2.Count) / 1000.0;
            return toReturn;
        }


        private static List<string> CreateSimpleList(string s)
        {
            var toReturn = new List<string>();
            foreach (var mot in s.Split(new []{' '} , StringSplitOptions.RemoveEmptyEntries))
            {
                var motSimplifie = Regex.Replace(mot.ToUpperInvariant(), @"\W", "");
                motSimplifie = motSimplifie.TrimEnd('S').TrimEnd('E');
                if (motSimplifie.Length>1)
                {
                    toReturn.Add(motSimplifie);
                }
            }

            return toReturn;
        }

    }

    public class Menu
    {

        public List<Plat> Plats { get; set; } = new List<Plat>();


        public Decimal[] Compositions { get; set; } 



    }


    public class Plat
    {

        public int PlatId { get; set; }


        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public Decimal[] Compositions { get; set; } 


    }


    public class PlatImport
    {

        public string Nom { get; set; }

		public int Ordre { get; set; }

		public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public Decimal[] Compositions { get; set; } 

    }


    public class Ingredient
    {

        public int Denree { get; set; }

        public decimal Quantite { get; set; }

    }


    public class DenreeImport
    {

        public string Nom { get; set; } = "";


        public Decimal[] Compositions { get; set; }
    }




    public class Constituant
    {

        public string Nom { get; set; }

    }


    public class MenuReadable
    {

        public List<PlatReadable> Plats { get; set; }

	    public Decimal[] Compositions { get; set; }


		public Decimal GetTeneur(string constituant)
        {
            return Plats.Select(
                p => p.Ingredients.Select(
                    i => i.Quantite * i.Denree.Compositions
                             .Where(c => c.Constituant.Nom == constituant)
                             .Select(c => c.Teneur).Sum()
                ).Sum()
            ).Sum();
        }

    }

    public class PlatReadable
    {

        public string Nom { get; set; }

        public List<IngredientReadable> Ingredients { get; set; } = new List<IngredientReadable>();

	    public List<CompositionReadable> Compositions { get; set; } = new List<CompositionReadable>();

	}

    public class IngredientReadable
    {

        public DenreeReadable Denree { get; set; }

        public Decimal Quantite { get; set; }

    }



    public class DenreeReadable
    {

        public string Nom { get; set; } = "";


        public List<CompositionReadable> Compositions { get; set; } = new List<CompositionReadable>();

    }


    public class CompositionReadable
    {

        public Constituant Constituant { get; set; }

        public Decimal Teneur { get; set; }

    }


	public class RestrictionReadable
	{

		public string Constituant { get; set; }

		public decimal Min { get; set; }

		public decimal Max { get; set; }

	}


	//public class Composition
	//{

	//    public string Constituant { get; set; }

	//    public Decimal Teneur { get; set; }

	//}


	//public class Restriction3
	//{

	//    public string Constituant { get; set; }

	//    public decimal Min { get; set; }

	//    public decimal Max { get; set; }

	//}




	//public class MenuSemaine
	//{

	//    public MenuLiteral Lundi { get; set; }
	//    public MenuLiteral Mardi { get; set; }
	//    public MenuLiteral Mercredi { get; set; }
	//    public MenuLiteral Jeudi { get; set; }
	//    public MenuLiteral Vendredi { get; set; }
	//    public MenuLiteral Samedi { get; set; }
	//    public MenuLiteral Dimanche { get; set; }

	//}


	//public class MenuLiteral
	//{

	//    public Plat Entree { get; set; }
	//    public Plat Viande { get; set; }
	//    public Plat Legumes { get; set; }
	//    public Plat Laitage { get; set; }
	//    public Plat Dessert { get; set; }
	//    public Plat Pain { get; set; }
	//    public Plat Gouter { get; set; }

	//}


	//public class DenreeLiteral
	//{

	//    public string Nom { get; set; } = "";

	//    public CompositionLiteral Composition { get; set; } = new CompositionLiteral();

	//}

	//public class CompositionLiteral
	//{
	//    public decimal Energie { get; set; } = -1;
	//    public decimal Eau { get; set; } = -1;
	//    public decimal Cendres { get; set; } = -1;
	//    public decimal Sel { get; set; } = -1;
	//    public decimal Sodium { get; set; } = -1;
	//    public decimal Magnesium { get; set; } = -1;
	//    public decimal Phosphore { get; set; } = -1;
	//    public decimal Chlorure { get; set; } = -1;
	//    public decimal Potassium { get; set; } = -1;
	//    public decimal Calcium { get; set; } = -1;
	//    public decimal Manganese { get; set; } = -1;
	//    public decimal Fer { get; set; } = -1;
	//    public decimal Cuivre { get; set; } = -1;
	//    public decimal Zinc { get; set; } = -1;
	//    public decimal Selenium { get; set; } = -1;
	//    public decimal Iode { get; set; } = -1;
	//    public decimal Proteines { get; set; } = -1;
	//    public decimal Glucides { get; set; } = -1;
	//    public decimal Sucres { get; set; } = -1;
	//    public decimal Amidon { get; set; } = -1;
	//    public decimal Polyols { get; set; } = -1;
	//    public decimal Fibres { get; set; } = -1;
	//    public decimal Lipides { get; set; } = -1;
	//    public decimal AcideGras { get; set; } = -1;
	//    public decimal Retinol { get; set; } = -1;
	//    public decimal BetaCarotene { get; set; } = -1;
	//    public decimal VitamineD { get; set; } = -1;
	//    public decimal VitamineE { get; set; } = -1;
	//    public decimal VitamineK1 { get; set; } = -1;
	//    public decimal VitamineK2 { get; set; } = -1;
	//    public decimal VitamineC { get; set; } = -1;
	//    public decimal VitamineB1 { get; set; } = -1;
	//    public decimal VitamineB2 { get; set; } = -1;
	//    public decimal VitamineB3 { get; set; } = -1;
	//    public decimal VitamineB5 { get; set; } = -1;
	//    public decimal VitamineB6 { get; set; } = -1;
	//    public decimal VitamineB12 { get; set; } = -1;
	//    public decimal Alcool { get; set; } = -1;
	//    public decimal AcidesOrganiques { get; set; } = -1;
	//    public decimal Cholesterol { get; set; } = -1;

	//}


	//public class PatientLiteral
	//{
	//    public List<RestrictionLiteral> Restrictions { get; set; } = new List<RestrictionLiteral>();
	//}


	//public class RestrictionLiteral
	//{

	//    public CompositionLiteral Min { get; set; } = new CompositionLiteral();

	//    public CompositionLiteral Max { get; set; } = new CompositionLiteral();

	//}
}