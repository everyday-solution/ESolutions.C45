using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DocumentProcessing.Model;

namespace DocumentProcessing.GUI
{
	/// <summary>
	/// Form for the extraction frame.
	/// </summary>
	public class TableExtractorFrame : System.Windows.Forms.Form
	{
		// I was not able to determine the size of the non-client elements, so I decided to "const" them
		// (optimization possible ;-))
		private const int NONCLIENT_FORM_TOP = 18;
		private const int NONCLIENT_FORM_LEFT = 4;
		private const int NONCLIENT_FORM_RIGHT = 4;
		private const int NONCLIENT_FORM_BOTTOM = 4;

		// the viewer reference is used for the UpdateTableRequestByFrame() method only
		// suggestion: do UpdateTableRequestByFrame() in the Main-Form and remove this reference
		private AxMODI.AxMiDocView _documentViewer = null;
		private TableRequest _request = null;

		private System.Windows.Forms.Panel panel1;
		private DocumentProcessing.GUI.TransparentListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader4;

		private System.ComponentModel.Container components = null;

		public TableExtractorFrame(AxMODI.AxMiDocView documentViewer,TableRequest request )
		{
			InitializeComponent();
			_documentViewer = documentViewer;
			_request = request;
			// Synchronize transparent color
			this.TransparencyKey = TransparentListView.TransparentColor;
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Vom Windows Form-Designer generierter Code

		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TableExtractorFrame));
			this.panel1 = new System.Windows.Forms.Panel();
			this.listView1 = new DocumentProcessing.GUI.TransparentListView();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.listView1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(328, 158);
			this.panel1.TabIndex = 0;
			// 
			// listView1
			// 
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView1.Location = new System.Drawing.Point(0, 0);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(328, 158);
			this.listView1.TabIndex = 0;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "";
			this.columnHeader4.Width = 868;
			// 
			// TableExtractorFrame
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(328, 158);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TableExtractorFrame";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "TableExtractor";
			this.TopMost = true;
			this.TransparencyKey = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(192)), ((System.Byte)(192)));
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Adjust the transparent frame to the target area
		/// </summary>
		/// <param name="target">Bounds In screen coordinates</param>
		public void AdjustFrameToTarget(Rectangle target)
		{
			this.Focus();
			Rectangle r4 = this.ClientRectangle;
			Rectangle newBounds = target;

			int offsetLeft =  NONCLIENT_FORM_LEFT +listView1.Left - r4.Left; 
			int offsetTop =  NONCLIENT_FORM_TOP +listView1.Top - r4.Top + TransparentListView.NONCLIENT_LISTVIEW_HEADER_HEIGHT; 
			int offsetRight = NONCLIENT_FORM_RIGHT + r4.Right - listView1.Right;
			int offsetBottom = NONCLIENT_FORM_BOTTOM +r4.Bottom - listView1.Bottom;

			newBounds.X = target.X - (offsetLeft);
			newBounds.Y = target.Y - (offsetTop);
			newBounds.Width += offsetLeft + offsetRight -2;
			newBounds.Height += offsetTop + offsetBottom -2;
			
			this.SetBounds( newBounds.X,newBounds.Y,newBounds.Width,newBounds.Height);
			this.Location = new Point(newBounds.X,newBounds.Y);
			listView1.UpdateHeaders();
			this.Invalidate();
			this.Refresh();
		}
		
	
		/// <summary>
		/// Adjust the columns and their sizes to the current table request
		/// </summary>
		public void AdjustColumnsToRequest()
		{
			listView1.Columns.Clear();
			for (int i = 0; i <_request.ColumnRequests.Count; i++)
			{
				ColumnRequest colRequest = (ColumnRequest) _request.ColumnRequests[i];
				float colWidth = colRequest.Width*listView1.Width;
				listView1.Columns.Add(i.ToString(),(int)colWidth,HorizontalAlignment.Left);
			}
			listView1.UpdateHeaders();
		}
					
		private Rectangle GetFrameAreaToScreen()
		{
			Rectangle listViewBounds = listView1.Bounds; 
			listViewBounds.Y += TransparentListView.NONCLIENT_LISTVIEW_HEADER_HEIGHT;
			listViewBounds.Height -= TransparentListView.NONCLIENT_LISTVIEW_HEADER_HEIGHT;
			return this.RectangleToScreen( listViewBounds );
		}
	
		/// <summary>
		/// If the user does changes for the frame (moving, resizing, column resizing), update the table request
		/// </summary>
		public void UpdateTableRequestByFrame()
		{
			Rectangle frameAreaOnScreen = GetFrameAreaToScreen();
			
			_request.DocumentArea = DocumentViewerSupport.GetScreenToImageRectangle(_documentViewer,frameAreaOnScreen);
			int offset = 1;
			for (int l = 0; l < listView1.Columns.Count; l++)
			{
				ColumnRequest colRequest =(ColumnRequest) _request.ColumnRequests[l];
				Rectangle columnRect = listView1.ClientRectangle;
				columnRect.X = offset;
				columnRect.Width = listView1.Columns[l].Width;
				columnRect = listView1.RectangleToScreen( columnRect );
				colRequest.Width = (float) ( 1f* listView1.Columns[l].Width / listView1.Width);
				offset += listView1.Columns[l].Width;
			}
			
		}

	

	}

	
}
