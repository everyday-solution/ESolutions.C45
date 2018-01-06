using System;
using System.Collections.Generic;
using System.Text;

namespace EverydaySolutions.C45
{
    #region C45SingleFeatureSetClassifiedEventArgs
    public class C45SingleFeatureSetClassifiedEventArgs : EventArgs
    {
        #region C45SingleFeatureSetClassifiedEventArgs
        public C45SingleFeatureSetClassifiedEventArgs (C45Statistics stats)
		{
			this.statistics = stats;
        }
        #endregion

        #region Statistics
        private C45Statistics statistics;
		public C45Statistics Statistics
		{
			get
			{
				return statistics;
			}
			set
			{
				statistics = value;
			}
		}
		#endregion 
    }
    #endregion
}
