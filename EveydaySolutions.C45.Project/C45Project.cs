using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EverydaySolutions.C45
{
    #region C45ProjectType
    public enum C45ProjectType : int
	{
		General,
		HRT		//Header-region-title
    }
    #endregion

    #region C45Project
    sealed public class C45Project
	{
		XmlDocument doc = new XmlDocument ();

		#region C45Project
		private C45Project (String filename)
		{
            this.filename = filename;
            this.doc.Load (filename);
		}
		#endregion

		#region CreateInstance
		static public C45Project CreateInstance (String filename)
		{
			try
			{
				C45Project instance = new C45Project (filename);
                instance.C45ConfigFile = "C45Interop.config";
				instance.columnWidth = new ColumnWidthCollection (instance.doc);
				instance.columnWidth.DirtyStateChanged += new DirtyStateChangedEventHandler (ColumnWidth_DirtyStateChanged);
				
				return instance;
			}
			catch (Exception ex)
			{
				throw (new Exception ("An error occured while parsing C45-Project file", 
					ex));
			}
		}
		#endregion

        #region Filename
        private String filename = String.Empty;
        public String Filename
        {
            get
            {
                return filename;
            }
        }
        #endregion

        #region ColumnWidth_DirtyStateChanged
        static void ColumnWidth_DirtyStateChanged (
			object sender, 
			EventArgs e)
		{
			isDirty = true;
		}
		#endregion

		#region Save
		public void Save ()
		{
			this.doc.Save (this.filename);
            isDirty = false;
		}
		#endregion

		#region ProjectName
		public String ProjectName
		{
			get
			{
				return this.doc.SelectSingleNode (@"//c45project/project_name").InnerText;
			}
			set
			{
                isDirty = true;
				this.doc.SelectSingleNode (@"//c45project/project_name").InnerText = value;
			}
		}
		#endregion

		#region SchemaFile
		public String SchemaFile
		{
			get
			{
				return this.doc.SelectSingleNode (@"//c45project/attributes/schema_file").InnerText;
			}
			set
			{
                isDirty = true;
				this.doc.SelectSingleNode (@"//c45project/attributes/schema_file").InnerText = value;
			}
		}
		#endregion

        public String C45ConfigFile
        {
            get
            {
                return this.doc.SelectSingleNode (@"//c45project/c45_config_file").InnerText;
            }
            set
            {
                isDirty = true;
                this.doc.SelectSingleNode (@"//c45project/c45_config_file").InnerText = value;
            }
        }

		#region ProjectType
		public C45ProjectType ProjectType
		{
			get
			{
				object projectType = Enum.Parse (
					typeof (C45ProjectType), 
					this.doc.SelectSingleNode (@"//c45project/attributes/type").InnerText);

				return (C45ProjectType) projectType;
			}
			set
			{
                isDirty = true;
				doc.SelectSingleNode (@"//c45project/attributes/type").InnerText = value.ToString ();
			}
		}
		#endregion

		#region IsDirty
		static private Boolean isDirty;
		static public Boolean IsDirty
		{
			get
			{
				return isDirty;
			}
		}
		#endregion

		#region ColumnWidth
		private ColumnWidthCollection columnWidth = null;
		public ColumnWidthCollection ColumnWidth
		{
			get
			{
				return columnWidth;
			}
		}
		#endregion
    }
    #endregion
}
