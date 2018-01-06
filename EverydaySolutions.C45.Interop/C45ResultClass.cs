using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace EverydaySolutions.C45
{
	#region _resultClass
	[StructLayout (LayoutKind.Explicit, Pack = 4, CharSet = CharSet.Ansi)]
	public struct _resultClass
	{
		[FieldOffset (0)]
		public float iBestGuess;
		[FieldOffset (4)]
		public float iLowerBound;
		[FieldOffset (8)]
		public float iUpperBound;
		[FieldOffset (12), MarshalAs (UnmanagedType.ByValArray, SizeConst = C45Constants.MAX_CHAR)]
		public byte[] bClassBuffer;

		public C45ComputedResult C45ResultClass
		{
			get
			{
				throw new System.NotImplementedException ();
			}
			set
			{
			}
		}
	}
	#endregion

	public class C45ComputedResult
	{
		#region LowerRangeProbability
		private float lowerRangeProbability;
		public float LowerRangeProbability
		{
			get
			{
				return lowerRangeProbability;
			}
			set
			{
				lowerRangeProbability = value;
			}
		}
		#endregion

		#region UpperRangeProbability
		private float upperRangeProbability;
		public float UpperRangeProbability
		{
			get
			{
				return upperRangeProbability;
			}
			set
			{
				upperRangeProbability = value;
			}
		}
		#endregion

		#region BestGuessProbability
		private float bestGuessProbability;
		public float BestGuessProbability
		{
			get
			{
				return bestGuessProbability;
			}
			set
			{
				bestGuessProbability = value;
			}
		}
		#endregion

		#region ResultClassName
		private String resultClassName;
		public String ResultClassName
		{
			get
			{
				return resultClassName;
			}
			set
			{
				resultClassName = value;
			}
		}
		#endregion

		#region ToValueString
		public String ToValueString ()
		{
			String valueString = String.Empty;

			valueString = this.ResultClassName
				+ " ("
				+ this.BestGuessProbability.ToString ()
				+ " / "
				+ this.LowerRangeProbability.ToString ()
				+ " - "
				+ this.UpperRangeProbability.ToString ()
				+ ")";

			return valueString;
		}
		#endregion
		
	}
}