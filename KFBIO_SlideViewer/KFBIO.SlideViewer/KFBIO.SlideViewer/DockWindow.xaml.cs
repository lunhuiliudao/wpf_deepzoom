using KFBIO.Common;
using LanguageLib;
using LanguageLib.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Themes;

namespace KFBIO.SlideViewer
{
    /// <summary>
    /// DockWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DockWindow : Window
    {
        public struct CopyDataStruct
        {
            public IntPtr dwData;

            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        public const int WM_DOWNLOAD_COMPLETED = 170;

        public IniFile ifile;

        public ImageBrower imgs;

        private SetSysWind _SetSysWind;

        public string Configpath = string.Empty;

        private string Toolpath = string.Empty;

        public MessageWind _MemoryMessageWind;

        public LocalizedStrings languageSetter = (LocalizedStrings)System.Windows.Application.Current.Resources["Lang"];

        public ObservableResources LanguageResource = new ObservableResources();

        private Process myprocess;

        private Dictionary<int, LayoutDocumentPaneGroup> ilist = new Dictionary<int, LayoutDocumentPaneGroup>();

        public DockWindow()
        {
            InitializeComponent();
            ifile = new IniFile(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Config.ini");
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string text = System.IO.Path.Combine(folderPath, "KFBIO\\K-Viewer");
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            if (!File.Exists(System.IO.Path.Combine(text, "config.ini")))
            {
                File.Copy(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Config.ini", System.IO.Path.Combine(text, "config.ini"), true);
            }
            else
            {
                IniFile iniFile = new IniFile(System.IO.Path.Combine(text, "config.ini"));
                string text2 = iniFile.IniReadValue("Ver", "Value");
                if (text2.Trim().Length == 0)
                {
                    File.Copy(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Config.ini", System.IO.Path.Combine(text, "config.ini"), true);
                }
                else
                {
                    string a = ifile.IniReadValue("Ver", "Value");
                    if (a != text2)
                    {
                        File.Copy(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Config.ini", System.IO.Path.Combine(text, "config.ini"), true);
                    }
                }
            }
            Configpath = System.IO.Path.Combine(text, "config.ini");
            ifile = new IniFile(Configpath);
            imgs = new ImageBrower();
            imagelist.Content = imgs;
            imagelist.IconSource = new BitmapImage(new Uri("images/open.png", UriKind.Relative));
            imagelist.Hide();
            imgs._evm.SearchViewModel.UIConfirmedParseName = ifile.IniReadValue("SelectPath", "Value");
            Ctcimagelist.IconSource = new BitmapImage(new Uri("images/open.png", UriKind.Relative));
            Ctcimagelist.Content = null;
            Ctcimagelist.Hide();
            _SetSysWind = new SetSysWind();
            _SetSysWind.MaxMagRefresh += _SetSysWind_MaxMagRefresh;
            _SetSysWind.LabelRefresh += _SetSysWind_LabelRefresh;
            _SetSysWind.RuleRefresh += _SetSysWind_RuleRefresh;
            _SetSysWind.NavRefresh += _SetSysWind_NavRefresh;
            _SetSysWind.LangRefresh += _SetSysWind_LangRefresh;
            _SetSysWind.MagnifierRefresh += _SetSysWind_MagnifierRefresh;
            _SetSysWind.OperateballRefresh += _SetSysWind_OperateballRefresh;
            _SetSysWind.RotateRefresh += _SetSysWind_RotateRefresh;
            _SetSysWind.MagnifierCheckRefresh += _SetSysWind_MagnifierCheckRefresh;
            _SetSysWind.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Setting.IsLabel = ifile.IniReadValue("Switch", "IsLabel");
            Setting.IsCase = ifile.IniReadValue("Switch", "IsCase");
            Setting.IsNav = ifile.IniReadValue("Switch", "IsNav");
            Setting.IsSingleFile = ifile.IniReadValue("Switch", "IsSingleFile");
            Setting.IsMutiScreen = ifile.IniReadValue("Switch", "IsMutiScreen");
            Setting.IsRule = ifile.IniReadValue("Switch", "IsRule");
            Setting.MaxMagValue = double.Parse(ifile.IniReadValue("MagScale", "Value"));
            Setting.Language = ifile.IniReadValue("Language", "Type");
            Setting.IsHsv = ifile.IniReadValue("Switch", "IsHsv");
            Setting.AnalysisPath = ifile.IniReadValue("ToolPath", "AnalysisPath");
            Setting.IsRotate = ifile.IniReadValue("Switch", "IsRotate");
            Setting.IsMagnifier = ifile.IniReadValue("Switch", "IsMagnifier");
            Setting.IsOperateball = ifile.IniReadValue("Switch", "IsOperateball");
            Setting.IsLogo = ifile.IniReadValue("Switch", "IsLogo");
            Setting.Magnifier = int.Parse(ifile.IniReadValue("Magnifier", "Value"));
            string text3 = ifile.IniReadValue("Calibration", "Value40");
            if (text3 == "")
            {
                text3 = "0";
            }
            string text4 = ifile.IniReadValue("Calibration", "Value20");
            if (text4 == "")
            {
                text4 = "0";
            }
            string text5 = ifile.IniReadValue("Calibration", "ValueX40");
            if (text5 == "")
            {
                text5 = "1";
            }
            string text6 = ifile.IniReadValue("Calibration", "ValueX20");
            if (text6 == "")
            {
                text6 = "1";
            }
            Setting.Calibration40 = double.Parse(text3);
            Setting.Calibration20 = double.Parse(text4);
            Setting.CalibrationX40 = double.Parse(text5);
            Setting.CalibrationX20 = double.Parse(text6);
            if (Setting.Language == "0")
            {
                _SetSysWind.langzh.IsChecked = true;
                ChangeLanguage("zh-Hans");
            }
            else if (Setting.Language == "1")
            {
                _SetSysWind.langen.IsChecked = true;
                ChangeLanguage("en-US");
            }
            Toolpath = ifile.IniReadValue("ToolPath", "Value");
            if (Toolpath.Trim().Length == 0)
            {
                Tool.Visibility = Visibility.Collapsed;
            }
            _SetSysWind._ZoomSlider.Value = double.Parse(ifile.IniReadValue("MagScale", "Value"));
            if (Setting.IsLabel == "1")
            {
                _SetSysWind.Ck_Label.IsChecked = true;
            }
            else
            {
                _SetSysWind.Ck_Label.IsChecked = false;
            }
            if (Setting.IsNav == "1")
            {
                _SetSysWind.Ck_Nav.IsChecked = true;
            }
            else
            {
                _SetSysWind.Ck_Nav.IsChecked = false;
            }
            if (Setting.IsRule == "1")
            {
                _SetSysWind.Ck_Rule.IsChecked = true;
            }
            else
            {
                _SetSysWind.Ck_Rule.IsChecked = false;
            }
            if (Setting.IsRotate == "1")
            {
                _SetSysWind.Ck_Rotate.IsChecked = true;
            }
            else
            {
                _SetSysWind.Ck_Rotate.IsChecked = false;
            }
            if (Setting.IsMagnifier == "1")
            {
                _SetSysWind.Ck_Magnifier.IsChecked = true;
            }
            else
            {
                _SetSysWind.Ck_Magnifier.IsChecked = false;
            }
            if (Setting.IsOperateball == "1")
            {
                _SetSysWind.Ck_Operateball.IsChecked = true;
            }
            else
            {
                _SetSysWind.Ck_Magnifier.IsChecked = false;
            }
            if (Setting.Magnifier == 2)
            {
                _SetSysWind.Magnifier_2.IsChecked = true;
            }
            if (Setting.Magnifier == 4)
            {
                _SetSysWind.Magnifier_4.IsChecked = true;
            }
            Help.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Help_Click), true);
            About.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(SAbout_Click), true);
            FullScreen.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(FullScreen_Click), true);
            EscFullScreen.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(EscFullScreen_Click), true);
            ImageBrower.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(ImageBrower_Click), true);
            SetSys.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(SetSys_Click), true);
            Ctctool.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(CtcTool_Click), true);
            Tool.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Tool_Click), true);
            btnMin.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(btnMin_Click), true);
            imgshow.AddHandler(UIElement.MouseEnterEvent, new RoutedEventHandler(Image_MouseLeftButtonDown), true);
            btnClose.AddHandler(UIElement.MouseEnterEvent, new RoutedEventHandler(btnClose2_Click), true);
            imagelist.Title = languageSetter.LanguageResource["ImageList"];
            imagebrower.Title = languageSetter.LanguageResource["Index"];
            Ctcimagelist.Title = "CTC诊断";
        }

        private void _SetSysWind_MagnifierCheckRefresh(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    Mainpage mainpage = (Mainpage)item.Value;
                    if (((System.Windows.Controls.RadioButton)sender).Tag.ToString() == "1")
                    {
                        mainpage.MagnifierScale = 2;
                    }
                    else if (((System.Windows.Controls.RadioButton)sender).Tag.ToString() == "2")
                    {
                        mainpage.MagnifierScale = 4;
                    }
                    else
                    {
                        mainpage.MagnifierScale = 0;
                    }
                    Setting.Magnifier = mainpage.MagnifierScale;
                    mainpage.Canv_Magnifier.ContextMenu = null;
                    mainpage.CreateMagnifierMenu();
                }
            }
        }

        private void _SetSysWind_RotateRefresh(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    Mainpage mainpage = (Mainpage)item.Value;
                    if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
                    {
                        mainpage._RotateViewer.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        mainpage._RotateViewer.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void _SetSysWind_OperateballRefresh(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    Mainpage mainpage = (Mainpage)item.Value;
                    if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
                    {
                        mainpage.Canvas_Operateball.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        mainpage.Canvas_Operateball.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void _SetSysWind_MagnifierRefresh(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    Mainpage mainpage = (Mainpage)item.Value;
                    if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
                    {
                        mainpage.Canv_Magnifier.Visibility = Visibility.Visible;
                        mainpage._Magnifiertimer.Start();
                    }
                    else
                    {
                        mainpage.Canv_Magnifier.Visibility = Visibility.Collapsed;
                        mainpage._Magnifiertimer.Stop();
                    }
                }
            }
        }

        private void _SetSysWind_LangRefresh(object sender, RoutedEventArgs e)
        {
            if (((System.Windows.Controls.RadioButton)sender).Tag.ToString() == "1")
            {
                ChangeLanguage("zh-Hans");
            }
            else if (((System.Windows.Controls.RadioButton)sender).Tag.ToString() == "2")
            {
                ChangeLanguage("en-US");
            }
        }

        public void UpdateLang()
        {
            imagelist.Title = languageSetter.LanguageResource["ImageList"];
            imagebrower.Title = languageSetter.LanguageResource["Index"];
        }

        private void ChangeLanguage(string CultureName)
        {
            CultureInfo cultureInfo = new CultureInfo(CultureName);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            MultiLanguage.Culture = cultureInfo;
            languageSetter.ChangeLanguage();
            UpdateLang();
            UpdateSetParam();
            if (CultureName == "en-US")
            {
                Setting.Language = "1";
                Help.Visibility = Visibility.Hidden;
                indeximg.Source = new BitmapImage(new Uri("images/newp_en.jpg", UriKind.Relative));
            }
            else if (CultureName == "zh-Hans")
            {
                Setting.Language = "0";
                indeximg.Source = new BitmapImage(new Uri("images/newp.jpg", UriKind.Relative));
                Help.Visibility = Visibility.Visible;
            }
        }

        public void UpdateSetParam()
        {
            Setting.Line = languageSetter.LanguageResource["Line"];
            Setting.Arrow = languageSetter.LanguageResource["Arrow"];
            Setting.Rectangle = languageSetter.LanguageResource["Rectangle"];
            Setting.Ellipse = languageSetter.LanguageResource["Ellipse"];
            Setting.Polygon = languageSetter.LanguageResource["Polyline"];
            Setting.Remark = languageSetter.LanguageResource["Remark"];
            Setting.Width = languageSetter.LanguageResource["Width"];
            Setting.Half_Width = languageSetter.LanguageResource["Half_Width"];
            Setting.Half_Height = languageSetter.LanguageResource["Half_Height"];
            Setting.Height = languageSetter.LanguageResource["Height"];
            Setting.Length = languageSetter.LanguageResource["Length"];
            Setting.xLength = languageSetter.LanguageResource["xLength"];
            Setting.Angle = languageSetter.LanguageResource["Angle"];
            Setting.Area = languageSetter.LanguageResource["Area"];
            Setting.AnnoDesStr = languageSetter.LanguageResource["AnnoDesStr"];
            dm.Theme = new GenericTheme();
        }

        public void Tool_Click(object sender, EventArgs e)
        {
            StartProcess(Toolpath, null);
        }

        public bool StartProcess(string filename, string[] args)
        {
            if (myprocess != null && !myprocess.HasExited)
            {
                NativeMethods.ShowWindow(myprocess.MainWindowHandle, WindowShowStyle.Restore);
                NativeMethods.SetForegroundWindow(myprocess.MainWindowHandle);
                return false;
            }
            try
            {
                string text = "";
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
                System.Windows.MessageBox.Show("启功工具出错！原因：" + ex.Message);
            }
            return false;
        }

        private void addEvent()
        {
            mtitle.MouseEnter += mtitle_MouseEnter;
            mtitle.MouseLeave += mtitle_MouseLeave;
        }

        private void minEvent()
        {
            mtitle.MouseEnter -= mtitle_MouseEnter;
            mtitle.MouseLeave -= mtitle_MouseLeave;
        }

        private void ShowMenu()
        {
            mtitle.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
            mtitle.Opacity = 1.0;
        }

        private void HiddMenu()
        {
            mtitle.Margin = new Thickness(0.0, -23.0, 0.0, 0.0);
            mtitle.Opacity = 0.0;
        }

        public void mtitle_MouseEnter(object sender, EventArgs e)
        {
            ShowMenu();
        }

        public void mtitle_MouseLeave(object sender, EventArgs e)
        {
            HiddMenu();
        }

        public void SAbout_Click(object sender, EventArgs e)
        {
            AboutWind aboutWind = new AboutWind();
            aboutWind.Owner = this;
            aboutWind.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            aboutWind.ShowDialog();
        }

        public void Help_Click(object sender, EventArgs e)
        {
            HelpWind helpWind = new HelpWind();
            helpWind.Owner = this;
            helpWind.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            helpWind.Show();
        }

        public void FullScreen_Click(object sender, EventArgs e)
        {
            dm.Theme = new GenericTheme2();
            imagelist.Hide();
            EscFullScreen.Visibility = Visibility.Visible;
            FullScreen.Visibility = Visibility.Collapsed;
            base.WindowState = WindowState.Maximized;
            base.Topmost = true;
        }

        public void EscFullScreen_Click(object sender, EventArgs e)
        {
            dm.Theme = new GenericTheme();
            imagelist.Show();
            FullScreen.Visibility = Visibility.Visible;
            EscFullScreen.Visibility = Visibility.Collapsed;
            base.WindowState = WindowState.Normal;
            base.Topmost = false;
        }

        public void CtcTool_Click(object sender, EventArgs e)
        {
            imagelist.ToggleAutoHide();
            Ctcimagelist.Show();
        }

        public void SZ_Click(object sender, EventArgs e)
        {
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            base.Left = 0.0;
            base.Top = 0.0;
            base.Height = SystemParameters.WorkArea.Height;
            base.Width = SystemParameters.WorkArea.Width;
            (PresentationSource.FromVisual(this) as HwndSource).AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 170)
            {
                try
                {
                    base.Topmost = true;
                    string text = string.Empty;
                    string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string text2 = System.IO.Path.Combine(folderPath, "KFBIO\\K-Viewer");
                    if (!Directory.Exists(text2))
                    {
                        return IntPtr.Zero;
                    }
                    string path = text2 + "\\Temp";
                    if (!File.Exists(path))
                    {
                        return IntPtr.Zero;
                    }
                    StreamReader streamReader = new StreamReader(path, Encoding.Unicode);
                    string text3 = "";
                    while ((text3 = streamReader.ReadLine()) != null)
                    {
                        text += text3;
                    }
                    streamReader.Close();
                    if (Setting.IsSingleFile == "1" && Setting.TabsDic.Count > 0)
                    {
                        foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                        {
                            if (((Mainpage)item.Value).FilePath == text)
                            {
                                ((LayoutDocument)item.Key).IsActive = true;
                                ((LayoutDocument)item.Key).IsSelected = true;
                                base.Topmost = false;
                                return IntPtr.Zero;
                            }
                        }
                    }
                    if (!KCommon.CheckVersion(text))
                    {
                        return IntPtr.Zero;
                    }
                    Mainpage content = new Mainpage(text);
                    string text4 = text.Substring(text.LastIndexOf("\\") + 1, text.Length - text.LastIndexOf("\\") - 1);
                    text4 = text4.Substring(0, text4.Length - 4);
                    BitmapImage kFBThumnail = KCommon.GetKFBThumnail(text);
                    if (kFBThumnail.StreamSource == null)
                    {
                        MessageWind messageWind = new MessageWind(MessageBoxButton.OK, System.Windows.Application.Current.MainWindow, text + "数字图像已损坏！", "提示", MessageBoxIcon.Exclamation, false);
                        messageWind.ShowDialog();
                        return IntPtr.Zero;
                    }
                    AddItem(text + DateTime.Now.ToString("yyyyMMddhhmmss"), text4, content, kFBThumnail);
                    imgs._evm.SearchViewModel.UIConfirmedParseName = text.Substring(0, text.LastIndexOf("\\"));
                    base.Topmost = false;
                    return hwnd;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                    return hwnd;
                }
            }
            return hwnd;
        }

        [DllImport("User32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr wnd, int msg, IntPtr wP, IntPtr lP);

        [DllImport("User32.dll")]
        public static extern int PostMessage(IntPtr wnd, int msg, IntPtr wP, IntPtr lP);

        private void d_IsSelectedChanged(object sender, EventArgs e)
        {
            if (!((LayoutDocument)sender).IsSelected)
            {
                ((Mainpage)Setting.TabsDic[(LayoutDocument)sender])._CaseInfoWind.Visibility = Visibility.Collapsed;
                if (((Mainpage)Setting.TabsDic[(LayoutDocument)sender])._SlideInfoWind != null)
                {
                    ((Mainpage)Setting.TabsDic[(LayoutDocument)sender])._SlideInfoWind.Visibility = Visibility.Collapsed;
                }
                ((Mainpage)Setting.TabsDic[(LayoutDocument)sender])._AdjustmentWind.Visibility = Visibility.Collapsed;
                ((Mainpage)Setting.TabsDic[(LayoutDocument)sender])._AnnoListWind.Visibility = Visibility.Collapsed;
                return;
            }
            if (((Mainpage)Setting.TabsDic[(LayoutDocument)sender]).IsCaseWind == Visibility.Visible)
            {
                ((Mainpage)Setting.TabsDic[(LayoutDocument)sender])._CaseInfoWind.Visibility = Visibility.Visible;
            }
            if (((Mainpage)Setting.TabsDic[(LayoutDocument)sender]).IsSlideWind == Visibility.Visible)
            {
                ((Mainpage)Setting.TabsDic[(LayoutDocument)sender])._SlideInfoWind.Visibility = Visibility.Visible;
            }
            if (((Mainpage)Setting.TabsDic[(LayoutDocument)sender]).IsImageAdjustWind == Visibility.Visible)
            {
                ((Mainpage)Setting.TabsDic[(LayoutDocument)sender])._AdjustmentWind.Visibility = Visibility.Visible;
            }
            if (((Mainpage)Setting.TabsDic[(LayoutDocument)sender]).IsAnnoManageWind == Visibility.Visible)
            {
                ((Mainpage)Setting.TabsDic[(LayoutDocument)sender])._AnnoListWind.Visibility = Visibility.Visible;
            }
        }

        private void d_IsActiveChanged(object sender, EventArgs e)
        {
            if (!((LayoutDocument)sender).IsActive)
            {
                ((Mainpage)Setting.TabsDic[(LayoutDocument)sender])._CaseInfoWind.Visibility = Visibility.Collapsed;
            }
        }

        public void AddItem(string ContentId, string Title, object Content, ImageSource bs)
        {
            if (dpg.Children.IndexOf(imagebrower) != -1)
            {
                HiddMenu();
                addEvent();
                ((LayoutDocumentPane)LDPaneGroup.Children[0]).Children.Remove(imagebrower);
            }
            if (Setting.TabsDic.Count >= 10)
            {
                MessageWind messageWind = new MessageWind(MessageBoxButton.OK, this, "打开文件不得大于10张！", "提示", MessageBoxIcon.Exclamation, false);
                messageWind.ShowDialog();
                return;
            }
            LayoutDocument layoutDocument = new LayoutDocument();
            layoutDocument.ContentId = ContentId;
            layoutDocument.IconSource = bs;
            layoutDocument.Title = Title;
            layoutDocument.Content = Content;
            layoutDocument.Closed += d_Closed;
            layoutDocument.IsActive = false;
            layoutDocument.IsSelected = true;
            layoutDocument.IsSelectedChanged += d_IsSelectedChanged;
            LayoutDocumentPane layoutDocumentPane = null;
            if (LDPaneGroup.Children[0].GetType() == typeof(LayoutDocumentPane))
            {
                layoutDocumentPane = (LayoutDocumentPane)LDPaneGroup.Children[0];
            }
            else if (LDPaneGroup.Children[0].GetType() == typeof(LayoutDocumentPaneGroup))
            {
                layoutDocumentPane = (LayoutDocumentPane)((LayoutDocumentPaneGroup)LDPaneGroup.Children[0]).Children[0];
            }
            layoutDocumentPane.Children.Insert(layoutDocumentPane.ChildrenCount, layoutDocument);
            Setting.TabsDic.Add(layoutDocument, Content);
            if (Setting.IsFloat == 1)
            {
                double actualWidth = ((ILayoutPositionableElementWithActualSize)LDPaneGroup).ActualWidth;
                double actualHeight = ((ILayoutPositionableElementWithActualSize)LDPaneGroup).ActualHeight;
                layoutDocument.FloatingWidth = actualWidth / 1.2;
                layoutDocument.FloatingHeight = actualHeight / 1.2;
                layoutDocument.FloatingLeft = actualWidth / 2.0 - actualWidth / 1.2 / 2.0;
                layoutDocument.FloatingTop = (actualHeight + 28.0) / 2.0 - actualHeight / 1.2 / 2.0;
                layoutDocument.Float();
            }
        }

        public void d_Closed(object sender, EventArgs e)
        {
            ((Mainpage)Setting.TabsDic[(LayoutDocument)sender]).SingleReaseSlide();
            Setting.TabsDic.Remove((LayoutDocument)sender);
            if (Setting.TabsDic.Count == 0)
            {
                foreach (KeyValuePair<object, object> item in Setting.TabsDic)
                {
                    ((Mainpage)item.Value).ReaseSlide();
                }
                Setting.TabsDic.Clear();
                Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                ifile.IniWriteValue("SelectPath", "Value", ((ImageBrower)imagelist.Content)._evm.SearchViewModel.UIConfirmedParseName);
                ifile.IniWriteValue("Switch", "IsLabel", Setting.IsLabel);
                ifile.IniWriteValue("Switch", "IsNav", Setting.IsNav);
                ifile.IniWriteValue("Switch", "IsRule", Setting.IsRule);
                ifile.IniWriteValue("MagScale", "Value", Setting.MaxMagValue.ToString());
                ifile.IniWriteValue("Switch", "IsRotate", Setting.IsRotate);
                ifile.IniWriteValue("Switch", "IsMagnifier", Setting.IsMagnifier);
                ifile.IniWriteValue("Switch", "IsOperateball", Setting.IsOperateball);
                ifile.IniWriteValue("Magnifier", "Value", Setting.Magnifier.ToString());
                System.Windows.Application.Current.Shutdown();
                base.OnClosing(e);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Mainpage mainpage = null;
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (((((LayoutDocument)item.Key).IsSelected && ((LayoutDocument)item.Key).IsActive) || (((LayoutDocument)item.Key).IsFloating && ((LayoutDocument)item.Key).IsSelected && ((LayoutDocument)item.Key).IsActive)) && item.Value.GetType() == typeof(Mainpage))
                {
                    mainpage = (Mainpage)item.Value;
                }
            }
            if (mainpage != null)
            {
                Keyboard.Focus(mainpage);
                mainpage.KeyValue(e);
            }
            switch (e.Key)
            {
                case Key.LeftCtrl:
                    Setting.isCtrl = 1;
                    break;
                case Key.RightCtrl:
                    Setting.isCtrl = 1;
                    break;
                case Key.Escape:
                    foreach (KeyValuePair<object, object> item2 in Setting.TabsDic)
                    {
                        ((Mainpage)item2.Value).ReaseSlide();
                    }
                    Setting.TabsDic.Clear();
                    Close();
                    break;
                case Key.F1:
                    Help_Click(null, null);
                    break;
                case Key.F11:
                    if (FullScreen.Visibility == Visibility.Visible)
                    {
                        FullScreen_Click(null, null);
                    }
                    else
                    {
                        EscFullScreen_Click(null, null);
                    }
                    break;
            }
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            base.WindowState = WindowState.Minimized;
        }

        private void btnClose2_Click(object sender, RoutedEventArgs e)
        {
            Activate();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ifile.IniWriteValue("Language", "Type", Setting.Language);
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                ((Mainpage)item.Value).ReaseSlide();
            }
            Setting.TabsDic.Clear();
            Close();
        }

        private void ImageBrower_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (imagelist.IsHidden)
                {
                    Activate();
                    imagelist.IsSelected = true;
                    imagelist.IsActive = true;
                    imagelist.Show();
                }
                else
                {
                    imagelist.Hide();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void SetSys_Click(object sender, RoutedEventArgs e)
        {
            _SetSysWind.Owner = this;
            _SetSysWind.Show();
        }

        private void _SetSysWind_NavRefresh(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    Mainpage mainpage = (Mainpage)item.Value;
                    if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
                    {
                        mainpage.nav.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        mainpage.nav.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void _SetSysWind_RuleRefresh(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    Mainpage mainpage = (Mainpage)item.Value;
                    if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
                    {
                        mainpage.RuleCanvas.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        mainpage.RuleCanvas.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void _SetSysWind_LabelRefresh(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    Mainpage mainpage = (Mainpage)item.Value;
                    if (((System.Windows.Controls.CheckBox)sender).IsChecked == true)
                    {
                        mainpage.Image_lable.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        mainpage.Image_lable.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void _SetSysWind_MaxMagRefresh(object sender, RoutedEventArgs e)
        {
            Mainpage mainpage = null;
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (((LayoutDocument)item.Key).IsSelected && ((LayoutDocument)item.Key).IsActive && item.Value.GetType() == typeof(Mainpage))
                {
                    mainpage = (Mainpage)item.Value;
                }
            }
            if (mainpage != null && Setting.MaxMagValue * (double)mainpage.SlideZoom < mainpage.Curscale)
            {
                mainpage.ZoomRatio(Setting.MaxMagValue * (double)mainpage.SlideZoom);
            }
        }

        private void mtitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Setting.isCtrl = 0;
        }

        private void men_horizontal_Click(object sender, RoutedEventArgs e)
        {
            Setting.IsFloat = 1;
            int num = 0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            int num5 = 28;
            double actualWidth = ((ILayoutPositionableElementWithActualSize)LDPaneGroup).ActualWidth;
            double actualHeight = ((ILayoutPositionableElementWithActualSize)LDPaneGroup).ActualHeight;
            minEvent();
            ShowMenu();
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    LayoutDocument layoutDocument = (LayoutDocument)item.Key;
                    if (Setting.TabsDic.Count == 2 || Setting.TabsDic.Count == 3)
                    {
                        num2 = actualWidth;
                        num3 = (actualHeight - (double)num5) / (double)Setting.TabsDic.Count;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        layoutDocument.FloatingLeft = 0.0;
                        layoutDocument.FloatingTop = num3 * (double)num + (double)num5;
                    }
                    if (Setting.TabsDic.Count == 4)
                    {
                        num2 = actualWidth / 2.0;
                        num3 = (actualHeight - (double)num5) / 2.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num == 0)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = num5;
                        }
                        if (num == 1)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = (double)num5 + num3;
                        }
                        if (num == 2)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = num5;
                        }
                        if (num == 3)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = (double)num5 + num3;
                        }
                    }
                    if (Setting.TabsDic.Count == 5)
                    {
                        num2 = actualWidth / 2.0;
                        num3 = (actualHeight - (double)num5) / 2.0;
                        num4 = (actualHeight - (double)num5) / 3.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num >= 0 && num <= 2)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = (double)num5 + num3 * (double)num;
                        }
                        if (num >= 2 && num <= 4)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = (double)num5 + num4 * (double)(num - 2);
                            layoutDocument.FloatingHeight = num4;
                        }
                    }
                    if (Setting.TabsDic.Count == 6)
                    {
                        num2 = actualWidth / 2.0;
                        num3 = (actualHeight - (double)num5) / 3.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num >= 0 && num <= 2)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = (double)num5 + num3 * (double)num;
                        }
                        if (num >= 3 && num <= 5)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = (double)num5 + num3 * (double)(num - 3);
                        }
                    }
                    if (Setting.TabsDic.Count == 7)
                    {
                        num2 = actualWidth / 2.0;
                        num3 = (actualHeight - (double)num5) / 3.0;
                        num4 = (actualHeight - (double)num5) / 4.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num >= 0 && num <= 2)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = (double)num5 + num3 * (double)num;
                        }
                        if (num >= 3 && num <= 6)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = (double)num5 + num4 * (double)(num - 3);
                            layoutDocument.FloatingHeight = num4;
                        }
                    }
                    if (Setting.TabsDic.Count == 8)
                    {
                        num2 = actualWidth / 2.0;
                        num3 = (actualHeight - (double)num5) / 4.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num >= 0 && num <= 3)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = (double)num5 + num3 * (double)num;
                        }
                        if (num >= 4 && num <= 7)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = (double)num5 + num3 * (double)(num - 4);
                        }
                    }
                    if (Setting.TabsDic.Count == 9)
                    {
                        num2 = actualWidth / 3.0;
                        num3 = (actualHeight - (double)num5) / 3.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num >= 0 && num <= 2)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = (double)num5 + num3 * (double)num;
                        }
                        if (num >= 3 && num <= 5)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = (double)num5 + num3 * (double)(num - 3);
                        }
                        if (num >= 6 && num <= 8)
                        {
                            layoutDocument.FloatingLeft = num2 * 2.0;
                            layoutDocument.FloatingTop = (double)num5 + num3 * (double)(num - 6);
                        }
                    }
                    if (Setting.TabsDic.Count == 10)
                    {
                        num2 = actualWidth / 3.0;
                        num3 = (actualHeight - (double)num5) / 3.0;
                        double num6 = (actualHeight - (double)num5) / 4.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num >= 0 && num <= 2)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = (double)num5 + num3 * (double)num;
                        }
                        if (num >= 3 && num <= 5)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = (double)num5 + num3 * (double)(num - 3);
                        }
                        if (num >= 6 && num <= 9)
                        {
                            layoutDocument.FloatingLeft = num2 * 2.0;
                            layoutDocument.FloatingTop = (double)num5 + num6 * (double)(num - 6);
                            layoutDocument.FloatingHeight = num6;
                        }
                    }
                    if (layoutDocument.IsFloating)
                    {
                        layoutDocument.Dock();
                    }
                    layoutDocument.Float();
                    num++;
                }
            }
        }

        private void men_vertical_Click(object sender, RoutedEventArgs e)
        {
            Setting.IsFloat = 1;
            int num = 0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            int num5 = 28;
            double actualWidth = ((ILayoutPositionableElementWithActualSize)LDPaneGroup).ActualWidth;
            double actualHeight = ((ILayoutPositionableElementWithActualSize)LDPaneGroup).ActualHeight;
            minEvent();
            ShowMenu();
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    LayoutDocument layoutDocument = (LayoutDocument)item.Key;
                    if (Setting.TabsDic.Count == 2 || Setting.TabsDic.Count == 3)
                    {
                        num2 = actualWidth / (double)Setting.TabsDic.Count;
                        num3 = actualHeight - (double)num5;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        layoutDocument.FloatingLeft = num2 * (double)num;
                        layoutDocument.FloatingTop = num5;
                    }
                    if (Setting.TabsDic.Count == 4)
                    {
                        num2 = actualWidth / 2.0;
                        num3 = (actualHeight - (double)num5) / 2.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num == 0)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = num5;
                        }
                        if (num == 1)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = num5;
                        }
                        if (num == 2)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = num3 + (double)num5;
                        }
                        if (num == 3)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = num3 + (double)num5;
                        }
                    }
                    if (Setting.TabsDic.Count == 5)
                    {
                        num2 = actualWidth / 2.0;
                        num3 = (actualHeight - (double)num5) / 2.0;
                        num4 = (actualHeight - (double)num5) / 3.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num == 0)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = num5;
                        }
                        if (num == 1)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = num5;
                            layoutDocument.FloatingHeight = num4;
                        }
                        if (num == 2)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = num3 + (double)num5;
                        }
                        if (num == 3)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = num4 + (double)num5;
                            layoutDocument.FloatingHeight = num4;
                        }
                        if (num == 4)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = num4 * 2.0 + (double)num5;
                            layoutDocument.FloatingHeight = num4;
                        }
                    }
                    if (Setting.TabsDic.Count == 6)
                    {
                        num2 = actualWidth / 3.0;
                        num3 = (actualHeight - (double)num5) / 2.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num >= 0 && num <= 2)
                        {
                            layoutDocument.FloatingLeft = num2 * (double)num;
                            layoutDocument.FloatingTop = num5;
                        }
                        if (num >= 3 && num <= 5)
                        {
                            layoutDocument.FloatingLeft = num2 * (double)(num - 3);
                            layoutDocument.FloatingTop = (double)num5 + num3;
                        }
                    }
                    if (Setting.TabsDic.Count == 7)
                    {
                        num2 = actualWidth / 3.0;
                        num3 = (actualHeight - (double)num5) / 2.0;
                        num4 = (actualHeight - (double)num5) / 3.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num == 0)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = num5;
                        }
                        if (num == 1)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = num5;
                        }
                        if (num == 2)
                        {
                            layoutDocument.FloatingLeft = num2 * 2.0;
                            layoutDocument.FloatingTop = num5;
                            layoutDocument.FloatingHeight = num4;
                        }
                        if (num == 3)
                        {
                            layoutDocument.FloatingLeft = 0.0;
                            layoutDocument.FloatingTop = (double)num5 + num3;
                            layoutDocument.FloatingHeight = num3;
                        }
                        if (num == 4)
                        {
                            layoutDocument.FloatingLeft = num2;
                            layoutDocument.FloatingTop = (double)num5 + num3;
                            layoutDocument.FloatingHeight = num3;
                        }
                        if (num == 5)
                        {
                            layoutDocument.FloatingLeft = num2 * 2.0;
                            layoutDocument.FloatingTop = num4 + (double)num5;
                            layoutDocument.FloatingHeight = num4;
                        }
                        if (num == 6)
                        {
                            layoutDocument.FloatingLeft = num2 * 2.0;
                            layoutDocument.FloatingTop = num4 * 2.0 + (double)num5;
                            layoutDocument.FloatingHeight = num4;
                        }
                    }
                    if (Setting.TabsDic.Count == 8)
                    {
                        num2 = actualWidth / 4.0;
                        num3 = (actualHeight - (double)num5) / 2.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num >= 0 && num <= 3)
                        {
                            layoutDocument.FloatingLeft = num2 * (double)num;
                            layoutDocument.FloatingTop = num5;
                        }
                        if (num >= 4 && num <= 7)
                        {
                            layoutDocument.FloatingLeft = num2 * (double)(num - 4);
                            layoutDocument.FloatingTop = (double)num5 + num3;
                        }
                    }
                    if (Setting.TabsDic.Count == 9)
                    {
                        num2 = actualWidth / 3.0;
                        num3 = (actualHeight - (double)num5) / 3.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num >= 0 && num <= 2)
                        {
                            layoutDocument.FloatingLeft = num2 * (double)num;
                            layoutDocument.FloatingTop = num5;
                        }
                        if (num >= 3 && num <= 5)
                        {
                            layoutDocument.FloatingLeft = num2 * (double)(num - 3);
                            layoutDocument.FloatingTop = (double)num5 + num3;
                        }
                        if (num >= 6 && num <= 8)
                        {
                            layoutDocument.FloatingLeft = num2 * (double)(num - 6);
                            layoutDocument.FloatingTop = (double)num5 + num3 * 2.0;
                        }
                    }
                    if (Setting.TabsDic.Count == 10)
                    {
                        num2 = actualWidth / 3.0;
                        num3 = (actualHeight - (double)num5) / 3.0;
                        double num6 = (actualHeight - (double)num5) / 4.0;
                        layoutDocument.FloatingWidth = num2;
                        layoutDocument.FloatingHeight = num3;
                        if (num >= 0 && num <= 2)
                        {
                            layoutDocument.FloatingLeft = num2 * (double)num;
                            layoutDocument.FloatingTop = num5;
                        }
                        if (num >= 3 && num <= 5)
                        {
                            layoutDocument.FloatingLeft = num2 * (double)(num - 3);
                            layoutDocument.FloatingTop = (double)num5 + num3;
                        }
                        if (num >= 6 && num <= 8)
                        {
                            layoutDocument.FloatingLeft = num2 * (double)(num - 6);
                            layoutDocument.FloatingTop = (double)num5 + num3 * 2.0;
                        }
                        if (num == 2)
                        {
                            layoutDocument.FloatingHeight = num6;
                        }
                        if (num == 5)
                        {
                            layoutDocument.FloatingHeight = num6;
                            layoutDocument.FloatingTop = (double)num5 + num6;
                        }
                        if (num == 8)
                        {
                            layoutDocument.FloatingHeight = num6;
                            layoutDocument.FloatingTop = (double)num5 + num6 * 2.0;
                        }
                        if (num == 9)
                        {
                            layoutDocument.FloatingHeight = num6;
                            layoutDocument.FloatingLeft = num2 * 2.0;
                            layoutDocument.FloatingTop = (double)num5 + num6 * 3.0;
                        }
                    }
                    if (layoutDocument.IsFloating)
                    {
                        layoutDocument.Dock();
                    }
                    layoutDocument.Float();
                    num++;
                }
            }
        }

        private void FourCell_Click(object sender, RoutedEventArgs e)
        {
            int num = 0;
            LayoutDocumentPaneGroup layoutDocumentPaneGroup = null;
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    if (num == 0)
                    {
                        LDPaneGroup.InsertChildAt(0, new LayoutDocumentPane((LayoutDocument)item.Key));
                        LDPaneGroup.Orientation = System.Windows.Controls.Orientation.Vertical;
                    }
                    if (num == 1)
                    {
                        LayoutDocumentPaneGroup layoutDocumentPaneGroup2 = new LayoutDocumentPaneGroup();
                        layoutDocumentPaneGroup2.Orientation = System.Windows.Controls.Orientation.Vertical;
                        layoutDocumentPaneGroup = layoutDocumentPaneGroup2;
                        layoutDocumentPaneGroup.InsertChildAt(0, new LayoutDocumentPane((LayoutDocument)item.Key));
                        lp.Children.Insert(1, layoutDocumentPaneGroup);
                    }
                    if (num == 2)
                    {
                        layoutDocumentPaneGroup.InsertChildAt(1, new LayoutDocumentPane((LayoutDocument)item.Key));
                    }
                    if (num == 3)
                    {
                        LDPaneGroup.InsertChildAt(1, new LayoutDocumentPane((LayoutDocument)item.Key));
                    }
                    ((LayoutDocument)item.Key).Root.CollectGarbage();
                    num++;
                }
            }
        }

        private void SexCell_Click(object sender, RoutedEventArgs e)
        {
            int num = 0;
            LayoutDocumentPaneGroup layoutDocumentPaneGroup = null;
            LayoutDocumentPaneGroup layoutDocumentPaneGroup2 = null;
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    if (num == 0)
                    {
                        LDPaneGroup.InsertChildAt(0, new LayoutDocumentPane((LayoutDocument)item.Key));
                        LDPaneGroup.Orientation = System.Windows.Controls.Orientation.Vertical;
                    }
                    if (num == 1)
                    {
                        LayoutDocumentPaneGroup layoutDocumentPaneGroup3 = new LayoutDocumentPaneGroup();
                        layoutDocumentPaneGroup3.Orientation = System.Windows.Controls.Orientation.Vertical;
                        layoutDocumentPaneGroup = layoutDocumentPaneGroup3;
                        layoutDocumentPaneGroup.InsertChildAt(0, new LayoutDocumentPane((LayoutDocument)item.Key));
                        lp.Children.Insert(1, layoutDocumentPaneGroup);
                    }
                    if (num == 2)
                    {
                        LayoutDocumentPaneGroup layoutDocumentPaneGroup4 = new LayoutDocumentPaneGroup();
                        layoutDocumentPaneGroup4.Orientation = System.Windows.Controls.Orientation.Vertical;
                        layoutDocumentPaneGroup2 = layoutDocumentPaneGroup4;
                        layoutDocumentPaneGroup2.InsertChildAt(0, new LayoutDocumentPane((LayoutDocument)item.Key));
                        lp.Children.Insert(2, layoutDocumentPaneGroup2);
                    }
                    if (num == 3)
                    {
                        LDPaneGroup.InsertChildAt(1, new LayoutDocumentPane((LayoutDocument)item.Key));
                    }
                    if (num == 4)
                    {
                        layoutDocumentPaneGroup.InsertChildAt(1, new LayoutDocumentPane((LayoutDocument)item.Key));
                    }
                    if (num == 5)
                    {
                        layoutDocumentPaneGroup2.InsertChildAt(1, new LayoutDocumentPane((LayoutDocument)item.Key));
                    }
                    ((LayoutDocument)item.Key).Root.CollectGarbage();
                    num++;
                }
            }
        }

        private void NienCell_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<object, object> item in Setting.TabsDic)
            {
                if (item.Value.GetType() == typeof(Mainpage))
                {
                    LayoutDocument layoutDocument = (LayoutDocument)item.Key;
                    layoutDocument.Float();
                }
            }
        }

        private void Image_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromArgb(byte.MaxValue, 190, 190, 190);
            Color color2 = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
            txt_men_horizontal.Foreground = new SolidColorBrush(color);
            txtmen_vertical.Foreground = new SolidColorBrush(color);
            txtmen_dock.Foreground = new SolidColorBrush(color);
            men_horizontal.IsEnabled = false;
            men_vertical.IsEnabled = false;
            men_dock.IsEnabled = false;
            OrderList.IsSubmenuOpen = true;
            if (Setting.TabsDic.Count >= 2)
            {
                men_horizontal.IsEnabled = true;
                men_vertical.IsEnabled = true;
                txt_men_horizontal.Foreground = new SolidColorBrush(color2);
                txtmen_vertical.Foreground = new SolidColorBrush(color2);
            }
            if (Setting.TabsDic.Count >= 1)
            {
                men_dock.IsEnabled = true;
                txtmen_dock.Foreground = new SolidColorBrush(color2);
            }
        }

        private void men_dock_Click(object sender, RoutedEventArgs e)
        {
            addEvent();
            HiddMenu();
            List<LayoutDocument> list = new List<LayoutDocument>();
            foreach (KeyValuePair<object, object> item2 in Setting.TabsDic)
            {
                if (item2.Value.GetType() == typeof(Mainpage))
                {
                    LayoutDocument item = (LayoutDocument)item2.Key;
                    list.Add(item);
                }
            }
            for (int num = list.Count - 1; num >= 0; num--)
            {
                if (list[num].IsFloating)
                {
                    list[num].Dock();
                }
            }
            Setting.IsFloat = 0;
        }
    }
}
