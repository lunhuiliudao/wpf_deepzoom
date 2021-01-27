using KFBIO.DeepZoom;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyDeepZoomTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Point lastMouseDownPos;
        private System.Windows.Point lastMousePos;
        private double MaxScaleZoom = 40.0;
        public double curscale;
        private bool isDrag;
        private MultiScaleImage msi = null;

        /// <summary>
        /// 无参构造
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.InitWindowSize();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitMultiScaleImage();
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        private void InitMultiScaleImage()
        {
            this.msi = new MultiScaleImage();
            //string imgDir = @"D:\MyTestCode\wpf_deepzoom_demo\MyDeepZoomTest\MyDeepZoomTest\Doc\Sample\9_files\";
            //this.msi.Source = new MyMultiScaleTileSource(65792, 63488, 256, 0, imgDir); // 9层图
            //this.msi.Source = new MyMultiScaleTileSource(33024, 31744, 256, 0, imgDir); // 8层图
            //this.msi.Source = new MyMultiScaleTileSource(16640, 15872, 256, 0, imgDir); // 7层图
            //this.msi.Source = new MyMultiScaleTileSource(8448, 7936, 256, 0, imgDir); // 6层图
            //this.msi.Source = new MyMultiScaleTileSource(4352, 4096, 256, 0, imgDir); // 5层图
            //this.msi.Source = new MyMultiScaleTileSource(2304, 2048, 256, 0, imgDir); // 4层图
            //this.msi.Source = new MyMultiScaleTileSource(1280, 1024, 256, 0, imgDir); // 3层图
            //this.msi.Source = new MyMultiScaleTileSource(768, 512, 256, 0, imgDir); // 2层图

            string datPath = @"D:\WSITestData\DatImages\411681C03200807006.dat";
            this.msi.Source = new DatMultiScaleTileSource(65792, 63488, 256, 0, datPath);

            this.Bg.Children.Add(this.msi);

            this.msi.MouseLeftButtonDown += Msi_MouseLeftButtonDown;
            this.msi.MouseMove += Msi_MouseMove;
            this.msi.MouseLeftButtonUp += Msi_MouseLeftButtonUp;
            this.msi.MouseWheel += Msi_MouseWheel;
            ZoomableCanvas.Refresh += ZoomableCanvas_Refresh;
        }

        private void Msi_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isDrag = false;
        }

        private void Msi_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(this);
            this.lastMouseDownPos = e.GetPosition(msi);
            this.isDrag = true;
        }

        private void Msi_MouseMove(object sender, MouseEventArgs e)
        {
            lastMousePos = e.GetPosition(msi);
            if (isDrag)
            {
                double x = msi.ZoomableCanvas.Offset.X + (lastMouseDownPos.X - lastMousePos.X);
                double y = msi.ZoomableCanvas.Offset.Y + (lastMouseDownPos.Y - lastMousePos.Y);
                msi.ZoomableCanvas.Offset = new System.Windows.Point(x, y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                lastMouseDownPos = lastMousePos;
            }
        }

        private void ZoomableCanvas_Refresh(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.Refresh();
        }

        private void Refresh()
        {
            if (this.msi.ZoomableCanvas == null)
            {
                return;
            }

            this.curscale = msi.ZoomableCanvas.Scale * this.MaxScaleZoom;
        }

        /// <summary>
        /// 鼠标滚轮事件
        /// </summary>
        private void Msi_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            System.Windows.Point position = e.GetPosition(this);
            double tmpScale = this.curscale;
            tmpScale = ((e.Delta <= 0) ? (tmpScale - 0.268745) : (tmpScale + 0.268745));
            ZoomRatio(tmpScale, position.X, position.Y);
        }

        public void ZoomRatio(double zoom_ratio, double x, double y)
        {
            if(zoom_ratio < 0)
            {
                return;
            }
            System.Windows.Point point = new System.Windows.Point(x, y);
            System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
            point = new System.Windows.Point(point.X - this.Bg.ActualWidth / 2.0, point.Y - this.Bg.ActualHeight / 2.0);
            System.Windows.Point point2 = point;
            double x2 = point2.X + msi.ActualWidth / 2.0;
            double y2 = point2.Y + msi.ActualHeight / 2.0;
            System.Windows.Point elementPoint = new System.Windows.Point(x2, y2);
            System.Windows.Point point3 = msi.ElementToLogicalPoint(elementPoint);
            msi.ZoomAboutLogicalPoint(zoom_ratio / curscale, point3.X, point3.Y);
            curscale = zoom_ratio;
        }

        /// <summary>
        /// 全屏，但现实任务栏
        /// </summary>
        private void InitWindowSize()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.CanMinimize;
            base.Top = 0;
            base.Left = 0;
            base.Width = SystemParameters.WorkArea.Width;
            base.Height = SystemParameters.WorkArea.Height;
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                if (System.Windows.MessageBox.Show("确认退出？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
        }
    }
}
