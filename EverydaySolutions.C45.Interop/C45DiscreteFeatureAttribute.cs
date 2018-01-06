using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace EverydaySolutions.C45
{
	#region _discreteFeatureAttribute
	[StructLayout (LayoutKind.Explicit, Pack = 4, Size = 260)]
	internal struct _C45DiscreteFeatureAttribute
	{
		[FieldOffset (00)]
		internal float fProbability;
		[FieldOffset (04), MarshalAs (UnmanagedType.ByValArray, SizeConst = C45Constants.MAX_CHAR)]
		internal char[] szAttribute;
	}
	#endregion

	#region C45DiscreteFeatureAttribute
	public class C45DiscreteFeatureAttribute
	{
		#region C45DiscreteFeatureAttribute
		public C45DiscreteFeatureAttribute ()
		{
			this._discreteFeatureAttribute.szAttribute = new char[C45Constants.MAX_CHAR];
		}
		#endregion

		#region _DiscreteFeatureAttribute
		internal _C45DiscreteFeatureAttribute _discreteFeatureAttribute = new _C45DiscreteFeatureAttribute ();
		internal _C45DiscreteFeatureAttribute _DiscreteFeatureAttribute
		{
			get
			{
				return _discreteFeatureAttribute;
			}
			set
			{
				_discreteFeatureAttribute = value;
			}
		}
		#endregion

		#region Probability
		protected float probability;
		public float Probability
		{
			get
			{
				return probability;
			}
			set
			{
				probability = value;
				this._discreteFeatureAttribute.fProbability = value;
			}
		}
		#endregion

		#region AttributeName
		private String name;
		public String AttributeName
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
				Array.Copy (value.ToCharArray (), 
					this._discreteFeatureAttribute.szAttribute, 
					value.Length);
			}
		}
		#endregion
	}
	#endregion
}