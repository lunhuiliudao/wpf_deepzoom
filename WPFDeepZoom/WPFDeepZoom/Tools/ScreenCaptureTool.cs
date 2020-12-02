using CommonClassLibrary;
using MultilayerZoom.Controls;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class ScreenCaptureTool
    {
        public static void InitializeTool(MultiScaleImage mSlide, Image imgPreview, Border bNavigation, Image imgDragTool, Image imgLabel, Polyline pShowMarksChecked, Polyline pShowRulerChecked, Polyline pShowNavigationChecked, Polyline pShowSlideLabelChecked, float fScreenRatio, ResourceDictionary rdDirectory)
        {
            Grid grid = (mSlide.Parent as Grid).Parent as Grid;
            if (grid.ActualHeight > 500.0)
            {
                imgPreview.Height = 500.0;
                imgPreview.Width = imgPreview.Height * grid.ActualWidth / grid.ActualHeight;
            }
            else
            {
                imgPreview.Height = grid.ActualHeight;
                imgPreview.Width = grid.ActualWidth;
            }
            imgPreview.Stretch = Stretch.Uniform;
            imgPreview.Source = GetPreviewPicture(grid, bNavigation, imgDragTool, imgLabel, pShowMarksChecked, pShowRulerChecked, pShowNavigationChecked, pShowSlideLabelChecked, fScreenRatio, rdDirectory);
        }

        public static void CameraOptionsChanged(MultiScaleImage mSlide, Image imgPreview, Border bNavigation, Image imgDragTool, Image imgLabel, Polyline pShowMarksChecked, Polyline pShowRulerChecked, Polyline pShowNavigationChecked, Polyline pShowSlideLabelChecked, float fScreenRatio, ResourceDictionary rdDirectory)
        {
            if (mSlide.Source != null)
            {
                Grid gSlideBox = (mSlide.Parent as Grid).Parent as Grid;
                imgPreview.Source = GetPreviewPicture(gSlideBox, bNavigation, imgDragTool, imgLabel, pShowMarksChecked, pShowRulerChecked, pShowNavigationChecked, pShowSlideLabelChecked, fScreenRatio, rdDirectory);
            }
        }

        public static ImageSource GetPreviewPicture(Grid gSlideBox, Border bNavigation, Image imgDragTool, Image imgLabel, Polyline pShowMarksChecked, Polyline pShowRulerChecked, Polyline pShowNavigationChecked, Polyline pShowSlideLabelChecked, float fScreenRatio, ResourceDictionary rdDirectory)
        {
            Border border = gSlideBox.FindName("bSlideNameBorder") as Border;
            Grid grid = gSlideBox.FindName("gMarkBox") as Grid;
            Grid grid2 = (gSlideBox.Parent as Border).Parent as Grid;
            border.Visibility = Visibility.Collapsed;
            SplitScreenTool.RemoveActiveStyle(gSlideBox);
            if (pShowMarksChecked.Visibility == Visibility.Collapsed)
            {
                grid.Visibility = Visibility.Collapsed;
            }
            StackPanel stackPanel = gSlideBox.FindName("sRulerToolBox") as StackPanel;
            if (pShowRulerChecked.Visibility == Visibility.Collapsed)
            {
                stackPanel.Visibility = Visibility.Collapsed;
            }
            if (pShowNavigationChecked.Visibility == Visibility.Visible)
            {
                NavigationPutInSlideBox(gSlideBox, bNavigation, imgDragTool, fScreenRatio, rdDirectory);
            }
            if (pShowSlideLabelChecked.Visibility == Visibility.Visible)
            {
                SlideLabelPutInSlideBox(gSlideBox, imgLabel, rdDirectory);
            }
            gSlideBox.UpdateLayout();
            BitmapSource result = SaveFrameworkElementToImage(gSlideBox, fScreenRatio);
            if (grid2.Children.Count > 1)
            {
                SplitScreenTool.AddActiveStyle(gSlideBox);
            }
            border.Visibility = Visibility.Visible;
            grid.Visibility = Visibility.Visible;
            stackPanel.Visibility = Visibility.Visible;
            Image image = gSlideBox.FindName("iNavigationPicture") as Image;
            if (image != null)
            {
                gSlideBox.Children.Remove(image);
                gSlideBox.UnregisterName("iNavigationPicture");
            }
            Image image2 = gSlideBox.FindName("iSlideLabelPicture") as Image;
            if (image2 != null)
            {
                gSlideBox.Children.Remove(image2);
                gSlideBox.UnregisterName("iSlideLabelPicture");
            }
            return result;
        }

        public static void NavigationPutInSlideBox(Grid gSlideBox, Border bNavigation, Image imgDragTool, float fScreenRatio, ResourceDictionary rdDirectory)
        {
            Image image = new Image();
            image.Width = bNavigation.Width;
            image.Height = bNavigation.Height;
            imgDragTool.Visibility = Visibility.Collapsed;
            image.Source = SaveFrameworkElementToImage(bNavigation, fScreenRatio);
            imgDragTool.Visibility = Visibility.Visible;
            image.SetValue(FrameworkElement.StyleProperty, rdDirectory["cameraNavigation"]);
            gSlideBox.Children.Add(image);
            gSlideBox.RegisterName("iNavigationPicture", image);
        }

        public static void SlideLabelPutInSlideBox(Grid gSlideBox, Image imgLabel, ResourceDictionary rdDirectory)
        {
            Image image = new Image();
            image.Source = imgLabel.Source;
            image.Width = imgLabel.Width;
            image.Height = imgLabel.Height;
            image.SetValue(FrameworkElement.StyleProperty, rdDirectory["cameraLabel"]);
            gSlideBox.Children.Add(image);
            int elementActualAngle = PublicMethods.GetElementActualAngle(imgLabel);
            image.RenderTransformOrigin = new Point(0.5, 0.5);
            image.RenderTransform = new RotateTransform(elementActualAngle);
            if (elementActualAngle / 90 % 2 == 1)
            {
                float num = (float)(imgLabel.Width - imgLabel.Height) / 2f;
                image.Margin = new Thickness(0.0, num, 0f - num, 0.0);
            }
            else
            {
                image.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
            }
            gSlideBox.RegisterName("iSlideLabelPicture", image);
        }

        public static BitmapSource SaveFrameworkElementToImage(FrameworkElement feUI, float fScreenRatio)
        {
            try
            {
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)(feUI.ActualWidth * (double)fScreenRatio), (int)(feUI.ActualHeight * (double)fScreenRatio), 96f * fScreenRatio, 96f * fScreenRatio, PixelFormats.Pbgra32);
                renderTargetBitmap.Render(feUI);
                return renderTargetBitmap;
            }
            catch
            {
                return null;
            }
        }

        public static void SaveImageToFile(BitmapSource bmpSourceImage, string strImgPath)
        {
            BitmapEncoder bitmapEncoder = null;
            bitmapEncoder = ((!System.IO.Path.GetExtension(strImgPath).ToLower().Equals(".png")) ? ((BitmapEncoder)new JpegBitmapEncoder()) : ((BitmapEncoder)new PngBitmapEncoder()));
            bitmapEncoder.Frames.Add(BitmapFrame.Create(bmpSourceImage));
            using (FileStream stream = new FileStream(strImgPath, FileMode.Create))
            {
                bitmapEncoder.Save(stream);
            }
        }
    }

}
