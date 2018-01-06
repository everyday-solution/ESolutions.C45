using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Xml;

namespace EverydaySolutions.C45
{
	public delegate void DirtyStateChangedEventHandler (
		object sender,
		EventArgs e);

	public sealed class ColumnWidthCollection : List<Int32>
    {
        #region Attributes
        XmlDocument doc = null;
        #endregion

        #region Events
        public event DirtyStateChangedEventHandler DirtyStateChanged;
        #endregion

        #region this
        public Int32 this[Int32 index]
		{			
			get
			{
                return base[index];
			}
			set
			{
				this.AddNode (
					index, 
					value);
			}
        }
        #endregion

        #region OnDirtyStateChanged
        protected void OnDirtyStateChanged (	
			object sender,
			EventArgs e)
		{
			if (DirtyStateChanged != null)
			{
				DirtyStateChanged (
					sender, 
					e);
			}
		}
		#endregion

		#region ColumnWidthCollection
		public ColumnWidthCollection (XmlDocument doc)
		{
			this.doc = doc;

			try
			{
				for (int column = 0; column <= 99999; column++)
				{
                    String xPathQuery = @"//c45project/attributes/column_width/" + "col" + column.ToString("00000");
					base.Add (Convert.ToInt32 (this.doc.SelectSingleNode (xPathQuery).InnerText));
				}
			}
			catch (Exception ex)
			{				
			}
		}
		#endregion

		#region Add
		public void Add (Int32 columnWidth)
		{
			base.Add (columnWidth);

			AddNode (
				this.Count - 1, 
				columnWidth);
		}
		#endregion

		#region AddNode
		private void AddNode (
			Int32 columnIndex, 
			Int32 columnWidth)
		{
			if (this.doc.SelectSingleNode (@"//c45project/attributes/column_width/" + "col" + columnIndex.ToString ("00000")) != null)
			{
				this.doc.SelectSingleNode (@"//c45project/attributes/column_width/" + "col" + columnIndex.ToString ("00000")).InnerText = columnWidth.ToString ();
			}
			else
			{
				XmlNode newNode = this.doc.CreateNode (
															XmlNodeType.Element,
															columnIndex.ToString ("00000"),
															"");
				XmlElement element = doc.CreateElement (
					"col" +
					columnIndex.ToString ("00000"));
					element.InnerText = columnWidth.ToString ();

				XmlNode root = doc.DocumentElement["attributes"]["column_width"];
				root.InsertAfter (
					element,
					root.LastChild);
			}

			OnDirtyStateChanged (
				this,
				new EventArgs ());
		}
		#endregion
	}
}
