using KFBIO.DeepZoom;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransDat;
using TransDat.Model;

namespace MyDeepZoomTest
{
    public class DatMultiScaleTileSource : MultiScaleTileSource
    {
        private string datFullPath = string.Empty;

        public DatMultiScaleTileSource(long imageWidth, long imageHeight, int tileSize, int tileOverlap, string datFullPath) :
            base(imageWidth, imageHeight, tileSize, tileOverlap)
        {
            this.datFullPath = datFullPath;
        }

        public override void GetTileLayersAngle(ref double CenterX, ref double CenterY, ref double Angle, ref double OffsetX, ref double OffsetY)
        {
        }

        protected override object GetTileLayers(int tileLevel, int tilePositionX, int tilePositionY)
        {
            if (tileLevel > 8)
            {
                DateTime start = DateTime.Now;
                byte[] imgDat = TransDatManager.GetSingleTileImageFromDat(this.datFullPath, tileLevel - 8, tilePositionX, tilePositionY);
                if (imgDat != null)
                {
                    Bitmap b = TransHelper.SetByteToImage(imgDat);
                    Bitmap bmp = new Bitmap(b.Width, b.Height);
                    Graphics g = Graphics.FromImage(bmp);
                    g.DrawImage(b, 0, 0, b.Width, b.Height);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        DateTime end = DateTime.Now;
                        Console.WriteLine($"tileLevel={tileLevel}, tilePositionX={tilePositionX}, tilePositionY={tilePositionY}, time={(end - start).TotalMilliseconds}");
                        return new MemoryStream(ms.ToArray());
                    }
                }
            }

            return null;
        }
    }
}
