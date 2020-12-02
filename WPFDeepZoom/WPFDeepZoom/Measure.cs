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
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPFDeepZoom
{
    internal class Measure
    {
        public ResourceDictionary rdDirectory = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Themes/Style.xaml")
        };

        public Line lMeasureLine;

        public int iMeasurePointCount;

        public Point pStartRatio;

        public Point pOverRatio;

        public Label lMeasureValue;

        public Measure(Grid gBox)
        {
            lMeasureLine = (gBox.FindName("lMeasureLine") as Line);
            lMeasureValue = (gBox.FindName("lMeasureValue") as Label);
            if (lMeasureLine == null || lMeasureValue == null)
            {
                CreateMeasureToolUI(gBox);
            }
            if (lMeasureLine.X1 != 0.0 || lMeasureLine.Y1 != 0.0)
            {
                iMeasurePointCount++;
            }
            if (lMeasureLine.X2 != 0.0 || lMeasureLine.Y2 != 0.0)
            {
                iMeasurePointCount++;
            }
            SetCursor(gBox, Cursors.Cross);
            if (iMeasurePointCount == 2)
            {
                GetData();
            }
        }

        private void SetCursor(Grid gBox, Cursor cTarget)
        {
            foreach (FrameworkElement child in ((gBox.Parent as Border).Parent as Grid).Children)
            {
                if (child is Border)
                {
                    ((child as Border).Child as Grid).Cursor = cTarget;
                }
            }
        }

        public void Close(Grid gAllSlideBox)
        {
            foreach (FrameworkElement child in gAllSlideBox.Children)
            {
                if (child is Border)
                {
                    Grid grid = (child as Border).Child as Grid;
                    lMeasureLine = (grid.FindName("lMeasureLine") as Line);
                    lMeasureValue = (grid.FindName("lMeasureValue") as Label);
                    if (lMeasureLine != null && lMeasureValue != null)
                    {
                        lMeasureLine.UnregisterName("lMeasureLine");
                        lMeasureValue.UnregisterName("lMeasureValue");
                        grid.Children.Remove(lMeasureLine);
                        grid.Children.Remove(lMeasureValue);
                    }
                    grid.Cursor = Cursors.Arrow;
                }
            }
        }

        public void CreateMeasureToolUI(Grid gSlideBox)
        {
            lMeasureLine = new Line();
            lMeasureValue = new Label();
            lMeasureLine.SetValue(FrameworkElement.StyleProperty, rdDirectory["lMeasureLine"]);
            lMeasureValue.SetValue(FrameworkElement.StyleProperty, rdDirectory["lMeasureValue"]);
            gSlideBox.Children.Add(lMeasureLine);
            gSlideBox.Children.Add(lMeasureValue);
            lMeasureLine.RegisterName("lMeasureLine", lMeasureLine);
            lMeasureValue.RegisterName("lMeasureValue", lMeasureValue);
        }

        public void SetPointValue(Point pPoint)
        {
            if (iMeasurePointCount == 0 || iMeasurePointCount == 2)
            {
                lMeasureValue.Visibility = Visibility.Collapsed;
                lMeasureLine.X1 = pPoint.X;
                lMeasureLine.Y1 = pPoint.Y;
                lMeasureLine.X2 = pPoint.X;
                lMeasureLine.Y2 = pPoint.Y;
                CloseMeasureToolAnimation();
                iMeasurePointCount = 1;
            }
            else
            {
                lMeasureLine.X2 = pPoint.X;
                lMeasureLine.Y2 = pPoint.Y;
                CloseMeasureToolAnimation(2);
                iMeasurePointCount = 2;
            }
        }

        public void SetRatioPointValue(Point pRatio)
        {
            if (iMeasurePointCount == 0 || iMeasurePointCount == 2)
            {
                pStartRatio = pRatio;
                pOverRatio = pRatio;
            }
            else
            {
                pOverRatio = pRatio;
            }
            SaveData();
        }

        private void SaveData()
        {
            MeasureToolData measureToolData = new MeasureToolData();
            measureToolData.pStartRatio = pStartRatio;
            measureToolData.pOverRatio = pOverRatio;
            lMeasureLine.DataContext = measureToolData;
        }

        private void GetData()
        {
            MeasureToolData measureToolData = lMeasureLine.DataContext as MeasureToolData;
            if (measureToolData != null)
            {
                pStartRatio = measureToolData.pStartRatio;
                pOverRatio = measureToolData.pOverRatio;
            }
        }

        public void Measuring(Point pMeasure, float fWidthPerPixel)
        {
            lMeasureValue.Visibility = Visibility.Visible;
            CloseMeasureToolAnimation(3);
            lMeasureLine.X2 = pMeasure.X;
            lMeasureLine.Y2 = pMeasure.Y;
            float num = (float)Math.Sqrt(Math.Pow(lMeasureLine.X2 - lMeasureLine.X1, 2.0) + Math.Pow(lMeasureLine.Y2 - lMeasureLine.Y1, 2.0));
            lMeasureValue.Margin = new Thickness((lMeasureLine.X2 - lMeasureLine.X1) / 2.0 + lMeasureLine.X1 - 30.0, (lMeasureLine.Y2 - lMeasureLine.Y1) / 2.0 + lMeasureLine.Y1 - 20.0, 0.0, 0.0);
            lMeasureValue.Content = (fWidthPerPixel * num).ToString("0.00") + "μm";
        }

        public void UpdateMeasureTool(MultiScaleImage mSlide, bool animate = false, float fDurationTime = 0f, EasingFunctionBase easing = null)
        {
            Grid gSlideBox = PublicMethods.FindSlideBox(mSlide);
            Point pNormalPoint = mSlide.SlideRatioToBoxPixel(pStartRatio);
            Point pNormalPoint2 = mSlide.SlideRatioToBoxPixel(pOverRatio);
            pNormalPoint = PublicMethods.FlipPoint(gSlideBox, pNormalPoint);
            pNormalPoint2 = PublicMethods.FlipPoint(gSlideBox, pNormalPoint2);
            if (!animate)
            {
                CloseMeasureToolAnimation();
                lMeasureLine.X1 = pNormalPoint.X;
                lMeasureLine.Y1 = pNormalPoint.Y;
                lMeasureLine.X2 = pNormalPoint2.X;
                lMeasureLine.Y2 = pNormalPoint2.Y;
                lMeasureValue.Margin = new Thickness((lMeasureLine.X2 - lMeasureLine.X1) / 2.0 + lMeasureLine.X1 - 30.0, (lMeasureLine.Y2 - lMeasureLine.Y1) / 2.0 + lMeasureLine.Y1 - 20.0, 0.0, 0.0);
            }
            else
            {
                lMeasureLine.BeginAnimation(Line.Y1Property, new DoubleAnimation(pNormalPoint.Y, TimeSpan.FromMilliseconds(fDurationTime))
                {
                    EasingFunction = easing
                }, HandoffBehavior.Compose);
                lMeasureLine.BeginAnimation(Line.X2Property, new DoubleAnimation(pNormalPoint2.X, TimeSpan.FromMilliseconds(fDurationTime))
                {
                    EasingFunction = easing
                }, HandoffBehavior.Compose);
                lMeasureLine.BeginAnimation(Line.Y2Property, new DoubleAnimation(pNormalPoint2.Y, TimeSpan.FromMilliseconds(fDurationTime))
                {
                    EasingFunction = easing
                }, HandoffBehavior.Compose);
                DoubleAnimation doubleAnimation = new DoubleAnimation(pNormalPoint.X, TimeSpan.FromMilliseconds(fDurationTime))
                {
                    EasingFunction = easing
                };
                doubleAnimation.CurrentTimeInvalidated += delegate
                {
                    CloseMeasureToolAnimation(4);
                    lMeasureValue.Margin = new Thickness((lMeasureLine.X2 - lMeasureLine.X1) / 2.0 + lMeasureLine.X1 - 30.0, (lMeasureLine.Y2 - lMeasureLine.Y1) / 2.0 + lMeasureLine.Y1 - 20.0, 0.0, 0.0);
                };
                lMeasureLine.BeginAnimation(Line.X1Property, doubleAnimation, HandoffBehavior.Compose);
            }
        }

        private void CloseMeasureToolAnimation(int iFlag = 1)
        {
            try
            {
                switch (iFlag)
                {
                    case 2:
                        lMeasureLine.ApplyAnimationClock(Line.X2Property, null);
                        lMeasureLine.ApplyAnimationClock(Line.Y2Property, null);
                        break;
                    case 3:
                        lMeasureValue.ApplyAnimationClock(FrameworkElement.MarginProperty, null);
                        lMeasureLine.ApplyAnimationClock(Line.X2Property, null);
                        lMeasureLine.ApplyAnimationClock(Line.Y2Property, null);
                        break;
                    case 4:
                        lMeasureValue.ApplyAnimationClock(FrameworkElement.MarginProperty, null);
                        break;
                    default:
                        lMeasureLine.ApplyAnimationClock(Line.X1Property, null);
                        lMeasureLine.ApplyAnimationClock(Line.Y1Property, null);
                        lMeasureLine.ApplyAnimationClock(Line.X2Property, null);
                        lMeasureLine.ApplyAnimationClock(Line.Y2Property, null);
                        lMeasureValue.ApplyAnimationClock(FrameworkElement.MarginProperty, null);
                        break;
                }
            }
            catch
            {
            }
        }
    }
}
