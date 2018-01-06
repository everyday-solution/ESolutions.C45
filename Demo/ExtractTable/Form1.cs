using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using DocumentProcessing;
using DocumentProcessing.Model;
using DocumentProcessing.GUI;
/*

Main reference: 
http://msdn.microsoft.com/library/default.asp?url=/library/en-us/Mspauto/html/dihowUsingMODIObjectModel_HV01049396.asp
     
Please note:
The Microsoft® Office Document Imaging Library 2003 (MODI) object model makes 
it possible to develop custom applications for managing document images 
(such as scanned and faxed documents) and the recognizable text that they 
contain. The MODI components include the MODI Viewer Control, an ActiveX® control 
that you can use to display MODI documents. 

Important: The MODI programmability features described in this document are 
available only in Microsoft Office Document Imaging 2003. The Microsoft Office XP 
version of document imaging does not include a programmable object model.

*/

namespace DocumentProcessing
{

	public class Form1 : System.Windows.Forms.Form
	{
		private Document _Document = null;
		private MODIOCRParameters _MODIParameters = new MODIOCRParameters();
		private MODI.Document _MODIDocument = null;
		private TableExtractorFrame _ExtractionFrame = null; 
		private TableRequest _tableRequest = new TableRequest();
		private TableResult _tableResult = null;
		private string _filename = "";

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem miFile;
		private System.Windows.Forms.MenuItem miOpen;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem miCopy;
		private System.Windows.Forms.MenuItem miAnalyse;
		private System.Windows.Forms.MenuItem miSave;
		private System.Windows.Forms.MenuItem miOCRParameters;
		private System.Windows.Forms.MenuItem miExtractTable;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Splitter splitter1;
		private AxMODI.AxMiDocView axMiDocView1;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton toolBarButton5;
		private System.Windows.Forms.ToolBarButton toolBarButton6;
		private System.Windows.Forms.ToolBarButton toolBarButton7;
		private System.Windows.Forms.ToolBarButton toolBarButton8;
		private System.Windows.Forms.MenuItem miExportTable;
		private System.Windows.Forms.ToolBarButton toolBarButton9;

		private System.ComponentModel.IContainer components;

		public Form1()
		{
			InitializeComponent();

			axMiDocView1.FitMode = MODI.MiFITMODE.miByWindow;

			statusBar1.Text = "Ready.";
		
			NewRequest();

			InitExtractionFrame();

			ShowTitel();
		}

		
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Vom Windows Form-Designer generierter Code

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container ();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager (typeof (Form1));
			this.mainMenu1 = new System.Windows.Forms.MainMenu (this.components);
			this.miFile = new System.Windows.Forms.MenuItem ();
			this.miOpen = new System.Windows.Forms.MenuItem ();
			this.miExportTable = new System.Windows.Forms.MenuItem ();
			this.miSave = new System.Windows.Forms.MenuItem ();
			this.menuItem1 = new System.Windows.Forms.MenuItem ();
			this.miCopy = new System.Windows.Forms.MenuItem ();
			this.miAnalyse = new System.Windows.Forms.MenuItem ();
			this.miOCRParameters = new System.Windows.Forms.MenuItem ();
			this.miExtractTable = new System.Windows.Forms.MenuItem ();
			this.statusBar1 = new System.Windows.Forms.StatusBar ();
			this.axMiDocView1 = new AxMODI.AxMiDocView ();
			this.toolBar1 = new System.Windows.Forms.ToolBar ();
			this.toolBarButton8 = new System.Windows.Forms.ToolBarButton ();
			this.toolBarButton9 = new System.Windows.Forms.ToolBarButton ();
			this.toolBarButton4 = new System.Windows.Forms.ToolBarButton ();
			this.toolBarButton6 = new System.Windows.Forms.ToolBarButton ();
			this.toolBarButton5 = new System.Windows.Forms.ToolBarButton ();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton ();
			this.toolBarButton7 = new System.Windows.Forms.ToolBarButton ();
			this.toolBarButton3 = new System.Windows.Forms.ToolBarButton ();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton ();
			this.imageList1 = new System.Windows.Forms.ImageList (this.components);
			this.listView1 = new System.Windows.Forms.ListView ();
			this.splitter1 = new System.Windows.Forms.Splitter ();
			((System.ComponentModel.ISupportInitialize)(this.axMiDocView1)).BeginInit ();
			this.SuspendLayout ();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange (new System.Windows.Forms.MenuItem[] {
            this.miFile,
            this.menuItem1});
			// 
			// miFile
			// 
			this.miFile.Index = 0;
			this.miFile.MenuItems.AddRange (new System.Windows.Forms.MenuItem[] {
            this.miOpen,
            this.miExportTable,
            this.miSave});
			this.miFile.Text = "File";
			// 
			// miOpen
			// 
			this.miOpen.Index = 0;
			this.miOpen.Text = "Open..";
			this.miOpen.Click += new System.EventHandler (this.miOpen_Click);
			// 
			// miExportTable
			// 
			this.miExportTable.Index = 1;
			this.miExportTable.Text = "Export table..";
			this.miExportTable.Click += new System.EventHandler (this.miExportTable_Click);
			// 
			// miSave
			// 
			this.miSave.Index = 2;
			this.miSave.Text = "Save";
			this.miSave.Click += new System.EventHandler (this.miSave_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 1;
			this.menuItem1.MenuItems.AddRange (new System.Windows.Forms.MenuItem[] {
            this.miCopy,
            this.miAnalyse,
            this.miOCRParameters,
            this.miExtractTable});
			this.menuItem1.Text = "Edit";
			// 
			// miCopy
			// 
			this.miCopy.Index = 0;
			this.miCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.miCopy.Text = "Copy text to clipboard";
			this.miCopy.Click += new System.EventHandler (this.miCopy_Click);
			// 
			// miAnalyse
			// 
			this.miAnalyse.Index = 1;
			this.miAnalyse.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.miAnalyse.Text = "Start OCR";
			this.miAnalyse.Click += new System.EventHandler (this.miOCR_Click);
			// 
			// miOCRParameters
			// 
			this.miOCRParameters.Index = 2;
			this.miOCRParameters.Text = "Select OCR Parameters..";
			this.miOCRParameters.Click += new System.EventHandler (this.miOCRParameters_Click);
			// 
			// miExtractTable
			// 
			this.miExtractTable.Index = 3;
			this.miExtractTable.Shortcut = System.Windows.Forms.Shortcut.F2;
			this.miExtractTable.Text = "Extract Table";
			this.miExtractTable.Click += new System.EventHandler (this.miExtractTable_Click);
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point (0, 595);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size (760, 22);
			this.statusBar1.TabIndex = 4;
			// 
			// axMiDocView1
			// 
			this.axMiDocView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.axMiDocView1.Enabled = true;
			this.axMiDocView1.Location = new System.Drawing.Point (275, 42);
			this.axMiDocView1.Name = "axMiDocView1";
			this.axMiDocView1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject ("axMiDocView1.OcxState")));
			this.axMiDocView1.Size = new System.Drawing.Size (485, 553);
			this.axMiDocView1.SelectionChanged += new System.EventHandler (this.axMiDocView1_SelectionChanged);
			// 
			// toolBar1
			// 
			this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar1.Buttons.AddRange (new System.Windows.Forms.ToolBarButton[] {
            this.toolBarButton8,
            this.toolBarButton9,
            this.toolBarButton4,
            this.toolBarButton6,
            this.toolBarButton5,
            this.toolBarButton1,
            this.toolBarButton7,
            this.toolBarButton3,
            this.toolBarButton2});
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Location = new System.Drawing.Point (0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size (760, 42);
			this.toolBar1.TabIndex = 6;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler (this.toolBar1_ButtonClick);
			// 
			// toolBarButton8
			// 
			this.toolBarButton8.ImageIndex = 0;
			this.toolBarButton8.Name = "toolBarButton8";
			this.toolBarButton8.Tag = "OpenDocument";
			this.toolBarButton8.Text = "Open";
			// 
			// toolBarButton9
			// 
			this.toolBarButton9.ImageIndex = 0;
			this.toolBarButton9.Name = "toolBarButton9";
			this.toolBarButton9.Tag = "Export";
			this.toolBarButton9.Text = "Export";
			// 
			// toolBarButton4
			// 
			this.toolBarButton4.ImageIndex = 0;
			this.toolBarButton4.Name = "toolBarButton4";
			this.toolBarButton4.Tag = "OCR";
			this.toolBarButton4.Text = "OCR";
			// 
			// toolBarButton6
			// 
			this.toolBarButton6.ImageIndex = 0;
			this.toolBarButton6.Name = "toolBarButton6";
			this.toolBarButton6.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButton5
			// 
			this.toolBarButton5.ImageIndex = 0;
			this.toolBarButton5.Name = "toolBarButton5";
			this.toolBarButton5.Tag = "NewRequest";
			this.toolBarButton5.Text = "New Request";
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.ImageIndex = 0;
			this.toolBarButton1.Name = "toolBarButton1";
			this.toolBarButton1.Tag = "AppendColumn";
			this.toolBarButton1.Text = "Append Column";
			// 
			// toolBarButton7
			// 
			this.toolBarButton7.ImageIndex = 0;
			this.toolBarButton7.Name = "toolBarButton7";
			this.toolBarButton7.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButton3
			// 
			this.toolBarButton3.ImageIndex = 0;
			this.toolBarButton3.Name = "toolBarButton3";
			this.toolBarButton3.Tag = "AdjustFrame";
			this.toolBarButton3.Text = "Adjust and Capture";
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.ImageIndex = 0;
			this.toolBarButton2.Name = "toolBarButton2";
			this.toolBarButton2.Tag = "Capture";
			this.toolBarButton2.Text = "Capture";
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject ("imageList1.ImageStream")));
			this.imageList1.Images.SetKeyName (0, "");
			// 
			// listView1
			// 
			this.listView1.Dock = System.Windows.Forms.DockStyle.Left;
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point (0, 42);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size (272, 553);
			this.listView1.TabIndex = 7;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.Click += new System.EventHandler (this.listView1_Click);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point (272, 42);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size (3, 553);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size (5, 13);
			this.ClientSize = new System.Drawing.Size (760, 617);
			this.Controls.Add (this.axMiDocView1);
			this.Controls.Add (this.splitter1);
			this.Controls.Add (this.listView1);
			this.Controls.Add (this.toolBar1);
			this.Controls.Add (this.statusBar1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject ("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Table Extraction";
			((System.ComponentModel.ISupportInitialize)(this.axMiDocView1)).EndInit ();
			this.ResumeLayout (false);
			this.PerformLayout ();

		}
		#endregion

		[STAThread]
		static void Main() 
		{
			System.Windows.Forms.Application.EnableVisualStyles();
			System.Windows.Forms.Application.DoEvents();
			Application.Run(new Form1());
		}


		#region shared

		public void ShowProgress(int progress, ref bool cancel)
		{
			statusBar1.Text = progress.ToString() + "% processed.";
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			// toolbar implementation
			switch ((string) e.Button.Tag)
			{
				case "OpenDocument": 
					OpenDocument();
					break;
				case "OCR": 
					OCR();
					break;
				case "Export": 
					ExportDocument();
					break;
				case "NewRequest": 
					NewRequest();
					break;
				case "AppendColumn": 
					AppendColumn();
					break;
				case "Capture": 
					CaptureContent();
					break;
				case "AdjustFrame": 
					AdjustExtractorFrame();
					break;
				default:
					break;
			}
		}

		
		private void EnsureOCR()
		{
			if (_Document == null)
			{
				if (_MODIDocument != null)
				{
					OCR();
				}
			}
		}

	
		private void miOpen_Click(object sender, System.EventArgs e)
		{
			OpenDocument();
		}

		
		private void ShowTitel()
		{
			this.Text = "Table Extraction";
			if (_filename != "")
			{
				this.Text = "Table Extraction - "+ _filename;
			}
			
		}

		
		/// <summary>
		/// open document file..
		/// </summary>
		private void OpenDocument()
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Image files|*.tif;*.tiff;*.bmp|All files (*.*)|*.*" ;
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				_filename = dialog.FileName;
				SetImage(_filename);
				ShowTitel();
			}
		}

	
		#endregion

		#region OCR
		
		private bool IsDocumentAlreadyAnalysed()
		{
			if (_MODIDocument == null) return false;
			if (_MODIDocument.Images.Count == 0) return true;
			// try to access the Layout property: this is only available after OCR process.
			try
			{
				string text = (_MODIDocument.Images[0] as MODI.Image).Layout.Text;
			}
			catch(Exception e)
			{
				return false;
			}
			return true;
		}

	
		public void OCR()
		{
			if (_MODIDocument == null) return;
			try 
			{
				if (!IsDocumentAlreadyAnalysed()) 
				{
					// add event handler for progress visualisation
					_MODIDocument.OnOCRProgress +=  new MODI._IDocumentEvents_OnOCRProgressEventHandler(this.ShowProgress);
                
					// the MODI call for OCR
					_MODIDocument.OCR(_MODIParameters.Language,_MODIParameters.WithAutoRotation,_MODIParameters.WithStraightenImage);

				}
				// create document instance
				_Document = Model.Document.CreateByMODI(_MODIDocument);
				statusBar1.Text = "Ready.";
			}
			catch(Exception ee)
			{
				// simple exception "handling"
				MessageBox.Show(ee.Message);
			}
		}
	

		/// <summary>
		/// Set the image..
		/// </summary>
		/// <param name="filename"></param>
		private void SetImage(string filename)
		{
			try 
			{
				_MODIDocument = new MODI.Document();
				_MODIDocument.OnOCRProgress += new MODI._IDocumentEvents_OnOCRProgressEventHandler(this.ShowProgress);
				_MODIDocument.Create(filename);
				_Document = null;
				axMiDocView1.Document = _MODIDocument;
				axMiDocView1.Refresh();
			}
			catch(System.Runtime.InteropServices.COMException ee)
			{
				MessageBox.Show(ee.Message);
			}
		}
	
	
		private void miCopy_Click(object sender, System.EventArgs e)
		{
			// copy to clipboard..
			if (axMiDocView1.TextSelection != null)
			{
				axMiDocView1.TextSelection.CopyToClipboard();	

			}
		}

	
		private void miOCR_Click(object sender, System.EventArgs e)
		{
			// analyse document..
			if (_MODIDocument == null)
			{
				MessageBox.Show("No document selected!");
				return;
			}
			OCR();
		}

	
		private void miSave_Click(object sender, System.EventArgs e)
		{
			// save as..
			if (_MODIDocument != null)
			{
				_MODIDocument.Save();
			}
		}

	
		private void miOCRParameters_Click(object sender, System.EventArgs e)
		{
			// simple dialog to modify the MODI OCR parameters..
			// !!no OK checking provided!!
			MODISettings dialog = new MODISettings();
			dialog.Settings = _MODIParameters;
			dialog.ShowDialog();
		}
		
	
		#endregion

		#region Table extraction 

		private void miExtractTable_Click(object sender, System.EventArgs e)
		{
			AdjustExtractorFrame();
		}

		
		private void miExportTable_Click(object sender, System.EventArgs e)
		{
			ExportDocument();
		}

	
		private void InitExtractionFrame()
		{
			if (_ExtractionFrame == null || _ExtractionFrame.IsDisposed) 
			{
				_ExtractionFrame = new TableExtractorFrame(axMiDocView1,_tableRequest);
			}
		}

		
		private void AppendColumn()
		{
			AdjustExtractorFrame();
			try 
			{
				_ExtractionFrame.UpdateTableRequestByFrame();
				ColumnRequest lastCol = (ColumnRequest) _tableRequest.ColumnRequests[ _tableRequest.ColumnRequests.Count-1];
				lastCol.Width = lastCol.Width/2;
				ColumnRequest newCol = new ColumnRequest();
				newCol.Width = lastCol.Width ;
				_tableRequest.ColumnRequests.Add(newCol);
				_ExtractionFrame.AdjustColumnsToRequest();
				_ExtractionFrame.Focus();
				
			}
			catch(DocumentAreaException exc)
			{
				MessageBox.Show("Please adjust capture frame first!");
			}
		}
		
	
		private void CaptureContent()
		{
			EnsureOCR();
			if (_Document == null)
			{
				MessageBox.Show("No OCR data available!");
				return;
			}
			InitExtractionFrame();
			try 
			{
				_ExtractionFrame.UpdateTableRequestByFrame();
			
				_tableResult = _tableRequest.GetTableContent(_Document);

				if (_tableResult != null)
				{
					_tableResult.FillListViewColumns(listView1);
					_tableResult.FillListViewItems(listView1);
				}
				_ExtractionFrame.BringToFront();
				_ExtractionFrame.Show();
			}
			catch (DocumentAreaException exc)
			{
				MessageBox.Show(exc.Description);
			}		
		}
	
		private void AdjustExtractorFrame()
		{
			InitExtractionFrame();
			try 
			{
				_ExtractionFrame.UpdateTableRequestByFrame();
			}
			catch (DocumentAreaException exc)
			{
			}
			Rectangle selection = DocumentViewerSupport. GetImageSelectionAreaToScreen(axMiDocView1);
			if (selection.IsEmpty)
			{
				MessageBox.Show("Please select an area!");
				return;
			}
			_ExtractionFrame.AdjustFrameToTarget(selection);
			_ExtractionFrame.AdjustColumnsToRequest();
			_ExtractionFrame.BringToFront();
			_ExtractionFrame.Show();
			CaptureContent();
		}
		
	
		private void NewRequest()
		{
			_tableRequest.ColumnRequests.Clear();
			ColumnRequest newCol = new ColumnRequest();
			newCol.Width = 1f;
			_tableRequest.ColumnRequests.Add(newCol);
			_tableRequest.DocumentArea = null;
			if (_ExtractionFrame != null)
			_ExtractionFrame.AdjustColumnsToRequest();
			listView1.Clear();
		}

	
		private void ExportDocument()
		{
			if (_tableResult == null)
			{
				MessageBox.Show("No result to export!");
				return;
			}
			SaveFileDialog d = new SaveFileDialog();
			d.FileName = _filename + ".txt";
			if (d.ShowDialog() == DialogResult.OK)
			{
				
				_tableResult.ExportToFile(d.FileName,"\t");
			}
		}
		
		#endregion 

		private void axMiDocView1_SelectionChanged (object sender, EventArgs e)
		{
			int a;
		}

		private void listView1_Click (object sender, EventArgs e)
		{
			int a;
		}
	}


	
}
