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
    public class MagnifierTool
    {
        public static void MagnifierZoomAboutCenter(Border bMagnifierBorder, MainWindow mainWindow, Grid gAllSlideBox, float fScreenRatio)
        {
            Grid gMagnifierFlipBox = bMagnifierBorder.FindName("gMagnifierFlipBox") as Grid;
            if (bMagnifierBorder.Margin == default(Thickness))
            {
                float num = (float)(mainWindow.Width - 60.0 - bMagnifierBorder.Width) / 2f;
                float num2 = (float)(mainWindow.Height - 20.0 - bMagnifierBorder.Height) / 2f;
                bMagnifierBorder.Margin = new Thickness(num, num2, 0.0, 0.0);
                bMagnifierBorder.UpdateLayout();
            }
            Point point = new Point(bMagnifierBorder.Margin.Left + bMagnifierBorder.Width / 2.0, bMagnifierBorder.Margin.Top + bMagnifierBorder.Height / 2.0);
            Point pSlideBoxPixelPoint = new Point(0.0, 0.0);
            MultiScaleImage multiScaleImage = null;
            foreach (FrameworkElement child in gAllSlideBox.Children)
            {
                if (child is Border)
                {
                    Border border = child as Border;
                    multiScaleImage = ((border.Child as Grid).FindName("mMultiScaleImage") as MultiScaleImage);
                    if (multiScaleImage.Source != null)
                    {
                        Point point2 = border.TranslatePoint(new Point(0.0, 0.0), mainWindow);
                        if (new Rect(point2.X, point2.Y, border.ActualWidth, border.ActualHeight).Contains(point))
                        {
                            pSlideBoxPixelPoint = mainWindow.TransformToVisual(border).Transform(point);
                            break;
                        }
                    }
                }
            }
            if (multiScaleImage != null)
            {
                MagnifierZoomAboutPoint(gMagnifierFlipBox, multiScaleImage, bMagnifierBorder, pSlideBoxPixelPoint, fScreenRatio);
            }
        }

        public static void MagnifierZoomAboutPoint(Grid gMagnifierFlipBox, MultiScaleImage mSlide, Border bMagnifierBorder, Point pSlideBoxPixelPoint, float fScreenRatio)
        {
            MultiScaleImage multiScaleImage = gMagnifierFlipBox.FindName("mMagnifier") as MultiScaleImage;
            gMagnifierFlipBox.FindName("gMagnifierMarkBox");
            if (mSlide.Source == null)
            {
                multiScaleImage.Visibility = Visibility.Collapsed;
                return;
            }
            multiScaleImage.Visibility = Visibility.Visible;
            if (multiScaleImage.Source != mSlide.Source)
            {
                multiScaleImage.Source = mSlide.Source;
                GetMagnifierMark(mSlide, gMagnifierFlipBox, fScreenRatio);
            }
            float num = mSlide.fTargetScale * 2f;
            if (multiScaleImage.OriginalScale != (double)num)
            {
                float num2 = float.Parse(multiScaleImage.DataContext.ToString()) * num;
                multiScaleImage.minScaleRelativeToMinSize = 1.0;
                multiScaleImage.OriginalScale = num;
                multiScaleImage.MaxScaleRelativeToMaxSize = 4f * num;
                float currentScale = multiScaleImage.GetCurrentScale();
                multiScaleImage.ScaleCanvas(num2 / currentScale, new Point(bMagnifierBorder.Width / 2.0, bMagnifierBorder.Height / 2.0));
            }
            Point slideBoxFlipValue = PublicMethods.GetSlideBoxFlipValue(PublicMethods.FindSlideBox(mSlide));
            if (PublicMethods.GetElementFlipValue(gMagnifierFlipBox) != slideBoxFlipValue)
            {
                FlipTool.FlipElement(gMagnifierFlipBox, slideBoxFlipValue.X, slideBoxFlipValue.Y);
            }
            int elementActualAngle = PublicMethods.GetElementActualAngle(mSlide);
            if (PublicMethods.GetElementActualAngle(multiScaleImage) != elementActualAngle)
            {
                RotateMagnifier(multiScaleImage, elementActualAngle);
            }
            pSlideBoxPixelPoint = PublicMethods.FlipPoint(PublicMethods.FindSlideBox(mSlide), pSlideBoxPixelPoint);
            Point pPixelPoint = mSlide.BoxPixelToControlPixel(pSlideBoxPixelPoint);
            Point pNavigatorRatio = mSlide.ControlPixelToSlideRatio(pPixelPoint, false);
            Point pDeltaOffset = multiScaleImage.RatioOffsetToDeltaOffset(pNavigatorRatio);
            multiScaleImage.DeltaToPan(pDeltaOffset);
            Mark.UpdateAllMarks(gMagnifierFlipBox, fScreenRatio);
        }

        public static void GetMagnifierMark(MultiScaleImage mSlide, Grid gMagnifierFlipBox, float fScreenRatio)
        {
            MultiScaleImage multiScaleImage = gMagnifierFlipBox.FindName("mMagnifier") as MultiScaleImage;
            Grid grid = gMagnifierFlipBox.FindName("gMagnifierMarkBox") as Grid;
            if (mSlide.Source == multiScaleImage.Source)
            {
                grid.Children.Clear();
                Mark.GetMark(multiScaleImage, grid, fScreenRatio, 2);
                Mark.UpdateAllMarks(gMagnifierFlipBox, fScreenRatio);
                Grid grid2 = PublicMethods.FindSlideBox(mSlide).FindName("gMarkBox") as Grid;
                for (int i = 0; i < grid2.Children.Count; i++)
                {
                    grid.Children[i].Visibility = grid2.Children[i].Visibility;
                }
            }
        }

        public static void UpdateMagnifierMark(MultiScaleImage mSlide, Grid gMagnifierFlipBox)
        {
            MultiScaleImage multiScaleImage = gMagnifierFlipBox.FindName("mMagnifier") as MultiScaleImage;
            Grid grid = gMagnifierFlipBox.FindName("gMagnifierMarkBox") as Grid;
            if (mSlide.Source == multiScaleImage.Source)
            {
                Grid grid2 = PublicMethods.FindSlideBox(mSlide).FindName("gMarkBox") as Grid;
                for (int i = 0; i < grid2.Children.Count; i++)
                {
                    grid.Children[i].Visibility = grid2.Children[i].Visibility;
                }
            }
        }

        public static void RotateMagnifier(MultiScaleImage mMagnifier, int iTargetActualAngle)
        {
            RotateTransform renderTransform = new RotateTransform(iTargetActualAngle);
            mMagnifier.RenderTransformOrigin = new Point(0.5, 0.5);
            mMagnifier.RenderTransform = renderTransform;
            int acuteAngle = PublicMethods.GetAcuteAngle(iTargetActualAngle);
            float num = (float)Math.Sin(Math.PI / 180.0 * (double)acuteAngle);
            float num2 = (float)Math.Cos(Math.PI / 180.0 * (double)acuteAngle);
            float num3 = (float)mMagnifier.ActualWidth;
            float num4 = (float)mMagnifier.ActualHeight;
            float num5 = num3 * num2 + num4 * num;
            float num6 = num3 * num + num4 * num2;
            float num7 = (0f - (num5 - num3)) / 2f;
            float num8 = (0f - (num6 - num4)) / 2f;
            mMagnifier.Margin = new Thickness(num7, num8, num7, num8);
        }
    }

}
