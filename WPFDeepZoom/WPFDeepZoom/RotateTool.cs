using CommonClassLibrary;
using MultilayerZoom.Controls;
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
    internal class RotateTool
    {
        public static void RotateIcon(int iRelativeAngle, int iDefaultAngle, Image imgRotate, Label lAngleNum)
        {
            int num = iRelativeAngle - iDefaultAngle;
            if (num < 0)
            {
                num += 360;
            }
            num %= 360;
            lAngleNum.Content = num;
            RotateTransform renderTransform = new RotateTransform(num);
            Point point2 = imgRotate.RenderTransformOrigin = new Point(0.5, 0.5);
            imgRotate.RenderTransform = renderTransform;
        }

        public static void RotateNavigator(Border bNavigation, MultiScaleImage mNavigationImage, int iActualAngle, float fNavigatorWidth, float fNavigatorHeight)
        {
            try
            {
                RotateTransform renderTransform = new RotateTransform(iActualAngle);
                Point renderTransformOrigin = new Point(0.5, 0.5);
                int acuteAngle = PublicMethods.GetAcuteAngle(iActualAngle);
                float num = (float)Math.Sin(Math.PI / 180.0 * (double)acuteAngle);
                float num2 = (float)Math.Cos(Math.PI / 180.0 * (double)acuteAngle);
                float num3 = fNavigatorHeight * num + fNavigatorWidth * num2;
                float num4 = fNavigatorHeight * num2 + fNavigatorWidth * num;
                bNavigation.Width = 4f + num3;
                bNavigation.Height = 4f + num4;
                mNavigationImage.RenderTransformOrigin = renderTransformOrigin;
                mNavigationImage.RenderTransform = renderTransform;
            }
            catch
            {
            }
        }

        public static void RotateSlide(Grid gSlideBox, int iTargetActualAngle)
        {
            MultiScaleImage multiScaleImage = gSlideBox.FindName("mMultiScaleImage") as MultiScaleImage;
            RotateTransform renderTransform = new RotateTransform(iTargetActualAngle);
            int acuteAngle = PublicMethods.GetAcuteAngle(iTargetActualAngle);
            float num = (float)Math.Sin(Math.PI / 180.0 * (double)acuteAngle);
            float num2 = (float)Math.Cos(Math.PI / 180.0 * (double)acuteAngle);
            float num3 = (0f - (float)(multiScaleImage.ActualWidth - gSlideBox.ActualWidth)) / 2f;
            float num4 = (0f - (float)(multiScaleImage.ActualHeight - gSlideBox.ActualHeight)) / 2f;
            multiScaleImage.RenderTransformOrigin = new Point(0.5, 0.5);
            multiScaleImage.RenderTransform = renderTransform;
            float num5 = (float)gSlideBox.ActualWidth;
            float num6 = (float)gSlideBox.ActualHeight;
            float num7 = num5 * num2 + num6 * num;
            float num8 = num5 * num + num6 * num2;
            float num9 = (0f - (num7 - num5)) / 2f;
            float num10 = (0f - (num8 - num6)) / 2f;
            multiScaleImage.Margin = new Thickness(num9, num10, num9, num10);
            Point pDeltaOffset = default(Point);
            if (multiScaleImage.Source != null)
            {
                pDeltaOffset.X = 0f - num3 + num9;
                pDeltaOffset.Y = 0f - num4 + num10;
                multiScaleImage.DeltaToPan(pDeltaOffset, false, 2);
            }
        }
    }

}
