using KFBIO.Common;
using KFBIO.SlideViewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Xceed.Wpf.AvalonDock.Layout;

namespace QuickZip.UserControls.MVVM.Model
{
	public class ExFileModel : FileModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>
	{
		private FileInfoEx files;

		public ExFileModel(FileInfoEx file)
			: base(file)
		{
			files = file;
		}

		public override bool Open()
		{
			if (KCommon.CheckVersion(base.EmbeddedEntry.FullName))
			{
				DockWindow dockWindow = System.Windows.Application.Current.MainWindow as DockWindow;
				if (Setting.IsSingleFile == "1" && Setting.TabsDic.Count > 0)
				{
					foreach (KeyValuePair<object, object> item in Setting.TabsDic)
					{
						if (((Mainpage)item.Value).FilePath == base.EmbeddedEntry.FullName)
						{
							((LayoutDocument)item.Key).IsActive = true;
							((LayoutDocument)item.Key).IsSelected = true;
							return true;
						}
					}
				}
				Mainpage content = new Mainpage(base.EmbeddedEntry.FullName);
				string title = base.EmbeddedEntry.Name.Substring(0, base.EmbeddedEntry.Name.Length - 4);
				BitmapImage kFBThumnail = KCommon.GetKFBThumnail(base.EmbeddedEntry.FullName);
				if (kFBThumnail.StreamSource == null)
				{
					MessageWind messageWind = new MessageWind(MessageBoxButton.OK, System.Windows.Application.Current.MainWindow, dockWindow.languageSetter.LanguageResource["Filedamage"], dockWindow.languageSetter.LanguageResource["Prompt"], MessageBoxIcon.Exclamation, false);
					messageWind.ShowDialog();
					return false;
				}
				dockWindow.AddItem(base.EmbeddedEntry.Name + DateTime.Now.ToString("yyyyMMddhhmmss"), title, content, kFBThumnail);
				return true;
			}
			return false;
		}

		public override void Refresh(bool full = true)
		{
			base.Name = base.EmbeddedFile.Name;
			base.Label = base.EmbeddedFile.Label;
			base.ParseName = base.EmbeddedFile.FullName;
			if (full)
			{
				base.Length = base.EmbeddedFile.Length;
				base.CreationTime = base.EmbeddedFile.CreationTime;
				base.LastAccessTime = base.EmbeddedFile.LastAccessTime;
				base.LastWriteTime = base.EmbeddedFile.LastWriteTime;
				base.EntryTypeName = "File";
			}
		}

		public override bool Equals(EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> other)
		{
			return base.EmbeddedEntry.FullName.Equals(other.ParseName);
		}

		public override bool HasParent(DirectoryInfoEx directory)
		{
			return IOTools.HasParent(base.EmbeddedEntry, directory);
		}

		public override void Rename(string newName)
		{
			IOTools.Rename(base.EmbeddedFile.FullName, newName);
		}

		protected override DirectoryInfoEx getParent()
		{
			return base.EmbeddedFile.Parent;
		}

		public override void Delete()
		{
			throw new NotImplementedException();
		}
	}
}
