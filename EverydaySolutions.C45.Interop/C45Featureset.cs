using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace EverydaySolutions.C45
{
	public class C45FeatureSet : List <object>
	{
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = C45Constants.MAX_FEATURES)]
		internal _C45Feature[] _featureArray = new _C45Feature[C45Constants.MAX_FEATURES];
		
		#region Results
		private C45ComputedResultList computedResult = null;
		public C45ComputedResultList ComputedResult
		{
			get
			{
				return computedResult;
			}
			set
			{
				computedResult = value;
			}
		}
		#endregion

		#region UserResult
		private C45UserResult userResult = new C45UserResult ();
		public C45UserResult UserResult
		{
			get
			{
				return userResult;
			}
			set
			{
				userResult = value;
			}
		}
		#endregion

		#region Marked
		private bool isMarked;
		public bool IsMarked
		{
			get
			{
				return isMarked;
			}
			set
			{
				isMarked = value;
			}
		}
		#endregion

		#region IsClassified
		internal bool isClassified = false;
		public bool IsClassified
		{
			get
			{
				return isClassified;
			}
		}
		#endregion

		#region IsDirty
		private bool isDirty = false;
		public bool IsDirty
		{
			get
			{
				return isDirty;
			}
			set
			{
				isDirty = value;
			}
		}
		#endregion

		#region AddAddContinousFeature
		public void AddContinousFeature (C45ContinuousFeature newFeature)
		{
			this._featureArray[this.Count] = newFeature.Feature.Feature;
			this.Add (newFeature);
		}
		#endregion

		#region AddDiscreteFeature
		public void AddDiscreteFeature (C45DiscreteFeature newFeature)
		{
			this._featureArray[this.Count] = newFeature.Feature.Feature;
			this.Add (newFeature);
		}
		#endregion

		#region AddUnknownFeature
		public void AddUnknownFeature ()
		{
			C45Feature newFeature = new C45Feature ();
			this._featureArray[this.Count] = newFeature.Feature;
			this.Add (newFeature);
		}
		#endregion

		#region GetFSFromLine
		internal static C45FeatureSet GetFSFromLine (String line, C45Schema schema)
		{
			C45FeatureSet featureSet = new C45FeatureSet ();
			int currentFeature = 0;
			String[] features = line.Split (",".ToCharArray ());

			for (int i = 0; i < features.Length; i++)
			{
				features[i] = features[i].Trim ();
			}
			
			foreach (C45SchemaProperty column in schema.Properties)
			{
				if (features[currentFeature] == ""
					|| features[currentFeature] == "?")
				{
					featureSet.AddUnknownFeature ();
				}
				else
				{
					if (column.IsDiscrete)
					{
						C45DiscreteFeature discreteFeature = new C45DiscreteFeature ();
						C45DiscreteFeatureAttribute discreteFeatureAttribute = new C45DiscreteFeatureAttribute ();

						discreteFeatureAttribute.AttributeName = features[currentFeature];
						discreteFeatureAttribute.Probability = 100;

						discreteFeature.Add (discreteFeatureAttribute);

						featureSet.AddDiscreteFeature (discreteFeature);
					}
					else
					{
						C45ContinuousFeature continuousFeature = new C45ContinuousFeature ();
						continuousFeature.LowerBound = Convert.ToInt32 (features[currentFeature]);
						continuousFeature.UpperBound = Convert.ToInt32 (features[currentFeature]);

						featureSet.AddContinousFeature (continuousFeature);
					}
				}

				currentFeature++;
			}

			featureSet.UserResult.ClassName = features[currentFeature];
			
			return featureSet;
		}
		#endregion
	};
}