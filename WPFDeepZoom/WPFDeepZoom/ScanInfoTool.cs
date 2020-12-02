using MultilayerZoom.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFDeepZoom
{
    public class ScanInfoTool
    {
        public static void GetSlideScanInfo(MultiScaleImage mSlide, StackPanel sDefaultScanInfo, Grid gScanInfo)
        {
            if (mSlide.Source == null)
            {
                sDefaultScanInfo.Visibility = Visibility.Visible;
                gScanInfo.Visibility = Visibility.Collapsed;
                return;
            }
            sDefaultScanInfo.Visibility = Visibility.Collapsed;
            gScanInfo.Visibility = Visibility.Visible;
            string slideScanInfo = mSlide.Source.sbcCurrentSlide.GetSlideScanInfo();
            List<Label> list = new List<Label>();
            foreach (Label child in gScanInfo.Children)
            {
                if (Grid.GetColumn(child) == 1)
                {
                    list.Add(child);
                }
            }
            string[] array = slideScanInfo.Split(new string[1]
            {
                "|"
            }, StringSplitOptions.None);
            for (int i = 0; i < array.Length; i++)
            {
                list[i].Content = array[i];
            }
            list.Clear();
            array = null;
        }
    }

}
