using CommonClassLibrary;
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
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace WPFDeepZoom
{
    public class ViewRectTool
    {
        public static void CloseViewRectAnimation(Grid gSlideBox)
        {
            if ((gSlideBox.FindName("zcGrid") as ZoomableCanvas).Children.Count != 0)
            {
                Border obj = gSlideBox.FindName("bGridBox") as Border;
                Thickness margin = obj.Margin;
                obj.ApplyAnimationClock(FrameworkElement.MarginProperty, null);
                obj.Margin = margin;
            }
        }

        public static void UpdateViewRect(Grid gSlideBox, bool bScaleChanged, bool animate = false, float fDurationTime = 0f, EasingFunctionBase easing = null)
        {
            MultiScaleImage mSlide = gSlideBox.FindName("mMultiScaleImage") as MultiScaleImage;
            ZoomableCanvas zcCycleGrid = gSlideBox.FindName("zcGrid") as ZoomableCanvas;
            Border border = gSlideBox.FindName("bGridBox") as Border;
            float fTargetScale = mSlide.fTargetScale;
            if (zcCycleGrid.Children.Count == 0)
            {
                return;
            }
            System.Windows.Point point = mSlide.SlideRatioToBoxPixel(new System.Windows.Point(0.0, 0.0));
            int elementActualAngle = PublicMethods.GetElementActualAngle(mSlide);
            if (!animate)
            {
                zcCycleGrid.ApplyAnimationClock(ZoomableCanvas.ScaleProperty, null);
                border.ApplyAnimationClock(FrameworkElement.MarginProperty, null);
                border.Margin = new Thickness(point.X, point.Y, 0.0, 0.0);
                zcCycleGrid.Scale = fTargetScale;
                if (bScaleChanged)
                {
                    foreach (System.Windows.Shapes.Rectangle child in zcCycleGrid.Children)
                    {
                        child.StrokeThickness = 1f / fTargetScale;
                    }
                }
            }
            else
            {
                border.BeginAnimation(FrameworkElement.MarginProperty, new ThicknessAnimation(new Thickness(point.X, point.Y, 0.0, 0.0), TimeSpan.FromMilliseconds(fDurationTime))
                {
                    EasingFunction = easing
                }, HandoffBehavior.Compose);
                if (bScaleChanged)
                {
                    DoubleAnimation doubleAnimation = new DoubleAnimation(fTargetScale, TimeSpan.FromMilliseconds(fDurationTime))
                    {
                        EasingFunction = easing
                    };
                    int i = 0;
                    doubleAnimation.CurrentTimeInvalidated += delegate
                    {
                        Task.Factory.StartNew(delegate
                        {
                            i++;
                            if (i % 3 == 0)
                            {
                                float fCurrentScale = 0f;
                                zcCycleGrid.Dispatcher.BeginInvoke((Action)delegate
                                {
                                    fCurrentScale = mSlide.GetCurrentScale();
                                    foreach (System.Windows.Shapes.Rectangle child2 in zcCycleGrid.Children)
                                    {
                                        child2.StrokeThickness = 1f / fCurrentScale;
                                    }
                                }, DispatcherPriority.Loaded);
                            }
                        }, TaskCreationOptions.AttachedToParent);
                    };
                    zcCycleGrid.BeginAnimation(ZoomableCanvas.ScaleProperty, doubleAnimation, HandoffBehavior.Compose);
                }
            }
            System.Windows.Point point3 = border.RenderTransformOrigin = new System.Windows.Point(0.0, 0.0);
            border.RenderTransform = new RotateTransform(elementActualAngle);
        }

        public static void CreateAllSlideViewRect(Grid gAllSlideBox, Border bViewInfoBorder, Grid gViewInfo)
        {
            for (int i = 0; i < gAllSlideBox.Children.Count; i++)
            {
                if (gAllSlideBox.Children[i] is Border)
                {
                    Grid grid = (gAllSlideBox.Children[i] as Border).Child as Grid;
                    if ((grid.FindName("mMultiScaleImage") as MultiScaleImage).Source != null)
                    {
                        CreateSingleSlideViewRect(grid, bViewInfoBorder, gViewInfo);
                        UpdateViewRect(grid, true);
                    }
                }
            }
        }

        public static void CreateSingleSlideViewRect(Grid gSlideBox, Border bViewInfoBorder, Grid gViewInfo)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            MultiScaleImage multiScaleImage = gSlideBox.FindName("mMultiScaleImage") as MultiScaleImage;
            Border obj = gSlideBox.FindName("bGridBox") as Border;
            ZoomableCanvas zoomableCanvas = gSlideBox.FindName("zcGrid") as ZoomableCanvas;
            obj.Visibility = Visibility.Visible;
            System.Windows.Size imageSize = multiScaleImage.Source.ImageSize;
            zoomableCanvas.Width = imageSize.Width;
            zoomableCanvas.Height = imageSize.Height;
            dictionary = multiScaleImage.Source.sbcCurrentSlide.GetAllViewInfo();
            System.Drawing.Size viewSize = multiScaleImage.Source.sbcCurrentSlide.GetViewSize();
            string[] array = new string[7];
            foreach (string value in dictionary.Values)
            {
                array = value.Split(new string[1]
                {
                    "|"
                }, StringSplitOptions.RemoveEmptyEntries);
                System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                ViewRectInfo viewRectInfo = new ViewRectInfo();
                viewRectInfo.fXMotorPos = float.Parse(array[3]);
                viewRectInfo.fYMotorPos = float.Parse(array[4]);
                viewRectInfo.fZMotorPos = float.Parse(array[5]);
                if (float.Parse(array[6]) == 1f)
                {
                    viewRectInfo.strAutoFocus = GlobalVariable.listCurrentLanguage[116];
                    rectangle.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, 0, 0));
                    rectangle.SetValue(Panel.ZIndexProperty, 2);
                }
                else
                {
                    viewRectInfo.strAutoFocus = GlobalVariable.listCurrentLanguage[117];
                    rectangle.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 176, 181));
                }
                rectangle.DataContext = viewRectInfo;
                System.Windows.Point pPixelPoint = new System.Windows.Point(float.Parse(array[1]), float.Parse(array[2]));
                System.Windows.Point point = multiScaleImage.ScanPixelToSlideRatio(pPixelPoint);
                rectangle.HorizontalAlignment = HorizontalAlignment.Left;
                rectangle.VerticalAlignment = VerticalAlignment.Top;
                rectangle.StrokeThickness = 1.0;
                rectangle.Width = (float)viewSize.Width;
                rectangle.Height = (float)viewSize.Height;
                rectangle.Margin = new Thickness(point.X * imageSize.Width, point.Y * imageSize.Height, 0.0, 0.0);
                rectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, byte.MaxValue, byte.MaxValue, byte.MaxValue));
                rectangle.MouseMove += delegate (object sender, MouseEventArgs e)
                {
                    if (bViewInfoBorder.Visibility != Visibility.Collapsed)
                    {
                        ViewRectInfo dataContext = (sender as System.Windows.Shapes.Rectangle).DataContext as ViewRectInfo;
                        gViewInfo.DataContext = dataContext;
                    }
                };
                zoomableCanvas.Children.Add(rectangle);
            }
        }

        public static void DeleteSingleSlideViewRect(Grid gSlideBox)
        {
            Border border = gSlideBox.FindName("bGridBox") as Border;
            ZoomableCanvas obj = gSlideBox.FindName("zcGrid") as ZoomableCanvas;
            border.Visibility = Visibility.Collapsed;
            obj.Children.Clear();
        }

        public static void DeleteAllSlideViewRect(Grid gAllSlideBox)
        {
            foreach (FrameworkElement child in gAllSlideBox.Children)
            {
                if (child is Border)
                {
                    DeleteSingleSlideViewRect((child as Border).Child as Grid);
                }
            }
        }
    }

}
