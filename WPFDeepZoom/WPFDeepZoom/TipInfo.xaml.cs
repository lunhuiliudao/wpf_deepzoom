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
using System.Windows.Shapes;

namespace WPFDeepZoom
{
    /// <summary>
    /// TipInfo.xaml 的交互逻辑
    /// </summary>
    public partial class TipInfo : Window
    {
        public string strTitle;

        public string strTipInfo;

        public string strSureName;

        public string strCancleName;

        public TipInfo()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            labelTitle.Content = strTitle;
            labelTipInfo.Content = strTipInfo;
            lSure.Content = strSureName;
            lCancel.Content = strCancleName;
        }

        private void lSure_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DialogResult = true;
        }

        private void lCancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.DialogResult = false;
        }
    }
}
