using KFBIO.DeepZoom;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Blake.NUI.WPF.Touch
{
	public class MouseTouchDevice : TouchDevice
	{
		public MouseTouchDevice device;

		public Point Offset;

		public Point Position
		{
			get;
			set;
		}

		public event RoutedEventHandler TMove;

		public void RegisterEvents(FrameworkElement root)
		{
			root.MouseLeftButtonDown += MouseDown;
			root.MouseMove += MouseMove;
			root.MouseLeftButtonUp += MouseUp;
			root.LostMouseCapture += LostMouseCapture;
			root.MouseLeave += MouseLeave;
		}

		public void unRegisterEvents(FrameworkElement root)
		{
			root.MouseLeftButtonDown -= MouseDown;
			root.MouseMove -= MouseMove;
			root.MouseLeftButtonUp -= MouseUp;
			root.LostMouseCapture -= LostMouseCapture;
			root.MouseLeave -= MouseLeave;
		}

		public void MouseDown(object sender, MouseButtonEventArgs e)
		{
			Common.IsMoveComplete = false;
			if (device != null && device.IsActive)
			{
				device.ReportUp();
				device.Deactivate();
				device = null;
			}
			device = new MouseTouchDevice(e.MouseDevice.GetHashCode());
			device.SetActiveSource(e.MouseDevice.ActiveSource);
			device.Position = e.GetPosition(null);
			device.Activate();
			device.ReportDown();
		}

		private void MouseMove(object sender, MouseEventArgs e)
		{
			if (device != null && device.IsActive)
			{
				device.Position = e.GetPosition(null);
				device.ReportMove();
				Offset = ((MultiScaleImage)sender).ZoomableCanvas.Offset;
				if (this.TMove != null)
				{
					this.TMove(sender, e);
				}
			}
		}

		private void MouseUp(object sender, MouseButtonEventArgs e)
		{
			LostMouseCapture(sender, e);
		}

		private void LostMouseCapture(object sender, MouseEventArgs e)
		{
			Common.IsMoveComplete = true;
			if (device != null && device.IsActive)
			{
				device.Position = e.GetPosition(null);
				device.ReportUp();
				device.Deactivate();
				device = null;
			}
		}

		private void MouseLeave(object sender, MouseEventArgs e)
		{
			LostMouseCapture(sender, e);
		}

		public MouseTouchDevice(int deviceId)
			: base(deviceId)
		{
			Position = default(Point);
		}

		public override TouchPointCollection GetIntermediateTouchPoints(IInputElement relativeTo)
		{
			return new TouchPointCollection();
		}

		public override TouchPoint GetTouchPoint(IInputElement relativeTo)
		{
			Point point = Position;
			if (relativeTo != null)
			{
				point = ActiveSource.RootVisual.TransformToDescendant((Visual)relativeTo).Transform(Position);
			}
			Rect bounds = new Rect(point, new Size(1.0, 1.0));
			return new TouchPoint(this, point, bounds, TouchAction.Move);
		}
	}
}
