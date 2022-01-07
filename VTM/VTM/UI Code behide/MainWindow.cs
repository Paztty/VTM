
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
        Comunication_viewer comunication_Viewer;
        DispatcherTimer DateTimeUpdateTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };

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

            //Program.BarcodeReaderInit(BacodesTestingList, BacodesWaitingList, RX_RECT_COM14, CONNECTED_RECT_COM14);

            Program.Machine_Init();

            Program.CameraInit(
                cameraView,
                imgFNDviewA
                );

            Program.CreatMachineFolder();
            Program.ModelInit();
            Program.RootModel.contruction.ContructionChanged += Contruction_ContructionChanged;

            LoadAutopage();
            LoadModelPage();
            LoadManualPage();
            VisionPageInit();

            //atDisplayCanvas.Source = Program.CaptureCanvasLayout(drawingTable);


            //Thread.Sleep(5000);
            //splashScreen.Close();

            Program.ModelChangeEvent += Program_ModelChangeEvent;

            Program.RootModel.LoadFinish += Model_LoadFinish_AutoPage;
            Program.RootModel.LoadFinish += Model_LoadFinish_ManualPage;
            Program.RootModel.LoadFinish += Model_LoadFinish_ModelPage;

            Program.StepTestChange += Model_StepTestChangeAsync;
            Program.TestRunFinish += Model_TestRunFinish;
            Program.StateChange += Model_StateChange;
        }

        private void Contruction_ContructionChanged(object sender, EventArgs e)
        {
            Vision_ContructionlayoutUpdate(Program.RootModel.contruction);
            Autopanel_UpdateLayout(Program.RootModel.contruction);
            Manual_UpdateLayout(Program.RootModel.contruction);
            ModelPage_UpdateLayout(Program.RootModel.contruction);
        }

        private void Program_ModelChangeEvent(object sender, EventArgs e)
        {
            Program.RootModel.LoadFinish += Model_LoadFinish_AutoPage;
            Program.RootModel.LoadFinish += Model_LoadFinish_ManualPage;
            Program.RootModel.LoadFinish += Model_LoadFinish_ModelPage;
            Program.RootModel.LoadFinish += Model_LoadFinish_VisionPage;

            Program.RootModel.LoadFinish += Contruction_ContructionChanged;

            Program.StepTestChange += Model_StepTestChangeAsync;
            Program.TestRunFinish += Model_TestRunFinish;
            Program.StateChange += Model_StateChange;

            Program.RootModel.contruction.ContructionChanged += Contruction_ContructionChanged;
           
        }

        private void btCheckComunications_Click(object sender, RoutedEventArgs e)
        {
            Program.CloseDevices();

            Program.PrinterUiInit(TX_RECT_COM13, RX_RECT_COM13, CONNECTED_RECT_COM13);

            Program.MuxUIInit(MUX_Channels_Table1, MUX_Channels_Table2, wrapPanelMuxSelect, pnMux1, pnMux2,
                TX_RECT_COM3, RX_RECT_COM3, CONNECTED_RECT_COM3,
                TX_RECT_COM4, RX_RECT_COM4, CONNECTED_RECT_COM4
                );
            Program.RelayUIInit(pnRelaySelect, pnRelay1, pnRelay2, pnVisionRelays,
            TX_RECT_COM5, RX_RECT_COM5, CONNECTED_RECT_COM5);

            Program.SolenoidUIInit(pnSolenoid, pnVisionSolenoid,
            TX_RECT_COM6, RX_RECT_COM6, CONNECTED_RECT_COM6);

            Program.UUTPortUIInit(
                TX_RECT_COM8, RX_RECT_COM8, CONNECTED_RECT_COM8,
                TX_RECT_COM9, RX_RECT_COM9, CONNECTED_RECT_COM9,
                TX_RECT_COM10, RX_RECT_COM10, CONNECTED_RECT_COM10,
                TX_RECT_COM11, RX_RECT_COM11, CONNECTED_RECT_COM11
                );
            Program.DMM_UI_Init(MinVal_DMM1, MaxVal_DMM1, Arg_DMM1, Val_DMM1, MinVal_DMM2, MaxVal_DMM2, Arg_DMM2, Val_DMM2,
            RX_RECT_COM7, TX_RECT_COM7, CONNECTED_RECT_COM7, RX_RECT_COM15, TX_RECT_COM15, CONNECTED_RECT_COM15);
            
            //Program.DMM1.SearchCom().Wait();
            //Program.DMM2.SearchCom().Wait();
            //Program.PPS1.SearchCom().Wait();
            //Program.PPS2.SearchCom().Wait();
        }

        private void DateTimeUpdateTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                tbDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss");
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
            if (e.Key == Key.F5)
            {
                Runtest();
            }
            if (e.Key == Key.F1)
            {
                comunication_Viewer = new Comunication_viewer(Program);
                comunication_Viewer.Show();
            }
        }

        #endregion

        #region Page select
        private void BtOpenModel_Click(object sender, RoutedEventArgs e)
        {
            Program.LoadModel();
        }

        private void btPanelControl_Switch(object sender, RoutedEventArgs e)
        {
            Password PasswordPage;
            var boardSellected = Program.boardSelected;
            Program.boardSelected = Program.BoardSelected.All;
            Program.IsVisionWorking = false;

            btPageAuto.IsChecked = false;
            btPageManual.IsChecked = false;
            btPageModel.IsChecked = false;
            btPageVision.IsChecked = false;

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
                    if (Program.IsTestting) return;
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
                    if (Program.IsTestting) return;
                    PasswordPage = new Password(Users.Permissions.Tech);
                    if (PasswordPage.ShowDialog() == true)
                    {
                        AutoPanel.Visibility = Visibility.Hidden;
                        ModelPanel.Visibility = Visibility.Hidden;
                        ManualPanel.Visibility = Visibility.Hidden;
                        VisionPanel.Visibility = Visibility.Hidden;

                        Program.boardSelected = boardSellected;
                        Program.IsVisionWorking = true;
                        VisionPanel.Visibility = Visibility.Visible;
                        (sender as ToggleButton).IsChecked = true;
                    }

                    break;
                case "btPageModel":
                    if (Program.IsTestting) return;
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
                            VistionTestGrid.Focus();
                            (sender as ToggleButton).IsChecked = true;
                        }
                    }

                    break;
                default:
                    break;
            }


        }

        #endregion

    }

}