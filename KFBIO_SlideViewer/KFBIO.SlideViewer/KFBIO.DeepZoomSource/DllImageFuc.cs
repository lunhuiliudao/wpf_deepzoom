using System;
using System.IO;
using System.Runtime.InteropServices;

internal class DllImageFuc
{
	public bool CkGetHeaderInfoFunc(IMAGE_INFO_STRUCT k, ref int khiImageHeight, ref int khiImageWidth, ref int khiScanScale, ref float khiSpendTime, ref double khiScanTime, ref float khiImageCapRes, ref int TileSize)
	{
		return GetHeaderInfoFunc(k, ref khiImageHeight, ref khiImageWidth, ref khiScanScale, ref khiSpendTime, ref khiScanTime, ref khiImageCapRes, ref TileSize);
	}

	public bool CkUnInitImageFileFunc(ref IMAGE_INFO_STRUCT k)
	{
		return UnInitImageFileFunc(ref k);
	}

	public bool CkInitImageFileFunc(ref IMAGE_INFO_STRUCT k, string p)
	{
		return InitImageFileFunc(ref k, p);
	}

	public bool CkGetLableInfoFunc(IMAGE_INFO_STRUCT k, out IntPtr datas, ref int a, ref int b, ref int c)
	{
		return GetLableInfoFunc(k, out datas, ref a, ref b, ref c);
	}

	public bool CkDeleteImageDataFunc(IntPtr datas)
	{
		return DeleteImageDataFunc(datas);
	}

	public bool CkGetImageStreamFunc(ref IMAGE_INFO_STRUCT k, float fScale, int nImagePosX, int nImagePosY, ref int nDataLength, out IntPtr datas)
	{
		return GetImageStreamFunc(ref k, fScale, nImagePosX, nImagePosY, ref nDataLength, out datas);
	}

	public bool CkGetVersionInfoFunc(ref IMAGE_INFO_STRUCT k, ref float fScale)
	{
		return GetVersionInfoFunc(ref k, ref fScale);
	}

	public bool CkGetThumnailImagePathFunc(string path, out IntPtr datas, ref int a, ref int b, ref int c)
	{
		try
		{
			return GetThumnailImagePathFunc(path, out datas, ref a, ref b, ref c);
		}
		catch (Exception ex)
		{
			datas = IntPtr.Zero;
			AddLog(ex.ToString());
			return false;
		}
	}

	private void AddLog(string line)
	{
		string str = "Log.txt";
		try
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			FileStream fileStream = new FileStream(baseDirectory + str, FileMode.OpenOrCreate, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.BaseStream.Seek(0L, SeekOrigin.End);
			streamWriter.WriteLine(line);
			streamWriter.Flush();
			streamWriter.Close();
			fileStream.Close();
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	[DllImport("ImageOperationLib.dll")]
	public static extern bool GetLableInfoFunc(IMAGE_INFO_STRUCT k, out IntPtr datas, ref int a, ref int b, ref int c);

	[DllImport("ImageOperationLib.dll")]
	public static extern bool GetHeaderInfoFunc(IMAGE_INFO_STRUCT k, ref int khiImageHeight, ref int khiImageWidth, ref int khiScanScale, ref float khiSpendTime, ref double khiScanTime, ref float khiImageCapRes, ref int TileSize);

	[DllImport("ImageOperationLib.dll")]
	public static extern bool InitImageFileFunc(ref IMAGE_INFO_STRUCT k, string p);

	[DllImport("ImageOperationLib.dll")]
	public static extern bool UnInitImageFileFunc(ref IMAGE_INFO_STRUCT k);

	[DllImport("ImageOperationLib.dll")]
	public static extern bool GetImageStreamFunc(ref IMAGE_INFO_STRUCT k, float fScale, int nImagePosX, int nImagePosY, ref int nDataLength, out IntPtr datas);

	[DllImport("ImageOperationLib.dll")]
	public static extern bool DeleteImageDataFunc(IntPtr datas);

	[DllImport("ImageOperationLib.dll")]
	public static extern bool GetThumnailImagePathFunc(string path, out IntPtr datas, ref int a, ref int b, ref int c);

	[DllImport("ImageOperationLib.dll")]
	public static extern bool GetPriviewInfoPathFunc(string path, out IntPtr datas, ref int a, ref int b, ref int c);

	[DllImport("ImageOperationLib.dll")]
	public static extern bool GetLableInfoPathFunc(string path, out IntPtr datas, ref int a, ref int b, ref int c);

	[DllImport("ImageOperationLib.dll")]
	public static extern bool GetVersionInfoFunc(ref IMAGE_INFO_STRUCT k, ref float f);

	[DllImport("ImageOperationLib.dll")]
	public static extern bool GetMachineSerialNumFunc(ref IMAGE_INFO_STRUCT k, IntPtr str);

	[DllImport("ImageOperationLib.dll")]
	public static extern bool GetScanTimeDurationFunc(ref IMAGE_INFO_STRUCT k, ref int nYear, ref int nMonth, ref int nDay, ref int nHour, ref int nMiniter, ref int nSecond, ref int nDurHour, ref int nDurMin, ref int nDurSecond);

	[DllImport("ImageOperationLib.dll")]
	public static extern void GetDllVersionFunc(ref float f);
}
