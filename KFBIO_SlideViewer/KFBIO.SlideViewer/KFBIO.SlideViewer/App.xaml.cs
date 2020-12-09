using KFBIO.Common;
using LanguageLib.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace KFBIO.SlideViewer
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private Semaphore singleInstanceWatcher;

        private bool createdNew;

        public App()
        {
            base.Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                CultureInfo cultureInfo = null;
                if (Thread.CurrentThread.CurrentCulture.Name == "zh-CN")
                {
                    Setting.Language = "0";
                    cultureInfo = new CultureInfo("zh-Hans");
                }
                else
                {
                    Setting.Language = "1";
                    cultureInfo = new CultureInfo("en-US");
                }
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                MultiLanguage.Culture = cultureInfo;
                singleInstanceWatcher = new Semaphore(0, 1, Assembly.GetExecutingAssembly().GetName().Name, out createdNew);
                string text = string.Empty;
                string empty = string.Empty;
                if (e.Args.Length > 0)
                {
                    empty = e.Args[0].Substring(e.Args[0].LastIndexOf('.'));
                    text = ((empty.ToLower() == ".kfb") ? e.Args[0] : ((empty.ToLower() == ".case") ? e.Args[0].Substring(0, e.Args[0].Length - empty.Length) : ((!(empty.ToLower() == ".ano")) ? e.Args[0] : e.Args[0].Substring(0, e.Args[0].Length - empty.Length))));
                }
                if (createdNew)
                {
                    DockWindow dockWindow = new DockWindow();
                    if (e.Args.Length == 1)
                    {
                        if (File.Exists(text))
                        {
                            if (Setting.IsMutiScreen == "1" && Screen.AllScreens.Length > 1)
                            {
                                dockWindow.WindowState = System.Windows.WindowState.Minimized;
                                Screen screen = Screen.AllScreens[Screen.AllScreens.Length - 1];
                                Rectangle workingArea = screen.WorkingArea;
                                dockWindow.Top = workingArea.Y;
                                dockWindow.Left = workingArea.X;
                            }
                            dockWindow.Show();
                            dockWindow.Left = 0.0;
                            dockWindow.Top = 0.0;
                            dockWindow.Height = SystemParameters.WorkArea.Height;
                            dockWindow.Width = SystemParameters.WorkArea.Width;
                            if (KCommon.CheckVersion(text))
                            {
                                Mainpage content = new Mainpage(text);
                                string text2 = text.Substring(text.LastIndexOf("\\") + 1, text.Length - text.LastIndexOf("\\") - 1);
                                text2 = text2.Substring(0, text2.Length - 4);
                                BitmapImage kFBThumnail = KCommon.GetKFBThumnail(text);
                                if (kFBThumnail.StreamSource == null)
                                {
                                    MessageWind messageWind = new MessageWind(MessageBoxButton.OK, System.Windows.Application.Current.MainWindow, dockWindow.languageSetter.LanguageResource["Filedamage"], dockWindow.languageSetter.LanguageResource["Prompt"], MessageBoxIcon.Exclamation, false);
                                    messageWind.ShowDialog();
                                    dockWindow.Close();
                                }
                                else
                                {
                                    dockWindow.AddItem(text + DateTime.Now.ToString("yyyyMMddhhmmss"), text2, content, kFBThumnail);
                                    dockWindow.imgs._evm.SearchViewModel.UIConfirmedParseName = text.Substring(0, text.LastIndexOf("\\"));
                                }
                            }
                        }
                        else
                        {
                            MessageWind messageWind2 = new MessageWind(MessageBoxButton.OK, System.Windows.Application.Current.MainWindow, "文件不存在！", dockWindow.languageSetter.LanguageResource["Prompt"], MessageBoxIcon.Exclamation, false);
                            messageWind2.ShowDialog();
                            dockWindow.Close();
                        }
                    }
                    else
                    {
                        if (Setting.IsMutiScreen == "1" && Screen.AllScreens.Length > 1)
                        {
                            dockWindow.WindowState = System.Windows.WindowState.Minimized;
                            Screen screen2 = Screen.AllScreens[Screen.AllScreens.Length - 1];
                            Rectangle workingArea2 = screen2.WorkingArea;
                            dockWindow.Top = workingArea2.Y;
                            dockWindow.Left = workingArea2.X;
                            dockWindow.Width = workingArea2.Width;
                            dockWindow.Height = workingArea2.Height;
                        }
                        dockWindow.Show();
                        dockWindow.Left = 0.0;
                        dockWindow.Top = 0.0;
                        dockWindow.Height = SystemParameters.WorkArea.Height;
                        dockWindow.Width = SystemParameters.WorkArea.Width;
                        dockWindow.imagelist.Show();
                    }
                }
                else
                {
                    Process currentProcess = Process.GetCurrentProcess();
                    Process proc = null;
                    Process[] processesByName = Process.GetProcessesByName(currentProcess.ProcessName);
                    foreach (Process process in processesByName)
                    {
                        if (process.Id != currentProcess.Id)
                        {
                            NativeMethods.ShowWindow(process.MainWindowHandle, WindowShowStyle.ShowNormal);
                            NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                            proc = process;
                            break;
                        }
                    }
                    if (text.Length > 0)
                    {
                        writetemp(text);
                        PostMessage(proc, text);
                    }
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Xceed.Wpf.Toolkit.MessageBox.Show(ex.ToString());
            }
        }

        private void writetemp(string line)
        {
            string str = "temp";
            try
            {
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string text = Path.Combine(folderPath, "KFBIO\\K-Viewer");
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                }
                string path = text + "\\" + str;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Unicode);
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

        private void PostMessage(Process proc, string path)
        {
            string text = "aaaaa";
            CopyDataStruc lParam = default(CopyDataStruc);
            lParam.dwData = IntPtr.Zero;
            lParam.lpData = text;
            lParam.cbData = Encoding.Default.GetBytes(text).Length + 1;
            NativeMethods.SendMessage(proc.MainWindowHandle, 170, 0, ref lParam);
        }
    }

}
