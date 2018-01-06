using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml;
using EverydaySolutions.C45.C45Interop;

namespace EverydaySolutions.C45
{
	#region Delegates
	public delegate void DecisionTreeGenerationStartedEventHandler (
		object sender,
		EventArgs e);
	public delegate void DecisionTreeGenerationFinishedEventHandler (
		object sender,
		EventArgs e);
	public delegate void ConsultationStartedEventHandler (
		object sender,
		EventArgs e);
	public delegate void ConsultationFinishedEventHandler (
		object sender,
		EventArgs e);
	public delegate void SingleFeatureClassifiedEventHandler (
		object sender,
		C45SingleFeatureSetClassifiedEventArgs e);
	#endregion

	#region C45Controller
	internal class C45Controller
	{
		#region events
		public event DecisionTreeGenerationStartedEventHandler DecisionTreeGenerationStarted;
		public event DecisionTreeGenerationFinishedEventHandler DecisionTreeGenerationFinished;
		public event ConsultationStartedEventHandler ConsultationStarted;
		public event ConsultationFinishedEventHandler ConsultationFinished;
		#endregion

		#region OnDecisionTreeGenerationStarted
		protected void OnDecisionTreeGenerationStarted (
			object sender,
			EventArgs e)
		{
			if (DecisionTreeGenerationStarted != null)
			{
				DecisionTreeGenerationStarted (
					sender, 
					e);
			}
		}
		#endregion

		#region OnDecisionTreeGenerationFinished
		protected void OnDecisionTreeGenerationFinished (
			object sender,
			EventArgs e)
		{
			if (DecisionTreeGenerationFinished != null)
			{
				DecisionTreeGenerationFinished (
					sender,
					e);
			}
		}
		#endregion

		#region OnConsultationStarted
		protected void OnConsultationStarted (
			object sender,
			EventArgs e)
		{
			if (ConsultationStarted != null)
			{
				ConsultationStarted (
					sender, 
					e);
			}
		}
		#endregion

		#region OnConsultationFinished
		protected void OnConsultationFinished (
			object sender,
			EventArgs e)
		{
			if (ConsultationFinished != null)
			{
				ConsultationFinished (
					sender,
					e);
			}
		}
		#endregion

		#region Settings
        private C45Settings settings = null;
		public C45Settings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = value;
			}
		}
		#endregion

		#region IsTreeActive
		private bool isTreeActive = false;
		public bool IsTreeActive
		{
			get
			{
				return isTreeActive;
			}
			set
			{
				isTreeActive = value;
			}
		}
		#endregion	

		#region GenerateDTree
		/// <summary>
		/// Generates Descision Tree to file
		/// </summary>
		/// <remarks></remarks>
		/// <param name="pszFilename">C45SchemaFilename (filestem) of xyz.name and xyz.data</param>
		/// <param name="bGainRatio">If true, information gain ratio is used, otherwise the absolute information gain is used</param>
		/// <param name="bSubtreeRaising">Subtree raising make the tree more narrow (faster), but less certain</param>
		/// <param name="bProbtresh">Use soften threshold</param>
		/// <param name="iConfidenceLevel">Confidence level for tree pruning, default 25 (%)</param>
		/// <returns>C45 Returncode</returns>
		[DllImport ("C45.dll", EntryPoint = "GenerateDTree", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
		private extern static System.Int32 GenerateDTree (String	filename, 
			Boolean	gainRatio, 
			Boolean	subtreeRaising, 
			Boolean	softTresh, 
			Int32		confidenceLevel,
		_C45DebugParameter debugParameter);
		#endregion

		#region Classification
		/// <summary>
		/// Tries to classify c45feature data
		/// </summary>
		/// <param name="pszFilename">Pointer to the plasibility quote in percent</param>
		/// <param name="pFeatures">Pointer to buffer for the return value</param>
		/// <param name="pFeatures">Pointer to the plasibility quote in percent</param>
		/// <param name="piProbability">Pointer to the plasibility quote in percent</param>
		/// <returns>Class</returns>
		[DllImport ("C45.dll", EntryPoint = "Classification", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private extern static Int32 Consult (String	pszFilename, 
			IntPtr				pFeatures,
			ref Int32			pClassesCount,
			ref IntPtr			pOutClasses,
			_C45DebugParameter	debugParameter);
		#endregion

		#region GetStatistics
		[DllImport ("C45.dll", EntryPoint = "GetStatistics", ExactSpelling = false, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private extern static void GetStatistics (ref _C45Statistics pStatistics);
		#endregion

		#region GetStatistics
		public C45Statistics GetStatistics ()
		{
			C45Statistics c45stats = new C45Statistics ();
			C45Controller.GetStatistics (ref c45stats._statistics);

			c45stats.TreeItems = c45stats._statistics.iTreeItems;
			c45stats.DecisionTreeGenerationTime = c45stats._statistics.lDecisionTreeGenerationTime;
			c45stats.ConsultationTime = c45stats._statistics.lConsultationTime;

			return c45stats;
		}
		#endregion

		#region Classification
		public C45ComputedResultList Classification (_C45Feature[] features)
		{
			int classesCount = 0;
			String detectedClass = String.Empty;			

			if (this.Settings.C45SchemaFilename == String.Empty)
			{
				throw (new C45Exception (C45Exceptions.NO_FILENAME_SET));
			}

			if (!this.isTreeActive)
			{
				try
				{
					this.OnDecisionTreeGenerationStarted (
						this, 
						new EventArgs ());

					int c45RetCode = GenerateDTree (
											this.Settings.C45SchemaFilename,
											this.Settings.GainRatio,
											this.Settings.SubtreeRaising,
											this.Settings.SoftTresh,
											this.Settings.ConfidenceLevel,
											this.Settings.DebugParameter.DebugParameter);

					this.OnDecisionTreeGenerationFinished (this, new EventArgs ());

					if (c45RetCode == 0)
					{
						this.isTreeActive = true;
					}
					else
					{
						//Match C45Retcode with C45Exceptions here later
						throw (new C45Exception (C45Exceptions.TREE_NOT_GENERATED));
					}
				}
				catch (Exception e)
				{
					throw (new C45Exception (C45Exceptions.TREE_NOT_GENERATED, e));
				}
			}

			IntPtr featurePtr = Marshal.UnsafeAddrOfPinnedArrayElement (features, 0);
			_resultClass[] resultClasses = new _resultClass[C45Constants.MAX_RESULT_CLASSES];

			for (int i = 0; i < C45Constants.MAX_RESULT_CLASSES; i++)
			{
				resultClasses[i].bClassBuffer = new byte[C45Constants.MAX_CHAR];
			}

			IntPtr outClassesPtr = Marshal.UnsafeAddrOfPinnedArrayElement (resultClasses, 0);

			this.OnConsultationStarted (
				this, 
				new EventArgs ());
			
			Consult (
				this.Settings.C45SchemaFilename,
				featurePtr,
				ref classesCount,
				ref outClassesPtr,
				this.Settings.DebugParameter.DebugParameter);

			this.OnConsultationFinished (
				this, 
				new EventArgs ());

			return (new C45ComputedResultList (
				resultClasses, 
				classesCount));
		}
		#endregion

		#region ParseNamesFile
		public C45Schema ParseNamesFile (String filename)
		{
			C45Schema schema = new C45Schema ();

			try
			{
				FileStream f = new FileStream (filename, 
											FileMode.Open);
				StreamReader s = new StreamReader (f);
				String line = String.Empty;

				do
				{
					line = s.ReadLine ();
					line = line.TrimStart ("\t".ToCharArray ());
				} while (line == String.Empty || line[0] == '|');

				while (!line.Contains (".") && !s.EndOfStream)
				{
					line += s.ReadLine ();
				}

				line = line.Remove (line.LastIndexOfAny (".".ToCharArray ()));
				String[] values = line.Split (",".ToCharArray ());

				//Add classes
				if (values.Length > 0)
				{
					foreach (String newClass in values)
					{
						String newString = newClass.Trim ();
						schema.AddClass (newString);
					}
				}

				//Add properties
				while (!s.EndOfStream)
				{
					line = s.ReadLine ();

					if (		line != String.Empty
							&&	line[0] != '|')
					{
						values = this.FormatLine (line);

						if (values[1].Contains ("continuous"))
						{
							schema.Properties.Add (false, values[0], null);
						}
						else
						{
							//Discrete attribute found							
							String[] discreteAttributes = values[1].Split (",".ToCharArray ());
							List<String> attributeList = new List<String> ();

							foreach (String singleAttribute in discreteAttributes)
							{
								String attribute = singleAttribute.Trim ();
								attributeList.Add (attribute);
							}
							schema.Properties.Add (true, values[0], attributeList);							
						}
					}
				}
			}
			catch (Exception e)
			{
				throw (new C45Exception (C45Exceptions.PARSE_NAMES_ERROR, e));
			}

			return schema;
		}
		#endregion

		#region FormatLine
		private String[] FormatLine (String line)
		{
			String[] values;
			line = line.Remove (line.LastIndexOfAny (".".ToCharArray ()));
			line = line.Trim ();
			line = line.Replace ("\t", null);
			line = line.Replace ("\n", null);
			values = line.Split (":".ToCharArray ());
			values[0] = values[0].Trim ();
			values[1] = values[1].Trim ();

			return values;
		}
		#endregion

		#region AddComputedResults
		public void AddResult (
			C45FeatureSet featureSet, 
			String result)
		{
			try
			{
				String dataFile = this.Settings.C45SchemaFilename + @".data";
				FileStream fs = new FileStream (dataFile, 
					FileMode.Append);
				String newCase = String.Empty;

				for (int i = 0; i < featureSet.Count; i++)
				{
					if (featureSet._featureArray[i].bIsKnown)
					{
						if (featureSet._featureArray[i].bIsDiscreteFeature)
						{
							newCase += this.ConvertByteArrayToString (featureSet._featureArray[i].discreteFeature.data, 4) + @",";
						}
						else
						{
							//Calculate the average continuous value
							int continuous = Convert.ToInt32 ((featureSet._featureArray[i].continuousFeature.fLowerBound + featureSet._featureArray[i].continuousFeature.fUpperBound) / 2);
							newCase += continuous.ToString () + @",";
						}
					}
					else
					{
						newCase += @"?,";
					}
				}

				newCase += result;

				fs.WriteByte (0x0A);
				foreach (byte b in newCase.ToCharArray ())
				{
					fs.WriteByte (b);
				}
				fs.Close ();
			}
			catch (Exception ex)
			{
				throw (new C45Exception (C45Exceptions.CASES_NOT_ADDED, ex));
			}
		}
		#endregion

		#region GetFSCFromFile
		internal C45FeatureSetCollection GetFSCFromFile (String filename)
		{
			C45FeatureSetCollection featureSetCollection = new C45FeatureSetCollection ();
			C45Schema schema = C45Schema.CreateFromFile (filename.Replace (".data".ToString (), ".names".ToString ()));			
			int character = 0;
			String line = String.Empty;
			FileStream file = new FileStream (
				filename, 
				FileMode.Open);

			while (character >= 0)
			{
				line = String.Empty;
				character = file.ReadByte ();

				while (character != 0x0A
					&& character > 0)
				{
					line += Convert.ToChar (character);
					character = file.ReadByte ();
				}
				featureSetCollection.Add (C45FeatureSet.GetFSFromLine (line, schema));
			}

			return featureSetCollection;
		}
		#endregion

		#region ConvertByteArrayToString
		private String ConvertByteArrayToString (
			byte[] byteArray,
			int index)
		{
			byte[] newArray = new byte[byteArray.Length];
			Array.Copy (byteArray, index, newArray, 0, byteArray.Length - index);
			return this.ConvertByteArrayToString (newArray);
		}
		#endregion

		#region ConvertByteArrayToString
		private String ConvertByteArrayToString (byte[] byteArray)
		{
			String newString = String.Empty;

			foreach (char b in byteArray)
			{
				if (b > 0)
				{
					newString += b.ToString ();
				}
			}

			return newString;
		}
		#endregion
	}
	#endregion
}