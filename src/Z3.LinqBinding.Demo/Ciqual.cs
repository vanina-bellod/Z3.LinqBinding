using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Z3.LinqBindingDemo
{
    public class Ciqual
    {
        public TableConstituant Constituants { get; set; }

        public TableGroupesAliments GroupesAliments { get; set; }

        public TableAliments Aliments { get; set; }

        public TableComposition Compositions { get; set; }

        public static Ciqual Load(string folderPath)
        {
            var toReturn = new Ciqual();

            var fileName = Path.Combine(folderPath, "const_2017 11 21.xml");
            using (var reader = File.OpenText(fileName))
            {
                toReturn.Constituants = (TableConstituant)new XmlSerializer(typeof(TableConstituant)).Deserialize(reader);
            }
            fileName = Path.Combine(folderPath, "alim_grp_2017 11 21.xml");
            using (var reader = File.OpenText(fileName))
            {
                toReturn.GroupesAliments = (TableGroupesAliments)new XmlSerializer(typeof(TableGroupesAliments)).Deserialize(reader);
            }
            fileName = Path.Combine(folderPath, "alim_2017 11 21.xml");
            using (var reader = File.OpenText(fileName))
            {
                toReturn.Aliments = (TableAliments)new XmlSerializer(typeof(TableAliments)).Deserialize(reader);
            }
            fileName = Path.Combine(folderPath, "compo_2017 11 21.xml");
            using (var reader = File.OpenText(fileName))
            {
                toReturn.Compositions = (TableComposition)new XmlSerializer(typeof(TableComposition)).Deserialize(reader);
            }


            return toReturn;
        }


    }


    // REMARQUE : Le code généré peut nécessiter au moins .NET Framework 4.5 ou .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "TABLE")]
    public partial class TableConstituant
    {

        private TABLECONST[] cONSTField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CONST")]
        public TABLECONST[] CONST
        {
            get
            {
                return this.cONSTField;
            }
            set
            {
                this.cONSTField = value;
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class TABLECONST
        {

            private uint const_codeField;

            private string const_nom_frField;

            private string const_nom_engField;

            /// <remarks/>
            public uint const_code
            {
                get
                {
                    return this.const_codeField;
                }
                set
                {
                    this.const_codeField = value;
                }
            }

            /// <remarks/>
            public string const_nom_fr
            {
                get
                {
                    return this.const_nom_frField;
                }
                set
                {
                    this.const_nom_frField = value;
                }
            }

            /// <remarks/>
            public string const_nom_eng
            {
                get
                {
                    return this.const_nom_engField;
                }
                set
                {
                    this.const_nom_engField = value;
                }
            }
        }

    }


    // REMARQUE : Le code généré peut nécessiter au moins .NET Framework 4.5 ou .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "TABLE")]
    public partial class TableGroupesAliments
    {

        private TABLEALIM_GRP[] aLIM_GRPField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ALIM_GRP")]
        public TABLEALIM_GRP[] ALIM_GRP
        {
            get
            {
                return this.aLIM_GRPField;
            }
            set
            {
                this.aLIM_GRPField = value;
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class TABLEALIM_GRP
        {

            private byte alim_grp_codeField;

            private string alim_grp_nom_frField;

            private string alim_grp_nom_engField;

            private ushort alim_ssgrp_codeField;

            private string alim_ssgrp_nom_frField;

            private string alim_ssgrp_nom_engField;

            private uint alim_ssssgrp_codeField;

            private string alim_ssssgrp_nom_frField;

            private string alim_ssssgrp_nom_engField;

            /// <remarks/>
            public byte alim_grp_code
            {
                get
                {
                    return this.alim_grp_codeField;
                }
                set
                {
                    this.alim_grp_codeField = value;
                }
            }

            /// <remarks/>
            public string alim_grp_nom_fr
            {
                get
                {
                    return this.alim_grp_nom_frField;
                }
                set
                {
                    this.alim_grp_nom_frField = value;
                }
            }

            /// <remarks/>
            public string alim_grp_nom_eng
            {
                get
                {
                    return this.alim_grp_nom_engField;
                }
                set
                {
                    this.alim_grp_nom_engField = value;
                }
            }

            /// <remarks/>
            public ushort alim_ssgrp_code
            {
                get
                {
                    return this.alim_ssgrp_codeField;
                }
                set
                {
                    this.alim_ssgrp_codeField = value;
                }
            }

            /// <remarks/>
            public string alim_ssgrp_nom_fr
            {
                get
                {
                    return this.alim_ssgrp_nom_frField;
                }
                set
                {
                    this.alim_ssgrp_nom_frField = value;
                }
            }

            /// <remarks/>
            public string alim_ssgrp_nom_eng
            {
                get
                {
                    return this.alim_ssgrp_nom_engField;
                }
                set
                {
                    this.alim_ssgrp_nom_engField = value;
                }
            }

            /// <remarks/>
            public uint alim_ssssgrp_code
            {
                get
                {
                    return this.alim_ssssgrp_codeField;
                }
                set
                {
                    this.alim_ssssgrp_codeField = value;
                }
            }

            /// <remarks/>
            public string alim_ssssgrp_nom_fr
            {
                get
                {
                    return this.alim_ssssgrp_nom_frField;
                }
                set
                {
                    this.alim_ssssgrp_nom_frField = value;
                }
            }

            /// <remarks/>
            public string alim_ssssgrp_nom_eng
            {
                get
                {
                    return this.alim_ssssgrp_nom_engField;
                }
                set
                {
                    this.alim_ssssgrp_nom_engField = value;
                }
            }
        }

    }



    // REMARQUE : Le code généré peut nécessiter au moins .NET Framework 4.5 ou .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "TABLE")]
    public partial class TableAliments
    {

        private TABLEALIM[] aLIMField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ALIM")]
        public TABLEALIM[] ALIM
        {
            get
            {
                return this.aLIMField;
            }
            set
            {
                this.aLIMField = value;
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class TABLEALIM
        {

            private uint alim_codeField;

            private string alim_nom_frField;

            private string alim_nom_index_frField;

            private string alim_nom_engField;

            private string alim_nom_index_engField;

            private byte alim_grp_codeField;

            private ushort alim_ssgrp_codeField;

            private uint alim_ssssgrp_codeField;

            /// <remarks/>
            public uint alim_code
            {
                get
                {
                    return this.alim_codeField;
                }
                set
                {
                    this.alim_codeField = value;
                }
            }

            /// <remarks/>
            public string alim_nom_fr
            {
                get
                {
                    return this.alim_nom_frField;
                }
                set
                {
                    this.alim_nom_frField = value;
                }
            }

            /// <remarks/>
            public string alim_nom_index_fr
            {
                get
                {
                    return this.alim_nom_index_frField;
                }
                set
                {
                    this.alim_nom_index_frField = value;
                }
            }

            /// <remarks/>
            public string alim_nom_eng
            {
                get
                {
                    return this.alim_nom_engField;
                }
                set
                {
                    this.alim_nom_engField = value;
                }
            }

            /// <remarks/>
            public string alim_nom_index_eng
            {
                get
                {
                    return this.alim_nom_index_engField;
                }
                set
                {
                    this.alim_nom_index_engField = value;
                }
            }

            /// <remarks/>
            public byte alim_grp_code
            {
                get
                {
                    return this.alim_grp_codeField;
                }
                set
                {
                    this.alim_grp_codeField = value;
                }
            }

            /// <remarks/>
            public ushort alim_ssgrp_code
            {
                get
                {
                    return this.alim_ssgrp_codeField;
                }
                set
                {
                    this.alim_ssgrp_codeField = value;
                }
            }

            /// <remarks/>
            public uint alim_ssssgrp_code
            {
                get
                {
                    return this.alim_ssssgrp_codeField;
                }
                set
                {
                    this.alim_ssssgrp_codeField = value;
                }
            }
        }




    }


    // REMARQUE : Le code généré peut nécessiter au moins .NET Framework 4.5 ou .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false, ElementName = "TABLE")]
    public partial class TableComposition
    {

        private TABLECOMPO[] cOMPOField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("COMPO")]
        public TABLECOMPO[] COMPO
        {
            get
            {
                return this.cOMPOField;
            }
            set
            {
                this.cOMPOField = value;
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class TABLECOMPO
        {

            private uint alim_codeField;

            private uint const_codeField;

            private string teneurField;

            private TABLECOMPOMin minField;

            private TABLECOMPOMax maxField;

            private TABLECOMPOCode_confiance code_confianceField;

            private TABLECOMPOSource_code source_codeField;

            /// <remarks/>
            public uint alim_code
            {
                get
                {
                    return this.alim_codeField;
                }
                set
                {
                    this.alim_codeField = value;
                }
            }

            /// <remarks/>
            public uint const_code
            {
                get
                {
                    return this.const_codeField;
                }
                set
                {
                    this.const_codeField = value;
                }
            }

            /// <remarks/>
            public string teneur
            {
                get
                {
                    return this.teneurField;
                }
                set
                {
                    this.teneurField = value;
                }
            }

            /// <remarks/>
            public TABLECOMPOMin min
            {
                get
                {
                    return this.minField;
                }
                set
                {
                    this.minField = value;
                }
            }

            /// <remarks/>
            public TABLECOMPOMax max
            {
                get
                {
                    return this.maxField;
                }
                set
                {
                    this.maxField = value;
                }
            }

            /// <remarks/>
            public TABLECOMPOCode_confiance code_confiance
            {
                get
                {
                    return this.code_confianceField;
                }
                set
                {
                    this.code_confianceField = value;
                }
            }

            /// <remarks/>
            public TABLECOMPOSource_code source_code
            {
                get
                {
                    return this.source_codeField;
                }
                set
                {
                    this.source_codeField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class TABLECOMPOMin
        {

            private string missingField;

            private string valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string missing
            {
                get
                {
                    return this.missingField;
                }
                set
                {
                    this.missingField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlTextAttribute()]
            public string Value
            {
                get
                {
                    return this.valueField;
                }
                set
                {
                    this.valueField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class TABLECOMPOMax
        {

            private string missingField;

            private string valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string missing
            {
                get
                {
                    return this.missingField;
                }
                set
                {
                    this.missingField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlTextAttribute()]
            public string Value
            {
                get
                {
                    return this.valueField;
                }
                set
                {
                    this.valueField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class TABLECOMPOCode_confiance
        {

            private string missingField;

            private string valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string missing
            {
                get
                {
                    return this.missingField;
                }
                set
                {
                    this.missingField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlTextAttribute()]
            public string Value
            {
                get
                {
                    return this.valueField;
                }
                set
                {
                    this.valueField = value;
                }
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class TABLECOMPOSource_code
        {

            private string missingField;

            private string valueField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string missing
            {
                get
                {
                    return this.missingField;
                }
                set
                {
                    this.missingField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlTextAttribute()]
            public string Value
            {
                get
                {
                    return this.valueField;
                }
                set
                {
                    this.valueField = value;
                }
            }
        }
    }









}