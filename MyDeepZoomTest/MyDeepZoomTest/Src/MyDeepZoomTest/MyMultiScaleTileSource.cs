using KFBIO.DeepZoom;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDeepZoomTest
{
    public class MyMultiScaleTileSource : MultiScaleTileSource
    {
        public MyMultiScaleTileSource(long imageWidth, long imageHeight, int tileSize, int tileOverlap):
            base(imageWidth, imageHeight, tileSize, tileOverlap)
        {

        }

        public override void GetTileLayersAngle(ref double CenterX, ref double CenterY, ref double Angle, ref double OffsetX, ref double OffsetY)
        {
        }

        protected override object GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY)
        {
            string imgDir = @"D:\MyTestCode\wpf_deepzoom_demo\MyDeepZoomTest\MyDeepZoomTest\Doc\Sample\6_files\";
            if(tileLevel > 8)
            {
                // return null;
            }
            string imgPath = imgDir + $"{tileLevel}\\{tilePositionX}_{tilePositionY}.jpg";
            Bitmap b = (Bitmap)Image.FromFile(imgPath);
            MemoryStream ms = new MemoryStream();
            b.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bytes = ms.ToArray();
            return new MemoryStream(bytes);
        }
    }
}
