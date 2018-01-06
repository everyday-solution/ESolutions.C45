using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EverydaySolutions.C45.C45DemoApp
{
	static class Program
    {
        #region Main
        /// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main ()
		{
			Application.EnableVisualStyles ();

			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler (Application_ThreadException);
			Application.Run (new MainForm ());
        }
        #endregion

        #region Application_ThreadException
        static void Application_ThreadException (object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			MessageBox.Show (e.Exception.Message 
				+ "\n" 
				+ e.Exception.StackTrace);
		}
		#endregion
	}
}