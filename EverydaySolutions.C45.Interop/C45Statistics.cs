using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace EverydaySolutions.C45
{
	#region _statistics
	[StructLayout (LayoutKind.Explicit, Pack = 4)]
	internal struct _C45Statistics
	{
		[FieldOffset (00)]
		internal int iTreeItems;
		[FieldOffset (04)]
		internal int lDecisionTreeGenerationTime;
		[FieldOffset (08)]
		internal int lConsultationTime;
	};
	#endregion

	public class C45Statistics
	{
		internal _C45Statistics _statistics;

		#region TreeItems
		private int treeItems;
		public int TreeItems
		{
			get
			{
				return treeItems;
			}
			set
			{
				treeItems = value;
			}
		}
		#endregion

		#region DecisionTreeGenerationTime
		private long decisionTreeGenerationTime;
		public long DecisionTreeGenerationTime
		{
			get
			{
				return decisionTreeGenerationTime;
			}
			set
			{
				decisionTreeGenerationTime = value;
			}
		}
		#endregion

		#region ConsultationTime
		private long consultationTime;
		public long ConsultationTime
		{
			get
			{
				return consultationTime;
			}
			set
			{
				consultationTime = value;
			}
		}
		#endregion
	}
}
