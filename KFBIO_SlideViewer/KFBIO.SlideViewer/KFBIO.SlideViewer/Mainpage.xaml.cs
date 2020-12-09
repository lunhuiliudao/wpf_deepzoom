using KFBIO.Common;
using KFBIO.DeepZoom;
using KFBIO.DynamicGeometry;
using KFBIO.SlideViewer.code;
using Shazzam.Shaders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.Toolkit;

namespace KFBIO.SlideViewer
{
    /// <summary>
    /// Mainpage.xaml 的交互逻辑
    /// </summary>
    public partial class Mainpage : System.Windows.Controls.UserControl
    {
        public enum ImageFormat
        {
            JPG,
            BMP,
            PNG,
            GIF,
            TIF
        }

        private CtcList CtcWind;

        private Dictionary<int, myDiyCtcRectangle> DiyCtcRectangleDic = new Dictionary<int, myDiyCtcRectangle>();

        private List<AnnotationBase> objectlist = new List<AnnotationBase>();

        private List<myTmaRectangle> Tmaobjectlist = new List<myTmaRectangle>();

        private myAnalysis _myAnalysis;

        private myArrowLine _myArrowLine;

        private myEllipse _myEllipse;

        private myLine _myLine;

        private myPin _myPin;

        private myPolyline _myPolyline;

        private myRectangle _myRectangle;

        private myDiyCtcRectangle _myDiyCtcRectangle;

        private ImgfiterEffect Imgfiter = new ImgfiterEffect();

        private ImgfiterEffect TempImgfiter = new ImgfiterEffect();

        private IMAGE_INFO_STRUCT InfoStruct;

        private AnnoListControl alc = new AnnoListControl();

        private List<System.Windows.Controls.Image> ListImg = new List<System.Windows.Controls.Image>();

        public AnnoListWind _AnnoListWind;

        private AnnoWind _AnnoWind;

        public AdjustmentWind _AdjustmentWind;

        public CaseInfoWind _CaseInfoWind;

        private System.Windows.Shapes.Path _AnnoPath;

        private VideoWind _VideoWind;

        private IniFile ifile;

        private DllImageFuc _DllImageFuc;

        private MultiScaleImage msi;

        private myRectZoom _myRectZoom;

        private bool IsDrop;

        public double Curscale;

        private bool Ischange;

        private double fitratio;

        private double fitx;

        private double fity;

        private double KeyStep = 50.0;

        private string TempPath = string.Empty;

        private string TempFilename = string.Empty;

        public string FilePath = string.Empty;

        public int SlideZoom;

        private double ImageW;

        private double ImageH;

        private double Calibration;

        private int TileSize;

        public bool IsDw;

        private double Gama;

        private double Contrast;

        private double Bright;

        private double R;

        private double G;

        private double B;

        private double Sharpen;

        private double Gama_Default;

        private double Contrast_Default;

        private double Bright_Default;

        private double R_Default;

        private double G_Default;

        private double B_Default;

        private double Gama_Compare;

        private double Contrast_Compare;

        private double Bright_Compare;

        private double R_Compare;

        private double G_Compare;

        private double B_Compare;

        private double Sharpen_Compare;

        private double Gama_Personal;

        private double Contrast_Personal;

        private double Bright_Personal;

        private double R_Personal;

        private double G_Personal;

        private double B_Personal;

        private int p_S_shift;

        private int p_V_shift;

        private int p_b;

        private int p_b_r;

        private int p_b_h;

        private int p_r;

        private int p_r_r;

        private int p_r_h;

        private float p_dbd;

        private int p_sp;

        private System.Windows.Point lastMouseDownPos;

        private System.Windows.Point lastMousePos;

        private bool isDrag;

        private bool isPerson;

        private double[] m_magV = new double[12]
        {
        320.0,
        128.0,
        60.0,
        40.0,
        30.0,
        20.0,
        10.0,
        8.0,
        5.0,
        2.0,
        1.5,
        0.0
        };

        private double[] m_adjV = new double[12]
        {
        10.0,
        5.0,
        4.0,
        3.0,
        2.0,
        1.5,
        1.0,
        0.7,
        0.5,
        0.4,
        0.3,
        0.2
        };

        private int m_prevTimeStap;

        private double m_prevNewzoom;

        public Visibility IsCaseWind = Visibility.Collapsed;

        public Visibility IsSlideWind = Visibility.Collapsed;

        public Visibility IsImageAdjustWind = Visibility.Collapsed;

        public Visibility IsAnnoManageWind = Visibility.Collapsed;

        private int DefSpeed;

        private int nCurLevel;

        private int nTotalLevel;

        private string LevelFilePath = "";

        private int MinLevel;

        private int MaxLevel;

        private int CtcCheck;

        public int MagnifierScale;

        private DispatcherTimer _Operationtimer = new DispatcherTimer();

        public DispatcherTimer _Magnifiertimer = new DispatcherTimer();

        public System.Windows.Point MagnifierMovePoint = new System.Windows.Point(0.0, 0.0);

        private DispatcherTimer _timer;

        private List<int> touch = new List<int>();

        private HSVWindow _HSVWindow;

        private List<int> idlist = new List<int>();

        private List<AnnotationBase> Tempobjectlist = new List<AnnotationBase>();

        private List<CtcVo> Tempobjectlist2 = new List<CtcVo>();

        private AnnotationBase SelecresultUser;

        private StringBuilder CaseLine = new StringBuilder();

        private myTmaRectangle _myTmaRectangle;

        private myTmaLine _myTmaLine;

        private MessageWind _TMAMessageWind;

        private BackgroundWorker BarckgroundworkerBtn;

        private List<string[]> ListTma;

        private Process myprocess;

        private DispatcherTimer timer = new DispatcherTimer();

        private int Video_Status;

        private string VDrection = "up";

        private string HDrection = "left";

        private string CruDrection = "up";

        private double CurOffsetX = -1.0;

        private int MinRuleV = 200;

        public SlideInfoWind _SlideInfoWind;

        private EditCaseInfoWind _EditCaseInfoWind;

        private double HorizontalChangeX;

        private double VerticalChangeY;

        private double Rotate;

        public Mainpage()
        {
            InitializeComponent();
            InitOnce();
        }

        public Mainpage(string filepath)
        {
            InitializeComponent();
            FilePath = filepath;
            InitOnce();
            StartupOpenFiles(filepath);
            ClearMemoryThread();
        }

        public void ClearMemoryThread()
        {
            _timer = new DispatcherTimer(DispatcherPriority.Background);
            _timer.Interval = new TimeSpan(0, 0, 3);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            int privateMemory = MemoryHelper.getPrivateMemory();
            if (privateMemory >= 1048576)
            {
                nav.m_IsDragging = false;
            }
        }

        public void StartupOpenFiles(string filename)
        {
            FilePath = filename;
            LoadMsi(filename);
            msi.Ini += msi_Ini;
        }

        public void ArcMenu()
        {
            if (_AnnoPath != null)
            {
                Zoomcanvas.Children.Remove(_AnnoPath);
                int count = ListImg.Count;
                for (int i = 0; i < count; i++)
                {
                    Zoomcanvas.Children.Remove(ListImg[i]);
                }
                ListImg.Clear();
            }
            _AnnoPath = new System.Windows.Shapes.Path();
            _AnnoPath.Visibility = Visibility.Collapsed;
            Zoomcanvas.Children.Add(_AnnoPath);
            PathGeometry pathGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure();
            ArcSegment arcSegment = new ArcSegment();
            double num = msi.ActualWidth;
            double num2 = msi.ActualHeight;
            if (Rotate > 0.0 && num > Setting.AngelMsiOffset * 2.0 && num2 > Setting.AngelMsiOffset * 2.0)
            {
                num -= Setting.AngelMsiOffset * 2.0;
                num2 -= Setting.AngelMsiOffset * 2.0;
            }
            pathFigure.StartPoint = new System.Windows.Point(num - 150.0, num2);
            arcSegment.Point = new System.Windows.Point(num, num2 - 150.0);
            arcSegment.Size = new System.Windows.Size(1.0, 1.0);
            _AnnoPath.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 74, 132));
            _AnnoPath.StrokeThickness = 4.0;
            arcSegment.IsLargeArc = true;
            arcSegment.SweepDirection = SweepDirection.Clockwise;
            pathFigure.Segments.Add(arcSegment);
            pathGeometry.Figures.Add(pathFigure);
            _AnnoPath.Data = pathGeometry;
            Zoomcanvas.Children.Add(DrawZoomImage(num - 50.0, num2 - 200.0, "1"));
            Zoomcanvas.Children.Add(DrawZoomImage(num - 100.0, num2 - 210.0, "2"));
            Zoomcanvas.Children.Add(DrawZoomImage(num - 150.0, num2 - 200.0, "4"));
            Zoomcanvas.Children.Add(DrawZoomImage(num - 190.0, num2 - 170.0, "10"));
            Zoomcanvas.Children.Add(DrawZoomImage(num - 210.0, num2 - 120.0, "20"));
            Zoomcanvas.Children.Add(DrawZoomImage(num - 200.0, num2 - 60.0, "40"));
        }

        public void IsArcMenu(Visibility v)
        {
            _AnnoPath.Visibility = v;
            int count = ListImg.Count;
            for (int i = 0; i < count; i++)
            {
                ListImg[i].Visibility = v;
            }
        }

        private System.Windows.Controls.Image DrawZoomImage(double Left, double Top, string Scale)
        {
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            if (Scale == "40")
            {
                image.Width = 50.0;
                image.Height = 50.0;
            }
            else
            {
                image.Width = 60.0;
                image.Height = 60.0;
            }
            image.Source = new BitmapImage(new Uri("images/bei" + Scale + ".png", UriKind.Relative));
            image.SetValue(Canvas.LeftProperty, Left);
            image.SetValue(Canvas.TopProperty, Top);
            image.Visibility = Visibility.Collapsed;
            image.Name = "buttonR" + Scale;
            image.MouseLeftButtonDown += ZoomClick;
            ListImg.Add(image);
            return image;
        }

        public void msi_MouseLeftButtonDown(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Keyboard.Focus(this);
            Setting.isCtrl = 0;
            Setting.Opacity = 0;
            IsDw = false;
            IsArcMenu(Visibility.Collapsed);
            if (_AnnoListWind != null)
            {
                int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
                if (selectedIndex != -1 && objectlist[selectedIndex].isFinish)
                {
                    foreach (AnnotationBase item in objectlist)
                    {
                        item.IsActive(Visibility.Collapsed);
                        _AnnoListWind.cbo_mc.SelectedIndex = -1;
                        _AnnoListWind.txt_qsr.Text = "";
                        _AnnoListWind.txt_xbz.Text = "";
                        _AnnoListWind.tbk_info.Text = "";
                        _AnnoListWind.ckb_clinfo.IsChecked = false;
                    }
                }
            }
            lastMouseDownPos = e.GetPosition(msi);
            isDrag = true;
            System.Windows.Point position = e.GetPosition(this);
            for (int i = 0; i < objectlist.Count; i++)
            {
                AnnotationBase annotationBase = objectlist[i];
                if (annotationBase.AnnotationType == AnnotationType.TmaRectangle)
                {
                    annotationBase.IsActive(Visibility.Collapsed);
                    double x = annotationBase.MsiToCanvas(annotationBase.CurrentStart).X;
                    double y = annotationBase.MsiToCanvas(annotationBase.CurrentStart).Y;
                    double x2 = annotationBase.MsiToCanvas(annotationBase.CurrentEnd).X;
                    double y2 = annotationBase.MsiToCanvas(annotationBase.CurrentEnd).Y;
                    double num = Math.Abs(x - x2);
                    double num2 = Math.Abs(y - y2);
                    x = Math.Min(x, x2);
                    y = Math.Min(y, y2);
                    double num3 = x;
                    double num4 = y;
                    double num5 = num3 + num;
                    double num6 = num4 + num2;
                    double x3 = position.X;
                    double y3 = position.Y;
                    if (x3 <= num5 && y3 <= num6 && x3 >= num3 && y3 >= num4)
                    {
                        annotationBase.IsActive(Visibility.Visible);
                        annotationBase.AnnoControl.CB.SelectedIndex = objectlist.IndexOf(annotationBase);
                    }
                }
            }
        }

        public void msi_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            lastMousePos = e.GetPosition(msi);
            if (isDrag)
            {
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {
                        if ((Mainpage)item.Value != this)
                        {
                            System.Windows.Point p = new System.Windows.Point(lastMouseDownPos.X - lastMousePos.X, lastMouseDownPos.Y - lastMousePos.Y);
                            System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
                            System.Windows.Point point = KCommon.PointRotate(center, p, 360.0 - Rotate);
                            ((Mainpage)item.Value).IsDw = false;
                            ((Mainpage)item.Value).TmoveSetOffset((Mainpage)item.Value, new System.Windows.Point(point.X, point.Y));
                            ((Mainpage)item.Value).ZoomRatio(msi.ZoomableCanvas.Scale * (double)SlideZoom);
                        }
                    }
                }
                double x = msi.ZoomableCanvas.Offset.X + (lastMouseDownPos.X - lastMousePos.X);
                double y = msi.ZoomableCanvas.Offset.Y + (lastMouseDownPos.Y - lastMousePos.Y);
                msi.ZoomableCanvas.Offset = new System.Windows.Point(x, y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                lastMouseDownPos = lastMousePos;
            }
        }

        public void msi_MouseLeftButtonMouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isDrag = false;
        }

        public void RegisterMsiEvents()
        {
            msi.TouchUp += msi_TouchUp;
            msi.TouchMove += msi_TouchMove;
            msi.TouchDown += msi_TouchDown;
            msi.MouseLeftButtonDown += msi_MouseLeftButtonDown;
            msi.MouseMove += msi_MouseMove;
            msi.MouseLeftButtonUp += msi_MouseLeftButtonMouseUp;
            msi.LostMouseCapture += msi_MouseLeftButtonMouseUp;
            msi.MouseLeave += msi_MouseLeftButtonMouseUp;
            msi.MouseEnter += msi_MouseLeftButtonMouseUp;
            msi.MouseWheel += msi_PreviewMouseWheel;
            msi.MouseDoubleClick += msi_MouseDoubleClick;
            msi.MouseRightButtonDown += msi_MouseRightButtonDown;
            msi.IsManipulationEnabled = true;
            msi.ManipulationStarting += msi_ManipulationStarting;
            msi.ManipulationDelta += msi_ManipulationDelta;
        }

        private void msi_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (touch.Count == 1)
            {
                return;
            }
            FrameworkElement frameworkElement = (FrameworkElement)e.Source;
            IsDw = false;
            int timeDelta = e.Timestamp - m_prevTimeStap;
            double num = Setting.MaxMagValue * (double)SlideZoom;
            CalcSpeed(timeDelta);
            m_prevTimeStap = e.Timestamp;
            double num2 = Curscale;
            if (num2 == m_prevNewzoom || !(m_prevNewzoom > 1E-08))
            {
                GetMagAdjValueByCurMag(Curscale);
                if (e.DeltaManipulation.Scale.X > 1.0 || e.DeltaManipulation.Scale.Y > 1.0)
                {
                    double num3 = Math.Max(e.DeltaManipulation.Scale.Y, e.DeltaManipulation.Scale.X);
                    num2 *= num3;
                }
                else if (e.DeltaManipulation.Scale.X != 0.0 && e.DeltaManipulation.Scale.Y != 0.0)
                {
                    double num4 = Math.Max(e.DeltaManipulation.Scale.Y, e.DeltaManipulation.Scale.X);
                    num2 *= num4;
                }
                if (num2 > num)
                {
                    num2 = num;
                }
                System.Windows.Point point = new System.Windows.Point(frameworkElement.ActualWidth / 2.0, frameworkElement.ActualHeight / 2.0);
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {
                        if ((Mainpage)item.Value != this)
                        {
                            ((Mainpage)item.Value).ZoomRatio(num2, point.X, point.Y);
                        }
                    }
                }
                ZoomRatio(num2, point.X, point.Y);
                m_prevNewzoom = num2;
            }
        }

        private void msi_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = msi;
            e.Mode = ManipulationModes.All;
        }

        private void msi_TouchDown(object sender, TouchEventArgs e)
        {
            IsDw = false;
            IsArcMenu(Visibility.Collapsed);
            if (_AnnoListWind != null)
            {
                int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
                if (selectedIndex != -1 && objectlist[selectedIndex].isFinish)
                {
                    foreach (AnnotationBase item in objectlist)
                    {
                        item.IsActive(Visibility.Collapsed);
                        _AnnoListWind.cbo_mc.SelectedIndex = -1;
                        _AnnoListWind.txt_qsr.Text = "";
                        _AnnoListWind.txt_xbz.Text = "";
                        _AnnoListWind.tbk_info.Text = "";
                        _AnnoListWind.ckb_clinfo.IsChecked = false;
                    }
                }
            }
            TouchPoint touchPoint = e.GetTouchPoint(msi);
            lastMouseDownPos = new System.Windows.Point(touchPoint.Position.X, touchPoint.Position.Y);
            isDrag = true;
            touch.Add(e.TouchDevice.Id);
        }

        private void msi_TouchMove(object sender, TouchEventArgs e)
        {
            if (touch.Count > 1)
            {
                return;
            }
            TouchPoint touchPoint = e.GetTouchPoint(msi);
            lastMousePos = new System.Windows.Point(touchPoint.Position.X, touchPoint.Position.Y);
            if (isDrag)
            {
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {
                        if ((Mainpage)item.Value != this)
                        {
                            ((Mainpage)item.Value).IsDw = false;
                            ((Mainpage)item.Value).TmoveSetOffset((Mainpage)item.Value, new System.Windows.Point(lastMouseDownPos.X - lastMousePos.X, lastMouseDownPos.Y - lastMousePos.Y));
                            ((Mainpage)item.Value).ZoomRatio(msi.ZoomableCanvas.Scale * (double)SlideZoom);
                        }
                    }
                }
                double x = msi.ZoomableCanvas.Offset.X + (lastMouseDownPos.X - lastMousePos.X);
                double y = msi.ZoomableCanvas.Offset.Y + (lastMouseDownPos.Y - lastMousePos.Y);
                msi.ZoomableCanvas.Offset = new System.Windows.Point(x, y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                lastMouseDownPos = lastMousePos;
            }
        }

        private void msi_TouchUp(object sender, TouchEventArgs e)
        {
            touch.Remove(e.TouchDevice.Id);
            isDrag = false;
        }

        public void UnRegisterMsiEvents()
        {
            msi.MouseLeftButtonDown -= msi_MouseLeftButtonDown;
            msi.MouseMove -= msi_MouseMove;
            msi.MouseLeftButtonUp -= msi_MouseLeftButtonMouseUp;
            msi.LostMouseCapture -= msi_MouseLeftButtonMouseUp;
            msi.MouseLeave -= msi_MouseLeftButtonMouseUp;
            msi.MouseWheel -= msi_PreviewMouseWheel;
            msi.MouseDoubleClick -= msi_MouseDoubleClick;
            msi.MouseRightButtonDown -= msi_MouseRightButtonDown;
            msi.MouseEnter -= msi_MouseLeftButtonMouseUp;
            msi.TouchUp -= msi_TouchUp;
            msi.TouchMove -= msi_TouchMove;
            msi.TouchDown -= msi_TouchDown;
            msi.IsManipulationEnabled = false;
            msi.ManipulationStarting -= msi_ManipulationStarting;
            msi.ManipulationDelta -= msi_ManipulationDelta;
        }

        public void InitOnce()
        {
            _DllImageFuc = new DllImageFuc();
            msi = new MultiScaleImage();
            Bg.Children.Add(msi);
            RegisterMsiEvents();
            Setting.IsSynchronous = false;
            ifile = new IniFile(((DockWindow)System.Windows.Application.Current.MainWindow).Configpath);
            _AnnoListWind = new AnnoListWind();
            _AnnoListWind.Owner = System.Windows.Application.Current.MainWindow;
            _AnnoListWind.txt_xbz.TextChanged += mc_TextChanged;
            _AnnoListWind.cbo_mc.SelectionChanged += cbo_mcSelectionChanged;
            _AnnoListWind.txt_qsr.TextChanged += txt_qsrTextChanged;
            _AnnoListWind.btnyc.MouseLeftButtonDown += btnyc_change;
            _AnnoListWind.btnqyc.MouseLeftButtonDown += allhidden_change;
            _AnnoListWind.ckb_clinfo.Click += ckb_clinfo;
            _AnnoListWind.btnsc.MouseLeftButtonDown += DeleteItem;
            _AnnoListWind.btnallsc.MouseLeftButtonDown += btnallsc_MouseLeftButtonDown;
            _AnnoListWind.cbo_mc.DropDownOpened += DropDownOpened;
            _AnnoListWind.cbo_mc.DropDownClosed += DropDownClosed;
            _AnnoListWind.LineWidthComboBox.SelectionChanged += LineWidthComboBox_SelectionChanged;
            _AnnoListWind._colorPicker.SelectedColorChanged += _colorPicker_SelectedColorChanged;
            _AnnoListWind.Rad_1.Checked += Rad_Checked;
            _AnnoListWind.Rad_2.Checked += Rad_Checked;
            _AnnoListWind.Rad_3.Checked += Rad_Checked;
            _AnnoListWind.Rad_4.Checked += Rad_Checked;
            _AnnoListWind.ShowMs.Click += ShowMs_Click;
            _AnnoListWind.All_ClShow.Click += All_ClShow_Click;
            _AnnoListWind.All_ClHidden.Click += All_ClHidden_Click;
            _AnnoListWind.All_MsShow.Click += All_MsShow_Click;
            _AnnoListWind.All_MsHidden.Click += All_MsHidden_Click;
            _AnnoListWind.CloseHandler += _AnnoListWind_CloseHandler;
            _AnnoWind = new AnnoWind();
            _AnnoWind.Owner = System.Windows.Application.Current.MainWindow;
            _AnnoWind._SaveAnno.Click += _SaveAnno_Click;
            _AnnoWind._CancelAnno.Click += _CancelAnno_Click;
            _CaseInfoWind = new CaseInfoWind();
            _CaseInfoWind.edit.Click += edit_Click;
            _CaseInfoWind.CloseHandler += _CaseInfoWind_CloseHandler;
            _AdjustmentWind = new AdjustmentWind();
            _AdjustmentWind.Owner = System.Windows.Application.Current.MainWindow;
            _AdjustmentWind.GamaSlider.ValueChanged += GamaSlider_ValueChanged;
            _AdjustmentWind.ContrastSilder.ValueChanged += ContrastSlider_ValueChanged;
            _AdjustmentWind.BrightSlider.ValueChanged += BrightSlider_ValueChanged;
            _AdjustmentWind.Sharpen.ValueChanged += Sharpen_ValueChanged;
            _AdjustmentWind.RSlider.ValueChanged += RSlider_ValueChanged;
            _AdjustmentWind.GSlider.ValueChanged += GSlider_ValueChanged;
            _AdjustmentWind.BSlider.ValueChanged += BSlider_ValueChanged;
            _AdjustmentWind.SysSetRadio.Click += SysSetRadio_Click;
            _AdjustmentWind.PersonSetRadio.Click += PersonSetRadio_Click;
            _AdjustmentWind.SysSetRadio.IsChecked = true;
            _AdjustmentWind.cancle.Click += cancle_Click;
            _AdjustmentWind.reset.Click += reset_Click;
            _AdjustmentWind.apply.Click += apply_Click;
            _AdjustmentWind.Compare.AddHandler(UIElement.MouseDownEvent, new RoutedEventHandler(Compare_MouseLeftButtonDown), true);
            _AdjustmentWind.Compare.AddHandler(UIElement.MouseUpEvent, new RoutedEventHandler(Compare_MouseLeftButtonUp), true);
            _AdjustmentWind._CloseButton.Click += cancle_Click;
            _HSVWindow = new HSVWindow();
            _HSVWindow.Owner = System.Windows.Application.Current.MainWindow;
            _HSVWindow._CloseButton.Click += _CloseButton_Click;
            _HSVWindow.cancle.Click += _CloseButton_Click;
            _HSVWindow.reset.Click += reset2_Click;
            _HSVWindow.rh_slider.ValueChanged += rh_slider_ValueChanged;
            _HSVWindow.Sshift_Silder.ValueChanged += Sshift_Silder_ValueChanged;
            _HSVWindow.Vshift_Silder.ValueChanged += Vshift_Silder_ValueChanged;
            _HSVWindow.k_Silder.ValueChanged += k_Silder_ValueChanged;
            _HSVWindow.b_Slider.ValueChanged += b_Slider_ValueChanged;
            _HSVWindow.b_rSlider.ValueChanged += b_rSlider_ValueChanged;
            _HSVWindow.b_hSlider.ValueChanged += b_hSlider_ValueChanged;
            _HSVWindow.r_Slider.ValueChanged += r_Slider_ValueChanged;
            _HSVWindow.r_rSlider.ValueChanged += r_rSlider_ValueChanged;
            _HSVWindow.r_hSlider.ValueChanged += r_hSlider_ValueChanged;
            _HSVWindow.rh_slider.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider1_MouseLeftButtonUp), true);
            _HSVWindow.Sshift_Silder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider1_MouseLeftButtonUp), true);
            _HSVWindow.Vshift_Silder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider1_MouseLeftButtonUp), true);
            _HSVWindow.k_Silder.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider1_MouseLeftButtonUp), true);
            _HSVWindow.b_Slider.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider1_MouseLeftButtonUp), true);
            _HSVWindow.b_rSlider.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider1_MouseLeftButtonUp), true);
            _HSVWindow.b_hSlider.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider1_MouseLeftButtonUp), true);
            _HSVWindow.r_Slider.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider1_MouseLeftButtonUp), true);
            _HSVWindow.r_rSlider.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider1_MouseLeftButtonUp), true);
            _HSVWindow.r_hSlider.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Slider1_MouseLeftButtonUp), true);
            _HSVWindow.isopen.Click += isopen_Click;
            x3dSlider.UpZ.MouseLeftButtonDown += UpZ_MouseLeftButtonDown;
            x3dSlider.DownZ.MouseLeftButtonDown += DownZ_MouseLeftButtonDown;
            Gama_Default = double.Parse(ifile.IniReadValue("Gama", "Default"));
            B_Default = double.Parse(ifile.IniReadValue("B", "Default"));
            R_Default = double.Parse(ifile.IniReadValue("R", "Default"));
            G_Default = double.Parse(ifile.IniReadValue("G", "Default"));
            Bright_Default = double.Parse(ifile.IniReadValue("Bright", "Default"));
            Contrast_Default = double.Parse(ifile.IniReadValue("Contrast", "Default"));
            p_S_shift = int.Parse(ifile.IniReadValue("HSV", "param_s_shift"));
            p_V_shift = int.Parse(ifile.IniReadValue("HSV", "param_v_shift"));
            p_b = int.Parse(ifile.IniReadValue("HSV", "param_b"));
            p_b_r = int.Parse(ifile.IniReadValue("HSV", "param_b_r"));
            p_b_h = int.Parse(ifile.IniReadValue("HSV", "param_b_h"));
            p_r = int.Parse(ifile.IniReadValue("HSV", "param_r"));
            p_r_r = int.Parse(ifile.IniReadValue("HSV", "param_r_r"));
            p_r_h = int.Parse(ifile.IniReadValue("HSV", "param_r_h"));
            p_dbd = float.Parse(ifile.IniReadValue("HSV", "param_k"));
            p_sp = int.Parse(ifile.IniReadValue("HSV", "param_sp"));
            sethsvenble(false);
            _HSVWindow.rh_slider.Value = p_sp;
            _HSVWindow.Sshift_Silder.Value = p_S_shift;
            _HSVWindow.Vshift_Silder.Value = p_V_shift;
            _HSVWindow.k_Silder.Value = p_dbd;
            _HSVWindow.b_Slider.Value = p_b;
            _HSVWindow.b_rSlider.Value = p_b_r;
            _HSVWindow.b_hSlider.Value = p_b_h;
            _HSVWindow.r_Slider.Value = p_r;
            _HSVWindow.r_rSlider.Value = p_r_r;
            _HSVWindow.r_hSlider.Value = p_r_h;
            _AdjustmentWind.GamaSlider.Value = Gama_Default;
            _AdjustmentWind.BSlider.Value = B_Default;
            _AdjustmentWind.RSlider.Value = R_Default;
            _AdjustmentWind.GSlider.Value = G_Default;
            _AdjustmentWind.BrightSlider.Value = Bright_Default;
            _AdjustmentWind.ContrastSilder.Value = Contrast_Default;
            _SlideInfoWind = new SlideInfoWind();
            _SlideInfoWind.Owner = System.Windows.Application.Current.MainWindow;
            _SlideInfoWind.CloseHandler += _SlideInfoWind_CloseHandler;
            Setting.Move_Video_Speed = int.Parse(ifile.IniReadValue("AutoPaly", "Speed"));
            Setting.Move_Video_Step = int.Parse(ifile.IniReadValue("AutoPaly", "Step"));
            DefSpeed = Setting.Move_Video_Speed;
            msi.Effect = Imgfiter;
            int num = 0;
            string filePath = FilePath;
            TempPath = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);
            TempFilename = filePath.Substring(filePath.LastIndexOf("\\") + 1, filePath.Length - filePath.LastIndexOf("\\") - 1);
            if (File.Exists(TempPath + TempFilename + ".Ano"))
            {
                num = LoadPersonImageProcess(TempPath + TempFilename + ".Ano");
            }
            if (num == 0)
            {
                _AdjustmentWind.GamaSlider.Value = double.Parse(ifile.IniReadValue("Gama", "Value"));
                _AdjustmentWind.BSlider.Value = double.Parse(ifile.IniReadValue("B", "Value"));
                _AdjustmentWind.RSlider.Value = double.Parse(ifile.IniReadValue("R", "Value"));
                _AdjustmentWind.GSlider.Value = double.Parse(ifile.IniReadValue("G", "Value"));
                _AdjustmentWind.BrightSlider.Value = double.Parse(ifile.IniReadValue("Bright", "Value"));
                _AdjustmentWind.ContrastSilder.Value = double.Parse(ifile.IniReadValue("Contrast", "Value"));
            }
            canvasboard.MouseRightButtonDown += msi_MouseRightButtonDown;
            ZoomableCanvas.Refresh += ZoomableCanvas_Refresh;
            bk_Scale.MouseEnter += bk_Scale_MouseEnter;
            bk_Scale.MouseLeftButtonDown += bk_Scale_MouseEnter;
            lbl_Scale.MouseEnter += bk_Scale_MouseEnter;
            lbl_Scale.MouseLeftButtonDown += bk_Scale_MouseEnter;
            System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();
            System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem();
            ColorPicker colorPicker = new ColorPicker();
            colorPicker.SelectedColorChanged += _RuleColorPicker_SelectedColorChanged;
            colorPicker.DisplayColorAndName = true;
            colorPicker.Width = 40.0;
            colorPicker.Margin = new Thickness(-13.0, 0.0, -10.0, 0.0);
            menuItem.Header = colorPicker;
            menuItem.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/color.png", UriKind.Relative)),
                Width = 20.0,
                Height = 20.0
            };
            contextMenu.Items.Add(menuItem);
            Separator newItem = new Separator();
            contextMenu.Items.Add(newItem);
            System.Windows.Controls.MenuItem menuItem2 = new System.Windows.Controls.MenuItem();
            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            label.Content = "移动";
            label.FontSize = 12.0;
            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label.VerticalContentAlignment = VerticalAlignment.Top;
            label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header = label;
            menuItem2.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/move.png", UriKind.Relative)),
                Width = 20.0,
                Height = 20.0
            };
            menuItem2.Header = header;
            menuItem2.Click += Menu_Move_Click;
            contextMenu.Items.Add(menuItem2);
            RuleCanvas.ContextMenu = contextMenu;
            _VideoWind = new VideoWind();
            _VideoWind.Owner = System.Windows.Application.Current.MainWindow;
            _VideoWind.Btn_Play.Click += Btn_Play_Click;
            _VideoWind._ZoomSlider.ValueChanged += _ZoomSlider_ValueChanged;
            timer.Tick += timer_Tick;
            _Operationtimer = new DispatcherTimer(DispatcherPriority.Background);
            _Operationtimer.Interval = new TimeSpan(0, 0, 0, 0, 80);
            _Operationtimer.Tick += _Operationtimer_Tick;
            Thumb_Magnifier.DragDelta += Thumb_Magnifier_DragDelta;
            _Magnifiertimer = new DispatcherTimer(DispatcherPriority.Background);
            _Magnifiertimer.Interval = new TimeSpan(0, 0, 0, 0, 80);
            _Magnifiertimer.Tick += _Magnifiertimer_Tick;
            _RotateViewer.ThumbAngle.DragDelta += ThumbAngle_DragDelta;
            _RotateViewer.RotateHandler += _RotateViewer_RotateHandler;
            _RotateViewer.Rotate_Anticlockwise.MouseLeftButtonDown += Rotate_Anticlockwise_MouseLeftButtonDown;
            _RotateViewer.Rotate_Clockwise.MouseLeftButtonDown += Rotate_Clockwise_MouseLeftButtonDown;
            _RotateViewer.lbl_Angle.MouseDown += centerelp_MouseLeftButtonDown;
            LayoutBody.MouseMove += LayoutBody_MouseMove;
            MagnifierScale = Setting.Magnifier;
            CreateMagnifierMenu();
            RoiOutPut.Click += RoiOutPut_Click;
            CurRoiOutPut.Click += CurRoiOutPut_Click;
            if (Setting.IsLogo == "1")
            {
                img_logo.Visibility = Visibility.Visible;
            }
            else
            {
                img_logo.Visibility = Visibility.Collapsed;
            }
        }

        public void sethsvenble(bool f)
        {
            _HSVWindow.rh_slider.IsEnabled = f;
            _HSVWindow.Sshift_Silder.IsEnabled = f;
            _HSVWindow.Vshift_Silder.IsEnabled = f;
            _HSVWindow.k_Silder.IsEnabled = f;
            _HSVWindow.b_Slider.IsEnabled = f;
            _HSVWindow.b_rSlider.IsEnabled = f;
            _HSVWindow.b_hSlider.IsEnabled = f;
            _HSVWindow.r_Slider.IsEnabled = f;
            _HSVWindow.r_rSlider.IsEnabled = f;
            _HSVWindow.r_hSlider.IsEnabled = f;
            _HSVWindow.reset.IsEnabled = f;
        }

        private void isopen_Click(object sender, RoutedEventArgs e)
        {
            sethsvenble(_HSVWindow.isopen.IsChecked.Value);
            ((MagicZoomTileSource1)msi.Source).Setisopenhsl(_HSVWindow.isopen.IsChecked.Value);
            Setting.Opacity = 1;
            if (_HSVWindow.isopen.IsChecked.Value)
            {
                UpdateHsv();
                return;
            }
            LinkedListNode<int> linkedListNode = msi.ZoomableCanvas.RealizedItems.First;
            LinkedListNode<int> last = msi.ZoomableCanvas.RealizedItems.Last;
            while (linkedListNode != last)
            {
                LinkedListNode<int> next = linkedListNode.Next;
                int value = linkedListNode.Value;
                msi.ZoomableCanvas.VirtualizeItem(value);
                msi.ZoomableCanvas.RealizeItem(value);
                linkedListNode = next;
            }
            if (linkedListNode == last)
            {
                int value2 = linkedListNode.Value;
                msi.ZoomableCanvas.VirtualizeItem(value2);
                msi.ZoomableCanvas.RealizeItem(value2);
            }
        }

        private void reset2_Click(object sender, RoutedEventArgs e)
        {
            _HSVWindow.Sshift_Silder.Value = p_S_shift;
            _HSVWindow.Vshift_Silder.Value = p_V_shift;
            _HSVWindow.b_Slider.Value = p_b;
            _HSVWindow.b_rSlider.Value = p_b_r;
            _HSVWindow.b_hSlider.Value = p_b_h;
            _HSVWindow.r_Slider.Value = p_r;
            _HSVWindow.r_rSlider.Value = p_r_r;
            _HSVWindow.r_hSlider.Value = p_r_h;
            _HSVWindow.k_Silder.Value = p_dbd;
            _HSVWindow.rh_slider.Value = p_sp;
            UpdateHsv();
        }

        private void _CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _HSVWindow.Hide();
        }

        public void Slider1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpdateHsv();
        }

        public void UpdateHsv()
        {
            int num = (int)_HSVWindow.Sshift_Silder.Value;
            int num2 = (int)_HSVWindow.Vshift_Silder.Value;
            int num3 = (int)_HSVWindow.b_Slider.Value;
            int num4 = (int)_HSVWindow.b_rSlider.Value;
            int num5 = (int)_HSVWindow.b_hSlider.Value;
            int num6 = (int)_HSVWindow.r_Slider.Value;
            int num7 = (int)_HSVWindow.r_rSlider.Value;
            int num8 = (int)_HSVWindow.r_hSlider.Value;
            float p_k = (float)_HSVWindow.k_Silder.Value;
            int num9 = (int)_HSVWindow.rh_slider.Value;
            if (msi.Source != null)
            {
                ((MagicZoomTileSource1)msi.Source).SetHSV(num, num2, num3, num4, num5, num6, num7, num8, p_k, num9);
                LinkedListNode<int> linkedListNode = msi.ZoomableCanvas.RealizedItems.First;
                LinkedListNode<int> last = msi.ZoomableCanvas.RealizedItems.Last;
                Setting.Opacity = 1;
                while (linkedListNode != last)
                {
                    LinkedListNode<int> next = linkedListNode.Next;
                    int value = linkedListNode.Value;
                    msi.ZoomableCanvas.VirtualizeItem(value);
                    msi.ZoomableCanvas.RealizeItem(value);
                    linkedListNode = next;
                }
                if (linkedListNode == last)
                {
                    int value2 = linkedListNode.Value;
                    msi.ZoomableCanvas.VirtualizeItem(value2);
                    msi.ZoomableCanvas.RealizeItem(value2);
                }
            }
        }

        private void rh_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _HSVWindow.txt_rh_slider.Text = ((int)e.NewValue).ToString();
        }

        private void Sshift_Silder_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _HSVWindow.txt_Sshift_Silder.Text = ((int)e.NewValue).ToString();
        }

        private void Vshift_Silder_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _HSVWindow.txt_Vshift_Silder.Text = ((int)e.NewValue).ToString();
        }

        private void k_Silder_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _HSVWindow.txt_k_Silder.Text = Math.Round(e.NewValue, 2).ToString();
        }

        private void b_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _HSVWindow.txt_b_Slider.Text = ((int)e.NewValue).ToString();
        }

        private void b_rSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _HSVWindow.txt_b_rSlider.Text = ((int)e.NewValue).ToString();
        }

        private void b_hSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _HSVWindow.txt_b_hSlider.Text = ((int)e.NewValue).ToString();
        }

        private void r_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _HSVWindow.txt_r_Slider.Text = ((int)e.NewValue).ToString();
        }

        private void r_rSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _HSVWindow.txt_r_rSlider.Text = ((int)e.NewValue).ToString();
        }

        private void r_hSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _HSVWindow.txt_r_hSlider.Text = ((int)e.NewValue).ToString();
        }

        private void _Magnifiertimer_Tick(object sender, EventArgs e)
        {
            if (msi.ZoomableCanvas != null)
            {
                System.Windows.Point magnifierMovePoint = MagnifierMovePoint;
                System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
                magnifierMovePoint = new System.Windows.Point(magnifierMovePoint.X - LayoutBody.ActualWidth / 2.0, magnifierMovePoint.Y - LayoutBody.ActualHeight / 2.0);
                System.Windows.Point point = KCommon.PointRotate(center, magnifierMovePoint, Rotate);
                double num = msi.ZoomableCanvas.Offset.X + point.X + msi.ActualWidth / 2.0;
                double num2 = msi.ZoomableCanvas.Offset.Y + point.Y + msi.ActualHeight / 2.0;
                float num3 = 0f;
                if (MagnifierScale == 2)
                {
                    num3 = (float)Curscale * 2f;
                }
                if (MagnifierScale == 4)
                {
                    num3 = (float)Curscale * 4f;
                }
                if (MagnifierScale == SlideZoom || MagnifierScale == 0)
                {
                    num3 = SlideZoom;
                }
                int posx = (int)(num * (double)num3 / Curscale) - 100;
                int posy = (int)(num2 * (double)num3 / Curscale) - 100;
                Thumb_Magnifier.Background = new ImageBrush(GetCap(num3, posx, posy, 200, 200));
            }
        }

        private PropertyInfo[] GetPropertyInfoArray()
        {
            PropertyInfo[] result = null;
            try
            {
                Type typeFromHandle = typeof(CtcCsv);
                Activator.CreateInstance(typeFromHandle);
                result = typeFromHandle.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        private bool SaveDataToCSVFile(List<CtcCsv> CtcList, string filePath)
        {
            bool result = true;
            new StringBuilder();
            StringBuilder stringBuilder = new StringBuilder();
            StreamWriter streamWriter = null;
            GetPropertyInfoArray();
            try
            {
                streamWriter = new StreamWriter(filePath);
                for (int i = 0; i < CtcList.Count; i++)
                {
                    stringBuilder.Remove(0, stringBuilder.Length);
                    stringBuilder.Append(CtcList[i].Left);
                    stringBuilder.Append(",");
                    stringBuilder.Append(CtcList[i].Top);
                    stringBuilder.Append(",");
                    stringBuilder.Append(CtcList[i].width);
                    stringBuilder.Append(",");
                    stringBuilder.Append(CtcList[i].height);
                    streamWriter.WriteLine(stringBuilder);
                }
                return result;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                streamWriter?.Dispose();
            }
        }

        private void CurRoiOutPut_Click(object sender, RoutedEventArgs e)
        {
            CtcVo ctcVo = (CtcVo)CtcWind.DgList.SelectedItem;
            if (ctcVo != null)
            {
                int num = int.Parse(RoiOutPutW.Text);
                int num2 = int.Parse(RoiOutPutH.Text);
                string text = FilePath.Replace(".kfb", "");
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                }
                for (int i = 0; i < objectlist.Count; i++)
                {
                    if (objectlist[i].AnnotationType != AnnotationType.DiyCtcRectangle)
                    {
                        continue;
                    }
                    double num3 = objectlist[i].CurrentStart.X * (double)SlideZoom;
                    double num4 = objectlist[i].CurrentStart.Y * (double)SlideZoom;
                    double num5 = objectlist[i].CurrentEnd.X * (double)SlideZoom;
                    double num6 = objectlist[i].CurrentEnd.Y * (double)SlideZoom;
                    if (!(objectlist[i].FontSize.ToString() == ctcVo.index.ToString()))
                    {
                        continue;
                    }
                    Bitmap bitmap = null;
                    Graphics graphics = null;
                    if (objectlist[i].AnnotationName.IndexOf('+') != -1)
                    {
                        text += "\\+";
                        if (!Directory.Exists(text))
                        {
                            Directory.CreateDirectory(text);
                        }
                    }
                    if (objectlist[i].AnnotationName.IndexOf('-') != -1)
                    {
                        text += "\\-";
                        if (!Directory.Exists(text))
                        {
                            Directory.CreateDirectory(text + "\\-");
                        }
                    }
                    double num7 = num3 + (num5 - num3) / 2.0 - (double)(num / 2);
                    double num8 = num4 + (num6 - num4) / 2.0 - (double)(num2 / 2);
                    int nDataLength = 0;
                    byte[] array = new byte[0];
                    DllImageFuc.GetImageDataRoiFunc(InfoStruct, SlideZoom, (int)num7, (int)num8, num, num2, out IntPtr datas, ref nDataLength, true);
                    array = new byte[nDataLength];
                    if (datas != IntPtr.Zero)
                    {
                        Marshal.Copy(datas, array, 0, nDataLength);
                    }
                    DllImageFuc.DeleteImageDataFunc(datas);
                    MemoryStream stream = new MemoryStream(array);
                    bitmap = new Bitmap(stream);
                    bitmap.Save(text + "\\" + (int)num7 + "_" + (int)num8 + "_org.jpg");
                    graphics = Graphics.FromImage(bitmap);
                    List<CtcCsv> list = new List<CtcCsv>();
                    for (int j = 0; j < objectlist.Count; j++)
                    {
                        if (objectlist[j].AnnotationType != AnnotationType.DiyCtcRectangle)
                        {
                            continue;
                        }
                        double num9 = objectlist[j].CurrentStart.X * (double)SlideZoom;
                        double num10 = objectlist[j].CurrentStart.Y * (double)SlideZoom;
                        double num11 = objectlist[j].CurrentEnd.X * (double)SlideZoom;
                        double num12 = objectlist[j].CurrentEnd.Y * (double)SlideZoom;
                        if (!(num11 <= num7 + (double)num) || !(num12 <= num8 + (double)num2) || !(num9 >= num7) || !(num10 >= num8))
                        {
                            continue;
                        }
                        double num13 = num9 - num7;
                        double num14 = num10 - num8;
                        double num15 = num11 - num9;
                        double num16 = num12 - num10;
                        System.Drawing.Rectangle rect = new System.Drawing.Rectangle((int)num13, (int)num14, (int)num15, (int)num16);
                        System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2f);
                        graphics.DrawRectangle(pen, rect);
                        CtcCsv ctcCsv = new CtcCsv();
                        ctcCsv.Left = (int)num13;
                        ctcCsv.Top = (int)num14;
                        ctcCsv.width = (int)num15;
                        ctcCsv.height = (int)num16;
                        list.Add(ctcCsv);
                        new System.Drawing.Pen(System.Drawing.Color.Black, 2f);
                        Font font = new Font("Arial Black", 12f, System.Drawing.FontStyle.Bold);
                        PointF point = new PointF((int)(num13 + num15), (int)(num14 + num16));
                        System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.FromArgb(255, System.Drawing.Color.Black));
                        string annotationDescription = ((myDiyCtcRectangle)objectlist[j]).AnnotationDescription;
                        if (annotationDescription == "")
                        {
                            for (int k = 0; k < objectlist.Count; k++)
                            {
                                if (objectlist[k].AnnotationType == AnnotationType.Polygon)
                                {
                                    double num17 = objectlist[k].CurrentStart.X * (double)SlideZoom;
                                    double num18 = objectlist[k].CurrentStart.Y * (double)SlideZoom;
                                    if (num9 < num17 && num10 < num18 && num11 > num17 && num12 > num18)
                                    {
                                        annotationDescription = ((myPolyline)objectlist[k]).AnnotationDescription;
                                    }
                                }
                            }
                        }
                        graphics.DrawString(annotationDescription, font, brush, point);
                    }
                    graphics.Save();
                    bitmap.Save(text + "\\" + (int)num7 + "_" + (int)num8 + "_ctc.jpg");
                    bitmap.Dispose();
                    graphics.Dispose();
                    SaveDataToCSVFile(list, text + "\\" + (int)num7 + "_" + (int)num8 + "_data.csv");
                }
                Process.Start("explorer.exe ", text);
            }
            else
            {
                System.Windows.MessageBox.Show("没有当前选中项！");
            }
        }

        public void checkctc(double erectx, double erecty, double srectx, double srecty)
        {
            int width = int.Parse(RoiOutPutW.Text);
            int height = int.Parse(RoiOutPutH.Text);
            string text = FilePath.Replace(".kfb", "");
            bool flag = false;
            Bitmap bitmap = null;
            Graphics graphics = null;
            List<CtcCsv> list = new List<CtcCsv>();
            for (int i = 0; i < CtcWind.listvo.Count; i++)
            {
                double num = CtcWind.listvo[i].GlobalPosX;
                double num2 = CtcWind.listvo[i].GlobalPosY;
                double num3 = num + (double)CtcWind.listvo[i].Width;
                double num4 = num2 + (double)CtcWind.listvo[i].Height;
                if (!(num3 <= erectx) || !(num4 <= erecty) || !(num >= srectx) || !(num2 >= srecty))
                {
                    continue;
                }
                Tempobjectlist2.Remove(CtcWind.listvo[i]);
                if (!flag)
                {
                    int nDataLength = 0;
                    byte[] array = new byte[0];
                    DllImageFuc.GetImageDataRoiFunc(InfoStruct, SlideZoom, (int)srectx, (int)srecty, width, height, out IntPtr datas, ref nDataLength, true);
                    array = new byte[nDataLength];
                    if (datas != IntPtr.Zero)
                    {
                        Marshal.Copy(datas, array, 0, nDataLength);
                    }
                    DllImageFuc.DeleteImageDataFunc(datas);
                    MemoryStream stream = new MemoryStream(array);
                    bitmap = new Bitmap(stream);
                    bitmap.Save(text + "\\" + (int)srectx + "_" + (int)srecty + "_org.jpg");
                    flag = true;
                    graphics = Graphics.FromImage(bitmap);
                }
                double num5 = num - srectx;
                double num6 = num2 - srecty;
                double num7 = num3 - num;
                double num8 = num4 - num2;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle((int)num5, (int)num6, (int)num7, (int)num8);
                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2f);
                graphics.DrawRectangle(pen, rect);
                CtcCsv ctcCsv = new CtcCsv();
                ctcCsv.Left = (int)num5;
                ctcCsv.Top = (int)num6;
                ctcCsv.width = (int)num7;
                ctcCsv.height = (int)num8;
                list.Add(ctcCsv);
                new System.Drawing.Pen(System.Drawing.Color.Black, 2f);
                Font font = new Font("Arial Black", 12f, System.Drawing.FontStyle.Bold);
                PointF point = new PointF((int)(num5 + num7), (int)(num6 + num8));
                System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.FromArgb(255, System.Drawing.Color.Black));
                string text2 = "";
                if (text2 == "")
                {
                    for (int j = 0; j < objectlist.Count; j++)
                    {
                        if (objectlist[j].AnnotationType == AnnotationType.Polygon || objectlist[j].AnnotationType == AnnotationType.Line || objectlist[j].AnnotationType == AnnotationType.Remark)
                        {
                            double num9 = objectlist[j].CurrentStart.X * (double)SlideZoom;
                            double num10 = objectlist[j].CurrentStart.Y * (double)SlideZoom;
                            if (num < num9 && num2 < num10 && num3 > num9 && num4 > num10)
                            {
                                text2 = objectlist[j].AnnotationDescription;
                            }
                        }
                    }
                }
                graphics.DrawString(text2, font, brush, point);
            }
            if (flag)
            {
                graphics.Save();
                bitmap.Save(text + "\\" + (int)srectx + "_" + (int)srecty + "_ctc.jpg");
                bitmap.Dispose();
                graphics.Dispose();
                SaveDataToCSVFile(list, text + "\\" + (int)srectx + "_" + (int)srecty + "_data.csv");
                list.Clear();
            }
        }

        private void RoiOutPut_Click(object sender, RoutedEventArgs e)
        {
            int num = int.Parse(RoiOutPutW.Text);
            int num2 = int.Parse(RoiOutPutH.Text);
            string text = FilePath.Replace(".kfb", "");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            objectlist.ForEach(delegate (AnnotationBase i)
            {
                Tempobjectlist.Add(i);
            });
            CtcWind.listvo.ForEach(delegate (CtcVo i)
            {
                Tempobjectlist2.Add(i);
            });
            int num3 = (ImageW % (double)num == 0.0) ? ((int)ImageW / num) : ((int)ImageW / num + 1);
            int num4 = (ImageH % (double)num2 == 0.0) ? ((int)ImageH / num2) : ((int)ImageH / num2 + 1);
            for (int j = 0; j < num3; j++)
            {
                for (int k = 0; k < num4; k++)
                {
                    int num5 = j * num;
                    int num6 = k * num2;
                    int num7 = num5 + num;
                    int num8 = num6 + num2;
                    bool flag = false;
                    Bitmap bitmap = null;
                    Graphics graphics = null;
                    List<CtcCsv> list = new List<CtcCsv>();
                    for (int l = 0; l < objectlist.Count; l++)
                    {
                        if (objectlist[l].AnnotationType != AnnotationType.DiyCtcRectangle)
                        {
                            continue;
                        }
                        double num9 = objectlist[l].CurrentStart.X * (double)SlideZoom;
                        double num10 = objectlist[l].CurrentStart.Y * (double)SlideZoom;
                        double num11 = objectlist[l].CurrentEnd.X * (double)SlideZoom;
                        double num12 = objectlist[l].CurrentEnd.Y * (double)SlideZoom;
                        if (!(num11 <= (double)num7) || !(num12 <= (double)num8) || !(num9 >= (double)num5) || !(num10 >= (double)num6))
                        {
                            continue;
                        }
                        Tempobjectlist.Remove(objectlist[l]);
                        if (!flag)
                        {
                            Tempobjectlist2.Clear();
                            int nDataLength = 0;
                            byte[] array = new byte[0];
                            DllImageFuc.GetImageDataRoiFunc(InfoStruct, SlideZoom, num5, num6, num, num2, out IntPtr datas, ref nDataLength, true);
                            array = new byte[nDataLength];
                            if (datas != IntPtr.Zero)
                            {
                                Marshal.Copy(datas, array, 0, nDataLength);
                            }
                            DllImageFuc.DeleteImageDataFunc(datas);
                            MemoryStream stream = new MemoryStream(array);
                            bitmap = new Bitmap(stream);
                            bitmap.Save(text + "\\" + num5 + "_" + num6 + "_org.jpg");
                            flag = true;
                            graphics = Graphics.FromImage(bitmap);
                        }
                        double num13 = num9 - (double)num5;
                        double num14 = num10 - (double)num6;
                        double num15 = num11 - num9;
                        double num16 = num12 - num10;
                        System.Drawing.Rectangle rect = new System.Drawing.Rectangle((int)num13, (int)num14, (int)num15, (int)num16);
                        System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 2f);
                        graphics.DrawRectangle(pen, rect);
                        CtcCsv ctcCsv = new CtcCsv();
                        ctcCsv.Left = (int)num13;
                        ctcCsv.Top = (int)num14;
                        ctcCsv.width = (int)num15;
                        ctcCsv.height = (int)num16;
                        list.Add(ctcCsv);
                        new System.Drawing.Pen(System.Drawing.Color.Black, 2f);
                        Font font = new Font("Arial Black", 12f, System.Drawing.FontStyle.Bold);
                        PointF point = new PointF((int)(num13 + num15), (int)(num14 + num16));
                        System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.FromArgb(255, System.Drawing.Color.Black));
                        string annotationDescription = ((myDiyCtcRectangle)objectlist[l]).AnnotationDescription;
                        if (annotationDescription == "")
                        {
                            for (int m = 0; m < objectlist.Count; m++)
                            {
                                if (objectlist[m].AnnotationType == AnnotationType.Polygon)
                                {
                                    double num17 = objectlist[m].CurrentStart.X * (double)SlideZoom;
                                    double num18 = objectlist[m].CurrentStart.Y * (double)SlideZoom;
                                    if (num9 < num17 && num10 < num18 && num11 > num17 && num12 > num18)
                                    {
                                        annotationDescription = ((myPolyline)objectlist[m]).AnnotationDescription;
                                    }
                                }
                            }
                        }
                        graphics.DrawString(annotationDescription, font, brush, point);
                    }
                    if (!flag)
                    {
                        checkctc(num7, num8, num5, num6);
                    }
                    if (flag)
                    {
                        graphics.Save();
                        bitmap.Save(text + "\\" + num5 + "_" + num6 + "_ctc.jpg");
                        bitmap.Dispose();
                        graphics.Dispose();
                        SaveDataToCSVFile(list, text + "\\" + num5 + "_" + num6 + "_data.csv");
                        list.Clear();
                    }
                }
            }
            for (int n = 0; n < Tempobjectlist.Count; n++)
            {
                if (Tempobjectlist[n].AnnotationType != AnnotationType.DiyCtcRectangle)
                {
                    continue;
                }
                double num19 = Tempobjectlist[n].CurrentStart.X * (double)SlideZoom;
                double num20 = Tempobjectlist[n].CurrentStart.Y * (double)SlideZoom;
                double num21 = Tempobjectlist[n].CurrentEnd.X * (double)SlideZoom;
                double num22 = Tempobjectlist[n].CurrentEnd.Y * (double)SlideZoom;
                double num23 = num19 - 4.0;
                double num24 = num20 - 4.0;
                double num25 = num23 + (double)num;
                double num26 = num24 + (double)num2;
                double num27 = num23;
                double num28 = num24;
                float fScale = SlideZoom;
                int nDataLength2 = 0;
                byte[] array2 = new byte[0];
                DllImageFuc.GetImageDataRoiFunc(InfoStruct, fScale, (int)num27, (int)num28, num, num2, out IntPtr datas2, ref nDataLength2, true);
                array2 = new byte[nDataLength2];
                if (datas2 != IntPtr.Zero)
                {
                    Marshal.Copy(datas2, array2, 0, nDataLength2);
                }
                DllImageFuc.DeleteImageDataFunc(datas2);
                MemoryStream stream2 = new MemoryStream(array2);
                Bitmap bitmap2 = new Bitmap(stream2);
                bitmap2.Save(text + "\\" + (int)num19 + "_" + (int)num20 + "_org.jpg");
                List<CtcCsv> list2 = new List<CtcCsv>();
                Graphics graphics2 = Graphics.FromImage(bitmap2);
                for (int num29 = 0; num29 < Tempobjectlist.Count; num29++)
                {
                    if (Tempobjectlist[num29].AnnotationType != AnnotationType.DiyCtcRectangle)
                    {
                        continue;
                    }
                    double num30 = Tempobjectlist[num29].CurrentStart.X * (double)SlideZoom;
                    double num31 = Tempobjectlist[num29].CurrentStart.Y * (double)SlideZoom;
                    double num32 = Tempobjectlist[num29].CurrentEnd.X * (double)SlideZoom;
                    double num33 = Tempobjectlist[num29].CurrentEnd.Y * (double)SlideZoom;
                    double num34 = num32 - num30;
                    double num35 = num33 - num31;
                    double num36 = num30 + num34;
                    double num37 = num31 + num35;
                    if (!(num36 <= num25) || !(num37 <= num26) || !(num30 >= num23) || !(num31 >= num24))
                    {
                        continue;
                    }
                    double num38 = num30 - num27;
                    double num39 = num31 - num28;
                    System.Drawing.Rectangle rect2 = new System.Drawing.Rectangle((int)num38, (int)num39, (int)num34, (int)num35);
                    System.Drawing.Pen pen2 = new System.Drawing.Pen(System.Drawing.Color.Red, 2f);
                    graphics2.DrawRectangle(pen2, rect2);
                    new System.Drawing.Pen(System.Drawing.Color.Black, 2f);
                    Font font2 = new Font("Arial Black", 12f, System.Drawing.FontStyle.Bold);
                    PointF point2 = new PointF((int)(num38 + num34), (int)(num39 + num35));
                    System.Drawing.Brush brush2 = new SolidBrush(System.Drawing.Color.FromArgb(255, System.Drawing.Color.Black));
                    string annotationDescription2 = ((myDiyCtcRectangle)Tempobjectlist[num29]).AnnotationDescription;
                    if (annotationDescription2 == "")
                    {
                        for (int num40 = 0; num40 < objectlist.Count; num40++)
                        {
                            if (objectlist[num40].AnnotationType == AnnotationType.Polygon)
                            {
                                double num41 = objectlist[num40].CurrentStart.X * (double)SlideZoom;
                                double num42 = objectlist[num40].CurrentStart.Y * (double)SlideZoom;
                                if (num30 < num41 && num31 < num42 && num32 > num41 && num33 > num42)
                                {
                                    annotationDescription2 = ((myPolyline)objectlist[num40]).AnnotationDescription;
                                }
                            }
                        }
                    }
                    graphics2.DrawString(annotationDescription2, font2, brush2, point2);
                    CtcCsv ctcCsv2 = new CtcCsv();
                    ctcCsv2.Left = (int)num38;
                    ctcCsv2.Top = (int)num39;
                    ctcCsv2.width = (int)num34;
                    ctcCsv2.height = (int)num35;
                    list2.Add(ctcCsv2);
                }
                graphics2.Save();
                bitmap2.Save(text + "\\" + (int)num19 + "_" + (int)num20 + "_ctc.jpg");
                bitmap2.Dispose();
                graphics2.Dispose();
                SaveDataToCSVFile(list2, text + "\\" + (int)num19 + "_" + (int)num20 + "_data.csv");
            }
            for (int num43 = 0; num43 < Tempobjectlist2.Count; num43++)
            {
                double num44 = Tempobjectlist2[num43].GlobalPosX;
                double num45 = Tempobjectlist2[num43].GlobalPosY;
                double num46 = num44 + (double)Tempobjectlist2[num43].Width;
                double num47 = num45 + (double)Tempobjectlist2[num43].Height;
                double num48 = num44 - 4.0;
                double num49 = num45 - 4.0;
                double num50 = num48 + (double)num;
                double num51 = num49 + (double)num2;
                double num52 = num48;
                double num53 = num49;
                float fScale2 = SlideZoom;
                int nDataLength3 = 0;
                byte[] array3 = new byte[0];
                DllImageFuc.GetImageDataRoiFunc(InfoStruct, fScale2, (int)num52, (int)num53, num, num2, out IntPtr datas3, ref nDataLength3, true);
                array3 = new byte[nDataLength3];
                if (datas3 != IntPtr.Zero)
                {
                    Marshal.Copy(datas3, array3, 0, nDataLength3);
                }
                DllImageFuc.DeleteImageDataFunc(datas3);
                MemoryStream stream3 = new MemoryStream(array3);
                Bitmap bitmap3 = new Bitmap(stream3);
                bitmap3.Save(text + "\\" + (int)num44 + "_" + (int)num45 + "_org.jpg");
                List<CtcCsv> list3 = new List<CtcCsv>();
                Graphics graphics3 = Graphics.FromImage(bitmap3);
                for (int num54 = 0; num54 < Tempobjectlist2.Count; num54++)
                {
                    double num55 = Tempobjectlist2[num54].GlobalPosX;
                    double num56 = Tempobjectlist2[num54].GlobalPosY;
                    double num57 = num55 + (double)Tempobjectlist2[num54].Width;
                    double num58 = num56 + (double)Tempobjectlist2[num54].Height;
                    double num59 = num57 - num55;
                    double num60 = num58 - num56;
                    double num61 = num55 + num59;
                    double num62 = num56 + num60;
                    if (!(num61 <= num50) || !(num62 <= num51) || !(num55 >= num48) || !(num56 >= num49))
                    {
                        continue;
                    }
                    double num63 = num55 - num52;
                    double num64 = num56 - num53;
                    System.Drawing.Rectangle rect3 = new System.Drawing.Rectangle((int)num63, (int)num64, (int)num59, (int)num60);
                    System.Drawing.Pen pen3 = new System.Drawing.Pen(System.Drawing.Color.Red, 2f);
                    graphics3.DrawRectangle(pen3, rect3);
                    new System.Drawing.Pen(System.Drawing.Color.Black, 2f);
                    Font font3 = new Font("Arial Black", 12f, System.Drawing.FontStyle.Bold);
                    PointF point3 = new PointF((int)(num63 + num59), (int)(num64 + num60));
                    System.Drawing.Brush brush3 = new SolidBrush(System.Drawing.Color.FromArgb(255, System.Drawing.Color.Black));
                    string text2 = "";
                    if (text2 == "")
                    {
                        for (int num65 = 0; num65 < objectlist.Count; num65++)
                        {
                            if (objectlist[num65].AnnotationType == AnnotationType.Polygon || objectlist[num65].AnnotationType == AnnotationType.Line || objectlist[num65].AnnotationType == AnnotationType.Remark)
                            {
                                double num66 = objectlist[num65].CurrentStart.X * (double)SlideZoom;
                                double num67 = objectlist[num65].CurrentStart.Y * (double)SlideZoom;
                                if (num55 < num66 && num56 < num67 && num57 > num66 && num58 > num67)
                                {
                                    text2 = objectlist[num65].AnnotationDescription;
                                }
                            }
                        }
                    }
                    graphics3.DrawString(text2, font3, brush3, point3);
                    CtcCsv ctcCsv3 = new CtcCsv();
                    ctcCsv3.Left = (int)num63;
                    ctcCsv3.Top = (int)num64;
                    ctcCsv3.width = (int)num59;
                    ctcCsv3.height = (int)num60;
                    list3.Add(ctcCsv3);
                }
                graphics3.Save();
                bitmap3.Save(text + "\\" + (int)num44 + "_" + (int)num45 + "_ctc.jpg");
                bitmap3.Dispose();
                graphics3.Dispose();
                SaveDataToCSVFile(list3, text + "\\" + (int)num44 + "_" + (int)num45 + "_data.csv");
            }
            Process.Start("explorer.exe ", text);
        }

        private void Thumb_Magnifier_DragDelta(object sender, DragDeltaEventArgs e)
        {
            System.Windows.Point p = new System.Windows.Point(e.HorizontalChange, e.VerticalChange);
            System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
            System.Windows.Point point = KCommon.PointRotate(center, p, 360.0 - Rotate);
            double left = Canvas.GetLeft(Thumb_Magnifier);
            double top = Canvas.GetTop(Thumb_Magnifier);
            Canvas.SetLeft(Thumb_Magnifier, left + point.X);
            Canvas.SetTop(Thumb_Magnifier, top + point.Y);
        }

        private void LayoutBody_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MagnifierMovePoint = e.GetPosition(LayoutBody);
        }

        public BitmapImage GetCap(float _scale, int posx, int posy, int width, int height)
        {
            int nDataLength = 0;
            byte[] array = new byte[0];
            BitmapImage bitmapImage = new BitmapImage();
            try
            {
                DllImageFuc.GetImageDataRoiFunc(InfoStruct, _scale, posx, posy, width, height, out IntPtr datas, ref nDataLength, true);
                array = new byte[nDataLength];
                if (datas != IntPtr.Zero)
                {
                    Marshal.Copy(datas, array, 0, nDataLength);
                }
                DllImageFuc.DeleteImageDataFunc(datas);
                if (array.Length == 0)
                {
                    Bitmap bitmap = new Bitmap(200, 200, System.Drawing.Imaging.PixelFormat.Format64bppPArgb);
                    Graphics graphics = Graphics.FromImage(bitmap);
                    graphics.Clear(System.Drawing.Color.White);
                    graphics.Save();
                    array = Bitmap2Byte(bitmap);
                }
                MemoryStream streamSource = new MemoryStream(array);
                bitmapImage.BeginInit();
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmapImage.CacheOption = BitmapCacheOption.None;
                bitmapImage.StreamSource = streamSource;
                bitmapImage.EndInit();
                return bitmapImage;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return bitmapImage;
            }
        }

        public static byte[] Bitmap2Byte(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] array = new byte[memoryStream.Length];
                memoryStream.Seek(0L, SeekOrigin.Begin);
                memoryStream.Read(array, 0, Convert.ToInt32(memoryStream.Length));
                return array;
            }
        }

        public List<T> GetChildObjects<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject dependencyObject = null;
            List<T> list = new List<T>();
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                dependencyObject = VisualTreeHelper.GetChild(obj, i);
                if (dependencyObject is T)
                {
                    T val = (T)dependencyObject;
                    if (val.Name == name || string.IsNullOrEmpty(name))
                    {
                        list.Add((T)dependencyObject);
                    }
                }
                list.AddRange(GetChildObjects<T>(dependencyObject, ""));
            }
            return list;
        }

        private void _ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Setting.Move_Video_Speed = (int)(double.Parse(DefSpeed.ToString()) / e.NewValue);
        }

        private void DownZ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string lowLevelFileName = GetLowLevelFileName();
            if (lowLevelFileName != "")
            {
                if (InfoStruct.DataFilePTR != 0)
                {
                    _DllImageFuc.CkUnInitImageFileFunc(ref InfoStruct);
                }
                _DllImageFuc.CkInitImageFileFunc(ref InfoStruct, lowLevelFileName);
                ((MagicZoomTileSource1)msi.Source).SetinfoStruct(InfoStruct);
                LinkedListNode<int> linkedListNode = msi.ZoomableCanvas.RealizedItems.First;
                LinkedListNode<int> last = msi.ZoomableCanvas.RealizedItems.Last;
                Setting.Opacity = 1;
                while (linkedListNode != last)
                {
                    LinkedListNode<int> next = linkedListNode.Next;
                    int value = linkedListNode.Value;
                    msi.ZoomableCanvas.VirtualizeItem(value);
                    msi.ZoomableCanvas.RealizeItem(value);
                    linkedListNode = next;
                }
                if (linkedListNode == last)
                {
                    int value2 = linkedListNode.Value;
                    msi.ZoomableCanvas.VirtualizeItem(value2);
                    msi.ZoomableCanvas.RealizeItem(value2);
                }
            }
        }

        public string GetLowLevelFileName()
        {
            int num = int.Parse(x3dSlider.Zvalue.Content.ToString()) - 1;
            if (num < MinLevel)
            {
                return "";
            }
            string text = LevelFilePath;
            if (num != 0)
            {
                text = LevelFilePath.Replace(".kfb", "_" + num + ".kfb");
            }
            if (File.Exists(text))
            {
                x3dSlider.Zvalue.Content = num;
                return text;
            }
            for (int i = 2; i < 99999; i++)
            {
                text = LevelFilePath;
                int num2 = int.Parse(x3dSlider.Zvalue.Content.ToString()) - i;
                if (num2 < MinLevel)
                {
                    return "";
                }
                if (num2 != 0)
                {
                    text = LevelFilePath.Replace(".kfb", "_" + num2 + ".kfb");
                }
                if (File.Exists(text))
                {
                    x3dSlider.Zvalue.Content = num2;
                    return text;
                }
            }
            return "";
        }

        public string GetUpLevelFileName()
        {
            int num = int.Parse(x3dSlider.Zvalue.Content.ToString()) + 1;
            if (num > MaxLevel)
            {
                return "";
            }
            string text = LevelFilePath;
            if (num != 0)
            {
                text = LevelFilePath.Replace(".kfb", "_" + num + ".kfb");
            }
            if (File.Exists(text))
            {
                x3dSlider.Zvalue.Content = num;
                return text;
            }
            for (int i = 2; i < 99999; i++)
            {
                text = LevelFilePath;
                int num2 = int.Parse(x3dSlider.Zvalue.Content.ToString()) + i;
                if (num2 > MaxLevel)
                {
                    return "";
                }
                if (num2 != 0)
                {
                    text = LevelFilePath.Replace(".kfb", "_" + num2 + ".kfb");
                }
                if (File.Exists(text))
                {
                    x3dSlider.Zvalue.Content = num2;
                    return text;
                }
            }
            return "";
        }

        private void UpZ_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string upLevelFileName = GetUpLevelFileName();
            if (upLevelFileName != "")
            {
                if (InfoStruct.DataFilePTR != 0)
                {
                    _DllImageFuc.CkUnInitImageFileFunc(ref InfoStruct);
                }
                _DllImageFuc.CkInitImageFileFunc(ref InfoStruct, upLevelFileName);
                ((MagicZoomTileSource1)msi.Source).SetinfoStruct(InfoStruct);
                LinkedListNode<int> linkedListNode = msi.ZoomableCanvas.RealizedItems.First;
                LinkedListNode<int> last = msi.ZoomableCanvas.RealizedItems.Last;
                Setting.Opacity = 1;
                while (linkedListNode != last)
                {
                    LinkedListNode<int> next = linkedListNode.Next;
                    int value = linkedListNode.Value;
                    msi.ZoomableCanvas.VirtualizeItem(value);
                    msi.ZoomableCanvas.RealizeItem(value);
                    linkedListNode = next;
                }
                if (linkedListNode == last)
                {
                    int value2 = linkedListNode.Value;
                    msi.ZoomableCanvas.VirtualizeItem(value2);
                    msi.ZoomableCanvas.RealizeItem(value2);
                }
            }
        }

        private void _AnnoListWind_CloseHandler(object sender, RoutedEventArgs e)
        {
            _AnnoListWind.Visibility = Visibility.Collapsed;
            IsAnnoManageWind = Visibility.Collapsed;
        }

        public double GetMagAdjValueByCurMag(double curMagV)
        {
            double result = 0.0;
            for (int i = 0; i < 11; i++)
            {
                double num = m_magV[i + 1];
                double num2 = m_magV[i];
                double y = m_adjV[i + 1];
                double y2 = m_adjV[i];
                if (curMagV >= num && curMagV <= num2)
                {
                    result = CalcFixValueY(num, num2, y, y2, curMagV);
                    break;
                }
            }
            return result;
        }

        private double CalcFixValueY(double x1, double x2, double y1, double y2, double x)
        {
            double num = x2 - x1;
            double num2 = x2 - x;
            double num3 = y2 - y1;
            double num4 = num3 * num2 / (1.0 * num);
            return y2 - num4;
        }

        private double CalcSpeed(int timeDelta)
        {
            double num = 1.0;
            double num2 = 3.0;
            double num3 = 1.0;
            double num4 = 80.0;
            if (timeDelta <= 0)
            {
                num = num2;
            }
            else if ((double)timeDelta >= num4)
            {
                num = num3;
            }
            else
            {
                num = (num4 - (double)timeDelta) * (num2 - num3) / num4 + num3;
                if (num < num3)
                {
                    num = num3;
                }
                if (num > num2)
                {
                    num = num2;
                }
            }
            return num;
        }

        private void Menu_Move_Click(object sender, RoutedEventArgs e)
        {
            if (RuleThumb.Visibility == Visibility.Visible)
            {
                RuleThumb.Visibility = Visibility.Collapsed;
            }
            else
            {
                RuleThumb.Visibility = Visibility.Visible;
            }
        }

        private void _RuleColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color> e)
        {
            RLine_L.Stroke = new SolidColorBrush(e.NewValue);
            RLine_0.Stroke = new SolidColorBrush(e.NewValue);
            RLine_1.Stroke = new SolidColorBrush(e.NewValue);
            RLine_2.Stroke = new SolidColorBrush(e.NewValue);
            RLine_3.Stroke = new SolidColorBrush(e.NewValue);
            RLine_4.Stroke = new SolidColorBrush(e.NewValue);
            RLine_5.Stroke = new SolidColorBrush(e.NewValue);
            Rtxt_0.Foreground = new SolidColorBrush(e.NewValue);
            Rtxt_1.Foreground = new SolidColorBrush(e.NewValue);
            Rtxt_2.Foreground = new SolidColorBrush(e.NewValue);
            Rtxt_3.Foreground = new SolidColorBrush(e.NewValue);
            Rtxt_4.Foreground = new SolidColorBrush(e.NewValue);
            Rtxt_5.Foreground = new SolidColorBrush(e.NewValue);
        }

        private void AddLog(string line)
        {
            string str = "Log.txt";
            try
            {
                FileStream fileStream = new FileStream("D:\\Li_ScanData\\" + str, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.BaseStream.Seek(0L, SeekOrigin.End);
                streamWriter.WriteLine(line);
                streamWriter.Flush();
                streamWriter.Close();
                fileStream.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void bk_Scale_MouseEnter(object sender, RoutedEventArgs e)
        {
            IsArcMenu(Visibility.Visible);
        }

        public void MouseTouchDevice_TMove(object sender, RoutedEventArgs e)
        {
        }

        public void TmoveSetOffset(Mainpage m, System.Windows.Point p)
        {
            System.Windows.Point offset = msi.ZoomableCanvas.Offset;
            System.Windows.Point p2 = new System.Windows.Point(p.X, p.Y);
            System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
            System.Windows.Point point = KCommon.PointRotate(center, p2, Rotate);
            offset.Y += point.Y;
            offset.X += point.X;
            m.msi.ZoomableCanvas.Offset = new System.Windows.Point(offset.X, offset.Y);
            m.msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
        }

        public void TmoveSetOffset2(Mainpage m, System.Windows.Point p, System.Windows.Point p1)
        {
            m.msi.ZoomableCanvas.Offset = new System.Windows.Point(m.msi.ZoomableCanvas.Offset.X + p.X, m.msi.ZoomableCanvas.Offset.Y + p.Y);
            m.msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
        }

        public void TmoveSetOffset3(Mainpage m, System.Windows.Point p)
        {
            m.msi.ZoomableCanvas.Offset = new System.Windows.Point(m.msi.ZoomableCanvas.Offset.X + p.X, m.msi.ZoomableCanvas.Offset.Y + p.Y);
            m.msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
        }

        public void TmoveSetOffset4(Mainpage m, System.Windows.Point p, double ARotate)
        {
            System.Windows.Point offset = msi.ZoomableCanvas.Offset;
            System.Windows.Point p2 = new System.Windows.Point(p.X, p.Y);
            System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
            System.Windows.Point point = KCommon.PointRotate(center, p2, Rotate - ARotate);
            offset.Y += point.Y;
            offset.X += point.X;
            m.msi.ZoomableCanvas.Offset = new System.Windows.Point(offset.X, offset.Y);
            m.msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
        }

        private void UnLoadHandle()
        {
            RotateTransform renderTransform = new RotateTransform(0.0);
            _RotateViewer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            _RotateViewer.Btn_AngleLine.RenderTransform = renderTransform;
            _RotateViewer.lbl_Angle.Content = 0 + "°";
            System.Windows.Point center = new System.Windows.Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            System.Windows.Point p = new System.Windows.Point(x, y);
            System.Windows.Point point = KCommon.PointRotate(center, p, 360.0);
            Canvas.SetLeft(_RotateViewer.ThumbAngle, point.X - 7.5);
            Canvas.SetTop(_RotateViewer.ThumbAngle, point.Y - 7.5);
            MsiRotate(0.0);
            IsDw = false;
            UnRegisterMsiEvents();
            if (_myArrowLine != null)
            {
                _myArrowLine.unload();
            }
            if (_myEllipse != null)
            {
                _myEllipse.unload();
            }
            if (_myRectangle != null)
            {
                _myRectangle.unload();
            }
            if (_myPin != null)
            {
                _myPin.unload();
            }
            if (_myPolyline != null)
            {
                _myPolyline.unload();
            }
            if (_myLine != null)
            {
                _myLine.unload();
            }
            if (_myDiyCtcRectangle != null)
            {
                _myDiyCtcRectangle.unload();
            }
            if (_myTmaRectangle != null)
            {
                _myTmaRectangle.unload();
            }
            _myArrowLine = null;
            _myEllipse = null;
            _myRectangle = null;
            _myPin = null;
            _myPolyline = null;
            _myLine = null;
            _myDiyCtcRectangle = null;
            nav.IsHitTestVisible = false;
            foreach (AnnotationBase item in objectlist)
            {
                item.IsActive(Visibility.Collapsed);
            }
        }

        public void IniFile(string fileName)
        {
            InfoStruct.DataFilePTR = 0;
            _DllImageFuc.CkInitImageFileFunc(ref InfoStruct, fileName);
        }

        private void CtcWind_DeleteRefresh(object sender, RoutedEventArgs e)
        {
            if (DiyCtcRectangleDic.ContainsKey(int.Parse(sender.ToString())))
            {
                DiyCtcRectangleDic[int.Parse(sender.ToString())].DeleteItem();
                _AnnoListWind.cbo_mc.SelectedIndex = -1;
            }
            Refresh();
        }

        private void DgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (msi.ZoomableCanvas != null)
                {
                    if (SelecresultUser != null)
                    {
                        SelecresultUser.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, 0));
                    }
                    CtcVo ctcVo = (CtcVo)CtcWind.DgList.SelectedItem;
                    if (ctcVo != null)
                    {
                        double scale = msi.ZoomableCanvas.Scale;
                        int level = msi.Source.GetLevel(SlideZoom / SlideZoom);
                        int currentLevel = msi._spatialSource.CurrentLevel;
                        if (level != currentLevel)
                        {
                            msi._spatialSource.CurrentLevel = level;
                        }
                        if (scale != (double)(SlideZoom / SlideZoom))
                        {
                            double num = 4.0;
                            TimeSpan timeSpan = TimeSpan.FromMilliseconds(num * 100.0);
                            CubicEase easingFunction = new CubicEase();
                            msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(SlideZoom / SlideZoom, timeSpan)
                            {
                                EasingFunction = easingFunction
                            }, HandoffBehavior.Compose);
                            msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new System.Windows.Point((double)ctcVo.GlobalPosX - msi.ActualWidth / 2.0 + (double)(ctcVo.Width / 2), (double)ctcVo.GlobalPosY - msi.ActualHeight / 2.0 + (double)(ctcVo.Height / 2)), timeSpan)
                            {
                                EasingFunction = easingFunction
                            }, HandoffBehavior.Compose);
                        }
                        else
                        {
                            msi.ZoomableCanvas.Offset = new System.Windows.Point((double)ctcVo.GlobalPosX - msi.ActualWidth / 2.0 + (double)(ctcVo.Width / 2), (double)ctcVo.GlobalPosY - msi.ActualHeight / 2.0 + (double)(ctcVo.Height / 2));
                            msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void SingleReaseSlide()
        {
            SaveAnoXmlFile();
            if (Gama_Personal == 0.0)
            {
                SaveImageProcess();
            }
            _AnnoListWind.Close();
            _AnnoWind.Close();
            _AdjustmentWind.Close();
            if (InfoStruct.DataFilePTR > 0)
            {
                _DllImageFuc.CkUnInitImageFileFunc(ref InfoStruct);
                InfoStruct.DataFilePTR = 0;
            }
            msi.ZoomableCanvas = null;
            ifile.IniWriteValue("Language", "Type", Setting.Language);
            KCommon.FlushMemory();
        }

        public void ReaseSlide()
        {
            SaveAnoXmlFile();
            if (Gama_Personal == 0.0)
            {
                SaveImageProcess();
            }
            _AnnoListWind.Close();
            _AnnoWind.Close();
            _AdjustmentWind.Close();
            ifile.IniWriteValue("Language", "Value", Setting.Language);
            if (InfoStruct.DataFilePTR > 0)
            {
                _DllImageFuc.CkUnInitImageFileFunc(ref InfoStruct);
                InfoStruct.DataFilePTR = 0;
            }
            msi.ZoomableCanvas = null;
        }

        public void KeyValue(System.Windows.Input.KeyEventArgs e)
        {
            IsDw = false;
            _ = _AnnoListWind.cbo_mc.SelectedIndex;
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            switch (e.Key)
            {
                case Key.RightCtrl:
                    Setting.isCtrl = 1;
                    break;
                case Key.LeftCtrl:
                    Setting.isCtrl = 1;
                    break;
                case Key.OemPlus:
                    {
                        double curscale = Curscale;
                        curscale *= 2.4000000953674316;
                        ZoomRatio(curscale);
                        num = curscale;
                        break;
                    }
                case Key.OemMinus:
                    {
                        double curscale2 = Curscale;
                        curscale2 /= 2.4000000953674316;
                        ZoomRatio(curscale2);
                        num = curscale2;
                        break;
                    }
                case Key.Delete:
                    Delete();
                    break;
                case Key.D1:
                    ZoomRatio(1.0);
                    num = 1.0;
                    break;
                case Key.NumPad1:
                    ZoomRatio(1.0);
                    num = 1.0;
                    break;
                case Key.D2:
                    ZoomRatio(2.0);
                    num = 2.0;
                    break;
                case Key.NumPad2:
                    ZoomRatio(2.0);
                    num = 2.0;
                    break;
                case Key.D3:
                    ZoomRatio(4.0);
                    num = 4.0;
                    break;
                case Key.NumPad3:
                    ZoomRatio(4.0);
                    num = 4.0;
                    break;
                case Key.D4:
                    ZoomRatio(10.0);
                    num = 10.0;
                    break;
                case Key.NumPad4:
                    ZoomRatio(10.0);
                    num = 10.0;
                    break;
                case Key.D5:
                    ZoomRatio(20.0);
                    num = 20.0;
                    break;
                case Key.NumPad5:
                    ZoomRatio(20.0);
                    num = 20.0;
                    break;
                case Key.D6:
                    ZoomRatio(40.0);
                    num = 40.0;
                    break;
                case Key.NumPad6:
                    ZoomRatio(40.0);
                    num = 40.0;
                    break;
                case Key.D7:
                    ZoomRatio(80.0);
                    num = 80.0;
                    break;
                case Key.NumPad7:
                    ZoomRatio(80.0);
                    num = 80.0;
                    break;
                case Key.D8:
                    ZoomRatio(160.0);
                    num = 160.0;
                    break;
                case Key.NumPad8:
                    ZoomRatio(160.0);
                    num = 160.0;
                    break;
                case Key.D0:
                    ZoomRatio(fitratio);
                    num = fitratio;
                    break;
                case Key.NumPad0:
                    ZoomRatio(fitratio);
                    num = fitratio;
                    break;
                case Key.Add:
                    {
                        double curscale4 = Curscale;
                        curscale4 *= 2.4000000953674316;
                        ZoomRatio(curscale4);
                        num = curscale4;
                        break;
                    }
                case Key.Subtract:
                    {
                        double curscale3 = Curscale;
                        curscale3 /= 2.4000000953674316;
                        ZoomRatio(curscale3);
                        num = curscale3;
                        break;
                    }
                case Key.Up:
                    {
                        System.Windows.Point offset4 = msi.ZoomableCanvas.Offset;
                        System.Windows.Point p4 = new System.Windows.Point(0.0, 0.0 - KeyStep);
                        System.Windows.Point center4 = new System.Windows.Point(0.0, 0.0);
                        System.Windows.Point point5 = KCommon.PointRotate(center4, p4, Rotate);
                        offset4.Y -= point5.Y;
                        offset4.X -= point5.X;
                        msi.ZoomableCanvas.Offset = offset4;
                        msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                        num3 = 0.0 - KeyStep;
                        break;
                    }
                case Key.Down:
                    {
                        System.Windows.Point offset3 = msi.ZoomableCanvas.Offset;
                        System.Windows.Point p3 = new System.Windows.Point(0.0, KeyStep);
                        System.Windows.Point center3 = new System.Windows.Point(0.0, 0.0);
                        System.Windows.Point point4 = KCommon.PointRotate(center3, p3, Rotate);
                        offset3.Y -= point4.Y;
                        offset3.X -= point4.X;
                        msi.ZoomableCanvas.Offset = offset3;
                        msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                        num3 = KeyStep;
                        break;
                    }
                case Key.Left:
                    {
                        System.Windows.Point offset2 = msi.ZoomableCanvas.Offset;
                        System.Windows.Point p2 = new System.Windows.Point(0.0 - KeyStep, 0.0);
                        System.Windows.Point center2 = new System.Windows.Point(0.0, 0.0);
                        System.Windows.Point point3 = KCommon.PointRotate(center2, p2, Rotate);
                        offset2.Y -= point3.Y;
                        offset2.X -= point3.X;
                        msi.ZoomableCanvas.Offset = offset2;
                        msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                        num2 = 0.0 - KeyStep;
                        break;
                    }
                case Key.Right:
                    {
                        System.Windows.Point offset = msi.ZoomableCanvas.Offset;
                        System.Windows.Point p = new System.Windows.Point(KeyStep, 0.0);
                        System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
                        System.Windows.Point point2 = KCommon.PointRotate(center, p, Rotate);
                        offset.Y -= point2.Y;
                        offset.X -= point2.X;
                        msi.ZoomableCanvas.Offset = offset;
                        msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                        num2 = KeyStep;
                        break;
                    }
                case Key.Space:
                    {
                        Fit();
                        System.Windows.Point point = new System.Windows.Point(fitx, fity);
                        double scale = msi.ZoomableCanvas.Scale;
                        int level = msi.Source.GetLevel(fitratio / (double)SlideZoom);
                        int currentLevel = msi._spatialSource.CurrentLevel;
                        if (level != currentLevel)
                        {
                            msi._spatialSource.CurrentLevel = level;
                        }
                        if (scale != fitratio / (double)SlideZoom)
                        {
                            double num4 = 4.0;
                            TimeSpan timeSpan = TimeSpan.FromMilliseconds(num4 * 100.0);
                            CubicEase easingFunction = new CubicEase();
                            msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(fitratio / (double)SlideZoom, timeSpan)
                            {
                                EasingFunction = easingFunction
                            }, HandoffBehavior.Compose);
                            msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new System.Windows.Point(point.X, point.Y), timeSpan)
                            {
                                EasingFunction = easingFunction
                            }, HandoffBehavior.Compose);
                        }
                        else
                        {
                            msi.ZoomableCanvas.Offset = new System.Windows.Point(point.X, point.Y);
                            msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                        }
                        if (Setting.IsSynchronous)
                        {
                            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                            {
                                if ((Mainpage)item.Value != this)
                                {
                                    ((Mainpage)item.Value).SynFit();
                                }
                            }
                        }
                        break;
                    }
            }
            if (Setting.IsSynchronous)
            {
                foreach (KeyValuePair<object, object> item2 in Setting.TabsDic)
                {
                    if ((Mainpage)item2.Value != this)
                    {
                        if (num2 != 0.0 || num3 != 0.0)
                        {
                            ((Mainpage)item2.Value).TmoveSetOffset((Mainpage)item2.Value, new System.Windows.Point(0.0 - num2, 0.0 - num3));
                        }
                        if (num != 0.0)
                        {
                            ((Mainpage)item2.Value).ZoomRatio(num);
                        }
                    }
                }
            }
        }

        public BitmapImage GetLable(int KfbioAddress)
        {
            IMAGE_INFO_STRUCT k = default(IMAGE_INFO_STRUCT);
            k.DataFilePTR = KfbioAddress;
            IntPtr datas = IntPtr.Zero;
            int b = 0;
            int c = 0;
            int a = 0;
            _DllImageFuc.CkGetLableInfoFunc(k, out datas, ref a, ref b, ref c);
            byte[] array = new byte[a];
            if (datas != IntPtr.Zero)
            {
                Marshal.Copy(datas, array, 0, a);
            }
            _DllImageFuc.CkDeleteImageDataFunc(datas);
            BitmapImage bitmapImage = new BitmapImage();
            MemoryStream streamSource = new MemoryStream(array);
            bitmapImage.BeginInit();
            bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            bitmapImage.CacheOption = BitmapCacheOption.None;
            bitmapImage.StreamSource = streamSource;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

        public Bitmap LoadImage2(int Level, int posx, int posy)
        {
            try
            {
                IMAGE_INFO_STRUCT k = default(IMAGE_INFO_STRUCT);
                k.DataFilePTR = InfoStruct.DataFilePTR;
                int nDataLength = 0;
                int num = Math.Max((int)ImageW, (int)ImageH);
                int num2 = IsInteger((Math.Log(num) / Math.Log(2.0)).ToString()) ? ((int)(Math.Log(num) / Math.Log(2.0))) : ((int)(Math.Log(num) / Math.Log(2.0)) + 1);
                float fScale = (num2 != Level) ? ((float)SlideZoom / (float)Math.Pow(2.0, num2 - Level)) : ((float)SlideZoom);
                _DllImageFuc.CkGetImageStreamFunc(ref k, fScale, posx * TileSize, posy * TileSize, ref nDataLength, out IntPtr datas);
                byte[] array = new byte[nDataLength];
                if (datas != IntPtr.Zero)
                {
                    Marshal.Copy(datas, array, 0, nDataLength);
                }
                MemoryStream stream = new MemoryStream(array);
                return new Bitmap(stream);
            }
            catch (FileNotFoundException)
            {
            }
            catch (FileFormatException)
            {
            }
            return null;
        }

        public BitmapImage LoadImage(int KfbioAddress, int Level, int posx, int posy)
        {
            try
            {
                IMAGE_INFO_STRUCT k = default(IMAGE_INFO_STRUCT);
                k.DataFilePTR = KfbioAddress;
                int nDataLength = 0;
                int num = Math.Max((int)ImageW, (int)ImageH);
                int num2 = IsInteger((Math.Log(num) / Math.Log(2.0)).ToString()) ? ((int)(Math.Log(num) / Math.Log(2.0))) : ((int)(Math.Log(num) / Math.Log(2.0)) + 1);
                float fScale = (num2 != Level) ? ((float)SlideZoom / (float)Math.Pow(2.0, num2 - Level)) : ((float)SlideZoom);
                _DllImageFuc.CkGetImageStreamFunc(ref k, fScale, posx * TileSize, posy * TileSize, ref nDataLength, out IntPtr datas);
                byte[] array = new byte[nDataLength];
                if (datas != IntPtr.Zero)
                {
                    Marshal.Copy(datas, array, 0, nDataLength);
                }
                BitmapImage bitmapImage = new BitmapImage();
                if (array.Length == 0)
                {
                    return new BitmapImage(new Uri("images/pp.png", UriKind.RelativeOrAbsolute));
                }
                try
                {
                    MemoryStream streamSource = new MemoryStream(array);
                    bitmapImage.BeginInit();
                    bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bitmapImage.CacheOption = BitmapCacheOption.None;
                    bitmapImage.StreamSource = streamSource;
                    bitmapImage.EndInit();
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
                return bitmapImage;
            }
            catch (FileNotFoundException)
            {
            }
            catch (FileFormatException)
            {
            }
            return null;
        }

        public void SaveImageProcess()
        {
            ifile.IniWriteValue("Gama", "Value", _AdjustmentWind.GamaSlider.Value.ToString().Trim());
            ifile.IniWriteValue("B", "Value", _AdjustmentWind.BSlider.Value.ToString().Trim());
            ifile.IniWriteValue("G", "Value", _AdjustmentWind.GSlider.Value.ToString().Trim());
            ifile.IniWriteValue("R", "Value", _AdjustmentWind.RSlider.Value.ToString().Trim());
            ifile.IniWriteValue("Bright", "Value", _AdjustmentWind.BrightSlider.Value.ToString().Trim());
            ifile.IniWriteValue("Contrast", "Value", _AdjustmentWind.ContrastSilder.Value.ToString().Trim());
        }

        private void PersonSetRadio_Click(object sender, RoutedEventArgs e)
        {
            _AdjustmentWind.TitleArea.Style = (System.Windows.Application.Current.Resources["DialogTitleBorderStyle2"] as Style);
            if (Gama_Personal != 0.0)
            {
                _AdjustmentWind.GamaSlider.Value = Gama_Personal;
                _AdjustmentWind.RSlider.Value = R_Personal;
                _AdjustmentWind.GSlider.Value = G_Personal;
                _AdjustmentWind.BSlider.Value = B_Personal;
                _AdjustmentWind.BrightSlider.Value = Bright_Personal;
                _AdjustmentWind.ContrastSilder.Value = Contrast_Personal;
            }
        }

        private void SysSetRadio_Click(object sender, RoutedEventArgs e)
        {
            _AdjustmentWind.TitleArea.Style = (System.Windows.Application.Current.Resources["DialogTitleBorderStyle"] as Style);
            _AdjustmentWind.GamaSlider.Value = double.Parse(ifile.IniReadValue("Gama", "Value"));
            _AdjustmentWind.BSlider.Value = double.Parse(ifile.IniReadValue("B", "Value"));
            _AdjustmentWind.RSlider.Value = double.Parse(ifile.IniReadValue("R", "Value"));
            _AdjustmentWind.GSlider.Value = double.Parse(ifile.IniReadValue("G", "Value"));
            _AdjustmentWind.BrightSlider.Value = double.Parse(ifile.IniReadValue("Bright", "Value"));
            _AdjustmentWind.ContrastSilder.Value = double.Parse(ifile.IniReadValue("Contrast", "Value"));
        }

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            _AdjustmentWind.Hide();
            IsImageAdjustWind = Visibility.Collapsed;
            if (_AdjustmentWind.PersonSetRadio.IsChecked == true)
            {
                Gama_Personal = _AdjustmentWind.GamaSlider.Value;
                R_Personal = _AdjustmentWind.RSlider.Value;
                G_Personal = _AdjustmentWind.GSlider.Value;
                B_Personal = _AdjustmentWind.BSlider.Value;
                Bright_Personal = _AdjustmentWind.BrightSlider.Value;
                Contrast_Personal = _AdjustmentWind.ContrastSilder.Value;
                isPerson = true;
                SaveAnoXmlFile2();
            }
            else
            {
                isPerson = false;
                Gama_Personal = 0.0;
                R_Personal = 0.0;
                G_Personal = 0.0;
                B_Personal = 0.0;
                Bright_Personal = 0.0;
                Contrast_Personal = 0.0;
                SaveAnoXmlFile2();
                SaveImageProcess();
                foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                {
                    if ((Mainpage)item.Value != this)
                    {
                        ((Mainpage)item.Value).SetImageProcess();
                    }
                }
            }
        }

        public void SetImageProcess()
        {
            if (!isPerson)
            {
                _AdjustmentWind.GamaSlider.Value = double.Parse(ifile.IniReadValue("Gama", "Value"));
                _AdjustmentWind.BSlider.Value = double.Parse(ifile.IniReadValue("B", "Value"));
                _AdjustmentWind.RSlider.Value = double.Parse(ifile.IniReadValue("R", "Value"));
                _AdjustmentWind.GSlider.Value = double.Parse(ifile.IniReadValue("G", "Value"));
                _AdjustmentWind.BrightSlider.Value = double.Parse(ifile.IniReadValue("Bright", "Value"));
                _AdjustmentWind.ContrastSilder.Value = double.Parse(ifile.IniReadValue("Contrast", "Value"));
            }
        }

        private void cancle_Click(object sender, RoutedEventArgs e)
        {
            _AdjustmentWind.GamaSlider.Value = Gama;
            _AdjustmentWind.RSlider.Value = R;
            _AdjustmentWind.GSlider.Value = G;
            _AdjustmentWind.BSlider.Value = B;
            _AdjustmentWind.Sharpen.Value = Sharpen;
            _AdjustmentWind.BrightSlider.Value = Bright;
            _AdjustmentWind.ContrastSilder.Value = Contrast;
            _AdjustmentWind.Hide();
            IsImageAdjustWind = Visibility.Collapsed;
        }

        private void Show_AdjustmentWind(object sender, RoutedEventArgs e)
        {
            Gama = _AdjustmentWind.GamaSlider.Value;
            R = _AdjustmentWind.RSlider.Value;
            G = _AdjustmentWind.GSlider.Value;
            B = _AdjustmentWind.BSlider.Value;
            Sharpen = _AdjustmentWind.Sharpen.Value;
            Bright = _AdjustmentWind.BrightSlider.Value;
            Contrast = _AdjustmentWind.ContrastSilder.Value;
            System.Windows.Point mainWindowPoint = GetMainWindowPoint();
            _AdjustmentWind.Top = mainWindowPoint.Y + (base.ActualHeight - _AdjustmentWind.Height) / 2.0;
            _AdjustmentWind.Left = mainWindowPoint.X + (base.ActualWidth - _AdjustmentWind.Width) / 2.0;
            if (isPerson)
            {
                _AdjustmentWind.TitleArea.Style = (System.Windows.Application.Current.Resources["DialogTitleBorderStyle2"] as Style);
                _AdjustmentWind.PersonSetRadio.IsChecked = true;
            }
            else
            {
                _AdjustmentWind.TitleArea.Style = (System.Windows.Application.Current.Resources["DialogTitleBorderStyle"] as Style);
                _AdjustmentWind.SysSetRadio.IsChecked = true;
            }
            _AdjustmentWind.Show();
            _AdjustmentWind.Activate();
            IsImageAdjustWind = Visibility.Visible;
        }

        public System.Windows.Point GetMainWindowPoint()
        {
            System.Windows.Point result = new System.Windows.Point(0.0, 0.0);
            try
            {
                result = TransformToAncestor(System.Windows.Application.Current.MainWindow).Transform(new System.Windows.Point(0.0, 0.0));
            }
            catch
            {
                foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                {
                    if (item.Value == this)
                    {
                        result = new System.Windows.Point(((LayoutDocument)item.Key).FloatingLeft, ((LayoutDocument)item.Key).FloatingTop);
                    }
                }
            }
            result.Y += System.Windows.Application.Current.MainWindow.Top;
            result.X += System.Windows.Application.Current.MainWindow.Left;
            return result;
        }

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            _AdjustmentWind.RSlider.Value = R_Default;
            _AdjustmentWind.GSlider.Value = G_Default;
            _AdjustmentWind.BSlider.Value = B_Default;
            _AdjustmentWind.GamaSlider.Value = Gama_Default;
            _AdjustmentWind.ContrastSilder.Value = Contrast_Default;
            _AdjustmentWind.BrightSlider.Value = Bright_Default;
        }

        private void Compare_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Gama_Compare = _AdjustmentWind.GamaSlider.Value;
            R_Compare = _AdjustmentWind.RSlider.Value;
            G_Compare = _AdjustmentWind.GSlider.Value;
            B_Compare = _AdjustmentWind.BSlider.Value;
            Sharpen_Compare = _AdjustmentWind.Sharpen.Value;
            Bright_Compare = _AdjustmentWind.BrightSlider.Value;
            Contrast_Compare = _AdjustmentWind.ContrastSilder.Value;
            _AdjustmentWind.GamaSlider.Value = Gama;
            _AdjustmentWind.RSlider.Value = R;
            _AdjustmentWind.GSlider.Value = G;
            _AdjustmentWind.BSlider.Value = B;
            _AdjustmentWind.Sharpen.Value = Sharpen;
            _AdjustmentWind.BrightSlider.Value = Bright;
            _AdjustmentWind.ContrastSilder.Value = Contrast;
        }

        private void Compare_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            _AdjustmentWind.GamaSlider.Value = Gama_Compare;
            _AdjustmentWind.RSlider.Value = R_Compare;
            _AdjustmentWind.GSlider.Value = G_Compare;
            _AdjustmentWind.BSlider.Value = B_Compare;
            _AdjustmentWind.Sharpen.Value = Sharpen_Compare;
            _AdjustmentWind.BrightSlider.Value = Bright_Compare;
            _AdjustmentWind.ContrastSilder.Value = Contrast_Compare;
        }

        private void GamaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Imgfiter.Gama = e.NewValue;
            _AdjustmentWind.txt_GamaSlider.Text = Math.Round(e.NewValue, 2).ToString();
        }

        private void RSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Imgfiter.R = (e.NewValue - 100.0) / 100.0;
            _AdjustmentWind.txt_RSlider.Text = Math.Round(e.NewValue, 0).ToString() + "%";
        }

        private void GSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Imgfiter.G = (e.NewValue - 100.0) / 100.0;
            _AdjustmentWind.txt_GSlider.Text = Math.Round(e.NewValue, 0).ToString() + "%";
        }

        private void BSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Imgfiter.B = (e.NewValue - 100.0) / 100.0;
            _AdjustmentWind.txt_BSlider.Text = Math.Round(e.NewValue, 0).ToString() + "%";
        }

        private void Sharpen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void ContrastSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Imgfiter.Contrast = e.NewValue / 100.0;
            _AdjustmentWind.txt_ContrastSlider.Text = Math.Round(e.NewValue, 0).ToString() + "%";
        }

        private void BrightSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Imgfiter.Brightness = (e.NewValue - 100.0) / 100.0;
            _AdjustmentWind.txt_BrightSlider.Text = Math.Round(e.NewValue, 0).ToString() + "%";
        }

        private void LoadDiyCtcRectangle(object sender, RoutedEventArgs e)
        {
            UnLoadHandle();
            _myDiyCtcRectangle = new myDiyCtcRectangle(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
            _myDiyCtcRectangle.FinishEvent += FinishEvent;
            SetAnnoRadioButton(false);
        }

        private void FinishEvent2(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RegisterMsiEvents();
            myDiyCtcRectangle myDiyCtcRectangle = (myDiyCtcRectangle)sender;
            System.Windows.Point originStart = myDiyCtcRectangle.OriginStart;
            double num = msi.ZoomableCanvas.Offset.X + originStart.X;
            double num2 = msi.ZoomableCanvas.Offset.Y + originStart.Y;
            double num3 = num * (double)SlideZoom / Curscale;
            double num4 = num2 * (double)SlideZoom / Curscale;
            double num5 = myDiyCtcRectangle.m_rectangle.Width * (double)SlideZoom / Curscale;
            double num6 = myDiyCtcRectangle.m_rectangle.Height * (double)SlideZoom / Curscale;
            double size = Math.Sqrt(num5 * num5 + num6 * num6) * Calibration;
            int key = CtcWind.AddData((int)num3, (int)num4, (int)num5, (int)num6, size);
            DiyCtcRectangleDic.Add(key, myDiyCtcRectangle);
            nav.IsHitTestVisible = true;
        }

        private void LoadRectangle(object sender, RoutedEventArgs e)
        {
            UnLoadHandle();
            _myRectangle = new myRectangle(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
            _myRectangle.FinishEvent += FinishEvent;
            SetAnnoRadioButton(false);
        }

        private void LoadEllipse(object sender, RoutedEventArgs e)
        {
            UnLoadHandle();
            _myEllipse = new myEllipse(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
            _myEllipse.FinishEvent += FinishEvent;
            SetAnnoRadioButton(false);
        }

        private void LoadLine(object sender, RoutedEventArgs e)
        {
            UnLoadHandle();
            _myLine = new myLine(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
            _myLine.FinishEvent += FinishEvent;
            SetAnnoRadioButton(false);
        }

        private void LoadArrowLine(object sender, RoutedEventArgs e)
        {
            UnLoadHandle();
            _myArrowLine = new myArrowLine(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
            _myArrowLine.FinishEvent += FinishEvent;
            SetAnnoRadioButton(false);
        }

        private void LoadPolyline(object sender, RoutedEventArgs e)
        {
            UnLoadHandle();
            _myPolyline = new myPolyline(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
            _myPolyline.FinishEvent += FinishEvent;
            SetAnnoRadioButton(false);
        }

        private void LoadPin(object sender, RoutedEventArgs e)
        {
            UnLoadHandle();
            _myPin = new myPin(alc, canvasboard, msi, SlideZoom, objectlist);
            _myPin.FinishEvent += FinishEvent;
            SetAnnoRadioButton(true);
        }

        private void _SaveAnno_Click(object sender, RoutedEventArgs e)
        {
            _AnnoWind._AnnotationBase.Size = int.Parse((_AnnoWind.LineWidthComboBox.SelectedItem as ComboBoxItem).Content.ToString());
            _AnnoWind._AnnotationBase.AnnotationName = _AnnoWind.AnnoName.Text;
            _AnnoWind._AnnotationBase.AnnotationDescription = _AnnoWind.AnnoDes.Text;
            _AnnoWind._AnnotationBase.BorderBrush = new SolidColorBrush(_AnnoWind._colorPicker.SelectedColor);
            _AnnoWind._AnnotationBase.isVisble = ((_AnnoWind.ShowInfo.IsChecked != true) ? Visibility.Collapsed : Visibility.Visible);
            _AnnoWind._AnnotationBase.isMsVisble = ((_AnnoWind.ShowMs.IsChecked == true) ? true : false);
            if (_AnnoWind._AnnotationBase.GetType() == typeof(myPin))
            {
                if (_AnnoWind.Rad_1.IsChecked == true)
                {
                    _AnnoWind._AnnotationBase.PinType = "images/pin_1.png";
                }
                else if (_AnnoWind.Rad_2.IsChecked == true)
                {
                    _AnnoWind._AnnotationBase.PinType = "images/pin_2.png";
                }
                else if (_AnnoWind.Rad_3.IsChecked == true)
                {
                    _AnnoWind._AnnotationBase.PinType = "images/pin_3.png";
                }
                else if (_AnnoWind.Rad_4.IsChecked == true)
                {
                    _AnnoWind._AnnotationBase.PinType = "images/pin_4.png";
                }
                _AnnoWind._AnnotationBase.FontColor = new SolidColorBrush(_AnnoWind._colorPicker.SelectedColor);
            }
            if (_AnnoWind._AnnotationBase.AnnotationType == AnnotationType.DiyCtcRectangle)
            {
                myDiyCtcRectangle myDiyCtcRectangle = (myDiyCtcRectangle)_AnnoWind._AnnotationBase;
                System.Windows.Point originStart = myDiyCtcRectangle.OriginStart;
                double num = msi.ZoomableCanvas.Offset.X + originStart.X;
                double num2 = msi.ZoomableCanvas.Offset.Y + originStart.Y;
                double num3 = num * (double)SlideZoom / Curscale;
                double num4 = num2 * (double)SlideZoom / Curscale;
                double num5 = myDiyCtcRectangle.m_rectangle.Width * (double)SlideZoom / Curscale;
                double num6 = myDiyCtcRectangle.m_rectangle.Height * (double)SlideZoom / Curscale;
                double size = Math.Sqrt(num5 * num5 + num6 * num6) * Calibration;
                int num7 = CtcWind.AddData((int)num3, (int)num4, (int)num5, (int)num6, size);
                DiyCtcRectangleDic.Add(num7, myDiyCtcRectangle);
                _AnnoWind._AnnotationBase.FontSize = num7;
                nav.IsHitTestVisible = true;
            }
            _AnnoWind._AnnotationBase.UpadteTextBlock();
            _AnnoWind._AnnotationBase.UpdateVisual();
            _AnnoWind.Hide();
        }

        private void _CancelAnno_Click(object sender, RoutedEventArgs e)
        {
            _AnnoWind._AnnotationBase.DeleteItem();
            _AnnoListWind.cbo_mc.SelectedIndex = -1;
            _AnnoWind.Hide();
        }

        private void FinishEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RegisterMsiEvents();
            _AnnoWind._AnnotationBase = (AnnotationBase)sender;
            if (_AnnoWind._AnnotationBase.OriginStart == _AnnoWind._AnnotationBase.OriginEnd && _AnnoWind._AnnotationBase.AnnotationType != AnnotationType.Remark)
            {
                _AnnoWind._AnnotationBase.DeleteItem();
                _AnnoWind._AnnotationBase.AnnoControl.CB.SelectedIndex = -1;
            }
            else
            {
                _AnnoWind.AnnoName.Text = _AnnoWind._AnnotationBase.AnnotationName;
                _AnnoWind.AnnoDes.Text = string.Empty;
                _AnnoWind.tbk_info.Text = _AnnoWind._AnnotationBase.CalcMeasureInfo();
                _AnnoWind._colorPicker.SelectedColor = System.Windows.Media.Color.FromArgb(byte.MaxValue, 0, 0, byte.MaxValue);
                _AnnoWind.LineWidthComboBox.SelectedIndex = 1;
                _AnnoWind.Rad_1.IsChecked = true;
                System.Windows.Point mainWindowPoint = GetMainWindowPoint();
                _AnnoWind.Top = mainWindowPoint.Y + (base.ActualHeight - _AnnoWind.Height) / 2.0;
                _AnnoWind.Left = mainWindowPoint.X + (base.ActualWidth - _AnnoWind.Width) / 2.0;
                _AnnoWind.ShowDialog();
            }
            nav.IsHitTestVisible = true;
        }

        public void SetAnnoRadioButton(bool v)
        {
            _AnnoWind.Rad_1.IsEnabled = v;
            _AnnoWind.Rad_2.IsEnabled = v;
            _AnnoWind.Rad_3.IsEnabled = v;
            _AnnoWind.Rad_4.IsEnabled = v;
        }

        public void SetAnnoListRadioButton(bool v)
        {
            _AnnoListWind.Rad_1.IsEnabled = v;
            _AnnoListWind.Rad_2.IsEnabled = v;
            _AnnoListWind.Rad_3.IsEnabled = v;
            _AnnoListWind.Rad_4.IsEnabled = v;
        }

        public void SaveAnoXmlFile()
        {
            try
            {
                if (objectlist.Count != 0 || Ischange)
                {
                    int num = 0;
                    StringBuilder stringBuilder = new StringBuilder();
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.Indent = true;
                    xmlWriterSettings.IndentChars = " ";
                    XmlWriter xmlWriter = XmlWriter.Create(stringBuilder);
                    xmlWriter.WriteStartElement("Slide");
                    if (objectlist.Count >= 0)
                    {
                        xmlWriter.WriteStartElement("Annotations");
                        xmlWriter.WriteStartElement("Regions");
                        foreach (AnnotationBase item in objectlist)
                        {
                            item.WriteXml(xmlWriter);
                        }
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndElement();
                        num++;
                    }
                    if (isPerson)
                    {
                        xmlWriter.WriteStartElement("ImageProcess");
                        xmlWriter.WriteAttributeString("Gama", Gama_Personal.ToString());
                        xmlWriter.WriteAttributeString("Brightness", Bright_Personal.ToString());
                        xmlWriter.WriteAttributeString("Contrast", Contrast_Personal.ToString());
                        xmlWriter.WriteAttributeString("R", R_Personal.ToString());
                        xmlWriter.WriteAttributeString("G", G_Personal.ToString());
                        xmlWriter.WriteAttributeString("B", B_Personal.ToString());
                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndElement();
                        num++;
                    }
                    xmlWriter.Flush();
                    xmlWriter.Close();
                    if (File.Exists(TempPath + TempFilename + ".Ano"))
                    {
                        File.Delete(TempPath + TempFilename + ".Ano");
                    }
                    if (num >= 1)
                    {
                        FileStream fileStream = new FileStream(TempPath + TempFilename + ".Ano", FileMode.OpenOrCreate);
                        byte[] bytes = new UTF8Encoding().GetBytes(stringBuilder.Replace("utf-16", "gb2312").ToString());
                        fileStream.Write(bytes, 0, bytes.Length);
                        fileStream.Close();
                    }
                }
            }
            catch
            {
            }
        }

        public void SaveAnoXmlFile2()
        {
            try
            {
                int num = 0;
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.IndentChars = " ";
                XmlWriter xmlWriter = XmlWriter.Create(stringBuilder);
                xmlWriter.WriteStartElement("Slide");
                if (isPerson)
                {
                    xmlWriter.WriteStartElement("ImageProcess");
                    xmlWriter.WriteAttributeString("Gama", Gama_Personal.ToString());
                    xmlWriter.WriteAttributeString("Brightness", Bright_Personal.ToString());
                    xmlWriter.WriteAttributeString("Contrast", Contrast_Personal.ToString());
                    xmlWriter.WriteAttributeString("R", R_Personal.ToString());
                    xmlWriter.WriteAttributeString("G", G_Personal.ToString());
                    xmlWriter.WriteAttributeString("B", B_Personal.ToString());
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    num++;
                }
                xmlWriter.Flush();
                xmlWriter.Close();
                if (File.Exists(TempPath + TempFilename + ".Ano"))
                {
                    File.Delete(TempPath + TempFilename + ".Ano");
                }
                if (num >= 1)
                {
                    FileStream fileStream = new FileStream(TempPath + TempFilename + ".Ano", FileMode.OpenOrCreate);
                    byte[] bytes = new UTF8Encoding().GetBytes(stringBuilder.Replace("utf-16", "gb2312").ToString());
                    fileStream.Write(bytes, 0, bytes.Length);
                    fileStream.Close();
                }
            }
            catch
            {
            }
        }

        public void LoadCaseXml(string filepath)
        {
            StreamReader streamReader = new StreamReader(filepath, Encoding.Default);
            string text = "";
            while ((text = streamReader.ReadLine()) != null)
            {
                CaseLine.AppendLine(text);
            }
            streamReader.Close();
            _CaseInfoWind._CaseInfo.Inlines.Add(CaseLine.ToString());
        }

        public void LoadAnoXml(string filepath)
        {
            StreamReader streamReader = new StreamReader(filepath);
            string text = "";
            while ((text = streamReader.ReadLine()) != null)
            {
                showAnnotation(xml2vo(text));
            }
            streamReader.Close();
        }

        public void Refresh()
        {
            if (msi.ZoomableCanvas == null)
            {
                return;
            }
            Curscale = msi.ZoomableCanvas.Scale * (double)SlideZoom;
            nav.UpdateThumbnailRect();
            nav.DrawRect(msi.ZoomableCanvas.Scale * (double)SlideZoom);
            double num = 0.0;
            if (SlideZoom == 40)
            {
                num = Setting.Calibration40;
            }
            else if (SlideZoom == 20)
            {
                num = Setting.Calibration20;
            }
            if (num == 0.0)
            {
                num = 1.0;
            }
            double num2 = msi.ZoomableCanvas.Scale * (double)SlideZoom;
            num2 /= num;
            double num3 = Math.Round(num2, 2);
            if (num3 >= (double)(SlideZoom * (int)Setting.MaxMagValue))
            {
                num3 = SlideZoom * (int)Setting.MaxMagValue;
            }
            lbl_Scale.Content = Math.Round(num3, 2) + "X";
            m_prevNewzoom = msi.ZoomableCanvas.Scale * (double)SlideZoom;
            if (num2 > (double)SlideZoom)
            {
                lbl_Scale.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, 0, 0));
            }
            else
            {
                lbl_Scale.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            }
            ReDraw();
            UpdateRule();
            if (!_timer.IsEnabled)
            {
                _timer.IsEnabled = true;
            }
            ctccanvasboard.Children.Clear();
            if (Math.Round(msi.ZoomableCanvas.Scale * (double)SlideZoom, 2) != (double)SlideZoom || CtcWind == null)
            {
                return;
            }
            List<CtcVo> list = CtcWind.listvo.FindAll(delegate (CtcVo user)
            {
                double num6 = user.GlobalPosX;
                double num7 = user.GlobalPosY;
                _ = user.GlobalPosX;
                _ = user.Width;
                _ = user.GlobalPosY;
                _ = user.Height;
                double x2 = msi.ZoomableCanvas.Offset.X;
                double y2 = msi.ZoomableCanvas.Offset.Y;
                double num8 = x2 + msi.ActualWidth;
                double num9 = y2 + msi.ActualHeight;
                bool result = false;
                if (num6 >= x2 && num7 >= y2 && num6 <= num8 && num7 <= num9)
                {
                    result = true;
                }
                return result;
            });
            for (int i = 0; i < list.Count; i++)
            {
                AnnotationBase annotationBase = new AnnotationBase();
                annotationBase.ControlName = annotationBase.AnnotationName;
                annotationBase.AnnotationDescription = "";
                annotationBase.Size = 2.0;
                CtcVo ctcVo = (CtcVo)CtcWind.DgList.SelectedItem;
                if (ctcVo != null)
                {
                    if (ctcVo.GlobalPosX == list[i].GlobalPosX && ctcVo.GlobalPosY == list[i].GlobalPosY)
                    {
                        annotationBase.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(byte.MaxValue, 0, byte.MaxValue, 0));
                    }
                    else
                    {
                        annotationBase.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, 0));
                    }
                }
                else
                {
                    annotationBase.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(byte.MaxValue, byte.MaxValue, 0, 0));
                }
                annotationBase.Zoom = SlideZoom;
                double num4 = (double)list[i].GlobalPosX / (double)SlideZoom;
                double num5 = (double)list[i].GlobalPosY / (double)SlideZoom;
                double x = num4 + (double)list[i].Width / (double)SlideZoom;
                double y = num5 + (double)list[i].Height / (double)SlideZoom;
                annotationBase.CurrentStart = new System.Windows.Point(num4, num5);
                annotationBase.CurrentEnd = new System.Windows.Point(x, y);
                annotationBase.XmlSetPara(alc, ctccanvasboard, msi, objectlist, SlideZoom, Calibration);
                new myCtcRectangle(annotationBase);
            }
        }

        public void ReDraw()
        {
            foreach (AnnotationBase item in objectlist)
            {
                item.UpdateVisual();
            }
        }

        private string GetValueFromDocument(XElement f, string name, string defaultV)
        {
            string result = defaultV;
            XAttribute xAttribute = f.Attribute(name);
            if (xAttribute != null)
            {
                result = xAttribute.Value;
            }
            return result;
        }

        private List<AnnotationBaseVo> xml2vo(string sbXml)
        {
            List<AnnotationBaseVo> list = new List<AnnotationBaseVo>();
            XmlReader reader = XmlReader.Create(new StringReader(sbXml));
            XDocument xDocument = XDocument.Load(reader);
            foreach (XElement item in xDocument.Descendants("Region"))
            {
                List<string> list2 = new List<string>();
                AnnotationBaseVo annotationBaseVo = new AnnotationBaseVo();
                annotationBaseVo.Guid = GetValueFromDocument(item, "Guid", "Line20140528092455");
                annotationBaseVo.Name = GetValueFromDocument(item, "Name", "直线");
                annotationBaseVo.FigureType = GetValueFromDocument(item, "FigureType", "Line");
                annotationBaseVo.Detail = GetValueFromDocument(item, "Detail", "");
                annotationBaseVo.Zoom = GetValueFromDocument(item, "Zoom", "1");
                annotationBaseVo.Size = GetValueFromDocument(item, "Size", "2");
                annotationBaseVo.Color = GetValueFromDocument(item, "Color", "4278190335");
                annotationBaseVo.Hidden = GetValueFromDocument(item, "Hidden", "Collapsed");
                annotationBaseVo.Visible = GetValueFromDocument(item, "Visible", "Collapsed");
                annotationBaseVo.isMsVisble = GetValueFromDocument(item, "MsVisble", "False");
                annotationBaseVo.FontSize = int.Parse(GetValueFromDocument(item, "FontSize", "12"));
                annotationBaseVo.FontItalic = GetValueFromDocument(item, "FontItalic", "False");
                annotationBaseVo.FontBold = GetValueFromDocument(item, "FontBold", "False");
                annotationBaseVo.PinType = GetValueFromDocument(item, "PinType", "images/pin_1.png");
                foreach (XElement item2 in item.Element("Vertices").Elements("Vertice"))
                {
                    list2.Add(item2.Attribute("X").Value + "," + item2.Attribute("Y").Value);
                }
                annotationBaseVo.ListPoint = list2;
                list.Add(annotationBaseVo);
            }
            return list;
        }

        public int LoadPersonImageProcess(string filepath)
        {
            StreamReader streamReader = new StreamReader(filepath);
            string text = "";
            int result = 0;
            while ((text = streamReader.ReadLine()) != null)
            {
                XmlReader reader = XmlReader.Create(new StringReader(text));
                XDocument xDocument = XDocument.Load(reader);
                foreach (XElement item in xDocument.Descendants("ImageProcess"))
                {
                    Gama_Personal = double.Parse(item.Attribute("Gama").Value);
                    R_Personal = double.Parse(item.Attribute("R").Value);
                    G_Personal = double.Parse(item.Attribute("G").Value);
                    B_Personal = double.Parse(item.Attribute("B").Value);
                    Bright_Personal = double.Parse(item.Attribute("Brightness").Value);
                    Contrast_Personal = double.Parse(item.Attribute("Contrast").Value);
                    _AdjustmentWind.GamaSlider.Value = Gama_Personal;
                    _AdjustmentWind.RSlider.Value = R_Personal;
                    _AdjustmentWind.GSlider.Value = G_Personal;
                    _AdjustmentWind.BSlider.Value = B_Personal;
                    _AdjustmentWind.BrightSlider.Value = Bright_Personal;
                    _AdjustmentWind.ContrastSilder.Value = Contrast_Personal;
                    _AdjustmentWind.PersonSetRadio.IsChecked = true;
                    isPerson = true;
                    _AdjustmentWind.TitleArea.Style = (System.Windows.Application.Current.Resources["DialogTitleBorderStyle2"] as Style);
                    result = 1;
                }
            }
            streamReader.Close();
            return result;
        }

        private void showAnnotation(List<AnnotationBaseVo> abvos)
        {
            try
            {
                for (int i = 0; i < abvos.Count; i++)
                {
                    AnnotationBase annotationBase = new AnnotationBase();
                    annotationBase.ControlName = abvos[i].Guid;
                    annotationBase.AnnotationName = abvos[i].Name;
                    annotationBase.AnnotationDescription = abvos[i].Detail;
                    annotationBase.Size = double.Parse(abvos[i].Size);
                    annotationBase.isHidden = ((!(abvos[i].Hidden == "Visible")) ? Visibility.Collapsed : Visibility.Visible);
                    annotationBase.isVisble = ((!(abvos[i].Visible == "Visible")) ? Visibility.Collapsed : Visibility.Visible);
                    annotationBase.BorderBrush = new SolidColorBrush(Setting.NumberToRgba(uint.Parse(abvos[i].Color)));
                    annotationBase.FontBold = ((abvos[i].FontBold == "True") ? true : false);
                    annotationBase.FontItalic = ((abvos[i].FontItalic == "True") ? true : false);
                    annotationBase.FontSize = abvos[i].FontSize;
                    annotationBase.PinType = abvos[i].PinType;
                    annotationBase.isMsVisble = ((abvos[i].isMsVisble == "True") ? true : false);
                    annotationBase.Zoom = double.Parse(abvos[i].Zoom);
                    if (abvos[i].ListPoint.Count() <= 2)
                    {
                        annotationBase.CurrentStart = new System.Windows.Point(double.Parse(abvos[i].ListPoint[0].Split(',')[0]), double.Parse(abvos[i].ListPoint[0].Split(',')[1]));
                        if (abvos[i].ListPoint.Count() == 2)
                        {
                            annotationBase.CurrentEnd = new System.Windows.Point(double.Parse(abvos[i].ListPoint[1].Split(',')[0]), double.Parse(abvos[i].ListPoint[1].Split(',')[1]));
                        }
                    }
                    else
                    {
                        PointCollection pointCollection = new PointCollection();
                        for (int j = 0; j < abvos[i].ListPoint.Count(); j++)
                        {
                            pointCollection.Add(new System.Windows.Point(double.Parse(abvos[i].ListPoint[j].Split(',')[0]), double.Parse(abvos[i].ListPoint[j].Split(',')[1])));
                        }
                        annotationBase.PointCollection = pointCollection;
                    }
                    annotationBase.XmlSetPara(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
                    switch (abvos[i].FigureType)
                    {
                        case "Line":
                            new myLine(annotationBase);
                            break;
                        case "Arrow":
                            new myArrowLine(annotationBase);
                            break;
                        case "Rectangle":
                            new myRectangle(annotationBase);
                            break;
                        case "Ellipse":
                            new myEllipse(annotationBase);
                            break;
                        case "Remark":
                            new myPin(annotationBase);
                            break;
                        case "Polygon":
                            new myPolyline(annotationBase);
                            break;
                        case "DiyCtcRectangle":
                            {
                                myDiyCtcRectangle value = new myDiyCtcRectangle(annotationBase);
                                if (annotationBase.FontSize != 12 || DiyCtcRectangleDic.Count > 0)
                                {
                                    DiyCtcRectangleDic.Add(annotationBase.FontSize, value);
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void Show_AnnoListWind(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                _AnnoListWind.LineWidthComboBox.SelectedValue = objectlist[selectedIndex].Size;
                foreach (ComboBoxItem item in (IEnumerable)_AnnoListWind.LineWidthComboBox.Items)
                {
                    if (item.Content.Equals(objectlist[selectedIndex].Size.ToString()))
                    {
                        _AnnoListWind.LineWidthComboBox.SelectedItem = item;
                        break;
                    }
                }
                if (objectlist[selectedIndex].GetType() == typeof(myPin))
                {
                    SetAnnoListRadioButton(true);
                    string a = objectlist[selectedIndex].PinType.Substring(11, 1);
                    if (a == "1")
                    {
                        _AnnoListWind.Rad_1.IsChecked = true;
                    }
                    else if (a == "2")
                    {
                        _AnnoListWind.Rad_2.IsChecked = true;
                    }
                    else if (a == "3")
                    {
                        _AnnoListWind.Rad_3.IsChecked = true;
                    }
                    else if (a == "4")
                    {
                        _AnnoListWind.Rad_4.IsChecked = true;
                    }
                }
                else
                {
                    SetAnnoListRadioButton(false);
                }
                _AnnoListWind.txt_xbz.Text = objectlist[selectedIndex].AnnotationName;
                _AnnoListWind.txt_qsr.Text = objectlist[selectedIndex].AnnotationDescription;
                _AnnoListWind._colorPicker.SelectedColor = ((SolidColorBrush)objectlist[selectedIndex].BorderBrush).Color;
                _AnnoListWind.ckb_clinfo.IsChecked = ((objectlist[selectedIndex].isVisble == Visibility.Visible) ? true : false);
                _AnnoListWind.ShowMs.IsChecked = objectlist[selectedIndex].isMsVisble;
                _AnnoListWind.tbk_info.Text = objectlist[selectedIndex].CalcMeasureInfo1();
            }
            else
            {
                _AnnoListWind.ckb_clinfo.IsChecked = false;
                _AnnoListWind.ShowMs.IsChecked = false;
                _AnnoListWind.tbk_info.Text = "";
                _AnnoListWind.txt_qsr.Text = "";
                _AnnoListWind.txt_xbz.Text = "";
            }
            System.Windows.Point mainWindowPoint = GetMainWindowPoint();
            _AnnoListWind.Top = mainWindowPoint.Y + (base.ActualHeight - _AnnoListWind.Height) / 2.0;
            _AnnoListWind.Left = mainWindowPoint.X + (base.ActualWidth - _AnnoListWind.Width) / 2.0;
            _AnnoListWind.Show();
            _AnnoListWind.Activate();
            IsAnnoManageWind = Visibility.Visible;
        }

        public XmlDocument LoadXml(string xmlFile)
        {
            if (xmlFile == null || !File.Exists(xmlFile))
            {
                return null;
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlFile);
            return xmlDocument;
        }

        private string GetData(string xpath, XmlDocument xmldoc)
        {
            XmlNodeList xmlNodeList = xmldoc.SelectNodes(xpath);
            return xmlNodeList[0].InnerText;
        }

        private void All_MsShow_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnotationBase item in objectlist)
            {
                item.isMsVisble = true;
                item.UpdateVisual();
            }
            if (_AnnoListWind.cbo_mc.SelectedIndex != -1)
            {
                _AnnoListWind.ShowMs.IsChecked = true;
            }
        }

        private void All_MsHidden_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnotationBase item in objectlist)
            {
                item.isMsVisble = false;
                item.UpdateVisual();
            }
            _AnnoListWind.ShowMs.IsChecked = false;
        }

        private void ShowMs_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                if (_AnnoListWind.ShowMs.IsChecked == true)
                {
                    objectlist[selectedIndex].isMsVisble = true;
                }
                else
                {
                    objectlist[selectedIndex].isMsVisble = false;
                }
                objectlist[selectedIndex].UpdateVisual();
            }
        }

        private void btnallsc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int count = objectlist.Count;
            for (int i = 0; i < count; i++)
            {
                objectlist[0].DeleteItem();
            }
            _AnnoListWind.cbo_mc.SelectedIndex = -1;
            _AnnoListWind.cbo_mc.ItemsSource = null;
            _AnnoListWind.cbo_mc.ItemsSource = objectlist;
        }

        private void All_ClHidden_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnotationBase item in objectlist)
            {
                item.isVisble = Visibility.Collapsed;
                item.UpdateVisual();
            }
            _AnnoListWind.ckb_clinfo.IsChecked = false;
        }

        private void All_ClShow_Click(object sender, RoutedEventArgs e)
        {
            foreach (AnnotationBase item in objectlist)
            {
                item.isVisble = Visibility.Visible;
                item.UpdateVisual();
            }
            if (_AnnoListWind.cbo_mc.SelectedIndex != -1)
            {
                _AnnoListWind.ckb_clinfo.IsChecked = true;
            }
        }

        public void DropDownOpened(object sender, EventArgs e)
        {
            IsDrop = true;
        }

        public void DropDownClosed(object sender, EventArgs e)
        {
            IsDrop = false;
        }

        private void DeleteItem(object sender, RoutedEventArgs e)
        {
            Delete();
            _myTmaRectangle_FinishEvent(null, null);
        }

        public void Delete()
        {
            if (_AnnoListWind.cbo_mc.SelectedIndex != -1)
            {
                object obj = objectlist[_AnnoListWind.cbo_mc.SelectedIndex];
                ((AnnotationBase)obj).DeleteItem();
                _AnnoListWind.cbo_mc.ItemsSource = null;
                _AnnoListWind.cbo_mc.ItemsSource = objectlist;
                if (objectlist.Count == 0)
                {
                    _AnnoListWind.txt_qsr.Text = "";
                    _AnnoListWind.txt_xbz.Text = "";
                    _AnnoListWind.tbk_info.Text = "";
                }
                else
                {
                    _AnnoListWind.cbo_mc.SelectedIndex = -1;
                }
            }
        }

        private void ckb_clinfo(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                if (_AnnoListWind.ckb_clinfo.IsChecked == true && objectlist[selectedIndex].isHidden == Visibility.Visible)
                {
                    objectlist[selectedIndex].isVisble = Visibility.Visible;
                }
                else
                {
                    objectlist[selectedIndex].isVisble = Visibility.Collapsed;
                }
                ReDraw();
            }
        }

        private void btnyc_change(object sender, RoutedEventArgs e)
        {
            int num = 0;
            int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex == -1)
            {
                return;
            }
            if (_AnnoListWind.btnyc.Opacity == 0.5)
            {
                objectlist[selectedIndex].IsActive(Visibility.Visible);
                if (_AnnoListWind.ckb_clinfo.IsChecked == true)
                {
                    objectlist[selectedIndex].isVisble = Visibility.Visible;
                }
                objectlist[selectedIndex].isHidden = Visibility.Visible;
                foreach (AnnotationBase item in objectlist)
                {
                    if (item.isHidden == Visibility.Visible)
                    {
                        num++;
                        if (num == objectlist.Count)
                        {
                            _AnnoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部隐藏");
                            _AnnoListWind.btnqyc.Opacity = 1.0;
                        }
                    }
                }
                _AnnoListWind.btnyc.Opacity = 1.0;
                _AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "隐藏");
            }
            else
            {
                num = 0;
                _AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "显示");
                objectlist[selectedIndex].IsActive(Visibility.Collapsed);
                objectlist[selectedIndex].isVisble = Visibility.Collapsed;
                objectlist[selectedIndex].isHidden = Visibility.Collapsed;
                _AnnoListWind.btnyc.Opacity = 0.5;
                foreach (AnnotationBase item2 in objectlist)
                {
                    if (item2.isHidden == Visibility.Collapsed)
                    {
                        num++;
                        if (num == objectlist.Count)
                        {
                            _AnnoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部显示");
                            _AnnoListWind.btnqyc.Opacity = 0.5;
                        }
                    }
                }
            }
            ReDraw();
        }

        private void allhidden_change(object sender, RoutedEventArgs e)
        {
            if (_AnnoListWind.btnqyc.Opacity == 1.0)
            {
                foreach (AnnotationBase item in objectlist)
                {
                    item.isHidden = Visibility.Collapsed;
                    item.IsActive(Visibility.Collapsed);
                    item.MTextBlock.Visibility = Visibility.Collapsed;
                    item.UpdateVisual();
                }
                _AnnoListWind.cbo_mc.SelectedIndex = -1;
                _AnnoListWind.txt_qsr.Text = "";
                _AnnoListWind.txt_xbz.Text = "";
                _AnnoListWind.tbk_info.Text = "";
                _AnnoListWind.btnyc.Opacity = 0.5;
                _AnnoListWind.btnqyc.Opacity = 0.5;
                _AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "显示");
                _AnnoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部显示");
            }
            else
            {
                _AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "隐藏");
                _AnnoListWind.btnqyc.SetValue(ToolTipService.ToolTipProperty, "全部隐藏");
                foreach (AnnotationBase item2 in objectlist)
                {
                    item2.isHidden = Visibility.Visible;
                    item2.isVisble = Visibility.Visible;
                    item2.MTextBlock.Visibility = Visibility.Visible;
                    item2.UpdateVisual();
                }
                _AnnoListWind.btnyc.Opacity = 1.0;
                _AnnoListWind.btnqyc.Opacity = 1.0;
            }
        }

        private void mc_TextChanged(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                objectlist[selectedIndex].AnnotationName = _AnnoListWind.txt_xbz.Text;
                _AnnoListWind.cbo_mc.ItemsSource = null;
                _AnnoListWind.cbo_mc.ItemsSource = objectlist;
                _AnnoListWind.cbo_mc.SelectedIndex = selectedIndex;
            }
        }

        private void cbo_mcSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex == -1)
            {
                return;
            }
            foreach (AnnotationBase item in objectlist)
            {
                item.IsActive(Visibility.Collapsed);
            }
            _AnnoListWind.txt_qsr.Text = objectlist[selectedIndex].AnnotationDescription;
            _AnnoListWind.txt_xbz.Text = objectlist[selectedIndex].AnnotationName;
            _AnnoListWind.tbk_info.Text = objectlist[selectedIndex].CalcMeasureInfo1();
            _AnnoListWind.ckb_clinfo.IsChecked = ((objectlist[selectedIndex].isVisble == Visibility.Visible) ? true : false);
            _AnnoListWind.ShowMs.IsChecked = objectlist[selectedIndex].isMsVisble;
            _AnnoListWind._colorPicker.SelectedColor = ((SolidColorBrush)objectlist[selectedIndex].BorderBrush).Color;
            _AnnoListWind.LineWidthComboBox.SelectedValue = objectlist[selectedIndex].Size;
            foreach (ComboBoxItem item2 in (IEnumerable)_AnnoListWind.LineWidthComboBox.Items)
            {
                if (item2.Content.Equals(objectlist[selectedIndex].Size.ToString()))
                {
                    _AnnoListWind.LineWidthComboBox.SelectedItem = item2;
                    break;
                }
            }
            if (objectlist[selectedIndex].isHidden == Visibility.Visible)
            {
                objectlist[selectedIndex].IsActive(Visibility.Visible);
            }
            if (objectlist[selectedIndex].isHidden == Visibility.Visible)
            {
                _AnnoListWind.btnyc.Opacity = 1.0;
                _AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "隐藏");
                objectlist[selectedIndex].isHidden = Visibility.Visible;
            }
            else
            {
                _AnnoListWind.btnyc.Opacity = 0.5;
                _AnnoListWind.btnyc.SetValue(ToolTipService.ToolTipProperty, "显示");
                objectlist[selectedIndex].isHidden = Visibility.Collapsed;
            }
            if (objectlist[selectedIndex].isFinish && IsDrop)
            {
                dw(sender, new RoutedEventArgs());
            }
            if (objectlist[selectedIndex].GetType() == typeof(myPin))
            {
                SetAnnoListRadioButton(true);
                string a = objectlist[selectedIndex].PinType.Substring(12, 1);
                if (a == "1")
                {
                    _AnnoListWind.Rad_1.IsChecked = true;
                }
                else if (a == "2")
                {
                    _AnnoListWind.Rad_2.IsChecked = true;
                }
                else if (a == "3")
                {
                    _AnnoListWind.Rad_3.IsChecked = true;
                }
                else if (a == "4")
                {
                    _AnnoListWind.Rad_4.IsChecked = true;
                }
            }
            else
            {
                SetAnnoListRadioButton(false);
            }
            ReDraw();
        }

        private void txt_qsrTextChanged(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                objectlist[selectedIndex].AnnotationDescription = _AnnoListWind.txt_qsr.Text;
                objectlist[selectedIndex].UpadteTextBlock();
            }
        }

        private void LineWidthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_AnnoListWind.LineWidthComboBox.SelectedItem != null)
            {
                int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
                if (selectedIndex != -1)
                {
                    objectlist[selectedIndex].Size = int.Parse((_AnnoListWind.LineWidthComboBox.SelectedItem as ComboBoxItem).Content.ToString());
                    objectlist[selectedIndex].UpdateVisual();
                }
            }
        }

        private void _colorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color> e)
        {
            int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                objectlist[selectedIndex].BorderBrush = new SolidColorBrush(_AnnoListWind._colorPicker.SelectedColor);
                if (objectlist[selectedIndex].GetType() == typeof(myPin))
                {
                    objectlist[selectedIndex].FontColor = new SolidColorBrush(_AnnoListWind._colorPicker.SelectedColor);
                }
                objectlist[selectedIndex].UpdateVisual();
            }
        }

        private void Rad_Checked(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                string a = ((System.Windows.Controls.RadioButton)sender).Name.ToString();
                if (a == "Rad_1")
                {
                    objectlist[selectedIndex].PinType = "images/pin_1.png";
                }
                else if (a == "Rad_2")
                {
                    objectlist[selectedIndex].PinType = "images/pin_2.png";
                }
                else if (a == "Rad_3")
                {
                    objectlist[selectedIndex].PinType = "images/pin_3.png";
                }
                else if (a == "Rad_4")
                {
                    objectlist[selectedIndex].PinType = "images/pin_4.png";
                }
                objectlist[selectedIndex].UpdateVisual();
            }
        }

        private void dw(object sender, RoutedEventArgs e)
        {
            int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                AnnotationBase annotationBase = objectlist[selectedIndex];
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {
                        if ((Mainpage)item.Value != this)
                        {
                            ((Mainpage)item.Value).SynMsiAnno(annotationBase);
                        }
                    }
                }
                double num = annotationBase.CurrentEnd.X * annotationBase.Zoom - annotationBase.Zoom * annotationBase.CurrentStart.X;
                double num2 = annotationBase.CurrentEnd.Y * annotationBase.Zoom - annotationBase.Zoom * annotationBase.CurrentStart.Y;
                System.Windows.Point point = new System.Windows.Point(annotationBase.Zoom * annotationBase.CurrentStart.X - msi.ActualWidth / 2.0 + num / 2.0, annotationBase.Zoom * annotationBase.CurrentStart.Y - msi.ActualHeight / 2.0 + num2 / 2.0);
                double scale = msi.ZoomableCanvas.Scale;
                int level = msi.Source.GetLevel(annotationBase.Zoom / (double)SlideZoom);
                int currentLevel = msi._spatialSource.CurrentLevel;
                if (level != currentLevel)
                {
                    msi._spatialSource.CurrentLevel = level;
                }
                if (scale != annotationBase.Zoom / (double)SlideZoom)
                {
                    double num3 = 4.0;
                    TimeSpan timeSpan = TimeSpan.FromMilliseconds(num3 * 100.0);
                    CubicEase easingFunction = new CubicEase();
                    msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(annotationBase.Zoom / (double)SlideZoom, timeSpan)
                    {
                        EasingFunction = easingFunction
                    }, HandoffBehavior.Compose);
                    msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new System.Windows.Point(point.X, point.Y), timeSpan)
                    {
                        EasingFunction = easingFunction
                    }, HandoffBehavior.Compose);
                }
                else
                {
                    msi.ZoomableCanvas.Offset = new System.Windows.Point(point.X, point.Y);
                    msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                }
            }
        }

        public void SynMsiAnno(AnnotationBase ab)
        {
            double num = ab.CurrentEnd.X * ab.Zoom - ab.Zoom * ab.CurrentStart.X;
            double num2 = ab.CurrentEnd.Y * ab.Zoom - ab.Zoom * ab.CurrentStart.Y;
            System.Windows.Point point = new System.Windows.Point(ab.Zoom * ab.CurrentStart.X - msi.ActualWidth / 2.0 + num / 2.0, ab.Zoom * ab.CurrentStart.Y - msi.ActualHeight / 2.0 + num2 / 2.0);
            double scale = msi.ZoomableCanvas.Scale;
            int level = msi.Source.GetLevel(ab.Zoom / (double)SlideZoom);
            int currentLevel = msi._spatialSource.CurrentLevel;
            if (level != currentLevel)
            {
                msi._spatialSource.CurrentLevel = level;
            }
            if (scale != ab.Zoom / (double)SlideZoom)
            {
                double num3 = 4.0;
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(num3 * 100.0);
                CubicEase easingFunction = new CubicEase();
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(ab.Zoom / (double)SlideZoom, timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new System.Windows.Point(point.X, point.Y), timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
            }
            else
            {
                msi.ZoomableCanvas.Offset = new System.Windows.Point(point.X, point.Y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            }
        }

        public void DoPostion()
        {
            double num = 0.0;
            double num2 = 0.0;
            int selectedIndex = _AnnoListWind.cbo_mc.SelectedIndex;
            if (selectedIndex != -1)
            {
                double num3 = msi.ActualWidth / ImageW;
                double num4 = num3 * (double)SlideZoom / objectlist[selectedIndex].Zoom;
                System.Windows.Point point = new System.Windows.Point(0.0, 0.0);
                System.Windows.Point point2 = default(System.Windows.Point);
                point2.X = (objectlist[selectedIndex].CurrentStart.X * objectlist[selectedIndex].Zoom + objectlist[selectedIndex].CurrentEnd.X * objectlist[selectedIndex].Zoom) / 2.0;
                point2.Y = (objectlist[selectedIndex].CurrentStart.Y * objectlist[selectedIndex].Zoom + objectlist[selectedIndex].CurrentEnd.Y * objectlist[selectedIndex].Zoom) / 2.0;
                num = msi.ActualWidth / 2.0 - point2.X;
                num2 = msi.ActualHeight / 2.0 - point2.Y;
                point.X += (0.0 - num) / msi.ActualWidth * num4;
                point.Y += (0.0 - num2) / msi.ActualWidth * num4;
                double num5 = ScaleToViewportWidth(msi.ZoomableCanvas.Scale, msi.ActualWidth, msi.Source.ImageSize.Width);
                System.Windows.Point offset = default(System.Windows.Point);
                offset.X = point.X * msi.ActualWidth / num5;
                offset.Y = point.Y * msi.ActualWidth / num5;
                double x = offset.X - msi.ZoomableCanvas.Offset.X;
                double y = offset.Y - msi.ZoomableCanvas.Offset.Y;
                msi.ZoomableCanvas.Offset = offset;
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {
                        if ((Mainpage)item.Value != this)
                        {
                            ((Mainpage)item.Value).TmoveSetOffset((Mainpage)item.Value, new System.Windows.Point(x, y));
                        }
                    }
                }
            }
        }

        public void ZoomableCanvas_Refresh(object d, DependencyPropertyChangedEventArgs e)
        {
            Refresh();
        }

        public void m_RightZoom(object sender, RoutedEventArgs e)
        {
            myRectZoom myRectZoom = (myRectZoom)sender;
            double num = myRectZoom.Scale;
            double curscale = Curscale;
            curscale *= num;
            if (curscale > (double)SlideZoom * Setting.MaxMagValue)
            {
                curscale = (double)SlideZoom * Setting.MaxMagValue;
                num = curscale / Curscale;
            }
            if (Setting.IsSynchronous)
            {
                foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                {
                    if ((Mainpage)item.Value != this)
                    {
                        ((Mainpage)item.Value).SynMsi(Curscale, myRectZoom.pCenter, num);
                    }
                }
            }
            System.Windows.Point pCenter = myRectZoom.pCenter;
            System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
            pCenter = new System.Windows.Point(pCenter.X - LayoutBody.ActualWidth / 2.0, pCenter.Y - LayoutBody.ActualHeight / 2.0);
            System.Windows.Point point = KCommon.PointRotate(center, pCenter, Rotate);
            double x = point.X + msi.ActualWidth / 2.0;
            double y = point.Y + msi.ActualHeight / 2.0;
            pCenter = new System.Windows.Point(x, y);
            double num2 = msi.ZoomableCanvas.Extent.Width * msi.ZoomableCanvas.Scale;
            double num3 = msi.ZoomableCanvas.Extent.Height * msi.ZoomableCanvas.Scale;
            double num4 = msi.ZoomableCanvas.Extent.Width * (curscale / (double)SlideZoom);
            double num5 = msi.ZoomableCanvas.Extent.Height * (curscale / (double)SlideZoom);
            System.Windows.Point point2 = new System.Windows.Point(pCenter.X + msi.ZoomableCanvas.Offset.X - num2 / 2.0, pCenter.Y + msi.ZoomableCanvas.Offset.Y - num3 / 2.0);
            System.Windows.Point point3 = new System.Windows.Point(point2.X * num + num4 / 2.0 - pCenter.X - (msi.ActualWidth / 2.0 - pCenter.X), point2.Y * num + num5 / 2.0 - pCenter.Y - (msi.ActualHeight / 2.0 - pCenter.Y));
            double scale = msi.ZoomableCanvas.Scale;
            int level = msi.Source.GetLevel(curscale / (double)SlideZoom);
            int currentLevel = msi._spatialSource.CurrentLevel;
            if (level != currentLevel)
            {
                msi._spatialSource.CurrentLevel = level;
            }
            if (scale != curscale / (double)SlideZoom)
            {
                double num6 = 4.0;
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(num6 * 100.0);
                CubicEase easingFunction = new CubicEase();
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(curscale / (double)SlideZoom, timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new System.Windows.Point(point3.X, point3.Y), timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
            }
            else
            {
                msi.ZoomableCanvas.Offset = new System.Windows.Point(point3.X, point3.Y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            }
        }

        private void msi_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            int num = e.Timestamp - m_prevTimeStap;
            if (Setting.isCtrl == 0 || x3dSlider.Visibility == Visibility.Collapsed)
            {
                Setting.Opacity = 1;
                IsDw = false;
                double num2 = 1.0;
                double num3 = Setting.MaxMagValue * (double)SlideZoom * Setting.MargPara;
                num2 = CalcSpeed(num);
                m_prevTimeStap = e.Timestamp;
                double curscale = Curscale;
                if (curscale == m_prevNewzoom || !(m_prevNewzoom > 1E-08))
                {
                    double magAdjValueByCurMag = GetMagAdjValueByCurMag(Curscale);
                    curscale = ((e.Delta <= 0) ? (curscale - magAdjValueByCurMag * num2) : (curscale + magAdjValueByCurMag * num2));
                    if (curscale < fitratio)
                    {
                        curscale = fitratio;
                    }
                    if (curscale > num3)
                    {
                        curscale = num3;
                    }
                    System.Windows.Point position = e.GetPosition(this);
                    if (Setting.IsSynchronous)
                    {
                        foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                        {
                            if ((Mainpage)item.Value != this)
                            {
                                ((Mainpage)item.Value).ZoomRatio(curscale, position.X, position.Y);
                            }
                        }
                    }
                    ZoomRatio(curscale, position.X, position.Y);
                    m_prevNewzoom = curscale;
                }
            }
            else if (num > 300)
            {
                m_prevTimeStap = e.Timestamp;
                if (e.Delta > 0)
                {
                    UpZ_MouseLeftButtonDown(null, null);
                }
                else
                {
                    DownZ_MouseLeftButtonDown(null, null);
                }
            }
        }

        private void msi_Ini(object sender, RoutedEventArgs e)
        {
            string filePath = FilePath;
            nav._Mainpage = this;
            TempPath = filePath.Substring(0, filePath.LastIndexOf("\\") + 1);
            TempFilename = filePath.Substring(filePath.LastIndexOf("\\") + 1, filePath.Length - filePath.LastIndexOf("\\") - 1);
            nav.SetThumbnail(LoadImage(InfoStruct.DataFilePTR, 8, 0, 0));
            nav.SetMultiScaleImage(msi);
            Image_lable.Source = GetLable(InfoStruct.DataFilePTR);
            msi.Tag = "1";
            alc.CB = _AnnoListWind.cbo_mc;
            alc.Tbk = _AnnoListWind.tbk_info;
            alc.Tbx = _AnnoListWind.txt_xbz;
            alc.qsr = _AnnoListWind.txt_qsr;
            if (File.Exists(TempPath + TempFilename + ".Ano"))
            {
                LoadAnoXml(TempPath + TempFilename + ".Ano");
                if (objectlist.Count > 0)
                {
                    Ischange = true;
                }
            }
            if (File.Exists(TempPath + TempFilename + ".case"))
            {
                LoadCaseXml(TempPath + TempFilename + ".case");
            }
            _myRectZoom = new myRectZoom(alc, RectCanvas, msi, objectlist, SlideZoom, Calibration);
            _myRectZoom.RightZoom += m_RightZoom;
            ArcMenu();
            fitratio = (double)SlideZoom * msi.ZoomableCanvas.Scale;
            fitx = msi.ZoomableCanvas.Offset.X;
            fity = msi.ZoomableCanvas.Offset.Y;
            Refresh();
            string isLabel = Setting.IsLabel;
            string isCase = Setting.IsCase;
            string isNav = Setting.IsNav;
            string isRule = Setting.IsRule;
            string isMagnifier = Setting.IsMagnifier;
            string isRotate = Setting.IsRotate;
            string isOperateball = Setting.IsOperateball;
            MagnifierScale = Setting.Magnifier;
            if (isMagnifier == "1")
            {
                Canv_Magnifier.Visibility = Visibility.Visible;
                _Magnifiertimer.Start();
            }
            else
            {
                Canv_Magnifier.Visibility = Visibility.Collapsed;
                _Magnifiertimer.Stop();
            }
            if (isRotate == "1")
            {
                _RotateViewer.Visibility = Visibility.Visible;
            }
            else
            {
                _RotateViewer.Visibility = Visibility.Collapsed;
            }
            if (isOperateball == "1")
            {
                Canvas_Operateball.Visibility = Visibility.Visible;
            }
            else
            {
                Canvas_Operateball.Visibility = Visibility.Collapsed;
            }
            if (isLabel == "1")
            {
                Image_lable.Visibility = Visibility.Visible;
            }
            else
            {
                Image_lable.Visibility = Visibility.Collapsed;
            }
            if (isNav == "1")
            {
                nav.Visibility = Visibility.Visible;
            }
            else
            {
                nav.Visibility = Visibility.Collapsed;
            }
            if (isRule == "1")
            {
                RuleCanvas.Visibility = Visibility.Visible;
            }
            else
            {
                RuleCanvas.Visibility = Visibility.Collapsed;
            }
            if (isCase == "1")
            {
                Show_CaseInfoWind(null, null);
            }
        }

        private void ZoomClick(object sender, RoutedEventArgs e)
        {
            IsDw = false;
            double zoom_ratio = 0.0;
            switch (((System.Windows.Controls.Image)sender).Name)
            {
                case "buttonR1":
                    zoom_ratio = 1.0;
                    break;
                case "buttonR2":
                    zoom_ratio = 2.0;
                    break;
                case "buttonR4":
                    zoom_ratio = 4.0;
                    break;
                case "buttonR10":
                    zoom_ratio = 10.0;
                    break;
                case "buttonR20":
                    zoom_ratio = 20.0;
                    break;
                case "buttonR40":
                    zoom_ratio = 40.0;
                    break;
            }
            if (Setting.IsSynchronous)
            {
                foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                {
                    if ((Mainpage)item.Value != this)
                    {
                        ((Mainpage)item.Value).ZoomRatio(zoom_ratio);
                    }
                }
            }
            ZoomRatio(zoom_ratio);
        }

        public void ZoomRatio(double zoom_ratio)
        {
            double num = 0.0;
            if (SlideZoom == 40)
            {
                num = Setting.Calibration40;
            }
            else if (SlideZoom == 20)
            {
                num = Setting.Calibration20;
            }
            if (num == 0.0)
            {
                num = 1.0;
            }
            System.Windows.Point elementPoint = new System.Windows.Point(msi.ActualWidth / 2.0, msi.ActualHeight / 2.0);
            System.Windows.Point point = msi.ElementToLogicalPoint(elementPoint);
            msi.ZoomAboutLogicalPoint(zoom_ratio * num / Curscale, point.X, point.Y);
            Curscale = zoom_ratio;
            Refresh();
        }

        public void ZoomRatio(double zoom_ratio, double x, double y)
        {
            double num = 0.0;
            if (SlideZoom == 40)
            {
                num = Setting.Calibration40;
            }
            else if (SlideZoom == 20)
            {
                num = Setting.Calibration20;
            }
            if (num == 0.0)
            {
                num = 1.0;
            }
            System.Windows.Point point = new System.Windows.Point(x, y);
            System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
            point = new System.Windows.Point(point.X - LayoutBody.ActualWidth / 2.0, point.Y - LayoutBody.ActualHeight / 2.0);
            System.Windows.Point point2 = KCommon.PointRotate(center, point, Rotate);
            double x2 = point2.X + msi.ActualWidth / 2.0;
            double y2 = point2.Y + msi.ActualHeight / 2.0;
            System.Windows.Point elementPoint = new System.Windows.Point(x2, y2);
            System.Windows.Point point3 = msi.ElementToLogicalPoint(elementPoint);
            msi.ZoomAboutLogicalPoint(zoom_ratio / Curscale, point3.X, point3.Y);
            Curscale = zoom_ratio;
            Refresh();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.RightCtrl:
                    Setting.isCtrl = 1;
                    break;
                case Key.OemPlus:
                    {
                        double curscale4 = Curscale;
                        curscale4 *= 2.4000000953674316;
                        ZoomRatio(curscale4);
                        break;
                    }
                case Key.OemMinus:
                    {
                        double curscale3 = Curscale;
                        curscale3 /= 2.4000000953674316;
                        ZoomRatio(curscale3);
                        break;
                    }
                case Key.Delete:
                    Delete();
                    break;
                case Key.D1:
                    ZoomRatio(1.0);
                    break;
                case Key.NumPad1:
                    ZoomRatio(1.0);
                    break;
                case Key.D2:
                    ZoomRatio(2.0);
                    break;
                case Key.NumPad2:
                    ZoomRatio(2.0);
                    break;
                case Key.D3:
                    ZoomRatio(4.0);
                    break;
                case Key.NumPad3:
                    ZoomRatio(4.0);
                    break;
                case Key.D4:
                    ZoomRatio(10.0);
                    break;
                case Key.NumPad4:
                    ZoomRatio(10.0);
                    break;
                case Key.D5:
                    ZoomRatio(20.0);
                    break;
                case Key.NumPad5:
                    ZoomRatio(20.0);
                    break;
                case Key.D6:
                    ZoomRatio(40.0);
                    break;
                case Key.NumPad6:
                    ZoomRatio(40.0);
                    break;
                case Key.D7:
                    ZoomRatio(80.0);
                    break;
                case Key.NumPad7:
                    ZoomRatio(80.0);
                    break;
                case Key.D8:
                    ZoomRatio(160.0);
                    break;
                case Key.NumPad8:
                    ZoomRatio(160.0);
                    break;
                case Key.D0:
                    ZoomRatio(fitratio);
                    break;
                case Key.NumPad0:
                    ZoomRatio(fitratio);
                    break;
                case Key.F18:
                    ZoomRatio(fitratio);
                    break;
                case Key.Add:
                    {
                        double curscale2 = Curscale;
                        curscale2 *= 2.4000000953674316;
                        ZoomRatio(curscale2);
                        break;
                    }
                case Key.Subtract:
                    {
                        double curscale = Curscale;
                        curscale /= 2.4000000953674316;
                        ZoomRatio(curscale);
                        break;
                    }
                case Key.Up:
                    {
                        System.Windows.Point offset5 = msi.ZoomableCanvas.Offset;
                        System.Windows.Point p = new System.Windows.Point(0.0, KeyStep);
                        System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
                        System.Windows.Point point = KCommon.PointRotate(center, p, Rotate);
                        offset5.Y += point.Y;
                        msi.ZoomableCanvas.Offset = offset5;
                        msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                        break;
                    }
                case Key.Down:
                    {
                        System.Windows.Point offset4 = msi.ZoomableCanvas.Offset;
                        offset4.Y -= KeyStep;
                        msi.ZoomableCanvas.Offset = offset4;
                        msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                        break;
                    }
                case Key.Left:
                    {
                        System.Windows.Point offset3 = msi.ZoomableCanvas.Offset;
                        offset3.X += KeyStep;
                        msi.ZoomableCanvas.Offset = offset3;
                        msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                        break;
                    }
                case Key.Right:
                    {
                        System.Windows.Point offset2 = msi.ZoomableCanvas.Offset;
                        offset2.X -= KeyStep;
                        msi.ZoomableCanvas.Offset = offset2;
                        msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                        break;
                    }
                case Key.Space:
                    {
                        System.Windows.Point offset = new System.Windows.Point(fitx, fity);
                        ZoomRatio(fitratio);
                        msi.ZoomableCanvas.Offset = offset;
                        msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                        break;
                    }
            }
        }

        private void msi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point position = e.GetPosition(this);
            double num = 2.4000000953674316;
            double curscale = Curscale;
            curscale *= num;
            if (curscale > (double)SlideZoom * Setting.MaxMagValue)
            {
                curscale = (double)SlideZoom * Setting.MaxMagValue;
                num = curscale / Curscale;
            }
            if (Setting.IsSynchronous)
            {
                foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                {
                    if ((Mainpage)item.Value != this)
                    {
                        ((Mainpage)item.Value).SynMsi(Curscale, position, num);
                    }
                }
            }
            System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
            position = new System.Windows.Point(position.X - LayoutBody.ActualWidth / 2.0, position.Y - LayoutBody.ActualHeight / 2.0);
            System.Windows.Point point = KCommon.PointRotate(center, position, Rotate);
            double x = point.X + msi.ActualWidth / 2.0;
            double y = point.Y + msi.ActualHeight / 2.0;
            position = new System.Windows.Point(x, y);
            double num2 = msi.ZoomableCanvas.Extent.Width * msi.ZoomableCanvas.Scale;
            double num3 = msi.ZoomableCanvas.Extent.Height * msi.ZoomableCanvas.Scale;
            double num4 = msi.ZoomableCanvas.Extent.Width * (curscale / (double)SlideZoom);
            double num5 = msi.ZoomableCanvas.Extent.Height * (curscale / (double)SlideZoom);
            System.Windows.Point point2 = new System.Windows.Point(position.X + msi.ZoomableCanvas.Offset.X - num2 / 2.0, position.Y + msi.ZoomableCanvas.Offset.Y - num3 / 2.0);
            System.Windows.Point point3 = new System.Windows.Point(point2.X * num + num4 / 2.0 - position.X - (msi.ActualWidth / 2.0 - position.X), point2.Y * num + num5 / 2.0 - position.Y - (msi.ActualHeight / 2.0 - position.Y));
            double scale = msi.ZoomableCanvas.Scale;
            int level = msi.Source.GetLevel(curscale / (double)SlideZoom);
            int currentLevel = msi._spatialSource.CurrentLevel;
            if (level != currentLevel)
            {
                msi._spatialSource.CurrentLevel = level;
            }
            if (scale != curscale / (double)SlideZoom)
            {
                double num6 = 4.0;
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(num6 * 100.0);
                CubicEase easingFunction = new CubicEase();
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(curscale / (double)SlideZoom, timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new System.Windows.Point(point3.X, point3.Y), timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
            }
            else
            {
                msi.ZoomableCanvas.Offset = new System.Windows.Point(point3.X, point3.Y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            }
        }

        public void SynMsi(double newzoom, System.Windows.Point p, double dbscale)
        {
            newzoom *= dbscale;
            if (newzoom > (double)SlideZoom * Setting.MaxMagValue)
            {
                newzoom = (double)SlideZoom * Setting.MaxMagValue;
                dbscale = newzoom / Curscale;
            }
            System.Windows.Point center = new System.Windows.Point(0.0, 0.0);
            p = new System.Windows.Point(p.X - LayoutBody.ActualWidth / 2.0, p.Y - LayoutBody.ActualHeight / 2.0);
            System.Windows.Point point = KCommon.PointRotate(center, p, Rotate);
            double x = point.X + msi.ActualWidth / 2.0;
            double y = point.Y + msi.ActualHeight / 2.0;
            p = new System.Windows.Point(x, y);
            double num = msi.ZoomableCanvas.Extent.Width * msi.ZoomableCanvas.Scale;
            double num2 = msi.ZoomableCanvas.Extent.Height * msi.ZoomableCanvas.Scale;
            double num3 = msi.ZoomableCanvas.Extent.Width * (newzoom / (double)SlideZoom);
            double num4 = msi.ZoomableCanvas.Extent.Height * (newzoom / (double)SlideZoom);
            System.Windows.Point point2 = new System.Windows.Point(p.X + msi.ZoomableCanvas.Offset.X - num / 2.0, p.Y + msi.ZoomableCanvas.Offset.Y - num2 / 2.0);
            System.Windows.Point point3 = new System.Windows.Point(point2.X * dbscale + num3 / 2.0 - p.X - (msi.ActualWidth / 2.0 - p.X), point2.Y * dbscale + num4 / 2.0 - p.Y - (msi.ActualHeight / 2.0 - p.Y));
            double scale = msi.ZoomableCanvas.Scale;
            int level = msi.Source.GetLevel(newzoom / (double)SlideZoom);
            int currentLevel = msi._spatialSource.CurrentLevel;
            if (level != currentLevel)
            {
                msi._spatialSource.CurrentLevel = level;
            }
            if (scale != newzoom / (double)SlideZoom)
            {
                double num5 = 4.0;
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(num5 * 100.0);
                CubicEase easingFunction = new CubicEase();
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.ScaleProperty, new DoubleAnimation(newzoom / (double)SlideZoom, timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
                msi.ZoomableCanvas.BeginAnimation(ZoomableCanvas.OffsetProperty, new PointAnimation(new System.Windows.Point(point3.X, point3.Y), timeSpan)
                {
                    EasingFunction = easingFunction
                }, HandoffBehavior.Compose);
            }
            else
            {
                msi.ZoomableCanvas.Offset = new System.Windows.Point(point3.X, point3.Y);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            }
        }

        public void msi_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_myRectZoom != null && _myRectZoom.MRectangle != null && _myRectZoom.MRectangle.Width > 0.0)
            {
                return;
            }
            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            label.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["ctcRect"];
            label.FontSize = 16.0;
            label.Height = 30.0;
            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label.VerticalContentAlignment = VerticalAlignment.Top;
            label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header = label;
            System.Windows.Controls.Label label2 = new System.Windows.Controls.Label();
            label2.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Line"];
            label2.FontSize = 16.0;
            label2.Height = 30.0;
            label2.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label2.VerticalContentAlignment = VerticalAlignment.Top;
            label2.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header2 = label2;
            System.Windows.Controls.Label label3 = new System.Windows.Controls.Label();
            label3.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Arrow"];
            label3.FontSize = 16.0;
            label3.Height = 30.0;
            label3.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label3.VerticalContentAlignment = VerticalAlignment.Top;
            label3.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header3 = label3;
            System.Windows.Controls.Label label4 = new System.Windows.Controls.Label();
            label4.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Rectangle"];
            label4.FontSize = 16.0;
            label4.Height = 30.0;
            label4.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label4.VerticalContentAlignment = VerticalAlignment.Top;
            label4.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header4 = label4;
            System.Windows.Controls.Label label5 = new System.Windows.Controls.Label();
            label5.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Ellipse"];
            label5.FontSize = 16.0;
            label5.Height = 30.0;
            label5.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label5.VerticalContentAlignment = VerticalAlignment.Top;
            label5.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header5 = label5;
            System.Windows.Controls.Label label6 = new System.Windows.Controls.Label();
            label6.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Polyline"];
            label6.FontSize = 16.0;
            label6.Height = 30.0;
            label6.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label6.VerticalContentAlignment = VerticalAlignment.Top;
            label6.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header6 = label6;
            System.Windows.Controls.Label label7 = new System.Windows.Controls.Label();
            label7.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Remark"];
            label7.FontSize = 16.0;
            label7.Height = 30.0;
            label7.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label7.VerticalContentAlignment = VerticalAlignment.Top;
            label7.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header7 = label7;
            System.Windows.Controls.Label label8 = new System.Windows.Controls.Label();
            label8.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Other"];
            label8.FontSize = 16.0;
            label8.Height = 30.0;
            label8.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label8.VerticalContentAlignment = VerticalAlignment.Center;
            label8.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header8 = label8;
            System.Windows.Controls.Label label9 = new System.Windows.Controls.Label();
            label9.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["ImageAdjustment"];
            label9.FontSize = 16.0;
            label9.Height = 30.0;
            label9.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label9.VerticalContentAlignment = VerticalAlignment.Center;
            label9.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header9 = label9;
            System.Windows.Controls.Label label10 = new System.Windows.Controls.Label();
            label10.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Export"];
            label10.FontSize = 16.0;
            label10.Height = 30.0;
            label10.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label10.VerticalContentAlignment = VerticalAlignment.Center;
            label10.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header10 = label10;
            System.Windows.Controls.Label label11 = new System.Windows.Controls.Label();
            label11.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["ImageSyn"];
            label11.FontSize = 16.0;
            label11.Height = 30.0;
            label11.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label11.VerticalContentAlignment = VerticalAlignment.Center;
            label11.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header11 = label11;
            System.Windows.Controls.Label label12 = new System.Windows.Controls.Label();
            label12.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["ImageSynCancle"];
            label12.FontSize = 16.0;
            label12.Height = 30.0;
            label12.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label12.VerticalContentAlignment = VerticalAlignment.Center;
            label12.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header12 = label12;
            System.Windows.Controls.Label label13 = new System.Windows.Controls.Label();
            label13.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["AnnotationsManagement"];
            label13.FontSize = 16.0;
            label13.Height = 30.0;
            label13.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            label13.VerticalContentAlignment = VerticalAlignment.Center;
            label13.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header13 = label13;
            System.Windows.Controls.Label label14 = new System.Windows.Controls.Label();
            label14.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["ImageInformation"];
            label14.FontSize = 16.0;
            label14.Height = 30.0;
            label14.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            label14.VerticalContentAlignment = VerticalAlignment.Center;
            label14.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header14 = label14;
            System.Windows.Controls.Label label15 = new System.Windows.Controls.Label();
            label15.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Label"];
            label15.FontSize = 16.0;
            label15.Height = 30.0;
            label15.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label15.VerticalContentAlignment = VerticalAlignment.Center;
            label15.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header15 = label15;
            System.Windows.Controls.Label label16 = new System.Windows.Controls.Label();
            label16.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Navigation"];
            label16.FontSize = 16.0;
            label16.Height = 30.0;
            label16.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label16.VerticalContentAlignment = VerticalAlignment.Center;
            label16.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header16 = label16;
            System.Windows.Controls.Label label17 = new System.Windows.Controls.Label();
            label17.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["ScaleRule"];
            label17.FontSize = 16.0;
            label17.Height = 30.0;
            label17.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label17.VerticalContentAlignment = VerticalAlignment.Center;
            label17.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header17 = label17;
            System.Windows.Controls.Label label18 = new System.Windows.Controls.Label();
            label18.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Case"];
            label18.FontSize = 16.0;
            label18.Height = 30.0;
            label18.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label18.VerticalContentAlignment = VerticalAlignment.Center;
            label18.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header18 = label18;
            System.Windows.Controls.Label label19 = new System.Windows.Controls.Label();
            label19.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Delete"];
            label19.FontSize = 20.0;
            label19.Height = 40.0;
            label19.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            label19.VerticalContentAlignment = VerticalAlignment.Center;
            label19.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header19 = label19;
            System.Windows.Controls.Label label20 = new System.Windows.Controls.Label();
            label20.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Edit"];
            label20.FontSize = 20.0;
            label20.Height = 40.0;
            label20.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            label20.VerticalContentAlignment = VerticalAlignment.Center;
            label20.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header20 = label20;
            System.Windows.Controls.Label label21 = new System.Windows.Controls.Label();
            label21.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Autoplay"];
            label21.FontSize = 16.0;
            label21.Height = 30.0;
            label21.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label21.VerticalContentAlignment = VerticalAlignment.Center;
            label21.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header21 = label21;
            System.Windows.Controls.Label label22 = new System.Windows.Controls.Label();
            label22.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Magnifier"];
            label22.FontSize = 16.0;
            label22.Height = 30.0;
            label22.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label22.VerticalContentAlignment = VerticalAlignment.Center;
            label22.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header22 = label22;
            System.Windows.Controls.Label label23 = new System.Windows.Controls.Label();
            label23.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Rotate"];
            label23.FontSize = 16.0;
            label23.Height = 30.0;
            label23.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label23.VerticalContentAlignment = VerticalAlignment.Center;
            label23.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header23 = label23;
            System.Windows.Controls.Label label24 = new System.Windows.Controls.Label();
            label24.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Operateball"];
            label24.FontSize = 16.0;
            label24.Height = 30.0;
            label24.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label24.VerticalContentAlignment = VerticalAlignment.Center;
            label24.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header24 = label24;
            System.Windows.Controls.Label label25 = new System.Windows.Controls.Label();
            label25.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Analysis"];
            label25.FontSize = 16.0;
            label25.Height = 30.0;
            label25.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label25.VerticalContentAlignment = VerticalAlignment.Center;
            label25.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header25 = label25;
            System.Windows.Controls.Label label26 = new System.Windows.Controls.Label();
            label26.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["AngleSyn"];
            label26.FontSize = 16.0;
            label26.Height = 30.0;
            label26.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label26.VerticalContentAlignment = VerticalAlignment.Center;
            label26.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header26 = label26;
            System.Windows.Controls.Label label27 = new System.Windows.Controls.Label();
            label27.Content = "识别";
            label27.FontSize = 16.0;
            label27.Height = 30.0;
            label27.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label27.VerticalContentAlignment = VerticalAlignment.Center;
            label27.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header27 = label27;
            System.Windows.Controls.Label label28 = new System.Windows.Controls.Label();
            label28.Content = "导出";
            label28.FontSize = 16.0;
            label28.Height = 30.0;
            label28.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label28.VerticalContentAlignment = VerticalAlignment.Center;
            label28.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header28 = label28;
            System.Windows.Controls.Label label29 = new System.Windows.Controls.Label();
            label29.Content = "TMA";
            label29.FontSize = 16.0;
            label29.Height = 30.0;
            label29.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label29.VerticalContentAlignment = VerticalAlignment.Center;
            label29.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header29 = label29;
            System.Windows.Controls.Label label30 = new System.Windows.Controls.Label();
            label30.Content = "直径测量";
            label30.FontSize = 16.0;
            label30.Height = 30.0;
            label30.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label30.VerticalContentAlignment = VerticalAlignment.Center;
            label30.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header30 = label30;
            System.Windows.Controls.Label label31 = new System.Windows.Controls.Label();
            label31.Content = "清除";
            label31.FontSize = 16.0;
            label31.Height = 30.0;
            label31.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label31.VerticalContentAlignment = VerticalAlignment.Center;
            label31.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header31 = label31;
            System.Windows.Controls.Label label32 = new System.Windows.Controls.Label();
            label32.Content = "添加区域";
            label32.FontSize = 16.0;
            label32.Height = 30.0;
            label32.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label32.VerticalContentAlignment = VerticalAlignment.Center;
            label32.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header32 = label32;
            System.Windows.Controls.Label label33 = new System.Windows.Controls.Label();
            label33.Content = "图像调整Plus";
            label33.FontSize = 16.0;
            label33.Height = 30.0;
            label33.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            label33.VerticalContentAlignment = VerticalAlignment.Center;
            label33.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header33 = label33;
            System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem();
            menuItem.Header = header;
            System.Windows.Controls.MenuItem menuItem2 = menuItem;
            menuItem2.PreviewMouseLeftButtonDown += LoadDiyCtcRectangle;
            menuItem2.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/ctcrect.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem3 = new System.Windows.Controls.MenuItem();
            menuItem3.Header = header2;
            System.Windows.Controls.MenuItem menuItem4 = menuItem3;
            menuItem4.PreviewMouseLeftButtonDown += LoadLine;
            menuItem4.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/zx.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem5 = new System.Windows.Controls.MenuItem();
            menuItem5.Header = header3;
            System.Windows.Controls.MenuItem menuItem6 = menuItem5;
            menuItem6.PreviewMouseLeftButtonDown += LoadArrowLine;
            menuItem6.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/jt.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem7 = new System.Windows.Controls.MenuItem();
            menuItem7.Header = header4;
            System.Windows.Controls.MenuItem menuItem8 = menuItem7;
            menuItem8.PreviewMouseLeftButtonDown += LoadRectangle;
            menuItem8.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/jx.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem9 = new System.Windows.Controls.MenuItem();
            menuItem9.Header = header5;
            System.Windows.Controls.MenuItem menuItem10 = menuItem9;
            menuItem10.PreviewMouseLeftButtonDown += LoadEllipse;
            menuItem10.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/ty.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem11 = new System.Windows.Controls.MenuItem();
            menuItem11.Header = header6;
            System.Windows.Controls.MenuItem menuItem12 = menuItem11;
            menuItem12.PreviewMouseLeftButtonDown += LoadPolyline;
            menuItem12.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/pen.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem13 = new System.Windows.Controls.MenuItem();
            menuItem13.Header = header7;
            System.Windows.Controls.MenuItem menuItem14 = menuItem13;
            menuItem14.PreviewMouseLeftButtonDown += LoadPin;
            menuItem14.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/pin.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem15 = new System.Windows.Controls.MenuItem();
            menuItem15.Header = header19;
            System.Windows.Controls.MenuItem menuItem16 = menuItem15;
            menuItem16.PreviewMouseLeftButtonDown += DeleteItem;
            menuItem16.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/sc.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem17 = new System.Windows.Controls.MenuItem();
            menuItem17.Header = header9;
            System.Windows.Controls.MenuItem menuItem18 = menuItem17;
            menuItem18.Click += Show_AdjustmentWind;
            menuItem18.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/adjust.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem19 = new System.Windows.Controls.MenuItem();
            menuItem19.Header = header10;
            System.Windows.Controls.MenuItem menuItem20 = menuItem19;
            menuItem20.PreviewMouseLeftButtonDown += ShotScreen;
            menuItem20.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/outImage.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem21 = new System.Windows.Controls.MenuItem();
            menuItem21.Header = header11;
            System.Windows.Controls.MenuItem menuItem22 = menuItem21;
            menuItem22.PreviewMouseLeftButtonDown += Synchronous_Click;
            menuItem22.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/synchronous.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem23 = new System.Windows.Controls.MenuItem();
            menuItem23.Header = header12;
            System.Windows.Controls.MenuItem menuItem24 = menuItem23;
            menuItem24.PreviewMouseLeftButtonDown += NoSynchronous_Click;
            menuItem24.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/synchronous.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem25 = new System.Windows.Controls.MenuItem();
            menuItem25.Header = header15;
            System.Windows.Controls.MenuItem menuItem26 = menuItem25;
            menuItem26.PreviewMouseLeftButtonDown += isSlideLabel;
            menuItem26.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/label.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem27 = new System.Windows.Controls.MenuItem();
            menuItem27.Header = header16;
            System.Windows.Controls.MenuItem menuItem28 = menuItem27;
            menuItem28.PreviewMouseLeftButtonDown += isSlideNav;
            menuItem28.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/nav.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem29 = new System.Windows.Controls.MenuItem();
            menuItem29.Header = header17;
            System.Windows.Controls.MenuItem menuItem30 = menuItem29;
            menuItem30.PreviewMouseLeftButtonDown += isRuleCanvas;
            menuItem30.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/rule.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem31 = new System.Windows.Controls.MenuItem();
            menuItem31.Header = header18;
            System.Windows.Controls.MenuItem menuItem32 = menuItem31;
            menuItem32.PreviewMouseLeftButtonDown += Show_CaseInfoWind;
            menuItem32.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/caseinfo.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem33 = new System.Windows.Controls.MenuItem();
            menuItem33.Header = header13;
            System.Windows.Controls.MenuItem menuItem34 = menuItem33;
            menuItem34.PreviewMouseDown += Show_AnnoListWind;
            menuItem34.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/edit.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem35 = new System.Windows.Controls.MenuItem();
            menuItem35.Header = header14;
            System.Windows.Controls.MenuItem menuItem36 = menuItem35;
            menuItem36.PreviewMouseDown += Show_SlideInfoWind;
            menuItem36.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/ImageInfo.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem37 = new System.Windows.Controls.MenuItem();
            menuItem37.Header = header8;
            System.Windows.Controls.MenuItem menuItem38 = menuItem37;
            menuItem38.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/other.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem39 = new System.Windows.Controls.MenuItem();
            menuItem39.Header = header20;
            System.Windows.Controls.MenuItem menuItem40 = menuItem39;
            menuItem40.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/edit.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem40.Click += Show_AnnoListWind;
            System.Windows.Controls.MenuItem menuItem41 = new System.Windows.Controls.MenuItem();
            menuItem41.Header = header21;
            System.Windows.Controls.MenuItem menuItem42 = menuItem41;
            menuItem42.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/Video.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem42.Click += mi_Video_Click;
            System.Windows.Controls.MenuItem menuItem43 = new System.Windows.Controls.MenuItem();
            menuItem43.Header = header22;
            System.Windows.Controls.MenuItem menuItem44 = menuItem43;
            menuItem44.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/magnifier.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem44.Click += mi_Magnifier_Click;
            System.Windows.Controls.MenuItem menuItem45 = new System.Windows.Controls.MenuItem();
            menuItem45.Header = header23;
            System.Windows.Controls.MenuItem menuItem46 = menuItem45;
            menuItem46.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/Spin.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem46.Click += mi_Spin_Click;
            System.Windows.Controls.MenuItem menuItem47 = new System.Windows.Controls.MenuItem();
            menuItem47.Header = header24;
            System.Windows.Controls.MenuItem menuItem48 = menuItem47;
            menuItem48.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/Dashboard.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem48.Click += mi_Dashboard_Click;
            System.Windows.Controls.MenuItem menuItem49 = new System.Windows.Controls.MenuItem();
            menuItem49.Header = header25;
            System.Windows.Controls.MenuItem menuItem50 = menuItem49;
            menuItem50.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/analytics.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem50.Click += mi_Analysis_Click;
            System.Windows.Controls.MenuItem menuItem51 = new System.Windows.Controls.MenuItem();
            menuItem51.Header = header26;
            System.Windows.Controls.MenuItem menuItem52 = menuItem51;
            menuItem52.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/syn.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem52.Click += mi_AnaSyn_Click;
            System.Windows.Controls.MenuItem menuItem53 = new System.Windows.Controls.MenuItem();
            menuItem53.Header = header27;
            System.Windows.Controls.MenuItem menuItem54 = menuItem53;
            menuItem54.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/tmasb.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem54.Click += Mi_TMA_Click;
            System.Windows.Controls.MenuItem menuItem55 = new System.Windows.Controls.MenuItem();
            menuItem55.Header = header28;
            System.Windows.Controls.MenuItem menuItem56 = menuItem55;
            menuItem56.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/tmaout.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem56.Click += Mi_TMAOut_Click;
            System.Windows.Controls.MenuItem menuItem57 = new System.Windows.Controls.MenuItem();
            menuItem57.Header = header30;
            System.Windows.Controls.MenuItem menuItem58 = menuItem57;
            menuItem58.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/tmaline.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem58.Click += Mi_TMALine_Click;
            System.Windows.Controls.MenuItem menuItem59 = new System.Windows.Controls.MenuItem();
            menuItem59.Header = header31;
            System.Windows.Controls.MenuItem menuItem60 = menuItem59;
            menuItem60.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/tmaclear.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem60.Click += Mi_TMAClear_Click;
            System.Windows.Controls.MenuItem menuItem61 = new System.Windows.Controls.MenuItem();
            menuItem61.Header = header32;
            System.Windows.Controls.MenuItem menuItem62 = menuItem61;
            menuItem62.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/tmaadd.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem62.Click += Mi_TMAFree_Click;
            System.Windows.Controls.MenuItem menuItem63 = new System.Windows.Controls.MenuItem();
            menuItem63.Header = header29;
            System.Windows.Controls.MenuItem menuItem64 = menuItem63;
            menuItem64.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/tma.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            System.Windows.Controls.MenuItem menuItem65 = new System.Windows.Controls.MenuItem();
            menuItem65.Header = header33;
            System.Windows.Controls.MenuItem menuItem66 = menuItem65;
            menuItem66.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("images/tmaadd.png", UriKind.Relative)),
                Width = 30.0,
                Height = 30.0
            };
            menuItem66.Click += mi_ImageProcesshsv_Click;
            Separator newItem = new Separator();
            Separator newItem2 = new Separator();
            System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();
            if (_AnnoListWind.cbo_mc.SelectedIndex != -1)
            {
                contextMenu.Items.Add(menuItem40);
                contextMenu.Items.Add(menuItem16);
                canvasboard.ContextMenu = contextMenu;
            }
            else
            {
                if (CtcCheck == 1)
                {
                    contextMenu.Items.Add(menuItem2);
                }
                contextMenu.Items.Add(menuItem4);
                contextMenu.Items.Add(menuItem6);
                contextMenu.Items.Add(menuItem8);
                contextMenu.Items.Add(menuItem10);
                contextMenu.Items.Add(menuItem12);
                contextMenu.Items.Add(menuItem14);
                menuItem38.Items.Add(menuItem26);
                menuItem38.Items.Add(menuItem28);
                menuItem38.Items.Add(menuItem30);
                menuItem38.Items.Add(menuItem32);
                menuItem38.Items.Add(menuItem34);
                menuItem38.Items.Add(menuItem36);
                menuItem38.Items.Add(menuItem44);
                menuItem38.Items.Add(menuItem46);
                menuItem38.Items.Add(menuItem48);
                contextMenu.Items.Add(newItem);
                contextMenu.Items.Add(menuItem18);
                if (Setting.IsHsv == "1")
                {
                    contextMenu.Items.Add(menuItem66);
                }
                contextMenu.Items.Add(menuItem20);
                contextMenu.Items.Add(menuItem42);
                if (Setting.AnalysisPath != "")
                {
                    contextMenu.Items.Add(menuItem50);
                }
                contextMenu.Items.Add(newItem2);
                contextMenu.Items.Add(menuItem38);
                if (Setting.TabsDic.Count >= 2 && !Setting.IsSynchronous)
                {
                    contextMenu.Items.Add(menuItem22);
                }
                else if (Setting.TabsDic.Count >= 2 && Setting.IsSynchronous)
                {
                    contextMenu.Items.Add(menuItem24);
                }
                if (Setting.TabsDic.Count >= 2)
                {
                    contextMenu.Items.Add(menuItem52);
                }
            }
            msi.ContextMenu = contextMenu;
        }

        private void mi_ImageProcesshsv_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Point mainWindowPoint = GetMainWindowPoint();
            _HSVWindow.Top = mainWindowPoint.Y + (base.ActualHeight - _HSVWindow.Height) / 2.0;
            _HSVWindow.Left = mainWindowPoint.X + (base.ActualWidth - _HSVWindow.Width) / 2.0;
            _HSVWindow.Show();
            _HSVWindow.Activate();
        }

        private void Mi_TMAFree_Click(object sender, RoutedEventArgs e)
        {
            double num = 100.0;
            double num2 = 100.0;
            if (Tmaobjectlist.Count > 0)
            {
                myTmaRectangle myTmaRectangle = Tmaobjectlist[0];
                System.Windows.Point point = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentStart);
                System.Windows.Point point2 = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentEnd);
                num = Math.Abs(point2.X - point.X);
                num2 = Math.Abs(point2.Y - point.Y);
            }
            UnLoadHandle();
            _myTmaRectangle = new myTmaRectangle(alc, canvasboard, msi, objectlist, SlideZoom, Calibration, Tmaobjectlist);
            _myTmaRectangle.m_Width = (int)num;
            _myTmaRectangle.m_Height = (int)num2;
            _myTmaRectangle.m_Num = Tmaobjectlist.Count + 1;
            _myTmaRectangle.load();
            _myTmaRectangle.FinishEvent += _myTmaRectangle_FinishEvent;
        }

        private void _myTmaRectangle_FinishEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Tmaobjectlist.Count == 0)
            {
                return;
            }
            List<myTmaRectangle> list = new List<myTmaRectangle>();
            List<myTmaRectangle> list2 = Tmaobjectlist.OrderBy((myTmaRectangle s) => s.CurrentStart.Y * Curscale).ToList();
            Dictionary<int, List<myTmaRectangle>> dictionary = new Dictionary<int, List<myTmaRectangle>>();
            List<myTmaRectangle> list3 = new List<myTmaRectangle>();
            double num = Math.Abs(Tmaobjectlist[0].CurrentEnd.Y - Tmaobjectlist[0].CurrentStart.Y) / 3.0;
            int num2 = 0;
            for (int i = 0; i < list2.Count; i++)
            {
                if (i < list2.Count - 1)
                {
                    double y = list2[i].CurrentEnd.Y;
                    double y2 = list2[i + 1].CurrentEnd.Y;
                    double num3 = Math.Abs(y - y2);
                    if (num3 > num)
                    {
                        list3.Add(list2[i]);
                        dictionary.Add(num2, list3);
                        num2++;
                        list3 = new List<myTmaRectangle>();
                    }
                    else
                    {
                        list3.Add(list2[i]);
                    }
                }
                else
                {
                    list3.Add(list2[i]);
                    dictionary.Add(num2, list3);
                }
            }
            foreach (KeyValuePair<int, List<myTmaRectangle>> item in dictionary)
            {
                List<myTmaRectangle> collection = item.Value.OrderBy((myTmaRectangle s) => s.CurrentStart.X * Curscale).ToList();
                list.AddRange(collection);
            }
            Mi_TMAClear_Click(null, null);
            Tmaobjectlist.Clear();
            for (int num4 = list.Count - 1; num4 >= 0; num4--)
            {
                myTmaRectangle myTmaRectangle = list[num4];
                double x = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentStart).X;
                double y3 = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentStart).Y;
                double x2 = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentEnd).X;
                double y4 = myTmaRectangle.MsiToCanvas(myTmaRectangle.CurrentEnd).Y;
                double num5 = Math.Abs(x - x2);
                double num6 = Math.Abs(y3 - y4);
                x = Math.Min(x, x2);
                y3 = Math.Min(y3, y4);
                double left = x;
                double top = y3;
                double width = num5;
                double height = num6;
                myTmaRectangle myTmaRectangle2 = new myTmaRectangle(alc, canvasboard, msi, objectlist, SlideZoom, Calibration, Tmaobjectlist);
                myTmaRectangle2.DrawRect(left, top, width, height, (num4 + 1).ToString(), myTmaRectangle.AnnotationDescription);
            }
            RegisterMsiEvents();
            nav.IsHitTestVisible = true;
            _AnnoListWind.cbo_mc.SelectedIndex = -1;
        }

        private void Mi_TMAClear_Click(object sender, RoutedEventArgs e)
        {
            List<AnnotationBase> list = new List<AnnotationBase>();
            foreach (AnnotationBase item in objectlist)
            {
                if (item.AnnotationType == AnnotationType.myTmaLine || item.AnnotationType == AnnotationType.TmaRectangle)
                {
                    list.Add(item);
                }
            }
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                list[i].DeleteItem();
            }
            list.Clear();
        }

        private void Mi_TMALine_Click(object sender, RoutedEventArgs e)
        {
            UnLoadHandle();
            if (_myTmaLine != null)
            {
                _myTmaLine.DeleteItem();
            }
            _myTmaLine = new myTmaLine(alc, canvasboard, msi, objectlist, SlideZoom, Calibration);
            _myTmaLine.FinishEvent += FinishEvent3;
        }

        private void FinishEvent3(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RegisterMsiEvents();
            nav.IsHitTestVisible = true;
        }

        private void Mi_TMAOut_Click(object sender, RoutedEventArgs e)
        {
            if (Tmaobjectlist.Count != 0)
            {
                BarckgroundworkerBtn = new BackgroundWorker();
                BarckgroundworkerBtn.WorkerSupportsCancellation = true;
                BarckgroundworkerBtn.DoWork += BarckgroundworkerBtn_DoWork;
                BarckgroundworkerBtn.RunWorkerAsync();
                _TMAMessageWind = new MessageWind(MessageBoxButton.OK, System.Windows.Application.Current.MainWindow, "TMA导出中", ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Prompt"], MessageBoxIcon.Exclamation, false);
                _TMAMessageWind._OKButton.Click += _OKButton_Click;
                _TMAMessageWind._OKButton.Content = "停止导出";
                _TMAMessageWind.ShowDialog();
            }
        }

        private void _OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (BarckgroundworkerBtn != null)
            {
                _TMAMessageWind.Close();
                BarckgroundworkerBtn.CancelAsync();
            }
        }

        private void BarckgroundworkerBtn_DoWork(object sender, DoWorkEventArgs e)
        {
            string text = FilePath.Replace(".kfb", "");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            int i = 1;
            foreach (myTmaRectangle item in Tmaobjectlist)
            {
                if (item.AnnotationType == AnnotationType.TmaRectangle)
                {
                    if (BarckgroundworkerBtn.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    try
                    {
                        double num = 0.5;
                        if (SlideZoom == 20)
                        {
                            num = 1.0;
                        }
                        double num2 = item.CurrentStart.X * (double)SlideZoom * num;
                        double num3 = item.CurrentStart.Y * (double)SlideZoom * num;
                        double num4 = item.CurrentEnd.X * (double)SlideZoom * num;
                        double num5 = item.CurrentEnd.Y * (double)SlideZoom * num;
                        int x = (int)num2;
                        int y = (int)num3;
                        int width = (int)num4 - (int)num2;
                        int height = (int)num5 - (int)num3;
                        int nDataLength = 0;
                        byte[] array = new byte[0];
                        DllImageFuc.GetImageDataRoiFunc(InfoStruct, (float)((double)SlideZoom * num), x, y, width, height, out IntPtr datas, ref nDataLength, true);
                        array = new byte[nDataLength];
                        if (datas != IntPtr.Zero)
                        {
                            Marshal.Copy(datas, array, 0, nDataLength);
                        }
                        DllImageFuc.DeleteImageDataFunc(datas);
                        MemoryStream memoryStream = new MemoryStream(array);
                        Bitmap bitmap = new Bitmap(memoryStream);
                        string text2 = "(" + item.AnnotationDescription + ")";
                        if (item.AnnotationDescription == "")
                        {
                            text2 = "";
                        }
                        bitmap.Save(text + "//" + i + text2 + ".jpg");
                        bitmap.Dispose();
                        memoryStream.Dispose();
                        array = null;
                        i++;
                        base.Dispatcher.Invoke((Action)delegate
                        {
                            if (_TMAMessageWind != null)
                            {
                                _TMAMessageWind.Message = i.ToString() + "/" + Tmaobjectlist.Count;
                            }
                        }, new object[0]);
                    }
                    catch (Exception value)
                    {
                        Console.WriteLine(value);
                    }
                }
            }
            base.Dispatcher.Invoke((Action)delegate
            {
                if (_TMAMessageWind != null)
                {
                    _TMAMessageWind.Hide();
                }
            }, new object[0]);
            Process.Start(text);
        }

        private void Mi_TMA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mi_TMAClear_Click(null, null);
                double num = 0.0;
                if (_myTmaLine != null)
                {
                    double x = _myTmaLine.CurrentStart.X;
                    double y = _myTmaLine.CurrentStart.Y;
                    double x2 = _myTmaLine.CurrentEnd.X;
                    double y2 = _myTmaLine.CurrentEnd.Y;
                    double num2 = Math.Abs(x - x2);
                    double num3 = Math.Abs(y - y2);
                    double num4 = Math.Sqrt(num2 * num2 + num3 * num3);
                    num = num4 / 2.0;
                }
                double num5 = 0.0;
                double num6 = 0.0;
                double num7 = ImageW / ((double)SlideZoom * 1.0);
                double num8 = ImageH / ((double)SlideZoom * 1.0);
                int x3 = (int)num5;
                int y3 = (int)num6;
                int width = (int)num7 - (int)num5;
                int height = (int)num8 - (int)num6;
                int nDataLength = 0;
                byte[] array = new byte[0];
                DllImageFuc.GetImageDataRoiFunc(InfoStruct, 1f, x3, y3, width, height, out IntPtr datas, ref nDataLength, true);
                array = new byte[nDataLength];
                if (datas != IntPtr.Zero)
                {
                    Marshal.Copy(datas, array, 0, nDataLength);
                }
                DllImageFuc.DeleteImageDataFunc(datas);
                MemoryStream stream = new MemoryStream(array);
                Bitmap bitmap = new Bitmap(stream);
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string str = System.IO.Path.Combine(folderPath, "KFBIO\\K-Viewer");
                string text = str + "\\Tma";
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                }
                bitmap.Save(text + "/1.jpg");
                bitmap.Dispose();
                string pSrcImagePath = text + "\\1.jpg";
                DllImageFuc.TMARecS(pSrcImagePath, (int)num);
                ListTma = ReadCSV(text + "\\TMAROI.csv");
                if (ListTma.Count != 0)
                {
                    List<string[]> list = new List<string[]>();
                    ListTma = ListTma.OrderBy((string[] s) => int.Parse(s[1])).ToList();
                    Dictionary<int, List<string[]>> dictionary = new Dictionary<int, List<string[]>>();
                    List<string[]> list2 = new List<string[]>();
                    double num9 = double.Parse(ListTma[0][2].ToString()) / 3.0;
                    int num10 = 0;
                    for (int i = 0; i < ListTma.Count; i++)
                    {
                        if (i < ListTma.Count - 1)
                        {
                            double num11 = double.Parse(ListTma[i][1].ToString());
                            double num12 = double.Parse(ListTma[i + 1][1].ToString());
                            double num13 = Math.Abs(num11 - num12);
                            if (num13 > num9)
                            {
                                list2.Add(ListTma[i]);
                                dictionary.Add(num10, list2);
                                num10++;
                                list2 = new List<string[]>();
                            }
                            else
                            {
                                list2.Add(ListTma[i]);
                            }
                        }
                        else
                        {
                            list2.Add(ListTma[i]);
                            dictionary.Add(num10, list2);
                        }
                    }
                    foreach (KeyValuePair<int, List<string[]>> item in dictionary)
                    {
                        List<string[]> collection = item.Value.OrderBy((string[] s) => int.Parse(s[0])).ToList();
                        list.AddRange(collection);
                    }
                    for (int num14 = list.Count - 1; num14 >= 0; num14--)
                    {
                        double num15 = double.Parse(list[num14][2].ToString()) * 0.3 * 0.0;
                        double num16 = double.Parse(list[num14][3].ToString()) * 0.3 * 0.0;
                        double num17 = (double.Parse(list[num14][0].ToString()) - num15) * Curscale;
                        double num18 = (double.Parse(list[num14][1].ToString()) - num16) * Curscale;
                        double width2 = (double.Parse(list[num14][2].ToString()) + num15 * 2.0) * Curscale;
                        double height2 = (double.Parse(list[num14][3].ToString()) + num16 * 2.0) * Curscale;
                        myTmaRectangle myTmaRectangle = new myTmaRectangle(alc, canvasboard, msi, objectlist, SlideZoom, Calibration, Tmaobjectlist);
                        myTmaRectangle.DrawRect(0.0 - msi.ZoomableCanvas.Offset.X + num17, 0.0 - msi.ZoomableCanvas.Offset.Y + num18, width2, height2, (num14 + 1).ToString(), "");
                    }
                    _AnnoListWind.cbo_mc.SelectedIndex = -1;
                }
            }
            catch
            {
            }
        }

        public static List<string[]> ReadCSV(string filePathName)
        {
            List<string[]> list = new List<string[]>();
            StreamReader streamReader = new StreamReader(filePathName);
            string text = "";
            while (text != null)
            {
                text = streamReader.ReadLine();
                if (text != null && text.Length > 0)
                {
                    list.Add(text.Split(','));
                }
            }
            streamReader.Close();
            return list;
        }

        public void Rotate0()
        {
            RotateTransform renderTransform = new RotateTransform(0.0);
            _RotateViewer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            _RotateViewer.Btn_AngleLine.RenderTransform = renderTransform;
            _RotateViewer.lbl_Angle.Content = 0 + "°";
            System.Windows.Point center = new System.Windows.Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            System.Windows.Point p = new System.Windows.Point(x, y);
            System.Windows.Point point = KCommon.PointRotate(center, p, 360.0);
            Canvas.SetLeft(_RotateViewer.ThumbAngle, point.X - 7.5);
            Canvas.SetTop(_RotateViewer.ThumbAngle, point.Y - 7.5);
            MsiRotate(0.0);
        }

        private void mi_AnaSyn_Click(object sender, RoutedEventArgs e)
        {
            Setting.IsSynchronous = false;
            Rotate0();
            int nDegree = 0;
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string str = System.IO.Path.Combine(folderPath, "KFBIO\\K-Viewer");
            string text = str + "\\Syn";
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            BitmapImage source = LoadImage(InfoStruct.DataFilePTR, 8, 0, 0);
            JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
            jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(source));
            FileStream fileStream = new FileStream(text + "\\1.jpg", FileMode.Create, FileAccess.ReadWrite);
            jpegBitmapEncoder.Save(fileStream);
            fileStream.Close();
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if ((Mainpage)item.Value != this)
                {
                    int dataFilePTR = ((Mainpage)item.Value).InfoStruct.DataFilePTR;
                    BitmapImage source2 = ((Mainpage)item.Value).LoadImage(dataFilePTR, 8, 0, 0);
                    jpegBitmapEncoder = new JpegBitmapEncoder();
                    jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(source2));
                    fileStream = new FileStream(text + "/2.jpg", FileMode.Create, FileAccess.ReadWrite);
                    jpegBitmapEncoder.Save(fileStream);
                    fileStream.Close();
                    DllImageFuc.ImageMatchRotate(text + "/1.jpg", text + "/2.jpg", ref nDegree);
                    nDegree = 360 - nDegree;
                    if (nDegree < 360)
                    {
                        nDegree += 360;
                    }
                    if (nDegree >= 360)
                    {
                        nDegree -= 360;
                    }
                    RotateTransform renderTransform = new RotateTransform(nDegree);
                    ((Mainpage)item.Value)._RotateViewer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                    ((Mainpage)item.Value)._RotateViewer.Btn_AngleLine.RenderTransform = renderTransform;
                    ((Mainpage)item.Value)._RotateViewer.lbl_Angle.Content = nDegree + "°";
                    System.Windows.Point center = new System.Windows.Point(40.0, 40.0);
                    double x = 39.0;
                    double y = 5.5;
                    System.Windows.Point p = new System.Windows.Point(x, y);
                    System.Windows.Point point = KCommon.PointRotate(center, p, 360 - nDegree);
                    Canvas.SetLeft(((Mainpage)item.Value)._RotateViewer.ThumbAngle, point.X - 7.5);
                    Canvas.SetTop(((Mainpage)item.Value)._RotateViewer.ThumbAngle, point.Y - 7.5);
                    ((Mainpage)item.Value).MsiRotate(nDegree);
                }
            }
        }

        private void mi_Dashboard_Click(object sender, RoutedEventArgs e)
        {
            if (Canvas_Operateball.Visibility == Visibility.Visible)
            {
                Canvas_Operateball.Visibility = Visibility.Collapsed;
            }
            else
            {
                Canvas_Operateball.Visibility = Visibility.Visible;
            }
        }

        private void mi_Spin_Click(object sender, RoutedEventArgs e)
        {
            if (_RotateViewer.Visibility == Visibility.Visible)
            {
                _RotateViewer.Visibility = Visibility.Collapsed;
            }
            else
            {
                _RotateViewer.Visibility = Visibility.Visible;
            }
        }

        private void mi_Magnifier_Click(object sender, RoutedEventArgs e)
        {
            if (Canv_Magnifier.Visibility == Visibility.Visible)
            {
                Canv_Magnifier.Visibility = Visibility.Collapsed;
                _Magnifiertimer.Stop();
            }
            else
            {
                Canv_Magnifier.Visibility = Visibility.Visible;
                _Magnifiertimer.Start();
            }
        }

        private void mi_Analysis_Click(object sender, RoutedEventArgs e)
        {
            if (_myAnalysis != null)
            {
                _myAnalysis.unload();
            }
            _myAnalysis = new myAnalysis(alc, CanvasAnalysis, msi, objectlist, SlideZoom, Calibration);
            _myAnalysis.RectSetPara(left_rect, right_rect, bottom_rect, top_rect);
            _myAnalysis.FinishEvent += _myAnalysis_FinishEvent;
            _myAnalysis.CloseFinishEvent += _myAnalysis_CloseFinishEvent;
            base.Cursor = System.Windows.Input.Cursors.Pen;
        }

        private void _myAnalysis_CloseFinishEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            base.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void _myAnalysis_FinishEvent(object sender, System.Windows.Input.MouseEventArgs e)
        {
            base.Cursor = System.Windows.Input.Cursors.Arrow;
            System.Windows.Point originStart = ((myAnalysis)sender).OriginStart;
            double num = msi.ZoomableCanvas.Offset.X + originStart.X;
            double num2 = msi.ZoomableCanvas.Offset.Y + originStart.Y;
            double num3 = num * (double)SlideZoom / Curscale;
            double num4 = num2 * (double)SlideZoom / Curscale;
            double num5 = ((myAnalysis)sender).m_rectangle.Width * (double)SlideZoom / Curscale;
            double num6 = ((myAnalysis)sender).m_rectangle.Height * (double)SlideZoom / Curscale;
            string[] array = new string[5];
            string filePath = FilePath;
            array[0] = filePath.Replace(" ", "$##$");
            array[1] = ((int)num3).ToString();
            array[2] = ((int)num4).ToString();
            array[3] = ((int)num5).ToString();
            array[4] = ((int)num6).ToString();
            StartProcess(Setting.AnalysisPath, array);
        }

        public void KillProcess(string FileName)
        {
            Process[] processesByName = Process.GetProcessesByName(FileName);
            Process[] array = processesByName;
            int num = 0;
            if (num < array.Length)
            {
                Process process = array[num];
                process.Kill();
                process.WaitForExit();
            }
        }

        public bool StartProcess(string filename, string[] args)
        {
            try
            {
                if (myprocess != null && !myprocess.HasExited)
                {
                    myprocess.Kill();
                    myprocess.WaitForExit();
                }
                else if (File.Exists(filename))
                {
                    FileInfo fileInfo = new FileInfo(filename);
                    KillProcess(fileInfo.Name.Replace(".exe", ""));
                }
                string text = "";
                foreach (string str in args)
                {
                    text = text + str + " ";
                }
                text = text.Trim();
                myprocess = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo(filename, text);
                myprocess.StartInfo = startInfo;
                myprocess.StartInfo.UseShellExecute = false;
                myprocess.Start();
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("启动出错！原因：" + ex.Message);
            }
            return false;
        }

        private void mi_Video_Click(object sender, RoutedEventArgs e)
        {
            Setting.IsSynchronous = false;
            RotateTransform renderTransform = new RotateTransform(0.0);
            _RotateViewer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            _RotateViewer.Btn_AngleLine.RenderTransform = renderTransform;
            _RotateViewer.lbl_Angle.Content = 0 + "°";
            System.Windows.Point center = new System.Windows.Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            System.Windows.Point p = new System.Windows.Point(x, y);
            System.Windows.Point point = KCommon.PointRotate(center, p, 360.0);
            Canvas.SetLeft(_RotateViewer.ThumbAngle, point.X - 7.5);
            Canvas.SetTop(_RotateViewer.ThumbAngle, point.Y - 7.5);
            MsiRotate(0.0);
            System.Windows.Point mainWindowPoint = GetMainWindowPoint();
            _VideoWind.Top = mainWindowPoint.Y + (base.ActualHeight - _VideoWind.Height) / 2.0;
            _VideoWind.Left = mainWindowPoint.X + (base.ActualWidth - _VideoWind.Width) / 2.0;
            _VideoWind.Show();
            _VideoWind.Activate();
        }

        private void Btn_Play_Click(object sender, RoutedEventArgs e)
        {
            if (Video_Status != 2)
            {
                Video_Status = 2;
                timer.Interval = new TimeSpan(0, 0, 0, 0, Setting.Move_Video_Speed);
                timer.Start();
                _VideoWind.Btn_Play.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Stop"];
                return;
            }
            Video_Status = 1;
            if (timer != null)
            {
                timer.Stop();
                CurOffsetX = -1.0;
                _VideoWind.Btn_Play.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Play"];
            }
        }

        private void Video_Drection(string drection)
        {
            System.Windows.Point offset = msi.ZoomableCanvas.Offset;
            switch (drection)
            {
                case "up":
                    offset.Y += Setting.Move_Video_Step;
                    break;
                case "down":
                    offset.Y += -Setting.Move_Video_Step;
                    break;
                case "left":
                    offset.X += msi.ActualWidth;
                    break;
                case "right":
                    offset.X += 0.0 - msi.ActualWidth;
                    break;
            }
            msi.ZoomableCanvas.Offset = offset;
            msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
        }

        public void timer_Tick(object sender, EventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, Setting.Move_Video_Speed);
            System.Windows.Point offset = msi.ZoomableCanvas.Offset;
            double num = ImageH / (double)SlideZoom * Curscale - offset.Y;
            double num2 = ImageW / (double)SlideZoom * Curscale - offset.X;
            if (num >= msi.ActualHeight && VDrection == "up")
            {
                Video_Drection(VDrection);
                CruDrection = "up";
            }
            if (num < msi.ActualHeight && VDrection == "up")
            {
                if (CurOffsetX == -1.0)
                {
                    CurOffsetX = offset.X;
                }
                if (offset.X - CurOffsetX >= msi.ActualWidth)
                {
                    CurOffsetX = -1.0;
                    VDrection = "down";
                }
                else
                {
                    Video_Drection(HDrection);
                    CruDrection = "left";
                }
            }
            if (offset.Y > 0.0 && VDrection == "down")
            {
                Video_Drection(VDrection);
                CruDrection = "down";
            }
            if (offset.Y <= 0.0 && VDrection == "down" && num2 >= 0.0)
            {
                if (CurOffsetX == -1.0)
                {
                    CurOffsetX = offset.X;
                }
                if (offset.X - CurOffsetX >= msi.ActualWidth)
                {
                    CurOffsetX = -1.0;
                    VDrection = "up";
                }
                else if (offset.X - CurOffsetX < msi.ActualWidth)
                {
                    Video_Drection(HDrection);
                    CruDrection = "left";
                }
            }
            if (num2 < msi.ActualWidth && num < msi.ActualHeight && CruDrection == "up")
            {
                timer.Stop();
                VDrection = "up";
                _VideoWind.Btn_Play.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Play"];
                CurOffsetX = -1.0;
            }
            if (offset.Y <= 0.0 && CruDrection == "down" && num2 < msi.ActualWidth)
            {
                timer.Stop();
                Video_Status = 3;
                VDrection = "up";
                _VideoWind.Btn_Play.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Play"];
                CurOffsetX = -1.0;
            }
            if (offset.X > ImageW / (double)SlideZoom * Curscale)
            {
                timer.Stop();
                Video_Status = 3;
                VDrection = "up";
                _VideoWind.Btn_Play.Content = ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Play"];
                CurOffsetX = -1.0;
            }
        }

        private void Synchronous_Click(object sender, RoutedEventArgs e)
        {
            Setting.IsSynchronous = true;
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if ((Mainpage)item.Value != this)
                {
                    ((Mainpage)item.Value).ZoomRatio(msi.ZoomableCanvas.Scale * (double)SlideZoom);
                }
            }
        }

        private void NoSynchronous_Click(object sender, RoutedEventArgs e)
        {
            Setting.IsSynchronous = false;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Fit();
            Refresh();
            ArcMenu();
        }

        public double ScaleToViewportWidth(double Scale, double viewportActualWidth, double slideWidth)
        {
            double num = viewportActualWidth / slideWidth;
            return num / Scale;
        }

        public void LoadMsi(string filename)
        {
            IniFile(filename);
            int khiImageHeight = 1;
            int khiImageWidth = 2;
            int khiScanScale = 3;
            float khiSpendTime = 0f;
            double khiScanTime = 0.0;
            float khiImageCapRes = 0f;
            if (filename.IndexOf(".kfb") != -1)
            {
                DllImageFuc.GetScanLevelInfoFunc(ref InfoStruct, ref nCurLevel, ref nTotalLevel);
                if (nTotalLevel > 2)
                {
                    if (nTotalLevel % 2 == 0)
                    {
                        MinLevel = -nTotalLevel / 2 + 1;
                        MaxLevel = nTotalLevel / 2;
                    }
                    else
                    {
                        MinLevel = -(nTotalLevel - 1) / 2;
                        MaxLevel = (nTotalLevel - 1) / 2;
                    }
                    x3dSlider.Zvalue.Content = nCurLevel;
                    if (nCurLevel == 0)
                    {
                        LevelFilePath = filename;
                    }
                    else
                    {
                        LevelFilePath = filename.Replace("_" + nCurLevel + ".kfb", ".kfb");
                    }
                    if (CheckAllLevel())
                    {
                        x3dSlider.Visibility = Visibility.Visible;
                    }
                    if (filename == LevelFilePath && nCurLevel != 0)
                    {
                        x3dSlider.Visibility = Visibility.Collapsed;
                    }
                }
            }
            _DllImageFuc.CkGetHeaderInfoFunc(InfoStruct, ref khiImageHeight, ref khiImageWidth, ref khiScanScale, ref khiSpendTime, ref khiScanTime, ref khiImageCapRes, ref TileSize);
            if (TileSize == 0)
            {
                TileSize = 256;
            }
            msi.Source = new MagicZoomTileSource1(khiImageWidth, khiImageHeight, TileSize, 0, InfoStruct, khiScanScale, msi);
            if ((double)khiImageCapRes == 0.0)
            {
                switch (khiScanScale)
                {
                    case 20:
                        Calibration = 0.5;
                        break;
                    case 40:
                        Calibration = 0.2439;
                        break;
                }
            }
            else
            {
                Calibration = khiImageCapRes;
                if (khiScanScale == 40)
                {
                    if (Setting.Calibration40 != 1.0)
                    {
                        Setting.MargPara = Setting.Calibration40;
                    }
                    if (Setting.CalibrationX40 != 1.0)
                    {
                        Calibration = Setting.CalibrationX40;
                    }
                }
                if (khiScanScale == 20)
                {
                    if (Setting.Calibration20 != 1.0)
                    {
                        Setting.MargPara = Setting.Calibration20;
                    }
                    if (Setting.CalibrationX20 != 1.0)
                    {
                        Calibration = Setting.CalibrationX20;
                    }
                }
            }
            SlideZoom = khiScanScale;
            ImageW = khiImageWidth;
            ImageH = khiImageHeight;
        }

        public bool CheckAllLevel()
        {
            for (int i = MinLevel; i < MinLevel + nTotalLevel; i++)
            {
                string path = LevelFilePath;
                if (i != 0)
                {
                    path = LevelFilePath.Replace(".kfb", "_" + i + ".kfb");
                }
                if (File.Exists(path) && i != nCurLevel)
                {
                    return true;
                }
            }
            return false;
        }

        private void Fit()
        {
            double num = Bg.ActualWidth;
            double num2 = Bg.ActualHeight;
            if (Rotate != 0.0)
            {
                num -= 2.0 * Setting.AngelMsiOffset;
                num2 -= 2.0 * Setting.AngelMsiOffset;
            }
            double num3 = Math.Min(num / ImageW, num2 / ImageH);
            System.Windows.Point point = new System.Windows.Point(ImageW * 0.5 * num3 - msi.ActualWidth * 0.5, ImageH * 0.5 * num3 - msi.ActualHeight * 0.5);
            fitratio = num3 * (double)SlideZoom;
            fitx = point.X;
            fity = point.Y;
        }

        private void SynFit()
        {
            double num = Bg.ActualWidth;
            double num2 = Bg.ActualHeight;
            if (Rotate != 0.0)
            {
                num -= 2.0 * Setting.AngelMsiOffset;
                num2 -= 2.0 * Setting.AngelMsiOffset;
            }
            double num3 = Math.Min(num / ImageW, num2 / ImageH);
            System.Windows.Point point = new System.Windows.Point(ImageW * 0.5 * num3 - msi.ActualWidth * 0.5, ImageH * 0.5 * num3 - msi.ActualHeight * 0.5);
            fitratio = num3 * (double)SlideZoom;
            fitx = point.X;
            fity = point.Y;
            System.Windows.Point offset = new System.Windows.Point(fitx, fity);
            ZoomRatio(fitratio);
            msi.ZoomableCanvas.Offset = offset;
            msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
        }

        public void isSlideLabel(object sender, RoutedEventArgs e)
        {
            if (Image_lable.Visibility == Visibility.Visible)
            {
                Image_lable.Visibility = Visibility.Collapsed;
            }
            else
            {
                Image_lable.Visibility = Visibility.Visible;
            }
        }

        public void isSlideNav(object sender, RoutedEventArgs e)
        {
            if (nav.Visibility == Visibility.Visible)
            {
                nav.Visibility = Visibility.Collapsed;
            }
            else
            {
                nav.Visibility = Visibility.Visible;
            }
        }

        public void isRuleCanvas(object sender, RoutedEventArgs e)
        {
            if (RuleCanvas.Visibility == Visibility.Visible)
            {
                RuleCanvas.Visibility = Visibility.Collapsed;
                return;
            }
            RuleCanvas.Margin = new Thickness(10.0, 0.0, 0.0, 0.0);
            RuleCanvas.Visibility = Visibility.Visible;
        }

        public void UpdateRule()
        {
            double num = (double)SlideZoom / Curscale;
            double num2 = (num <= 1.0) ? 1.0 : ((!(num > 1.0) || !(num < 2.0)) ? (num - num % 2.0) : 1.0);
            double UM = 100.0 * num2;
            if (UM < 1000.0)
            {
                if (UM % 5.0 != 0.0)
                {
                    UM += UM - UM % 5.0;
                }
            }
            else if (UM % 5000.0 != 0.0)
            {
                UM = ((!(UM < 5000.0)) ? (UM + (UM / 5000.0 - UM % 5000.0)) : 5000.0);
            }
            double XS = UM / (num * Calibration);
            CheckLimtRule(ref XS, ref UM);
            UpRuleLayout(XS, UM);
        }

        public void CheckLimtRule(ref double XS, ref double UM)
        {
            if (XS >= (double)(MinRuleV + MinRuleV / 2))
            {
                XS /= 2.0;
                UM /= 2.0;
                if (XS >= (double)(MinRuleV + MinRuleV / 2))
                {
                    CheckLimtRule(ref XS, ref UM);
                }
            }
            if (XS < 150.0)
            {
                XS *= 2.0;
                UM *= 2.0;
            }
        }

        public void UpRuleLayout(double XS, double UM)
        {
            double num = 0.0;
            double num2 = 0.0;
            string empty = string.Empty;
            if (UM >= 1000.0)
            {
                num = UM / 5000.0;
                empty = "mm";
            }
            else
            {
                num = UM / 5.0;
                empty = "μm";
            }
            num2 = XS / 5.0;
            RLine_1.X1 = num2 * 1.0;
            RLine_1.X2 = num2 * 1.0;
            RLine_2.X1 = num2 * 2.0;
            RLine_2.X2 = num2 * 2.0;
            RLine_3.X1 = num2 * 3.0;
            RLine_3.X2 = num2 * 3.0;
            RLine_4.X1 = num2 * 4.0;
            RLine_4.X2 = num2 * 4.0;
            RLine_5.X1 = num2 * 5.0;
            RLine_5.X2 = num2 * 5.0;
            RLine_L.X2 = num2 * 5.0;
            Rtxt_1.Content = Math.Round(num * 1.0, 2);
            Rtxt_2.Content = Math.Round(num * 2.0, 2);
            Rtxt_3.Content = Math.Round(num * 3.0, 2);
            Rtxt_4.Content = Math.Round(num * 4.0, 2);
            Rtxt_5.Content = Math.Round(num * 5.0, 2) + empty;
            Rtxt_1.Margin = new Thickness(num2 * 1.0 - (double)GetMargin(Math.Round(num * 1.0, 2).ToString()), 10.0, 0.0, 0.0);
            Rtxt_2.Margin = new Thickness(num2 * 2.0 - (double)GetMargin(Math.Round(num * 2.0, 2).ToString()), 10.0, 0.0, 0.0);
            Rtxt_3.Margin = new Thickness(num2 * 3.0 - (double)GetMargin(Math.Round(num * 3.0, 2).ToString()), 10.0, 0.0, 0.0);
            Rtxt_4.Margin = new Thickness(num2 * 4.0 - (double)GetMargin(Math.Round(num * 4.0, 2).ToString()), 10.0, 0.0, 0.0);
            Rtxt_5.Margin = new Thickness(num2 * 5.0 - (double)GetMargin(Math.Round(num * 5.0, 2).ToString()), 10.0, 0.0, 0.0);
        }

        public int GetMargin(string V)
        {
            int result = 0;
            switch (V.Length)
            {
                case 1:
                    result = 10;
                    break;
                case 2:
                    result = 15;
                    break;
                case 3:
                    result = 20;
                    break;
                case 4:
                    result = 25;
                    break;
            }
            return result;
        }

        public void Show_SlideInfoWind(object sender, RoutedEventArgs e)
        {
            System.Windows.Point mainWindowPoint = GetMainWindowPoint();
            _SlideInfoWind.Top = mainWindowPoint.Y + (base.ActualHeight - _SlideInfoWind.Height) / 2.0;
            _SlideInfoWind.Left = mainWindowPoint.X + (base.ActualWidth - _SlideInfoWind.Width) / 2.0;
            _SlideInfoWind.FileName.Text = FilePath;
            _SlideInfoWind.ScanScale.Text = SlideZoom.ToString() + "X";
            double num = Math.Round(KCommon.GetFileSize(FilePath), 2);
            string str = "M";
            if (num >= 1024.0)
            {
                num = Math.Round(num / 1024.0, 2);
                str = "G";
            }
            _SlideInfoWind.FileSize.Text = num.ToString() + str;
            _SlideInfoWind.ImagePixel.Text = ImageW.ToString() + " Pixel ×" + ImageH.ToString() + " Pixel （" + Math.Round(ImageW * Calibration / 1000.0, 2) + "mm * " + Math.Round(ImageH * Calibration / 1000.0, 2) + "mm）";
            _SlideInfoWind.ImageLabel.Source = KCommon.GetKFBLabel(FilePath);
            _SlideInfoWind.ImagePreview.Source = KCommon.GetKFBPreView(FilePath);
            int nYear = 0;
            int nMonth = 0;
            int nDay = 0;
            int nHour = 0;
            int nMiniter = 0;
            int nSecond = 0;
            int nDurHour = 0;
            int nDurMin = 0;
            int nDurSecond = 0;
            DllImageFuc.GetScanTimeDurationFunc(ref InfoStruct, ref nYear, ref nMonth, ref nDay, ref nHour, ref nMiniter, ref nSecond, ref nDurHour, ref nDurMin, ref nDurSecond);
            string s = nYear + "-" + nMonth + "-" + nDay + " " + nHour + ":" + nMiniter + ":" + nSecond;
            string text = nDurHour + ":" + nDurMin + ":" + nDurSecond;
            if (nYear == 0)
            {
                _SlideInfoWind.ScanDate.Text = "无";
                _SlideInfoWind.Durtime.Text = "无";
                _SlideInfoWind.ScanMac.Text = "无";
            }
            else
            {
                _SlideInfoWind.ScanDate.Text = DateTime.Parse(s).ToString();
                _SlideInfoWind.Durtime.Text = text;
                string s2 = new string(' ', 512);
                IntPtr intPtr = Marshal.StringToHGlobalAnsi(s2);
                DllImageFuc.GetMachineSerialNumFunc(ref InfoStruct, intPtr);
                _SlideInfoWind.ScanMac.Text = Marshal.PtrToStringAnsi(intPtr).Trim();
                Marshal.FreeHGlobal(intPtr);
            }
            _SlideInfoWind.Show();
            _SlideInfoWind.Activate();
            IsSlideWind = Visibility.Visible;
        }

        private void _SlideInfoWind_CloseHandler(object sender, RoutedEventArgs e)
        {
            _SlideInfoWind.Visibility = Visibility.Collapsed;
            IsSlideWind = Visibility.Collapsed;
        }

        public void Show_CaseInfoWind(object sender, RoutedEventArgs e)
        {
            _CaseInfoWind.Owner = System.Windows.Application.Current.MainWindow;
            System.Windows.Point point = new System.Windows.Point(0.0, 0.0);
            try
            {
                point = GetMainWindowPoint();
            }
            catch
            {
            }
            _CaseInfoWind.Top = point.Y + (base.ActualHeight - _CaseInfoWind.Height) / 2.0;
            _CaseInfoWind.Left = point.X + (base.ActualWidth - _CaseInfoWind.Width) / 2.0;
            _CaseInfoWind.Show();
            _CaseInfoWind.Activate();
            IsCaseWind = Visibility.Visible;
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            _EditCaseInfoWind = new EditCaseInfoWind();
            _EditCaseInfoWind.Owner = System.Windows.Application.Current.MainWindow;
            System.Windows.Point point = TransformToAncestor(System.Windows.Application.Current.MainWindow).Transform(new System.Windows.Point(0.0, 0.0));
            _EditCaseInfoWind.Top = point.Y + (base.ActualHeight - _EditCaseInfoWind.Height) / 2.0;
            _EditCaseInfoWind.Left = point.X + (base.ActualWidth - _EditCaseInfoWind.Width) / 2.0;
            _EditCaseInfoWind.save.Click += save_Click;
            _EditCaseInfoWind.txt_caseinfo.Text = CaseLine.ToString();
            _EditCaseInfoWind.ShowDialog();
        }

        private void _CaseInfoWind_CloseHandler(object sender, RoutedEventArgs e)
        {
            _CaseInfoWind.Visibility = Visibility.Collapsed;
            IsCaseWind = Visibility.Collapsed;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            if (_EditCaseInfoWind != null)
            {
                _CaseInfoWind._CaseInfo.Inlines.Clear();
                _CaseInfoWind._CaseInfo.Inlines.Add(_EditCaseInfoWind.txt_caseinfo.Text);
                CaseLine.Clear();
                CaseLine.Append(_EditCaseInfoWind.txt_caseinfo.Text);
                FileStream fileStream = new FileStream(TempPath + TempFilename + ".Case", FileMode.OpenOrCreate);
                Encoding encoding = Encoding.GetEncoding("gb2312");
                byte[] bytes = encoding.GetBytes(_EditCaseInfoWind.txt_caseinfo.Text);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
                MessageWind messageWind = new MessageWind(MessageBoxButton.OK, System.Windows.Application.Current.MainWindow, ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["SaveSuccess"], ((DockWindow)System.Windows.Application.Current.MainWindow).languageSetter.LanguageResource["Prompt"], MessageBoxIcon.Exclamation, false);
                messageWind.ShowDialog();
            }
        }

        public void ShotScreen(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMddHHmmss");
            saveFileDialog.DefaultExt = ".jpg";
            saveFileDialog.Filter = "(.jpg)|*.jpg|(.bmp)|*.bmp|(.tiff)|*.tiff|(.png)|*.png";
            if (saveFileDialog.ShowDialog() == true)
            {
                if (saveFileDialog.FilterIndex == 2)
                {
                    SaveToImage(OutBg, saveFileDialog.FileName, ImageFormat.BMP);
                }
                else if (saveFileDialog.FilterIndex == 3)
                {
                    SaveToImage(OutBg, saveFileDialog.FileName, ImageFormat.TIF);
                }
                else if (saveFileDialog.FilterIndex == 4)
                {
                    SaveToImage(OutBg, saveFileDialog.FileName, ImageFormat.PNG);
                }
                else
                {
                    SaveToImage(OutBg, saveFileDialog.FileName, ImageFormat.JPG);
                }
            }
        }

        private void SaveToImage(FrameworkElement p_FrameworkElement, string p_FileName, ImageFormat format)
        {
            double num = Bg.ActualWidth;
            double num2 = Bg.ActualHeight;
            if (Rotate != 0.0)
            {
                num -= 2.0 * Setting.AngelMsiOffset;
                num2 -= 2.0 * Setting.AngelMsiOffset;
            }
            FileStream fileStream = new FileStream(p_FileName, FileMode.Create);
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)num, (int)num2, 96.0, 96.0, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(p_FrameworkElement);
            GenerateImage(renderTargetBitmap, format, fileStream);
            fileStream.Close();
            fileStream.Dispose();
        }

        private void GenerateImage(BitmapSource bitmap, ImageFormat format, Stream destStream)
        {
            BitmapEncoder bitmapEncoder = null;
            switch (format)
            {
                case ImageFormat.JPG:
                    {
                        JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                        jpegBitmapEncoder.QualityLevel = 100;
                        bitmapEncoder = jpegBitmapEncoder;
                        break;
                    }
                case ImageFormat.PNG:
                    bitmapEncoder = new PngBitmapEncoder();
                    break;
                case ImageFormat.BMP:
                    bitmapEncoder = new BmpBitmapEncoder();
                    break;
                case ImageFormat.GIF:
                    bitmapEncoder = new GifBitmapEncoder();
                    break;
                case ImageFormat.TIF:
                    bitmapEncoder = new TiffBitmapEncoder();
                    break;
                default:
                    throw new InvalidOperationException();
            }
            bitmapEncoder.Frames.Add(BitmapFrame.Create(bitmap));
            bitmapEncoder.Save(destStream);
        }

        public static bool IsInteger(string s)
        {
            string pattern = "^\\d*$";
            return Regex.IsMatch(s, pattern);
        }

        public void ShotScreenImage(double Left, double Top, int ViewWidth, int ViewHeight, double scale)
        {
            scale = Math.Round(scale, 2);
            int width = 0;
            int height = 0;
            int num = Math.Max((int)ImageW, (int)ImageH);
            int num2 = IsInteger((Math.Log(num) / Math.Log(2.0)).ToString()) ? ((int)(Math.Log(num) / Math.Log(2.0))) : ((int)(Math.Log(num) / Math.Log(2.0)) + 1);
            int num3 = (int)(ImageW / (double)SlideZoom * scale);
            int num4 = (int)(ImageH / (double)SlideZoom * scale);
            int num5 = Math.Max(num3, num4);
            int num6 = IsInteger((Math.Log(num5) / Math.Log(2.0)).ToString()) ? ((int)(Math.Log(num5) / Math.Log(2.0))) : ((int)(Math.Log(num5) / Math.Log(2.0)) + 1);
            double num7 = (double)SlideZoom / Math.Pow(2.0, num2 - num6);
            double num8 = scale / num7;
            if (scale > (double)SlideZoom)
            {
                num8 = scale / (double)SlideZoom;
            }
            int num9 = (int)((double)TileSize * num8);
            int num10 = (int)((double)TileSize * num8);
            Bitmap bitmap = new Bitmap(ViewWidth, ViewHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(System.Drawing.Color.White);
            int num11 = num3 % num9;
            int num12 = num4 % num10;
            int num13 = (num11 == 0) ? (num3 / num9) : (num3 / num9 + 1);
            int num14 = (num12 == 0) ? (num4 / num10) : (num4 / num10 + 1);
            int num15 = (int)(Left / (double)num9);
            int num16 = (int)(Top / (double)num10);
            int num17 = (Left % (double)num9 == 0.0) ? num9 : (num9 - (int)(Left % ((double)TileSize * num8)));
            int num18 = (Top % (double)num10 == 0.0) ? num10 : (num10 - (int)(Top % ((double)TileSize * num8)));
            int num19 = ((ViewWidth - num17) % num9 == 0) ? ((ViewWidth - num17) / num9) : ((ViewWidth - num17) / num9 + 1);
            int num20 = ((ViewHeight - num18) % num10 == 0) ? ((ViewHeight - num18) / num10) : ((ViewHeight - num18) / num10 + 1);
            for (int i = 0; i < num19 + 1; i++)
            {
                for (int j = 0; j < num20 + 1; j++)
                {
                    if (i + num15 == num13 - 1 && j + num16 == num14 - 1)
                    {
                        if (num12 != 0 && num11 != 0)
                        {
                            width = num11;
                            height = num12;
                        }
                    }
                    else if (i + num15 == num13 - 1)
                    {
                        if (num11 != 0)
                        {
                            width = num11;
                            height = num10;
                        }
                    }
                    else if (j + num16 == num14 - 1)
                    {
                        if (num12 != 0)
                        {
                            width = num9;
                            height = num12;
                        }
                    }
                    else
                    {
                        width = num9;
                        height = num10;
                    }
                    float num21 = 0f;
                    num21 = ((num2 != num6) ? ((float)SlideZoom / (float)Math.Pow(2.0, num2 - num6)) : ((float)SlideZoom));
                    if (num21 > (float)SlideZoom)
                    {
                        num21 = SlideZoom;
                    }
                    int nDataLength = 0;
                    _DllImageFuc.CkGetImageStreamFunc(ref InfoStruct, num21, (num15 + i) * TileSize, (j + num16) * TileSize, ref nDataLength, out IntPtr datas);
                    if (nDataLength == 0)
                    {
                        break;
                    }
                    byte[] array = new byte[nDataLength];
                    if (datas != IntPtr.Zero)
                    {
                        Marshal.Copy(datas, array, 0, nDataLength);
                    }
                    _DllImageFuc.CkDeleteImageDataFunc(datas);
                    MemoryStream stream = new MemoryStream(array);
                    Bitmap original = (Bitmap)System.Drawing.Image.FromStream(stream);
                    Bitmap image = new Bitmap(original, new System.Drawing.Size(width, height));
                    graphics.DrawImage(image, new System.Drawing.Point(num17 + (i - 1) * num9, num18 + (j - 1) * num10));
                }
            }
            graphics.Save();
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + (int)scale + "_" + (int)Left + "_" + (int)Top;
            saveFileDialog.DefaultExt = ".jpg";
            saveFileDialog.Filter = "(.jpg)|*.jpg|(.bmp)|*.bmp|(.tiff)|*.tiff";
            if (saveFileDialog.ShowDialog() == true)
            {
                if (saveFileDialog.FilterIndex == 2)
                {
                    bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                else if (saveFileDialog.FilterIndex == 3)
                {
                    bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                }
                else
                {
                    bitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        private void RuleThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thickness margin = RuleCanvas.Margin;
            RuleCanvas.Margin = new Thickness(margin.Left + e.HorizontalChange, 0.0, 0.0, margin.Bottom - e.VerticalChange);
        }

        private void myThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            int num = 40;
            double num2 = Canvas.GetLeft(myThumb) + e.HorizontalChange;
            double num3 = Canvas.GetTop(myThumb) + e.VerticalChange;
            System.Windows.Point point = new System.Windows.Point(num2, num3);
            System.Windows.Point point2 = new System.Windows.Point(20.0, 20.0);
            double num4 = Math.Sqrt(Math.Abs(point.X - point2.X) * Math.Abs(point.X - point2.X) + Math.Abs(point.Y - point2.Y) * Math.Abs(point.Y - point2.Y));
            if (num4 > (double)num)
            {
                double num5 = 0.0;
                double num6 = 0.0;
                if (num2 > (double)num && num3 < (double)num && num3 > -10.0)
                {
                    num6 = Canvas.GetTop(myThumb) + e.VerticalChange;
                    num5 = Math.Sqrt((double)(num * num) - Math.Abs(num6 - point2.Y) * Math.Abs(num6 - point2.Y)) + point2.X;
                }
                if (num2 < -10.0 && num3 < (double)num && num3 > -10.0)
                {
                    num6 = Canvas.GetTop(myThumb) + e.VerticalChange;
                    num5 = (Math.Sqrt((double)(num * num) - Math.Abs(num6 - point2.Y) * Math.Abs(num6 - point2.Y)) - point2.X) * -1.0;
                }
                if (num3 < -10.0 && num2 < (double)num && num2 > -10.0)
                {
                    num5 = Canvas.GetLeft(myThumb) + e.HorizontalChange;
                    num6 = (Math.Sqrt((double)(num * num) - Math.Abs(num5 - point2.X) * Math.Abs(num5 - point2.X)) - point2.Y) * -1.0;
                }
                if (num3 > (double)num && num2 < (double)num && num2 > -10.0)
                {
                    num5 = Canvas.GetLeft(myThumb) + e.HorizontalChange;
                    num6 = Math.Sqrt((double)(num * num) - Math.Abs(num5 - point2.X) * Math.Abs(num5 - point2.X)) + point2.Y;
                }
                Console.WriteLine(num2 + "," + num3);
                Console.WriteLine(num5 + "," + num6);
                if (num5 != 0.0 && num6 != 0.0)
                {
                    Canvas.SetTop(myThumb, num6);
                    Canvas.SetLeft(myThumb, num5);
                }
            }
            else
            {
                Canvas.SetLeft(myThumb, Canvas.GetLeft(myThumb) + e.HorizontalChange);
                Canvas.SetTop(myThumb, Canvas.GetTop(myThumb) + e.VerticalChange);
            }
            HorizontalChangeX = Canvas.GetLeft(myThumb) + e.HorizontalChange;
            VerticalChangeY = Canvas.GetTop(myThumb) + e.VerticalChange;
            _Operationtimer.Start();
        }

        private void _Operationtimer_Tick(object sender, EventArgs e)
        {
            if (HorizontalChangeX != 20.0 || VerticalChangeY != 20.0)
            {
                System.Windows.Point offset = msi.ZoomableCanvas.Offset;
                System.Windows.Point offset2 = msi.ZoomableCanvas.Offset;
                System.Windows.Point p = new System.Windows.Point(HorizontalChangeX, VerticalChangeY);
                System.Windows.Point center = new System.Windows.Point(20.0, 20.0);
                System.Windows.Point point = KCommon.PointRotate(center, p, Rotate);
                offset2.X += point.X - 20.0;
                offset2.Y += point.Y - 20.0;
                msi.ZoomableCanvas.Offset = offset2;
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                    {
                        if ((Mainpage)item.Value != this)
                        {
                            double x = msi.ZoomableCanvas.Offset.X - offset.X;
                            double y = msi.ZoomableCanvas.Offset.Y - offset.Y;
                            System.Windows.Point p2 = new System.Windows.Point(x, y);
                            System.Windows.Point center2 = new System.Windows.Point(0.0, 0.0);
                            System.Windows.Point p3 = KCommon.PointRotate(center2, p2, 360.0 - Rotate);
                            ((Mainpage)item.Value).IsDw = false;
                            ((Mainpage)item.Value).TmoveSetOffset((Mainpage)item.Value, p3);
                            ((Mainpage)item.Value).ZoomRatio(msi.ZoomableCanvas.Scale * (double)SlideZoom);
                        }
                    }
                }
            }
        }

        private void myThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            Canvas.SetLeft(myThumb, 20.0);
            Canvas.SetTop(myThumb, 20.0);
            HorizontalChangeX = 20.0;
            VerticalChangeY = 20.0;
            _Operationtimer.Stop();
        }

        private void Rotate_Anticlockwise_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double num = Rotate - 5.0;
            if (num >= 360.0)
            {
                num = 360.0 - num;
            }
            if (num < 0.0)
            {
                num = 0.0;
            }
            RotateTransform renderTransform = new RotateTransform(num);
            _RotateViewer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            _RotateViewer.Btn_AngleLine.RenderTransform = renderTransform;
            _RotateViewer.lbl_Angle.Content = (int)num + "°";
            System.Windows.Point center = new System.Windows.Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            System.Windows.Point p = new System.Windows.Point(x, y);
            System.Windows.Point point = KCommon.PointRotate(center, p, 360.0 - num);
            Canvas.SetLeft(_RotateViewer.ThumbAngle, point.X - 7.5);
            Canvas.SetTop(_RotateViewer.ThumbAngle, point.Y - 7.5);
            MsiRotate(num);
        }

        private void centerelp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                RotateTransform renderTransform = new RotateTransform(0.0);
                _RotateViewer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                _RotateViewer.Btn_AngleLine.RenderTransform = renderTransform;
                _RotateViewer.lbl_Angle.Content = 0 + "°";
                System.Windows.Point center = new System.Windows.Point(40.0, 40.0);
                double x = 39.0;
                double y = 5.5;
                System.Windows.Point p = new System.Windows.Point(x, y);
                System.Windows.Point point = KCommon.PointRotate(center, p, 360.0);
                Canvas.SetLeft(_RotateViewer.ThumbAngle, point.X - 7.5);
                Canvas.SetTop(_RotateViewer.ThumbAngle, point.Y - 7.5);
                MsiRotate(0.0);
            }
        }

        public void MsiRotate(double dRotate)
        {
            try
            {
                double rotate = Rotate;
                Thickness t = new Thickness(0.0, 0.0, 0.0, 0.0);
                if (Bg.Margin == t)
                {
                    Bg.Margin = new Thickness(0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset);
                    msi.ZoomableCanvas.Offset = new System.Windows.Point(msi.ZoomableCanvas.Offset.X - Setting.AngelMsiOffset, msi.ZoomableCanvas.Offset.Y - Setting.AngelMsiOffset);
                    msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                }
                _ = msi.ActualWidth;
                double num = dRotate;
                if (num < 360.0)
                {
                    num += 360.0;
                }
                if (num >= 360.0)
                {
                    num -= 360.0;
                }
                RotateTransform renderTransform = new RotateTransform(num);
                Rotate = num;
                msi.Tag = Rotate;
                Bg.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                Bg.RenderTransform = renderTransform;
                Thumb_Magnifier.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                Thumb_Magnifier.RenderTransform = renderTransform;
                nav.m_Angle = Rotate;
                int nDataLength = 0;
                int num2 = Math.Max((int)ImageW, (int)ImageH);
                int num3 = IsInteger((Math.Log(num2) / Math.Log(2.0)).ToString()) ? ((int)(Math.Log(num2) / Math.Log(2.0))) : ((int)(Math.Log(num2) / Math.Log(2.0)) + 1);
                float fScale = (num3 != 8) ? ((float)SlideZoom / (float)Math.Pow(2.0, num3 - 8)) : ((float)SlideZoom);
                _DllImageFuc.CkGetImageStreamFunc(ref InfoStruct, fScale, 0, 0, ref nDataLength, out IntPtr datas);
                byte[] array = new byte[nDataLength];
                if (datas != IntPtr.Zero)
                {
                    Marshal.Copy(datas, array, 0, nDataLength);
                }
                MemoryStream stream = new MemoryStream(array);
                Bitmap b = new Bitmap(stream);
                nav.cvThumbnail.Children.Remove(nav.m_HLine);
                nav.cvThumbnail.Children.Remove(nav.m_VLine);
                BitmapSource bitmapSource = BitToBS(KCommon.Rotate(b, (int)(360.0 - Rotate)));
                if (bitmapSource != null)
                {
                    nav.SetThumbnail(bitmapSource);
                }
                nav.UpdateThumbnailRect();
                if (Rotate == 0.0)
                {
                    Bg.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
                    msi.ZoomableCanvas.Offset = new System.Windows.Point(msi.ZoomableCanvas.Offset.X + Setting.AngelMsiOffset, msi.ZoomableCanvas.Offset.Y + Setting.AngelMsiOffset);
                    msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                    nav.m_Angle = 0.0;
                }
                if (nav.ret40.Children.Count > 0)
                {
                    nav.ret40.Children.Clear();
                    foreach (mRectangle item in nav.listR40)
                    {
                        Rect rotatePoint = nav.GetRotatePoint(item.offsetx, item.offsety, item.scale);
                        System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                        rectangle.SetValue(Canvas.LeftProperty, rotatePoint.Left);
                        rectangle.SetValue(Canvas.TopProperty, rotatePoint.Top);
                        rectangle.SetValue(System.Windows.Controls.Panel.ZIndexProperty, item.zindex);
                        rectangle.Width = rotatePoint.Width;
                        rectangle.Height = rotatePoint.Height;
                        rectangle.Fill = new SolidColorBrush(item.color);
                        nav.ret40.Children.Add(rectangle);
                    }
                }
                if (nav.ret20.Children.Count > 0)
                {
                    nav.ret20.Children.Clear();
                    foreach (mRectangle item2 in nav.listR20)
                    {
                        Rect rotatePoint2 = nav.GetRotatePoint(item2.offsetx, item2.offsety, item2.scale);
                        System.Windows.Shapes.Rectangle rectangle2 = new System.Windows.Shapes.Rectangle();
                        rectangle2.SetValue(Canvas.LeftProperty, rotatePoint2.Left);
                        rectangle2.SetValue(Canvas.TopProperty, rotatePoint2.Top);
                        rectangle2.SetValue(System.Windows.Controls.Panel.ZIndexProperty, item2.zindex);
                        rectangle2.Width = rotatePoint2.Width;
                        rectangle2.Height = rotatePoint2.Height;
                        rectangle2.Fill = new SolidColorBrush(item2.color);
                        nav.ret20.Children.Add(rectangle2);
                    }
                }
                if (nav.ret10.Children.Count > 0)
                {
                    nav.ret10.Children.Clear();
                    foreach (mRectangle item3 in nav.listR10)
                    {
                        Rect rotatePoint3 = nav.GetRotatePoint(item3.offsetx, item3.offsety, item3.scale);
                        System.Windows.Shapes.Rectangle rectangle3 = new System.Windows.Shapes.Rectangle();
                        rectangle3.SetValue(Canvas.LeftProperty, rotatePoint3.Left);
                        rectangle3.SetValue(Canvas.TopProperty, rotatePoint3.Top);
                        rectangle3.SetValue(System.Windows.Controls.Panel.ZIndexProperty, item3.zindex);
                        rectangle3.Width = rotatePoint3.Width;
                        rectangle3.Height = rotatePoint3.Height;
                        rectangle3.Fill = new SolidColorBrush(item3.color);
                        nav.ret10.Children.Add(rectangle3);
                    }
                }
                if (Setting.IsSynchronous)
                {
                    foreach (KeyValuePair<object, object> item4 in Setting.TabsDic)
                    {
                        if ((Mainpage)item4.Value != this)
                        {
                            double tmpRotate = Rotate - rotate;
                            ((Mainpage)item4.Value).SynRotate(tmpRotate);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void SynMsiRotate(double dRotate)
        {
            try
            {
                double num = dRotate;
                if (num < 360.0)
                {
                    num += 360.0;
                }
                if (num >= 360.0)
                {
                    num -= 360.0;
                }
                RotateTransform renderTransform = new RotateTransform(num);
                Rotate = num;
                msi.Tag = Rotate;
                Bg.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                Bg.RenderTransform = renderTransform;
                Thumb_Magnifier.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
                Thumb_Magnifier.RenderTransform = renderTransform;
                nav.m_Angle = Rotate;
                if (Rotate == 0.0)
                {
                    Bg.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
                    msi.ZoomableCanvas.Offset = new System.Windows.Point(msi.ZoomableCanvas.Offset.X + Setting.AngelMsiOffset, msi.ZoomableCanvas.Offset.Y + Setting.AngelMsiOffset);
                    msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
                    nav.m_Angle = 0.0;
                    _ = msi.ActualWidth;
                }
                int nDataLength = 0;
                int num2 = Math.Max((int)ImageW, (int)ImageH);
                int num3 = IsInteger((Math.Log(num2) / Math.Log(2.0)).ToString()) ? ((int)(Math.Log(num2) / Math.Log(2.0))) : ((int)(Math.Log(num2) / Math.Log(2.0)) + 1);
                float fScale = (num3 != 8) ? ((float)SlideZoom / (float)Math.Pow(2.0, num3 - 8)) : ((float)SlideZoom);
                _DllImageFuc.CkGetImageStreamFunc(ref InfoStruct, fScale, 0, 0, ref nDataLength, out IntPtr datas);
                byte[] array = new byte[nDataLength];
                if (datas != IntPtr.Zero)
                {
                    Marshal.Copy(datas, array, 0, nDataLength);
                }
                _DllImageFuc.CkDeleteImageDataFunc(datas);
                MemoryStream stream = new MemoryStream(array);
                Bitmap b = new Bitmap(stream);
                nav.cvThumbnail.Children.Remove(nav.m_HLine);
                nav.cvThumbnail.Children.Remove(nav.m_VLine);
                BitmapSource bitmapSource = BitToBS(KCommon.Rotate(b, (int)(360.0 - Rotate)));
                if (bitmapSource != null)
                {
                    nav.SetThumbnail(bitmapSource);
                }
                nav.UpdateThumbnailRect();
                if (nav.ret40.Children.Count > 0)
                {
                    nav.ret40.Children.Clear();
                    foreach (mRectangle item in nav.listR40)
                    {
                        Rect rotatePoint = nav.GetRotatePoint(item.offsetx, item.offsety, item.scale);
                        System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                        rectangle.SetValue(Canvas.LeftProperty, rotatePoint.Left);
                        rectangle.SetValue(Canvas.TopProperty, rotatePoint.Top);
                        rectangle.SetValue(System.Windows.Controls.Panel.ZIndexProperty, item.zindex);
                        rectangle.Width = rotatePoint.Width;
                        rectangle.Height = rotatePoint.Height;
                        rectangle.Fill = new SolidColorBrush(item.color);
                        nav.ret40.Children.Add(rectangle);
                    }
                }
                if (nav.ret20.Children.Count > 0)
                {
                    nav.ret20.Children.Clear();
                    foreach (mRectangle item2 in nav.listR20)
                    {
                        Rect rotatePoint2 = nav.GetRotatePoint(item2.offsetx, item2.offsety, item2.scale);
                        System.Windows.Shapes.Rectangle rectangle2 = new System.Windows.Shapes.Rectangle();
                        rectangle2.SetValue(Canvas.LeftProperty, rotatePoint2.Left);
                        rectangle2.SetValue(Canvas.TopProperty, rotatePoint2.Top);
                        rectangle2.SetValue(System.Windows.Controls.Panel.ZIndexProperty, item2.zindex);
                        rectangle2.Width = rotatePoint2.Width;
                        rectangle2.Height = rotatePoint2.Height;
                        rectangle2.Fill = new SolidColorBrush(item2.color);
                        nav.ret20.Children.Add(rectangle2);
                    }
                }
                if (nav.ret10.Children.Count > 0)
                {
                    nav.ret10.Children.Clear();
                    foreach (mRectangle item3 in nav.listR10)
                    {
                        Rect rotatePoint3 = nav.GetRotatePoint(item3.offsetx, item3.offsety, item3.scale);
                        System.Windows.Shapes.Rectangle rectangle3 = new System.Windows.Shapes.Rectangle();
                        rectangle3.SetValue(Canvas.LeftProperty, rotatePoint3.Left);
                        rectangle3.SetValue(Canvas.TopProperty, rotatePoint3.Top);
                        rectangle3.SetValue(System.Windows.Controls.Panel.ZIndexProperty, item3.zindex);
                        rectangle3.Width = rotatePoint3.Width;
                        rectangle3.Height = rotatePoint3.Height;
                        rectangle3.Fill = new SolidColorBrush(item3.color);
                        nav.ret10.Children.Add(rectangle3);
                    }
                }
            }
            catch
            {
            }
        }

        public void SynRotate(double TmpRotate)
        {
            Thickness t = new Thickness(0.0, 0.0, 0.0, 0.0);
            if (Bg.Margin == t)
            {
                Bg.Margin = new Thickness(0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset, 0.0 - Setting.AngelMsiOffset);
                msi.ZoomableCanvas.Offset = new System.Windows.Point(msi.ZoomableCanvas.Offset.X - Setting.AngelMsiOffset, msi.ZoomableCanvas.Offset.Y - Setting.AngelMsiOffset);
                msi.ZoomableCanvas.ApplyAnimationClock(ZoomableCanvas.OffsetProperty, null);
            }
            Bg.UpdateLayout();
            _ = msi.ActualWidth;
            double num = Rotate + TmpRotate;
            if (num < 360.0)
            {
                num += 360.0;
            }
            if (num >= 360.0)
            {
                num -= 360.0;
            }
            RotateTransform renderTransform = new RotateTransform(num);
            _RotateViewer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            _RotateViewer.Btn_AngleLine.RenderTransform = renderTransform;
            _RotateViewer.lbl_Angle.Content = (int)num + "°";
            System.Windows.Point center = new System.Windows.Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            System.Windows.Point p = new System.Windows.Point(x, y);
            System.Windows.Point point = KCommon.PointRotate(center, p, 360.0 - num);
            Canvas.SetLeft(_RotateViewer.ThumbAngle, point.X - 7.5);
            Canvas.SetTop(_RotateViewer.ThumbAngle, point.Y - 7.5);
            SynMsiRotate(num);
        }

        private void Rotate_Clockwise_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double num = Rotate + 5.0;
            if (num >= 360.0)
            {
                num = 360.0 - num;
            }
            if (num < 0.0)
            {
                num = 360.0 + num;
            }
            RotateTransform renderTransform = new RotateTransform(num);
            _RotateViewer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            _RotateViewer.Btn_AngleLine.RenderTransform = renderTransform;
            _RotateViewer.lbl_Angle.Content = (int)num + "°";
            System.Windows.Point center = new System.Windows.Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            System.Windows.Point p = new System.Windows.Point(x, y);
            System.Windows.Point point = KCommon.PointRotate(center, p, 360.0 - num);
            Canvas.SetLeft(_RotateViewer.ThumbAngle, point.X - 7.5);
            Canvas.SetTop(_RotateViewer.ThumbAngle, point.Y - 7.5);
            MsiRotate(num);
        }

        private void _RotateViewer_RotateHandler(object sender, RoutedEventArgs e)
        {
            double num = double.Parse(((System.Windows.Shapes.Rectangle)sender).Tag.ToString());
            RotateTransform renderTransform = new RotateTransform(num);
            _RotateViewer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            _RotateViewer.Btn_AngleLine.RenderTransform = renderTransform;
            _RotateViewer.lbl_Angle.Content = (int)num + "°";
            System.Windows.Point center = new System.Windows.Point(40.0, 40.0);
            double x = 39.0;
            double y = 5.5;
            System.Windows.Point p = new System.Windows.Point(x, y);
            System.Windows.Point point = KCommon.PointRotate(center, p, 360.0 - num);
            Canvas.SetLeft(_RotateViewer.ThumbAngle, point.X - 7.5);
            Canvas.SetTop(_RotateViewer.ThumbAngle, point.Y - 7.5);
            MsiRotate(num);
        }

        private void ThumbAngle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double x = 39.0;
            double y = 5.5;
            double x2 = Canvas.GetLeft(_RotateViewer.ThumbAngle) + 7.5 + e.HorizontalChange;
            double y2 = Canvas.GetTop(_RotateViewer.ThumbAngle) + 7.5 + e.VerticalChange;
            System.Windows.Point point = new System.Windows.Point(40.0, 40.0);
            System.Windows.Point point2 = new System.Windows.Point(x, y);
            System.Windows.Point p = new System.Windows.Point(x2, y2);
            double num = KCommon.AngleDegree(p, point, point2);
            System.Windows.Point point3 = KCommon.PointRotate(point, point2, num);
            Canvas.SetLeft(_RotateViewer.ThumbAngle, point3.X - 7.5);
            Canvas.SetTop(_RotateViewer.ThumbAngle, point3.Y - 7.5);
            RotateTransform renderTransform = new RotateTransform(360.0 - num);
            _RotateViewer.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            _RotateViewer.Btn_AngleLine.RenderTransform = renderTransform;
            _RotateViewer.lbl_Angle.Content = (int)(360.0 - num) + "°";
            MsiRotate(360.0 - num);
        }

        public static BitmapSource BitToBS(Bitmap bmp)
        {
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch
            {
                return null;
            }
        }

        private void Canvas_OperateballMenu_Click(object sender, RoutedEventArgs e)
        {
            Canvas_Operateball.Visibility = Visibility.Collapsed;
        }

        public void CreateMagnifierMenu()
        {
            System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();
            System.Windows.Controls.MenuItem menuItem = new System.Windows.Controls.MenuItem();
            System.Windows.Controls.MenuItem menuItem2 = new System.Windows.Controls.MenuItem();
            System.Windows.Controls.MenuItem menuItem3 = new System.Windows.Controls.MenuItem();
            System.Windows.Controls.MenuItem menuItem4 = new System.Windows.Controls.MenuItem();
            new System.Windows.Controls.MenuItem();
            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            label.Content = "2X";
            label.FontSize = 14.0;
            label.Height = 30.0;
            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header = label;
            System.Windows.Controls.Label label2 = new System.Windows.Controls.Label();
            label2.Content = "4X";
            label2.FontSize = 14.0;
            label2.Height = 30.0;
            label2.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label2.VerticalContentAlignment = VerticalAlignment.Center;
            label2.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header2 = label2;
            System.Windows.Controls.Label label3 = new System.Windows.Controls.Label();
            label3.Content = "Max";
            label3.FontSize = 14.0;
            label3.Height = 30.0;
            label3.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label3.VerticalContentAlignment = VerticalAlignment.Center;
            label3.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header3 = label3;
            System.Windows.Controls.Label label4 = new System.Windows.Controls.Label();
            label4.Content = "关闭";
            label4.FontSize = 14.0;
            label4.Height = 30.0;
            label4.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
            label4.VerticalContentAlignment = VerticalAlignment.Center;
            label4.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            System.Windows.Controls.Label header4 = label4;
            menuItem.Header = header;
            menuItem2.Header = header2;
            menuItem3.Header = header3;
            menuItem4.Header = header4;
            if (MagnifierScale == 2)
            {
                menuItem.Icon = new System.Windows.Controls.Image
                {
                    Source = new BitmapImage(new Uri("images/Ok1.png", UriKind.Relative)),
                    Width = 30.0,
                    Height = 30.0
                };
            }
            if (MagnifierScale == 4)
            {
                menuItem2.Icon = new System.Windows.Controls.Image
                {
                    Source = new BitmapImage(new Uri("images/Ok1.png", UriKind.Relative)),
                    Width = 30.0,
                    Height = 30.0
                };
            }
            else if (MagnifierScale == SlideZoom || MagnifierScale == 0)
            {
                menuItem3.Icon = new System.Windows.Controls.Image
                {
                    Source = new BitmapImage(new Uri("images/Ok1.png", UriKind.Relative)),
                    Width = 30.0,
                    Height = 30.0
                };
            }
            contextMenu.Items.Add(menuItem);
            contextMenu.Items.Add(menuItem2);
            contextMenu.Items.Add(menuItem3);
            contextMenu.Items.Add(menuItem4);
            menuItem.Click += menuItem_Click;
            menuItem2.Click += menuItem1_Click;
            menuItem3.Click += menuItem2_Click;
            menuItem4.Click += menuItem3_Click;
            Canv_Magnifier.ContextMenu = contextMenu;
        }

        private void menuItem_Click(object sender, RoutedEventArgs e)
        {
            MagnifierScale = 2;
            Canv_Magnifier.ContextMenu = null;
            CreateMagnifierMenu();
        }

        private void menuItem1_Click(object sender, RoutedEventArgs e)
        {
            MagnifierScale = 4;
            Canv_Magnifier.ContextMenu = null;
            CreateMagnifierMenu();
        }

        private void menuItem2_Click(object sender, RoutedEventArgs e)
        {
            MagnifierScale = SlideZoom;
            Canv_Magnifier.ContextMenu = null;
            CreateMagnifierMenu();
        }

        private void menuItem3_Click(object sender, RoutedEventArgs e)
        {
            Canv_Magnifier.Visibility = Visibility.Collapsed;
            _Magnifiertimer.Stop();
        }
    }
}
