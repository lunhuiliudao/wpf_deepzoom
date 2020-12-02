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
    public class RulerTool
    {
        public static void UpdateRuler(Grid gSlideBox, ref float fWidthPerPixel)
        {
            MultiScaleImage multiScaleImage = gSlideBox.FindName("mMultiScaleImage") as MultiScaleImage;
            StackPanel obj = gSlideBox.FindName("sRulerToolBox") as StackPanel;
            Label label = obj.FindName("lRulerValue") as Label;
            StackPanel stackPanel = obj.FindName("sRulerItem") as StackPanel;
            obj.Visibility = Visibility.Visible;
            float fTargetScale = multiScaleImage.fTargetScale;
            float num = (float)(10.0 / multiScaleImage.Source.sbcCurrentSlide.dMaxActualZoom);
            fWidthPerPixel = num / fTargetScale;
            float num2 = fWidthPerPixel * 80f;
            num2 = ((num2 < 20f) ? 20f : ((num2 < 50f) ? 50f : ((num2 < 100f) ? 100f : ((num2 < 200f) ? 200f : ((num2 < 500f) ? 500f : ((num2 < 1000f) ? 1000f : ((num2 < 2000f) ? 2000f : ((!(num2 < 5000f)) ? 10000f : 5000f))))))));
            label.Content = num2 + "μm";
            foreach (Label child in stackPanel.Children)
            {
                child.Width = num2 / fWidthPerPixel / 5f;
            }
        }
    }

}
