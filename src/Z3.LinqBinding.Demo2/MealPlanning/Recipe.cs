using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Z3.LinqBinding.Demo
{


    public class Recipe
    {

	    public recipeml Recettes { get; set; }


        public static Recipe Load(string folderPath)
        {
            var toReturn = new Recipe();

            
	        var files = Directory.GetFiles(folderPath, "*.xml", SearchOption.AllDirectories);
	        foreach (var fileName in files)
	        {
		        using (var reader = File.OpenText(fileName))
		        {
			        toReturn.Recettes = (recipeml)new XmlSerializer(typeof(recipeml)).Deserialize(reader);
		        }
			}


            return toReturn;
        }



		// REMARQUE : Le code généré peut nécessiter au moins .NET Framework 4.5 ou .NET Core/Standard 2.0.
		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
		public partial class recipeml
		{

			private recipemlRecipe recipeField;

			private decimal versionField;

			/// <remarks/>
			public recipemlRecipe recipe
			{
				get
				{
					return this.recipeField;
				}
				set
				{
					this.recipeField = value;
				}
			}

			/// <remarks/>
			[System.Xml.Serialization.XmlAttributeAttribute()]
			public decimal version
			{
				get
				{
					return this.versionField;
				}
				set
				{
					this.versionField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class recipemlRecipe
		{

			private recipemlRecipeHead headField;

			private recipemlRecipeIng[] ingredientsField;

			private recipemlRecipeDirections directionsField;

			/// <remarks/>
			public recipemlRecipeHead head
			{
				get
				{
					return this.headField;
				}
				set
				{
					this.headField = value;
				}
			}

			/// <remarks/>
			[System.Xml.Serialization.XmlArrayItemAttribute("ing", IsNullable = false)]
			public recipemlRecipeIng[] ingredients
			{
				get
				{
					return this.ingredientsField;
				}
				set
				{
					this.ingredientsField = value;
				}
			}

			/// <remarks/>
			public recipemlRecipeDirections directions
			{
				get
				{
					return this.directionsField;
				}
				set
				{
					this.directionsField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class recipemlRecipeHead
		{

			private string titleField;

			private recipemlRecipeHeadCategories categoriesField;

			private byte yieldField;

			/// <remarks/>
			public string title
			{
				get
				{
					return this.titleField;
				}
				set
				{
					this.titleField = value;
				}
			}

			/// <remarks/>
			public recipemlRecipeHeadCategories categories
			{
				get
				{
					return this.categoriesField;
				}
				set
				{
					this.categoriesField = value;
				}
			}

			/// <remarks/>
			public byte yield
			{
				get
				{
					return this.yieldField;
				}
				set
				{
					this.yieldField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class recipemlRecipeHeadCategories
		{

			private string catField;

			/// <remarks/>
			public string cat
			{
				get
				{
					return this.catField;
				}
				set
				{
					this.catField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class recipemlRecipeIng
		{

			private recipemlRecipeIngAmt amtField;

			private string itemField;

			/// <remarks/>
			public recipemlRecipeIngAmt amt
			{
				get
				{
					return this.amtField;
				}
				set
				{
					this.amtField = value;
				}
			}

			/// <remarks/>
			public string item
			{
				get
				{
					return this.itemField;
				}
				set
				{
					this.itemField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class recipemlRecipeIngAmt
		{

			private string qtyField;

			private string unitField;

			/// <remarks/>
			public string qty
			{
				get
				{
					return this.qtyField;
				}
				set
				{
					this.qtyField = value;
				}
			}

			/// <remarks/>
			public string unit
			{
				get
				{
					return this.unitField;
				}
				set
				{
					this.unitField = value;
				}
			}
		}

		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute("code")]
		[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
		public partial class recipemlRecipeDirections
		{

			private string stepField;

			/// <remarks/>
			public string step
			{
				get
				{
					return this.stepField;
				}
				set
				{
					this.stepField = value;
				}
			}
		}



	}




}