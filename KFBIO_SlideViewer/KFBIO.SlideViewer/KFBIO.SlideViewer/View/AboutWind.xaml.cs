using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace KFBIO.SlideViewer
{
    /// <summary>
    /// AboutWind.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWind : Window
    {
        public AboutWind()
        {
            InitializeComponent();
            _VersionLabel.Content = $"Code {Assembly.GetExecutingAssembly().GetName().Version}";
            _CopyRight.Content = $"© Copyright 2013 - {DateTime.Now.Year} KFBIO.  All Rights Reserved.";
        }

        private void _CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
