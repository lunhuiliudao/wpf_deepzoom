using System;
using System.Runtime.InteropServices;

namespace KFBIO.SlideViewer
{
	public struct CopyDataStruc
	{
		public IntPtr dwData;

		public int cbData;

		[MarshalAs(UnmanagedType.LPStr)]
		public string lpData;
	}
}
