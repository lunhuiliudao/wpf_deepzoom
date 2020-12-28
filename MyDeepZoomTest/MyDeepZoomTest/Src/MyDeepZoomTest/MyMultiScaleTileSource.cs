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
            string imgDir = this.imgDir; 
            string imgPath = imgDir + $"{tileLevel}\\{tilePositionX}_{tilePositionY}.jpg";
            if (System.IO.File.Exists(imgPath))
            {
                Bitmap b = (Bitmap)Image.FromFile(imgPath);
                //b.Save("D:\\12.jpg");
                Bitmap bmp = new Bitmap(b.Width, b.Height);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(b, 0, 0, b.Width, b.Height);
                //g.DrawString($"{tileLevel}\\{tilePositionX}_{tilePositionY}.jpg", new Font("宋体", 15), Brushes.Red, 0, 30);
                //g.DrawRectangle(new Pen(Brushes.Red), 0, 0, b.Width, b.Height);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] bytes = ms.ToArray();

                //// 把 byte[] 写入文件
                //FileStream fs = new FileStream("D:\\2.jpg", FileMode.Create);
                //BinaryWriter bw = new BinaryWriter(fs);
                //bw.Write(bytes);
                //bw.Close();
                //fs.Close();

                return new MemoryStream(bytes);
            }
            else
            {
                return null;
            }
        }
    }
}
