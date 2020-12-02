using CommonClassLibrary;
using SliceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDeepZoom
{
    public class LabelTool
    {
        public static void UpdateSlideLabel(string strSlidePath, Image imgLabel, Canvas cLabelTool, ZYPSlice zypSlide)
        {
            if (strSlidePath != null)
            {
                cLabelTool.Visibility = Visibility.Visible;
                imgLabel.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(zypSlide.GetLabel(strSlidePath));
            }
            else
            {
                imgLabel.Source = null;
                cLabelTool.Visibility = Visibility.Collapsed;
            }
        }

        public static void ResetLabelTool(Image imgLabel, Border bLabelToggle, Canvas cLabelTool, Label lLabelToggle)
        {
            if (PublicMethods.GetElementActualAngle(imgLabel) != 0)
            {
                int num = 0;
                imgLabel.RenderTransformOrigin = new Point(0.5, 0.5);
                imgLabel.RenderTransform = new RotateTransform(num);
                imgLabel.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
                bLabelToggle.Margin = new Thickness(-12.0, 42.0, 0.0, 0.0);
            }
            cLabelTool.Width = 180.0;
            lLabelToggle.Content = ">";
            lLabelToggle.ToolTip = GlobalVariable.listCurrentLanguage[88];
        }
    }

}
