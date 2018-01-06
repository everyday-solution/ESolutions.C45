using System;
using System.Collections.Generic;
using System.Text;

namespace EverydaySolutions.C45
{
	public class C45SchemaProperty
	{
		#region IsDiscrete
		private Boolean isDiscrete = false;
		public Boolean IsDiscrete
		{
			get
			{
				return isDiscrete;
			}
			set
			{
				isDiscrete = value;
			}
		}
		#endregion

		#region Name
		/// <summary>
		/// Every 
		/// </summary>
		private String name;
		public String Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}
		#endregion

		#region Attributes
		/// <summary>
		/// Attributes for discrete column
		/// </summary>
		private List<String> discreteAttributes;
		public List<String> DiscreteAttributes
		{
			get
			{
				return discreteAttributes;
			}
			set
			{
				discreteAttributes = value;
			}
		}
		#endregion 	
	}
}
