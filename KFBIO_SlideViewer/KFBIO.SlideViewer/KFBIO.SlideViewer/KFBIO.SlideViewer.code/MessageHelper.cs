using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace KFBIO.SlideViewer.code
{
	public class MessageHelper
	{
		public struct CopyDataStruct
		{
			public IntPtr dwData;

			public int cbData;

			[MarshalAs(UnmanagedType.LPStr)]
			public string lpData;
		}

		public const int WM_COPYDATA = 74;

		[DllImport("User32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref CopyDataStruct lParam);

		[DllImport("User32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		public static void SendMessage(string windowName, string strMsg)
		{
			if (strMsg != null)
			{
				IntPtr intPtr = FindWindow(null, windowName);
				if (intPtr != IntPtr.Zero)
				{
					CopyDataStruct lParam = default(CopyDataStruct);
					lParam.dwData = IntPtr.Zero;
					lParam.lpData = strMsg;
					lParam.cbData = Encoding.Default.GetBytes(strMsg).Length + 1;
					int wParam = 0;
					SendMessage(intPtr, 74, wParam, ref lParam);
				}
			}
		}

		public static void SendMessageByProcess(Process process, string strMsg)
		{
			if (strMsg != null)
			{
				IntPtr mainWindowHandle = process.MainWindowHandle;
				if (!(mainWindowHandle == IntPtr.Zero) && mainWindowHandle != IntPtr.Zero)
				{
					CopyDataStruct lParam = default(CopyDataStruct);
					lParam.dwData = IntPtr.Zero;
					lParam.lpData = strMsg;
					lParam.cbData = Encoding.Default.GetBytes(strMsg).Length + 1;
					int wParam = 0;
					SendMessage(mainWindowHandle, 74, wParam, ref lParam);
				}
			}
		}
	}
}
