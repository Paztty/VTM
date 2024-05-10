using HVT.VTM.Base;
using HVT.VTM.Program;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VTM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();


        AutoPage AutoPage = new AutoPage();
        ManualPage ManualPage = new ManualPage();
        ModelPage ModelPage = new ModelPage();
        VisionPage VisionPage = new VisionPage();
        SettingPage SettingPage = new SettingPage();

        public DispatcherTimer Cleanning = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(1000),
        };

        // 
        public Program MainProgram = new Program();
        public MainWindow()
        {

            InitializeComponent();
            //ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

            Cleanning.Tick += Cleanning_Tick;
            Cleanning.IsEnabled = true;


            AutoPageHolder.Content = AutoPage;
            ManualPageHolder.Content = ManualPage;
            ModelPageHolder.Content = ModelPage;
            VisionPageHolder.Content = VisionPage;
            SettingPageHolder.Content = SettingPage;

            MainProgram.Machine_Init();

            AutoPage.Program = MainProgram;
            ManualPage.Program = MainProgram;
            ModelPage.Program = MainProgram;
            VisionPage.Program = MainProgram;
            SettingPage.Program = MainProgram;

            // communication get
            stackpanelComunication.Children.Add(MainProgram.SYSTEM.System_Board.SerialPort);
            stackpanelComunication.Children.Add(MainProgram.MuxCard.SerialPort1);
            stackpanelComunication.Children.Add(MainProgram.MuxCard.SerialPort2);
            stackpanelComunication.Children.Add(MainProgram._DMM.DMM1.SerialPort);
            stackpanelComunication.Children.Add(MainProgram._DMM.DMM2.SerialPort);
            stackpanelComunication.Children.Add(MainProgram.RELAY.SerialPort);
            stackpanelComunication.Children.Add(MainProgram.LEVEL.SerialPort);
            stackpanelComunication.Children.Add(MainProgram.Solenoid.SerialPort);
            stackpanelComunication.Children.Add(MainProgram.PowerMetter.SerialPort);
            stackpanelComunication.Children.Add(MainProgram.MotorExtension.SerialPort);
            stackpanelComunication.Children.Add(MainProgram.UUTs[0].serial);
            stackpanelComunication.Children.Add(MainProgram.UUTs[1].serial);
            stackpanelComunication.Children.Add(MainProgram.UUTs[2].serial);
            stackpanelComunication.Children.Add(MainProgram.UUTs[3].serial);
            stackpanelComunication.Children.Add(MainProgram.BarcodeReader);
            stackpanelComunication.Children.Add(MainProgram.Printer.Serial);

            //btSelectPage_Click(btAutoPage, null);
            //// binding camera source between 3 page
            Binding cameraBinding = new Binding("LastFrame")
            {
                Source = ManualPage.camera
            };
            AutoPage.cameraViewer.SetBinding(Image.SourceProperty, cameraBinding);
            VisionPage.cameraViewer.SetBinding(Image.SourceProperty, cameraBinding);

            AutoPage.cameraSetting.Capture = ManualPage.camera;
            ManualPage.cameraSetting.Capture = ManualPage.camera;
            VisionPage.cameraSetting.Capture = ManualPage.camera;
            MainProgram.Capture = ManualPage.camera;
            VisionPage.EditModel = ModelPage.EditModel;

            Binding stepsBinding = new Binding("Steps");
            stepsBinding.Source = AutoPage.TestModel;
            ManualPage.dgModelSteps.SetBinding(DataGrid.ItemsSourceProperty, stepsBinding);

            MainProgram.EditModel_OnSave += MainProgram_EditModel_OnSave;
            MainProgram.EditModel_OnLoaded += MainProgram_EditModel_OnLoaded;
        }

        private void Cleanning_Tick(object sender, EventArgs e)
        {
            //GC.Collect();
            lbDateTime.Content = DateTime.Now.ToString();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Cleanning.Start();
            btAutoPage.IsChecked = true;
            Task.Delay(1000).Wait();
            ManualPage.camera.START();
            MainProgram.START();

        }

        private void MainProgram_EditModel_OnLoaded(object sender, EventArgs e)
        {
            if (MainProgram.EditModel != null)
            {
                VisionPage.EditModel = MainProgram.EditModel;
                ModelPage.EditModel = MainProgram.EditModel;
            }
        }

        private void MainProgram_EditModel_OnSave(object sender, EventArgs e) 
        {
            SHOW_LOADING();
            var str = AutoPage.LoadModel(MainProgram.EditModel.Path);
            ManualPage.TestModel = AutoPage.TestModel;
            MainProgram.SetBoards();
            if (str != null)
            {
                MainProgram.EditModel = HVT.Utility.Extensions.ConvertFromJson<Model>(str);
                ModelPage.EditModel = MainProgram.EditModel;
                VisionPage.EditModel = MainProgram.EditModel;
            }
            HIDE_LOADING();
        }

        #region Form control
        //variable
        string LastPageSelected = "";

        // -Functions-

        // 현재 run 하고있는 operation 다 멈추고 앱 끄기
        private void btCloseWindow_Click(object sender, RoutedEventArgs e)
        {
         
            foreach (var item in stackpanelComunication.Children)
            {
                
                (item as HVT.Controls.SerialPortDisplay)?._shutDown.Cancel();
            }
            AutoPage._shutDown?.Cancel();

            App.Current.Shutdown();
            Close();
        }

        // WindowState 바꾸기 -> 현재 윈도우가 최대 화면이면 일반 크기 화면으로 바꾸고 
        // 최대 화면이 아니면 최대 화면으로 바꾸기
        private void btMaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        // 윈도우 화면 내리기 (minimized)
        private void btMinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // DockPanel을 왼쪽 마우스로 누르면 앱 윈도우 전체를 드래그 해서 움직일수있다
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        #endregion

        #region Menu Panel
        // Enter, leaver effect 
        // Page Select

        private void btSelectPage_Click(object sender, RoutedEventArgs e)
        {
            var bt = (sender as ToggleButton);
            btAutoPage.IsChecked = bt == btAutoPage;
            btManualPage.IsChecked = bt == btManualPage;
            btModelPage.IsChecked = bt == btModelPage;
            btVisionPage.IsChecked = bt == btVisionPage;
            btSettingPage.IsChecked = bt == btSettingPage;

            if (MainProgram.IsTestting)
            {
                btAutoPage.IsChecked = LastPageSelected == btAutoPage.Name;
                btManualPage.IsChecked = LastPageSelected == btManualPage.Name;
                btModelPage.IsChecked = LastPageSelected == btModelPage.Name;
                btSettingPage.IsChecked = LastPageSelected == btSettingPage.Name;

                return;
            }

            if (bt.Name == LastPageSelected)
            {
                bt.IsChecked = true;
                return;
            }

            AutoPageHolder.Visibility = Visibility.Hidden;
            ManualPageHolder.Visibility = Visibility.Hidden;
            ModelPageHolder.Visibility = Visibility.Hidden;
            VisionPageHolder.Visibility = Visibility.Hidden;
            SettingPageHolder.Visibility = Visibility.Hidden;

            MainProgram.EditModel.CameraSetting = VisionPage.cameraSetting.Capture?.cameraSetting;
            

            VisionPage.DisableLive();
            AutoPage.DisableLive();

            LastPageSelected = bt.Name;
            switch (bt.Name)
            {
                case "btAutoPage":
                    AutoPageHolder.Visibility = Visibility.Visible;
                    AutoPage.EnableLive();
                    ManualPage.camera.SetParammeter(MainProgram.TestModel.CameraSetting);
                    MainProgram.pageActive = Program.PageActive.AutoPage;
                    MainProgram.ResetTest();
                    break;
                case "btManualPage":
                    ManualPageHolder.Visibility = Visibility.Visible;
                    MainProgram.pageActive = Program.PageActive.ManualPage;
                    ManualPage.camera.SetParammeter(MainProgram.TestModel.CameraSetting);
                    break;
                case "btModelPage":
                    MainProgram.pageActive = Program.PageActive.ModelPage;
                    ModelPageHolder.Visibility = Visibility.Visible;
                    ModelPage.StepsGridData.Items.Refresh();
                    break;
                case "btVisionPage":
                    ManualPage.camera.SetParammeter(MainProgram.EditModel.CameraSetting);
                    MainProgram.pageActive = Program.PageActive.VistionPage;
                    VisionPageHolder.Visibility = Visibility.Visible;
                    VisionPage.EnableLive();
                    //VisionPage.VisionStepsGrid.Items.Refresh();
                    break;
                case "btSettingPage":
                    SettingPage.Program = MainProgram;
                    MainProgram.pageActive = Program.PageActive.ModelPage;
                    SettingPage.LoadDataSetting();
                    SettingPageHolder.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
        #endregion

        //Open Model


        private void btOpenModel_Click(object sender, RoutedEventArgs e)
        {
           
            SHOW_LOADING();
            var str = AutoPage.LoadModel();
            ManualPage.TestModel = AutoPage.TestModel;
            MainProgram.SetBoards();
            if (str != null)
            {
                MainProgram.EditModel = HVT.Utility.Extensions.ConvertFromJson<Model>(str);
                ModelPage.EditModel = MainProgram.EditModel;
                VisionPage.EditModel = MainProgram.EditModel;
            }
            HIDE_LOADING();
        }

        public System.Timers.Timer disableSpam = new System.Timers.Timer() { Interval = 3000};

        private void btCheckCommunications_Click(object sender, RoutedEventArgs e)
        {
            btCheckCommunications.IsEnabled = false;
            disableSpam.Elapsed += DisableSpam_Elapsed;
            disableSpam.Enabled = true;
            disableSpam.AutoReset = false;
            disableSpam.Stop();
            disableSpam.Start();
            MainProgram.CheckComnunication();
        }

        private void DisableSpam_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => btCheckCommunications.IsEnabled = true));
            disableSpam.Elapsed -= DisableSpam_Elapsed;
            disableSpam.Stop();
        }


        // pnLoading = 로딩 스크린
        // 로딩 스크린 보여주기
        public void SHOW_LOADING()
        {
            pnLoading.Visibility = Visibility.Visible;
        }

        public void HIDE_LOADING()
        {
            pnLoading.Visibility = Visibility.Collapsed;
        }

        private void btReportPage_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Directory.GetCurrentDirectory() + @"\VTM_Report.exe");
        }

        private void btCheckCommunications_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToString() + e.NewValue);
        }

        private void AutoPageHolder_Navigated(object sender, NavigationEventArgs e)
        {

        }
        private void window_SourceInitialized(object sender, EventArgs e)
        {
            //WindowSizing.WindowInitialized(this);
        }

    }

    public static class WindowSizing
    {
        const int MONITOR_DEFAULTTONEAREST = 0x00000002;

        #region DLLImports

        [DllImport("shell32", CallingConvention = CallingConvention.StdCall)]
        public static extern int SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [DllImport("user32", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);

        [DllImport("user32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        #endregion

        private static MINMAXINFO AdjustWorkingAreaForAutoHide(IntPtr monitorContainingApplication, MINMAXINFO mmi)
        {
            IntPtr hwnd = FindWindow("Shell_TrayWnd", null);
            if (hwnd == null) return mmi;
            IntPtr monitorWithTaskbarOnIt = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (!monitorContainingApplication.Equals(monitorWithTaskbarOnIt)) return mmi;
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = hwnd;
            SHAppBarMessage((int)ABMsg.ABM_GETTASKBARPOS, ref abd);
            int uEdge = GetEdge(abd.rc);
            bool autoHide = System.Convert.ToBoolean(SHAppBarMessage((int)ABMsg.ABM_GETSTATE, ref abd));

            if (!autoHide) return mmi;

            switch (uEdge)
            {
                case (int)ABEdge.ABE_LEFT:
                    mmi.ptMaxPosition.x += 2;
                    mmi.ptMaxTrackSize.x -= 2;
                    mmi.ptMaxSize.x -= 2;
                    break;
                case (int)ABEdge.ABE_RIGHT:
                    mmi.ptMaxSize.x -= 2;
                    mmi.ptMaxTrackSize.x -= 2;
                    break;
                case (int)ABEdge.ABE_TOP:
                    mmi.ptMaxPosition.y += 2;
                    mmi.ptMaxTrackSize.y -= 2;
                    mmi.ptMaxSize.y -= 2;
                    break;
                case (int)ABEdge.ABE_BOTTOM:
                    mmi.ptMaxSize.y -= 2;
                    mmi.ptMaxTrackSize.y -= 2;
                    break;
                default:
                    return mmi;
            }
            return mmi;
        }

        private static int GetEdge(RECT rc)
        {
            int uEdge = -1;
            if (rc.top == rc.left && rc.bottom > rc.right)
                uEdge = (int)ABEdge.ABE_LEFT;
            else if (rc.top == rc.left && rc.bottom < rc.right)
                uEdge = (int)ABEdge.ABE_TOP;
            else if (rc.top > rc.left)
                uEdge = (int)ABEdge.ABE_BOTTOM;
            else
                uEdge = (int)ABEdge.ABE_RIGHT;
            return uEdge;
        }

        public static void WindowInitialized(Window window)
        {
            IntPtr handle = (new System.Windows.Interop.WindowInteropHelper(window)).Handle;
            System.Windows.Interop.HwndSource.FromHwnd(handle).AddHook(new System.Windows.Interop.HwndSourceHook(WindowProc));
        }

        private static IntPtr WindowProc(System.IntPtr hwnd, int msg, System.IntPtr wParam, System.IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (IntPtr)0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            IntPtr monitorContainingApplication = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

            if (monitorContainingApplication != System.IntPtr.Zero)
            {
                MONITORINFO monitorInfo = new MONITORINFO();
                GetMonitorInfo(monitorContainingApplication, monitorInfo);
                RECT rcWorkArea = monitorInfo.rcWork;
                RECT rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                mmi.ptMaxTrackSize.x = mmi.ptMaxSize.x + 4;                                                 //maximum drag X size for the window
                mmi.ptMaxTrackSize.y = mmi.ptMaxSize.y + 4;                                                //maximum drag Y size for the window
                mmi.ptMinTrackSize.x = 800;                                                             //minimum drag X size for the window
                mmi.ptMinTrackSize.y = 600;                                                             //minimum drag Y size for the window
                mmi = AdjustWorkingAreaForAutoHide(monitorContainingApplication, mmi);                  //need to adjust sizing if taskbar is set to autohide
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        public enum ABEdge
        {
            ABE_LEFT = 0,
            ABE_TOP = 1,
            ABE_RIGHT = 2,
            ABE_BOTTOM = 3
        }

        public enum ABMsg
        {
            ABM_NEW = 0,
            ABM_REMOVE = 1,
            ABM_QUERYPOS = 2,
            ABM_SETPOS = 3,
            ABM_GETSTATE = 4,
            ABM_GETTASKBARPOS = 5,
            ABM_ACTIVATE = 6,
            ABM_GETAUTOHIDEBAR = 7,
            ABM_SETAUTOHIDEBAR = 8,
            ABM_WINDOWPOSCHANGED = 9,
            ABM_SETSTATE = 10
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public bool lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class MONITORINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
            public RECT rcMonitor = new RECT();
            public RECT rcWork = new RECT();
            public int dwFlags = 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}
