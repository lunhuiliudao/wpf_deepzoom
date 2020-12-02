using MultilayerZoom.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFDeepZoom
{
    public class FlatTool
    {
        public static void ResetFlatTool(Grid gSlideBox)
        {
            StackPanel stackPanel = gSlideBox.FindName("sRulerToolBox") as StackPanel;
            StackPanel sFlatTool = stackPanel.FindName("sFlatTool") as StackPanel;
            MultiScaleImage mSlide = gSlideBox.FindName("mMultiScaleImage") as MultiScaleImage;
            sFlatTool.Children.Clear();
            if (mSlide.Source.sbcCurrentSlide.iScanMode != 1)
            {
                return;
            }
            double num = mSlide.Source.sbcCurrentSlide.iFlatNum;
            int num2 = (int)Math.Floor(num / 2.0);
            for (int i = 0; (double)i <= num - 1.0; i++)
            {
                Label label = new Label();
                if (i == num2)
                {
                    label.Background = new SolidColorBrush(Color.FromRgb(169, 169, 169));
                }
                else
                {
                    label.Background = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                }
                label.Width = 30.0;
                label.Height = 30.0;
                label.Cursor = Cursors.Hand;
                label.BorderBrush = Brushes.Gray;
                if ((double)i == num - 1.0)
                {
                    label.BorderThickness = new Thickness(1.0);
                }
                else
                {
                    label.BorderThickness = new Thickness(1.0, 1.0, 0.0, 1.0);
                }
                label.Content = i;
                label.HorizontalContentAlignment = HorizontalAlignment.Center;
                label.MouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e)
                {
                    Label label2 = sender as Label;
                    if (!(label2.Background.ToString() == "#FFA9A9A9"))
                    {
                        try
                        {
                            int iFlat = (int)label2.Content;
                            mSlide.ChangeFlat(iFlat);
                            foreach (Label child in sFlatTool.Children)
                            {
                                child.Background = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                            }
                            label2.Background = new SolidColorBrush(Color.FromRgb(169, 169, 169));
                        }
                        catch
                        {
                        }
                    }
                };
                sFlatTool.Children.Add(label);
            }
        }
    }

}
