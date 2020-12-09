using KFBIO.Common;
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

namespace KFBIO.SlideViewer
{
    /// <summary>
    /// SetSysWind.xaml 的交互逻辑
    /// </summary>
    public partial class SetSysWind : Window
    {
        public event RoutedEventHandler MaxMagRefresh;

        public event RoutedEventHandler LabelRefresh;

        public event RoutedEventHandler RuleRefresh;

        public event RoutedEventHandler NavRefresh;

        public event RoutedEventHandler LangRefresh;

        public event RoutedEventHandler RotateRefresh;

        public event RoutedEventHandler MagnifierRefresh;

        public event RoutedEventHandler OperateballRefresh;

        public event RoutedEventHandler MagnifierCheckRefresh;

        public SetSysWind()
        {
            InitializeComponent();
            MainNavigationAccordion.SelectionMode = AccordionSelectionMode.OneOrMore;
            StartShow.IsSelected = true;
            NavShow.IsSelected = true;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                base.Visibility = Visibility.Collapsed;
            }
        }

        private void _CloseButton_Click(object sender, RoutedEventArgs e)
        {
            base.Visibility = Visibility.Collapsed;
        }

        private void _ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txt_Scale != null)
            {
                txt_Scale.Text = e.NewValue + "X";
                Setting.MaxMagValue = (int)e.NewValue;
                if (this.MaxMagRefresh != null)
                {
                    this.MaxMagRefresh(sender, e);
                }
            }
        }

        private void Ck_Label_Click(object sender, RoutedEventArgs e)
        {
            if (this.LabelRefresh != null)
            {
                this.LabelRefresh(sender, e);
            }
            if (Ck_Label.IsChecked == true)
            {
                Setting.IsLabel = "1";
            }
            else
            {
                Setting.IsLabel = "0";
            }
        }

        private void Ck_Rule_Click(object sender, RoutedEventArgs e)
        {
            if (this.RuleRefresh != null)
            {
                this.RuleRefresh(sender, e);
            }
            if (Ck_Rule.IsChecked == true)
            {
                Setting.IsRule = "1";
            }
            else
            {
                Setting.IsRule = "0";
            }
        }

        private void Ck_Nav_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavRefresh != null)
            {
                this.NavRefresh(sender, e);
            }
            if (Ck_Nav.IsChecked == true)
            {
                Setting.IsNav = "1";
            }
            else
            {
                Setting.IsNav = "0";
            }
        }

        private void Ck_Operateball_Click(object sender, RoutedEventArgs e)
        {
            if (this.OperateballRefresh != null)
            {
                this.OperateballRefresh(sender, e);
            }
            if (Ck_Operateball.IsChecked == true)
            {
                Setting.IsOperateball = "1";
            }
            else
            {
                Setting.IsOperateball = "0";
            }
        }

        private void Ck_Magnifier_Click(object sender, RoutedEventArgs e)
        {
            if (this.MagnifierRefresh != null)
            {
                this.MagnifierRefresh(sender, e);
            }
            if (Ck_Magnifier.IsChecked == true)
            {
                Setting.IsMagnifier = "1";
            }
            else
            {
                Setting.IsMagnifier = "0";
            }
        }

        private void Ck_Rotate_Click(object sender, RoutedEventArgs e)
        {
            if (this.RotateRefresh != null)
            {
                this.RotateRefresh(sender, e);
            }
            if (Ck_Rotate.IsChecked == true)
            {
                Setting.IsRotate = "1";
            }
            else
            {
                Setting.IsRotate = "0";
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.LangRefresh != null)
            {
                this.LangRefresh(sender, e);
            }
        }

        private void MarRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.MagnifierCheckRefresh != null)
            {
                this.MagnifierCheckRefresh(sender, e);
            }
        }

    }
}
