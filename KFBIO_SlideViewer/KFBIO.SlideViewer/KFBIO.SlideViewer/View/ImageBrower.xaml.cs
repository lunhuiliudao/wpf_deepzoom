using QuickZip.UserControls;
using QuickZip.UserControls.MVVM;
using QuickZip.UserControls.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KFBIO.SlideViewer
{
    /// <summary>
    /// ImageBrower.xaml 的交互逻辑
    /// </summary>
    public partial class ImageBrower : UserControl
    {
        public ExExplorerViewModel _evm;

        public ImageBrower()
        {
            InitializeComponent();
            ExProfile profile = new ExProfile();
            _evm = null;
            explr.DataContext = (_evm = new ExExplorerViewModel(profile));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RoutedEventHandler onLoaded = null;
            onLoaded = delegate
            {
                explr.Loaded -= onLoaded;
                FileList2 fileList = explr.Template.FindName("PART_FileList", explr) as FileList2;
                fileList.ViewMode = ViewMode.vmExtraLargeIcon;
                _evm.RegisterDragAndDrop(fileList);
            };
            explr.Loaded += onLoaded;
        }
    }
}
