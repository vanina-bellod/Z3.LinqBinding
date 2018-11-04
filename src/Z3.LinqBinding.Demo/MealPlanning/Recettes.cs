using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Z3.LinqBinding.Demo
{


    public class Recettes
    {

        public List<DenreeRecord> Denrees { get; set; } = new List<DenreeRecord>();

        public List<PlatRecord> Plats { get; set; } = new List<PlatRecord>();

        public List<MenuRecord> Menus { get; set; } = new List<MenuRecord>();


        public static Recettes Load(string folderPath)
        {
            var toReturn = new Recettes();

            var serializer = new Newtonsoft.Json.JsonSerializer();
	        var files = Directory.GetFiles(folderPath, "denrees*");
	        foreach (var fileName in files)
	        {
				using (var reader = File.OpenText(fileName))
				{
					using (var jsonReader = new JsonTextReader(reader))
					{
						toReturn.Denrees.AddRange(serializer.Deserialize<List<DenreeRecord>>(jsonReader));
					}

				}
			}

	        foreach (var denree in toReturn.Denrees)
	        {
		        if (string.IsNullOrEmpty(denree.fields.libelle_denree))
		        {
			        denree.fields.libelle_denree = denree.fields.libelle_recette;
		        }
	        }

			files = Directory.GetFiles(folderPath, "plats*");
	        foreach (var fileName in files)
	        {
				using (var reader = File.OpenText(fileName))
				{
					using (var jsonReader = new JsonTextReader(reader))
					{
						toReturn.Plats.AddRange(serializer.Deserialize<List<PlatRecord>>(jsonReader)); 
					}

				}
			}

			files = Directory.GetFiles(folderPath, "menus*");
	        foreach (var fileName in files)
	        {
				using (var reader = File.OpenText(fileName))
				{
					using (var jsonReader = new JsonTextReader(reader))
					{
						toReturn.Menus.AddRange(serializer.Deserialize<List<MenuRecord>>(jsonReader));
					}

				}
			}

            return toReturn;
        }




    }

    public class DenreeRecord
    {
        public string datasetid { get; set; }
        public string recordid { get; set; }
        public Fields fields { get; set; }
        public DateTime record_timestamp { get; set; }

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

    public class PlatRecord
    {
        public string datasetid { get; set; }
        public string recordid { get; set; }
        public Fields fields { get; set; }
        public DateTime record_timestamp { get; set; }

        public class Fields
        {
            public string code_recette { get; set; }
            public string libelle_recette { get; set; }
            public string code_plat { get; set; }
            public string libelle_plat { get; set; }
            public int numero { get; set; }
        }

    }


    public class MenuRecord
    {
        public string datasetid { get; set; }
        public string recordid { get; set; }
        public Fields fields { get; set; }
        public DateTime record_timestamp { get; set; }

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