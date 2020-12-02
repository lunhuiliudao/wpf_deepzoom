using CommonClassLibrary;
using MultilayerZoom.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WPFDeepZoom
{
    public class NavigatorTool
    {
        public static void UpdateNavigationViewport(MultiScaleImage mCurrentMultiScaleImage, MultiScaleImage mNavigatorImage, Rectangle rViewport, Line lHorLine, Line lVerLine)
        {
            if (mCurrentMultiScaleImage.Source != null)
            {
                float fTargetScale = mCurrentMultiScaleImage.fTargetScale;
                Point pTargetOffset = mCurrentMultiScaleImage.pTargetOffset;
                Grid grid = PublicMethods.FindSlideBox(mCurrentMultiScaleImage);
                Size imageSize = mCurrentMultiScaleImage.Source.ImageSize;
                float num = (float)(mNavigatorImage.ActualWidth * grid.ActualWidth / (imageSize.Width * (double)fTargetScale));
                float num2 = (float)((double)num * grid.ActualHeight / grid.ActualWidth);
                float num3 = (float)mCurrentMultiScaleImage.Margin.Left;
                float num4 = (float)mCurrentMultiScaleImage.Margin.Top;
                float num5 = (float)((pTargetOffset.X - (double)num3) * (double)num / grid.ActualWidth);
                float num6 = (float)((pTargetOffset.Y - (double)num4) * (double)num2 / grid.ActualHeight);
                Point point = new Point(num5 + num / 2f, 0f - (num6 + num2 / 2f));
                int elementActualAngle = PublicMethods.GetElementActualAngle(mCurrentMultiScaleImage);
                Point point2 = (elementActualAngle != 0) ? PublicMethods.GetRotatePoint(point, -elementActualAngle, new Point(mNavigatorImage.ActualWidth / 2.0, (0.0 - mNavigatorImage.ActualHeight) / 2.0)) : point;
                rViewport.SetValue(Canvas.LeftProperty, point2.X - (double)(num / 2f));
                rViewport.SetValue(Canvas.TopProperty, 0.0 - point2.Y - (double)(num2 / 2f));
                rViewport.Width = num;
                rViewport.Height = num2;
                if (num < 15f && num2 < 15f)
                {
                    rViewport.Visibility = Visibility.Collapsed;
                }
                else
                {
                    rViewport.Visibility = Visibility.Visible;
                }
                lHorLine.X1 = -1000.0;
                lHorLine.X2 = 3000.0;
                lHorLine.Y1 = 0.0 - point2.Y;
                lHorLine.Y2 = 0.0 - point2.Y;
                lVerLine.X1 = point2.X;
                lVerLine.X2 = point2.X;
                lVerLine.Y1 = -1000.0;
                lVerLine.Y2 = 3000.0;
            }
        }

        public static void ResetNavigatorSizeAndSource(MultiScaleImage mCurrentMultiScaleImage, MultiScaleImage mNavigationImage, ref float fNavigatorWidth, ref float fNavigatorHeight, float fScreenRatio)
        {
            Grid grid = mNavigationImage.Parent as Grid;
            Border border = (grid.Parent as Grid).Parent as Border;
            if (mCurrentMultiScaleImage.Source == null)
            {
                border.Visibility = Visibility.Hidden;
                return;
            }
            Size imageSize = mCurrentMultiScaleImage.Source.ImageSize;
            fNavigatorWidth = (float)imageSize.Width / 256f;
            fNavigatorHeight = (float)imageSize.Height / 256f;
            float num = fNavigatorHeight / fNavigatorWidth;
            if (fNavigatorWidth < 220f || fNavigatorHeight < 220f)
            {
                if (num > 1f)
                {
                    fNavigatorWidth = 220f;
                    fNavigatorHeight = 220f * num;
                }
                else
                {
                    fNavigatorHeight = 220f;
                    fNavigatorWidth = 220f / num;
                }
            }
            if (fNavigatorWidth > 300f || fNavigatorHeight > 300f)
            {
                if (num > 1f)
                {
                    fNavigatorWidth = 300f / num;
                    fNavigatorHeight = 300f;
                }
                else
                {
                    fNavigatorHeight = 300f * num;
                    fNavigatorWidth = 300f;
                }
            }
            fNavigatorWidth /= fScreenRatio;
            fNavigatorHeight /= fScreenRatio;
            border.Width = fNavigatorWidth + 4f;
            border.Height = fNavigatorHeight + 4f;
            grid.Width = fNavigatorWidth;
            grid.Height = fNavigatorHeight;
            grid.UpdateLayout();
            if (mNavigationImage.Source != mCurrentMultiScaleImage.Source)
            {
                mNavigationImage.Source = mCurrentMultiScaleImage.Source;
            }
            border.Visibility = Visibility.Visible;
        }
    }

}
