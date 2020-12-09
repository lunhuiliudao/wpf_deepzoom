using System;
using System.Runtime.InteropServices;

namespace KFBIO.SlideViewer
{
	public static class NativeMethods
	{
		public const int WM_DOWNLOAD_COMPLETED = 170;

		[DllImport("User32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("User32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref CopyDataStruc lParam);

		[DllImport("User32.dll")]
		public static extern int PostMessage(IntPtr hWnd, int Msg, int wParam, ref CopyDataStruc lParam);

		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr SetFocus(IntPtr hWnd);

		[DllImport("user32.dll")]
		internal static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);
	}
}
