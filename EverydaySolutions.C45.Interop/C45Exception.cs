using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EverydaySolutions.C45
{
	#region C45Exception
	[global::System.Serializable]
	public class C45Exception : Exception
	{
		#region C45Exception
		public C45Exception ()
		{
		}
		#endregion

		#region C45Exception
		public C45Exception (string message)
			: base (message)
		{
		}
		#endregion

		#region C45Exception
		public C45Exception (string message, 
									Exception inner)
			: base (message, inner)
		{
		}
		#endregion

		#region C45Exception 
		protected C45Exception (System.Runtime.Serialization.SerializationInfo info,
										System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
		}
		#endregion

	
		#region GetString
		public String GetString ()
		{
			return (this.Message
				+ "\n" 
				+ @" ("
				+ this.InnerException.Message
				+ @" : "
				+ "\n"
				+ this.StackTrace
				+ @")");
		}
		#endregion
	}
	#endregion
}
