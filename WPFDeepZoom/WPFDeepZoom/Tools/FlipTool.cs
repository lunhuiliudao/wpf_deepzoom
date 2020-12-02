using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDeepZoom
{
    public class FlipTool
    {
        public static void FlipElement(Grid gFlipBox, double dScaleX, double dScaleY)
        {
            ScaleTransform scaleTransform = new ScaleTransform();
            scaleTransform.ScaleX = dScaleX;
            scaleTransform.ScaleY = dScaleY;
            scaleTransform.CenterX = gFlipBox.ActualWidth / 2.0;
            scaleTransform.CenterY = gFlipBox.ActualHeight / 2.0;
            gFlipBox.RenderTransform = scaleTransform;
        }

        public static void SetMirrorOption(double dScaleX, double dScaleY, CheckBox cbFlipHorizontal, CheckBox cbFlipVertical, int iDefaultAngle)
        {
            cbFlipHorizontal.DataContext = "2";
            cbFlipVertical.DataContext = "2";
            if (iDefaultAngle / 90 % 2 == 1)
            {
                if (dScaleY == 1.0)
                {
                    cbFlipHorizontal.IsChecked = false;
                }
                else
                {
                    cbFlipHorizontal.IsChecked = true;
                }
                if (dScaleX == 1.0)
                {
                    cbFlipVertical.IsChecked = false;
                }
                else
                {
                    cbFlipVertical.IsChecked = true;
                }
            }
            else
            {
                if (dScaleX == 1.0)
                {
                    cbFlipHorizontal.IsChecked = false;
                }
                else
                {
                    cbFlipHorizontal.IsChecked = true;
                }
                if (dScaleY == 1.0)
                {
                    cbFlipVertical.IsChecked = false;
                }
                else
                {
                    cbFlipVertical.IsChecked = true;
                }
            }
            cbFlipVertical.DataContext = "1";
            cbFlipHorizontal.DataContext = "1";
        }

        public static void ChangeNavigationFlipCenter(Grid gBox)
        {
            if (gBox.RenderTransform is ScaleTransform)
            {
                ScaleTransform obj = gBox.RenderTransform as ScaleTransform;
                obj.CenterX = gBox.Width / 2.0;
                obj.CenterY = gBox.Height / 2.0;
            }
        }
    }

}
