using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using EverydaySolutions.C45;
using EverydaySolutions.Net;
using EverydaySolutions.Diagnostic.Sniffer;
using EverydaySolutions.Documenting;

namespace EverydaySolutions.C45
{
    #region MainForm
    public partial class MainForm : Form
    {
        #region Attributes
        int userClassColumn = 0;
        int resultClassColumn = 0;
        int stateColumn = 0;
        C45Settings c45Settings = null;
        C45FeatureSetCollection featureSetCollection = new C45FeatureSetCollection ();
        C45Schema schema = null;
        Sniffer sniffer = new Sniffer ();
        C45Project project = null;
        DocumentImageForm docImageForm = new DocumentImageForm ();
        HRTFeaturesSetGenerator generator = null;
        C45ManagerConfig formConfig = C45ManagerConfig.CreateInstance ();
        #endregion
        
        #region MainForm
        public MainForm ()
        {
            InitializeComponent ();
            this.SetEventHandler ();
            this.docImageForm.Hide ();
            this.LoadFormAppearance ();
        }
        #endregion

        #region Application_ApplicationExit
        void Application_ApplicationExit (
            object sender, 
            EventArgs e)
        {
            this.QuitApplication ();
        }
        #endregion

        #region LoadFormAppearance
        private void LoadFormAppearance ()
        {
            this.Size = this.formConfig.FrmSize;
            this.StartPosition = this.formConfig.FrmStartPosition;
            this.WindowState = this.formConfig.FrmWindowState;
        }
        #endregion

        #region SetEventHandler
        private void SetEventHandler ()
        {
            this.featureSetCollection.ClassificationComplete += new EventHandler (FeatureSetCollection_ClassificationFinished);
            this.featureSetCollection.DecisionTreeGenerationStarted += new DecisionTreeGenerationStartedEventHandler (FeatureSetCollection_DecisionTreeGenerationStarted);
            this.featureSetCollection.DecisionTreeGenerationFinished += new DecisionTreeGenerationFinishedEventHandler (FeatureSetCollection_DecisionTreeGenerationFinished);
            this.featureSetCollection.ConsultationStarted += new ConsultationStartedEventHandler (FeatureSetCollection_ConsultationStarted);
            this.featureSetCollection.ConsultationFinished += new ConsultationFinishedEventHandler (FeatureSetCollection_ConsultationFinished);
            this.featureSetCollection.SingleFeatureClassified += new SingleFeatureClassifiedEventHandler (FeatureSetCollection_SingleFeatureClassified);

            this.sniffer.NewMessage += new NewMessageEventHandler (NewDebugMessage);
            Application.ApplicationExit += new EventHandler (Application_ApplicationExit);            
        }
        #endregion

        #region FeatureSetCollection_SingleFeatureClassified
        void FeatureSetCollection_SingleFeatureClassified (
            object sender,
            C45SingleFeatureSetClassifiedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke (new SingleFeatureClassifiedEventHandler (FeatureSetCollection_SingleFeatureClassified),
                    new object[] { sender, e });
            }
            else
            {
                this.progressBarState.PerformStep ();
                int totalConsultationtime = Convert.ToInt32 (this.labelFeatureSets.Text) * Convert.ToInt32 (this.labelConsultationTime.Text);
                totalConsultationtime += (int)e.Statistics.ConsultationTime;
                this.labelFeatureSets.Text = (Convert.ToInt32 (this.labelFeatureSets.Text) + 1).ToString ();
                this.labelConsultationTime.Text = (totalConsultationtime / Convert.ToInt32 (this.labelFeatureSets.Text)).ToString ();
                this.labelDecisionTreeSize.Text = e.Statistics.TreeItems.ToString ();
                this.labelDecisionTreeGenerationTime.Text = e.Statistics.DecisionTreeGenerationTime.ToString ();
                this.statusStrip.Text = "Single featureset classified";
            }
        }
        #endregion

        #region FeatureSetCollection_DecisionTreeGenerationStarted
        void FeatureSetCollection_DecisionTreeGenerationStarted (
            object sender,
            EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke (
                    new DecisionTreeGenerationStartedEventHandler (FeatureSetCollection_DecisionTreeGenerationStarted),
                    new object[] { sender, e });
            }
            else
            {
                this.progressBarState.PerformStep ();
                this.statusStrip.Text = "Decisiontree generation started";
            }
        }
        #endregion

        #region FeatureSetCollection_DecisionTreeGenerationFinished
        void FeatureSetCollection_DecisionTreeGenerationFinished (
            object sender,
            EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke (
                    new DecisionTreeGenerationFinishedEventHandler (FeatureSetCollection_DecisionTreeGenerationFinished),
                    new object[] { sender, e });
            }
            else
            {
                this.progressBarState.PerformStep ();
                this.statusStrip.Text = "Decisiontree generation finished";
            }
        }
        #endregion

        #region FeatureSetCollection_ConsultationStarted
        void FeatureSetCollection_ConsultationStarted (
            object sender,
            EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke (
                    new ConsultationStartedEventHandler (FeatureSetCollection_ConsultationStarted),
                    new object[] { sender, e });
            }
            else
            {
                this.progressBarState.PerformStep ();
                this.statusStrip.Text = "Consultation started";
            }
        }
        #endregion

        #region FeatureSetCollection_ConsultationFinished
        void FeatureSetCollection_ConsultationFinished (
            object sender,
            EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke (
                    new ConsultationFinishedEventHandler (FeatureSetCollection_ConsultationStarted),
                    new object[] { sender, e });
            }
            else
            {
                this.progressBarState.PerformStep ();
                this.statusStrip.Text = "Consultation finished";
            }
        }
        #endregion

        #region ButtonSelectFile_Click
        private void ButtonSelectFile_Click (
            object sender,
            EventArgs e)
        {
            this.openFileDialog.FileName = "";
            this.openFileDialog.ShowDialog ();

            if (openFileDialog.FileName == String.Empty)
            {
                String Filename = System.IO.Path.GetFileNameWithoutExtension (this.openFileDialog.FileName);
                String Path = System.IO.Path.GetDirectoryName (this.openFileDialog.FileName);

                this.groupBoxStats.Text = Path + @"\" + Filename;

                CreateColumns ();

                this.openToolStripMenuItem.Enabled = false;
                this.newRowToolStripMenuItem.Enabled = true;
                this.loadFSCToolStripMenuItem.Enabled = true;
                this.closeToolStripMenuItem.Enabled = true;
            }
        }
        #endregion

        #region CreateColumns
        private void CreateColumns ()
        {
            try
            {
                this.schema = C45Schema.CreateFromFile (c45Settings.C45SchemaFilename + @".names");
                int index = 0;
                foreach (C45SchemaProperty property in schema.Properties)
                {
                    if (property.IsDiscrete)
                    {
                        DataGridViewComboBoxColumn combo = new DataGridViewComboBoxColumn ();
                        combo.Name = property.Name;
                        combo.HeaderText = property.Name;

                        combo.Items.Add ("?".ToString ());
                        foreach (String attribute in property.DiscreteAttributes)
                        {
                            combo.Items.Add (attribute);
                        }
                        this.dataGridView.Columns.Add (combo);
                    }
                    else
                    {
                        DataGridViewTextBoxColumn text = new DataGridViewTextBoxColumn ();
                        text.Name = property.Name;
                        text.HeaderText = property.Name;

                        this.dataGridView.Columns.Add (text);
                    }

                    this.dataGridView.Columns[index].Tag = property;
                    index++;
                }

                this.userClassColumn = index++;
                this.resultClassColumn = index++;
                this.stateColumn = index;

                AddClassesToGridView (schema.Classes);
                AddResultColumn ();
            }
            catch (C45Exception ex)
            {
                MessageBox.Show (ex.GetString ());
                CloseToolStripMenuItem_Click (this, new EventArgs ());
            }
        }
        #endregion

        #region AddResultColumn
        private void AddResultColumn ()
        {
            DataGridViewComboBoxColumn resultColumn = new DataGridViewComboBoxColumn ();
            resultColumn.Name = "Result class";
            resultColumn.HeaderText = "Result class";
            DataGridViewComboBoxCell resultCellTemplate = new DataGridViewComboBoxCell ();
            resultColumn.CellTemplate = resultCellTemplate;
            this.dataGridView.Columns.Add (resultColumn);
        }
        #endregion

        #region AddClassesToGridView
        private void AddClassesToGridView (List<String> classes)
        {
            DataGridViewComboBoxColumn classColumm = new DataGridViewComboBoxColumn ();
            classColumm.Name = "User class";
            classColumm.HeaderText = "User class";

            foreach (String classElement in classes)
            {
                classColumm.Items.Add (classElement);
            }

            this.dataGridView.Columns.Add (classColumm);

            this.dataGridView.Columns[this.userClassColumn].Tag = classes;
        }
        #endregion

        #region AddResultClassesToGridView
        private void AddResultClassesToGridView (int column)
        {
            int index = 0;

            foreach (C45FeatureSet featureSet in this.featureSetCollection)
            {
                if (featureSet.IsClassified)
                {
                    DataGridViewComboBoxCell resultCell = new DataGridViewComboBoxCell ();
                    resultCell.Tag = featureSet.ComputedResult;

                    foreach (C45ComputedResult result in featureSet.ComputedResult)
                    {
                        resultCell.Items.Add (result.ToValueString ());
                        resultCell.DropDownWidth = result.ToValueString ().Length * 10;

                        if (this.dataGridView.Rows[index].Cells[this.userClassColumn].Value != null
                            && featureSet.ComputedResult.GetBestResult ().ResultClassName == this.dataGridView.Rows[index].Cells[this.userClassColumn].Value.ToString ())
                        {
                            resultCell.Style.BackColor = Color.Green;
                        }
                        else
                        {
                            resultCell.Style.BackColor = Color.Red;
                        }
                    }
                    resultCell.Value = featureSet.ComputedResult.GetBestResult ().ToValueString ();
                    this.dataGridView.Rows[index].Cells[column] = resultCell;
                }
                index++;
            }
        }
        #endregion

        #region GetFeatureSetFromGrid
        private C45FeatureSet GetFeatureSetFromGrid (DataGridViewRow row)
        {
            C45FeatureSet newSet = new C45FeatureSet ();

            foreach (DataGridViewCell cell in row.Cells)
            {
                this.AppendFeatureValueFromGridView (cell, newSet);
            }
            newSet.IsMarked = row.Selected;
            newSet.UserResult.ClassName = row.Cells[this.userClassColumn].FormattedValue.ToString ();

            return newSet;
        }
        #endregion

        #region AppendFeatureValueFromGridView
        private void AppendFeatureValueFromGridView (
            DataGridViewCell cell,
            C45FeatureSet featureSet)
        {
            if (cell.ColumnIndex < this.schema.Properties.Count)
            {
                String cellValue = String.Empty;

                if (cell.IsInEditMode)
                {
                    cellValue = cell.EditedFormattedValue.ToString ();
                }
                else
                {
                    cellValue = cell.FormattedValue.ToString ();
                }

                C45SchemaProperty col = (C45SchemaProperty)this.dataGridView.Columns[cell.ColumnIndex].Tag;

                if (cellValue == "?".ToString ()
                    || cellValue == "".ToString ())
                {
                    featureSet.AddUnknownFeature ();
                }
                else
                {
                    if (col.IsDiscrete)
                    {
                        // Just one attribute with 100 % probability supported, yet. 
                        // Multiple attribute has to be implemented in future.

                        C45DiscreteFeature discreteFeature = new C45DiscreteFeature ();
                        C45DiscreteFeatureAttribute discreteFeatureAttribute = new C45DiscreteFeatureAttribute ();

                        discreteFeatureAttribute.AttributeName = (String)cellValue;
                        discreteFeatureAttribute.Probability = 100;

                        discreteFeature.Add (discreteFeatureAttribute);

                        featureSet.AddDiscreteFeature (discreteFeature);
                    }
                    else
                    {
                        // Just a fix continous value or ignore-value supported, yet.
                        // Lower and upper bound has to be implemented in future.

                        C45ContinuousFeature continuousFeature = new C45ContinuousFeature ();
                        continuousFeature.LowerBound = Convert.ToInt32 (cellValue);
                        continuousFeature.UpperBound = Convert.ToInt32 (cellValue);

                        featureSet.AddContinousFeature (continuousFeature);
                    }
                }
            }
        }
        #endregion

        #region OpenToolStripMenuItem_Click
        private void OpenToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            this.openFileDialog.FileName = String.Empty;
            this.openFileDialog.Filter = C45Constants.FILTER_C45PROJECTS;
            this.openFileDialog.ShowDialog ();

            try
            {                
                this.project = C45Project.CreateInstance (this.openFileDialog.FileName);
                this.c45Settings = C45Settings.CreateFromFile (project.C45ConfigFile);

                if (project.ProjectType == C45ProjectType.HRT)
                {
                    hRTMapperToolStripMenuItem.Visible = true;
                }
                else
                {
                    hRTMapperToolStripMenuItem.Visible = false;
                }

                this.c45Settings.C45SchemaFilename = Path.GetFileNameWithoutExtension (project.SchemaFile);
                this.groupBoxStats.Text = project.ProjectName;

                this.CreateColumns ();
                this.AcquireColumnWidth ();
                this.EnableControls ();
            }
            catch (Exception ex)
            {
                MessageBox.Show (ex.Message);
            }
        }
        #endregion

        #region EnableControls
        private void EnableControls ()
        {
            this.openToolStripMenuItem.Enabled = false;
            this.newRowToolStripMenuItem.Enabled = true;
            this.loadFSCToolStripMenuItem.Enabled = true;
            this.removeRowToolStripMenuItem.Enabled = true;
            this.closeToolStripMenuItem.Enabled = true;
            this.dataGridView.Enabled = true;
            this.newRowToolStripMenuItem1.Enabled = true;
            this.viewToolStripMenuItem.Enabled = true;
            this.actionToolStripMenuItem1.Enabled = true;
            this.extrasToolStripMenuItem.Enabled = true;

            this.statusBoxToolStripMenuItem.CheckState = this.formConfig.StatusBox;
            this.textBoxToolStripMenuItem1.CheckState = this.formConfig.TextBox;
            this.documentImageToolStripMenuItem.CheckState = this.formConfig.DocumentImage;        
        }
        #endregion

        #region AcquireColumnWidth
        private void AcquireColumnWidth ()
        {
            for (int columnIndex = 0; columnIndex < this.dataGridView.Columns.Count; columnIndex++)
            {
                if (this.project.ColumnWidth.Count > columnIndex + 1)
                {
                    this.dataGridView.Columns[columnIndex].Width = this.project.ColumnWidth[columnIndex];
                }
                else
                {
                    this.project.ColumnWidth.Add (this.dataGridView.Columns[columnIndex].Width);
                }
            }
        }
        #endregion

        #region NewCaseToolStripMenuItem_Click
        private void NewCaseToolStripMenuItem_Click (object sender, EventArgs e)
        {
            this.dataGridView.Rows.Add ();
            this.featureSetCollection.Add (this.GetFeatureSetFromGrid (this.dataGridView.Rows[this.dataGridView.Rows.Count - 1]));
        }
        #endregion

        #region CloseToolStripMenuItem_Click
        private void CloseToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            DisableControls ();
        }
        #endregion

        #region DisableControls
        private void DisableControls ()
        {
            this.dataGridView.Rows.Clear ();
            this.dataGridView.Columns.Clear ();
            this.schema = null;
            this.generator = null;
            this.docImageForm = null;

            this.openToolStripMenuItem.Enabled = true;
            this.newRowToolStripMenuItem.Enabled = false;
            this.loadFSCToolStripMenuItem.Enabled = false;
            this.removeRowToolStripMenuItem.Enabled = false;
            this.addallresultcasesToolStripMenuItem.Enabled = false;
            this.addAllUserResultsToolStripMenuItem.Enabled = false;
            this.addResultsToolStripMenuItem.Enabled = false;
            this.classifyToolStripMenuItem.Enabled = false;
            this.closeToolStripMenuItem.Enabled = false;
            this.removeRowToolStripMenuItem1.Enabled = false;
            this.removeRowToolStripMenuItem.Enabled = false;
            this.saveDebugInfoToolStripMenuItem.Enabled = false;
            this.clearDebugInfoToolStripMenuItem.Enabled = false;
            this.viewToolStripMenuItem.Enabled = false;
            this.extrasToolStripMenuItem.Enabled = false;
            this.actionToolStripMenuItem1.Enabled = false;
            this.hRTMapperToolStripMenuItem.Visible = false;
            this.listBoxStatus.Enabled = true;
            this.listBoxStatus.Items.Clear ();
            this.progressBarState.Value = 0;
        }
        #endregion
        
        #region RemoveRowsToolStripMenuItem_Click
        private void RemoveRowsToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            foreach (DataGridViewRow row in this.dataGridView.SelectedRows)
            {
                this.featureSetCollection.Remove (this.featureSetCollection[row.Index]);
                this.dataGridView.Rows.Remove (row);
            }
        }
        #endregion

        #region ClassifyToolStripMenuItem_Click
        private void ClassifyToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count <= 0)
            {
                MessageBox.Show ("No rows selected");
            }
            else
            {
                this.progressBarState.Maximum = 4 * this.dataGridView.SelectedRows.Count;
                this.progressBarState.Step = 1;
                this.progressBarState.Value = 1;

                foreach (DataGridViewRow row in this.dataGridView.Rows)
                {
                    this.featureSetCollection[row.Index].IsMarked = row.Selected;
                }

                this.featureSetCollection.StartClassify ();
            }
        }
        #endregion

        #region AddAllUserResultsToolStripMenuItem_Click
        private void AddAllUserResultsToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count <= 0)
            {
                MessageBox.Show ("No rows selected.");
            }
            else
            {
                try
                {
                    if (CheckRows ())
                    {
                        foreach (DataGridViewRow row in this.dataGridView.Rows)
                        {
                            this.featureSetCollection[row.Index].IsMarked = row.Selected;
                        }
                        this.featureSetCollection.AddUserResults ();
                        MessageBox.Show ("Results added successfully");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.Message + @" / " + ex.InnerException);
                }
            }
        }
        #endregion

        #region CheckRows
        private bool CheckRows ()
        {
            bool returnValue = false;
            int errors = 0;

            foreach (DataGridViewRow row in this.dataGridView.SelectedRows)
            {
                if (this.featureSetCollection[row.Index].UserResult == null
                    || this.featureSetCollection[row.Index].UserResult.ClassName == String.Empty)
                {
                    errors++;
                }
            }

            if (errors > 0)
            {
                MessageBox.Show ("One or more user results missing");
            }
            else
            {
                returnValue = true;
            }

            return returnValue;
        }
        #endregion

        #region OptionsToolStripMenuItem_Click
        private void OptionsToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            C45InteropConfiguration config = new C45InteropConfiguration (this.c45Settings, null);
            if (config.Changed)
            {
                this.c45Settings = config.Settings;
                this.c45Settings.Save ();
            }
        }
        #endregion

        #region NewRowToolStripMenuItem_Click
        private void NewRowToolStripMenuItem_Click (object sender, EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count > 0)
            {
                this.dataGridView.Rows.Insert (this.dataGridView.SelectedRows[this.dataGridView.SelectedRows.Count - 1].Index + 1);
            }
            else
            {
                this.dataGridView.Rows.Add ();
            }
            this.dataGridView.Focus ();
        }
        #endregion

        #region DataGridView_RowsAdded
        private void DataGridView_RowsAdded (
            object sender,
            DataGridViewRowsAddedEventArgs e)
        {
            this.classifyToolStripMenuItem.Enabled = true;
            this.addResultsToolStripMenuItem.Enabled = true;
            this.removeRowToolStripMenuItem.Enabled = true;

            this.classifyToolStripMenuItem1.Enabled = true;
            this.addResultsToolStripMenuItem1.Enabled = true;
            this.removeRowToolStripMenuItem1.Enabled = true;
        }
        #endregion

        #region ClassifyToolStripMenuItem1_Click
        private void ClassifyToolStripMenuItem1_Click (
            object sender,
            EventArgs e)
        {
            this.ClassifyToolStripMenuItem_Click (sender, e);
        }
        #endregion

        #region AddAllUserResultsToolStripMenuItem1_Click
        private void AddAllUserResultsToolStripMenuItem1_Click (
            object sender,
            EventArgs e)
        {
            this.AddAllUserResultsToolStripMenuItem_Click (sender, e);
        }
        #endregion

        #region AddAllMatchingResultsToolStripMenuItem_Click
        private void AddAllMatchingResultsToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count <= 0)
            {
                MessageBox.Show ("No rows selected.");
            }
            else
            {
                try
                {
                    if (this.CheckRows ())
                    {
                        foreach (DataGridViewRow row in this.dataGridView.Rows)
                        {
                            //If user result == computed result
                            if (((C45ComputedResultList)(row.Cells[this.resultClassColumn].Tag)).GetBestResult ().ResultClassName
                                    == row.Cells[this.userClassColumn].FormattedValue.ToString ())
                            {
                                this.featureSetCollection[row.Index].IsMarked = true;
                            }
                            else
                            {
                                this.featureSetCollection[row.Index].IsMarked = false;
                            }
                        }
                        this.featureSetCollection.AddComputedResults ();
                        MessageBox.Show ("Results added successfully");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.Message + @" / " + ex.InnerException);
                }
            }
        }
        #endregion

        #region AddAllResultsToolStripMenuItem_Click
        private void AddAllResultsToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count <= 0)
            {
                MessageBox.Show ("No rows selected.");
            }
            else
            {
                try
                {
                    foreach (DataGridViewRow row in this.dataGridView.SelectedRows)
                    {
                        this.featureSetCollection[row.Index].IsMarked = row.Selected;
                    }
                    this.featureSetCollection.AddComputedResults ();
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.Message + @" / " + ex.InnerException);
                }
                MessageBox.Show ("Results added successfully");
            }
        }
        #endregion

        #region AddAllMatchingUserResultsToolStripMenuItem_Click
        private void AddAllMatchingUserResultsToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            this.AddAllMatchingResultsToolStripMenuItem_Click (sender, e);
        }
        #endregion

        #region DataGridView_CellEndEdit
        private void DataGridView_CellEndEdit (
            object sender,
            DataGridViewCellEventArgs e)
        {
            this.featureSetCollection[e.RowIndex].IsDirty = true;
            this.featureSetCollection[e.RowIndex] = this.GetFeatureSetFromGrid (this.dataGridView.Rows[e.RowIndex]);
        }
        #endregion

        #region RemoveRowsToolStripMenuItem_Click_1
        private void RemoveRowsToolStripMenuItem_Click_1 (
            object sender,
            EventArgs e)
        {
            RemoveRowsToolStripMenuItem_Click (sender, e);
        }
        #endregion

        #region NewDebugMessage
        private void NewDebugMessage (
            object sender,
            NewMessageEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke (new NewMessageEventHandler (NewDebugMessage),
                    new object[] { sender, e });
            }
            else
            {
                try
                {
                    this.listBoxStatus.Items.Add (e.Package.PayloadElement.SelectSingleNode (@"//tracepackage/data").InnerText);
                    saveDebugInfoToolStripMenuItem.Enabled = true;
                    clearDebugInfoToolStripMenuItem.Enabled = true;
                    this.listBoxStatus.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show (ex.Message);
                }
            }
        }
        #endregion

        #region FeatureSetCollection_ClassificationFinished
        private void FeatureSetCollection_ClassificationFinished (
            object sender,
            EventArgs e)
        {
            this.AddResultClassesToGridView (this.resultClassColumn);
            MessageBox.Show ("Classification finished");
        }
        #endregion

        #region ClearDebugInfoToolStripMenuItem_Click
        private void ClearDebugInfoToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            this.listBoxStatus.Items.Clear ();
        }
        #endregion

        #region SaveDebugInfoToolStripMenuItem_Click
        private void SaveDebugInfoToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            this.saveFileDialog.ShowDialog ();

            FileStream fs = new FileStream (this.saveFileDialog.FileName,
                FileMode.Create);

            fs.Position = 0;

            foreach (String s in this.listBoxStatus.Items)
            {
                //Write linefeed
                foreach (byte b in "\n".ToCharArray ())
                {
                    fs.WriteByte (b);
                }

                //Write line
                foreach (byte b in s.ToCharArray ())
                {
                    fs.WriteByte (b);
                }
            }
            fs.Close ();
        }
        #endregion

        #region ExitToolStripMenuItem_Click
        private void ExitToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            if (MessageBox.Show ("Are you sure", "Quit application?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //this.QuitApplication ();
                this.Close ();
            }
        }
        #endregion

        #region QuitApplication
        private void QuitApplication ()
        {
            if (C45Project.IsDirty)
            {
                if (MessageBox.Show (
                    "Save Project File?",
                    "Exiting Program ...",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.project.Save ();
                }
            }

            this.SaveFormAppearance ();
        }
        #endregion

        #region SaveFormAppearance
        private void SaveFormAppearance ()
        {
            this.formConfig.FrmSize = this.Size;
            this.formConfig.FrmStartPosition = this.StartPosition;
            this.formConfig.FrmWindowState = this.WindowState;
            this.formConfig.StatusBox = this.statusBoxToolStripMenuItem.CheckState;
            this.formConfig.TextBox = this.textBoxToolStripMenuItem1.CheckState;
            this.formConfig.DocumentImage = this.documentImageToolStripMenuItem.CheckState;

            this.formConfig.Save ();
        }
        #endregion

        #region AddAllresultsToolStripMenuItem_Click_1
        private void AddAllresultsToolStripMenuItem_Click_1 (
            object sender,
            EventArgs e)
        {
            this.AddAllResultsToolStripMenuItem_Click (
                sender,
                e);
        }
        #endregion

        #region DataGridView_RowsRemoved
        private void DataGridView_RowsRemoved (
            object sender,
            DataGridViewRowsRemovedEventArgs e)
        {
            if (this.dataGridView.Rows.Count <= 0)
            {
                this.classifyToolStripMenuItem.Enabled = false;
                this.addResultsToolStripMenuItem.Enabled = false;
                this.removeRowToolStripMenuItem.Enabled = false;

                this.classifyToolStripMenuItem1.Enabled = false;
                this.addResultsToolStripMenuItem1.Enabled = false;
                this.removeRowToolStripMenuItem1.Enabled = false;
            }
        }
        #endregion

        #region LoadFSCToolStripMenuItem_Click_1
        private void LoadFSCToolStripMenuItem_Click_1 (
            object sender,
            EventArgs e)
        {
            DialogResult result = DialogResult.No;

            if (this.dataGridView.Rows.Count > 0)
            {
                result = MessageBox.Show (
                    "Clear Grid?",
                    "Question",
                    MessageBoxButtons.YesNo);
            }
            else
            {
                result = DialogResult.Yes;
            }

            this.openFileDialog.Filter = C45Constants.FILTER_C45DATA;

            if (result == DialogResult.Yes)
            {
                this.openFileDialog.FileName = "";
                this.openFileDialog.ShowDialog ();

                if (this.openFileDialog.FileName != String.Empty)
                {
                    this.featureSetCollection = C45FeatureSetCollection.LoadFSCFromFile (this.openFileDialog.FileName);
                    this.SetEventHandler ();
                    this.RestoreDataGridView ();
                }
            }
        }
        #endregion

        #region RestoreDataGridView
        private void RestoreDataGridView ()
        {
            int rowIndex = 0;

            foreach (C45FeatureSet fs in this.featureSetCollection)
            {
                this.dataGridView.Rows.Add ();

                this.InsertFeatureSetIntoGridView (
                    fs,
                    rowIndex);

                this.dataGridView.Rows[rowIndex].Cells[this.userClassColumn].Value = fs.UserResult.ClassName;
                rowIndex++;
            }
        }
        #endregion

        #region InsertFeatureSetIntoGridView
        private void InsertFeatureSetIntoGridView (
            C45FeatureSet fs,
            int rowIndex)
        {
            int columnIndex = 0;

            foreach (object f in fs)
            {
                this.InsertFeatureValueIntoGridView (
                    f,
                    columnIndex,
                    rowIndex);

                columnIndex++;
            }
        }
        #endregion

        #region InsertFeatureValueIntoGridView
        private void InsertFeatureValueIntoGridView (
            object f,
            int columnIndex,
            int rowIndex)
        {
            if (f is EverydaySolutions.C45.C45ContinuousFeature)
            {
                this.dataGridView[columnIndex, rowIndex].Value = (f as C45ContinuousFeature).LowerBound;
            }
            else
            {
                if (f is EverydaySolutions.C45.C45DiscreteFeature)
                {
                    //Sorry, just one discrete feature-attribute with 100% probability supported, yet.
                    this.dataGridView[columnIndex, rowIndex].Value = (f as C45DiscreteFeature)[0].AttributeName;
                }
            }
        }
        #endregion

        #region LoadFSCfromDocumentToolStripMenuItem_Click
        private void LoadFSCfromDocumentToolStripMenuItem_Click (
            object sender,
            EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.dataGridView.Rows.Clear (); 
   
            this.openFileDialog.Filter = C45Constants.FILTER_IMAGES;
            this.openFileDialog.FileName = String.Empty;
            this.openFileDialog.ShowDialog ();

            if (this.openFileDialog.FileName != String.Empty)
            {
                HRTFeaturesSetGenerator.OCRProgress += new HRTFeaturesSetGenerator.OCRProgressEventHandler (HRTFeaturesSetGenerator_OCRProgress);
                this.generator = new HRTFeaturesSetGenerator (this.openFileDialog.FileName);                
                
                if (documentImageToolStripMenuItem.CheckState == CheckState.Checked)
                {
                    this.LoadDocumentImageForm ();
                    docImageForm.ModiDocument = this.generator.ModiDocument;
                    docImageForm.Lines = this.generator.Lines;
                    docImageForm.Show ();
                }

                if (this.textBoxToolStripMenuItem1.CheckState == CheckState.Checked)
                {
                    this.listBoxText.Visible = true;
                    this.listBoxStatus.Visible = false;
                }

                if (this.statusBoxToolStripMenuItem.CheckState == CheckState.Checked)
                {
                    this.listBoxText.Visible = false;
                    this.listBoxStatus.Visible = true;
                }

                this.progressBarState.Value = 100;
                this.labelState.Text = "OCR recognition finished";

                foreach (C45FeatureSet fs in generator.FeatureSetCollection)
                {
                    this.featureSetCollection.Add (fs);
                }

                this.generator.SaveDocInformationAsText (Path.GetFileName (this.openFileDialog.FileName) + @".txt");

                this.SetEventHandler ();
                this.RestoreDataGridView ();
                this.FillTextListBox ();
            }
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region FillTextListBox
        private void FillTextListBox ()
        {
            this.listBoxText.Items.Clear ();
            foreach (String line in generator.TextLines)
            {
                this.listBoxText.Items.Add (line);
            }
        }
        #endregion

        #region HRTFeaturesSetGenerator_OCRProgress
        void HRTFeaturesSetGenerator_OCRProgress (
            object sender,
            OCRProgressEventArgs e)
        {
            if (!this.InvokeRequired)
            {
                this.progressBarState.Value = e.Progress;
                this.labelState.Text = "OCR recognition";
            }
            else
            {
                this.HRTFeaturesSetGenerator_OCRProgress (
                    sender,
                    e);
            }
        }
        #endregion

        #region DataGridView_ColumnWidthChanged
        private void DataGridView_ColumnWidthChanged (
            object sender,
            DataGridViewColumnEventArgs e)
        {
            this.project.ColumnWidth[e.Column.Index] = e.Column.Width;
        }
        #endregion

        #region DataGridView_RowEnter
        private void DataGridView_RowEnter (
            object sender, 
            DataGridViewCellEventArgs e)
        {
            if (this.listBoxText.Items.Count > 0)
            {
                this.listBoxText.SelectedIndex = e.RowIndex;
            }
        }
        #endregion

        #region StatusBoxToolStripMenuItem_Click
        private void StatusBoxToolStripMenuItem_Click (
            object sender, 
            EventArgs e)
        {
            this.textBoxToolStripMenuItem1.CheckState = CheckState.Unchecked;
            this.statusBoxToolStripMenuItem.CheckState = CheckState.Checked;
            this.listBoxStatus.Visible = true;
            this.listBoxText.Visible = false;
        }
        #endregion

        #region TextBoxToolStripMenuItem1_Click
        private void TextBoxToolStripMenuItem1_Click (
            object sender, 
            EventArgs e)
        {
            this.textBoxToolStripMenuItem1.CheckState = CheckState.Checked;
            this.statusBoxToolStripMenuItem.CheckState = CheckState.Unchecked;
            this.listBoxStatus.Visible = false;
            this.listBoxText.Visible = true;
        }
        #endregion        

        #region DocumentImageToolStripMenuItem_Click
        private void DocumentImageToolStripMenuItem_Click (
            object sender, 
            EventArgs e)
        {
            if (this.documentImageToolStripMenuItem.CheckState == CheckState.Checked)
            {
                this.LoadDocumentImageForm ();

                if (this.generator != null
                    && this.generator.ModiDocument != null)
                {
                    this.docImageForm.ModiDocument = this.generator.ModiDocument;
                    this.docImageForm.Lines = this.generator.Lines;
                }
                this.docImageForm.Show ();
            }
            else
            {
                this.docImageForm.Close ();
                this.docImageForm = null;
            }
        }
        #endregion

        #region LoadDocumentImageForm
        private void LoadDocumentImageForm ()
        {
            this.docImageForm = new DocumentImageForm ();
            this.docImageForm.Size = this.formConfig.DocFormSize;
            this.docImageForm.Location = this.formConfig.DocFormLocation;
            this.docImageForm.Resize += new EventHandler (DocImageForm_Resize);
            this.docImageForm.LocationChanged += new EventHandler (DocImageForm_LocationChanged);
        }
        #endregion

        #region DocImageForm_LocationChanged
        void DocImageForm_LocationChanged (
            object sender, 
            EventArgs e)
        {            
            this.formConfig.DocFormLocation  = this.docImageForm.Location;
        }
        #endregion

        #region DocImageForm_Resize
        void DocImageForm_Resize (
            object sender, 
            EventArgs e)
        {
            this.formConfig.DocFormSize = this.docImageForm.Size;            
        }
        #endregion

        #region ListBoxText_SelectedIndexChanged
        private void ListBoxText_SelectedIndexChanged (
            object sender, 
            EventArgs e)
        {
            foreach (DataGridViewRow row in this.dataGridView.Rows)
            {
                this.dataGridView.Rows[row.Index].Selected = false;
            }
            if (this.listBoxText.SelectedIndex >= 0)
            {
                this.dataGridView.Rows[this.listBoxText.SelectedIndex].Selected = true;
            }
        }
        #endregion
    }
    #endregion
}