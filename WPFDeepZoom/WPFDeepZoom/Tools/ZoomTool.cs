using MultilayerZoom.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFDeepZoom
{
    internal class ZoomTool
    {
        public static List<int> listChoice;

        public static int iMaxSetZoom;

        public static float fMinSetZoom;

        public static float fTotalHeight;

        public static float ScaleToZoom(MultiScaleImage mSlide, float fScale, float fScreenRatio)
        {
            return (float)Math.Round(fScale * (float)mSlide.Source.sbcCurrentSlide.dMaxActualZoom * fScreenRatio, 6);
        }

        public static float ZoomToScale(MultiScaleImage mSlide, float fZoom, float fScreenRatio)
        {
            return fZoom / fScreenRatio / (float)mSlide.Source.sbcCurrentSlide.dMaxActualZoom;
        }

        public static float GetTopValue(float fValue)
        {
            if (fMinSetZoom < 1f)
            {
                return (float)(30.0 * Math.Log(fValue) / Math.Log(2.0)) + 30f * (1f - fMinSetZoom);
            }
            return (float)(30.0 * Math.Log(fValue) / Math.Log(2.0));
        }

        public static bool ResetZoomTool(MultiScaleImage mSlide, StackPanel sZoomTool, Canvas cChoicesBox, Line lBGLine, Line lValueLine, int iDefaultAngle, float fScreenRatio, ResourceDictionary rdDirectory)
        {
            Grid grid = mSlide.Parent as Grid;
            if (mSlide.Source == null)
            {
                sZoomTool.Visibility = Visibility.Collapsed;
                return false;
            }
            sZoomTool.Visibility = Visibility.Visible;
            listChoice = new List<int>
            {
                1,
                2,
                4,
                10
            };
            if (cChoicesBox.Children.Count > 4)
            {
                cChoicesBox.Children.RemoveRange(4, cChoicesBox.Children.Count - 1);
            }
            double dMaxActualZoom = mSlide.Source.sbcCurrentSlide.dMaxActualZoom;
            if (dMaxActualZoom >= 120.0)
            {
                iMaxSetZoom = 160;
                listChoice.AddRange(new List<int>
                {
                    20,
                    40,
                    80,
                    120,
                    160
                });
            }
            else if (dMaxActualZoom >= 80.0)
            {
                iMaxSetZoom = 120;
                listChoice.AddRange(new List<int>
                {
                    20,
                    40,
                    80,
                    120
                });
            }
            else if (dMaxActualZoom >= 40.0)
            {
                iMaxSetZoom = 80;
                listChoice.AddRange(new List<int>
                {
                    20,
                    40,
                    80
                });
            }
            else if (dMaxActualZoom >= 20.0)
            {
                iMaxSetZoom = 40;
                listChoice.AddRange(new List<int>
                {
                    20,
                    40
                });
            }
            else
            {
                iMaxSetZoom = 20;
                listChoice.Add(20);
            }
            float num = (float)grid.ActualWidth;
            float num2 = (float)grid.ActualHeight;
            if (iDefaultAngle / 90 % 2 == 1)
            {
                num = (float)grid.ActualHeight;
                num2 = (float)grid.ActualWidth;
            }
            float num3 = (float)Math.Min((double)num / mSlide.Source.ImageSize.Width, (double)num2 / mSlide.Source.ImageSize.Height);
            mSlide.OriginalScale = num3;
            mSlide.MaxScaleRelativeToMaxSize = (double)iMaxSetZoom / mSlide.Source.sbcCurrentSlide.dMaxActualZoom / (double)fScreenRatio;
            float fScale = (float)mSlide.minScaleRelativeToMinSize * num3;
            fMinSetZoom = ScaleToZoom(mSlide, fScale, fScreenRatio);
            fTotalHeight = GetTopValue(iMaxSetZoom) + 10f;
            cChoicesBox.Height = fTotalHeight;
            lBGLine.Y2 = fTotalHeight;
            lValueLine.Y1 = fTotalHeight;
            foreach (int item in listChoice)
            {
                Label label = new Label();
                label.Margin = new Thickness(-35.0, fTotalHeight - GetTopValue(item) - 18f, 0.0, 0.0);
                label.SetValue(FrameworkElement.StyleProperty, rdDirectory["ZoomToolChoices"]);
                label.Content = item;
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 0, 0));
                line.StrokeThickness = 1.0;
                line.X1 = 5.0;
                line.X2 = 15.0;
                line.Y1 = fTotalHeight - GetTopValue(item) - 5f;
                line.Y2 = fTotalHeight - GetTopValue(item) - 5f;
                cChoicesBox.Children.Add(label);
                cChoicesBox.Children.Add(line);
            }
            float currentScale = mSlide.GetCurrentScale();
            ScaleToZoom(mSlide, currentScale, fScreenRatio);
            return true;
        }

        public static void UpdateZoomValue(MultiScaleImage mSlide, Label lZoomValue, Rectangle rController, Line lValueLine, float fScreenRatio, Label lOne, Label lDefault)
        {
            if (mSlide.Source != null)
            {
                float num = ScaleToZoom(mSlide, mSlide.fTargetScale, fScreenRatio);
                num = (float)Math.Round(num, 2);
                float num2 = 0f;
                num2 = ((!(num >= 1f)) ? (fTotalHeight - GetTopValue(1f) - 10f + 30f * (1f - num)) : (fTotalHeight - GetTopValue(num) - 10f));
                lZoomValue.Margin = new Thickness(20.0, num2 - 5f, 0.0, 0.0);
                rController.Margin = new Thickness(0.0, num2, 0.0, 0.0);
                lZoomValue.Content = num.ToString("0.00") + "X";
                lValueLine.Y2 = num2;
                float num3 = (float)Math.Round(mSlide.Source.sbcCurrentSlide.dMaxActualZoom, 2);
                if (num > num3)
                {
                    lZoomValue.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, 0, 0));
                    lOne.Background = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                    lOne.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                }
                else if (num < num3)
                {
                    lZoomValue.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                    lOne.Background = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                    lOne.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                }
                else
                {
                    lZoomValue.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                    lOne.Background = new SolidColorBrush(Color.FromRgb(57, 176, 181));
                    lOne.Foreground = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                }
                float num4 = ScaleToZoom(mSlide, (float)mSlide.OriginalScale, fScreenRatio);
                num4 = (float)Math.Round(num4, 2);
                if (num == num4)
                {
                    lDefault.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Image/home_active.png", UriKind.RelativeOrAbsolute)));
                }
                else
                {
                    lDefault.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Image/home.png", UriKind.RelativeOrAbsolute)));
                }
            }
        }
    }

}
