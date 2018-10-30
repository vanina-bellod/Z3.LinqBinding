using System;
using System.Collections.Generic;
using System.Linq;
using Z3.LinqBinding;

namespace Z3.LinqBindingDemo
{
    public class PlanificateurDeMenus
    {

        public const int NbConstituants = 27;


        public int NombreMenus { get; set; } = 7;
        public int NombrePlatsParMenu { get; set; } = 7;

        


        public List<Menu> Menus { get; set; }


        public Theorem<PlanificateurDeMenus> Create(Z3Context context, List<Patient> patients, Dietetique dietetique)
        {
            var theorem = context.NewTheorem<PlanificateurDeMenus>();

            var indicesMenus = Enumerable.Range(0, NombreMenus);
            var indicesPlats = Enumerable.Range(0, NombrePlatsParMenu);
            // Pas deux fois le même plat
            theorem = theorem.Where(t => Z3Methods.Distinct(indicesMenus
                    .Select(m => indicesPlats
                        .Select(p => t.Menus[m].Plats[p].PlatId).ToArray())
                    .ToArray()));

            // Plats parmi la collection proposée
            for (int m = 0; m < NombreMenus; m++)
            {
                for (int p = 0; p < NombrePlatsParMenu; p++)
                {
                    // Les plats sont dans la collection
                    theorem = theorem.Where(t =>
                        t.Menus[m].Plats[p].PlatId >= 0 && t.Menus[m].Plats[p].PlatId < dietetique.Plats.Count);

                    // Les compositions sont celles des plats
                    for (int recetteId = 0; recetteId < dietetique.Plats.Count; recetteId++)
                    {
                        //for (int c = 0; c < dietetique.Plats[recettId].Compositions.Count; c++)
                        for (int c = 0; c < NbConstituants; c++)
                        {
                            theorem = theorem.Where(t => t.Menus[m].Plats[p].PlatId != recetteId
                                                         || t.Menus[m].Plats[p].Compositions[c] ==
                                                         dietetique.Plats[recetteId].Compositions[c]);
                        }

                    }

                }

                // La composition des menu est la somme de celle des plats
                for (int c = 0; c < NbConstituants; c++)
                {
                    theorem = theorem.Where(t => t.Menus[m].Compositions[c] == indicesPlats
                                                .Select(p=>t.Menus[m].Plats[p].Compositions[c]).Sum());
                }
                

                // Les menus respectent les restrictions des patients.
                for (int p = 0; p < patients.Count; p++)
                {
                    for (int c = 0; c < NbConstituants; c++)
                    {
                        var restriction = patients[p].Restrictions[c];
                        theorem = theorem.Where(t => (restriction.Min == -1 || t.Menus[m].Compositions[c]> restriction.Min)
                                                      && (restriction.Max == -1 || t.Menus[m].Compositions[c] < restriction.Max));
                    }
                }
            }

            return theorem;

        }


    }


    public class Patient
    {
        public Restriction[] Restrictions { get; set; } = new Restriction[PlanificateurDeMenus.NbConstituants];
    }


    public class Restriction
    {


        public Decimal Min { get; set; } = -1;

        public Decimal Max { get; set; } = -1;

    }









    public class Dietetique
    {

        public List<Menu> Menus { get; set; }

        public List<PlatImport> Plats { get; set; }

        public List<DenreeImport> Denrees { get; set; }

        public List<Constituant> Constituants { get; set; }


    }




    public class Menu
    {

        public List<Plat> Plats { get; set; } = new List<Plat>();


        public Decimal[] Compositions { get; set; } = new decimal[PlanificateurDeMenus.NbConstituants];

        //public List<Composition> Compositions { get; set; } = new List<Composition>();


    }


    public class Plat
    {

        public int PlatId { get; set; }


        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public Decimal[] Compositions { get; set; } = new decimal[PlanificateurDeMenus.NbConstituants];

        //public List<Composition> Compositions { get; set; } = new List<Composition>();

    }


    public class PlatImport
    {

        public string Nom { get; set; }

        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public Decimal[] Compositions { get; set; } = new decimal[PlanificateurDeMenus.NbConstituants];
        //public List<Composition> Compositions { get; set; } = new List<Composition>();

    }


    public class Ingredient
    {

        public int Denree { get; set; }

        public decimal Quantite { get; set; }

    }


    public class DenreeImport
    {

        public string Nom { get; set; } = "";


        public Decimal[] Compositions { get; set; } = new decimal[PlanificateurDeMenus.NbConstituants];
        //public List<Composition> Compositions { get; set; } = new List<Composition>();
    }




    public class Constituant
    {

        public string Nom { get; set; }

    }



    //public class Composition
    //{

    //    public int Constituant { get; set; }

    //    public Decimal Teneur { get; set; }

    //}










    //public class Menu3
    //{

    //    public List<Plat3> Plats { get; set; }

    //    public Decimal GetTeneur(string constituant)
    //    {
    //        return Plats.Select(
    //            p => p.Ingredients.Select(
    //                i => i.Quantite * i.Denree.Compositions
    //                         .Where(c => c.Constituant.Nom == constituant)
    //                         .Select(c => c.Teneur).Sum()
    //            ).Sum()
    //        ).Sum();
    //    }

    //}

    //public class Plat3
    //{

    //    public string Nom { get; set; }

    //    public List<Ingredient3> Ingredients { get; set; } = new List<Ingredient3>();

    //}

    //public class Ingredient3
    //{

    //    public Denree3 Denree { get; set; }

    //    public int Quantite { get; set; }

    //}



    //public class Denree3
    //{

    //    public string Nom { get; set; } = "";


    //    public List<Composition3> Compositions { get; set; } = new List<Composition3>();

    //}


    //public class Composition3
    //{

    //    public Constituant Constituant { get; set; }

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