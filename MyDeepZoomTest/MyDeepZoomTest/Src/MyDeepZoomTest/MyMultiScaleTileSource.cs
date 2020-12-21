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
        private string imgDir = string.Empty;

        public MyMultiScaleTileSource(long imageWidth, long imageHeight, int tileSize, int tileOverlap):
            base(imageWidth, imageHeight, tileSize, tileOverlap)
        {

        }

        public MyMultiScaleTileSource(long imageWidth, long imageHeight, int tileSize, int tileOverlap, string imgDir) :
            base(imageWidth, imageHeight, tileSize, tileOverlap)
        {
            this.imgDir = imgDir;
        }

        public override void GetTileLayersAngle(ref double CenterX, ref double CenterY, ref double Angle, ref double OffsetX, ref double OffsetY)
        {
        }

        protected override object GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY)
        {
            string imgDir = this.imgDir; // @"D:\MyTestCode\wpf_deepzoom_demo\MyDeepZoomTest\MyDeepZoomTest\Doc\Sample\2_files\";
            if(tileLevel < 15)
            {
                //return null;
            }
            string imgPath = imgDir + $"{tileLevel}\\{tilePositionX}_{tilePositionY}.jpg";
            if (System.IO.File.Exists(imgPath))
            {
                Bitmap b = (Bitmap)Image.FromFile(imgPath);

                Bitmap bmp = new Bitmap(b.Width, b.Height);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(b, 0, 0);
                g.DrawString($"{tileLevel}\\{tilePositionX}_{tilePositionY}.jpg", new Font("宋体", 15), Brushes.Red, 0, 30);
                g.DrawRectangle(new Pen(Brushes.Red), 0, 0, b.Width, b.Height);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] bytes = ms.ToArray();
                return new MemoryStream(bytes);
            }
            else
            {
                return null;
            }
        }
    }
}
