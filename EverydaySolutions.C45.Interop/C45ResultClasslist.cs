using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace EverydaySolutions.C45
{
	public class C45ComputedResultList : List<C45ComputedResult>
	{
		public C45ComputedResultList (
			_resultClass[] resultClasses, 
			int classesCount)
		{
			for (int i = 0; i < classesCount; i++)
			{
				this.Add (resultClasses[i].iLowerBound,
					resultClasses[i].iUpperBound,
					resultClasses[i].iBestGuess,
					this.ConvertByteArrayToString (resultClasses[i].bClassBuffer));
			}
		}

		public C45ComputedResult C45ResultClass
		{
			get
			{
				throw new System.NotImplementedException ();
			}
			set
			{
			}
		}

		#region Add
		public void Add (
			float lowerBoundProbability, 
			float upperBoundProbability, 
			float bestGuessProbability, 
			String outClassName)
		{
			C45ComputedResult newResultClass = new C45ComputedResult ();
			newResultClass.LowerRangeProbability = lowerBoundProbability;
			newResultClass.UpperRangeProbability = upperBoundProbability;
			newResultClass.BestGuessProbability = bestGuessProbability;
			newResultClass.ResultClassName = outClassName;
			base.Add (newResultClass);
		}
		#endregion

		#region GetBestResult
		public C45ComputedResult GetBestResult ()
		{
			C45ComputedResult c45BestResult = new C45ComputedResult ();
			float bestGuessProbability = -1;

			foreach (C45ComputedResult result in this)
			{							
				if (result.BestGuessProbability > bestGuessProbability)
				{
					bestGuessProbability = result.BestGuessProbability;
					c45BestResult = result;
				}
			}

			return c45BestResult;
		}
		#endregion

		#region ConvertByteArrayToString
		private String ConvertByteArrayToString (byte[] byteArray)
		{
			String newString = String.Empty;

			foreach (char b in byteArray)
			{
				if (b > 0)
				{
					newString += b.ToString ();
				}
			}

			return newString;
		}
		#endregion
	}
}