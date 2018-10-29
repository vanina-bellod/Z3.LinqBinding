using System;
using System.Collections.Generic;
using System.Linq;

namespace Z3.LinqBindingDemo
{
    public class HospitalMealPlanner
    {
        public List<MenuLiteral> Menus { get; set; }
    }







    public class Plat
    {

        public int PlatId { get; set; }


        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    }

    public class Ingredient
    {

        public int DenreeId { get; set; }

        public decimal Quantite { get; set; }

    }





    public class DietetiqueImport
    {

        public List<MenuImport> Menus { get; set; }

        public List<PlatImport> Plats { get; set; }

        public List<DenreeImport> Denrees { get; set; }

        public List<Constituant> Constituants { get; set; }


    }




    public class MenuImport
    {

        public List<int> Plats { get; set; }


    }

    public class PlatImport
    {

        public string Nom { get; set; }

        public List<IngredientImport> Ingredients { get; set; } = new List<IngredientImport>();

    }

    public class IngredientImport
    {

        public int Denree { get; set; }

        public decimal Quantite { get; set; }

    }



    public class DenreeImport
    {

        public string Nom { get; set; } = "";


        public List<CompositionImport> Compositions { get; set; } = new List<CompositionImport>();

    }


    public class CompositionImport
    {

        public int Constituant { get; set; }

        public Decimal Teneur { get; set; }

    }


    public class Constituant
    {

        public string Nom { get; set; }

    }



    public class Menu3
    {

        public List<Plat3> Plats { get; set; }

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

    public class Plat3
    {

        public string Nom { get; set; }

        public List<Ingredient3> Ingredients { get; set; } = new List<Ingredient3>();

    }

    public class Ingredient3
    {

        public Denree3 Denree { get; set; }

        public int Quantite { get; set; }

    }



    public class Denree3
    {

        public string Nom { get; set; } = "";


        public List<Composition3> Compositions { get; set; } = new List<Composition3>();

    }


    public class Composition3
    {

        public Constituant Constituant { get; set; }

        public Decimal Teneur { get; set; }

    }

    public class Restriction3
    {

        public string Constituant { get; set; }

        public decimal Min { get; set; }

        public decimal Max { get; set; }

    }




    public class MenuSemaine
    {

        public MenuLiteral Lundi { get; set; }
        public MenuLiteral Mardi { get; set; }
        public MenuLiteral Mercredi { get; set; }
        public MenuLiteral Jeudi { get; set; }
        public MenuLiteral Vendredi { get; set; }
        public MenuLiteral Samedi { get; set; }
        public MenuLiteral Dimanche { get; set; }

    }


    public class MenuLiteral
    {

        public Plat Entree { get; set; }
        public Plat Viande { get; set; }
        public Plat Legumes { get; set; }
        public Plat Laitage { get; set; }
        public Plat Dessert { get; set; }
        public Plat Pain { get; set; }
        public Plat Gouter { get; set; }

    }


    public class DenreeLiteral
    {

        public string Nom { get; set; } = "";

        public CompositionLiteral Composition { get; set; } = new CompositionLiteral();

    }

    public class CompositionLiteral
    {
        public decimal Energie { get; set; } = -1;
        public decimal Eau { get; set; } = -1;
        public decimal Cendres { get; set; } = -1;
        public decimal Sel { get; set; } = -1;
        public decimal Sodium { get; set; } = -1;
        public decimal Magnesium { get; set; } = -1;
        public decimal Phosphore { get; set; } = -1;
        public decimal Chlorure { get; set; } = -1;
        public decimal Potassium { get; set; } = -1;
        public decimal Calcium { get; set; } = -1;
        public decimal Manganese { get; set; } = -1;
        public decimal Fer { get; set; } = -1;
        public decimal Cuivre { get; set; } = -1;
        public decimal Zinc { get; set; } = -1;
        public decimal Selenium { get; set; } = -1;
        public decimal Iode { get; set; } = -1;
        public decimal Proteines { get; set; } = -1;
        public decimal Glucides { get; set; } = -1;
        public decimal Sucres { get; set; } = -1;
        public decimal Amidon { get; set; } = -1;
        public decimal Polyols { get; set; } = -1;
        public decimal Fibres { get; set; } = -1;
        public decimal Lipides { get; set; } = -1;
        public decimal AcideGras { get; set; } = -1;
        public decimal Retinol { get; set; } = -1;
        public decimal BetaCarotene { get; set; } = -1;
        public decimal VitamineD { get; set; } = -1;
        public decimal VitamineE { get; set; } = -1;
        public decimal VitamineK1 { get; set; } = -1;
        public decimal VitamineK2 { get; set; } = -1;
        public decimal VitamineC { get; set; } = -1;
        public decimal VitamineB1 { get; set; } = -1;
        public decimal VitamineB2 { get; set; } = -1;
        public decimal VitamineB3 { get; set; } = -1;
        public decimal VitamineB5 { get; set; } = -1;
        public decimal VitamineB6 { get; set; } = -1;
        public decimal VitamineB12 { get; set; } = -1;
        public decimal Alcool { get; set; } = -1;
        public decimal AcidesOrganiques { get; set; } = -1;
        public decimal Cholesterol { get; set; } = -1;

    }


    public class PatientLiteral
    {
        public List<RestrictionLiteral> Restrictions { get; set; } = new List<RestrictionLiteral>();
    }


    public class RestrictionLiteral
    {

        public CompositionLiteral Min { get; set; } = new CompositionLiteral();

        public CompositionLiteral Max { get; set; } = new CompositionLiteral();

    }
}