using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace EverydaySolutions.C45
{
	#region _feature
	[StructLayout (LayoutKind.Explicit, Pack = 4, Size = 20)]
	internal struct _C45Feature
	{
		[FieldOffset (00)]
		internal bool bIsDiscreteFeature;
		[FieldOffset (04)]
		internal _C45ContinousFeature continuousFeature;
		[FieldOffset (12)]
		internal _C45DiscreteFeature discreteFeature;
		[FieldOffset (16)]
		internal bool bIsKnown;
	}
	#endregion

	public class C45Feature
	{
		#region C45Feature
		internal _C45Feature _feature = new _C45Feature ();
		internal _C45Feature Feature
		{
			get
			{
				return _feature;
			}
			set
			{
				_feature = value;
			}
		}
		#endregion 

		#region IsDiscreteFeature
		private bool isDiscreteFeature;
		internal bool IsDiscreteFeature
		{
			get
			{
				return isDiscreteFeature;
			}
			set
			{
				isDiscreteFeature = value;
				this._feature.bIsDiscreteFeature = value;
			}
		}
		#endregion

		#region IsKnown
		private bool isKnown = false;
		internal bool IsKnown
		{
			get
			{
				return isKnown;
			}
			set
			{
				isKnown = value;
				this._feature.bIsKnown = value;
			}
		}
		#endregion
	}
}
