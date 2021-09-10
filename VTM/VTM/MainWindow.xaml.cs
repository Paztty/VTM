using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Drawing;
using System.Collections.ObjectModel;
using AForge.Imaging.Filters;
using System.Drawing.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using Windows.Devices;
using System.Runtime.InteropServices;
//using System.Windows.Forms;

namespace VTM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // model data
        static Model model = new Model();
        // page
        AutoPage autoPage = new AutoPage(model);
        VisionPage visionPage = new VisionPage();
        ManualPage manualPage = new ManualPage();
        ModelPage modelPage = new ModelPage(model);
        Splash splashScreen = new Splash();
        private ObservableCollection<string[]> ModelProgram = new ObservableCollection<string[]>();
        
        public MainWindow()
        {
            splashScreen.Show();
            InitializeComponent();
            Support.tbLog = autoPage.tbProgramLog;
            // Model.ViewModel = this.Steps;
            Console.WriteLine("Window Loaded");

            //CameraInit();
            autoPage.Width = MainFrame.ActualWidth;
            autoPage.Height = MainFrame.ActualHeight;
            MainFrame.Content = autoPage;
            btPageAuto.IsChecked = true;


            //Password password = new Password();
            //password.ShowDialog();

            model.LoadFinish += Model_LoadFinish;
            model.StepTestChange += Model_StepTestChangeAsync;
            model.TestRunFinish += Model_TestRunFinish1;

            // start update datetime
            Thread UpdateDateTime = new Thread(DateTimeUpdate);
            UpdateDateTime.IsBackground = true;
            UpdateDateTime.Start();
            Thread.Sleep(2000);
            // start camera
            CameraInit();
            splashScreen.Close();

        }

        private void Model_TestRunFinish1(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                lbTestResultTesting.Visibility = Visibility.Hidden;
                lbTestResultGood.Visibility = Visibility.Visible;
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void DateTimeUpdate()
        {
            while (true)
            {
                try
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        tbDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd   HH:mm:ss");
                    }), DispatcherPriority.ApplicationIdle);
                }
                catch (Exception)
                {
                }

                Thread.Sleep(1000);
            }
        }

        private void Model_LoadFinish(object sender, EventArgs e)
        {
            tbModelName.Text = model.Name;
            TestStepsGrid.ItemsSource = null;
            TestStepsGrid.ItemsSource = new ObservableCollection<Model.Step>(model.Steps);
        }

        #region Form control

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


        private void BtOpenModel_Click(object sender, RoutedEventArgs e)
        {
            model.Load();
        }

        #endregion

        #region Page select


        private void btPanelControl_Switch(object sender, RoutedEventArgs e)
        {
            btPageAuto.IsChecked = false;
            //btPageLog.IsChecked = false;
            btPageManual.IsChecked = false;
            btPageModel.IsChecked = false;
            btPageVision.IsChecked = false;
            AutoPanel.Visibility = Visibility.Hidden;

            (sender as ToggleButton).IsChecked = true;

            switch ((sender as ToggleButton).Name)
            {
                case "btPageAuto":
                    AutoPanel.Visibility = Visibility.Visible;
                    break;
                case "btPageManual":
                    //IsShowPassWorldPanel = !IsShowPassWorldPanel;
                    //if (IsShowPassWorldPanel)
                    //{
                    //    pnPassworld.Visibility = Visibility.Visible;
                    //}
                    //else
                    //{
                    //    pnPassworld.Visibility = Visibility.Collapsed;
                    //}
                    manualPage.Width = MainFrame.ActualWidth;
                    manualPage.Height = MainFrame.ActualHeight;
                    MainFrame.Navigate(manualPage);
                    //MainFrame.Content = manualPage;
                    break;
                case "btPageVision":
                    visionPage.Width = MainFrame.ActualWidth;
                    visionPage.Height = MainFrame.ActualHeight;
                    //MainFrame.Content = visionPage;
                    MainFrame.Navigate(VisionPanel);
                    break;
                case "btPageModel":
                    modelPage.Width = MainFrame.ActualWidth;
                    modelPage.Height = MainFrame.ActualHeight;
                    //MainFrame.Content = modelPage;
                    MainFrame.Navigate(modelPage);
                    break;
                default:
                    MainFrame.Content = null;
                    break;
            }
        }

        #endregion

        #region AutoPage

        DateTime EscapTime;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EscapTime = DateTime.Now;
            lbTestResultTesting.Visibility = Visibility.Visible;
            lbTestResultGood.Visibility = Visibility.Hidden;
            Runtest();
        }

        public void Runtest()
        {
            foreach (var item in model.Steps)
            {
                item.ValueGet1 = "";
                item.ValueGet2 = "";
                item.ValueGet3 = "";
                item.ValueGet4 = "";
                item.ValueGet5 = "";
                item.ValueGet6 = "";
                item.ValueGet7 = "";
                item.ValueGet8 = "";
            }
            TestStepsGrid.Items.Refresh();
            if (!model.IsTestting)
            {
                model.StepTesting = 0;
                model.IsTestting = false;
                Thread RunTest = new Thread(model.runTest, BackgroundProperty.GlobalIndex);
                RunTest.IsBackground = true;
                RunTest.Start();
            }
        }

        private void Model_StepTestChangeAsync(object sender, EventArgs e)
        {
            if (model.StepTesting >= 0)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    TestStepsGrid.SelectedItem = model.Steps[(int)sender];
                    lbEscapTime.Content = DateTime.Now.Subtract(EscapTime).TotalSeconds.ToString("F1") + "s";
                    TestStepsGrid.ScrollIntoView(TestStepsGrid.SelectedItem);
                }), DispatcherPriority.Send);
            }
        }
        private void Model_TestRunFinish(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                lbTestResultTesting.Visibility = Visibility.Hidden;
                lbTestResultGood.Visibility = Visibility.Visible;
            }));
        }
        private void TestStepsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (model.IsTestting)
            {
                ProgressTestSlider.Value = (int)((double)model.StepTesting / (model.Steps.Count - 2) * 100);
            }
        }
        #endregion


        #region Camera
        // Camera
        VideoCaptureDevice LocalWebCam;
        public FilterInfoCollection LoadWebCamsCollection;
        public bool IsLastPreviewState = true;
        public bool IsPreviewing = false;
        public DispatcherTimer CameraCheckConnect = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
        void CameraInit()
        {
            //CameraCheckConnect.Tick -= CameraCheckConnect_Tick;
            LoadWebCamsCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (LoadWebCamsCollection.Count >= 1)
            {
                CameraCheckConnect.Stop();

                LocalWebCam = new VideoCaptureDevice(LoadWebCamsCollection[0].MonikerString);
                //LocalWebCam.NewFrame -= new NewFrameEventHandler(Cam_NewFrame);
                LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);

                LocalWebCam.VideoResolution = LocalWebCam.VideoCapabilities[15];
                //for (int i = 0; i < LocalWebCam.VideoCapabilities.Length; i++)
                //{
                Console.WriteLine("Resolution Number 15:" + LocalWebCam.VideoCapabilities[15].FrameSize.ToString());
                //}
                LocalWebCam.Start();

                if (!IsPreviewing)
                {
                    Support.WriteLine("Camera initting... (Đang khởi tạo lại camera...)");
                    LocalWebCam.DisplayPropertyPage(IntPtr.Zero);
                }
                IsPreviewing = true;
            }
            else
            {
                if (IsLastPreviewState != IsPreviewing)
                {
                    Support.WriteLine("No camera fount (Không tìm thấy phương tiện media).");
                    IsLastPreviewState = IsPreviewing;
                }

                CameraCheckConnect.Start();
            }
            //CameraCheckConnect.Tick += CameraCheckConnect_Tick;

        }

        private void CameraCheckConnect_Tick(object sender, EventArgs e)
        {
            LoadWebCamsCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            CameraCheckConnect.Stop();
            if (LoadWebCamsCollection.Count == 0)
            {
                IsPreviewing = false;
            }
            if (!IsPreviewing)
            {
                CameraInit();
            }
            CameraCheckConnect.Start();
        }

        void CameraDisponse()
        {
            if (IsPreviewing)
            {
                LocalWebCam.Stop();
                //LocalWebCam.SignalToStop();
            }
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(Bitmap image)
        {
            using (System.Drawing.Bitmap source = image)
            {
                IntPtr ptr = source.GetHbitmap();

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr);
                return bs;
            }
        }

     void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            var bitmap = (Bitmap)eventArgs.Frame.Clone();
            try
            {
                var filter = new Mirror(false, true);
                filter.ApplyInPlace(bitmap);
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                     cameraView.Source = ToBitmapSource(bitmap);

                }), DispatcherPriority.ApplicationIdle);

                if (!IsLastPreviewState)
                {
                    Support.WriteLine("Camera started.");
                    CameraCheckConnect.Start();
                    IsLastPreviewState = IsPreviewing;
                }
            }
            catch (Exception ex)
            {
                GC.Collect();
                Support.WriteLine("Camera get frame error " + ex.GetHashCode().ToString());
            }
            finally
            {
                bitmap.Dispose();
            }
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CameraDisponse();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(e.Key);
            if (e.Key == Key.F5)
            {
                autoPage.Runtest();
            }
        }

    }
}