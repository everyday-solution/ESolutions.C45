using System;
using System.Collections.Generic;
using System.Text;

namespace EverydaySolutions.C45
{
	public class C45SchemaPropertyList : List<C45SchemaProperty>
	{
		public C45SchemaProperty C45SchemaProperty
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
			Boolean isDiscrete, 
			String name, 
			List<String> attributes)
		{
			C45SchemaProperty newColumn = new C45SchemaProperty ();
			newColumn.IsDiscrete = isDiscrete;
			newColumn.Name = name;
			newColumn.DiscreteAttributes = attributes;
			this.Add (newColumn);
		}
		#endregion
	};
}
