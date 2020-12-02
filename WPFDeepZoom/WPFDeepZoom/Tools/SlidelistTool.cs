using SliceInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPFDeepZoom
{
    public class SlidelistTool
    {
        public static void CreateSlideList(StackPanel sSlideList, string strFolderPath, ZYPSlice zypSlide, CancellationTokenSource ctsSource, ResourceDictionary rdDirectory, string strCurrentSlidePath)
        {
            ArrayList slideFileList = GetSlideFileList(strFolderPath, ".zyp");
            if (slideFileList.Count != 0)
            {
                foreach (string item in slideFileList)
                {
                    SlideListItem slideListItem = new SlideListItem();
                    slideListItem.strFileName = item;
                    MemoryStream thumbnail = zypSlide.GetThumbnail(item);
                    MemoryStream label = zypSlide.GetLabel(item);
                    if (thumbnail != null && label != null)
                    {
                        ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                        slideListItem.imgSourceThumbnail = (ImageSource)imageSourceConverter.ConvertFrom(thumbnail);
                        slideListItem.imgSourceLabel = (ImageSource)imageSourceConverter.ConvertFrom(label);
                        if (slideListItem != null)
                        {
                            int count = sSlideList.Children.Count;
                            Border border = new Border();
                            border.SetValue(FrameworkElement.StyleProperty, rdDirectory["SlideListItemBorder"]);
                            border.DataContext = slideListItem.strFileName;
                            Canvas canvas = new Canvas();
                            Border border2 = new Border();
                            border2.SetValue(FrameworkElement.StyleProperty, rdDirectory["SlideListItemThumbnailBorder"]);
                            border2.RenderTransform = new RotateTransform(-90.0);
                            Image image = new Image();
                            image.SetValue(FrameworkElement.StyleProperty, rdDirectory["SlideListItemThumbnail"]);
                            image.Source = slideListItem.imgSourceThumbnail;
                            RotateTransform rotateTransform = new RotateTransform();
                            rotateTransform.Angle = 180.0;
                            rotateTransform.CenterX = 47.5;
                            rotateTransform.CenterY = 100.0;
                            image.RenderTransform = rotateTransform;
                            border2.Child = image;
                            Image image2 = new Image();
                            image2.SetValue(FrameworkElement.StyleProperty, rdDirectory["SlideListItemLabel"]);
                            image2.Source = slideListItem.imgSourceLabel;
                            Label label2 = new Label();
                            label2.SetValue(FrameworkElement.StyleProperty, rdDirectory["SlideListItemSlideName"]);
                            label2.Content = Path.GetFileNameWithoutExtension(slideListItem.strFileName);
                            if (strCurrentSlidePath == slideListItem.strFileName)
                            {
                                border.BorderBrush = new SolidColorBrush(Color.FromRgb(57, 176, 181));
                                int num = (count - 1) * 130 + count * 10;
                                (sSlideList.Parent as ScrollViewer).ScrollToVerticalOffset(num - 10);
                            }
                            else
                            {
                                border.BorderBrush = Brushes.Gray;
                            }
                            canvas.Children.Add(border2);
                            canvas.Children.Add(image2);
                            canvas.Children.Add(label2);
                            border.Child = canvas;
                            if (ctsSource.IsCancellationRequested)
                            {
                                break;
                            }
                            sSlideList.Children.Add(border);
                        }
                    }
                }
            }
        }

        public static ArrayList GetSlideFileList(string strFolderPath, string strType)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(strFolderPath);
            string fullName = directoryInfo.FullName;
            if (!directoryInfo.Exists)
            {
                MessageBox.Show(GlobalVariable.listCurrentLanguage[115] + "!");
                return null;
            }
            ArrayList arrayList = new ArrayList();
            int num = 0;
            DirectoryInfo directoryInfo2 = null;
            try
            {
                directoryInfo2 = new DirectoryInfo(fullName);
                if (num != 0)
                {
                    return arrayList;
                }
                FileInfo[] files = directoryInfo2.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    if (Path.GetExtension(files[i].Name) == strType)
                    {
                        arrayList.Add(files[i].FullName);
                    }
                }
                return arrayList;
            }
            catch (UnauthorizedAccessException)
            {
                return arrayList;
            }
        }

        public static void SearchSlideList(string strFilter, StackPanel sSlideList)
        {
            if (sSlideList != null && sSlideList.Children != null && !(strFilter == "输入名称搜索"))
            {
                if (strFilter == "")
                {
                    foreach (Border child in sSlideList.Children)
                    {
                        _ = child.Child;
                        child.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    foreach (Border child2 in sSlideList.Children)
                    {
                        if (!((child2.Child as Canvas).Children[2] as Label).Content.ToString().Contains(strFilter))
                        {
                            child2.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            child2.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        public static void SetThumbnailBorderStyle(string strSlidePath, StackPanel sSlideList)
        {
            foreach (Border child in sSlideList.Children)
            {
                if ((string)child.DataContext == strSlidePath)
                {
                    child.BorderBrush = new SolidColorBrush(Color.FromRgb(57, 176, 181));
                }
                else
                {
                    child.BorderBrush = Brushes.Gray;
                }
            }
        }
    }
}
