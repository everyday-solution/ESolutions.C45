using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace EverydaySolutions.C45
{
	#region _continousFeature
	[StructLayout (LayoutKind.Explicit, Pack = 4)]
	internal struct _C45ContinousFeature
	{
		[FieldOffset (00)]
		public float fLowerBound;
		[FieldOffset (04)]
		public float fUpperBound;
	};
	#endregion

	public class C45ContinuousFeature
	{
		#region C45ContinuousFeature
		public C45ContinuousFeature ()
		{
			this.feature._feature.continuousFeature = new _C45ContinousFeature ();
			this.feature._feature.bIsDiscreteFeature = false;
			this.feature.IsKnown = true;
		}
		#endregion

		#region C45Feature
		private C45Feature feature = new C45Feature ();
		public C45Feature Feature
		{
			get
			{
				return feature;
			}
			set
			{
				feature = value;
			}
		}
		#endregion

		#region LowerBound
		private Int32 lowerBound;
		public Int32 LowerBound
		{
			get
			{
				return lowerBound;
			}
			set
			{
				lowerBound = value;
				this.feature._feature.continuousFeature.fLowerBound = value;
			}
		}
		#endregion

		#region UpperBound
		private Int32 upperBound;
		public Int32 UpperBound
		{
			get
			{
				return upperBound;
			}
			set
			{
				upperBound = value;
				this.feature._feature.continuousFeature.fUpperBound = value;
			}
		}
		#endregion
	}
}
