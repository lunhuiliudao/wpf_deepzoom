using System;
using System.IO;
using System.Runtime.InteropServices;

namespace KFBIO.SlideViewer
{
	internal class DllImageFuc
	{
        /// <summary>
        /// ��ȡ�ļ���ͷ����Ϣ
        /// </summary>
        /// <param name="k">�ļ���ַ</param>
        /// <param name="khiImageHeight">ͼ��߶�</param>
        /// <param name="khiImageWidth">ͼ����</param>
        /// <param name="khiScanScale">ɨ�豶�ʣ�20����40</param>
        /// <param name="khiSpendTime">����ʱ��</param>
        /// <param name="khiScanTime">ɨ��ʱ��</param>
        /// <param name="khiImageCapRes"></param>
        /// <param name="TileSize">��Ƭͼ�Ĵ�С</param>
        /// <returns></returns>
		public bool CkGetHeaderInfoFunc(IMAGE_INFO_STRUCT k, ref int khiImageHeight, ref int khiImageWidth, ref int khiScanScale, ref float khiSpendTime, ref double khiScanTime, ref float khiImageCapRes, ref int TileSize)
		{
			return GetHeaderInfoFunc(k, ref khiImageHeight, ref khiImageWidth, ref khiScanScale, ref khiSpendTime, ref khiScanTime, ref khiImageCapRes, ref TileSize);
		}

		public bool CkUnInitImageFileFunc(ref IMAGE_INFO_STRUCT k)
		{
			return UnInitImageFileFunc(ref k);
		}
        /// <summary>
        /// ��ȡkfb�ļ����ܴ�С
        /// </summary>
        /// <param name="k">�ļ��ĵ�ַ</param>
        /// <param name="p">kfb�ļ�������·��</param>
        /// <returns></returns>
		public bool CkInitImageFileFunc(ref IMAGE_INFO_STRUCT k, string p)
		{
			return InitImageFileFunc(ref k, p);
		}

        /// <summary>
        /// ��ȡ��ά����Ϣ
        /// </summary>
		public bool CkGetLableInfoFunc(IMAGE_INFO_STRUCT k, out IntPtr datas, ref int a, ref int b, ref int c)
		{
			return GetLableInfoFunc(k, out datas, ref a, ref b, ref c);
		}

        /// <summary>
        /// ɾ���ڴ�����
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
		public bool CkDeleteImageDataFunc(IntPtr datas)
		{
			return DeleteImageDataFunc(datas);
		}

        /// <summary>
        /// ��ȡ��Ƭͼ
        /// </summary>
        /// <param name="k">��ַ</param>
        /// <param name="fScale">���ű���</param>
        /// <param name="nImagePosX">��Ƭͼ�����꣺X</param>
        /// <param name="nImagePosY">��Ƭͼ�����꣺Y</param>
        /// <param name="nDataLength">ͼƬ���ֽڳ���</param>
        /// <param name="datas">ͼƬ��byte</param>
        /// <returns></returns>
		public bool CkGetImageStreamFunc(ref IMAGE_INFO_STRUCT k, float fScale, int nImagePosX, int nImagePosY, ref int nDataLength, out IntPtr datas)
		{
			return GetImageStreamFunc(ref k, fScale, nImagePosX, nImagePosY, ref nDataLength, out datas);
		}
        /// <summary>
        /// ��ȡ�汾��Ϣ
        /// </summary>
        /// <param name="k">�ļ��ĵ�ַ</param>
        /// <param name="fScale">���Ĳ���</param>
        /// <returns></returns>
		public bool CkGetVersionInfoFunc(ref IMAGE_INFO_STRUCT k, ref float fScale)
		{
			return GetVersionInfoFunc(ref k, ref fScale);
		}

        /// <summary>
        /// ��ȡ���ߵĶ�ά�������������ͼ
        /// </summary>
        /// <param name="path">kfb��ַ</param>
        /// <param name="datas">��ַ</param>
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

        /// <summary>
        /// ��ȡkfb�ļ��Ĳ�����Ϣ
        /// </summary>
        /// <param name="k">�ļ���ַ</param>
        /// <param name="nCurLevel">��ǰ����</param>
        /// <param name="nTotalLevel">�ܲ���</param>
		[DllImport("ImageOperationLib.dll")]
		public static extern void GetScanLevelInfoFunc(ref IMAGE_INFO_STRUCT k, ref int nCurLevel, ref int nTotalLevel);

		[DllImport("ImageOperationLib.dll")]
		public static extern void GetImageDataRoiFunc(IMAGE_INFO_STRUCT k, float fScale, int x, int y, int width, int height, out IntPtr datas, ref int nDataLength, bool flag);

		[DllImport("ImageSprocLib.dll")]
		public static extern void ImageMatchRotate(string pSrcImagePath, string pRotateImagePath, ref int nDegree);

		[DllImport("ImageSprocLib.dll")]
		public static extern int TMARec(string pSrcImagePath);

		[DllImport("ImageSprocLib.dll")]
		public static extern int TMARecS(string pSrcImagePath, int Param);

		[DllImport("ImageOperationLib.dll")]
		public static extern bool GetHSVImage(ref IMAGE_INFO_STRUCT kfbPoint, float fScale, int nImagePosX, int nImagePosY, ref int nDataLength, out IntPtr datas, int S_shift, int V_shift, int b, int b_r, int b_h, int r, int r_r, int r_h, float k, int sp);
	}
}
