using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Z3.LinqBindingDemo
{


    public class Recettes
    {

        public DenreesTable Denrees { get; set; }

        public PlatsTable Plats { get; set; }

        public MenusTable Menus { get; set; }


        public static Recettes Load(string folderPath)
        {
            var toReturn = new Recettes();

            var serializer = new Newtonsoft.Json.JsonSerializer();
            var fileName = Path.Combine(folderPath, "denrees.json");
            using (var reader = File.OpenText(fileName))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    toReturn.Denrees = serializer.Deserialize<List<DenreesTable>>(jsonReader).First();
                }
                
            }
            fileName = Path.Combine(folderPath, "plats.json");
            using (var reader = File.OpenText(fileName))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    toReturn.Plats = serializer.Deserialize<List<PlatsTable>>(jsonReader).First(); ;
                }
                
            }
            fileName = Path.Combine(folderPath, "menus.json");
            using (var reader = File.OpenText(fileName))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    toReturn.Menus = serializer.Deserialize<List<MenusTable>>(jsonReader).First(); 
                }
                
            }


            return toReturn;
        }




    }



    public class DenreesTable
    {
        public DenreeRecord[] Property1 { get; set; }

        public class DenreeRecord
        {
            public string datasetid { get; set; }
            public string recordid { get; set; }
            public Fields fields { get; set; }
            public DateTime record_timestamp { get; set; }
        }

        public class Fields
        {
            public string libelle_recette { get; set; }
            public string libellelong_denree { get; set; }
            public string code_denree { get; set; }
            public string code_recette { get; set; }
            public string libelle_denree { get; set; }
            public string lait { get; set; }
            public string celeri { get; set; }
            public string ovoproduit { get; set; }
            public string gluten { get; set; }
            public string fruits_a_coque { get; set; }
            public string poisson { get; set; }
            public string moutarde { get; set; }
            public string soja { get; set; }
            public string sulfites { get; set; }
        }
    }



    public class PlatsTable
    {
        public PlatRecord[] Property1 { get; set; }

        public class PlatRecord
        {
            public string datasetid { get; set; }
            public string recordid { get; set; }
            public Fields fields { get; set; }
            public DateTime record_timestamp { get; set; }
        }

        public class Fields
        {
            public string code_recette { get; set; }
            public string libelle_recette { get; set; }
            public string code_plat { get; set; }
            public string libelle_plat { get; set; }
            public int numero { get; set; }
        }
    }

   




    public class MenusTable
    {
        public MenuRecord[] Property1 { get; set; }


        public class MenuRecord
        {
            public string datasetid { get; set; }
            public string recordid { get; set; }
            public Fields fields { get; set; }
            public DateTime record_timestamp { get; set; }
        }

        public class Fields
        {
            public string libelle_repas { get; set; }
            public string code_plat { get; set; }
            public string libelle_population_cible { get; set; }
            public DateTime datemenu { get; set; }
            public int ordre_plat { get; set; }
            public string libelle_ordre_plat { get; set; }
            public string libelle_plat { get; set; }
        }
    }




}