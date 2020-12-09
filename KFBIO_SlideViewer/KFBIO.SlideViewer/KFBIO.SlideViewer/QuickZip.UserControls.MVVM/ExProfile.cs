using QuickZip.Converters;
using QuickZip.UserControls.Logic.Tools.IconExtractor;
using QuickZip.UserControls.MVVM.Command.Model;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.UserControls.MVVM.Notification.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace QuickZip.UserControls.MVVM
{
	public class ExProfile : Profile<System.IO.FileInfoEx, System.IO.DirectoryInfoEx, System.IO.FileSystemInfoEx>
	{
		public ExProfile()
			: base((IconExtractor<System.IO.FileSystemInfoEx>)new ExIconExtractor())
		{
            base.RootDirectories = new System.IO.DirectoryInfoEx[1]
            {
                System.IO.DirectoryInfoEx.DesktopDirectory
            };
        }

		public override DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> ConstructDirectoryModel(DirectoryInfoEx dir)
		{
			return new ExDirectoryModel(dir);
		}

		public override FileModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> ConstructFileModel(FileInfoEx file)
		{
			return new ExFileModel(file);
		}

		public override FileSystemInfoEx ConstructEntry(string parseName)
		{
			return FileSystemInfoEx.FromString(parseName);
		}

		public override string GetDiskPath(FileSystemInfoEx entry, out bool isDir, bool createNowIfNotExist = true)
		{
			isDir = (entry is DirectoryInfoEx);
			return entry.FullName;
		}

		public override IEnumerable<Suggestion> Lookup(string lookupText)
		{
			yield return new Suggestion(lookupText + "_FileExplorer2");
		}

		public override IEnumerable<GenericMetadataModel> GetMetadata(EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>[] appliedModels)
		{
			if (appliedModels.Length == 1)
			{
				EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> model = appliedModels[0];
				FileSystemInfoEx entry = model.EmbeddedEntry;
				yield return new EntryMetadataModel<string, FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>(appliedModels, model.Label, "Key_Selected_0_Label");
				if (entry is FileInfoEx)
				{
					yield return new EntryMetadataModel<string, FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>(appliedModels, UITools.SizeInK((ulong)model.Length), "Key_Selected_0_Size", "Selected size");
				}
				yield return new EntryMetadataModel<DateTime, FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>(appliedModels, DateTime.Now, "DateTime_Test", "Now");
			}
		}

		public override IEnumerable<CommandModel> GetCommands(EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>[] appliedModels)
		{
			yield return new GenericCommandModel(ApplicationCommands.Close);
		}

		public override string ShowContextmenu(Point screenPos, EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>[] appliedModels)
		{
			return null;
		}

		public override void ShowProperties(Point screenPos, EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>[] appliedModels)
		{
		}

		public override IEnumerable<NotificationSourceModel> GetNotificationSources()
		{
			yield break;
		}

		public override void Copy(FileSystemInfoEx[] entries, DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> directoryModel, bool allowThread = true)
		{
			throw new NotImplementedException();
		}

		public override void Link(FileSystemInfoEx[] entries, DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> directoryModel)
		{
			throw new NotImplementedException();
		}

		public override AddActions GetSupportedAddActions(FileSystemInfoEx[] entries, DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> directoryModel)
		{
			return AddActions.Copy;
		}

		public override DirectoryInfoEx CreateDirectory(DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> directoryModel, string name, string type)
		{
			throw new NotImplementedException();
		}
	}
}
