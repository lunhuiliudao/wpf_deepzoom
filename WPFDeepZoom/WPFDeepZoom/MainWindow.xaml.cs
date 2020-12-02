using CommonClassLibrary;
using MultilayerZoom;
using MultilayerZoom.Controls;
using SliceInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZYSlice;

namespace WPFDeepZoom
{
    public struct CopyDataStruct
    {
        public IntPtr dwData;

        public int cbData;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }

    public partial class MainWindow : Window
    {
        public struct CopyDataStruct
        {
            public IntPtr dwData;

            public int cbData;

            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        private int iDefaultAngle;

        private Grid gMainMultiScaleImageBox;

        private MultiScaleImage mMainMultiScaleImage;

        private Grid gMainMarkBox;

        public static ResourceDictionary rdDirectory = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Themes/Style.xaml")
        };

        private float fScreenRatio;

        private float fNavigatorWidth;

        private float fNavigatorHeight;

        private string strCurrentFolderPath;

        private ZYPSlice zypSlide = new ZYPSlice();

        private DeepZoomImageTileSource dtTileSource;

        private bool bNavigationChangeSizeToolPress;

        private System.Windows.Point pMouseStartPosition;

        private System.Windows.Point pNavigatorBoxSize;

        private int iAngle;

        private bool bRotateToolPress;

        private bool bZoomToolPress;

        private bool rViewportPress;

        private float fStartY;

        private float fStartTop;

        private float fMoveY;

        private System.Windows.Point pStartPosition;

        private Dictionary<Border, FrameworkElement> dtTool;

        private bool bStartMeasure;

        private int iMeasurePointCount;

        private Measure mtMeasureTool;

        private System.Windows.Point pMouseDownPointUseForMeasuring;

        private System.Windows.Point pMouseDownPointUseForMarking;

        private System.Windows.Point pMouseDownPointUseForSlideMoving;

        private float fWidthPerPixel;

        private Mark mCurrentMark;

        private bool bAddMarkState;

        private bool bEditMarkState;

        private CancellationTokenSource ctsCurrentSource;

        private string strCurrentSlidePath;

        private System.Windows.Media.Brush bLineColor = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));

        private float fLineWidth = 2f;

        private bool bMarkPress;

        private bool bSlidePress;

        private bool bSlidePressAndLoseFocus;

        private System.Windows.Point pSlideLoseFocusPoint;

        private bool bDoubleClickTimeOut;

        private System.Windows.Point pStartRowColumn = new System.Windows.Point(-1.0, -1.0);

        private System.Windows.Point pOverRowColumn = new System.Windows.Point(0.0, 0.0);

        private bool bCheckedPermissions;

        private System.Windows.Point pMouseDownPointUseForShowInfo;

        private bool bMagnifierChangeSizeToolPress;

        private const int WM_COPYDATA = 74;

        public MainWindow()
        {
            PublicMethods.SetRegedit();
            InitializeComponent();
            dtTool = new Dictionary<Border, FrameworkElement>
            {
                {
                    bSlideList,
                    bChooseSlideToolBorder
                },
                {
                    bRotate,
                    bRotateToolBorder
                },
                {
                    bMeasure,
                    null
                },
                {
                    bMark,
                    bMarkToolBorder
                },
                {
                    bMarkList,
                    bMarkListBorder
                },
                {
                    bViewInfo,
                    bViewInfoBorder
                },
                {
                    bSplitScreen,
                    bSplitScreenToolBorder
                },
                {
                    bScanInfo,
                    bScanInfoBorder
                },
                {
                    bCamera,
                    gCameraPannelBG
                },
                {
                    bMirror,
                    bMirrorToolBorder
                },
                {
                    bMagnifier,
                    bMagnifierBorder
                },
                {
                    bExport,
                    null
                },
                {
                    borderLanguage,
                    bLanguage
                }
            };

            foreach (Border child in sToolBar.Children)
            {
                child.MouseLeftButtonDown += ToolBarBtn_MouseLeftButtonDown;
                child.MouseEnter += ToolBarBtn_MouseEnter;
                child.MouseLeave += ToolBarBtn_MouseLeave;
            }

            bSlideList.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 176, 181));
            fScreenRatio = PublicMethods.GetScreenRatio(this);
            foreach (System.Windows.Controls.Label child2 in cAllShape.Children)
            {
                child2.MouseLeftButtonDown += lShape_MouseLeftButtonDown;
            }
            foreach (System.Windows.Controls.Label child3 in sChooseColor.Children)
            {
                child3.MouseLeftButtonDown += lColor_MouseLeftButtonDown;
            }
            foreach (System.Windows.Controls.Label child4 in sEditColor.Children)
            {
                child4.MouseLeftButtonDown += lEditMarkPannelColorBox_MouseDown;
            }
            foreach (Border child5 in gAllRect.Children)
            {
                child5.MouseLeftButtonDown += lRect_MouseLeftButtonDown;
                child5.MouseMove += lRect_MouseMove;
                child5.MouseUp += lRect_MouseUp;
            }
            Border border = SplitScreenTool.CreateSlideBox(rdDirectory);
            AddEventForSlideBox(border);
            gAllSlideBox.Children.Add(border);
            SetCurrentSlide(border.Child as Grid);
            mNavigatorImage.bMouseWheelAvailable = false;
            mNavigatorImage.bMouseDragAvailable = false;
            mMagnifier.bMouseDragAvailable = false;
            mMagnifier.bOnlyCenterScale = true;
            mMagnifier.ScaleOrOffsetChanged += mMagnifier_MouseWheel;
            MovePannel.AddMovement(bMagnifierBorder, this);
            MovePannel.AddMovement(bEditMarkPannel, this);
            MovePannel.AddMovement(sCameraPannel, this);
            MovePannel.PannelMoved += delegate (object sender)
            {
                if (sender as FrameworkElement == bMagnifierBorder && lFixed.Background.ToString() == "#FFFFFFFF")
                {
                    MagnifierTool.MagnifierZoomAboutCenter(bMagnifierBorder, this, gAllSlideBox, fScreenRatio);
                }
            };
            AddEventForMagnifier();
            string key = IniSetting.GetKey(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/Configuration.ini", "System", "DefaultAngle");
            if (key != null && key.Trim() != "")
            {
                try
                {
                    iDefaultAngle = int.Parse(key);
                }
                catch
                {
                }
            }
            MultiScaleImage.fScreenRatio = fScreenRatio + 0.03f;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref CopyDataStruct lParam);

        private void ToolBarBtn_MouseEnter(object sender, RoutedEventArgs e)
        {
            (sender as Border).Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 176, 181));
        }

        private void ToolBarBtn_MouseLeave(object sender, RoutedEventArgs e)
        {
            Border border = sender as Border;
            FrameworkElement frameworkElement = dtTool[border];
            if (bMeasure == border)
            {
                if (!bStartMeasure)
                {
                    border.Background = new SolidColorBrush();
                }
            }
            else if (bExport == border || frameworkElement.Visibility != 0)
            {
                border.Background = new SolidColorBrush();
            }
        }

        private void ToolBarBtn_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            Border border = sender as Border;
            if (border == bMeasure)
            {
                if (CheckSlideState())
                {
                    if (!bStartMeasure)
                    {
                        CloseNormalTool();
                        CloseMarkTool();
                        bStartMeasure = true;
                        bMeasure.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 176, 181));
                        mtMeasureTool = new Measure(gMainMultiScaleImageBox);
                    }
                    else
                    {
                        CloseMeasureTool();
                    }
                }
                return;
            }
            if (border == bMark)
            {
                if (bMarkToolBorder.Visibility == Visibility.Collapsed)
                {
                    CloseNormalTool();
                    CloseMeasureTool();
                    bMarkToolBorder.Visibility = Visibility.Visible;
                    bMark.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 176, 181));
                }
                else
                {
                    CloseMarkTool();
                }
                return;
            }
            if (border == bViewInfo)
            {
                if (!CheckSlideState())
                {
                    return;
                }
                if (bViewInfoBorder.Visibility == Visibility.Collapsed)
                {
                    if (bCheckedPermissions)
                    {
                        CloseNormalTool();
                        bViewInfoBorder.Visibility = Visibility.Visible;
                        bViewInfo.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 176, 181));
                        CloseMeasureTool();
                        CloseMarkTool();
                        return;
                    }
                    bPasswordBG.Visibility = Visibility.Visible;
                    if (new Password().ShowDialog() == true)
                    {
                        bCheckedPermissions = true;
                        CloseNormalTool();
                        CloseMeasureTool();
                        bViewInfoBorder.Visibility = Visibility.Visible;
                        bViewInfo.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 176, 181));
                    }
                    bPasswordBG.Visibility = Visibility.Collapsed;
                }
                else
                {
                    bViewInfoBorder.Visibility = Visibility.Collapsed;
                }
                return;
            }
            FrameworkElement frameworkElement = dtTool[border];
            if (frameworkElement.Visibility == Visibility.Collapsed || frameworkElement.Visibility == Visibility.Hidden)
            {
                if (!CheckSlideState())
                {
                    return;
                }
                CloseNormalTool();
                CloseMeasureTool();
                if (!bAddMarkState && !bEditMarkState)
                {
                    CloseMarkTool();
                }
                if (border == bCamera)
                {
                    if (mMainMultiScaleImage.Source == null)
                    {
                        System.Windows.MessageBox.Show(GlobalVariable.listCurrentLanguage[57]);
                        return;
                    }
                    MovePannel.RestorePosition(sCameraPannel, this);
                    ScreenCaptureTool.InitializeTool(mMainMultiScaleImage, imgPreview, bNavigation, imgDragTool, imgLabel, pShowMarksChecked, pShowRulerChecked, pShowNavigationChecked, pShowSlideLabelChecked, fScreenRatio, rdDirectory);
                }
                else if (border == bMirror)
                {
                    if (mMainMultiScaleImage.Source == null)
                    {
                        System.Windows.MessageBox.Show(GlobalVariable.listCurrentLanguage[57]);
                        return;
                    }
                }
                else if (border == bMagnifier && lFixed.Background.ToString() == "#FFFFFFFF")
                {
                    MagnifierTool.MagnifierZoomAboutCenter(bMagnifierBorder, this, gAllSlideBox, fScreenRatio);
                }
                frameworkElement.Visibility = Visibility.Visible;
                border.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 176, 181));
            }
            else
            {
                frameworkElement.Visibility = Visibility.Collapsed;
            }
        }

        private void lSplitScreenToolHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bSplitScreenToolBorder.Visibility = Visibility.Collapsed;
            ToolBarBtn_MouseLeave(bSplitScreen, new RoutedEventArgs());
        }

        private void lRotateToolHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bRotateToolBorder.Visibility = Visibility.Collapsed;
            ToolBarBtn_MouseLeave(bRotate, new RoutedEventArgs());
        }

        private void lMarkListHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bMarkListBorder.Visibility = Visibility.Collapsed;
            ToolBarBtn_MouseLeave(bMarkList, new RoutedEventArgs());
        }

        private void lMarkToolHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bMarkToolBorder.Visibility = Visibility.Collapsed;
            ToolBarBtn_MouseLeave(bMark, new RoutedEventArgs());
        }

        private void lScanInfoHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bScanInfoBorder.Visibility = Visibility.Collapsed;
            ToolBarBtn_MouseLeave(bScanInfo, new RoutedEventArgs());
        }

        private void lCameraPannelHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gCameraPannelBG.Visibility = Visibility.Collapsed;
            ToolBarBtn_MouseLeave(bCamera, new RoutedEventArgs());
        }

        private void lMirrorToolHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bMirrorToolBorder.Visibility = Visibility.Collapsed;
            ToolBarBtn_MouseLeave(bMirror, new RoutedEventArgs());
        }

        private void lMagnifierHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            bMagnifierBorder.Visibility = Visibility.Collapsed;
            ToolBarBtn_MouseLeave(bMagnifier, new RoutedEventArgs());
        }

        private void CloseNormalTool()
        {
            foreach (FrameworkElement value in dtTool.Values)
            {
                if (value != null)
                {
                    value.Visibility = Visibility.Collapsed;
                }
            }
            foreach (Border key in dtTool.Keys)
            {
                key.Background = new SolidColorBrush();
            }
        }

        private void CloseMarkTool(int iType = 1)
        {
            Mark.CloseMarkTool(bMarkToolBorder, gAllSlideBox, cAllShape, iType);
            bEditMarkState = false;
            bAddMarkState = false;
            iMeasurePointCount = 0;
        }

        private void CloseMeasureTool()
        {
            if (bStartMeasure)
            {
                bStartMeasure = false;
                mtMeasureTool.Close(gAllSlideBox);
                mtMeasureTool = null;
                bMeasure.Background = new SolidColorBrush();
            }
        }

        private void bChooseSlideToolBorder_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Border border = sender as Border;
            if (cbAutoHide.IsChecked == true && bLoading.Visibility == Visibility.Collapsed)
            {
                border.Visibility = Visibility.Collapsed;
                ToolBarBtn_MouseLeave(bSlideList, new RoutedEventArgs());
            }
        }

        private void btnChooseFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = GlobalVariable.listCurrentLanguage[14];
                folderBrowserDialog.SelectedPath = strCurrentFolderPath;
                if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
                {
                    string text = folderBrowserDialog.SelectedPath.Trim();
                    if (!(text == strCurrentFolderPath))
                    {
                        SplitScreenTool.ClearSplitScreen(gAllSlideBox);
                        SplitScreenTool.GetSelectedArea(gAllRect, ref pStartRowColumn, ref pOverRowColumn);
                        SplitScreen();
                        sZoomTool.Visibility = Visibility.Collapsed;
                        sMoveTool.Visibility = Visibility.Collapsed;
                        cLabelTool.Visibility = Visibility.Collapsed;
                        bNavigation.Visibility = Visibility.Hidden;
                        sMarkList.Children.Clear();
                        sDefaultScanInfo.Visibility = Visibility.Visible;
                        gScanInfo.Visibility = Visibility.Collapsed;
                        CreateSlideList(text);
                    }
                }
            }
            catch
            {
            }
        }

        private void bRefresh_Click(object sender, RoutedEventArgs e)
        {
            CreateSlideList(strCurrentFolderPath);
        }

        private void bThumbnailBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            string text = border.DataContext.ToString();
            if (!(mMainMultiScaleImage.DataContext as string == text))
            {
                foreach (Border child in sSlideList.Children)
                {
                    child.BorderBrush = System.Windows.Media.Brushes.Gray;
                }
                border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 176, 181));
                gMainMarkBox.Children.Clear();
                OpenSlide(text);
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            SlidelistTool.SearchSlideList((sender as System.Windows.Controls.TextBox).Text, sSlideList);
        }

        private void tbSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox.Text == GlobalVariable.listCurrentLanguage[16])
            {
                textBox.Text = "";
                textBox.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            }
        }

        private void tbSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox.Text == "")
            {
                textBox.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(169, 169, 169));
                textBox.Text = GlobalVariable.listCurrentLanguage[16];
            }
        }

        private void CreateSlideList(string strFolderPath)
        {
            strCurrentFolderPath = strFolderPath;
            lCurrentFolder.Content = GlobalVariable.listCurrentLanguage[18] + ": " + strFolderPath;
            sSlideList.Children.Clear();
            if (ctsCurrentSource != null)
            {
                ctsCurrentSource.Cancel();
            }
            CancellationTokenSource ctsNewSource = new CancellationTokenSource();
            Task.Factory.StartNew(delegate
            {
                base.Dispatcher.Invoke(delegate
                {
                    SlidelistTool.CreateSlideList(sSlideList, strFolderPath, zypSlide, ctsNewSource, rdDirectory, strCurrentSlidePath);
                });
            }).ContinueWith(delegate
            {
                base.Dispatcher.Invoke(delegate
                {
                    if (sSlideList.Children.Count == 0)
                    {
                        System.Windows.MessageBox.Show(GlobalVariable.listCurrentLanguage[89] + "!");
                    }
                    else
                    {
                        foreach (Border child in sSlideList.Children)
                        {
                            child.MouseLeftButtonDown += bThumbnailBorder_MouseLeftButtonDown;
                        }
                    }
                });
            });
            ctsCurrentSource = ctsNewSource;
        }

        private void bNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                rViewportPress = true;
                System.Windows.Point pNavigatorRatio = pStartPosition = e.GetPosition(mNavigatorImage);
                pNavigatorRatio.X /= mNavigatorImage.ActualWidth;
                pNavigatorRatio.Y /= mNavigatorImage.ActualHeight;
                System.Windows.Point pFlipDelta = mMainMultiScaleImage.RatioOffsetToDeltaOffset(pNavigatorRatio);
                pFlipDelta = PublicMethods.FlipDelta(gMainMultiScaleImageBox, pFlipDelta);
                UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pFlipDelta, new System.Windows.Point(0.0, 0.0));
            }
        }

        private void bNavigation_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (rViewportPress && !(pStartPosition == e.GetPosition(mNavigatorImage)))
            {
                System.Windows.Point position = e.GetPosition(mNavigatorImage);
                position.X /= mNavigatorImage.ActualWidth;
                position.Y /= mNavigatorImage.ActualHeight;
                System.Windows.Point pFlipDelta = mMainMultiScaleImage.RatioOffsetToDeltaOffset(position);
                pFlipDelta = PublicMethods.FlipDelta(gMainMultiScaleImageBox, pFlipDelta);
                UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pFlipDelta, new System.Windows.Point(0.0, 0.0));
            }
        }

        private void bNavigation_MouseUp(object sender, MouseButtonEventArgs e)
        {
            rViewportPress = false;
        }

        private void bNavigation_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            rViewportPress = false;
        }

        private void imgDragTool_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (CheckSlideState())
            {
                rViewportPress = false;
                bNavigationChangeSizeToolPress = true;
                pMouseStartPosition = e.GetPosition(this);
                pNavigatorBoxSize = new System.Windows.Point(bNavigation.Width, bNavigation.Height);
            }
        }

        private void gNavigatorImageBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (mMainMultiScaleImage.Source != null)
            {
                System.Windows.Size imageSize = mMainMultiScaleImage.Source.ImageSize;
                double width = e.NewSize.Width;
                double dTargetScale = Math.Min(val2: e.NewSize.Height / imageSize.Height, val1: width / imageSize.Width);
                System.Windows.Point pTargetOffset = new System.Windows.Point(0.0, 0.0);
                mNavigatorImage.TargetChangeScale(dTargetScale, pTargetOffset);
                NavigatorTool.UpdateNavigationViewport(mMainMultiScaleImage, mNavigatorImage, rViewport, lHorLine, lVerLine);
            }
        }

        private void lRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            pStartRowColumn = new System.Windows.Point(Grid.GetRow(border), Grid.GetColumn(border));
            pOverRowColumn = pStartRowColumn;
            foreach (Border child in gAllRect.Children)
            {
                child.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
            }
            border.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 176, 181));
        }

        private void lRect_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (pStartRowColumn != new System.Windows.Point(-1.0, -1.0))
            {
                Border element = sender as Border;
                int row = Grid.GetRow(element);
                int column = Grid.GetColumn(element);
                pOverRowColumn = new System.Windows.Point(row, column);
                int num;
                int num2;
                if (pStartRowColumn.X < (double)row)
                {
                    num = (int)pStartRowColumn.X;
                    num2 = row;
                }
                else
                {
                    num = row;
                    num2 = (int)pStartRowColumn.X;
                }
                int num3;
                int num4;
                if (pStartRowColumn.Y < (double)column)
                {
                    num3 = (int)pStartRowColumn.Y;
                    num4 = column;
                }
                else
                {
                    num3 = column;
                    num4 = (int)pStartRowColumn.Y;
                }
                foreach (Border child in gAllRect.Children)
                {
                    int row2 = Grid.GetRow(child);
                    int column2 = Grid.GetColumn(child);
                    if (num <= row2 && row2 <= num2 && num3 <= column2 && column2 <= num4)
                    {
                        child.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(57, 176, 181));
                    }
                    else
                    {
                        child.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue));
                    }
                }
            }
        }

        private void lRect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (pStartRowColumn != new System.Windows.Point(-1.0, -1.0))
            {
                SplitScreen();
            }
        }

        private void gAllRect_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (pStartRowColumn != new System.Windows.Point(-1.0, -1.0))
            {
                SplitScreen();
            }
        }

        private void SplitScreen()
        {
            SplitScreenTool.FocusArea(pStartRowColumn, pOverRowColumn, gAllRect);
            foreach (Border item in SplitScreenTool.SplitScreen(pStartRowColumn, pOverRowColumn, gAllSlideBox, rdDirectory))
            {
                AddEventForSlideBox(item);
            }
            if (SplitScreenTool.CurrentSlideEmpty(gAllSlideBox, gMainMultiScaleImageBox))
            {
                strCurrentSlidePath = null;
                foreach (FrameworkElement child in gAllSlideBox.Children)
                {
                    if (child is Border)
                    {
                        SetCurrentSlide((child as Border).Child as Grid);
                        break;
                    }
                }
            }
            else if (gAllSlideBox.Children.Count > 1)
            {
                SplitScreenTool.AddActiveStyle(gMainMultiScaleImageBox);
            }
            pStartRowColumn = new System.Windows.Point(-1.0, -1.0);
            pOverRowColumn = new System.Windows.Point(-1.0, -1.0);
        }

        private void OpenSlide(string strSlidePath)
        {
            try
            {
                mMainMultiScaleImage.OnApplyTemplate();
                mMainMultiScaleImage.CloseSource();
                bool bSuccessOpen = true;
                strCurrentSlidePath = strSlidePath;
                mMainMultiScaleImage.DataContext = strSlidePath;
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += delegate
                {
                    dtTileSource = new DeepZoomImageTileSource(new Uri(strSlidePath));
                    bSuccessOpen = dtTileSource.bSuccessOpen;
                };
                backgroundWorker.RunWorkerCompleted += delegate
                {
                    if (!bSuccessOpen)
                    {
                        System.Windows.MessageBox.Show(GlobalVariable.listCurrentLanguage[90] + "!");
                        bLoading.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        RotateTool.RotateSlide(gMainMultiScaleImageBox, iDefaultAngle);
                        mMainMultiScaleImage.UpdateLayout();
                        mMainMultiScaleImage.Source = dtTileSource;
                        gMainMultiScaleImageBox.Visibility = Visibility.Visible;
                        NavigatorTool.ResetNavigatorSizeAndSource(mMainMultiScaleImage, mNavigatorImage, ref fNavigatorWidth, ref fNavigatorHeight, fScreenRatio);
                        FlatTool.ResetFlatTool(gMainMultiScaleImageBox);
                        sMarkList.Children.Clear();
                        foreach (KeyValuePair<Mark, StackPanel> item in Mark.GetMark(mMainMultiScaleImage, gMainMarkBox, fScreenRatio))
                        {
                            AddEventForMark(item.Key, item.Value);
                        }
                        ViewRectTool.DeleteSingleSlideViewRect(gMainMultiScaleImageBox);
                        if (lGridShow.Style == rdDirectory["SwitchBtnActive"])
                        {
                            ViewRectTool.CreateSingleSlideViewRect(gMainMultiScaleImageBox, bViewInfoBorder, gViewInfo);
                        }
                        ResetZoomTool();
                        if (PublicMethods.GetSlideBoxFlipValue(gMainMultiScaleImageBox) != new System.Windows.Point(1.0, 1.0))
                        {
                            FlipSlide(gMainMultiScaleImageBox, 1.0, 1.0, 0, 2);
                        }
                        RotateTool.RotateNavigator(bNavigation, mNavigatorImage, PublicMethods.GetElementActualAngle(mMainMultiScaleImage), fNavigatorWidth, fNavigatorHeight);
                        iAngle = PublicMethods.GetElemenRelativeAngle(mMainMultiScaleImage);
                        RotateTool.RotateIcon(iAngle, iDefaultAngle, imgRotate, lAngleNum);
                        ViewRectTool.UpdateViewRect(gMainMultiScaleImageBox, true);
                        Mark.UpdateAllMarks(gMainMultiScaleImageBox, fScreenRatio);
                        sZoomTool.Visibility = Visibility.Visible;
                        sMoveTool.Visibility = Visibility.Visible;
                        cLabelTool.Visibility = Visibility.Visible;
                        bNavigation.Visibility = Visibility.Visible;
                        (gMainMultiScaleImageBox.FindName("bSlideNameBorder") as Border).Visibility = Visibility.Visible;
                        (gMainMultiScaleImageBox.FindName("lSlideName") as System.Windows.Controls.Label).Content = System.IO.Path.GetFileNameWithoutExtension(strSlidePath);
                        Mark.UpdateAllMarkSwitch(gMainMarkBox, lAllMarkShow, lAllMarkHide, rdDirectory);
                        Mark.UpdateAllMarkInfoSwitch(gMainMarkBox, lAllInfoShow, lAllInfoHide, rdDirectory);
                        ZoomTool.UpdateZoomValue(mMainMultiScaleImage, lZoomValue, rController, lValueLine, fScreenRatio, lOne, lDefault);
                        RulerTool.UpdateRuler(gMainMultiScaleImageBox, ref fWidthPerPixel);
                        ScanInfoTool.GetSlideScanInfo(mMainMultiScaleImage, sDefaultScanInfo, gScanInfo);
                        LabelTool.ResetLabelTool(imgLabel, bLabelToggle, cLabelTool, lLabelToggle);
                        LabelTool.UpdateSlideLabel(strSlidePath, imgLabel, cLabelTool, zypSlide);
                        bLoading.Visibility = Visibility.Collapsed;
                    }
                };
                bLoading.Visibility = Visibility.Visible;
                backgroundWorker.RunWorkerAsync();
            }
            catch
            {
                System.Windows.MessageBox.Show(GlobalVariable.listCurrentLanguage[91] + "!");
                bLoading.Visibility = Visibility.Collapsed;
            }
        }

        public void ChangeOtherSlidePriority(object sender, int iType)
        {
            foreach (FrameworkElement child in gAllSlideBox.Children)
            {
                if (child is Border)
                {
                    Grid grid = (child as Border).Child as Grid;
                    MultiScaleImage multiScaleImage = grid.FindName("mMultiScaleImage") as MultiScaleImage;
                    if (multiScaleImage.Source != null)
                    {
                        if ((cbJoinSlide.IsChecked == true || multiScaleImage == mMainMultiScaleImage) && (iType == 1 || iType == 2 || iType == 5))
                        {
                            ViewRectTool.CloseViewRectAnimation(grid);
                            Mark.CloseAllMarksAnimation(grid);
                        }
                        if (multiScaleImage != mMainMultiScaleImage)
                        {
                            switch (iType)
                            {
                                case 1:
                                    multiScaleImage.SetMouseDownPriority();
                                    multiScaleImage.CloseSelfAnimation();
                                    break;
                                case 2:
                                    multiScaleImage.SetDoubleClickPriority();
                                    multiScaleImage.CloseSelfAnimation();
                                    break;
                                case 3:
                                    multiScaleImage.SetStopMoveTimeoutPriority();
                                    break;
                                case 4:
                                    multiScaleImage.SetStopMovePriority();
                                    break;
                                case 5:
                                    multiScaleImage.SetParentDoubleClickPriority();
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void lAngleLess_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState() && PublicMethods.CheckNumber(tMinDelta))
            {
                iAngle -= int.Parse(tMinDelta.Text);
                if (iAngle < 0)
                {
                    iAngle += 360;
                }
                RotateAllSlide();
            }
        }

        private void lAngleAdd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState() && PublicMethods.CheckNumber(tMinDelta))
            {
                iAngle += int.Parse(tMinDelta.Text);
                iAngle %= 360;
                RotateAllSlide();
            }
        }

        private void lAngleReset_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                iAngle = iDefaultAngle;
                RotateAllSlide();
            }
        }

        private void bDragRotate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                bRotateToolPress = true;
            }
        }

        private void bDragRotate_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (bRotateToolPress)
            {
                System.Windows.Point position = e.GetPosition(bDragRotate);
                double num = position.X - 100.0;
                double num2 = 80.0 - position.Y;
                double num3 = 0.0;
                if (num > 0.0 && num2 > 0.0)
                {
                    num3 = Math.Atan(Math.Abs(num) / Math.Abs(num2)) * 180.0 / Math.PI;
                }
                else if (num > 0.0 && num2 < 0.0)
                {
                    num3 = Math.Atan(Math.Abs(num2) / Math.Abs(num)) * 180.0 / Math.PI;
                    num3 += 90.0;
                }
                else if (num < 0.0 && num2 < 0.0)
                {
                    num3 = Math.Atan(Math.Abs(num) / Math.Abs(num2)) * 180.0 / Math.PI;
                    num3 += 180.0;
                }
                else
                {
                    num3 = Math.Atan(Math.Abs(num2) / Math.Abs(num)) * 180.0 / Math.PI;
                    num3 += 270.0;
                }
                iAngle = (int)num3;
                if (iAngle < 0)
                {
                    iAngle += 360;
                }
                iAngle += iDefaultAngle;
                iAngle %= 360;
                RotateAllSlide();
            }
        }

        private void bDragRotate_MouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            bRotateToolPress = false;
        }

        private void bDragRotate_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            bRotateToolPress = false;
        }

        private void AngleLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!CheckSlideState())
                {
                    e.Handled = true;
                }
                else
                {
                    System.Windows.Controls.Label label = sender as System.Windows.Controls.Label;
                    iAngle = int.Parse(label.DataContext.ToString());
                    iAngle += iDefaultAngle;
                    iAngle %= 360;
                    RotateAllSlide();
                }
            }
            catch
            {
            }
        }

        private void RotateSingleSlideAndUpdateTools(Grid gSlideBox, int iTargetActualAngle)
        {
            RotateTool.RotateSlide(gSlideBox, iTargetActualAngle);
            MultiScaleImage multiScaleImage = gSlideBox.FindName("mMultiScaleImage") as MultiScaleImage;
            if (gSlideBox == gMainMultiScaleImageBox)
            {
                int num = 0;
                num = ((multiScaleImage.Source != null) ? PublicMethods.GetElemenRelativeAngle(multiScaleImage) : iDefaultAngle);
                RotateTool.RotateIcon(num, iDefaultAngle, imgRotate, lAngleNum);
                RotateTool.RotateNavigator(bNavigation, mNavigatorImage, iTargetActualAngle, fNavigatorWidth, fNavigatorHeight);
                NavigatorTool.UpdateNavigationViewport(mMainMultiScaleImage, mNavigatorImage, rViewport, lHorLine, lVerLine);
            }
            gSlideBox.FindName("gMarkBox");
            ViewRectTool.UpdateViewRect(gSlideBox, true);
            Mark.UpdateAllMarks(gSlideBox, fScreenRatio);
        }

        public void RotateAllSlide(int iType = 1)
        {
            if (mMainMultiScaleImage.Source != null)
            {
                int elemenRelativeAngle = PublicMethods.GetElemenRelativeAngle(mMainMultiScaleImage);
                int num = iAngle - elemenRelativeAngle;
                foreach (FrameworkElement child in gAllSlideBox.Children)
                {
                    if (child is Border)
                    {
                        Grid grid = (child as Border).Child as Grid;
                        MultiScaleImage multiScaleImage = grid.FindName("mMultiScaleImage") as MultiScaleImage;
                        if ((iType != 2 || grid == gMainMultiScaleImageBox) && (iType != 1 || grid == gMainMultiScaleImageBox || cbJoinSlide.IsChecked != false) && multiScaleImage.Source != null)
                        {
                            int iNumber = PublicMethods.GetElemenRelativeAngle(multiScaleImage) + num;
                            iNumber = PublicMethods.RemainderAngle(iNumber, 360);
                            int iNumber2 = PublicMethods.RelativeAngleToActualAngle(grid, iNumber);
                            iNumber2 = PublicMethods.RemainderAngle(iNumber2, 360);
                            RotateSingleSlideAndUpdateTools(grid, iNumber2);
                        }
                    }
                }
            }
        }

        private bool CheckSlideState()
        {
            if (bAddMarkState || bEditMarkState)
            {
                System.Windows.MessageBox.Show(GlobalVariable.listCurrentLanguage[92] + "!");
                return false;
            }
            return true;
        }

        private void ResetZoomTool()
        {
            if (ZoomTool.ResetZoomTool(mMainMultiScaleImage, sZoomTool, cChoicesBox, lBGLine, lValueLine, iDefaultAngle, fScreenRatio, rdDirectory))
            {
                foreach (FrameworkElement child in cChoicesBox.Children)
                {
                    if (child.Style == rdDirectory["ZoomToolChoices"])
                    {
                        child.MouseLeftButtonDown += lChoice_MouseLeftButtonDown;
                    }
                }
            }
        }

        private void ChangeZoom(MultiScaleImage mSlide, float fTargetZoom, System.Windows.Point pCenter, bool animate = true)
        {
            float fMainSlideTargetScale = ZoomTool.ZoomToScale(mSlide, fTargetZoom, fScreenRatio);
            UpdateSlideAndTools(fMainSlideTargetScale, new System.Windows.Point(0.0, 0.0), pCenter, animate);
        }

        private void lChoice_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (CheckSlideState())
                {
                    System.Windows.Controls.Label label = sender as System.Windows.Controls.Label;
                    ChangeZoom(mMainMultiScaleImage, float.Parse(label.Content.ToString()), new System.Windows.Point(mMainMultiScaleImage.ActualWidth / 2.0, mMainMultiScaleImage.ActualHeight / 2.0));
                }
            }
            catch
            {
            }
        }

        private void LOne_mouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                ChangeZoom(mMainMultiScaleImage, (float)mMainMultiScaleImage.Source.sbcCurrentSlide.dMaxActualZoom, new System.Windows.Point(mMainMultiScaleImage.ActualWidth / 2.0, mMainMultiScaleImage.ActualHeight / 2.0));
            }
        }

        private void Add_mouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                float num = float.Parse(((float)((double)mMainMultiScaleImage.fTargetScale * mMainMultiScaleImage.Source.sbcCurrentSlide.dMaxActualZoom) * fScreenRatio).ToString("0.00"));
                try
                {
                    int num2 = 0;
                    while (true)
                    {
                        if (num2 >= ZoomTool.listChoice.Count)
                        {
                            return;
                        }
                        if ((float)ZoomTool.listChoice[num2] > num)
                        {
                            break;
                        }
                        num2++;
                    }
                    num = ZoomTool.listChoice[num2];
                    ChangeZoom(mMainMultiScaleImage, num, new System.Windows.Point(mMainMultiScaleImage.ActualWidth / 2.0, mMainMultiScaleImage.ActualHeight / 2.0));
                }
                catch
                {
                }
            }
        }

        private void Less_mouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                float num = float.Parse(((float)((double)mMainMultiScaleImage.fTargetScale * mMainMultiScaleImage.Source.sbcCurrentSlide.dMaxActualZoom) * fScreenRatio).ToString("0.00"));
                try
                {
                    for (int num2 = ZoomTool.listChoice.Count - 1; num2 >= 0; num2--)
                    {
                        if ((float)ZoomTool.listChoice[num2] < num)
                        {
                            num = ZoomTool.listChoice[num2];
                            ChangeZoom(mMainMultiScaleImage, num, new System.Windows.Point(mMainMultiScaleImage.ActualWidth / 2.0, mMainMultiScaleImage.ActualHeight / 2.0));
                            return;
                        }
                    }
                    ChangeZoom(mMainMultiScaleImage, ZoomTool.fMinSetZoom, new System.Windows.Point(mMainMultiScaleImage.ActualWidth / 2.0, mMainMultiScaleImage.ActualHeight / 2.0));
                }
                catch
                {
                }
            }
        }

        private void rController_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                bZoomToolPress = true;
                fStartY = (float)e.GetPosition(this).Y;
                fStartTop = (float)rController.Margin.Top;
                fMoveY = fStartY;
            }
        }

        private void rController_Move(object sender, System.Windows.Input.MouseEventArgs e)
        {
            float num = (float)e.GetPosition(this).Y;
            float num2 = num - fMoveY;
            if (num2 == 0f)
            {
                return;
            }
            float num3 = ZoomTool.fTotalHeight - ZoomTool.GetTopValue(1f) - 10f;
            float num4 = fStartTop + num - fStartY;
            if (num4 >= ZoomTool.fTotalHeight - 9f)
            {
                return;
            }
            float num5;
            if (num4 <= num3)
            {
                num5 = (float)Math.Pow(2.0, (0f - num2) / 30f);
            }
            else
            {
                float num6 = 1f - (num4 - num3) / 30f;
                float num7 = 1f - (fMoveY - fStartY + fStartTop - num3) / 30f;
                num5 = num6 / num7;
                if (num5 <= 0f)
                {
                    return;
                }
            }
            fMoveY = num;
            UpdateSlideAndTools(pScaleCenter: new System.Windows.Point(mMainMultiScaleImage.ActualWidth / 2.0, mMainMultiScaleImage.ActualHeight / 2.0), fMainSlideTargetScale: mMainMultiScaleImage.GetCurrentScale() * num5, pMainSlideDeltaOffset: new System.Windows.Point(0.0, 0.0));
        }

        private void lDefault_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                float fScale = (float)mMainMultiScaleImage.OriginalScale;
                float fTargetZoom = ZoomTool.ScaleToZoom(mMainMultiScaleImage, fScale, fScreenRatio);
                ChangeZoom(mMainMultiScaleImage, fTargetZoom, new System.Windows.Point(mMainMultiScaleImage.ActualWidth / 2.0, mMainMultiScaleImage.ActualHeight / 2.0), false);
                System.Windows.Point pFlipDelta = mMainMultiScaleImage.RatioOffsetToDeltaOffset(new System.Windows.Point(0.5, 0.5));
                pFlipDelta = PublicMethods.FlipDelta(gMainMultiScaleImageBox, pFlipDelta);
                UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pFlipDelta, new System.Windows.Point(0.0, 0.0), true);
            }
        }

        private void lLabelRotate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int elementActualAngle = PublicMethods.GetElementActualAngle(imgLabel);
            elementActualAngle += 90;
            elementActualAngle %= 360;
            imgLabel.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
            imgLabel.RenderTransform = new RotateTransform(elementActualAngle);
            if (elementActualAngle / 90 % 2 == 1)
            {
                imgLabel.Margin = new Thickness(22.0, 22.0, 0.0, 0.0);
                bLabelToggle.Margin = new Thickness(33.0, 65.0, 0.0, 0.0);
            }
            else
            {
                imgLabel.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
                bLabelToggle.Margin = new Thickness(-12.0, 42.0, 0.0, 0.0);
            }
        }

        private void bLabelToggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int elementActualAngle = PublicMethods.GetElementActualAngle(imgLabel);
            if (cLabelTool.Width != 0.0 && elementActualAngle / 90 % 2 != 1)
            {
                cLabelTool.Width = 0.0;
                bLabelToggle.Margin = new Thickness(-12.0, 42.0, 0.0, 0.0);
                lLabelToggle.Content = "<";
                lLabelToggle.ToolTip = GlobalVariable.listCurrentLanguage[93];
            }
            else if (cLabelTool.Width == 0.0 && elementActualAngle / 90 % 2 != 1)
            {
                cLabelTool.Width = 180.0;
                bLabelToggle.Margin = new Thickness(-12.0, 42.0, 0.0, 0.0);
                lLabelToggle.Content = ">";
                lLabelToggle.ToolTip = GlobalVariable.listCurrentLanguage[88];
            }
            else if (cLabelTool.Width != 0.0 && elementActualAngle / 90 % 2 == 1)
            {
                cLabelTool.Width = 0.0;
                bLabelToggle.Margin = new Thickness(-12.0, 65.0, 0.0, 0.0);
                lLabelToggle.Content = "<";
                lLabelToggle.ToolTip = GlobalVariable.listCurrentLanguage[93];
            }
            else
            {
                cLabelTool.Width = 180.0;
                bLabelToggle.Margin = new Thickness(33.0, 65.0, 0.0, 0.0);
                lLabelToggle.Content = ">";
                lLabelToggle.ToolTip = GlobalVariable.listCurrentLanguage[88];
            }
        }

        private void tbTitle_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox.Text == GlobalVariable.listCurrentLanguage[47])
            {
                textBox.Text = "";
            }
        }

        private void tbTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox.Text == "")
            {
                textBox.Text = GlobalVariable.listCurrentLanguage[47];
            }
        }

        private void tbContent_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox.Text == GlobalVariable.listCurrentLanguage[49])
            {
                textBox.Text = "";
            }
        }

        private void tbContent_LostFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
            if (textBox.Text == "")
            {
                textBox.Text = GlobalVariable.listCurrentLanguage[49];
            }
        }

        private void lShape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (bAddMarkState)
            {
                try
                {
                    CloseEditMarkPannel();
                    gMainMarkBox.Children.Remove(mCurrentMark.cMarkBox);
                }
                catch
                {
                }
            }
            if (bEditMarkState)
            {
                CheckSlideState();
                return;
            }
            System.Windows.Controls.Label label = sender as System.Windows.Controls.Label;
            if (label.BorderBrush.ToString() != "#FF000000")
            {
                foreach (System.Windows.Controls.Label child in cAllShape.Children)
                {
                    child.BorderBrush = System.Windows.Media.Brushes.White;
                }
                label.BorderBrush = System.Windows.Media.Brushes.Black;
                Mark.IntoAddMarkState(gAllSlideBox);
                switch (label.DataContext.ToString())
                {
                    case "Rectangle":
                        mCurrentMark = new RectangleMark();
                        break;
                    case "Ellipse":
                        mCurrentMark = new EllipseMark();
                        break;
                    case "Line":
                        mCurrentMark = new LineMark();
                        break;
                    case "Arrow":
                        mCurrentMark = new ArrowMark();
                        break;
                    case "Polygon":
                        mCurrentMark = new PolygonMark();
                        break;
                }
                bAddMarkState = true;
                mCurrentMark.bLineColor = bLineColor;
                mCurrentMark.fLineWidth = fLineWidth / fScreenRatio;
                mCurrentMark.fWidthPerPixel = fWidthPerPixel;
            }
            else
            {
                CloseMarkTool(2);
            }
        }

        private void lColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (System.Windows.Controls.Label child in sChooseColor.Children)
            {
                child.BorderBrush = System.Windows.Media.Brushes.White;
            }
            lMoreColor.BorderBrush = System.Windows.Media.Brushes.White;
            System.Windows.Controls.Label label = sender as System.Windows.Controls.Label;
            label.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 204));
            bLineColor = label.Background;
            if (mCurrentMark != null && bAddMarkState)
            {
                mCurrentMark.bLineColor = bLineColor;
            }
        }

        private void cSideWidth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ComboBox comboBox = sender as System.Windows.Controls.ComboBox;
            fLineWidth = Convert.ToSingle(comboBox.SelectedValue.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", ""));
            if (mCurrentMark != null && bAddMarkState)
            {
                mCurrentMark.fLineWidth = fLineWidth / fScreenRatio;
            }
        }

        private void lEditMark_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (CheckSlideState())
                {
                    e.Handled = true;
                    bMirrorToolBorder.Visibility = Visibility.Collapsed;
                    ToolBarBtn_MouseLeave(bMirror, new RoutedEventArgs());
                    bEditMarkState = true;
                    Canvas canvas = (sender as System.Windows.Controls.Label).DataContext as Canvas;
                    MarkInfo markInfo = canvas.DataContext as MarkInfo;
                    if (markInfo != null)
                    {
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        mCurrentMark = Mark.DeserializeMarkInfo(markInfo, serializer, fScreenRatio);
                        PositionMark(mCurrentMark);
                        mMainMultiScaleImage.bMouseDragAvailable = false;
                        mMainMultiScaleImage.bMouseWheelAvailable = false;
                        mCurrentMark.cMarkBox = canvas;
                        mCurrentMark.fCurrentScreenRatio = fScreenRatio;
                        int elemenRelativeAngle = PublicMethods.GetElemenRelativeAngle(mMainMultiScaleImage);
                        StackPanel stackPanel = mCurrentMark.IntoEditState(this, elemenRelativeAngle);
                        stackPanel.Children[0].MouseLeftButtonDown += delegate (object oSender, MouseButtonEventArgs mE)
                        {
                            mE.Handled = true;
                            bEditMarkState = true;
                            ShowEditMarkPannel();
                        };
                        stackPanel.Children[1].MouseLeftButtonDown += delegate (object oSender, MouseButtonEventArgs mE)
                        {
                            mE.Handled = true;
                            bEditMarkState = false;
                            mCurrentMark.CancelTempEdit();
                            CloseMarkTool(2);
                        };
                    }
                }
            }
            catch
            {
            }
        }

        private void lEditMarkPannelColorBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            System.Windows.Controls.Label label = sender as System.Windows.Controls.Label;
            if (mCurrentMark != null)
            {
                foreach (System.Windows.Controls.Label child in sEditColor.Children)
                {
                    child.BorderBrush = System.Windows.Media.Brushes.White;
                }
                lMoreEditColor.BorderBrush = System.Windows.Media.Brushes.White;
                label.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 204));
                mCurrentMark.bLineColor = label.Background;
            }
        }

        private void cEditSideWidth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            float num = Convert.ToSingle((sender as System.Windows.Controls.ComboBox).SelectedValue.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", ""));
            if (mCurrentMark != null)
            {
                mCurrentMark.fLineWidth = num / fScreenRatio;
            }
        }

        private void bCancelMark_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (bAddMarkState)
            {
                gMainMarkBox.Children.Remove(mCurrentMark.cMarkBox);
                CloseMarkTool(2);
            }
            else if (bEditMarkState)
            {
                bEditMarkState = false;
            }
            CloseEditMarkPannel();
        }

        private void bSaveMark_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            mCurrentMark.strMarkName = ((tbTitle.Text == GlobalVariable.listCurrentLanguage[47]) ? "" : tbTitle.Text);
            mCurrentMark.strMarkDescription = ((tbContent.Text == GlobalVariable.listCurrentLanguage[49]) ? "" : tbContent.Text);
            mCurrentMark.bStartSaveMark = true;
            if (bAddMarkState)
            {
                CloseEditMarkPannel();
                StackPanel stackPanel = mCurrentMark.SaveMarkInfo(mMainMultiScaleImage, fScreenRatio);
                if (stackPanel != null)
                {
                    AddEventForMark(mCurrentMark, stackPanel);
                }
                else
                {
                    gMainMarkBox.Children.Remove(mCurrentMark.cMarkBox);
                }
            }
            else if (bEditMarkState)
            {
                int index = gMainMarkBox.Children.IndexOf(mCurrentMark.cMarkBox);
                mCurrentMark.SaveTempEdit();
                CloseEditMarkPannel();
                mCurrentMark.RemoveEditTool();
                mCurrentMark.EditMarkInfo(mMainMultiScaleImage, fScreenRatio);
                Border obj = sMarkList.Children[index] as Border;
                TextBlock textBlock = obj.FindName("tbTitle") as TextBlock;
                TextBlock obj2 = obj.FindName("tbDescription") as TextBlock;
                textBlock.Text = GlobalVariable.listCurrentLanguage[46] + "：" + mCurrentMark.strMarkName;
                obj2.Text = GlobalVariable.listCurrentLanguage[48] + "：" + mCurrentMark.strMarkDescription;
            }
            CloseMarkTool(2);
            Mark.UpdateAllMarkSwitch(gMainMarkBox, lAllMarkShow, lAllMarkHide, rdDirectory);
            Mark.UpdateAllMarkInfoSwitch(gMainMarkBox, lAllInfoShow, lAllInfoHide, rdDirectory);
            MagnifierTool.GetMagnifierMark(mMainMultiScaleImage, gMagnifierFlipBox, fScreenRatio);
            Focus();
        }

        private void DeleteMark(string strId, Canvas cMarkBox, Border bMarkListItem)
        {
            Mark.DelateMark(strId, cMarkBox, bMarkListItem, mMainMultiScaleImage, gMainMarkBox, sMarkList);
            MagnifierTool.GetMagnifierMark(mMainMultiScaleImage, gMagnifierFlipBox, fScreenRatio);
        }

        private void PositionMark(Mark mMark)
        {
            ChangeZoom(mMainMultiScaleImage, mMark.fSlideZoomWhenAdd, new System.Windows.Point(mMainMultiScaleImage.ActualWidth / 2.0, mMainMultiScaleImage.ActualHeight / 2.0), false);
            System.Windows.Point pSlideFlipValueWhenAdd = mMark.pSlideFlipValueWhenAdd;
            if (pSlideFlipValueWhenAdd.X == -1.0)
            {
                cbFlipHorizontal.IsChecked = true;
            }
            else
            {
                cbFlipHorizontal.IsChecked = false;
            }
            if (pSlideFlipValueWhenAdd.Y == -1.0)
            {
                cbFlipVertical.IsChecked = true;
            }
            else
            {
                cbFlipVertical.IsChecked = false;
            }
            iAngle = PublicMethods.FlipAngle(pSlideFlipValueWhenAdd, mMark.iSlideAngleWhenAdd);
            RotateAllSlide();
            mMainMultiScaleImage.UpdateLayout();
            System.Windows.Point pFlipDelta = mMainMultiScaleImage.RatioOffsetToDeltaOffset(mMark.pSlideRatioCenter);
            pFlipDelta = PublicMethods.FlipDelta(gMainMultiScaleImageBox, pFlipDelta);
            UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pFlipDelta, new System.Windows.Point(0.0, 0.0));
        }

        private void ShowEditMarkPannel()
        {
            gEditMarkPannelBG.Visibility = Visibility.Visible;
            lMoreEditColor.BorderBrush = System.Windows.Media.Brushes.White;
            if (bAddMarkState)
            {
                bool flag = false;
                foreach (System.Windows.Controls.Label child in sChooseColor.Children)
                {
                    if (child.BorderBrush.ToString() == "#FFCCCCCC")
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    foreach (System.Windows.Controls.Label child2 in sEditColor.Children)
                    {
                        if (mCurrentMark.bLineColor.ToString() == child2.Background.ToString())
                        {
                            child2.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 204));
                        }
                        else
                        {
                            child2.BorderBrush = System.Windows.Media.Brushes.White;
                        }
                    }
                }
                else
                {
                    foreach (System.Windows.Controls.Label child3 in sEditColor.Children)
                    {
                        child3.BorderBrush = System.Windows.Media.Brushes.White;
                    }
                    rChooseEditColor.Fill = mCurrentMark.bLineColor;
                    lMoreEditColor.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 204));
                }
            }
            else
            {
                tbTitle.Text = ((mCurrentMark.strMarkName.Trim() == "") ? GlobalVariable.listCurrentLanguage[47] : mCurrentMark.strMarkName);
                tbContent.Text = ((mCurrentMark.strMarkDescription.Trim() == "") ? GlobalVariable.listCurrentLanguage[49] : mCurrentMark.strMarkDescription);
                tbTitle.Focus();
                tbTitle.SelectAll();
                bool flag2 = false;
                foreach (System.Windows.Controls.Label child4 in sEditColor.Children)
                {
                    if (mCurrentMark.bLineColor.ToString() == child4.Background.ToString())
                    {
                        child4.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 204));
                        flag2 = true;
                        break;
                    }
                    child4.BorderBrush = System.Windows.Media.Brushes.White;
                }
                if (!flag2)
                {
                    rChooseEditColor.Fill = mCurrentMark.bLineColor;
                    lMoreEditColor.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 204));
                }
            }
            string text = ((int)(mCurrentMark.fLineWidth * fScreenRatio)).ToString();
            switch (text)
            {
                case "1":
                    cEditSideWidth.SelectedIndex = 0;
                    return;
                case "2":
                    cEditSideWidth.SelectedIndex = 1;
                    return;
                case "5":
                    cEditSideWidth.SelectedIndex = 2;
                    return;
            }
            if (text == "10")
            {
                cEditSideWidth.SelectedIndex = 3;
            }
        }

        public void CloseEditMarkPannel()
        {
            cEditSideWidth.Focus();
            MovePannel.RestorePosition(bEditMarkPannel, this);
            gEditMarkPannelBG.Visibility = Visibility.Collapsed;
            tbTitle.Text = GlobalVariable.listCurrentLanguage[47];
            tbContent.Text = GlobalVariable.listCurrentLanguage[49];
        }

        private void lAllMarkShow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                lAllMarkShow.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtnActive"]);
                lAllMarkHide.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtn"]);
                for (int i = 0; i < gMainMarkBox.Children.Count; i++)
                {
                    (gMainMarkBox.Children[i] as Canvas).Visibility = Visibility.Visible;
                }
                for (int j = 0; j < sMarkList.Children.Count; j++)
                {
                    ((sMarkList.Children[j] as Border).FindName("lToggleMark") as System.Windows.Controls.Label).Content = GlobalVariable.listCurrentLanguage[94];
                }
                MagnifierTool.UpdateMagnifierMark(mMainMultiScaleImage, gMagnifierFlipBox);
            }
        }

        private void lAllMarkHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                lAllMarkHide.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtnActive"]);
                lAllMarkShow.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtn"]);
                for (int i = 0; i < gMainMarkBox.Children.Count; i++)
                {
                    (gMainMarkBox.Children[i] as Canvas).Visibility = Visibility.Collapsed;
                }
                for (int j = 0; j < sMarkList.Children.Count; j++)
                {
                    ((sMarkList.Children[j] as Border).FindName("lToggleMark") as System.Windows.Controls.Label).Content = GlobalVariable.listCurrentLanguage[106];
                }
                MagnifierTool.UpdateMagnifierMark(mMainMultiScaleImage, gMagnifierFlipBox);
            }
        }

        private void lAllInfoShow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                lAllInfoShow.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtnActive"]);
                lAllInfoHide.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtn"]);
                for (int i = 0; i < gMainMarkBox.Children.Count; i++)
                {
                    ((gMainMarkBox.Children[i] as Canvas).Children[0] as Border).Visibility = Visibility.Visible;
                }
                for (int j = 0; j < sMarkList.Children.Count; j++)
                {
                    ((sMarkList.Children[j] as Border).FindName("lToggleMarkInfo") as System.Windows.Controls.Label).Content = GlobalVariable.listCurrentLanguage[95];
                }
            }
        }

        private void lAllInfoHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CheckSlideState())
            {
                lAllInfoShow.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtn"]);
                lAllInfoHide.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtnActive"]);
                for (int i = 0; i < gMainMarkBox.Children.Count; i++)
                {
                    ((gMainMarkBox.Children[i] as Canvas).Children[0] as Border).Visibility = Visibility.Collapsed;
                }
                for (int j = 0; j < sMarkList.Children.Count; j++)
                {
                    ((sMarkList.Children[j] as Border).FindName("lToggleMarkInfo") as System.Windows.Controls.Label).Content = GlobalVariable.listCurrentLanguage[100];
                }
            }
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadLanguages();
            ChangeLanguage();
            try
            {
                if (Environment.GetCommandLineArgs().Length > 1)
                {
                    string text = Environment.GetCommandLineArgs()[1];
                    for (int i = 2; i < Environment.GetCommandLineArgs().Length; i++)
                    {
                        text = text + " " + Environment.GetCommandLineArgs()[i];
                    }
                    if (System.IO.Path.GetExtension(text) == ".zyp" && File.Exists(text))
                    {
                        OpenSlide(text);
                        string directoryName = System.IO.Path.GetDirectoryName(text);
                        CreateSlideList(directoryName);
                        cbAutoHide.IsChecked = true;
                        bChooseSlideToolBorder.Visibility = Visibility.Collapsed;
                        ToolBarBtn_MouseLeave(bSlideList, new RoutedEventArgs());
                    }
                }
                else
                {
                    string key = IniSetting.GetKey(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/Configuration.ini", "Custom", "OldFolderPath");
                    if (key != null && key.Trim() != "" && new DirectoryInfo(key).Exists)
                    {
                        CreateSlideList(key);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
        }

        private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                sSlideListScroll.MaxHeight = (base.Content as FrameworkElement).ActualHeight - 70.0;
                sMarkListScroll.MaxHeight = (base.Content as FrameworkElement).ActualHeight - 137.0;
            }
            catch
            {
            }
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {
            if (strCurrentFolderPath != null)
            {
                IniSetting.SaveKey(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/Configuration.ini", "Custom", "OldFolderPath", strCurrentFolderPath);
            }
        }

        private void mainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            bMagnifierChangeSizeToolPress = false;
            bNavigationChangeSizeToolPress = false;
            bZoomToolPress = false;
            bMarkPress = false;
            bSlidePress = false;
            bSlidePressAndLoseFocus = false;
        }

        private void mainWindow_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            bMagnifierChangeSizeToolPress = false;
            bNavigationChangeSizeToolPress = false;
            bZoomToolPress = false;
            bMarkPress = false;
            bSlidePressAndLoseFocus = false;
            if (bSlidePress)
            {
                mMainMultiScaleImage.bMouseDown = true;
                mMainMultiScaleImage.bMouseMove = true;
                mMainMultiScaleImage.ParentOnMouseLeave(e);
            }
            bSlidePress = false;
        }

        private void mainWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                lMoveUp_Click(lMoveUp, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                lMoveLeft_Click(lMoveLeft, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                lMoveDown_Click(lMoveDown, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                lMoveRight_Click(lMoveRight, new RoutedEventArgs());
                e.Handled = true;
            }
        }

        private void mainWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (bZoomToolPress)
            {
                rController_Move(sender, e);
            }
            if (bNavigationChangeSizeToolPress)
            {
                System.Windows.Point position = e.GetPosition(this);
                float num = (float)((pNavigatorBoxSize.X - 4.0) / (pNavigatorBoxSize.Y - 4.0));
                float num2 = (float)(pNavigatorBoxSize.X + position.X - pMouseStartPosition.X);
                float num3 = (num2 - 4f) / num + 4f;
                if ((double)num2 > this.ActualWidth / 2.0)
                {
                    num2 = (float)this.ActualWidth / 2f;
                    num3 = num2 / num;
                    return;
                }
                if ((double)num3 > this.ActualHeight - 60.0)
                {
                    num3 = (float)(this.ActualHeight - 60.0);
                    num2 = num3 * num;
                    return;
                }
                num2 -= 4f;
                num3 -= 4f;
                int acuteAngle = PublicMethods.GetAcuteAngle(iAngle);
                float num7;
                float num8;
                switch (acuteAngle)
                {
                    case 0:
                        num7 = num2;
                        num8 = num3;
                        break;
                    case 45:
                        {
                            float num9 = fNavigatorWidth / fNavigatorHeight;
                            num7 = (float)((double)num2 * Math.Sqrt(2.0) / (double)(1f / num9 + 1f));
                            num8 = num7 / num9;
                            break;
                        }
                    default:
                        {
                            float num4 = (float)Math.Sin(Math.PI / 180.0 * (double)acuteAngle);
                            float num5 = (float)Math.Cos(Math.PI / 180.0 * (double)acuteAngle);
                            float num6 = fNavigatorWidth / fNavigatorHeight;
                            num7 = ((num2 + num3) / (num4 + num5) - (num2 - num3) / (num4 - num5)) / 2f;
                            num8 = num7 / num6;
                            break;
                        }
                }
                if (num7 <= 0f || num8 <= 0f)
                {
                    return;
                }
                float num10 = mNavigatorImage.Source.GetLevel(num7 - 5f, num8 - 5f);
                if ((float)mNavigatorImage.Source.sbcCurrentSlide.iMaxLevel - num10 > 9f)
                {
                    return;
                }
                bNavigation.Width = num2 + 4f;
                bNavigation.Height = num3 + 4f;
                gNavigatorImageBox.Width = num7;
                gNavigatorImageBox.Height = num8;
                fNavigatorWidth = num7;
                fNavigatorHeight = num8;
                FlipTool.ChangeNavigationFlipCenter(gNavigatorImageBox);
            }
            if (!bMagnifierChangeSizeToolPress)
            {
                return;
            }
            System.Windows.Point position2 = e.GetPosition(this);
            System.Windows.Point point = (System.Windows.Point)iMagnifierChangeSizeTool.DataContext;
            float num11 = (float)(bMagnifierBorder.Width + position2.X - point.X);
            float num12 = (float)(bMagnifierBorder.Height + position2.Y - point.Y);
            ScaleTransform scaleTransform = null;
            if (gMagnifierFlipBox.RenderTransform is ScaleTransform)
            {
                scaleTransform = (gMagnifierFlipBox.RenderTransform as ScaleTransform);
            }
            if (num11 >= 60f)
            {
                if (scaleTransform != null)
                {
                    scaleTransform.CenterX = (num11 - 6f) / 2f;
                }
                bMagnifierBorder.Width = bMagnifierBorder.Width + position2.X - point.X;
            }
            if (num12 >= 60f)
            {
                if (scaleTransform != null)
                {
                    scaleTransform.CenterY = (num12 - 6f) / 2f;
                }
                bMagnifierBorder.Height = bMagnifierBorder.Height + position2.Y - point.Y;
            }
            num11 = (float)bMagnifierBorder.Width;
            num12 = (float)bMagnifierBorder.Height;
            int num13 = PublicMethods.GetElementActualAngle(gMagnifierFlipBox);
            if (num13 < 0)
            {
                num13 += 360;
            }
            new RotateTransform(num13);
            int acuteAngle2 = PublicMethods.GetAcuteAngle(PublicMethods.GetElemenRelativeAngle(mMagnifier));
            float num14 = (float)Math.Sin(Math.PI / 180.0 * (double)acuteAngle2);
            float num15 = (float)Math.Cos(Math.PI / 180.0 * (double)acuteAngle2);
            float num16 = num11;
            float num17 = num12;
            float num18 = num11 * num15 + num12 * num14;
            float num19 = num11 * num14 + num12 * num15;
            float num20 = (0f - (num18 - num16)) / 2f;
            float num21 = (0f - (num19 - num17)) / 2f;
            mMagnifier.Margin = new Thickness(num20, num21, num20, num21);
            iMagnifierChangeSizeTool.DataContext = position2;
            if (lFixed.Background.ToString() == "#FFFFFFFF")
            {
                MagnifierTool.MagnifierZoomAboutCenter(bMagnifierBorder, this, gAllSlideBox, fScreenRatio);
            }
        }

        private void bMultiScaleImageBoxBorder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            SetCrrentSlideBox(border.Child as Grid);
        }

        private void bMultiScaleImageBoxBorder_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Border border = sender as Border;
            SetCrrentSlideBox(border.Child as Grid);
        }

        private void gMultiScaleImageBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Grid grid = sender as Grid;
            MultiScaleImage multiScaleImage = grid.FindName("mMultiScaleImage") as MultiScaleImage;
            if (multiScaleImage.Source != null)
            {
                Grid grid2 = grid.FindName("gFlipBox") as Grid;
                if (grid2.RenderTransform is ScaleTransform)
                {
                    ScaleTransform obj = grid2.RenderTransform as ScaleTransform;
                    obj.CenterX = grid.ActualWidth / 2.0;
                    obj.CenterY = grid.ActualHeight / 2.0;
                }
                int acuteAngle = PublicMethods.GetAcuteAngle(PublicMethods.GetElementActualAngle(multiScaleImage));
                float num = (float)Math.Sin(Math.PI / 180.0 * (double)acuteAngle);
                float num2 = (float)Math.Cos(Math.PI / 180.0 * (double)acuteAngle);
                float num3 = (float)e.NewSize.Width;
                float num4 = (float)e.NewSize.Height;
                float num5 = (float)e.NewSize.Width * num2 + (float)e.NewSize.Height * num;
                float num6 = (float)e.NewSize.Width * num + (float)e.NewSize.Height * num2;
                float num7 = (0f - (num5 - num3)) / 2f;
                float num8 = (0f - (num6 - num4)) / 2f;
                multiScaleImage.Margin = new Thickness(num7, num8, num7, num8);
                System.Windows.Point point = default(System.Windows.Point);
                point.X = (0.0 - (e.NewSize.Width - e.PreviousSize.Width)) / 2.0;
                point.Y = (0.0 - (e.NewSize.Height - e.PreviousSize.Height)) / 2.0;
                System.Windows.Point pDeltaOffset = default(System.Windows.Point);
                pDeltaOffset.X = point.X * (double)num2 + point.Y * (double)num;
                pDeltaOffset.Y = point.Y * (double)num2 + point.X * (double)num;
                multiScaleImage.DeltaToPan(pDeltaOffset, false, 5);
                multiScaleImage.UpdateLayout();
                ResetZoomTool();
                float num9 = (float)grid.ActualWidth;
                float num10 = (float)grid.ActualHeight;
                if (iDefaultAngle / 90 % 2 == 1)
                {
                    num9 = (float)grid.ActualHeight;
                    num10 = (float)grid.ActualWidth;
                }
                float num11 = (float)Math.Min((double)num9 / multiScaleImage.Source.ImageSize.Width, (double)num10 / multiScaleImage.Source.ImageSize.Height);
                float num12 = (float)multiScaleImage.minScaleRelativeToMinSize * num11;
                bool bScaleChanged = false;
                float currentScale = multiScaleImage.GetCurrentScale();
                if (Math.Round(currentScale, 6) < Math.Round(num12, 6))
                {
                    multiScaleImage.ScaleCanvas(num12 / currentScale, new System.Windows.Point(multiScaleImage.ActualWidth / 2.0, multiScaleImage.ActualHeight / 2.0));
                    bScaleChanged = true;
                }
                UpdateTools(multiScaleImage, bScaleChanged);
            }
        }

        private void mMultiScaleImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!bAddMarkState && !bEditMarkState)
            {
                bSlidePress = true;
                if (e.ClickCount == 2)
                {
                    bSlidePress = false;
                }
            }
        }

        private void gMultiScaleImageBox_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (bSlidePress)
            {
                pSlideLoseFocusPoint = e.GetPosition(gMainMultiScaleImageBox);
                bSlidePressAndLoseFocus = true;
            }
        }

        private void gMultiScaleImageBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mMainMultiScaleImage.Source == null)
            {
                return;
            }
            if (bStartMeasure)
            {
                pMouseDownPointUseForMeasuring = e.GetPosition(gMainMultiScaleImageBox);
            }
            else if (bAddMarkState)
            {
                pMouseDownPointUseForMarking = e.GetPosition(gMainMultiScaleImageBox);
                mCurrentMark.MouseDownForDrawShape(pMouseDownPointUseForMarking);
                if (mCurrentMark is LineMark || mCurrentMark is ArrowMark)
                {
                    iMeasurePointCount++;
                }
            }
            else if (e.ClickCount == 2 && !bDoubleClickTimeOut)
            {
                if (!CheckSlideState())
                {
                    return;
                }
                System.Windows.Point position = e.GetPosition(mMainMultiScaleImage);
                float num = (float)((double)mMainMultiScaleImage.fTargetScale * mMainMultiScaleImage.Source.sbcCurrentSlide.dMaxActualZoom) * fScreenRatio;
                mMainMultiScaleImage.SetParentDoubleClickPriority();
                ChangeOtherSlidePriority(mMainMultiScaleImage, 5);
                if (Math.Round((float)mMainMultiScaleImage.Source.sbcCurrentSlide.dMaxActualZoom, 2) != Math.Round(num, 2))
                {
                    ChangeZoom(mMainMultiScaleImage, (float)mMainMultiScaleImage.Source.sbcCurrentSlide.dMaxActualZoom, position);
                }
                else
                {
                    float fScale = (float)mMainMultiScaleImage.OriginalScale;
                    float fTargetZoom = ZoomTool.ScaleToZoom(mMainMultiScaleImage, fScale, fScreenRatio);
                    ChangeZoom(mMainMultiScaleImage, fTargetZoom, new System.Windows.Point(mMainMultiScaleImage.ActualWidth / 2.0, mMainMultiScaleImage.ActualHeight / 2.0), false);
                    System.Windows.Point pFlipDelta = mMainMultiScaleImage.RatioOffsetToDeltaOffset(new System.Windows.Point(0.5, 0.5));
                    pFlipDelta = PublicMethods.FlipDelta(gMainMultiScaleImageBox, pFlipDelta);
                    UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pFlipDelta, new System.Windows.Point(0.0, 0.0));
                }
            }
            bDoubleClickTimeOut = false;
        }

        private void gMultiScaleImageBox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mMainMultiScaleImage.Source == null)
            {
                return;
            }
            if (bMarkPress && !bAddMarkState && !bEditMarkState)
            {
                System.Windows.Point position = e.GetPosition(this);
                System.Windows.Point pMainSlideDeltaOffset = default(System.Windows.Point);
                float num = (float)(position.X - pMouseDownPointUseForSlideMoving.X);
                float num2 = (float)(position.Y - pMouseDownPointUseForSlideMoving.Y);
                pMouseDownPointUseForSlideMoving = position;
                float num3 = (float)Math.Sin(Math.PI / 180.0 * (double)iAngle);
                float num4 = (float)Math.Cos(Math.PI / 180.0 * (double)iAngle);
                pMainSlideDeltaOffset.X = (0f - num) * num4 - num2 * num3;
                pMainSlideDeltaOffset.Y = (0f - num2) * num4 + num * num3;
                UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pMainSlideDeltaOffset, new System.Windows.Point(0.0, 0.0));
            }
            else if (bSlidePressAndLoseFocus && !bAddMarkState && !bEditMarkState)
            {
                System.Windows.Point position2 = e.GetPosition(gMainMultiScaleImageBox);
                System.Windows.Point pMainSlideDeltaOffset2 = default(System.Windows.Point);
                float num5 = (float)(position2.X - pSlideLoseFocusPoint.X);
                float num6 = (float)(position2.Y - pSlideLoseFocusPoint.Y);
                pSlideLoseFocusPoint = position2;
                float num7 = (float)Math.Sin(Math.PI / 180.0 * (double)iAngle);
                float num8 = (float)Math.Cos(Math.PI / 180.0 * (double)iAngle);
                pMainSlideDeltaOffset2.X = (0f - num5) * num8 - num6 * num7;
                pMainSlideDeltaOffset2.Y = (0f - num6) * num8 + num5 * num7;
                UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pMainSlideDeltaOffset2, new System.Windows.Point(0.0, 0.0));
            }
            else if (bStartMeasure && mtMeasureTool.iMeasurePointCount == 1)
            {
                System.Windows.Point position3 = e.GetPosition(gMainMultiScaleImageBox);
                mtMeasureTool.Measuring(position3, fWidthPerPixel);
                System.Windows.Point ratioPointValue = mMainMultiScaleImage.BoxPixelToSlideRatio(position3, 2);
                mtMeasureTool.SetRatioPointValue(ratioPointValue);
            }
            else if (bAddMarkState && mCurrentMark != null)
            {
                System.Windows.Point position4 = e.GetPosition(gMainMultiScaleImageBox);
                if (mCurrentMark is ArrowMark)
                {
                    System.Windows.Point pControlPixelPoint = mMainMultiScaleImage.BoxPixelToControlPixel(position4);
                    System.Windows.Point point = mMainMultiScaleImage.ControlPixelToSlidePixel(pControlPixelPoint);
                    float fRealLeft = (float)(point.X * (double)fWidthPerPixel);
                    float fRealTop = (float)(point.Y * (double)fWidthPerPixel);
                    mCurrentMark.MouseMoveForDrawShape(position4, fRealLeft, fRealTop);
                }
                else
                {
                    mCurrentMark.MouseMoveForDrawShape(position4);
                }
            }
        }

        private void gMultiScaleImageBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mMainMultiScaleImage.Source == null)
            {
                return;
            }
            if (bStartMeasure)
            {
                System.Windows.Point position = e.GetPosition(gMainMultiScaleImageBox);
                if (pMouseDownPointUseForMeasuring == position)
                {
                    System.Windows.Point ratioPointValue = mMainMultiScaleImage.BoxPixelToSlideRatio(position, 2);
                    mtMeasureTool.SetRatioPointValue(ratioPointValue);
                    mtMeasureTool.SetPointValue(position);
                }
            }
            else if (bAddMarkState)
            {
                if (mCurrentMark is LineMark || mCurrentMark is ArrowMark)
                {
                    if (e.GetPosition(gMainMultiScaleImageBox) == pMouseDownPointUseForMarking && iMeasurePointCount == 1)
                    {
                        return;
                    }
                    mCurrentMark.OverDrawShape();
                    ShowEditMarkPannel();
                }
                else if (!(mCurrentMark is PolygonMark) && mCurrentMark != null)
                {
                    mCurrentMark.OverDrawShape();
                    ShowEditMarkPannel();
                }
            }
            else if (bSlidePressAndLoseFocus)
            {
                mMainMultiScaleImage.bMouseDown = true;
                mMainMultiScaleImage.bMouseMove = true;
                mMainMultiScaleImage.MarkMouseUp(e);
                bSlidePress = false;
                bMarkPress = false;
            }
            if (bMarkPress)
            {
                mMainMultiScaleImage.MarkMouseUp(e);
                bSlidePress = false;
                bMarkPress = false;
            }
        }

        private void gMultiScaleImageBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mCurrentMark is PolygonMark)
            {
                mCurrentMark.OverDrawShape();
                ShowEditMarkPannel();
            }
        }

        private void AddEventToMoveSlide(Grid gSlideBox, MultiScaleImage mSlide, FrameworkElement feBox)
        {
            feBox.MouseLeftButtonDown += delegate (object oSender, MouseButtonEventArgs mE)
            {
                if (mE.ClickCount < 2 && !bAddMarkState && !bEditMarkState)
                {
                    mSlide.MarkMouseDown(mE);
                    bMarkPress = true;
                    pMouseDownPointUseForSlideMoving = mE.GetPosition(this);
                    mMainMultiScaleImage.Startstopwatch();
                }
            };
            feBox.MouseMove += delegate
            {
                if (bMarkPress)
                {
                    mMainMultiScaleImage.bMouseDown = true;
                    mMainMultiScaleImage.bMouseMove = true;
                }
            };
            feBox.MouseEnter += delegate (object oSender, System.Windows.Input.MouseEventArgs mE)
            {
                if (bSlidePress)
                {
                    pSlideLoseFocusPoint = mE.GetPosition(gSlideBox);
                    bSlidePressAndLoseFocus = true;
                }
            };
            feBox.MouseLeave += delegate (object oSender, System.Windows.Input.MouseEventArgs mE)
            {
                if (bSlidePress)
                {
                    pSlideLoseFocusPoint = mE.GetPosition(gSlideBox);
                    bSlidePressAndLoseFocus = true;
                }
            };
            feBox.MouseWheel += delegate (object oSender, MouseWheelEventArgs e)
            {
                mSlide.MarkMouseWheel(e);
                e.Handled = true;
            };
        }

        private void AddEventForSlideBox(Border bBoxBorder)
        {
            Grid grid = bBoxBorder.Child as Grid;
            MultiScaleImage multiScaleImage = grid.FindName("mMultiScaleImage") as MultiScaleImage;
            Border feBox = grid.FindName("bGridBox") as Border;
            Grid feBox2 = grid.FindName("gMarkBox") as Grid;
            bBoxBorder.PreviewMouseDown += bMultiScaleImageBoxBorder_PreviewMouseDown;
            bBoxBorder.PreviewMouseWheel += bMultiScaleImageBoxBorder_PreviewMouseWheel;
            grid.SizeChanged += gMultiScaleImageBox_SizeChanged;
            grid.MouseMove += gMultiScaleImageBox_MouseMove;
            grid.MouseUp += gMultiScaleImageBox_MouseUp;
            grid.MouseRightButtonDown += gMultiScaleImageBox_MouseRightButtonDown;
            grid.MouseLeftButtonDown += gMultiScaleImageBox_MouseLeftButtonDown;
            grid.MouseLeave += gMultiScaleImageBox_MouseLeave;
            multiScaleImage.MouseLeftButtonDown += mMultiScaleImage_MouseLeftButtonDown;
            multiScaleImage.ScaleOrOffsetChanged += DelegateUpdateSlideAndTools;
            multiScaleImage.PriorityChanged += ChangeOtherSlidePriority;
            AddEventToMoveSlide(grid, multiScaleImage, feBox);
            AddEventToMoveSlide(grid, multiScaleImage, feBox2);
        }

        private void AddEventForMark(Mark mMark, StackPanel sBtnBox)
        {
            AddEventForMarkList(mMark, mMark.strShapeName, mMark.cMarkBox);
            foreach (Shape sMarkShape in Mark.GetMarkEffectiveArea(mMark.cMarkBox, mMark.strShapeName))
            {
                bool bDoubleClick = false;
                sMarkShape.MouseLeftButtonDown += delegate (object oSender, MouseButtonEventArgs mE)
                {
                    mE.Handled = false;
                    bDoubleClick = false;
                    pMouseDownPointUseForShowInfo = mE.GetPosition(gMainMultiScaleImageBox);
                    if (mE.ClickCount == 2)
                    {
                        bDoubleClick = true;
                    }
                };
                sMarkShape.MouseUp += delegate (object oSender, MouseButtonEventArgs mE)
                {
                    System.Windows.Point pMouseUpPoint = mE.GetPosition(gMainMultiScaleImageBox);
                    mE.Handled = false;
                    Task.Run(delegate
                    {
                        Thread.Sleep(200);
                        if (!bDoubleClick)
                        {
                            bDoubleClickTimeOut = true;
                            base.Dispatcher.Invoke(delegate
                            {
                                if (pMouseUpPoint == pMouseDownPointUseForShowInfo)
                                {
                                    Canvas canvas = sMarkShape.DataContext as Canvas;
                                    canvas.Children[0].Visibility = Visibility.Visible;
                                    int index3 = gMainMarkBox.Children.IndexOf(canvas);
                                    ((sMarkList.Children[index3] as Border).FindName("lToggleMarkInfo") as System.Windows.Controls.Label).Content = GlobalVariable.listCurrentLanguage[95];
                                    Mark.UpdateAllMarkInfoSwitch(gMainMarkBox, lAllInfoShow, lAllInfoHide, rdDirectory);
                                }
                            });
                        }
                    });
                };
            }
            sBtnBox.Children[0].MouseLeftButtonDown += lEditMark_MouseDown;
            sBtnBox.Children[1].MouseLeftButtonDown += delegate (object oSender, MouseButtonEventArgs mE)
            {
                mE.Handled = true;
                System.Windows.Media.Brush background = mMark.cMarkBox.Background;
                mMark.cMarkBox.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 51, 76, 107));
                bool? flag = new TipInfo
                {
                    strTipInfo = GlobalVariable.listCurrentLanguage[96],
                    strTitle = GlobalVariable.listCurrentLanguage[97],
                    strSureName = GlobalVariable.listCurrentLanguage[98],
                    strCancleName = GlobalVariable.listCurrentLanguage[99]
                }.ShowDialog();
                mMark.cMarkBox.Background = background;
                if (flag != false)
                {
                    Canvas element2 = (oSender as System.Windows.Controls.Label).DataContext as Canvas;
                    int index2 = gMainMarkBox.Children.IndexOf(element2);
                    Border bMarkListItem = sMarkList.Children[index2] as Border;
                    DeleteMark(mMark.strGuId, mMark.cMarkBox, bMarkListItem);
                }
            };
            sBtnBox.Children[2].MouseLeftButtonDown += delegate (object oSender, MouseButtonEventArgs mE)
            {
                mE.Handled = true;
                (mMark.cMarkBox.Children[0] as Border).Visibility = Visibility.Collapsed;
                Canvas element = (oSender as System.Windows.Controls.Label).DataContext as Canvas;
                int index = gMainMarkBox.Children.IndexOf(element);
                ((sMarkList.Children[index] as Border).FindName("lToggleMarkInfo") as System.Windows.Controls.Label).Content = GlobalVariable.listCurrentLanguage[100];
                Mark.UpdateAllMarkInfoSwitch(gMainMarkBox, lAllInfoShow, lAllInfoHide, rdDirectory);
            };
        }

        private Border AddEventForMarkList(Mark mMark, string strShape, Canvas cMarkBox)
        {
            Border bMarkListItemBorder = new Border();
            bMarkListItemBorder.DataContext = mMark.strGuId;
            bMarkListItemBorder.SetValue(FrameworkElement.StyleProperty, rdDirectory["MarkListItem"]);
            NameScope.SetNameScope(bMarkListItemBorder, new NameScope());
            StackPanel stackPanel = new StackPanel();
            StackPanel stackPanel2 = new StackPanel();
            stackPanel2.Orientation = System.Windows.Controls.Orientation.Horizontal;
            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            label.Width = 180.0;
            label.SetValue(FrameworkElement.StyleProperty, rdDirectory["MarkListItemText"]);
            switch (strShape)
            {
                case "Rectangle":
                    label.Content = GlobalVariable.listCurrentLanguage[101];
                    break;
                case "Ellipse":
                    label.Content = GlobalVariable.listCurrentLanguage[102];
                    break;
                case "Line":
                    label.Content = GlobalVariable.listCurrentLanguage[103];
                    break;
                case "Arrow":
                    label.Content = GlobalVariable.listCurrentLanguage[104];
                    break;
                case "Polygon":
                    label.Content = GlobalVariable.listCurrentLanguage[105];
                    break;
            }
            stackPanel2.Children.Add(label);
            switch (strShape)
            {
                case "Rectangle":
                case "Ellipse":
                case "Polygon":
                    {
                        System.Windows.Controls.CheckBox checkBox = new System.Windows.Controls.CheckBox();
                        checkBox.VerticalContentAlignment = VerticalAlignment.Center;
                        stackPanel2.Children.Add(checkBox);
                        bMarkListItemBorder.RegisterName("cbRadio", checkBox);
                        checkBox.DataContext = 1;
                        if (cbSelectAllMark.IsChecked == true)
                        {
                            checkBox.IsChecked = true;
                        }
                        checkBox.Checked += MarkListCheckBox_CheckChanged;
                        checkBox.Unchecked += MarkListCheckBox_CheckChanged;
                        break;
                    }
            }
            TextBlock textBlock = new TextBlock();
            textBlock.SetValue(FrameworkElement.StyleProperty, rdDirectory["MarkListItemTextBox"]);
            textBlock.Text = GlobalVariable.listCurrentLanguage[46] + "：" + mMark.strMarkName;
            TextBlock textBlock2 = new TextBlock();
            textBlock2.SetValue(FrameworkElement.StyleProperty, rdDirectory["MarkListItemTextBox"]);
            textBlock2.Text = GlobalVariable.listCurrentLanguage[48] + "：" + mMark.strMarkDescription;
            StackPanel stackPanel3 = new StackPanel();
            stackPanel3.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtnBox"]);
            System.Windows.Controls.Label lToggleMarkInfo = new System.Windows.Controls.Label();
            lToggleMarkInfo.SetValue(FrameworkElement.StyleProperty, rdDirectory["MarkListItemFirstBtn"]);
            Border bInfoPannelBorder = cMarkBox.Children[0] as Border;
            System.Windows.Controls.Label lToggleMark = new System.Windows.Controls.Label();
            lToggleMark.SetValue(FrameworkElement.StyleProperty, rdDirectory["MarkListItemBtn"]);
            lToggleMark.Content = GlobalVariable.listCurrentLanguage[94];
            if (bInfoPannelBorder.Visibility == Visibility.Collapsed)
            {
                lToggleMarkInfo.Content = GlobalVariable.listCurrentLanguage[100];
            }
            else
            {
                lToggleMarkInfo.Content = GlobalVariable.listCurrentLanguage[95];
            }
            lToggleMarkInfo.MouseLeftButtonDown += delegate
            {
                if (CheckSlideState())
                {
                    if (bInfoPannelBorder.Visibility == Visibility.Visible)
                    {
                        bInfoPannelBorder.Visibility = Visibility.Collapsed;
                        lToggleMarkInfo.Content = GlobalVariable.listCurrentLanguage[100];
                        Mark.UpdateAllMarkInfoSwitch(gMainMarkBox, lAllInfoShow, lAllInfoHide, rdDirectory);
                    }
                    else
                    {
                        bInfoPannelBorder.Visibility = Visibility.Visible;
                        lToggleMarkInfo.Content = GlobalVariable.listCurrentLanguage[95];
                        cMarkBox.Visibility = Visibility.Visible;
                        lToggleMark.Content = GlobalVariable.listCurrentLanguage[94];
                        Mark.UpdateAllMarkSwitch(gMainMarkBox, lAllMarkShow, lAllMarkHide, rdDirectory);
                        Mark.UpdateAllMarkInfoSwitch(gMainMarkBox, lAllInfoShow, lAllInfoHide, rdDirectory);
                    }
                }
            };
            lToggleMark.MouseLeftButtonDown += delegate
            {
                if (cMarkBox.Visibility == Visibility.Visible)
                {
                    cMarkBox.Visibility = Visibility.Collapsed;
                    lToggleMark.Content = GlobalVariable.listCurrentLanguage[106];
                    Mark.UpdateAllMarkSwitch(gMainMarkBox, lAllMarkShow, lAllMarkHide, rdDirectory);
                    MagnifierTool.UpdateMagnifierMark(mMainMultiScaleImage, gMagnifierFlipBox);
                }
                else
                {
                    cMarkBox.Visibility = Visibility.Visible;
                    lToggleMark.Content = GlobalVariable.listCurrentLanguage[94];
                    Mark.UpdateAllMarkSwitch(gMainMarkBox, lAllMarkShow, lAllMarkHide, rdDirectory);
                    MagnifierTool.UpdateMagnifierMark(mMainMultiScaleImage, gMagnifierFlipBox);
                }
            };
            System.Windows.Controls.Label label2 = new System.Windows.Controls.Label();
            label2.SetValue(FrameworkElement.StyleProperty, rdDirectory["MarkListItemBtn"]);
            label2.Content = GlobalVariable.listCurrentLanguage[114];
            System.Windows.Media.Brush bOldBackground = cMarkBox.Background;
            label2.MouseLeftButtonDown += delegate
            {
                if (CheckSlideState())
                {
                    if (cMarkBox.Visibility == Visibility.Collapsed)
                    {
                        cMarkBox.Visibility = Visibility.Visible;
                        lToggleMark.Content = GlobalVariable.listCurrentLanguage[94];
                        Mark.UpdateAllMarkSwitch(gMainMarkBox, lAllMarkShow, lAllMarkHide, rdDirectory);
                    }
                    PositionMark(mMark);
                    cMarkBox.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 51, 76, 107));
                }
            };
            bool bQueryDeleteState = false;
            System.Windows.Controls.Label label3 = new System.Windows.Controls.Label();
            label3.SetValue(FrameworkElement.StyleProperty, rdDirectory["MarkListItemBtn"]);
            label3.Content = GlobalVariable.listCurrentLanguage[107];
            label3.MouseLeftButtonDown += delegate
            {
                if (CheckSlideState())
                {
                    PositionMark(mMark);
                    bQueryDeleteState = true;
                    bMarkPress = false;
                    bOldBackground = cMarkBox.Background;
                    cMarkBox.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 51, 76, 107));
                    bool? flag = new TipInfo
                    {
                        strTipInfo = GlobalVariable.listCurrentLanguage[96],
                        strTitle = GlobalVariable.listCurrentLanguage[97],
                        strSureName = GlobalVariable.listCurrentLanguage[98],
                        strCancleName = GlobalVariable.listCurrentLanguage[99]
                    }.ShowDialog();
                    bQueryDeleteState = false;
                    cMarkBox.Background = bOldBackground;
                    if (flag != false)
                    {
                        DeleteMark(mMark.strGuId, cMarkBox, bMarkListItemBorder);
                    }
                }
            };
            bMarkListItemBorder.MouseLeave += delegate
            {
                if (!bQueryDeleteState)
                {
                    cMarkBox.Background = bOldBackground;
                }
            };
            stackPanel3.Children.Add(lToggleMarkInfo);
            stackPanel3.Children.Add(label2);
            stackPanel3.Children.Add(label3);
            stackPanel3.Children.Add(lToggleMark);
            stackPanel.Children.Add(stackPanel2);
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(textBlock2);
            stackPanel.Children.Add(stackPanel3);
            bMarkListItemBorder.Child = stackPanel;
            bMarkListItemBorder.RegisterName("tbTitle", textBlock);
            bMarkListItemBorder.RegisterName("tbDescription", textBlock2);
            bMarkListItemBorder.RegisterName("lToggleMarkInfo", lToggleMarkInfo);
            bMarkListItemBorder.RegisterName("lToggleMark", lToggleMark);
            sMarkList.Children.Add(bMarkListItemBorder);
            return bMarkListItemBorder;
        }

        private void AddEventForMagnifier()
        {
            mMagnifier.MouseEnter += delegate (object oSender, System.Windows.Input.MouseEventArgs mE)
            {
                if (bSlidePress)
                {
                    pSlideLoseFocusPoint = mE.GetPosition(gMainMultiScaleImageBox);
                    bSlidePressAndLoseFocus = true;
                }
            };
            mMagnifier.MouseLeave += delegate (object oSender, System.Windows.Input.MouseEventArgs mE)
            {
                if (bSlidePress)
                {
                    pSlideLoseFocusPoint = mE.GetPosition(gMainMultiScaleImageBox);
                    bSlidePressAndLoseFocus = true;
                }
            };
            mMagnifier.MouseMove += delegate (object o, System.Windows.Input.MouseEventArgs e)
            {
                if (!bAddMarkState && !bEditMarkState && bSlidePressAndLoseFocus)
                {
                    System.Windows.Point position = e.GetPosition(gMainMultiScaleImageBox);
                    System.Windows.Point pMainSlideDeltaOffset = default(System.Windows.Point);
                    float num = (float)(position.X - pSlideLoseFocusPoint.X);
                    float num2 = (float)(position.Y - pSlideLoseFocusPoint.Y);
                    pSlideLoseFocusPoint = position;
                    float num3 = (float)Math.Sin(Math.PI / 180.0 * (double)iAngle);
                    float num4 = (float)Math.Cos(Math.PI / 180.0 * (double)iAngle);
                    pMainSlideDeltaOffset.X = (0f - num) * num4 - num2 * num3;
                    pMainSlideDeltaOffset.Y = (0f - num2) * num4 + num * num3;
                    UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pMainSlideDeltaOffset, new System.Windows.Point(0.0, 0.0));
                }
            };
            mMagnifier.MouseUp += delegate (object o, MouseButtonEventArgs e)
            {
                if (bSlidePressAndLoseFocus)
                {
                    mMainMultiScaleImage.bMouseDown = true;
                    mMainMultiScaleImage.bMouseMove = true;
                    mMainMultiScaleImage.MarkMouseUp(e);
                    bSlidePress = false;
                    bMarkPress = false;
                }
            };
        }

        private void UpdateSlideAndTools(float fMainSlideTargetScale, System.Windows.Point pMainSlideDeltaOffset, System.Windows.Point pScaleCenter, bool animate = false, float fDurationTime = 0f, EasingFunctionBase easing = null)
        {
            float currentScale = mMainMultiScaleImage.GetCurrentScale();
            float fZoom = ZoomTool.ScaleToZoom(mMainMultiScaleImage, fMainSlideTargetScale, fScreenRatio);
            foreach (FrameworkElement child in gAllSlideBox.Children)
            {
                if (child is Border)
                {
                    MultiScaleImage multiScaleImage = ((child as Border).Child as Grid).FindName("mMultiScaleImage") as MultiScaleImage;
                    if ((cbJoinSlide.IsChecked != false || multiScaleImage == mMainMultiScaleImage) && multiScaleImage.Source != null)
                    {
                        ZoomTool.ZoomToScale(multiScaleImage, fZoom, fScreenRatio);
                        fDurationTime = UpdateSlide(multiScaleImage, fMainSlideTargetScale, pMainSlideDeltaOffset, pScaleCenter, animate, fDurationTime, easing);
                        easing = new CubicEase();
                        bool bScaleChanged = Math.Round(fMainSlideTargetScale, 6) != Math.Round(currentScale, 6);
                        UpdateTools(multiScaleImage, bScaleChanged, animate, fDurationTime, easing, pScaleCenter);
                    }
                }
            }
        }

        public void DelegateUpdateSlideAndTools(object sender, System.Windows.Point pDeltaOffset, System.Windows.Point pScaleCenter, bool animate = false, float fDurationTime = 0f, EasingFunctionBase easing = null)
        {
            float fTargetScale = mMainMultiScaleImage.fTargetScale;
            float currentScale = mMainMultiScaleImage.GetCurrentScale();
            float fZoom = ZoomTool.ScaleToZoom(mMainMultiScaleImage, fTargetScale, fScreenRatio);
            System.Windows.Point point = default(System.Windows.Point);
            foreach (FrameworkElement child in gAllSlideBox.Children)
            {
                if (child is Border)
                {
                    MultiScaleImage multiScaleImage = ((child as Border).Child as Grid).FindName("mMultiScaleImage") as MultiScaleImage;
                    if ((cbJoinSlide.IsChecked != false || multiScaleImage == mMainMultiScaleImage) && multiScaleImage.Source != null)
                    {
                        if (multiScaleImage == mMainMultiScaleImage)
                        {
                            bool bScaleChanged = Math.Round(fTargetScale, 6) != Math.Round(currentScale, 6);
                            UpdateTools(multiScaleImage, bScaleChanged, animate, fDurationTime, easing, pScaleCenter);
                        }
                        else
                        {
                            float num = ZoomTool.ZoomToScale(multiScaleImage, fZoom, fScreenRatio);
                            float num2 = multiScaleImage.fTargetScale;
                            point = PublicMethods.FlipDelta(gMainMultiScaleImageBox, pDeltaOffset);
                            UpdateSlide(multiScaleImage, fTargetScale, point, pScaleCenter, animate, fDurationTime, easing);
                            bool bScaleChanged2 = Math.Round(num, 6) != Math.Round(num2, 6);
                            UpdateTools(multiScaleImage, bScaleChanged2, animate, fDurationTime, easing, pScaleCenter);
                        }
                    }
                }
            }
        }

        public float UpdateSlide(MultiScaleImage mCycleSlide, float fMainSlideTargetScale, System.Windows.Point pDeltaOffset, System.Windows.Point pScaleCenter, bool animate = false, float fDurationTime = 0f, EasingFunctionBase easing = null)
        {
            if (mCycleSlide.Source == null)
            {
                return 0f;
            }
            if (pDeltaOffset != new System.Windows.Point(0.0, 0.0))
            {
                int elemenRelativeAngle = PublicMethods.GetElemenRelativeAngle(mMainMultiScaleImage);
                System.Windows.Point pFlipDelta = default(System.Windows.Point);
                int elemenRelativeAngle2 = PublicMethods.GetElemenRelativeAngle(mCycleSlide);
                float num = (float)Math.Sin(Math.PI / 180.0 * (double)elemenRelativeAngle);
                float num2 = (float)Math.Cos(Math.PI / 180.0 * (double)elemenRelativeAngle);
                System.Windows.Point point = default(System.Windows.Point);
                point.X = pDeltaOffset.X * (double)num2 - pDeltaOffset.Y * (double)num;
                point.Y = pDeltaOffset.Y * (double)num2 + pDeltaOffset.X * (double)num;
                num = (float)Math.Sin(Math.PI / 180.0 * (double)elemenRelativeAngle2);
                num2 = (float)Math.Cos(Math.PI / 180.0 * (double)elemenRelativeAngle2);
                pFlipDelta.X = 0.0 - ((0.0 - point.X) * (double)num2 - point.Y * (double)num);
                pFlipDelta.Y = 0.0 - (point.X * (double)num - point.Y * (double)num2);
                pFlipDelta = PublicMethods.FlipDelta(PublicMethods.FindSlideBox(mCycleSlide), pFlipDelta);
                mCycleSlide.DeltaToPan(pFlipDelta, animate, 1, fDurationTime, easing);
            }
            float num3 = ZoomTool.ScaleToZoom(mMainMultiScaleImage, mMainMultiScaleImage.GetCurrentScale(), fScreenRatio);
            float num4 = ZoomTool.ScaleToZoom(mMainMultiScaleImage, fMainSlideTargetScale, fScreenRatio);
            float num5 = ZoomTool.ScaleToZoom(mCycleSlide, mCycleSlide.GetCurrentScale(), fScreenRatio);
            float num6 = num4;
            if (num5 != num6)
            {
                float num7;
                float currentScale;
                if (num5 != num3)
                {
                    num7 = ZoomTool.ZoomToScale(mCycleSlide, num3, fScreenRatio);
                    currentScale = mCycleSlide.GetCurrentScale();
                    mCycleSlide.ScaleCanvas(num7 / currentScale, pScaleCenter);
                    UpdateTools(mCycleSlide, true);
                }
                num7 = ZoomTool.ZoomToScale(mCycleSlide, num6, fScreenRatio);
                currentScale = mCycleSlide.GetCurrentScale();
                fDurationTime = mCycleSlide.ScaleCanvas(num7 / currentScale, pScaleCenter, animate);
            }
            return fDurationTime;
        }

        public void UpdateTools(MultiScaleImage mSlide, bool bScaleChanged, bool animate = false, float fDurationTime = 0f, EasingFunctionBase easing = null, System.Windows.Point? pScaleCenter = null)
        {
            Grid grid = PublicMethods.FindSlideBox(mSlide);
            grid.FindName("gMarkBox");
            Mark.UpdateAllMarks(grid, fScreenRatio, animate, fDurationTime, easing);
            ViewRectTool.UpdateViewRect(grid, bScaleChanged, animate, fDurationTime, easing);
            if (mtMeasureTool != null)
            {
                mtMeasureTool.UpdateMeasureTool(mSlide, animate, fDurationTime, easing);
            }
            if (bScaleChanged)
            {
                RulerTool.UpdateRuler(grid, ref fWidthPerPixel);
            }
            if (bMagnifierBorder.Visibility == Visibility.Visible)
            {
                if (lFixed.Background.ToString() == "#FFFFFFFF")
                {
                    MagnifierTool.MagnifierZoomAboutCenter(bMagnifierBorder, this, gAllSlideBox, fScreenRatio);
                }
                else
                {
                    System.Windows.Point pSlideBoxPixelPoint = new System.Windows.Point(0.0, 0.0);
                    if (pScaleCenter.HasValue)
                    {
                        pSlideBoxPixelPoint = pScaleCenter.Value;
                    }
                    MagnifierTool.MagnifierZoomAboutPoint(gMagnifierFlipBox, mSlide, bMagnifierBorder, pSlideBoxPixelPoint, fScreenRatio);
                }
            }
            if (mSlide == mMainMultiScaleImage)
            {
                NavigatorTool.UpdateNavigationViewport(mMainMultiScaleImage, mNavigatorImage, rViewport, lHorLine, lVerLine);
                if (bScaleChanged)
                {
                    ZoomTool.UpdateZoomValue(mMainMultiScaleImage, lZoomValue, rController, lValueLine, fScreenRatio, lOne, lDefault);
                }
            }
        }

        private void lNext_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int num = 0;
            string a = mMainMultiScaleImage.DataContext as string;
            foreach (Border child in sSlideList.Children)
            {
                if (a == child.DataContext.ToString())
                {
                    num = sSlideList.Children.IndexOf(child);
                    break;
                }
            }
            if (num != sSlideList.Children.Count - 1)
            {
                num++;
                bThumbnailBorder_MouseLeftButtonDown(sSlideList.Children[num], e);
            }
        }

        private void lPrevious_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int num = 0;
            string a = mMainMultiScaleImage.DataContext as string;
            foreach (Border child in sSlideList.Children)
            {
                if (a == child.DataContext.ToString())
                {
                    num = sSlideList.Children.IndexOf(child);
                    break;
                }
            }
            if (num != 0)
            {
                num--;
                bThumbnailBorder_MouseLeftButtonDown(sSlideList.Children[num], e);
            }
        }

        private void lMoveUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckSlideState() && mMainMultiScaleImage.bMouseDragAvailable)
                {
                    System.Windows.Point pMainSlideDeltaOffset = default(System.Windows.Point);
                    int num = 20;
                    float num2 = (float)Math.Sin(Math.PI / 180.0 * (double)iAngle);
                    float num3 = (float)Math.Cos(Math.PI / 180.0 * (double)iAngle);
                    pMainSlideDeltaOffset.X = (float)(-num) * num2;
                    pMainSlideDeltaOffset.Y = (float)(-num) * num3;
                    CubicEase easing = new CubicEase();
                    float fDurationTime = 100f;
                    UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pMainSlideDeltaOffset, new System.Windows.Point(0.0, 0.0), true, fDurationTime, easing);
                }
            }
            catch
            {
            }
        }

        private void lMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckSlideState() && mMainMultiScaleImage.bMouseDragAvailable)
                {
                    System.Windows.Point pMainSlideDeltaOffset = default(System.Windows.Point);
                    int num = 20;
                    float num2 = (float)Math.Sin(Math.PI / 180.0 * (double)iAngle);
                    float num3 = (float)Math.Cos(Math.PI / 180.0 * (double)iAngle);
                    pMainSlideDeltaOffset.X = (float)(-num) * num3;
                    pMainSlideDeltaOffset.Y = (float)num * num2;
                    CubicEase easing = new CubicEase();
                    float fDurationTime = 100f;
                    UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pMainSlideDeltaOffset, new System.Windows.Point(0.0, 0.0), true, fDurationTime, easing);
                }
            }
            catch
            {
            }
        }

        private void lMoveDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckSlideState() && mMainMultiScaleImage.bMouseDragAvailable)
                {
                    System.Windows.Point pMainSlideDeltaOffset = default(System.Windows.Point);
                    int num = 20;
                    float num2 = (float)Math.Sin(Math.PI / 180.0 * (double)iAngle);
                    float num3 = (float)Math.Cos(Math.PI / 180.0 * (double)iAngle);
                    pMainSlideDeltaOffset.X = (float)num * num2;
                    pMainSlideDeltaOffset.Y = (float)num * num3;
                    CubicEase easing = new CubicEase();
                    float fDurationTime = 100f;
                    UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pMainSlideDeltaOffset, new System.Windows.Point(0.0, 0.0), true, fDurationTime, easing);
                }
            }
            catch
            {
            }
        }

        private void lMoveRight_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckSlideState() && mMainMultiScaleImage.bMouseDragAvailable)
                {
                    System.Windows.Point pMainSlideDeltaOffset = default(System.Windows.Point);
                    int num = 20;
                    float num2 = (float)Math.Sin(Math.PI / 180.0 * (double)iAngle);
                    float num3 = (float)Math.Cos(Math.PI / 180.0 * (double)iAngle);
                    pMainSlideDeltaOffset.X = (float)num * num3;
                    pMainSlideDeltaOffset.Y = (float)(-num) * num2;
                    CubicEase easing = new CubicEase();
                    float fDurationTime = 100f;
                    UpdateSlideAndTools(mMainMultiScaleImage.GetCurrentScale(), pMainSlideDeltaOffset, new System.Windows.Point(0.0, 0.0), true, fDurationTime, easing);
                }
            }
            catch
            {
            }
        }

        private void lViewInformationHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bViewInfoBorder.Visibility = Visibility.Collapsed;
            ToolBarBtn_MouseLeave(bViewInfo, new RoutedEventArgs());
        }

        private void lGridShow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lGridShow.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtnActive"]);
            lGridHide.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtn"]);
            ViewRectTool.CreateAllSlideViewRect(gAllSlideBox, bViewInfoBorder, gViewInfo);
        }

        private void lGridHide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lGridShow.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtn"]);
            lGridHide.SetValue(FrameworkElement.StyleProperty, rdDirectory["SwitchBtnActive"]);
            ViewRectTool.DeleteAllSlideViewRect(gAllSlideBox);
        }

        private void SetCrrentSlideBox(Grid gSlideBox)
        {
            if ((bAddMarkState && gMainMarkBox.Children.Contains(mCurrentMark.cMarkBox)) || (bStartMeasure && mtMeasureTool.iMeasurePointCount == 1))
            {
                return;
            }
            if (gMainMultiScaleImageBox != gSlideBox)
            {
                foreach (FrameworkElement child in gAllSlideBox.Children)
                {
                    if (child is Border)
                    {
                        SplitScreenTool.RemoveActiveStyle((child as Border).Child as Grid);
                    }
                }
                SetCurrentSlide(gSlideBox);
            }
            if (bAddMarkState)
            {
                if (mMainMultiScaleImage.Source == null)
                {
                    CloseMarkTool(2);
                }
                else
                {
                    gMainMarkBox.Children.Add(mCurrentMark.cMarkBox);
                }
            }
            if (bStartMeasure)
            {
                if (mMainMultiScaleImage.Source != null)
                {
                    mtMeasureTool = new Measure(gMainMultiScaleImageBox);
                }
                else
                {
                    CloseMeasureTool();
                }
            }
        }

        private void SetCurrentSlide(Grid gSlideBox)
        {
            gMainMultiScaleImageBox = gSlideBox;
            mMainMultiScaleImage = (gMainMultiScaleImageBox.FindName("mMultiScaleImage") as MultiScaleImage);
            if (gAllSlideBox.Children.Count > 1)
            {
                SplitScreenTool.AddActiveStyle(gMainMultiScaleImageBox);
            }
            gMainMarkBox = (gMainMultiScaleImageBox.FindName("gMarkBox") as Grid);
            NavigatorTool.ResetNavigatorSizeAndSource(mMainMultiScaleImage, mNavigatorImage, ref fNavigatorWidth, ref fNavigatorHeight, fScreenRatio);
            NavigatorTool.UpdateNavigationViewport(mMainMultiScaleImage, mNavigatorImage, rViewport, lHorLine, lVerLine);
            System.Windows.Point slideBoxFlipValue = PublicMethods.GetSlideBoxFlipValue(gMainMultiScaleImageBox);
            FlipTool.SetMirrorOption(slideBoxFlipValue.X, slideBoxFlipValue.Y, cbFlipHorizontal, cbFlipVertical, iDefaultAngle);
            if (PublicMethods.GetElementFlipValue(gNavigatorImageBox) != slideBoxFlipValue)
            {
                FlipTool.FlipElement(gNavigatorImageBox, slideBoxFlipValue.X, slideBoxFlipValue.Y);
            }
            RotateTool.RotateNavigator(bNavigation, mNavigatorImage, PublicMethods.GetElementActualAngle(mMainMultiScaleImage), fNavigatorWidth, fNavigatorHeight);
            int num = 0;
            if (mMainMultiScaleImage.Source == null)
            {
                num = iDefaultAngle;
                bMirrorToolBorder.Visibility = Visibility.Collapsed;
                ToolBarBtn_MouseLeave(bMirror, new RoutedEventArgs());
            }
            else
            {
                num = PublicMethods.GetElemenRelativeAngle(mMainMultiScaleImage);
            }
            RotateTool.RotateIcon(num, iDefaultAngle, imgRotate, lAngleNum);
            iAngle = PublicMethods.GetElemenRelativeAngle(mMainMultiScaleImage);
            ResetZoomTool();
            ZoomTool.UpdateZoomValue(mMainMultiScaleImage, lZoomValue, rController, lValueLine, fScreenRatio, lOne, lDefault);
            LabelTool.UpdateSlideLabel(mMainMultiScaleImage.DataContext as string, imgLabel, cLabelTool, zypSlide);
            sMarkList.Children.Clear();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            for (int i = 0; i < gMainMarkBox.Children.Count; i++)
            {
                Canvas canvas = gMainMarkBox.Children[i] as Canvas;
                MarkInfo markInfo = canvas.DataContext as MarkInfo;
                Mark mMark = Mark.DeserializeMarkInfo(markInfo, serializer, fScreenRatio);
                AddEventForMarkList(mMark, markInfo.strShape, canvas);
            }
            SlidelistTool.SetThumbnailBorderStyle(mMainMultiScaleImage.DataContext as string, sSlideList);
            ScanInfoTool.GetSlideScanInfo(mMainMultiScaleImage, sDefaultScanInfo, gScanInfo);
        }

        private void cbJoinSlide_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void cbJoinSlide_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void lMoreColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            colorDialog.FullOpen = true;
            System.Windows.Media.Color color = ((SolidColorBrush)rChooseColor.Fill).Color;
            colorDialog.Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
            foreach (System.Windows.Controls.Label child in sChooseColor.Children)
            {
                child.BorderBrush = System.Windows.Media.Brushes.White;
            }
            lMoreColor.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 204));
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color2 = colorDialog.Color;
                rChooseColor.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color2.A, color2.R, color2.G, color2.B));
                bLineColor = rChooseColor.Fill;
            }
            else
            {
                bLineColor = rChooseColor.Fill;
            }
            if (mCurrentMark != null && bAddMarkState)
            {
                mCurrentMark.bLineColor = bLineColor;
            }
        }

        private void lMoreEditColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (mCurrentMark != null)
            {
                ColorDialog colorDialog = new ColorDialog();
                colorDialog.AllowFullOpen = true;
                colorDialog.FullOpen = true;
                System.Windows.Media.Color color = ((SolidColorBrush)rChooseEditColor.Fill).Color;
                colorDialog.Color = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
                foreach (System.Windows.Controls.Label child in sEditColor.Children)
                {
                    child.BorderBrush = System.Windows.Media.Brushes.White;
                }
                lMoreEditColor.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 204, 204));
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    System.Drawing.Color color2 = colorDialog.Color;
                    rChooseEditColor.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color2.A, color2.R, color2.G, color2.B));
                    mCurrentMark.bLineColor = rChooseEditColor.Fill;
                }
                else
                {
                    mCurrentMark.bLineColor = rChooseEditColor.Fill;
                }
            }
        }

        private void lPictureSave_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true;
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(mMainMultiScaleImage.DataContext as string);
                string str = lZoomValue.Content as string;
                saveFileDialog.FileName = fileNameWithoutExtension + "_" + str;
                saveFileDialog.Filter = "JPEG files(*.jpg)| *.jpg|PNG files(*.png)| *.png|TIFF files(*.tif)| *.tif|BMP files(*.bmp)| *.bmp";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string fileName = saveFileDialog.FileName;
                    ScreenCaptureTool.SaveImageToFile((BitmapSource)imgPreview.Source, fileName);
                    gCameraPannelBG.Visibility = Visibility.Collapsed;
                    ToolBarBtn_MouseLeave(bCamera, new RoutedEventArgs());
                    System.Windows.MessageBox.Show(GlobalVariable.listCurrentLanguage[108] + "！");
                }
            }
            catch
            {
                System.Windows.MessageBox.Show(GlobalVariable.listCurrentLanguage[109] + "！");
            }
        }

        private void rShowMarks_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (pShowMarksChecked.Visibility == Visibility.Collapsed)
            {
                pShowMarksChecked.Visibility = Visibility.Visible;
            }
            else
            {
                pShowMarksChecked.Visibility = Visibility.Collapsed;
            }
        }

        private void rShowRuler_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (pShowRulerChecked.Visibility == Visibility.Collapsed)
            {
                pShowRulerChecked.Visibility = Visibility.Visible;
            }
            else
            {
                pShowRulerChecked.Visibility = Visibility.Collapsed;
            }
        }

        private void rShowNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (pShowNavigationChecked.Visibility == Visibility.Collapsed)
            {
                pShowNavigationChecked.Visibility = Visibility.Visible;
            }
            else
            {
                pShowNavigationChecked.Visibility = Visibility.Collapsed;
            }
        }

        private void rShowSlideLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (pShowSlideLabelChecked.Visibility == Visibility.Collapsed)
            {
                pShowSlideLabelChecked.Visibility = Visibility.Visible;
            }
            else
            {
                pShowSlideLabelChecked.Visibility = Visibility.Collapsed;
            }
        }

        private void rShowNavigation_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScreenCaptureTool.CameraOptionsChanged(mMainMultiScaleImage, imgPreview, bNavigation, imgDragTool, imgLabel, pShowMarksChecked, pShowRulerChecked, pShowNavigationChecked, pShowSlideLabelChecked, fScreenRatio, rdDirectory);
        }

        private void rShowSlideLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScreenCaptureTool.CameraOptionsChanged(mMainMultiScaleImage, imgPreview, bNavigation, imgDragTool, imgLabel, pShowMarksChecked, pShowRulerChecked, pShowNavigationChecked, pShowSlideLabelChecked, fScreenRatio, rdDirectory);
        }

        private void rShowRuler_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScreenCaptureTool.CameraOptionsChanged(mMainMultiScaleImage, imgPreview, bNavigation, imgDragTool, imgLabel, pShowMarksChecked, pShowRulerChecked, pShowNavigationChecked, pShowSlideLabelChecked, fScreenRatio, rdDirectory);
        }

        private void rShowMarks_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScreenCaptureTool.CameraOptionsChanged(mMainMultiScaleImage, imgPreview, bNavigation, imgDragTool, imgLabel, pShowMarksChecked, pShowRulerChecked, pShowNavigationChecked, pShowSlideLabelChecked, fScreenRatio, rdDirectory);
        }

        private void mMagnifier_MouseWheel(object sender, System.Windows.Point pDeltaOffset, System.Windows.Point pScaleCenter, bool animate = false, float fDurationTime = 0f, EasingFunctionBase easing = null)
        {
            float num = mMagnifier.GetCurrentScale() / (float)mMagnifier.OriginalScale;
            mMagnifier.DataContext = num;
            Mark.UpdateAllMarks(gMagnifierFlipBox, fScreenRatio);
        }

        private void MagnifierExistAndgSlideBox_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!bSlidePress)
            {
                Grid grid = sender as Grid;
                MultiScaleImage mSlide = grid.FindName("mMultiScaleImage") as MultiScaleImage;
                System.Windows.Point position = e.GetPosition(grid);
                MagnifierTool.MagnifierZoomAboutPoint(gMagnifierFlipBox, mSlide, bMagnifierBorder, position, fScreenRatio);
            }
        }

        private void iMagnifierChangeSizeTool_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            bMagnifierChangeSizeToolPress = true;
            System.Windows.Point position = e.GetPosition(this);
            iMagnifierChangeSizeTool.DataContext = position;
        }

        private void lFixed_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Label label = sender as System.Windows.Controls.Label;
            if (label.Background.ToString() == "#FFFFFFFF")
            {
                label.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(192, 192, 192));
                label.ToolTip = GlobalVariable.listCurrentLanguage[110];
                foreach (FrameworkElement child in gAllSlideBox.Children)
                {
                    if (child is Border)
                    {
                        ((child as Border).Child as Grid).MouseMove += MagnifierExistAndgSlideBox_MouseMove;
                    }
                }
            }
            else
            {
                label.Background = new SolidColorBrush(Colors.White);
                label.ToolTip = GlobalVariable.listCurrentLanguage[111];
                foreach (FrameworkElement child2 in gAllSlideBox.Children)
                {
                    if (child2 is Border)
                    {
                        ((child2 as Border).Child as Grid).MouseMove -= MagnifierExistAndgSlideBox_MouseMove;
                    }
                }
                MagnifierTool.MagnifierZoomAboutCenter(bMagnifierBorder, this, gAllSlideBox, fScreenRatio);
            }
        }

        private void cbFlipHorizontal_Checked(object sender, RoutedEventArgs e)
        {
            if (int.Parse(cbFlipHorizontal.DataContext.ToString()) != 2)
            {
                System.Windows.Point slideBoxFlipValue = PublicMethods.GetSlideBoxFlipValue(gMainMultiScaleImageBox);
                foreach (FrameworkElement child in gAllSlideBox.Children)
                {
                    if (child is Border)
                    {
                        Grid grid = (child as Border).Child as Grid;
                        MultiScaleImage multiScaleImage = grid.FindName("mMultiScaleImage") as MultiScaleImage;
                        if ((cbJoinSlide.IsChecked != false || multiScaleImage == mMainMultiScaleImage) && multiScaleImage.Source != null)
                        {
                            System.Windows.Point slideBoxFlipValue2 = PublicMethods.GetSlideBoxFlipValue(grid);
                            if (iDefaultAngle / 90 % 2 == 1)
                            {
                                if (slideBoxFlipValue2.Y == slideBoxFlipValue.Y)
                                {
                                    slideBoxFlipValue2.Y = -1.0;
                                }
                                else
                                {
                                    slideBoxFlipValue2.Y = 1.0;
                                }
                            }
                            else if (slideBoxFlipValue2.X == slideBoxFlipValue.X)
                            {
                                slideBoxFlipValue2.X = -1.0;
                            }
                            else
                            {
                                slideBoxFlipValue2.X = 1.0;
                            }
                            int elemenRelativeAngle = PublicMethods.GetElemenRelativeAngle(multiScaleImage);
                            FlipSlide(grid, slideBoxFlipValue2.X, slideBoxFlipValue2.Y, elemenRelativeAngle);
                        }
                    }
                }
            }
        }

        private void cbFlipHorizontal_Unchecked(object sender, RoutedEventArgs e)
        {
            if (int.Parse(cbFlipHorizontal.DataContext.ToString()) != 2)
            {
                System.Windows.Point slideBoxFlipValue = PublicMethods.GetSlideBoxFlipValue(gMainMultiScaleImageBox);
                foreach (FrameworkElement child in gAllSlideBox.Children)
                {
                    if (child is Border)
                    {
                        Grid grid = (child as Border).Child as Grid;
                        MultiScaleImage multiScaleImage = grid.FindName("mMultiScaleImage") as MultiScaleImage;
                        if ((cbJoinSlide.IsChecked != false || multiScaleImage == mMainMultiScaleImage) && multiScaleImage.Source != null)
                        {
                            System.Windows.Point slideBoxFlipValue2 = PublicMethods.GetSlideBoxFlipValue(grid);
                            if (iDefaultAngle / 90 % 2 == 1)
                            {
                                if (slideBoxFlipValue2.Y == slideBoxFlipValue.Y)
                                {
                                    slideBoxFlipValue2.Y = 1.0;
                                }
                                else
                                {
                                    slideBoxFlipValue2.Y = -1.0;
                                }
                            }
                            else if (slideBoxFlipValue2.X == slideBoxFlipValue.X)
                            {
                                slideBoxFlipValue2.X = 1.0;
                            }
                            else
                            {
                                slideBoxFlipValue2.X = -1.0;
                            }
                            int elemenRelativeAngle = PublicMethods.GetElemenRelativeAngle(multiScaleImage);
                            FlipSlide(grid, slideBoxFlipValue2.X, slideBoxFlipValue2.Y, elemenRelativeAngle);
                        }
                    }
                }
            }
        }

        private void cbFlipVertical_Checked(object sender, RoutedEventArgs e)
        {
            if (int.Parse(cbFlipVertical.DataContext.ToString()) != 2)
            {
                System.Windows.Point slideBoxFlipValue = PublicMethods.GetSlideBoxFlipValue(gMainMultiScaleImageBox);
                foreach (FrameworkElement child in gAllSlideBox.Children)
                {
                    if (child is Border)
                    {
                        Grid grid = (child as Border).Child as Grid;
                        MultiScaleImage multiScaleImage = grid.FindName("mMultiScaleImage") as MultiScaleImage;
                        if ((cbJoinSlide.IsChecked != false || multiScaleImage == mMainMultiScaleImage) && multiScaleImage.Source != null)
                        {
                            System.Windows.Point slideBoxFlipValue2 = PublicMethods.GetSlideBoxFlipValue(grid);
                            if (iDefaultAngle / 90 % 2 == 1)
                            {
                                if (slideBoxFlipValue2.X == slideBoxFlipValue.X)
                                {
                                    slideBoxFlipValue2.X = -1.0;
                                }
                                else
                                {
                                    slideBoxFlipValue2.X = 1.0;
                                }
                            }
                            else if (slideBoxFlipValue2.Y == slideBoxFlipValue.Y)
                            {
                                slideBoxFlipValue2.Y = -1.0;
                            }
                            else
                            {
                                slideBoxFlipValue2.Y = 1.0;
                            }
                            int elemenRelativeAngle = PublicMethods.GetElemenRelativeAngle(multiScaleImage);
                            FlipSlide(grid, slideBoxFlipValue2.X, slideBoxFlipValue2.Y, elemenRelativeAngle);
                        }
                    }
                }
            }
        }

        private void cbFlipVertical_Unchecked(object sender, RoutedEventArgs e)
        {
            if (int.Parse(cbFlipVertical.DataContext.ToString()) != 2)
            {
                System.Windows.Point slideBoxFlipValue = PublicMethods.GetSlideBoxFlipValue(gMainMultiScaleImageBox);
                foreach (FrameworkElement child in gAllSlideBox.Children)
                {
                    if (child is Border)
                    {
                        Grid grid = (child as Border).Child as Grid;
                        MultiScaleImage multiScaleImage = grid.FindName("mMultiScaleImage") as MultiScaleImage;
                        if ((cbJoinSlide.IsChecked != false || multiScaleImage == mMainMultiScaleImage) && multiScaleImage.Source != null)
                        {
                            System.Windows.Point slideBoxFlipValue2 = PublicMethods.GetSlideBoxFlipValue(grid);
                            if (iDefaultAngle / 90 % 2 == 1)
                            {
                                if (slideBoxFlipValue2.X == slideBoxFlipValue.X)
                                {
                                    slideBoxFlipValue2.X = 1.0;
                                }
                                else
                                {
                                    slideBoxFlipValue2.X = -1.0;
                                }
                            }
                            else if (slideBoxFlipValue2.Y == slideBoxFlipValue.Y)
                            {
                                slideBoxFlipValue2.Y = 1.0;
                            }
                            else
                            {
                                slideBoxFlipValue2.Y = -1.0;
                            }
                            int elemenRelativeAngle = PublicMethods.GetElemenRelativeAngle(multiScaleImage);
                            FlipSlide(grid, slideBoxFlipValue2.X, slideBoxFlipValue2.Y, elemenRelativeAngle);
                        }
                    }
                }
            }
        }

        private void FlipSlide(Grid gSlideBox, double dScaleX, double dScaleY, int iOldRelativeAngle, int iType = 1)
        {
            FlipTool.FlipElement(gSlideBox.FindName("gFlipBox") as Grid, dScaleX, dScaleY);
            if (gSlideBox == gMainMultiScaleImageBox)
            {
                FlipTool.FlipElement(gNavigatorImageBox, dScaleX, dScaleY);
                FlipTool.SetMirrorOption(dScaleX, dScaleY, cbFlipHorizontal, cbFlipVertical, iDefaultAngle);
            }
            Mark.UpdateMarksInfo(gSlideBox, dScaleX, dScaleY, fScreenRatio);
            if (iType == 1)
            {
                int iNumber = PublicMethods.RelativeAngleToActualAngle(gSlideBox, iOldRelativeAngle);
                iNumber = PublicMethods.RemainderAngle(iNumber, 360);
                RotateSingleSlideAndUpdateTools(gSlideBox, iNumber);
            }
        }

        private void FindSlideToAlign()
        {
            if (gAllSlideBox.Children.Count != 1)
            {
                foreach (FrameworkElement child in gAllSlideBox.Children)
                {
                    if (child is Border)
                    {
                        Grid obj = (child as Border).Child as Grid;
                        MultiScaleImage multiScaleImage = obj.FindName("mMultiScaleImage") as MultiScaleImage;
                        if (obj != gMainMultiScaleImageBox && multiScaleImage.Source != null)
                        {
                            AlignSingleSlide(multiScaleImage, mMainMultiScaleImage);
                            break;
                        }
                    }
                }
            }
        }

        private void AlignSingleSlide(MultiScaleImage mMainSlide, MultiScaleImage mSlide)
        {
        }

        private void AlignAllSlide()
        {
            float fTargetScale = mMainMultiScaleImage.fTargetScale;
            float num = ZoomTool.ScaleToZoom(mMainMultiScaleImage, fTargetScale, fScreenRatio);
            _ = mMainMultiScaleImage.pTargetOffset;
            _ = mMainMultiScaleImage.Source.sbcCurrentSlide.dMaxActualZoom;
            int elementActualAngle = PublicMethods.GetElementActualAngle(mMainMultiScaleImage);
            System.Windows.Point slideBoxFlipValue = PublicMethods.GetSlideBoxFlipValue(gMainMultiScaleImageBox);
            MemoryStream thumbnail = zypSlide.GetThumbnail(mMainMultiScaleImage.DataContext.ToString());
            System.Drawing.Image rOIThumbnail = mMainMultiScaleImage.Source.sbcCurrentSlide.GetROIThumbnail(thumbnail);
            float fScale = 0.001953125f;
            float num2 = ZoomTool.ScaleToZoom(mMainMultiScaleImage, fScale, fScreenRatio);
            float num3 = (float)mMainMultiScaleImage.pTargetOffset.X;
            float num4 = (float)mMainMultiScaleImage.pTargetOffset.Y;
            foreach (FrameworkElement child in gAllSlideBox.Children)
            {
                if (child is Border)
                {
                    Grid grid = (child as Border).Child as Grid;
                    MultiScaleImage multiScaleImage = grid.FindName("mMultiScaleImage") as MultiScaleImage;
                    if (grid != gMainMultiScaleImageBox && multiScaleImage.Source != null)
                    {
                        MemoryStream thumbnail2 = zypSlide.GetThumbnail(multiScaleImage.DataContext.ToString());
                        System.Drawing.Image rOIThumbnail2 = multiScaleImage.Source.sbcCurrentSlide.GetROIThumbnail(thumbnail2);
                        int iDeltaAngle = 0;
                        System.Windows.Point pDeltaFlip = new System.Windows.Point(1.0, 1.0);
                        System.Windows.Point pDeltaOffset = new System.Windows.Point(0.0, 0.0);
                        float fRatio = 0f;
                        AlignTool.RotateImage(rOIThumbnail, iDefaultAngle);
                        AlignTool.RotateImage(rOIThumbnail2, iDefaultAngle);
                        AlignTool.CompareThumbnail(rOIThumbnail, rOIThumbnail2, ref iDeltaAngle, ref pDeltaFlip, ref pDeltaOffset, ref fRatio);
                        thumbnail2.Close();
                        float num5 = ZoomTool.ZoomToScale(multiScaleImage, num, fScreenRatio) / multiScaleImage.fTargetScale;
                        multiScaleImage.ScaleCanvas(num5, new System.Windows.Point(multiScaleImage.ActualWidth / 2.0, multiScaleImage.ActualHeight / 2.0));
                        FlipSlide(grid, slideBoxFlipValue.X, slideBoxFlipValue.Y * pDeltaFlip.Y, 0, 2);
                        int iNumber = elementActualAngle - iDeltaAngle;
                        iNumber = PublicMethods.RemainderAngle(iNumber, 360);
                        RotateSingleSlideAndUpdateTools(grid, iNumber);
                        int acuteAngle = PublicMethods.GetAcuteAngle(iNumber);
                        float num6 = (float)Math.Sin(Math.PI / 180.0 * (double)acuteAngle);
                        float num7 = (float)Math.Cos(Math.PI / 180.0 * (double)acuteAngle);
                        float num8 = (float)grid.ActualWidth;
                        float num9 = (float)grid.ActualHeight;
                        float num10 = num8 * num7 + num9 * num6;
                        float num11 = num8 * num6 + num9 * num7;
                        float num12 = (0f - (num10 - num8)) / 2f;
                        float num13 = (0f - (num11 - num9)) / 2f;
                        fScale = 0.001953125f;
                        num2 = ZoomTool.ScaleToZoom(mMainMultiScaleImage, fScale, fScreenRatio);
                        float num14 = num / num2;
                        pDeltaOffset = new System.Windows.Point(pDeltaOffset.X * (double)num14, pDeltaOffset.Y * (double)num14);
                        System.Windows.Point point = default(System.Windows.Point);
                        point.X = (double)num3 * Math.Cos((double)iNumber * Math.PI / 180.0) + (double)num12;
                        point.Y = (double)num4 * Math.Cos((double)iNumber * Math.PI / 180.0) + (double)num13;
                        pDeltaOffset.X += point.X - multiScaleImage.pTargetOffset.X;
                        pDeltaOffset.Y += point.Y - multiScaleImage.pTargetOffset.Y;
                        multiScaleImage.DeltaToPan(pDeltaOffset);
                        UpdateTools(multiScaleImage, num5 == 1f);
                        thumbnail2.Close();
                    }
                }
            }
            thumbnail.Close();
        }

        private void cbAlignSlide_Checked(object sender, RoutedEventArgs e)
        {
            if (mMainMultiScaleImage.Source == null)
            {
                System.Windows.MessageBox.Show(GlobalVariable.listCurrentLanguage[57] + "!");
                cbAlignSlide.IsChecked = false;
            }
            else
            {
                cbJoinSlide.IsChecked = true;
                cbJoinSlide.IsEnabled = false;
                AlignAllSlide();
            }
        }

        private void cbAlignSlide_Unchecked(object sender, RoutedEventArgs e)
        {
            cbJoinSlide.IsChecked = false;
            cbJoinSlide.IsEnabled = true;
        }

        private void bSubmitExport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                List<string> list = new List<string>();
                foreach (Border child in sMarkList.Children)
                {
                    System.Windows.Controls.CheckBox checkBox = child.FindName("cbRadio") as System.Windows.Controls.CheckBox;
                    if (checkBox != null && checkBox.IsChecked == true)
                    {
                        list.Add(child.DataContext.ToString());
                    }
                }
                if (list.Count == 0)
                {
                    System.Windows.MessageBox.Show(GlobalVariable.listCurrentLanguage[112] + "!");
                }
                else
                {
                    string text = mMainMultiScaleImage.DataContext.ToString();
                    foreach (string item in list)
                    {
                        text = text + "_" + item;
                    }
                    string applicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                    string key = IniSetting.GetKey(applicationBase + "/Configuration.ini", "System", "ExportAppName");
                    Process[] processesByName = Process.GetProcessesByName(key.Replace(".exe", ""));
                    if (processesByName.Length == 0)
                    {
                        Process.Start(new ProcessStartInfo(applicationBase + key, text));
                    }
                    else
                    {
                        IntPtr mainWindowHandle = processesByName[0].MainWindowHandle;
                        int num = Encoding.Default.GetBytes(text).Length;
                        CopyDataStruct lParam = default(CopyDataStruct);
                        lParam.dwData = (IntPtr)0;
                        lParam.cbData = num + 1;
                        lParam.lpData = text;
                        SendMessage(mainWindowHandle, 74, IntPtr.Zero, ref lParam);
                    }
                }
            }
            catch
            {
            }
        }

        private void MarkListCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox checkBox = sender as System.Windows.Controls.CheckBox;
            if (checkBox.DataContext == null || !(checkBox.DataContext.ToString() == "2"))
            {
                bool flag = true;
                foreach (Border child in sMarkList.Children)
                {
                    System.Windows.Controls.CheckBox checkBox2 = child.FindName("cbRadio") as System.Windows.Controls.CheckBox;
                    if (checkBox2 != null && checkBox2.IsChecked == false)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    cbSelectAllMark.DataContext = 2;
                    cbSelectAllMark.IsChecked = true;
                    cbSelectAllMark.DataContext = 1;
                }
                else
                {
                    cbSelectAllMark.DataContext = 2;
                    cbSelectAllMark.IsChecked = false;
                    cbSelectAllMark.DataContext = 1;
                }
            }
        }

        private void cbSelectAllMark_Checked(object sender, RoutedEventArgs e)
        {
            if (cbSelectAllMark.DataContext == null || !(cbSelectAllMark.DataContext.ToString() == "2"))
            {
                foreach (Border child in sMarkList.Children)
                {
                    System.Windows.Controls.CheckBox checkBox = child.FindName("cbRadio") as System.Windows.Controls.CheckBox;
                    if (checkBox != null)
                    {
                        checkBox.DataContext = 2;
                        checkBox.IsChecked = true;
                        checkBox.DataContext = 1;
                    }
                }
            }
        }

        private void cbSelectAllMark_Unchecked(object sender, RoutedEventArgs e)
        {
            if (cbSelectAllMark.DataContext == null || !(cbSelectAllMark.DataContext.ToString() == "2"))
            {
                foreach (Border child in sMarkList.Children)
                {
                    System.Windows.Controls.CheckBox checkBox = child.FindName("cbRadio") as System.Windows.Controls.CheckBox;
                    if (checkBox != null)
                    {
                        checkBox.DataContext = 2;
                        checkBox.IsChecked = false;
                        checkBox.DataContext = 1;
                    }
                }
            }
        }

        private void lLanguageClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bLanguage.Visibility = Visibility.Collapsed;
            ToolBarBtn_MouseLeave(borderLanguage, new RoutedEventArgs());
        }

        private void LoadLanguages()
        {
            GlobalVariable.dictionaryLanguage.Clear();
            string startupPath = System.Windows.Forms.Application.StartupPath;
            FileInfo[] files = new DirectoryInfo(startupPath + "\\Language").GetFiles();
            foreach (FileInfo fileInfo in files)
            {
                cbLanguage.Items.Add(fileInfo.Name);
                using (StreamReader streamReader = new StreamReader(fileInfo.FullName, Encoding.GetEncoding("GBK")))
                {
                    List<string> list = new List<string>();
                    string item;
                    while ((item = streamReader.ReadLine()) != null)
                    {
                        list.Add(item);
                    }
                    GlobalVariable.dictionaryLanguage.Add(fileInfo.Name, list);
                }
            }
            string text = IniSetting.GetKey(startupPath + "/Configuration.ini", "Custom", "Language");
            if (text == "")
            {
                cbLanguage.SelectedIndex = 0;
                text = cbLanguage.SelectedValue.ToString();
            }
            else
            {
                cbLanguage.SelectedIndex = cbLanguage.Items.IndexOf(text);
            }
            GlobalVariable.listCurrentLanguage = GlobalVariable.dictionaryLanguage[text];
            PublicMethods.listCurrentLanguage = GlobalVariable.listCurrentLanguage;
        }

        private void ChangeLanguage()
        {
            this.Title = GlobalVariable.listCurrentLanguage[0];
            bSlideList.ToolTip = GlobalVariable.listCurrentLanguage[1];
            bRotate.ToolTip = GlobalVariable.listCurrentLanguage[2];
            bMeasure.ToolTip = GlobalVariable.listCurrentLanguage[3];
            bMark.ToolTip = GlobalVariable.listCurrentLanguage[4];
            bMarkList.ToolTip = GlobalVariable.listCurrentLanguage[5];
            bSplitScreen.ToolTip = GlobalVariable.listCurrentLanguage[6];
            bCamera.ToolTip = GlobalVariable.listCurrentLanguage[7];
            bMirror.ToolTip = GlobalVariable.listCurrentLanguage[8];
            bMagnifier.ToolTip = GlobalVariable.listCurrentLanguage[9];
            bExport.ToolTip = GlobalVariable.listCurrentLanguage[10];
            bViewInfo.ToolTip = GlobalVariable.listCurrentLanguage[11];
            bScanInfo.ToolTip = GlobalVariable.listCurrentLanguage[12];
            borderLanguage.ToolTip = GlobalVariable.listCurrentLanguage[13];
            btnChooseFolder.ToolTip = GlobalVariable.listCurrentLanguage[14];
            bRefresh.ToolTip = GlobalVariable.listCurrentLanguage[15];
            tbSearch.Text = GlobalVariable.listCurrentLanguage[16];
            cbAutoHide.Content = GlobalVariable.listCurrentLanguage[17];
            lCurrentFolder.Content = GlobalVariable.listCurrentLanguage[18] + ":";
            bLabelToggle.ToolTip = GlobalVariable.listCurrentLanguage[88];
            labelRotate.Content = GlobalVariable.listCurrentLanguage[19];
            labelChangeDelta.Content = GlobalVariable.listCurrentLanguage[20];
            lAngleReset.Content = GlobalVariable.listCurrentLanguage[113];
            labelViewInfo.Content = GlobalVariable.listCurrentLanguage[11];
            labelGrid.Content = GlobalVariable.listCurrentLanguage[21];
            lGridShow.Content = GlobalVariable.listCurrentLanguage[22];
            lGridHide.Content = GlobalVariable.listCurrentLanguage[23];
            labelXMotor.Content = GlobalVariable.listCurrentLanguage[24] + ":";
            labelYMotor.Content = GlobalVariable.listCurrentLanguage[25] + ":";
            labelZMotor.Content = GlobalVariable.listCurrentLanguage[26] + ":";
            labelFocus.Content = GlobalVariable.listCurrentLanguage[27] + ":";
            labelZoomIn.ToolTip = GlobalVariable.listCurrentLanguage[28];
            labelZoomOut.ToolTip = GlobalVariable.listCurrentLanguage[29];
            lDefault.ToolTip = GlobalVariable.listCurrentLanguage[30];
            lPrevious.ToolTip = GlobalVariable.listCurrentLanguage[31];
            lMoveUp.ToolTip = GlobalVariable.listCurrentLanguage[32];
            lNext.ToolTip = GlobalVariable.listCurrentLanguage[33];
            lMoveLeft.ToolTip = GlobalVariable.listCurrentLanguage[34];
            lMoveDown.ToolTip = GlobalVariable.listCurrentLanguage[35];
            lMoveRight.ToolTip = GlobalVariable.listCurrentLanguage[36];
            labelSplitScreen.Content = GlobalVariable.listCurrentLanguage[6];
            labelRect.Content = GlobalVariable.listCurrentLanguage[37];
            bAllRectBorder.ToolTip = GlobalVariable.listCurrentLanguage[38];
            cbJoinSlide.Content = GlobalVariable.listCurrentLanguage[39];
            cbAlignSlide.Content = GlobalVariable.listCurrentLanguage[40];
            labelMark.Content = GlobalVariable.listCurrentLanguage[4];
            labelSelectGraph.Content = GlobalVariable.listCurrentLanguage[41];
            labelSelectColor.Content = GlobalVariable.listCurrentLanguage[42];
            lMoreColor.ToolTip = GlobalVariable.listCurrentLanguage[43];
            labelSelectLineWidth.Content = GlobalVariable.listCurrentLanguage[44];
            lPannelHead.Content = GlobalVariable.listCurrentLanguage[45];
            labelTitleInfo.Content = GlobalVariable.listCurrentLanguage[46];
            tbTitle.Text = GlobalVariable.listCurrentLanguage[47];
            labelContentInfo.Content = GlobalVariable.listCurrentLanguage[48];
            tbContent.Text = GlobalVariable.listCurrentLanguage[49];
            labelColor.Content = GlobalVariable.listCurrentLanguage[50];
            lMoreEditColor.ToolTip = GlobalVariable.listCurrentLanguage[43];
            labelLine.Content = GlobalVariable.listCurrentLanguage[51];
            labelSave.Content = GlobalVariable.listCurrentLanguage[52];
            labelCancle.Content = GlobalVariable.listCurrentLanguage[53];
            labelMarkList.Content = GlobalVariable.listCurrentLanguage[5];
            labelAllMark.Content = GlobalVariable.listCurrentLanguage[54];
            lAllMarkShow.Content = GlobalVariable.listCurrentLanguage[22];
            lAllMarkHide.Content = GlobalVariable.listCurrentLanguage[23];
            labelAllMarkInfo.Content = GlobalVariable.listCurrentLanguage[55];
            lAllInfoShow.Content = GlobalVariable.listCurrentLanguage[22];
            lAllInfoHide.Content = GlobalVariable.listCurrentLanguage[23];
            cbSelectAllMark.Content = GlobalVariable.listCurrentLanguage[56];
            labelExport.Content = GlobalVariable.listCurrentLanguage[10];
            labelScanInfo.Content = GlobalVariable.listCurrentLanguage[12];
            labelFirstOpenFile.Content = GlobalVariable.listCurrentLanguage[57];
            labelVersion.Content = GlobalVariable.listCurrentLanguage[58];
            labelSerialNumber.Content = GlobalVariable.listCurrentLanguage[59];
            labelDeviceType.Content = GlobalVariable.listCurrentLanguage[60];
            labelScanCameraType.Content = GlobalVariable.listCurrentLanguage[61];
            labelPreviewCameraType.Content = GlobalVariable.listCurrentLanguage[62];
            labelObjectiveName.Content = GlobalVariable.listCurrentLanguage[63];
            labelObjectiveLens.Content = GlobalVariable.listCurrentLanguage[64];
            labelInterface.Content = GlobalVariable.listCurrentLanguage[65];
            labelFormat.Content = GlobalVariable.listCurrentLanguage[66];
            labelQuality.Content = GlobalVariable.listCurrentLanguage[67];
            labelFocusInterval.Content = GlobalVariable.listCurrentLanguage[68];
            labelMicroPixel.Content = GlobalVariable.listCurrentLanguage[69];
            labelMagnification.Content = GlobalVariable.listCurrentLanguage[70];
            labelScanMode.Content = GlobalVariable.listCurrentLanguage[71];
            labelScanLevel.Content = GlobalVariable.listCurrentLanguage[72];
            labelScanStep.Content = GlobalVariable.listCurrentLanguage[73];
            labelSlideSize.Content = GlobalVariable.listCurrentLanguage[74];
            labelScanStartTime.Content = GlobalVariable.listCurrentLanguage[75];
            labelFocusTime.Content = GlobalVariable.listCurrentLanguage[76];
            labelWhiteTime.Content = GlobalVariable.listCurrentLanguage[77];
            labelScanTime.Content = GlobalVariable.listCurrentLanguage[78];
            labelChangeLanguage.Content = GlobalVariable.listCurrentLanguage[13];
            labelLanguage.Content = GlobalVariable.listCurrentLanguage[79] + ":";
            labelPicturePreview.Content = GlobalVariable.listCurrentLanguage[80];
            labelShowMarks.Content = GlobalVariable.listCurrentLanguage[81];
            labelShowRuler.Content = GlobalVariable.listCurrentLanguage[82];
            labelShowNavigation.Content = GlobalVariable.listCurrentLanguage[83];
            labelShowSlideLabel.Content = GlobalVariable.listCurrentLanguage[84];
            lPictureSave.Content = GlobalVariable.listCurrentLanguage[52];
            labelMirrorInfo.Content = GlobalVariable.listCurrentLanguage[8];
            cbFlipHorizontal.Content = GlobalVariable.listCurrentLanguage[85];
            cbFlipVertical.Content = GlobalVariable.listCurrentLanguage[86];
            lFixed.ToolTip = GlobalVariable.listCurrentLanguage[87];
            lMagnifierHide.ToolTip = GlobalVariable.listCurrentLanguage[88];
        }

        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = cbLanguage.SelectedValue.ToString();
            GlobalVariable.listCurrentLanguage = GlobalVariable.dictionaryLanguage[text];
            PublicMethods.listCurrentLanguage = GlobalVariable.listCurrentLanguage;
            IniSetting.SaveKey(System.Windows.Forms.Application.StartupPath + "/Configuration.ini", "Custom", "Language", text);
            ChangeLanguage();
        }
    }
}
