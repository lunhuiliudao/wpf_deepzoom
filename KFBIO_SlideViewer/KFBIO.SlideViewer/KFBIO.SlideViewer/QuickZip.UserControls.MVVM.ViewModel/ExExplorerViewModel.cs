using System.IO;

namespace QuickZip.UserControls.MVVM.ViewModel
{
	public class ExExplorerViewModel : HistoryExplorerViewModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>
	{
		public ExExplorerViewModel(ExProfile profile)
			: base((Profile<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>)profile)
		{
		}
	}
}
