using QuickZip.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace QuickZip.UserControls.MVVM.Model
{
	public class ExDirectoryModel : DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>
	{
		internal static ExIconExtractor _iconExtractor = new ExIconExtractor();

		public ExDirectoryModel(DirectoryInfoEx dir)
			: base(dir)
		{
			base.IsSupportAdd = true;
		}

		public override bool Open()
		{
			Process.Start(base.EmbeddedEntry.FullName);
			return true;
		}

		public override void Refresh(bool full = true)
		{
			base.Name = base.EmbeddedDirectory.Name;
			base.Label = base.EmbeddedDirectory.Label;
			base.ParseName = base.EmbeddedDirectory.FullName;
			if (full)
			{
				base.EntryTypeName = "Directory";
				base.HasSubDirectories = base.EmbeddedDirectory.HasSubFolder;
				base.CreationTime = base.EmbeddedDirectory.CreationTime;
				base.LastAccessTime = base.EmbeddedDirectory.LastAccessTime;
				base.LastWriteTime = base.EmbeddedDirectory.LastWriteTime;
			}
		}

		protected override IEnumerable<EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>> getSubEntries(string filter = "*", bool showDirectory = true, bool showFile = true)
		{
			IEnumerator<FileSystemInfoEx> enumerator = base.EmbeddedDirectory.EnumerateFileSystemInfos(filter).GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(enumerator.Current is FileInfoEx) && !(enumerator.Current is DirectoryInfoEx))
				{
					continue;
				}
				if (enumerator.Current is FileInfoEx && showFile)
				{
					FileInfoEx f = (FileInfoEx)enumerator.Current;
					if (f.Extension.ToLower() != ".kfb" && f.Extension.ToLower() != ".ndpi" && f.Extension.ToLower() != ".svs")
					{
						yield return null;
					}
					else
					{
						yield return new ExFileModel(enumerator.Current as FileInfoEx);
					}
				}
				else if (enumerator.Current is DirectoryInfoEx && showDirectory)
				{
					yield return new ExDirectoryModel(enumerator.Current as DirectoryInfoEx);
				}
			}
		}

		public override bool Equals(EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> other)
		{
			if (other == null)
			{
				return false;
			}
			if (base.ParseName.Equals(other.ParseName))
			{
				return true;
			}
			if (base.EmbeddedEntry.Equals(other.EmbeddedEntry))
			{
				return true;
			}
			return false;
		}

		public override bool HasParent(DirectoryInfoEx directory)
		{
			return IOTools.HasParent(base.EmbeddedEntry, directory);
		}

		public override bool HasChild(FileSystemInfoEx entry)
		{
			return IOTools.HasParent(entry, base.EmbeddedDirectory);
		}

		public override void Rename(string newName)
		{
			IOTools.Rename(base.EmbeddedDirectory.FullName, newName);
		}

		public override string ToString()
		{
			return base.ParseName;
		}

		protected override DirectoryInfoEx getParent()
		{
			return base.EmbeddedEntry.Parent;
		}

		public override void Delete()
		{
			throw new NotImplementedException();
		}

		public override void Delete(FileSystemInfoEx[] entries)
		{
			throw new NotImplementedException();
		}
	}
}
