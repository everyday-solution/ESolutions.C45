using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace EverydaySolutions.C45
{
	#region _debugParameter
	/// <summary>
	/// Marshalled debug-Parameter c-struct
	/// </summary>
	[StructLayout (LayoutKind.Explicit, Pack = 4)]
	internal struct _C45DebugParameter
	{
		[FieldOffset (00)]
		internal int iVerbosity;
		[FieldOffset (04)]
		internal int iDebugMode;
		[FieldOffset (08)]
		[MarshalAs (UnmanagedType.ByValTStr, SizeConst = C45Constants.MAX_CHAR)]
		internal String szSnifferHost;
		[FieldOffset (264)]
		internal int iSnifferPort;
	};
	#endregion
	
	#region C45DebugParameter
	/// <summary>
	/// Debug settings for C45.DLL
	/// </summary>
	[TypeConverter (typeof (C45DebugParameterConverter))]
	public class C45DebugParameter
	{
		#region DebugParameter
		/// <summary>
		/// Marshalled C-Struct
		/// </summary>
		[Browsable (false)]
		private _C45DebugParameter _debugParamter;
		internal _C45DebugParameter DebugParameter
		{
			get
			{
				this._debugParamter.iDebugMode = DebugMode;
				this._debugParamter.iSnifferPort = this.SnifferPort;
				this._debugParamter.iVerbosity = this.Verbosity;
				this._debugParamter.szSnifferHost = this.SnifferHost;
				return _debugParamter;
			}
			set
			{
				_debugParamter = value;
			}
		}
		#endregion

		#region Verbosity
		/// <summary>
		/// Verbosity level 0-5
		/// </summary>
		[Browsable (true), ReadOnly (false), Description ("Verbosity level 0-5")]
		private int verbosity = 0;
		public int Verbosity
		{
			get
			{
				return verbosity;
			}
			set
			{
				verbosity = value;
			}
		}
		#endregion

		#region DebugMode
		/// <summary>
		/// Debug Mode bitmask: 0 - Debugging of, 1 - Console, 2 - TCP/IP, 3 - Both
		/// </summary>
		[Browsable (true), ReadOnly (false)]
		private int debugMode = 0;
		public int DebugMode
		{
			get
			{
				return debugMode;
			}
			set
			{
				debugMode = value;
			}
		}
		#endregion

		#region SnifferHost
		/// <summary>
		/// Hostname of the sniffer (debug server)
		/// </summary>
		[Browsable (true), ReadOnly (false), Description ("Hostname of the sniffer (debug server)")]
		private String snifferHost = String.Empty;
		public String SnifferHost
		{
			get
			{
				return snifferHost;
			}
			set
			{
				snifferHost = value;
			}
		}
		#endregion

		#region SnifferPort
		/// <summary>
		/// Port of the sniffer (debug server)
		/// </summary>
		[Browsable (true), ReadOnly (false), Description ("Port of the sniffer (debug server)")]
		private int snifferPort = 4949;
		public int SnifferPort
		{
			get
			{
				return snifferPort;
			}
			set
			{
				snifferPort = value;
			}
		}
		#endregion

	}
	#endregion
}
