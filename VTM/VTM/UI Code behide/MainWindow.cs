using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Drawing;
using System.Collections.ObjectModel;
using AForge.Imaging.Filters;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using HVT.VTM.Program;
using HVT.StandantLocalUsers;
using HVT.VTM.Core;
using HVT.Utility;
using System.Threading.Tasks;

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

        private ObservableCollection<string[]> ModelProgram = new ObservableCollection<string[]>();

        List<System.Windows.Controls.Label> PCB_LABEL = new List<System.Windows.Controls.Label>();

        DispatcherTimer DateTimeUpdateTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };


        public MainWindow()
        {
            splashScreen.Show();
            InitializeComponent();
            Debug.LogBox = tbProgramLog;
            Debug.dispatcher = this.Dispatcher;
            Console.WriteLine("Window Loaded");
            Debug.Write("Program start.", Debug.ContentType.Notify);
            btPageAuto.IsChecked = true;

            Thread.Sleep(2000);

            DateTimeUpdateTimer.Tick += DateTimeUpdateTimer_Tick;
            DateTimeUpdateTimer.Start();

            
            Program.CameraInit(cameraView, imgFNDviewA);
            Program.ModelInit();

            LoadModelPage();
            LoadManualPage();
            VisionPageInit();
            splashScreen.Close();
        }



        #region Model Action

        //Event 
        private void Model_LoadFinish_AutoPage(object sender, EventArgs e)
        {
            tbModelName.Text = Program.RootModel.Name;
            TestStepsGrid.ItemsSource = null;
            TestStepsGrid.ItemsSource = new ObservableCollection<Model.Step>(Program.RootModel.Steps);
            ModelName.Text = Program.RootModel.Name;
            tbModelNamePath.Text = Program.RootModel.Path;
            TestStepsGridData.ItemsSource = null;
            TestStepsGridData.ItemsSource = Program.RootModel.Steps;
            Error_Positions_Table.ItemsSource = Program.RootModel.ErrorPositions;
        }

        private void Model_StateChange(object sender, EventArgs e)
        {
            switch (Program.RootModel.TestState)
            {
                case Model.RunTestState.WAIT:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultGood.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestResultGood);
                    }), DispatcherPriority.Send);
                    break;
                case Model.RunTestState.TESTTING:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultGood.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestResultGood);
                    }), DispatcherPriority.Send);
                    break;
                case Model.RunTestState.Pause:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultGood.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestResultGood);
                    }), DispatcherPriority.Send);
                    break;
                case Model.RunTestState.STOP:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultGood.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestResultGood);
                    }), DispatcherPriority.Send);
                    break;
                case Model.RunTestState.GOOD:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultGood.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestResultGood);
                    }), DispatcherPriority.Send);
                    break;
                case Model.RunTestState.FAIL:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultGood.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestResultGood);
                    }), DispatcherPriority.Send);
                    break;
                case Model.RunTestState.BUSY:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultGood.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestResultGood);
                    }), DispatcherPriority.Send);
                    break;
                case Model.RunTestState.READY:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultGood.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestResultGood);
                    }), DispatcherPriority.Send);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Form control
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Program.RootModel.LoadFinish += Model_LoadFinish_AutoPage;
            Program.RootModel.LoadFinish += Model_LoadFinish_ManualPage;
            Program.RootModel.LoadFinish += Model_LoadFinish_ModelPage;

            Program.RootModel.StepTestChange += Model_StepTestChangeAsync;
            Program.RootModel.TestRunFinish += Model_TestRunFinish;
            Program.RootModel.StateChange += Model_StateChange;
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


                        VisionPanel.Visibility = Visibility.Visible;
                        (sender as ToggleButton).IsChecked = true;
                    }

                    break;
                case "btPageModel":
                    PasswordPage = new Password(Users.Permissions.Tech);
                    if (PasswordPage.ShowDialog() == true)
                    {
                        AutoPanel.Visibility = Visibility.Hidden;
                        ModelPanel.Visibility = Visibility.Hidden;
                        ManualPanel.Visibility = Visibility.Hidden;
                        VisionPanel.Visibility = Visibility.Hidden;


                        ModelPanel.Visibility = Visibility.Visible;
                        (sender as ToggleButton).IsChecked = true;
                    }

                    break;
                default:
                    break;
            }
        }

        #endregion

    }

}