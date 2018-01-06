namespace EverydaySolutions.C45
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param projectName="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
            this.components = new System.ComponentModel.Container ();
            this.dataGridView = new System.Windows.Forms.DataGridView ();
            this.contextMenuStripDataGridView = new System.Windows.Forms.ContextMenuStrip (this.components);
            this.newRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem ();
            this.removeRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem ();
            this.classifyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem ();
            this.addResultsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem ();
            this.addalluserresultsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem ();
            this.addallmatchinguserresultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.addallresultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog ();
            this.menuStrip = new System.Windows.Forms.MenuStrip ();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem ();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.loadFSCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.saveDebugInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.clearDebugInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.statusBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.textBoxToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem ();
            this.documentImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.actionToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem ();
            this.newRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.classifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.addResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.addAllUserResultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.aToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.addallresultcasesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.extrasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.hRTMapperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.loadFSCfromDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.loadFSCfromTwainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
            this.groupBoxStats = new System.Windows.Forms.GroupBox ();
            this.statusStrip = new System.Windows.Forms.StatusStrip ();
            this.labelState = new System.Windows.Forms.ToolStripStatusLabel ();
            this.progressBarState = new System.Windows.Forms.ToolStripProgressBar ();
            this.label9 = new System.Windows.Forms.Label ();
            this.label5 = new System.Windows.Forms.Label ();
            this.label8 = new System.Windows.Forms.Label ();
            this.label2 = new System.Windows.Forms.Label ();
            this.label7 = new System.Windows.Forms.Label ();
            this.label3 = new System.Windows.Forms.Label ();
            this.label4 = new System.Windows.Forms.Label ();
            this.label6 = new System.Windows.Forms.Label ();
            this.labelConsultationTime = new System.Windows.Forms.Label ();
            this.labelFeatureSets = new System.Windows.Forms.Label ();
            this.label1 = new System.Windows.Forms.Label ();
            this.labelDecisionTreeGenerationTime = new System.Windows.Forms.Label ();
            this.labelDecisionTreeSize = new System.Windows.Forms.Label ();
            this.listBoxStatus = new System.Windows.Forms.ListBox ();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer ();
            this.listBoxText = new System.Windows.Forms.ListBox ();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog ();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit ();
            this.contextMenuStripDataGridView.SuspendLayout ();
            this.menuStrip.SuspendLayout ();
            this.groupBoxStats.SuspendLayout ();
            this.statusStrip.SuspendLayout ();
            this.splitContainer1.Panel1.SuspendLayout ();
            this.splitContainer1.Panel2.SuspendLayout ();
            this.splitContainer1.SuspendLayout ();
            this.SuspendLayout ();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ContextMenuStrip = this.contextMenuStripDataGridView;
            this.dataGridView.Enabled = false;
            this.dataGridView.Location = new System.Drawing.Point (3, 3);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size (777, 450);
            this.dataGridView.TabIndex = 5;
            this.dataGridView.Text = "dataGridView1";
            this.dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler (this.DataGridView_RowEnter);
            this.dataGridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler (this.DataGridView_RowsAdded);
            this.dataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler (this.DataGridView_CellEndEdit);
            this.dataGridView.ColumnWidthChanged += new System.Windows.Forms.DataGridViewColumnEventHandler (this.DataGridView_ColumnWidthChanged);
            this.dataGridView.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler (this.DataGridView_RowsRemoved);
            // 
            // contextMenuStripDataGridView
            // 
            this.contextMenuStripDataGridView.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
            this.newRowToolStripMenuItem1,
            this.removeRowToolStripMenuItem1,
            this.classifyToolStripMenuItem1,
            this.addResultsToolStripMenuItem1});
            this.contextMenuStripDataGridView.Name = "contextMenuStripDataGridView";
            this.contextMenuStripDataGridView.Size = new System.Drawing.Size (151, 92);
            // 
            // newRowToolStripMenuItem1
            // 
            this.newRowToolStripMenuItem1.Enabled = false;
            this.newRowToolStripMenuItem1.Name = "newRowToolStripMenuItem1";
            this.newRowToolStripMenuItem1.Size = new System.Drawing.Size (150, 22);
            this.newRowToolStripMenuItem1.Text = "New row";
            this.newRowToolStripMenuItem1.Click += new System.EventHandler (this.NewRowToolStripMenuItem_Click);
            // 
            // removeRowToolStripMenuItem1
            // 
            this.removeRowToolStripMenuItem1.Enabled = false;
            this.removeRowToolStripMenuItem1.Name = "removeRowToolStripMenuItem1";
            this.removeRowToolStripMenuItem1.Size = new System.Drawing.Size (150, 22);
            this.removeRowToolStripMenuItem1.Text = "Remove rows";
            this.removeRowToolStripMenuItem1.Click += new System.EventHandler (this.RemoveRowsToolStripMenuItem_Click_1);
            // 
            // classifyToolStripMenuItem1
            // 
            this.classifyToolStripMenuItem1.Enabled = false;
            this.classifyToolStripMenuItem1.Name = "classifyToolStripMenuItem1";
            this.classifyToolStripMenuItem1.Size = new System.Drawing.Size (150, 22);
            this.classifyToolStripMenuItem1.Text = "Classify";
            this.classifyToolStripMenuItem1.Click += new System.EventHandler (this.ClassifyToolStripMenuItem1_Click);
            // 
            // addResultsToolStripMenuItem1
            // 
            this.addResultsToolStripMenuItem1.DropDownItems.AddRange (new System.Windows.Forms.ToolStripItem[] {
            this.addalluserresultsToolStripMenuItem1,
            this.addallmatchinguserresultsToolStripMenuItem,
            this.addallresultsToolStripMenuItem});
            this.addResultsToolStripMenuItem1.Enabled = false;
            this.addResultsToolStripMenuItem1.Name = "addResultsToolStripMenuItem1";
            this.addResultsToolStripMenuItem1.Size = new System.Drawing.Size (150, 22);
            this.addResultsToolStripMenuItem1.Text = "Add Result";
            // 
            // addalluserresultsToolStripMenuItem1
            // 
            this.addalluserresultsToolStripMenuItem1.Name = "addalluserresultsToolStripMenuItem1";
            this.addalluserresultsToolStripMenuItem1.Size = new System.Drawing.Size (222, 22);
            this.addalluserresultsToolStripMenuItem1.Text = "Add all user results";
            this.addalluserresultsToolStripMenuItem1.Click += new System.EventHandler (this.AddAllUserResultsToolStripMenuItem1_Click);
            // 
            // addallmatchinguserresultsToolStripMenuItem
            // 
            this.addallmatchinguserresultsToolStripMenuItem.Name = "addallmatchinguserresultsToolStripMenuItem";
            this.addallmatchinguserresultsToolStripMenuItem.Size = new System.Drawing.Size (222, 22);
            this.addallmatchinguserresultsToolStripMenuItem.Text = "Add all matching user results";
            this.addallmatchinguserresultsToolStripMenuItem.Click += new System.EventHandler (this.AddAllMatchingUserResultsToolStripMenuItem_Click);
            // 
            // addallresultsToolStripMenuItem
            // 
            this.addallresultsToolStripMenuItem.Name = "addallresultsToolStripMenuItem";
            this.addallresultsToolStripMenuItem.Size = new System.Drawing.Size (222, 22);
            this.addallresultsToolStripMenuItem.Text = "Add all results";
            this.addallresultsToolStripMenuItem.Click += new System.EventHandler (this.AddAllresultsToolStripMenuItem_Click_1);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.viewToolStripMenuItem,
            this.actionToolStripMenuItem1,
            this.extrasToolStripMenuItem,
            this.hRTMapperToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point (0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size (975, 24);
            this.menuStrip.TabIndex = 11;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.DropDownItems.AddRange (new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.loadFSCToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.saveDebugInfoToolStripMenuItem,
            this.clearDebugInfoToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size (35, 20);
            this.fileToolStripMenuItem1.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size (168, 22);
            this.openToolStripMenuItem.Text = "&Open Project";
            this.openToolStripMenuItem.Click += new System.EventHandler (this.OpenToolStripMenuItem_Click);
            // 
            // loadFSCToolStripMenuItem
            // 
            this.loadFSCToolStripMenuItem.Enabled = false;
            this.loadFSCToolStripMenuItem.Name = "loadFSCToolStripMenuItem";
            this.loadFSCToolStripMenuItem.Size = new System.Drawing.Size (168, 22);
            this.loadFSCToolStripMenuItem.Text = "Load FSC";
            this.loadFSCToolStripMenuItem.Click += new System.EventHandler (this.LoadFSCToolStripMenuItem_Click_1);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Enabled = false;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size (168, 22);
            this.closeToolStripMenuItem.Text = "&Close Project";
            this.closeToolStripMenuItem.Click += new System.EventHandler (this.CloseToolStripMenuItem_Click);
            // 
            // saveDebugInfoToolStripMenuItem
            // 
            this.saveDebugInfoToolStripMenuItem.Enabled = false;
            this.saveDebugInfoToolStripMenuItem.Name = "saveDebugInfoToolStripMenuItem";
            this.saveDebugInfoToolStripMenuItem.Size = new System.Drawing.Size (168, 22);
            this.saveDebugInfoToolStripMenuItem.Text = "Save Debug-Info";
            this.saveDebugInfoToolStripMenuItem.Click += new System.EventHandler (this.SaveDebugInfoToolStripMenuItem_Click);
            // 
            // clearDebugInfoToolStripMenuItem
            // 
            this.clearDebugInfoToolStripMenuItem.Enabled = false;
            this.clearDebugInfoToolStripMenuItem.Name = "clearDebugInfoToolStripMenuItem";
            this.clearDebugInfoToolStripMenuItem.Size = new System.Drawing.Size (168, 22);
            this.clearDebugInfoToolStripMenuItem.Text = "Clear Debug-Info";
            this.clearDebugInfoToolStripMenuItem.Click += new System.EventHandler (this.ClearDebugInfoToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size (168, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler (this.ExitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange (new System.Windows.Forms.ToolStripItem[] {
            this.statusBoxToolStripMenuItem,
            this.textBoxToolStripMenuItem1,
            this.documentImageToolStripMenuItem});
            this.viewToolStripMenuItem.Enabled = false;
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size (41, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // statusBoxToolStripMenuItem
            // 
            this.statusBoxToolStripMenuItem.Checked = true;
            this.statusBoxToolStripMenuItem.CheckOnClick = true;
            this.statusBoxToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.statusBoxToolStripMenuItem.Name = "statusBoxToolStripMenuItem";
            this.statusBoxToolStripMenuItem.Size = new System.Drawing.Size (166, 22);
            this.statusBoxToolStripMenuItem.Text = "State";
            this.statusBoxToolStripMenuItem.Click += new System.EventHandler (this.StatusBoxToolStripMenuItem_Click);
            // 
            // textBoxToolStripMenuItem1
            // 
            this.textBoxToolStripMenuItem1.CheckOnClick = true;
            this.textBoxToolStripMenuItem1.Name = "textBoxToolStripMenuItem1";
            this.textBoxToolStripMenuItem1.Size = new System.Drawing.Size (166, 22);
            this.textBoxToolStripMenuItem1.Text = "Document Text";
            this.textBoxToolStripMenuItem1.Click += new System.EventHandler (this.TextBoxToolStripMenuItem1_Click);
            // 
            // documentImageToolStripMenuItem
            // 
            this.documentImageToolStripMenuItem.Checked = true;
            this.documentImageToolStripMenuItem.CheckOnClick = true;
            this.documentImageToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.documentImageToolStripMenuItem.Name = "documentImageToolStripMenuItem";
            this.documentImageToolStripMenuItem.Size = new System.Drawing.Size (166, 22);
            this.documentImageToolStripMenuItem.Text = "Document Image";
            this.documentImageToolStripMenuItem.Click += new System.EventHandler (this.DocumentImageToolStripMenuItem_Click);
            // 
            // actionToolStripMenuItem1
            // 
            this.actionToolStripMenuItem1.DropDownItems.AddRange (new System.Windows.Forms.ToolStripItem[] {
            this.newRowToolStripMenuItem,
            this.removeRowToolStripMenuItem,
            this.classifyToolStripMenuItem,
            this.addResultsToolStripMenuItem});
            this.actionToolStripMenuItem1.Enabled = false;
            this.actionToolStripMenuItem1.Name = "actionToolStripMenuItem1";
            this.actionToolStripMenuItem1.Size = new System.Drawing.Size (49, 20);
            this.actionToolStripMenuItem1.Text = "&Action";
            // 
            // newRowToolStripMenuItem
            // 
            this.newRowToolStripMenuItem.Enabled = false;
            this.newRowToolStripMenuItem.Name = "newRowToolStripMenuItem";
            this.newRowToolStripMenuItem.Size = new System.Drawing.Size (150, 22);
            this.newRowToolStripMenuItem.Text = "&New row";
            this.newRowToolStripMenuItem.Click += new System.EventHandler (this.NewCaseToolStripMenuItem_Click);
            // 
            // removeRowToolStripMenuItem
            // 
            this.removeRowToolStripMenuItem.Enabled = false;
            this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
            this.removeRowToolStripMenuItem.Size = new System.Drawing.Size (150, 22);
            this.removeRowToolStripMenuItem.Text = "Remove rows";
            this.removeRowToolStripMenuItem.Click += new System.EventHandler (this.RemoveRowsToolStripMenuItem_Click);
            // 
            // classifyToolStripMenuItem
            // 
            this.classifyToolStripMenuItem.Enabled = false;
            this.classifyToolStripMenuItem.Name = "classifyToolStripMenuItem";
            this.classifyToolStripMenuItem.Size = new System.Drawing.Size (150, 22);
            this.classifyToolStripMenuItem.Text = "&Classify";
            this.classifyToolStripMenuItem.Click += new System.EventHandler (this.ClassifyToolStripMenuItem_Click);
            // 
            // addResultsToolStripMenuItem
            // 
            this.addResultsToolStripMenuItem.DropDownItems.AddRange (new System.Windows.Forms.ToolStripItem[] {
            this.addAllUserResultsToolStripMenuItem,
            this.aToolStripMenuItem,
            this.addallresultcasesToolStripMenuItem});
            this.addResultsToolStripMenuItem.Enabled = false;
            this.addResultsToolStripMenuItem.Name = "addResultsToolStripMenuItem";
            this.addResultsToolStripMenuItem.Size = new System.Drawing.Size (150, 22);
            this.addResultsToolStripMenuItem.Text = "Add FSC";
            // 
            // addAllUserResultsToolStripMenuItem
            // 
            this.addAllUserResultsToolStripMenuItem.Name = "addAllUserResultsToolStripMenuItem";
            this.addAllUserResultsToolStripMenuItem.Size = new System.Drawing.Size (222, 22);
            this.addAllUserResultsToolStripMenuItem.Text = "Add all user result";
            this.addAllUserResultsToolStripMenuItem.Click += new System.EventHandler (this.AddAllUserResultsToolStripMenuItem_Click);
            // 
            // aToolStripMenuItem
            // 
            this.aToolStripMenuItem.Name = "aToolStripMenuItem";
            this.aToolStripMenuItem.Size = new System.Drawing.Size (222, 22);
            this.aToolStripMenuItem.Text = "Add all matching user results";
            this.aToolStripMenuItem.Click += new System.EventHandler (this.AddAllMatchingResultsToolStripMenuItem_Click);
            // 
            // addallresultcasesToolStripMenuItem
            // 
            this.addallresultcasesToolStripMenuItem.Name = "addallresultcasesToolStripMenuItem";
            this.addallresultcasesToolStripMenuItem.Size = new System.Drawing.Size (222, 22);
            this.addallresultcasesToolStripMenuItem.Text = "Add all results";
            this.addallresultcasesToolStripMenuItem.Click += new System.EventHandler (this.AddAllResultsToolStripMenuItem_Click);
            // 
            // extrasToolStripMenuItem
            // 
            this.extrasToolStripMenuItem.DropDownItems.AddRange (new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.extrasToolStripMenuItem.Enabled = false;
            this.extrasToolStripMenuItem.Name = "extrasToolStripMenuItem";
            this.extrasToolStripMenuItem.Size = new System.Drawing.Size (50, 20);
            this.extrasToolStripMenuItem.Text = "&Extras";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size (122, 22);
            this.optionsToolStripMenuItem.Text = "&Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler (this.OptionsToolStripMenuItem_Click);
            // 
            // hRTMapperToolStripMenuItem
            // 
            this.hRTMapperToolStripMenuItem.DropDownItems.AddRange (new System.Windows.Forms.ToolStripItem[] {
            this.loadFSCfromDocumentToolStripMenuItem,
            this.loadFSCfromTwainToolStripMenuItem});
            this.hRTMapperToolStripMenuItem.Name = "hRTMapperToolStripMenuItem";
            this.hRTMapperToolStripMenuItem.Size = new System.Drawing.Size (79, 20);
            this.hRTMapperToolStripMenuItem.Text = "&HRT-Mapper";
            this.hRTMapperToolStripMenuItem.Visible = false;
            // 
            // loadFSCfromDocumentToolStripMenuItem
            // 
            this.loadFSCfromDocumentToolStripMenuItem.Name = "loadFSCfromDocumentToolStripMenuItem";
            this.loadFSCfromDocumentToolStripMenuItem.Size = new System.Drawing.Size (186, 22);
            this.loadFSCfromDocumentToolStripMenuItem.Text = "Load FSC from &File";
            this.loadFSCfromDocumentToolStripMenuItem.Click += new System.EventHandler (this.LoadFSCfromDocumentToolStripMenuItem_Click);
            // 
            // loadFSCfromTwainToolStripMenuItem
            // 
            this.loadFSCfromTwainToolStripMenuItem.Name = "loadFSCfromTwainToolStripMenuItem";
            this.loadFSCfromTwainToolStripMenuItem.Size = new System.Drawing.Size (186, 22);
            this.loadFSCfromTwainToolStripMenuItem.Text = "Load FSC from &Twain";
            // 
            // groupBoxStats
            // 
            this.groupBoxStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxStats.Controls.Add (this.statusStrip);
            this.groupBoxStats.Controls.Add (this.label9);
            this.groupBoxStats.Controls.Add (this.label5);
            this.groupBoxStats.Controls.Add (this.label8);
            this.groupBoxStats.Controls.Add (this.label2);
            this.groupBoxStats.Controls.Add (this.label7);
            this.groupBoxStats.Controls.Add (this.label3);
            this.groupBoxStats.Controls.Add (this.label4);
            this.groupBoxStats.Controls.Add (this.label6);
            this.groupBoxStats.Controls.Add (this.labelConsultationTime);
            this.groupBoxStats.Controls.Add (this.labelFeatureSets);
            this.groupBoxStats.Controls.Add (this.label1);
            this.groupBoxStats.Controls.Add (this.labelDecisionTreeGenerationTime);
            this.groupBoxStats.Controls.Add (this.labelDecisionTreeSize);
            this.groupBoxStats.Location = new System.Drawing.Point (0, 459);
            this.groupBoxStats.Name = "groupBoxStats";
            this.groupBoxStats.Size = new System.Drawing.Size (783, 83);
            this.groupBoxStats.TabIndex = 12;
            this.groupBoxStats.TabStop = false;
            this.groupBoxStats.Text = "Project";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
            this.labelState,
            this.progressBarState});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip.Location = new System.Drawing.Point (3, 59);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size (777, 21);
            this.statusStrip.TabIndex = 19;
            this.statusStrip.Text = "statusStrip1";
            // 
            // labelState
            // 
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size (38, 13);
            this.labelState.Text = "Ready";
            // 
            // progressBarState
            // 
            this.progressBarState.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.progressBarState.Name = "progressBarState";
            this.progressBarState.Size = new System.Drawing.Size (100, 15);
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.Location = new System.Drawing.Point (9, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size (61, 18);
            this.label9.TabIndex = 17;
            this.label9.Text = "Featuresets";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.Location = new System.Drawing.Point (9, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size (61, 18);
            this.label5.TabIndex = 17;
            this.label5.Text = "Featuresets";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.Location = new System.Drawing.Point (9, 14);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size (61, 18);
            this.label8.TabIndex = 11;
            this.label8.Text = "Tree size";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.Location = new System.Drawing.Point (9, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size (61, 18);
            this.label2.TabIndex = 11;
            this.label2.Text = "Tree size";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.Location = new System.Drawing.Point (134, 33);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size (136, 18);
            this.label7.TabIndex = 12;
            this.label7.Text = "Generation Time";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.Location = new System.Drawing.Point (134, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size (136, 18);
            this.label3.TabIndex = 13;
            this.label3.Text = "Consultation Time (Avg)";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.Location = new System.Drawing.Point (134, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size (136, 18);
            this.label4.TabIndex = 12;
            this.label4.Text = "Generation Time";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.Location = new System.Drawing.Point (76, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size (52, 18);
            this.label6.TabIndex = 18;
            this.label6.Text = "0";
            // 
            // labelConsultationTime
            // 
            this.labelConsultationTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelConsultationTime.Location = new System.Drawing.Point (270, 15);
            this.labelConsultationTime.Name = "labelConsultationTime";
            this.labelConsultationTime.Size = new System.Drawing.Size (53, 18);
            this.labelConsultationTime.TabIndex = 16;
            this.labelConsultationTime.Text = "0";
            // 
            // labelFeatureSets
            // 
            this.labelFeatureSets.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelFeatureSets.Location = new System.Drawing.Point (76, 38);
            this.labelFeatureSets.Name = "labelFeatureSets";
            this.labelFeatureSets.Size = new System.Drawing.Size (52, 18);
            this.labelFeatureSets.TabIndex = 18;
            this.labelFeatureSets.Text = "0";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.Location = new System.Drawing.Point (76, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size (52, 18);
            this.label1.TabIndex = 14;
            this.label1.Text = "0";
            // 
            // labelDecisionTreeGenerationTime
            // 
            this.labelDecisionTreeGenerationTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelDecisionTreeGenerationTime.Location = new System.Drawing.Point (270, 34);
            this.labelDecisionTreeGenerationTime.Name = "labelDecisionTreeGenerationTime";
            this.labelDecisionTreeGenerationTime.Size = new System.Drawing.Size (53, 18);
            this.labelDecisionTreeGenerationTime.TabIndex = 15;
            this.labelDecisionTreeGenerationTime.Text = "0";
            // 
            // labelDecisionTreeSize
            // 
            this.labelDecisionTreeSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelDecisionTreeSize.Location = new System.Drawing.Point (76, 20);
            this.labelDecisionTreeSize.Name = "labelDecisionTreeSize";
            this.labelDecisionTreeSize.Size = new System.Drawing.Size (52, 18);
            this.labelDecisionTreeSize.TabIndex = 14;
            this.labelDecisionTreeSize.Text = "0";
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxStatus.FormattingEnabled = true;
            this.listBoxStatus.HorizontalScrollbar = true;
            this.listBoxStatus.Location = new System.Drawing.Point (0, 0);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.Size = new System.Drawing.Size (188, 537);
            this.listBoxStatus.TabIndex = 13;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point (0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add (this.dataGridView);
            this.splitContainer1.Panel1.Controls.Add (this.groupBoxStats);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add (this.listBoxText);
            this.splitContainer1.Panel2.Controls.Add (this.listBoxStatus);
            this.splitContainer1.Size = new System.Drawing.Size (975, 542);
            this.splitContainer1.SplitterDistance = 783;
            this.splitContainer1.TabIndex = 14;
            this.splitContainer1.Text = "splitContainer1";
            // 
            // listBoxText
            // 
            this.listBoxText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxText.FormattingEnabled = true;
            this.listBoxText.HorizontalScrollbar = true;
            this.listBoxText.Location = new System.Drawing.Point (0, 0);
            this.listBoxText.Name = "listBoxText";
            this.listBoxText.Size = new System.Drawing.Size (188, 537);
            this.listBoxText.TabIndex = 14;
            this.listBoxText.Visible = false;
            this.listBoxText.SelectedIndexChanged += new System.EventHandler (this.ListBoxText_SelectedIndexChanged);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Text files|*.txt|All files|*.*";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size (975, 566);
            this.Controls.Add (this.splitContainer1);
            this.Controls.Add (this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "C45DataManager";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit ();
            this.contextMenuStripDataGridView.ResumeLayout (false);
            this.menuStrip.ResumeLayout (false);
            this.menuStrip.PerformLayout ();
            this.groupBoxStats.ResumeLayout (false);
            this.groupBoxStats.PerformLayout ();
            this.statusStrip.ResumeLayout (false);
            this.statusStrip.PerformLayout ();
            this.splitContainer1.Panel1.ResumeLayout (false);
            this.splitContainer1.Panel2.ResumeLayout (false);
            this.splitContainer1.ResumeLayout (false);
            this.ResumeLayout (false);
            this.PerformLayout ();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadFSCToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem newRowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeRowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem classifyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addResultsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addAllUserResultsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addallresultcasesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem extrasToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
		private System.Windows.Forms.GroupBox groupBoxStats;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripDataGridView;
		private System.Windows.Forms.ToolStripMenuItem newRowToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem classifyToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem addResultsToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem addalluserresultsToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem addallmatchinguserresultsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addallresultsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem removeRowToolStripMenuItem1;
		private System.Windows.Forms.ListBox listBoxStatus;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStripMenuItem saveDebugInfoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clearDebugInfoToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelConsultationTime;
		private System.Windows.Forms.Label labelFeatureSets;
		private System.Windows.Forms.Label labelDecisionTreeGenerationTime;
		private System.Windows.Forms.Label labelDecisionTreeSize;
		private System.Windows.Forms.ToolStripMenuItem hRTMapperToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadFSCfromDocumentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadFSCfromTwainToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripStatusLabel labelState;
        private System.Windows.Forms.ToolStripProgressBar progressBarState;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusBoxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textBoxToolStripMenuItem1;
        private System.Windows.Forms.ListBox listBoxText;
        private System.Windows.Forms.ToolStripMenuItem documentImageToolStripMenuItem;
	}
}

