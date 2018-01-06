using System;
using System.Collections.Generic;
using System.Text;

namespace EverydaySolutions.C45
{
	public class C45Schema
	{
		#region Attributes
		//static private C45Schema instance = null;
		#endregion

		#region CreateFromFile
		public static C45Schema CreateFromFile (String filename)
		{
			C45Controller controller = new C45Controller ();
			return controller.ParseNamesFile (filename);
		}
		#endregion

		#region Attributes
		#endregion

		#region Empty
		private Boolean empty = true;
		public bool Empty
		{
			get
			{
				return empty;
			}
			set
			{
				empty = value;
			}
		}
		#endregion

		#region Classes
		private List<String> classes = new List<String> ();
		public List<String> Classes
		{
			get
			{
				return classes;
			}
			set
			{
				classes = value;
			}
		}
		#endregion

		#region Properties
		private C45SchemaPropertyList properties = new C45SchemaPropertyList ();
		public C45SchemaPropertyList Properties
		{
			get
			{
				return properties;
			}
			set
			{
				properties = value;
			}
		}
		#endregion

		#region AddClass
		public void AddClass (String className)
		{
			this.classes.Add (className);
		}
		#endregion

	}
}
