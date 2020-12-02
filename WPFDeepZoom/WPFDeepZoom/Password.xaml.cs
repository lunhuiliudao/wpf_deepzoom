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
    /// Password.xaml 的交互逻辑
    /// </summary>
    public partial class Password : Window
    {
        public Password()
        {
            InitializeComponent();
            base.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            pbPassword.Focus();
            labelPermission.Content = GlobalVariable.listCurrentLanguage[119];
            lSure.Content = GlobalVariable.listCurrentLanguage[98];
            lCancel.Content = GlobalVariable.listCurrentLanguage[99];
        }

        private void SurePassword()
        {
            if (pbPassword.Password == "Williswin")
            {
                pbPassword.Password = "";
                base.DialogResult = true;
            }
            else
            {
                MessageBox.Show("密码错误!");
                pbPassword.Password = "";
            }
        }

        private void lSure_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SurePassword();
        }

        private void lCancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pbPassword.Password = "";
            base.DialogResult = false;
        }

        private void gPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SurePassword();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
