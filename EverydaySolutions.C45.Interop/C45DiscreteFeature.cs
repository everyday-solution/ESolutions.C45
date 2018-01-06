using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace EverydaySolutions.C45
{
	#region _discreteFeature
	[StructLayout (LayoutKind.Sequential, Pack = 4, Size = 1)]
	internal struct _C45DiscreteFeature
	{
		[MarshalAs (UnmanagedType.ByValArray, SizeConst = C45Constants.MAX_DISCRETE_ATTRIBUTES * 260)]
		internal byte[] data;

		#region _discreteFeature
		internal _C45DiscreteFeature (int dummy)
		{
			//refactor
			data = new byte[C45Constants.MAX_DISCRETE_ATTRIBUTES * 260];
		}
		#endregion

		#region _discreteFeatureAttribute
		internal _C45DiscreteFeatureAttribute this[int index]
		{
			get
			{
				if ((index < 0) | (index >= C45Constants.MAX_DISCRETE_ATTRIBUTES))
				{
					throw new IndexOutOfRangeException ();
				}
				_C45DiscreteFeatureAttribute disFeature;
				GCHandle handle = GCHandle.Alloc (data, GCHandleType.Pinned);

				try
				{
					IntPtr buffer = handle.AddrOfPinnedObject ();
					buffer = (IntPtr)(buffer.ToInt32 () + (index * Marshal.SizeOf (typeof (_C45DiscreteFeatureAttribute))));
					disFeature = (_C45DiscreteFeatureAttribute)Marshal.PtrToStructure (buffer, typeof (_C45DiscreteFeatureAttribute));
				}
				finally
				{
					handle.Free ();
				}
				return disFeature;
			}
			set
			{
				_C45DiscreteFeatureAttribute disFeature = value;
				IntPtr ptr = Marshal.AllocHGlobal (C45Constants.MAX_CHAR + sizeof (float));
				Marshal.StructureToPtr (disFeature,
					ptr,
					false);
				data = new byte[C45Constants.MAX_DISCRETE_ATTRIBUTES * 260];
				Marshal.Copy (ptr,
					data,
					index * Marshal.SizeOf (typeof (_C45DiscreteFeatureAttribute)),
					Marshal.SizeOf (typeof (_C45DiscreteFeatureAttribute)));
			}
		}
		#endregion		

	}
	#endregion

	public class C45DiscreteFeature : List<C45DiscreteFeatureAttribute>
	{
		#region C45DiscreteFeature
		public C45DiscreteFeature ()
		{
			this.feature.IsKnown = true;
			this.feature.IsDiscreteFeature = true;
		}
		#endregion

		#region C45Feature
		private C45Feature feature = new C45Feature ();
		public C45Feature Feature
		{
			get
			{
				int index = 0;
				foreach (C45DiscreteFeatureAttribute f in this)
				{
					feature._feature.discreteFeature[index] = f._DiscreteFeatureAttribute;
					index++;
				}
				return feature;
			}
		}
		#endregion

		public C45DiscreteFeatureAttribute C45DiscreteFeatureAttribute
		{
			get
			{
				throw new System.NotImplementedException ();
			}
			set
			{
			}
		}



		/*
		#region _DiscreteFeatureAttribute
		private _discreteFeature discreteFeature;
		internal _discreteFeature DiscreteFeature
		{
			get
			{
				return discreteFeature;
			}
		}
		#endregion
		 */ 
	}
}