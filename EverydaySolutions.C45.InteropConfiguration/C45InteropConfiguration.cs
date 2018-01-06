using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EverydaySolutions.C45
{
	public class C45InteropConfiguration
	{
		#region C45InteropConfiguration
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">C45Settings object</param>
		/// <param name="windowOwner">Owner window handle interface</param>
		public C45InteropConfiguration (C45Settings settings,
			IWin32Window windowOwner)
		{
			this.settings = settings;
			OptionsForm form = new OptionsForm (settings);
			form.ShowDialog (windowOwner);

			if (form.DialogResult == DialogResult.OK)
			{
				this.settings = form.Settings;
				this.Changed = true;
			}
		}
		#endregion

		#region Changed
		/// <summary>
		/// Indicates whether the configuration has been changed
		/// </summary>
		private bool changed = false;
		public bool Changed
		{
			get
			{
				return changed;
			}
			set
			{
				changed = value;
			}
		}
		#endregion

		#region Settings
		/// <summary>
		/// C45Settings object (read-only)
		/// </summary>
		private C45Settings settings;
		public C45Settings Settings
		{
			get
			{
				return settings;
			}
		}
		#endregion
	}
}
