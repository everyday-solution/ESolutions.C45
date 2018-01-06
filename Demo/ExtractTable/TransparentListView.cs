using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices; 

namespace DocumentProcessing.GUI
{
	public class TransparentListView : System.Windows.Forms.ListView
	{
		// Instead of calulating the header's height, here is a constant
		public const int NONCLIENT_LISTVIEW_HEADER_HEIGHT = 14;

		// Just to choose one, "Bisque" is the transparency color
		public static Color TransparentColor = Color.Bisque;
		
		public TransparentListView()
		{
			SetStyle(ControlStyles.UserPaint|ControlStyles.ResizeRedraw,true);
			this.View = View.Details;
		}

		/// <summary>
		/// Adds the autosize effect
		/// </summary>
		public void UpdateHeaders()
		{
			// initialize all headers..
			for (int i = 0; i < this.Columns.Count; i++)
			{
				if ( this.Columns[this.Columns.Count - 1].Width == -2)
					this.Columns[this.Columns.Count - 1].Width = 10;
			}
			// initialize all headers..
			if ( this.View == View.Details && this.Columns.Count > 0 )
				this.Columns[this.Columns.Count - 1].Width = -2 ;

		}
		
		/// <summary>
		/// Overwritten to show row lines in transparent mode
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = this.CreateGraphics();
			g.FillRectangle(new SolidBrush(TransparentColor),0,0,this.Width,this.Height);
			int x = 0;
			for (int i = 0; i < Columns.Count-1; i++)
			{
				x += Columns[i].Width;
				g.DrawLine(new Pen(Color.Blue,2f),x-1,0,x-1,this.Height);
			}
			base.OnPaint (e);
		}

		protected override void WndProc( ref Message m )
		{
			const int WM_PAINT = 0xf ;

			switch ( m.Msg )
			{
				case WM_PAINT:
					UpdateHeaders();
					break ;
			}
			base.WndProc( ref m ) ;
		}
	
	}

}
