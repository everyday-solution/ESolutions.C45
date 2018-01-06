using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.InteropServices;
using EverydaySolutions.C45.C45Interop;

namespace EverydaySolutions.C45
{
	#region C45Settings
	/// <summary>
	/// C45Settings class keeps the C45.DLL configuration
	/// </summary>
	[Serializable]
	public class C45Settings
    {
        #region C45Settings
        private C45Settings ()
        {
        }
        #endregion

        #region Instance
        /// <summary>
		/// Object factory, create an static instance of c45 settings
		/// </summary>
		public static C45Settings CreateFromFile (String configFile)
		{
            C45Settings newInstance = new C45Settings ();
			FileStream fs = new FileStream (
                configFile, 
                FileMode.OpenOrCreate);
			XmlSerializer serializer = new XmlSerializer (typeof (C45Settings));

			try
			{
                newInstance = (C45Settings)serializer.Deserialize (fs);
				fs.Close ();
			}
			catch (Exception ex)
			{
				//Save settings with initial default values
				System.Diagnostics.Trace.Write (C45Exceptions.CONFIG_FILE_NOT_FOUND);
				fs.Close ();
                newInstance.Save ();
			}

            return newInstance;
		}
		#endregion

		#region C45DebugParameter
		/// <summary>
		/// 
		/// </summary>
		[Browsable (true), Description ("C45-debugging parameter")]
		private C45DebugParameter debugParameter = new C45DebugParameter ();
		public C45DebugParameter DebugParameter
		{
			get
			{
				return debugParameter;
			}
			set
			{
				debugParameter = value;
			}
		}
		#endregion

		#region ConfigFileName
		/// <summary>
		/// Name of the file where the C45 configuration is stored
		/// </summary>
		[Browsable (false), ReadOnly (true), Description ("Name of the file where the C45 configuration is stored")]
		private String configFileName = "C45Interop.config";
		public String ConfigFileName 
		{
			get
			{
				return configFileName;
			}
		}
		#endregion

		#region C45SchemaFilename
		/// <summary>
		/// c45shema c45SchemaFilename, whithout extension
		/// </summary>
		[Browsable (false)]
		private String c45SchemaFilename = String.Empty;
		public String C45SchemaFilename
		{
			get
			{
				return c45SchemaFilename;
			}
			set
			{
				c45SchemaFilename = value;
			}
		}
		#endregion

		#region GainRatio
		/// <summary>
		/// If true, information gain ratio is used, otherwise the absolute information gain is used
		/// </summary>
		[Browsable (true), ReadOnly (false), Description ("If true, information gain ratio is used, otherwise the absolute information gain is used")]
		private Boolean gainRatio = true;
		public Boolean GainRatio
		{
			get
			{
				return gainRatio;
			}
			set
			{
				gainRatio = value;
			}
		}
		#endregion

		#region SubtreeRaising
		/// <summary>
		/// Subtree raising make the tree more narrow (faster), but less certain
		/// </summary>
		[Browsable (true), ReadOnly (false), Description ("Subtree raising make the tree more narrow (faster), but less certain")]
		private Boolean subtreeRaising = false;
		public Boolean SubtreeRaising
		{
			get
			{
				return subtreeRaising;
			}
			set
			{
				subtreeRaising = value;
			}
		}
		#endregion

		#region SoftTresh
		/// <summary>
		/// Use soften threshold
		/// </summary>
		[Browsable (true), ReadOnly (false), Description ("Use soften threshold")]
		private Boolean softTresh = true;
		public Boolean SoftTresh
		{
			get
			{
				return softTresh;
			}
			set
			{
				softTresh = value;
			}
		}
		#endregion

		#region ConfidenceLevel
		/// <summary>
		/// Confidence level for tree pruning, default 25 (%)
		/// </summary>
		[Browsable (true), ReadOnly (false), Description ("Confidence level for tree pruning, default 25 (%)")]
		private int confidenceLevel = 25;
		public int ConfidenceLevel
		{
			get
			{
				return confidenceLevel;
			}
			set
			{
				confidenceLevel = value;
			}
		}
		#endregion

		#region Save
		/// <summary>
		/// Save the configuration to file.
		/// </summary>
		public void Save ()
		{
            try
            {
                File.Delete (this.ConfigFileName);
                FileStream fs = new FileStream (this.ConfigFileName, FileMode.OpenOrCreate);
                XmlSerializer serializer = new XmlSerializer (typeof (C45Settings));
                serializer.Serialize (fs, this);
                fs.Close ();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Write (C45Exceptions.ERROR_SAVING_XML_FILE);
            }
		}
		#endregion
	}
	#endregion
}
