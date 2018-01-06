using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EverydaySolutions.C45
{
	#region C45FeatureSetCollection
	public class C45FeatureSetCollection : List<C45FeatureSet>
	{
		#region Attributes
		private static C45FeatureSetCollection instance = null;
		#endregion

		#region Events
		public event EventHandler ClassificationComplete;
		public event DecisionTreeGenerationStartedEventHandler DecisionTreeGenerationStarted;
		public event DecisionTreeGenerationFinishedEventHandler DecisionTreeGenerationFinished;
		public event ConsultationStartedEventHandler ConsultationStarted;
		public event ConsultationFinishedEventHandler ConsultationFinished;
		public event SingleFeatureClassifiedEventHandler SingleFeatureClassified;
		#endregion

		public C45FeatureSet C45FeatureSet
		{
			get
			{
				throw new System.NotImplementedException ();
			}
			set
			{
			}
		}
		
		#region StartClassify
		public void StartClassify ()
		{
			Thread classifyThread = new Thread (Classify);
			classifyThread.Start ();
		}
		#endregion

		#region Classification
		private void Classify ()
		{			
			C45Controller c45controller = new C45Controller ();

			c45controller.DecisionTreeGenerationStarted += new DecisionTreeGenerationStartedEventHandler (C45Controller_DecisionTreeGenerationStarted);
			c45controller.DecisionTreeGenerationFinished += new DecisionTreeGenerationFinishedEventHandler (C45Controller_DecisionTreeGenerationFinished);
			c45controller.ConsultationStarted += new ConsultationStartedEventHandler (C45Controller_ConsultationStarted);
			c45controller.ConsultationFinished += new ConsultationFinishedEventHandler (C45Controller_ConsultationFinished);

			foreach (C45FeatureSet fs in this)
			{
				if (fs.IsMarked)
				{
					fs.ComputedResult = c45controller.Classification (fs._featureArray);
					fs.isClassified = true;
					
					this.OnSingleFeatureClassified (
						this, 
						new C45SingleFeatureSetClassifiedEventArgs (c45controller.GetStatistics ()));
				}
			}
			this.OnClassificationComplete (
				this, 
				new EventArgs ());
		}
		#endregion

		#region OnSingleFeatureClassified
		protected void OnSingleFeatureClassified (
			object sender,
			C45SingleFeatureSetClassifiedEventArgs e)
		{
			if (this.SingleFeatureClassified != null)
			{
				this.SingleFeatureClassified (
					sender,
					e);
			}
		}
		#endregion

		#region OnClassificationComplete
		private void OnClassificationComplete (
			object sender,
			EventArgs e)
		{
			if (this.ClassificationComplete != null)
			{
				this.ClassificationComplete (
					sender,
					e);
			}
		}
		#endregion

		#region C45controller_ConsultationFinished
		private void C45Controller_ConsultationFinished (
			object sender, 
			EventArgs e)
		{
			this.OnConsultationFinished(
				sender, 
				e);
		}
		#endregion
	
		#region OnConsultationFinished
		private void OnConsultationFinished (
			object sender,
			EventArgs e)
		{
			this.ConsultationFinished (
				sender, 
				e);
		}
		#endregion

		#region C45controller_ConsultationStarted
		private void C45Controller_ConsultationStarted (
			object sender, 
			EventArgs e)
		{
			this.OnConsultationStarted (
				sender, 
				e);
		}
		#endregion

		#region OnConsultationStarted
		private void OnConsultationStarted (
			object sender,
			EventArgs e)
		{
			if (this.ConsultationStarted != null)
			{
				this.ConsultationStarted (
					sender,
					e);
			}
		}
		#endregion

		#region C45controller_DecisionTreeGenerationFinished
		private void C45Controller_DecisionTreeGenerationFinished (
			object sender, 
			EventArgs e)
		{
			this.OnDecisionTreeGenerationFinished (
				sender,
				e);	
		}
		#endregion

		#region OnDecisionTreeGenerationFinished
		private void OnDecisionTreeGenerationFinished (
			object sender,
			EventArgs e)
		{
			if (this.DecisionTreeGenerationStarted != null)
			{
				this.DecisionTreeGenerationStarted (
					sender,
					e);
			}
		}
		#endregion

		#region C45controller_DecisionTreeGenerationStarted
		private void C45Controller_DecisionTreeGenerationStarted (
			object sender, 
			EventArgs e)
		{
			this.OnDecisionTreeGenerationStarted (
				sender,
				e);		
		}
		#endregion

		#region OnDecisionTreeGenerationStarted
		private void OnDecisionTreeGenerationStarted (
			object sender,
			EventArgs e)
		{
			this.DecisionTreeGenerationStarted (
			sender,
			e);
		}
		#endregion

		#region AddComputedResults
		public void AddComputedResults ()
		{
			C45Controller c45controller = new C45Controller ();

			foreach (C45FeatureSet fs in this)
			{
				if (fs.IsMarked)
				{
					c45controller.AddResult (fs,
						fs.ComputedResult.GetBestResult ().ResultClassName);
				}
			}
		}
		#endregion

		#region AddUserResults
		public void AddUserResults ()
		{
			C45Controller c45controller = new C45Controller ();

			foreach (C45FeatureSet fs in this)
			{
				if (fs.IsMarked)
				{
					c45controller.AddResult (
						fs,
						fs.UserResult.ClassName);
				}
			}
		}
		#endregion

		#region LoadFSCFromFile
		public static C45FeatureSetCollection LoadFSCFromFile (String filename)
		{
			C45Controller controller = new C45Controller ();

			if (instance == null)
			{
				instance = controller.GetFSCFromFile (filename);
			}

			return instance;
		}
		#endregion
	}
	#endregion
}