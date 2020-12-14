using KFBIO.Common;
using KFBIO.DeepZoom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace KFBIO.DynamicGeometry
{
	public class myArrowLine : AnnotationBase
	{
		private Line cap;

		private Line connect;

		private Thumb m_ThumbEnd;

		private Thumb m_ThumbStart;

		private Thumb m_ThumbMove;

		private RotateTransform rtf;

		private Line Cap
		{
			get
			{
				return cap;
			}
			set
			{
				cap = value;
			}
		}

		private RotateTransform RTF
		{
			get
			{
				return rtf;
			}
			set
			{
				rtf = value;
			}
		}

		private Line Connector
		{
			get
			{
				return connect;
			}
			set
			{
				connect = value;
			}
		}

		private Thumb ThumbEnd
		{
			get
			{
				return m_ThumbEnd;
			}
			set
			{
				m_ThumbEnd = value;
			}
		}

		private Thumb ThumbStart
		{
			get
			{
				return m_ThumbStart;
			}
			set
			{
				m_ThumbStart = value;
			}
		}

		private Thumb ThumbMove
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

		public myArrowLine(AnnoListControl alc, Canvas canvasboard, MultiScaleImage msi, List<AnnotationBase> objectlist, int SlideZoom, double Calibration)
		{
			SetPara(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
			base.AnnotationType = AnnotationType.Arrow;
			msi.MouseLeftButtonDown += MouseDown;
			base.ControlName = base.AnnotationType + DateTime.Now.ToString("yyyyMMddHHmmss");
		}

		public void unload()
		{
			base.msi.MouseLeftButtonDown -= MouseDown;
		}

		public myArrowLine(AnnotationBase ab)
		{
			base.Calibration = ab.Calibration;
			base.SlideZoom = ab.SlideZoom;
			base.AnnotationType = AnnotationType.Arrow;
			base.ControlName = ab.ControlName;
			base.Zoom = ab.Zoom;
			base.AnnotationDescription = ab.AnnotationDescription;
			base.CurrentStart = ab.CurrentStart;
			base.CurrentEnd = ab.CurrentEnd;
			base.Size = ab.Size;
			base.BorderBrush = ab.BorderBrush;
			base.isVisble = ab.isVisble;
			base.isHidden = ab.isHidden;
			base.isMsVisble = ab.isMsVisble;
			base.AnnotationName = ab.AnnotationName;
			base.isFinish = true;
			base.msi = ab.msi;
			base.FiguresCanvas = ab.FiguresCanvas;
			base.objectlist = ab.objectlist;
			base.AnnoControl = ab.AnnoControl;
			Cap = new Line();
			Cap.Name = base.ControlName + "Cap";
			Connector = new Line();
			rtf = new RotateTransform();
			Connector.Name = base.ControlName;
			Point point = MsiToCanvas(base.CurrentStart);
			Point point2 = MsiToCanvas(base.CurrentEnd);
			double angle = Math.Atan2(point.Y - point2.Y, point.X - point2.X) * 180.0 / Math.PI;
			Connector.X1 = point.X;
			Connector.Y1 = point.Y;
			Connector.X2 = point2.X;
			Connector.Y2 = point2.Y;
			Connector.StrokeThickness = base.Size;
			Connector.Stroke = base.BorderBrush;
			M_FiguresCanvas.Children.Add(Connector);
			Cap.X1 = point2.X;
			Cap.Y1 = point2.Y;
			Cap.X2 = point2.X;
			Cap.Y2 = point2.Y;
			Cap.StrokeStartLineCap = PenLineCap.Triangle;
			Cap.StrokeThickness = base.Size + 20.0;
			Cap.Stroke = base.BorderBrush;
			rtf.Angle = angle;
			rtf.CenterX = point2.X;
			rtf.CenterY = point2.Y;
			Cap.RenderTransform = rtf;
			M_FiguresCanvas.Children.Add(Cap);
			base.objectlist.Insert(0, this);
			CreateMTextBlock();
			CreateThumb();
			UpdateCB();
			Connector.MouseLeftButtonDown += Select_MouseDown;
			Cap.MouseLeftButtonDown += Select_MouseDown;
			Connector.MouseEnter += GotFocus;
			Cap.MouseEnter += GotFocus;
			base.UpadteTextBlock();
			IsActive(Visibility.Collapsed);
			base.AnnoControl.CB.SelectedIndex = -1;
		}

		public void GotFocus(object sender, EventArgs e)
		{
			if (Setting.isAnnoChange)
			{
				base.AnnoControl.CB.SelectedIndex = base.objectlist.IndexOf(this);
			}
		}

		private void MouseDown(object sender, MouseEventArgs e)
		{
			if (!base.isdraw)
			{
				Cap = new Line();
				Cap.Name = base.ControlName + "Cap";
				Connector = new Line();
				rtf = new RotateTransform();
				Connector.Name = base.ControlName;
				Point position = e.GetPosition(M_FiguresCanvas);
				double angle = Math.Atan2(position.Y - position.Y, position.X - position.X) * 180.0 / Math.PI;
				Connector.X1 = position.X;
				Connector.Y1 = position.Y;
				Connector.X2 = position.X;
				Connector.Y2 = position.Y;
				Connector.StrokeThickness = base.Size;
				Connector.Stroke = base.BorderBrush;
				M_FiguresCanvas.Children.Add(Connector);
				Cap.X1 = position.X;
				Cap.Y1 = position.Y;
				Cap.X2 = position.X;
				Cap.Y2 = position.Y;
				Cap.StrokeStartLineCap = PenLineCap.Triangle;
				Cap.StrokeThickness = base.Size * 10.0;
				Cap.Stroke = base.BorderBrush;
				rtf.Angle = angle;
				rtf.CenterX = position.X;
				rtf.CenterY = position.Y;
				Cap.RenderTransform = rtf;
				base.msi.MouseMove += MouseMove;
				M_FiguresCanvas.Children.Add(Cap);
				Application.Current.MainWindow.MouseUp += MouseUp;
				Application.Current.MainWindow.MouseLeave += MouseUp;
				base.OriginStart = position;
				base.OriginEnd = position;
				base.CurrentStart = CanvasToMsi(base.OriginStart);
				base.CurrentEnd = base.CurrentStart;
				base.AnnotationName = Setting.Arrow + (base.objectlist.Count + 1);
				base.AnnotationDescription = "";
				base.objectlist.Insert(0, this);
				UpdateCB();
				Cap.MouseLeftButtonDown += Select_MouseDown;
				base.isdraw = true;
			}
		}

		private void MouseMove(object sender, MouseEventArgs e)
		{
			Point position = e.GetPosition(M_FiguresCanvas);
			double angle = Math.Atan2(base.OriginStart.Y - position.Y, base.OriginStart.X - position.X) * 180.0 / Math.PI;
			Cap.X1 = position.X;
			Cap.Y1 = position.Y;
			Cap.X2 = position.X;
			Cap.Y2 = position.Y;
			rtf.Angle = angle;
			rtf.CenterX = position.X;
			rtf.CenterY = position.Y;
			Cap.RenderTransform = rtf;
			Connector.X1 = base.OriginStart.X;
			Connector.Y1 = base.OriginStart.Y;
			Connector.X2 = position.X;
			Connector.Y2 = position.Y;
			base.OriginEnd = position;
			base.CurrentEnd = CanvasToMsi(position);
			base.TextBlock_info = CalcMeasureInfo();
			base.AnnoControl.Tbk.Text = null;
			base.AnnoControl.Tbk.Text = CalcMeasureInfo();
		}

		private void MouseUp(object sender, MouseEventArgs e)
		{
			base.isFinish = true;
			CreateMTextBlock();
			CreateThumb();
			Connector.MouseLeftButtonDown += Select_MouseDown;
			Connector.MouseEnter += GotFocus;
			Cap.MouseEnter += GotFocus;
			base.msi.MouseLeftButtonDown -= MouseDown;
			base.msi.MouseMove -= MouseMove;
			Application.Current.MainWindow.MouseUp -= MouseUp;
			Application.Current.MainWindow.MouseLeave -= MouseUp;
			base.FinishFunc(this, e);
		}

		private void Select_MouseDown(object sender, MouseButtonEventArgs e)
		{
			FrameworkElement frameworkElement = sender as FrameworkElement;
			if (frameworkElement != null)
			{
				Line line = (Line)frameworkElement;
				foreach (AnnotationBase item in base.objectlist)
				{
					item.IsActive(Visibility.Collapsed);
					if (item.ControlName == line.Name || item.ControlName == line.Name.Substring(0, line.Name.Length - 3))
					{
						item.IsActive(Visibility.Visible);
						base.AnnoControl.CB.SelectedIndex = base.objectlist.IndexOf(this);
					}
				}
			}
		}

		private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
		{
		}

		private void ThumbMove_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.Center, e.HorizontalChange, e.VerticalChange);
		}

		private void ThumbEnd_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.RightBottom, e.HorizontalChange, e.VerticalChange);
			base.UpadteTextBlock();
		}

		private void ThumbStart_DragDelta(object sender, DragDeltaEventArgs e)
		{
			ResetLocation(Direction.LeftTop, e.HorizontalChange, e.VerticalChange);
			base.UpadteTextBlock();
		}

		public override void DeleteItem()
		{
			M_FiguresCanvas.Children.Remove(Connector);
			M_FiguresCanvas.Children.Remove(Cap);
			M_FiguresCanvas.Children.Remove(base.MTextBlock);
			M_FiguresCanvas.Children.Remove(m_ThumbStart);
			M_FiguresCanvas.Children.Remove(ThumbMove);
			M_FiguresCanvas.Children.Remove(m_ThumbEnd);
			base.objectlist.Remove(this);
		}

		public override void CreateThumb()
		{
			if (m_ThumbEnd == null)
			{
				m_ThumbEnd = new Thumb();
				m_ThumbStart = new Thumb();
				ThumbMove = new Thumb();
				m_ThumbEnd.Height = Setting.Thumb_w;
				m_ThumbEnd.Width = Setting.Thumb_w;
				m_ThumbStart.Height = Setting.Thumb_w;
				m_ThumbStart.Width = Setting.Thumb_w;
				ThumbMove.Height = Setting.Thumb_c;
				ThumbMove.Width = Setting.Thumb_c;
				m_ThumbStart.SetValue(Canvas.LeftProperty, Connector.X1 - Setting.Thumb_w / 2.0);
				m_ThumbStart.SetValue(Canvas.TopProperty, Connector.Y1 - Setting.Thumb_w / 2.0);
				m_ThumbEnd.SetValue(Canvas.LeftProperty, Connector.X2 - Setting.Thumb_w / 2.0);
				m_ThumbEnd.SetValue(Canvas.TopProperty, Connector.Y2 - Setting.Thumb_w / 2.0);
				ThumbMove.SetValue(Canvas.LeftProperty, (Connector.X1 + Connector.X2) / 2.0 - Setting.Thumb_c / 2.0);
				ThumbMove.SetValue(Canvas.TopProperty, (Connector.Y1 + Connector.Y2) / 2.0 - Setting.Thumb_c / 2.0);
				M_FiguresCanvas.Children.Add(m_ThumbStart);
				M_FiguresCanvas.Children.Add(ThumbMove);
				M_FiguresCanvas.Children.Add(m_ThumbEnd);
				m_ThumbEnd.DragDelta += ThumbEnd_DragDelta;
				m_ThumbStart.DragDelta += ThumbStart_DragDelta;
				ThumbMove.DragDelta += ThumbMove_DragDelta;
				m_ThumbEnd.DragCompleted += Thumb_DragCompleted;
				m_ThumbStart.DragCompleted += Thumb_DragCompleted;
				ThumbMove.DragCompleted += Thumb_DragCompleted;
			}
		}

		public override string CalcMeasureInfo()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (base.AnnotationDescription.Trim() != "" && base.isMsVisble)
			{
				if (base.isVisble == Visibility.Visible)
				{
					stringBuilder.AppendLine(Setting.AnnoDesStr + base.AnnotationDescription);
				}
				else
				{
					stringBuilder.Append(Setting.AnnoDesStr + base.AnnotationDescription);
				}
			}
			if (base.isVisble == Visibility.Collapsed && (!base.isMsVisble || base.AnnotationDescription.Trim() == ""))
			{
				base.MTextBlock.Visibility = Visibility.Collapsed;
			}
			if (base.isVisble == Visibility.Visible)
			{
				stringBuilder.Append(Setting.xLength + GetLength() + Setting.Unit);
			}
			return stringBuilder.ToString();
		}

		public override string CalcMeasureInfo1()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Setting.xLength + GetLength() + Setting.Unit);
			return stringBuilder.ToString();
		}

		public override void CreateMTextBlock()
		{
			base.MTextBlock = new TextBlock();
			base.MTextBlock.Visibility = Visibility.Collapsed;
			base.MTextBlock.Background = new SolidColorBrush(Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
			base.MTextBlock.Opacity = 0.7;
			base.MTextBlock.MaxWidth = 150.0;
			base.MTextBlock.TextWrapping = TextWrapping.Wrap;
			base.MTextBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
			base.MTextBlock.FontWeight = FontWeights.Bold;
			base.MTextBlock.Padding = new Thickness(2.0, 2.0, 2.0, 2.0);
			base.MTextBlock.Text = CalcMeasureInfo();
			base.MTextBlock.SetValue(Canvas.LeftProperty, (base.OriginStart.X + base.OriginEnd.X) / 2.0);
			base.MTextBlock.SetValue(Canvas.TopProperty, (base.OriginStart.Y + base.OriginEnd.Y) / 2.0);
			M_FiguresCanvas.Children.Add(base.MTextBlock);
		}

		public override void IsActive(Visibility A)
		{
			if (ThumbEnd != null)
			{
				ThumbEnd.Visibility = A;
				ThumbStart.Visibility = A;
				ThumbMove.Visibility = A;
			}
		}

		private string GetLength()
		{
			double num = Math.Abs(Connector.X1 - Connector.X2);
			double num2 = Math.Abs(Connector.Y1 - Connector.Y2);
			double value = Math.Sqrt(num * num + num2 * num2) * base.Calibration / base.msi.ZoomableCanvas.Scale;
			return Math.Round(value, 2).ToString();
		}

		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("Region");
			writer.WriteAttributeString("Guid", base.ControlName);
			writer.WriteAttributeString("Name", base.AnnotationName);
			writer.WriteAttributeString("Detail", base.AnnotationDescription);
			writer.WriteAttributeString("FontSize", base.FontSize.ToString());
			writer.WriteAttributeString("FontItalic", base.FontItalic.ToString());
			writer.WriteAttributeString("FontBold", base.FontBold.ToString());
			writer.WriteAttributeString("Size", base.Size.ToString());
			writer.WriteAttributeString("FigureType", base.AnnotationType.ToString());
			writer.WriteAttributeString("Hidden", base.isHidden.ToString());
			writer.WriteAttributeString("Zoom", base.Zoom.ToString());
			writer.WriteAttributeString("Visible", base.isVisble.ToString());
			writer.WriteAttributeString("MsVisble", base.isMsVisble.ToString());
			writer.WriteAttributeString("Color", GetColor().ToString());
			writer.WriteAttributeString("PinType", base.PinType);
			writer.WriteStartElement("Vertices");
			writer.WriteStartElement("Vertice");
			writer.WriteAttributeString("X", base.CurrentStart.X.ToString());
			writer.WriteAttributeString("Y", base.CurrentStart.Y.ToString());
			writer.WriteEndElement();
			writer.WriteStartElement("Vertice");
			writer.WriteAttributeString("X", base.CurrentEnd.X.ToString());
			writer.WriteAttributeString("Y", base.CurrentEnd.Y.ToString());
			writer.WriteEndElement();
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		public override void UpdateVisual()
		{
			if (ThumbEnd != null)
			{
				if (base.AnnotationDescription.Trim() != "" || base.isVisble == Visibility.Visible)
				{
					base.MTextBlock.Visibility = Visibility.Visible;
				}
				if (base.isVisble == Visibility.Collapsed && !base.isMsVisble)
				{
					base.MTextBlock.Visibility = Visibility.Collapsed;
				}
				if (base.isVisble == Visibility.Collapsed && base.AnnotationDescription.Trim() == "")
				{
					base.MTextBlock.Visibility = Visibility.Collapsed;
				}
				double x = MsiToCanvas(base.CurrentStart).X;
				double y = MsiToCanvas(base.CurrentStart).Y;
				double x2 = MsiToCanvas(base.CurrentEnd).X;
				double y2 = MsiToCanvas(base.CurrentEnd).Y;
				double angle = Math.Atan2(y - y2, x - x2) * 180.0 / Math.PI;
				Connector.StrokeThickness = base.Size;
				Connector.Stroke = base.BorderBrush;
				Cap.StrokeThickness = base.Size * 10.0;
				Cap.Stroke = base.BorderBrush;
				Connector.X1 = x;
				Connector.X2 = x2;
				Connector.Y1 = y;
				Connector.Y2 = y2;
				Cap.X1 = x2;
				Cap.Y1 = y2;
				Cap.X2 = x2;
				Cap.Y2 = y2;
				RTF.CenterX = x2;
				RTF.CenterY = y2;
				RTF.Angle = angle;
				m_ThumbStart.SetValue(Canvas.LeftProperty, Connector.X1 - Setting.Thumb_w / 2.0);
				m_ThumbStart.SetValue(Canvas.TopProperty, Connector.Y1 - Setting.Thumb_w / 2.0);
				m_ThumbEnd.SetValue(Canvas.LeftProperty, Connector.X2 - Setting.Thumb_w / 2.0);
				m_ThumbEnd.SetValue(Canvas.TopProperty, Connector.Y2 - Setting.Thumb_w / 2.0);
				ThumbMove.SetValue(Canvas.LeftProperty, (Connector.X1 + Connector.X2) / 2.0 - Setting.Thumb_c / 2.0);
				ThumbMove.SetValue(Canvas.TopProperty, (Connector.Y1 + Connector.Y2) / 2.0 - Setting.Thumb_c / 2.0);
				Canvas.SetLeft(base.MTextBlock, (Connector.X1 + Connector.X2) / 2.0);
				Canvas.SetTop(base.MTextBlock, (Connector.Y1 + Connector.Y2) / 2.0);
				base.MTextBlock.Visibility = base.isHidden;
				base.MTextBlock.Text = CalcMeasureInfo();
				Connector.Visibility = base.isHidden;
				Cap.Visibility = base.isHidden;
			}
		}

		public override void MoveUP()
		{
			ResetLocation(Direction.Center, 0.0, -Setting.Move_Step);
		}

		public override void MoveDown()
		{
			ResetLocation(Direction.Center, 0.0, Setting.Move_Step);
		}

		public override void MoveLeft()
		{
			ResetLocation(Direction.Center, -Setting.Move_Step, 0.0);
		}

		public override void MoveRight()
		{
			ResetLocation(Direction.Center, Setting.Move_Step, 0.0);
		}
	}
}
