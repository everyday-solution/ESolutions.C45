using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace EverydaySolutions.C45
{
	public class C45DebugParameterConverter : ExpandableObjectConverter
	{
		#region CanConvertFrom
		/// <summary>
		/// Checks, whether the specified object can be converted to a C45DebugParameter object
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="sourceType">A Type that represents the type you want to convert from.</param>
		/// <returns></returns>
		public override bool CanConvertFrom (ITypeDescriptorContext 
			context, Type sourceType)
		{
			bool returnValue = true;

			if (!(sourceType != Type.GetType ("String")))
			{
				returnValue = base.CanConvertFrom (context,
					sourceType);
			}

			return returnValue;
		}
		#endregion

		#region CanConvertTo
		/// <summary>
		/// Checks, whether the C45DebugParameter can be converted to the specified object type
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context. </param>
		/// <param name="destinationType">A Type that represents the type you want to convert to.</param>
		/// <returns></returns>
		public override bool CanConvertTo (ITypeDescriptorContext context, 
			Type destinationType)
		{
			bool returnValue = true;

			if (destinationType != Type.GetType ("C45DebugParameter"))
			{
				returnValue = base.CanConvertFrom (context, destinationType);
			}

			return returnValue;
		}
		#endregion

		#region ConvertFrom
		/// <summary>
		/// Converts a string object to a C45DebugParameter object
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">The CultureInfo to use as the current culture</param>
		/// <param name="value">The Object to convert</param>
		/// <returns></returns>
		public override object ConvertFrom (ITypeDescriptorContext context, 
			System.Globalization.CultureInfo culture, 
			object value)
		{
			object returnValue = null;

			if (value.GetType () == Type.GetType ("String"))
			{
				try
				{
					String s = (String)ConvertTo (value, Type.GetType ("String"));
					String[] values = s.Split (@"/".ToCharArray ());

					C45DebugParameter debugParameter = (C45DebugParameter)ConvertTo (value, Type.GetType ("C45DebugParameter"));
					debugParameter.DebugMode = Convert.ToInt32 (values[0]);
					debugParameter.SnifferHost = values[1];
					debugParameter.SnifferPort = Convert.ToInt32 (values[2]);
					debugParameter.Verbosity = Convert.ToInt32 (values[2]);

					returnValue = debugParameter;
				}
				catch (Exception ex)
				{
					throw (new ArgumentException ("Can not convert '" + value + "' to type C45DebugParameter", ex));
				}
			}
			else
			{
				returnValue = base.ConvertFrom (context,
					culture,
					value);
			}
			return returnValue;
		}
		#endregion

		#region ConvertTo
		/// <summary>
		/// Converts C45DebugParameter object to string
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">A CultureInfo object. If null is passed, the current culture is assumed.</param>
		/// <param name="value">The Object to convert.</param>
		/// <param name="destinationType">The Type to convert the value parameter to.</param>
		/// <returns></returns>
		public override object ConvertTo (ITypeDescriptorContext context, 
			System.Globalization.CultureInfo culture, 
			object value, 
			Type destinationType)
		{
			object returnValue = new object ();

			if (destinationType.GetType () == Type.GetType ("String")
				&& value.GetType () == Type.GetType ("C45DebugParameter"))
			{
				C45DebugParameter debugParameter = (C45DebugParameter) ConvertTo (value, Type.GetType ("C45DebugParameter"));
				String s = debugParameter.DebugMode.ToString ()
					+ @"/"
					+ debugParameter.SnifferHost
					+ @"/"
					+ debugParameter.SnifferPort.ToString ()
					+ @"/"
					+ debugParameter.Verbosity.ToString ();
				returnValue = s;
			}
			else
			{
				base.ConvertTo (context, culture, value, destinationType);
			}

			return returnValue;
		}
		#endregion
	}
}
