using KFBIO.SlideViewer;
using QuickZip.UserControls.Logic.Tools.IconExtractor;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace QuickZip.Converters
{
	public class ExIconExtractor : IconExtractor<FileSystemInfoEx>
	{
		protected static string fileBasedFSFilter = ".zip,.7z,.lha,.lzh,.sqx,.cab,.ace";

		private bool IsGuidPath(string FullName)
		{
			return FullName.StartsWith("::{");
		}

		protected override Bitmap GetIconInner(FileSystemInfoEx entry, string key, IconSize size)
		{
			if (key.StartsWith("."))
			{
				throw new Exception("ext item is handled by IconExtractor");
			}
			if (entry is FileInfoEx)
			{
				Bitmap bitmap = null;
				string extension = PathEx.GetExtension(entry.Name);
				if (IconExtractor.IsJpeg(extension))
				{
					bitmap = IconExtractor.GetExifThumbnail(entry.FullName);
				}
				if (IconExtractor.IsImageIcon(extension))
				{
					try
					{
						bitmap = new Bitmap(entry.FullName);
					}
					catch
					{
						bitmap = null;
					}
				}
				if (bitmap != null)
				{
					return bitmap;
				}
			}
			return GetBitmap(size, entry.PIDL.Ptr, entry is DirectoryInfoEx, false);
		}
        // 获取右侧列表的 文件图片
		protected override Bitmap GetKFBInner(FileSystemInfoEx entry, string key, IconSize size)
		{
			Bitmap bitmap = null;
			if (entry is FileInfoEx)
			{
				string extension = PathEx.GetExtension(entry.Name);
				if (IconExtractor.IsKFBIcon(extension))
				{
					try
					{
						bitmap = GetKFBThumnail(entry.FullName);
					}
					catch
					{
						bitmap = null;
					}
				}
				if (bitmap != null)
				{
					return bitmap;
				}
			}
			return bitmap;
		}

		public Bitmap GetKFBThumnail(string path)
		{
			KFBIO.SlideViewer.DllImageFuc dllImageFuc = new KFBIO.SlideViewer.DllImageFuc();
			Bitmap bitmap = new Bitmap(120, 120, PixelFormat.Format24bppRgb);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.Clear(Color.White);
			MemoryStream memoryStream = null;
			Bitmap bitmap2 = null;
			Bitmap bitmap3 = null;
			try
			{
				IntPtr datas = IntPtr.Zero;
				int b = 0;
				int c = 0;
				int a = 0;
				dllImageFuc.CkGetThumnailImagePathFunc(path, out datas, ref a, ref b, ref c);
				byte[] array = new byte[a];
				if (datas != IntPtr.Zero)
				{
					Marshal.Copy(datas, array, 0, a);
				}
				dllImageFuc.CkDeleteImageDataFunc(datas);
				memoryStream = new MemoryStream(array);
				bitmap2 = new Bitmap(IconExtractor.ReizeImage(memoryStream, 50.0));
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				Image image = Image.FromFile(baseDirectory + "images\\pp.png");
				graphics.DrawImage(image, 30, 0);
				graphics.Save();
				bitmap3 = new Bitmap(IconExtractor.ReizeImage(GetKFBLabel(path), 50.0));
				int width = bitmap3.Width;
				_ = bitmap3.Height;
				graphics.DrawImage(bitmap3, 30 + (60 - width) / 2, 5);
				graphics.Save();
				int width2 = bitmap2.Width;
				int height = bitmap2.Height;
				graphics.DrawImage(bitmap2, 30 + (60 - width2) / 2, 15 + (120 - height) / 2);
				graphics.Save();
			}
			catch
			{
				string baseDirectory2 = AppDomain.CurrentDomain.BaseDirectory;
				Image image2 = Image.FromFile(baseDirectory2 + "images\\KViewerIcon_5_allSize.png");
				graphics.DrawImage(image2, 0, 0);
				graphics.Save();
				return new Bitmap(bitmap);
			}
			return new Bitmap(bitmap);
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
        /// <summary>
        /// 获取患者的二维码图片
        /// </summary>
		public MemoryStream GetKFBLabel(string path)
		{
			IntPtr datas = IntPtr.Zero;
			int b = 0;
			int c = 0;
			int a = 0;
			KFBIO.SlideViewer.DllImageFuc.GetLableInfoPathFunc(path, out datas, ref a, ref b, ref c);
			byte[] array = new byte[a];
			if (datas != IntPtr.Zero)
			{
				Marshal.Copy(datas, array, 0, a);
			}
			KFBIO.SlideViewer.DllImageFuc.DeleteImageDataFunc(datas);
			return new MemoryStream(array);
		}

		protected override void GetIconKey(FileSystemInfoEx entry, IconSize size, out string fastKey, out string slowKey)
		{
			string extension = PathEx.GetExtension(entry.Name);
			if (entry is DirectoryInfoEx)
			{
				fastKey = entry.FullName;
				slowKey = entry.FullName;
			}
			else if (IsGuidPath(entry.Name))
			{
				fastKey = entry.FullName;
				slowKey = entry.FullName;
			}
			else if (IconExtractor.IsImageIcon(extension) || IconExtractor.IsSpecialIcon(extension))
			{
				fastKey = extension;
				slowKey = entry.FullName;
			}
			else
			{
				fastKey = (slowKey = extension);
			}
		}
	}
}
