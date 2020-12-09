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
    /// RotateViewer.xaml 的交互逻辑
    /// </summary>
    public partial class RotateViewer : UserControl
    {
        public event RoutedEventHandler RotateHandler;

        public RotateViewer()
        {
            InitializeComponent();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.RotateHandler != null)
            {
                this.RotateHandler(sender, null);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            base.Visibility = Visibility.Collapsed;
        }
    }
}
