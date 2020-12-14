using KFBIO.Common;
using KFBIO.DeepZoom;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KFBIO.DynamicGeometry
{
	public class myAnalysis : AnnotationBase
	{
		public Rectangle m_rectangle;

		private Thumb m_ThumbB;

		private Thumb m_ThumbL;

		private Thumb m_ThumbLB;

		private Thumb m_ThumbLT;

		private Thumb m_ThumbR;

		private Thumb m_ThumbRB;

		private Thumb m_ThumbRT;

		private Thumb m_ThumbT;

		private Thumb m_ThumbMove;

		private Point point;

		private Rectangle LeftRect;

		private Rectangle RightRect;

		private Rectangle BottomRect;

		private Rectangle TopRect;

		private Image newImage;

		private Image newImage2;

		public Thumb ThumbB
		{
			get
			{
				return m_ThumbB;
			}
			set
			{
				m_ThumbB = value;
			}
		}

		public Thumb ThumbL
		{
			get
			{
				return m_ThumbL;
			}
			set
			{
				m_ThumbL = value;
			}
		}

		public Thumb ThumbLB
		{
			get
			{
				return m_ThumbLB;
			}
			set
			{
				m_ThumbLB = value;
			}
		}

		public Thumb ThumbLT
		{
			get
			{
				return m_ThumbLT;
			}
			set
			{
				m_ThumbLT = value;
			}
		}

		public Thumb ThumbR
		{
			get
			{
				return m_ThumbR;
			}
			set
			{
				m_ThumbR = value;
			}
		}

		public Thumb ThumbRB
		{
			get
			{
				return m_ThumbRB;
			}
			set
			{
				m_ThumbRB = value;
			}
		}

		public Thumb ThumbRT
		{
			get
			{
				return m_ThumbRT;
			}
			set
			{
				m_ThumbRT = value;
			}
		}

		public Thumb ThumbT
		{
			get
			{
				return m_ThumbT;
			}
			set
			{
				m_ThumbT = value;
			}
		}

		public Thumb ThumbMove
		{
			get
			{
				return m_ThumbMove;
			}
			set
			{
				m_ThumbMove = value;
			}
		}

		public Rectangle MRectangle
		{
			get
			{
				return m_rectangle;
			}
			set
			{
				m_rectangle = value;
			}
		}

		public event MouseEventHandler CloseFinishEvent;

		public myAnalysis(AnnoListControl alc, Canvas canvasboard, MultiScaleImage msi, List<AnnotationBase> objectlist, int SlideZoom, double Calibration)
		{
			SetPara(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
			msi.MouseLeftButtonDown += MouseDown;
			base.AnnotationType = AnnotationType.Rectangle;
		}

		public void RectSetPara(Rectangle _LeftRect, Rectangle _RightRect, Rectangle _BottomRect, Rectangle _TopRect)
		{
			LeftRect = _LeftRect;
			RightRect = _RightRect;
			BottomRect = _BottomRect;
			TopRect = _TopRect;
		}

		public void unload()
		{
			base.msi.MouseLeftButtonDown -= MouseDown;
		}

		private void MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!base.isdraw)
			{
				m_rectangle = new Rectangle();
				Panel.SetZIndex(M_FiguresCanvas, 999);
				Point position = e.GetPosition(M_FiguresCanvas);
				m_rectangle.SetValue(Canvas.LeftProperty, position.X);
				m_rectangle.SetValue(Canvas.TopProperty, position.Y);
				m_rectangle.Width = 0.0;
				m_rectangle.Height = 0.0;
				m_rectangle.StrokeThickness = base.Size;
				m_rectangle.Stroke = base.BorderBrush;
				base.OriginStart = position;
				base.OriginEnd = position;
				point = position;
				base.CurrentStart = CanvasToMsi(position);
				base.CurrentEnd = CanvasToMsi(position);
				M_FiguresCanvas.Children.Add(m_rectangle);
				M_FiguresCanvas.MouseMove += MouseMove;
				Application.Current.MainWindow.MouseUp += MouseUp;
			}
		}

		private void MouseMove(object sender, MouseEventArgs e)
		{
			Point position = e.GetPosition(M_FiguresCanvas);
			Point originEnd = new Point(Math.Max(point.X, position.X), Math.Max(point.Y, position.Y));
			Point originStart = new Point(Math.Min(point.X, position.X), Math.Min(point.Y, position.Y));
			m_rectangle.SetValue(Canvas.LeftProperty, originStart.X);
			m_rectangle.SetValue(Canvas.TopProperty, originStart.Y);
			m_rectangle.Width = originEnd.X - originStart.X;
			m_rectangle.Height = originEnd.Y - originStart.Y;
			LeftRect.SetValue(Canvas.LeftProperty, 0.0);
			LeftRect.SetValue(Canvas.TopProperty, originStart.Y);
			LeftRect.Width = originStart.X;
			LeftRect.Height = m_rectangle.Height;
			RightRect.SetValue(Canvas.LeftProperty, originEnd.X);
			RightRect.SetValue(Canvas.TopProperty, originStart.Y);
			RightRect.Width = M_FiguresCanvas.ActualWidth - originEnd.X;
			RightRect.Height = m_rectangle.Height;
			TopRect.SetValue(Canvas.LeftProperty, 0.0);
			TopRect.SetValue(Canvas.TopProperty, 0.0);
			TopRect.Width = M_FiguresCanvas.ActualWidth;
			TopRect.Height = originStart.Y;
			BottomRect.SetValue(Canvas.LeftProperty, 0.0);
			BottomRect.SetValue(Canvas.TopProperty, originEnd.Y);
			BottomRect.Width = M_FiguresCanvas.ActualWidth;
			BottomRect.Height = M_FiguresCanvas.ActualHeight - originEnd.Y;
			base.OriginStart = originStart;
			base.OriginEnd = originEnd;
			base.CurrentStart = CanvasToMsi(base.OriginStart);
			base.CurrentEnd = CanvasToMsi(base.OriginEnd);
			base.TextBlock_info = CalcMeasureInfo();
			base.AnnoControl.Tbk.Text = null;
			base.AnnoControl.Tbk.Text = CalcMeasureInfo();
		}

		private void MouseUp(object sender, MouseEventArgs e)
		{
			base.isFinish = true;
			CreateMTextBlock();
			CreateThumb();
			base.msi.MouseLeftButtonDown -= MouseDown;
			M_FiguresCanvas.MouseMove -= MouseMove;
			M_FiguresCanvas.MouseUp -= MouseUp;
			M_FiguresCanvas.MouseUp -= MouseUp;
			Application.Current.MainWindow.MouseUp -= MouseUp;
			if (this.CloseFinishEvent != null)
			{
				this.CloseFinishEvent(this, e);
			}
		}

		public override void CreateMTextBlock()
		{
			newImage = new Image();
			newImage.Source = new BitmapImage(new Uri("../images/Ok.jpg", UriKind.Relative));
			newImage.Width = 24.0;
			newImage.Opacity = 1.0;
			newImage.Cursor = Cursors.Hand;
			newImage.SetValue(Canvas.LeftProperty, base.OriginEnd.X - 24.0);
			newImage.SetValue(Canvas.TopProperty, base.OriginEnd.Y);
			newImage.MouseLeftButtonDown += newImage_MouseLeftButtonDown;
			M_FiguresCanvas.Children.Add(newImage);
			newImage2 = new Image();
			newImage2.Source = new BitmapImage(new Uri("../images/Close.jpg", UriKind.Relative));
			newImage2.Width = 24.0;
			newImage2.Opacity = 1.0;
			newImage2.Cursor = Cursors.Hand;
			newImage2.SetValue(Canvas.LeftProperty, base.OriginEnd.X - 24.0 - 24.0 - 5.0);
			newImage2.SetValue(Canvas.TopProperty, base.OriginEnd.Y);
			newImage2.MouseLeftButtonDown += newImage2_MouseLeftButtonDown;
			M_FiguresCanvas.Children.Add(newImage2);
		}

		private void newImage2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			M_FiguresCanvas.Children.Remove(newImage);
			M_FiguresCanvas.Children.Remove(newImage2);
			M_FiguresCanvas.Children.Remove(m_rectangle);
			M_FiguresCanvas.Children.Remove(m_ThumbB);
			M_FiguresCanvas.Children.Remove(m_ThumbL);
			M_FiguresCanvas.Children.Remove(m_ThumbLB);
			M_FiguresCanvas.Children.Remove(m_ThumbLT);
			M_FiguresCanvas.Children.Remove(m_ThumbR);
			M_FiguresCanvas.Children.Remove(ThumbMove);
			M_FiguresCanvas.Children.Remove(m_ThumbRT);
			M_FiguresCanvas.Children.Remove(m_ThumbT);
			M_FiguresCanvas.Children.Remove(m_ThumbRB);
			Panel.SetZIndex(M_FiguresCanvas, -1);
		}

		private void newImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			M_FiguresCanvas.Children.Remove(newImage);
			M_FiguresCanvas.Children.Remove(newImage2);
			M_FiguresCanvas.Children.Remove(m_rectangle);
			M_FiguresCanvas.Children.Remove(m_ThumbB);
			M_FiguresCanvas.Children.Remove(m_ThumbL);
			M_FiguresCanvas.Children.Remove(m_ThumbLB);
			M_FiguresCanvas.Children.Remove(m_ThumbLT);
			M_FiguresCanvas.Children.Remove(m_ThumbR);
			M_FiguresCanvas.Children.Remove(ThumbMove);
			M_FiguresCanvas.Children.Remove(m_ThumbRT);
			M_FiguresCanvas.Children.Remove(m_ThumbT);
			M_FiguresCanvas.Children.Remove(m_ThumbRB);
			Panel.SetZIndex(M_FiguresCanvas, -1);
			base.FinishFunc(this, e);
		}

		private void m_ThumbMove_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.Center, e.HorizontalChange, e.VerticalChange);
		}

		private void m_ThumbB_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.Bottom, e.HorizontalChange, e.VerticalChange);
			base.UpadteTextBlock();
		}

		private void m_ThumbL_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.Left, e.HorizontalChange, e.VerticalChange);
			base.UpadteTextBlock();
		}

		private void m_ThumbLB_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.LeftBottom, e.HorizontalChange, e.VerticalChange);
			base.UpadteTextBlock();
		}

		private void m_ThumbLT_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.LeftTop, e.HorizontalChange, e.VerticalChange);
			base.UpadteTextBlock();
		}

		private void m_ThumbR_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.Right, e.HorizontalChange, e.VerticalChange);
			base.UpadteTextBlock();
		}

		private void m_ThumbRB_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.RightBottom, e.HorizontalChange, e.VerticalChange);
			base.UpadteTextBlock();
		}

		private void m_ThumbRT_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.RightTop, e.HorizontalChange, e.VerticalChange);
			base.UpadteTextBlock();
		}

		private void m_ThumbT_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.Top, e.HorizontalChange, e.VerticalChange);
			base.UpadteTextBlock();
		}

		private void DragCompleted(object sender, DragCompletedEventArgs e)
		{
			double x = Math.Min(base.CurrentStart.X, base.CurrentEnd.X);
			double x2 = Math.Max(base.CurrentStart.X, base.CurrentEnd.X);
			double y = Math.Min(base.CurrentStart.Y, base.CurrentEnd.Y);
			double y2 = Math.Max(base.CurrentStart.Y, base.CurrentEnd.Y);
			base.CurrentStart = new Point(x, y);
			base.CurrentEnd = new Point(x2, y2);
			base.OriginStart = MsiToCanvas(base.CurrentStart);
			base.OriginEnd = MsiToCanvas(base.CurrentEnd);
		}

		public override void DeleteItem()
		{
			M_FiguresCanvas.Children.Remove(m_rectangle);
			M_FiguresCanvas.Children.Remove(base.MTextBlock);
			M_FiguresCanvas.Children.Remove(m_ThumbB);
			M_FiguresCanvas.Children.Remove(m_ThumbL);
			M_FiguresCanvas.Children.Remove(m_ThumbLB);
			M_FiguresCanvas.Children.Remove(m_ThumbLT);
			M_FiguresCanvas.Children.Remove(m_ThumbR);
			M_FiguresCanvas.Children.Remove(ThumbMove);
			M_FiguresCanvas.Children.Remove(m_ThumbRT);
			M_FiguresCanvas.Children.Remove(m_ThumbT);
			M_FiguresCanvas.Children.Remove(m_ThumbRB);
			base.objectlist.Remove(this);
		}

		public override void UpdateVisual()
		{
			if (ThumbMove != null)
			{
				double x = MsiToCanvas(base.CurrentStart).X;
				double y = MsiToCanvas(base.CurrentStart).Y;
				double x2 = MsiToCanvas(base.CurrentEnd).X;
				double y2 = MsiToCanvas(base.CurrentEnd).Y;
				double num = Math.Abs(x - x2);
				double num2 = Math.Abs(y - y2);
				x = Math.Min(x, x2);
				y = Math.Min(y, y2);
				ThumbB.SetValue(Canvas.LeftProperty, x + num / 2.0 - Setting.Thumb_w / 2.0);
				ThumbB.SetValue(Canvas.TopProperty, y + num2 - Setting.Thumb_w / 2.0);
				ThumbL.SetValue(Canvas.LeftProperty, x - Setting.Thumb_w / 2.0);
				ThumbL.SetValue(Canvas.TopProperty, y + num2 / 2.0 - Setting.Thumb_w / 2.0);
				ThumbLB.SetValue(Canvas.LeftProperty, x - Setting.Thumb_w / 2.0);
				ThumbLB.SetValue(Canvas.TopProperty, y + num2 - Setting.Thumb_w / 2.0);
				ThumbLT.SetValue(Canvas.LeftProperty, x - Setting.Thumb_w / 2.0);
				ThumbLT.SetValue(Canvas.TopProperty, y - Setting.Thumb_w / 2.0);
				ThumbR.SetValue(Canvas.LeftProperty, x + num - Setting.Thumb_w / 2.0);
				ThumbR.SetValue(Canvas.TopProperty, y + num2 / 2.0 - Setting.Thumb_w / 2.0);
				ThumbRB.SetValue(Canvas.LeftProperty, x + num - Setting.Thumb_w / 2.0);
				ThumbRB.SetValue(Canvas.TopProperty, y + num2 - Setting.Thumb_w / 2.0);
				ThumbRT.SetValue(Canvas.LeftProperty, x + num - Setting.Thumb_w / 2.0);
				ThumbRT.SetValue(Canvas.TopProperty, y - Setting.Thumb_w / 2.0);
				ThumbT.SetValue(Canvas.LeftProperty, x + num / 2.0 - Setting.Thumb_w / 2.0);
				ThumbT.SetValue(Canvas.TopProperty, y - Setting.Thumb_w / 2.0);
				ThumbMove.SetValue(Canvas.LeftProperty, x + num / 2.0 - Setting.Thumb_c / 2.0);
				ThumbMove.SetValue(Canvas.TopProperty, y + num2 / 2.0 - Setting.Thumb_c / 2.0);
				m_rectangle.StrokeThickness = 2.0;
				m_rectangle.Stroke = base.BorderBrush;
				m_rectangle.Width = num;
				m_rectangle.Height = num2;
				Canvas.SetLeft(m_rectangle, x);
				Canvas.SetTop(m_rectangle, y);
				Canvas.SetLeft(newImage, x + num - 24.0);
				Canvas.SetTop(newImage, y + num2);
				Canvas.SetLeft(newImage2, x + num - 24.0 - 24.0 - 5.0);
				Canvas.SetTop(newImage2, y + num2);
				LeftRect.SetValue(Canvas.LeftProperty, 0.0);
				LeftRect.SetValue(Canvas.TopProperty, y);
				LeftRect.Width = x;
				LeftRect.Height = num2;
				RightRect.SetValue(Canvas.LeftProperty, x + m_rectangle.Width);
				RightRect.SetValue(Canvas.TopProperty, y);
				RightRect.Width = M_FiguresCanvas.ActualWidth - (x + num);
				RightRect.Height = num2;
				TopRect.SetValue(Canvas.LeftProperty, 0.0);
				TopRect.SetValue(Canvas.TopProperty, 0.0);
				TopRect.Width = M_FiguresCanvas.ActualWidth;
				TopRect.Height = y;
				BottomRect.SetValue(Canvas.LeftProperty, 0.0);
				BottomRect.SetValue(Canvas.TopProperty, y + num2);
				BottomRect.Width = M_FiguresCanvas.ActualWidth;
				BottomRect.Height = M_FiguresCanvas.ActualHeight - (y + num2);
			}
		}

		public override void CreateThumb()
		{
			if (m_ThumbB == null)
			{
				m_ThumbB = new Thumb();
				m_ThumbL = new Thumb();
				m_ThumbLB = new Thumb();
				m_ThumbLT = new Thumb();
				m_ThumbR = new Thumb();
				m_ThumbRB = new Thumb();
				m_ThumbRT = new Thumb();
				m_ThumbT = new Thumb();
				ThumbMove = new Thumb();
				m_ThumbB.Height = Setting.Thumb_w;
				m_ThumbB.Width = Setting.Thumb_w;
				m_ThumbL.Height = Setting.Thumb_w;
				m_ThumbL.Width = Setting.Thumb_w;
				m_ThumbLB.Height = Setting.Thumb_w;
				m_ThumbLB.Width = Setting.Thumb_w;
				m_ThumbLT.Height = Setting.Thumb_w;
				m_ThumbLT.Width = Setting.Thumb_w;
				m_ThumbR.Height = Setting.Thumb_w;
				m_ThumbR.Width = Setting.Thumb_w;
				m_ThumbRB.Height = Setting.Thumb_w;
				m_ThumbRB.Width = Setting.Thumb_w;
				m_ThumbRT.Height = Setting.Thumb_w;
				m_ThumbRT.Width = Setting.Thumb_w;
				m_ThumbT.Height = Setting.Thumb_w;
				m_ThumbT.Width = Setting.Thumb_w;
				ThumbMove.Height = Setting.Thumb_c;
				ThumbMove.Width = Setting.Thumb_c;
				double num = (double)m_rectangle.GetValue(Canvas.LeftProperty);
				double num2 = (double)m_rectangle.GetValue(Canvas.TopProperty);
				m_ThumbB.SetValue(Canvas.LeftProperty, num + m_rectangle.Width / 2.0 - Setting.Thumb_w / 2.0);
				m_ThumbB.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height - Setting.Thumb_w / 2.0);
				m_ThumbL.SetValue(Canvas.LeftProperty, num - Setting.Thumb_w / 2.0);
				m_ThumbL.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height / 2.0 - Setting.Thumb_w / 2.0);
				m_ThumbLB.SetValue(Canvas.LeftProperty, num - Setting.Thumb_w / 2.0);
				m_ThumbLB.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height - Setting.Thumb_w / 2.0);
				m_ThumbLT.SetValue(Canvas.LeftProperty, num - Setting.Thumb_w / 2.0);
				m_ThumbLT.SetValue(Canvas.TopProperty, num2 - Setting.Thumb_w / 2.0);
				m_ThumbR.SetValue(Canvas.LeftProperty, num + m_rectangle.Width - Setting.Thumb_w / 2.0);
				m_ThumbR.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height / 2.0 - Setting.Thumb_w / 2.0);
				m_ThumbRB.SetValue(Canvas.LeftProperty, num + m_rectangle.Width - Setting.Thumb_w / 2.0);
				m_ThumbRB.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height - Setting.Thumb_w / 2.0);
				m_ThumbRT.SetValue(Canvas.LeftProperty, num + m_rectangle.Width - Setting.Thumb_w / 2.0);
				m_ThumbRT.SetValue(Canvas.TopProperty, num2 - Setting.Thumb_w / 2.0);
				m_ThumbT.SetValue(Canvas.LeftProperty, num + m_rectangle.Width / 2.0 - Setting.Thumb_w / 2.0);
				m_ThumbT.SetValue(Canvas.TopProperty, num2 - Setting.Thumb_w / 2.0);
				ThumbMove.SetValue(Canvas.LeftProperty, num + m_rectangle.Width / 2.0 - Setting.Thumb_c / 2.0);
				ThumbMove.SetValue(Canvas.TopProperty, num2 + m_rectangle.Height / 2.0 - Setting.Thumb_c / 2.0);
				M_FiguresCanvas.Children.Add(m_ThumbL);
				M_FiguresCanvas.Children.Add(m_ThumbLB);
				M_FiguresCanvas.Children.Add(m_ThumbLT);
				M_FiguresCanvas.Children.Add(m_ThumbR);
				M_FiguresCanvas.Children.Add(m_ThumbRT);
				M_FiguresCanvas.Children.Add(m_ThumbT);
				M_FiguresCanvas.Children.Add(ThumbMove);
				M_FiguresCanvas.Children.Add(m_ThumbB);
				M_FiguresCanvas.Children.Add(m_ThumbRB);
				m_ThumbB.DragDelta += m_ThumbB_DragDelta;
				m_ThumbL.DragDelta += m_ThumbL_DragDelta;
				m_ThumbLB.DragDelta += m_ThumbLB_DragDelta;
				m_ThumbLT.DragDelta += m_ThumbLT_DragDelta;
				m_ThumbR.DragDelta += m_ThumbR_DragDelta;
				m_ThumbRB.DragDelta += m_ThumbRB_DragDelta;
				m_ThumbRT.DragDelta += m_ThumbRT_DragDelta;
				m_ThumbT.DragDelta += m_ThumbT_DragDelta;
				m_ThumbMove.DragDelta += m_ThumbMove_DragDelta;
				m_ThumbB.DragCompleted += DragCompleted;
				m_ThumbL.DragCompleted += DragCompleted;
				m_ThumbLB.DragCompleted += DragCompleted;
				m_ThumbLT.DragCompleted += DragCompleted;
				m_ThumbR.DragCompleted += DragCompleted;
				m_ThumbRB.DragCompleted += DragCompleted;
				m_ThumbRT.DragCompleted += DragCompleted;
				m_ThumbT.DragCompleted += DragCompleted;
				m_ThumbMove.DragCompleted += DragCompleted;
			}
		}
	}
}
