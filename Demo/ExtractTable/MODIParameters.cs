using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using DocumentProcessing;
using DocumentProcessing.Model;
using DocumentProcessing.GUI;

namespace DocumentProcessing
{
	/// <summary>
	/// Representation for the MODI OCR parameters
	/// </summary>
	public class MODIOCRParameters
	{
		private MODI.MiLANGUAGES _language = MODI.MiLANGUAGES.miLANG_SYSDEFAULT;
		public MODI.MiLANGUAGES Language 
		{
			get {return _language;}
			set {_language = value;}
		}
		private bool _withAutoRotation = true;
		public bool WithAutoRotation
		{
			get {return _withAutoRotation;}
			set {_withAutoRotation = value;}
		}
		private bool _WithStraightenImage = true;
		public bool WithStraightenImage
		{
			get {return _WithStraightenImage;}
			set {_WithStraightenImage = value;}
		}
	}

}
