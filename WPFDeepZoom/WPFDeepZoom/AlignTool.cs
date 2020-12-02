using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDeepZoom
{
    public class AlignTool
    {
        public static void RotateImage(Image imgOld, int iDefaultAngle)
        {
            RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;
            switch (iDefaultAngle / 90 % 4)
            {
                case 0:
                    rotateFlipType = RotateFlipType.RotateNoneFlipNone;
                    break;
                case 1:
                    rotateFlipType = RotateFlipType.Rotate90FlipNone;
                    break;
                case 2:
                    rotateFlipType = RotateFlipType.Rotate180FlipNone;
                    break;
                case 3:
                    rotateFlipType = RotateFlipType.Rotate270FlipNone;
                    break;
            }
            imgOld.RotateFlip(rotateFlipType);
        }

        public static void CompareImage()
        {
        }

        private static Image ResizeImage(Image imgToResize, OpenCvSharp.Size size)
        {
            int width = imgToResize.Width;
            int height = imgToResize.Height;
            float num = 0f;
            float num2 = 0f;
            float num3 = 0f;
            num2 = (float)size.Width / (float)width;
            num3 = (float)size.Height / (float)height;
            num = ((!(num3 < num2)) ? num2 : num3);
            int width2 = (int)((float)width * num);
            int height2 = (int)((float)height * num);
            Bitmap bitmap = new Bitmap(width2, height2);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(imgToResize, 0, 0, width2, height2);
            graphics.Dispose();
            return bitmap;
        }

        public static byte[] ImageToByte(Image _image)
        {
            MemoryStream memoryStream = new MemoryStream();
            _image.Save(memoryStream, ImageFormat.Png);
            return memoryStream.ToArray();
        }

        public static void CompareThumbnail(Image imgMain, Image imgSecondary, ref int iDeltaAngle, ref System.Windows.Point pDeltaFlip, ref System.Windows.Point pDeltaOffset, ref float fRatio)
        {
            byte[] imageBytes = ImageToByte(imgMain);
            byte[] imageBytes2 = ImageToByte(imgSecondary);
            Mat mat = Mat.FromImageData(imageBytes);
            Mat mat2 = Mat.FromImageData(imageBytes2);
            Mat homography = GetHomography(mat, mat2);
            iDeltaAngle = (int)MatrixToEulerAngles(homography).Z;
            if (iDeltaAngle < 0)
            {
                iDeltaAngle += 360;
            }
            GetFlipValue(mat, mat2, homography, ref iDeltaAngle, ref pDeltaFlip);
            _ = imgSecondary.Size;
            if (pDeltaFlip.Y == -1.0)
            {
                imgSecondary.RotateFlip(RotateFlipType.Rotate180FlipX);
            }
            if (iDeltaAngle != 0)
            {
                imgSecondary = KiRotate((Bitmap)imgSecondary, -iDeltaAngle, Color.White);
            }
            Mat mSecondary = Mat.FromImageData(ImageToByte(imgSecondary));
            Mat homography2 = GetHomography(mat, mSecondary);
            Point2f transformPoint = GetTransformPoint(new Point2f(0f, 0f), homography2);
            pDeltaOffset.X = transformPoint.X;
            pDeltaOffset.Y = transformPoint.Y;
        }

        public static Mat GetHomography(Mat mMain, Mat mSecondary)
        {
            KeyPoint[] keypoints = null;
            KeyPoint[] keypoints2 = null;
            using (SIFT sIFT = SIFT.Create(1000))
            {
                using (Mat mat = new Mat())
                {
                    using (Mat mat2 = new Mat())
                    {
                        sIFT.DetectAndCompute(mMain, new Mat(), out keypoints, mat);
                        sIFT.DetectAndCompute(mSecondary, new Mat(), out keypoints2, mat2);
                        FlannBasedMatcher flannBasedMatcher = new FlannBasedMatcher();
                        DMatch[] array = new DMatch[0];
                        array = flannBasedMatcher.Match(mat, mat2);
                        List<Point2f> list = new List<Point2f>();
                        List<Point2f> list2 = new List<Point2f>();
                        for (int i = 0; i < array.Length; i++)
                        {
                            list.Add(keypoints[array[i].QueryIdx].Pt);
                            list2.Add(keypoints2[array[i].TrainIdx].Pt);
                        }
                        return Cv2.FindHomography(InputArray.Create(list2), InputArray.Create(list), HomographyMethods.Ransac);
                    }
                }
            }
        }

        public static Bitmap KiRotate(Bitmap bmp, float angle, Color bkColor)
        {
            int num = bmp.Width + 2;
            int num2 = bmp.Height + 2;
            PixelFormat pixelFormat = bmp.PixelFormat;
            Bitmap bitmap = new Bitmap(num, num2, pixelFormat);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(bkColor);
            graphics.DrawImageUnscaled(bmp, 1, 1);
            graphics.Dispose();
            GraphicsPath graphicsPath = new GraphicsPath();
            graphicsPath.AddRectangle(new RectangleF(0f, 0f, num, num2));
            Matrix matrix = new Matrix();
            matrix.Rotate(angle);
            RectangleF bounds = graphicsPath.GetBounds(matrix);
            Bitmap bitmap2 = new Bitmap((int)bounds.Width, (int)bounds.Height, pixelFormat);
            Graphics graphics2 = Graphics.FromImage(bitmap2);
            graphics2.Clear(bkColor);
            graphics2.TranslateTransform(0f - bounds.X, 0f - bounds.Y);
            graphics2.RotateTransform(angle);
            graphics2.InterpolationMode = InterpolationMode.Low;
            graphics2.DrawImageUnscaled(bitmap, 0, 0);
            graphics2.Dispose();
            bitmap.Dispose();
            return bitmap2;
        }

        public static void GetFlipValue(Mat mCurrent, Mat mCycle, Mat mHomography, ref int iDeltaAngle, ref System.Windows.Point pDeltaFlip)
        {
            Point2f pOriginal = new Point2f(0f, 0f);
            Point2f pOriginal2 = new Point2f(mCycle.Width, 0f);
            Point2f pOriginal3 = new Point2f(0f, mCycle.Height);
            Point2f pOriginal4 = new Point2f(mCycle.Width, mCycle.Height);
            Point2f transformPoint = GetTransformPoint(pOriginal, mHomography);
            Point2f transformPoint2 = GetTransformPoint(pOriginal2, mHomography);
            Point2f transformPoint3 = GetTransformPoint(pOriginal3, mHomography);
            Point2f transformPoint4 = GetTransformPoint(pOriginal4, mHomography);
            if ((270 <= iDeltaAngle && iDeltaAngle <= 360) || (iDeltaAngle >= 0 && iDeltaAngle <= 90))
            {
                if (!(transformPoint.X <= transformPoint2.X) || !(transformPoint3.X <= transformPoint4.X) || !(transformPoint.Y <= transformPoint3.Y) || !(transformPoint2.Y <= transformPoint4.Y))
                {
                    pDeltaFlip.Y = -1.0;
                }
            }
            else if (!(transformPoint.X > transformPoint2.X) || !(transformPoint3.X > transformPoint4.X) || !(transformPoint.Y > transformPoint3.Y) || !(transformPoint2.Y > transformPoint4.Y))
            {
                pDeltaFlip.Y = -1.0;
            }
        }

        private static Point3f MatrixToEulerAngles(Mat mHomography)
        {
            float num = (float)Math.Sqrt(mHomography.At<double>(0, 0) * mHomography.At<double>(0, 0) + mHomography.At<double>(1, 0) * mHomography.At<double>(1, 0));
            float num2;
            float num3;
            float num4;
            if (!((double)num < 1E-06))
            {
                num2 = (float)Math.Atan2(mHomography.At<double>(2, 1), mHomography.At<double>(2, 2));
                num3 = (float)Math.Atan2(0.0 - mHomography.At<double>(2, 0), num);
                num4 = (float)Math.Atan2(mHomography.At<double>(1, 0), mHomography.At<double>(0, 0));
            }
            else
            {
                num2 = (float)Math.Atan2(0.0 - mHomography.At<double>(1, 2), mHomography.At<double>(1, 1));
                num3 = (float)Math.Atan2(0.0 - mHomography.At<double>(2, 0), num);
                num4 = 0f;
            }
            Point3f result = default(Point3f);
            result.X = (float)((double)num2 * (180.0 / Math.PI));
            result.Y = (float)((double)num3 * (180.0 / Math.PI));
            result.Z = (float)((double)num4 * (180.0 / Math.PI));
            return result;
        }

        public static Point2f GetTransformPoint(Point2f pOriginal, Mat mTransformMaxtri)
        {
            Mat b = new Mat(3, 1, 6, new double[3]
            {
                pOriginal.X,
                pOriginal.Y,
                1.0
            }, 0L);
            Mat mat = mTransformMaxtri * b;
            float x = (float)(mat.At<double>(0, 0) / mat.At<double>(2, 0));
            float y = (float)(mat.At<double>(1, 0) / mat.At<double>(2, 0));
            return new Point2f(x, y);
        }

        public static Mat StreamToMat(Stream streamSource)
        {
            return Mat.FromStream(streamSource, ImreadModes.Color);
        }
    }

}
