
using HVT.StandantLocalUsers;
using HVT.Utility;
using HVT.VTM.Program;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace VTM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Program Program = new Program();
        // page
        Splash splashScreen = new Splash();

        List<System.Windows.Controls.Label> PCB_LABEL = new List<System.Windows.Controls.Label>();

        DispatcherTimer DateTimeUpdateTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };

        Rect RectGeo = new Rect()
        { 
            X = 0,
            Y = 0,
            Width = 200,
            Height = 200,
        };

        public MainWindow()
        {
            splashScreen.Show();
            InitializeComponent();

            AutoPanel.Visibility = Visibility.Visible;
            ManualPanel.Visibility = Visibility.Hidden;
            ModelPanel.Visibility = Visibility.Hidden;
            VisionPanel.Visibility = Visibility.Hidden;

        }

        protected override void OnContentRendered(EventArgs e)
        {
            splashScreen.Close();
            this.BringIntoView();
        }

        #region Form control
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Debug.LogBox = tbProgramLog;
            Debug.dispatcher = this.Dispatcher;
            Console.WriteLine("Window Loaded");
            Debug.Write("Program start.", Debug.ContentType.Notify);
            btPageAuto.IsChecked = true;
            DateTimeUpdateTimer.Tick += DateTimeUpdateTimer_Tick;
            DateTimeUpdateTimer.Start();

            Program.CameraInit(
                cameraView,
                imgFNDviewA
                );
            Program.CreatMachineFolder();
            Program.ModelInit();
            Program.BarcodeReaderInit(BacodesTestingList, RX_RECT_COM14, CONNECTED_RECT_COM14);

            LoadAutopage();
            LoadModelPage();
            LoadManualPage();
            VisionPageInit();

            //atDisplayCanvas.Source = Program.CaptureCanvasLayout(drawingTable);
            

            //Thread.Sleep(5000);
            //splashScreen.Close();

            Program.RootModel.LoadFinish += Model_LoadFinish_AutoPage;
            Program.RootModel.LoadFinish += Model_LoadFinish_ManualPage;
            Program.RootModel.LoadFinish += Model_LoadFinish_ModelPage;

            Program.StepTestChange += Model_StepTestChangeAsync;
            Program.TestRunFinish += Model_TestRunFinish;
            Program.StateChange += Model_StateChange;
        }

        private void btCheckComunications_Click(object sender, RoutedEventArgs e)
        {
            Program.ConnectCheck();
        }

        private void DateTimeUpdateTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                tbDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd   HH:mm:ss");
            }), DispatcherPriority.Send);
        }

        private void btCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Program.CameraDisponse();
            Console.WriteLine("Close button click");
            Close();
        }
        private void btMaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
        private void btMinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Program.cameraStreaming != null)
            {
                Program.cameraStreaming.Dispose();
            } 
            Environment.Exit(0);
            Program.PPS1.canUpdate = false;
            Program.PPS2.canUpdate = false;
            try
            {
                while (Program.PPS1.UpdateValueTask.Status == TaskStatus.Running
                            || Program.PPS2.UpdateValueTask.Status == TaskStatus.Running) ;
                Program.PPS1.UpdateValueTask.Dispose();
                Program.PPS2.UpdateValueTask.Dispose();
            }
            catch (Exception)
            {
            }

        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Console.WriteLine(e.Key);
            if (e.Key == Key.F5)
            {
                Runtest();
            }
        }


        #endregion

        #region Page select
        private void BtOpenModel_Click(object sender, RoutedEventArgs e)
        {
            Program.RootModel.Load();
        }

        private void btPanelControl_Switch(object sender, RoutedEventArgs e)
        {
            Password PasswordPage;
            var boardSellected = Program.boardSelected;
            Program.boardSelected = Program.BoardSelected.All;
            switch ((sender as ToggleButton).Name)
            {
                case "btPageAuto":

                    AutoPanel.Visibility = Visibility.Visible;
                    ModelPanel.Visibility = Visibility.Hidden;
                    ManualPanel.Visibility = Visibility.Hidden;
                    VisionPanel.Visibility = Visibility.Hidden;
                    //atDisplayCanvas.Source = Program.CaptureCanvasLayout(drawingTable);
                    (sender as ToggleButton).IsChecked = true;
                    break;
                case "btPageManual":
                    PasswordPage = new Password(Users.Permissions.Tech);
                    if (PasswordPage.ShowDialog() == true)
                    {
                        AutoPanel.Visibility = Visibility.Hidden;
                        ModelPanel.Visibility = Visibility.Hidden;
                        ManualPanel.Visibility = Visibility.Hidden;
                        VisionPanel.Visibility = Visibility.Hidden;

                        ManualPanel.Visibility = Visibility.Visible;
                        (sender as ToggleButton).IsChecked = true;
                    }

                    break;
                case "btPageVision":
                    PasswordPage = new Password(Users.Permissions.Tech);
                    if (PasswordPage.ShowDialog() == true)
                    {
                        AutoPanel.Visibility = Visibility.Hidden;
                        ModelPanel.Visibility = Visibility.Hidden;
                        ManualPanel.Visibility = Visibility.Hidden;
                        VisionPanel.Visibility = Visibility.Hidden;

                        Program.boardSelected = boardSellected;

                        VisionPanel.Visibility = Visibility.Visible;
                        (sender as ToggleButton).IsChecked = true;
                    }

                    break;
                case "btPageModel":
                    PasswordPage = new Password(Users.Permissions.Tech);
                    if (!(bool)btPageAuto.IsChecked)
                    {
                        if (PasswordPage.ShowDialog() == true)
                        {
                            AutoPanel.Visibility = Visibility.Hidden;
                            ModelPanel.Visibility = Visibility.Hidden;
                            ManualPanel.Visibility = Visibility.Hidden;
                            VisionPanel.Visibility = Visibility.Hidden;

                            ModelPanel.Visibility = Visibility.Visible;
                            (sender as ToggleButton).IsChecked = true;
                        }
                    }

                    break;
                default:
                    break;
            }

            btPageAuto.IsChecked = false;
            btPageManual.IsChecked = false;
            btPageModel.IsChecked = false;
            btPageVision.IsChecked = false;
        }

        #endregion

    }

}