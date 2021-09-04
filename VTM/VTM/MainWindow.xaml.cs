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
//using System.Windows.Forms;

namespace VTM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Date time
        String CurrentDateTime { get; set; } = DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss  ");

        enum SoftwareModes
        {
            AutoMode = 0,
            ManualMode = 1,
            VisionMode = 2,
            ModelEditMode = 3,
            DataLogMode = 4,
            SettingMode = 5
        }
        private SoftwareModes SoftwareMode = SoftwareModes.AutoMode;

        // model data
        static Model model = new Model();
        // page
        AutoPage autoPage = new AutoPage(model);
        VisionPage visionPage = new VisionPage();
        ManualPage manualPage = new ManualPage();
        ModelPage modelPage = new ModelPage(model);

        private ObservableCollection<string[]> ModelProgram = new ObservableCollection<string[]>();

        public MainWindow()
        {
            InitializeComponent();
            Support.tbLog = autoPage.tbProgramLog;
            // Model.ViewModel = this.Steps;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Window Loaded");
            //comboboxColumn.DataSource = RetrieveAlternativeTitles();
            //comboboxColumn.ValueMember = ColumnName.TitleOfCourtesy.ToString();
            //comboboxColumn.DisplayMember = comboboxColumn.ValueMember;

            //Steps.DataContext = ModelProgram;

            //CameraInit();
            autoPage.Width = MainFrame.ActualWidth;
            autoPage.Height = MainFrame.ActualHeight;
            MainFrame.Content = autoPage;
            btPageAuto.IsChecked = true;
            DispatcherTimer dateTimeUpdateTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            dateTimeUpdateTimer.Tick += DateTimeUpdateTimer_Tick;
            dateTimeUpdateTimer.Start();

            //Password password = new Password();
            //password.ShowDialog();

            model.LoadFinish += Model_LoadFinish;

            CameraInit();
        }

        private void Model_LoadFinish(object sender, EventArgs e)
        {
            tbModelName.Text = model.Name;
        }

        private void DateTimeUpdateTimer_Tick(object sender, EventArgs e)
        {
            tbDateTime.Text = DateTime.Now.ToString("yyyy-MM-dd   HH:mm:ss");
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

        bool IsShowPassWorldPanel = false;

        private void btPanelControl_Switch(object sender, RoutedEventArgs e)
        {
            btPageAuto.IsChecked = false;
            //btPageLog.IsChecked = false;
            btPageManual.IsChecked = false;
            btPageModel.IsChecked = false;
            btPageVision.IsChecked = false;

            (sender as ToggleButton).IsChecked = true;

            switch ((sender as ToggleButton).Name)
            {
                case "btPageAuto":
                    autoPage.Width = MainFrame.ActualWidth;
                    autoPage.Height = MainFrame.ActualHeight;
                    MainFrame.Content = autoPage;
                    autoPage.RefreshData();
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
                    MainFrame.Content = manualPage;
                    break;
                case "btPageVision":
                    visionPage.Width = MainFrame.ActualWidth;
                    visionPage.Height = MainFrame.ActualHeight;
                    MainFrame.Content = visionPage;
                    break;
                case "btPageModel":
                    modelPage.Width = MainFrame.ActualWidth;
                    modelPage.Height = MainFrame.ActualHeight;
                    MainFrame.Content = modelPage;
                    break;
                default:
                    MainFrame.Content = null;
                    break;
            }
        }

        private void BtOpenModel_Click(object sender, RoutedEventArgs e)
        {
            model.Load();
        }

        #endregion

        #region Page select

        #endregion


        //private async void BtRunTest_Click(object sender, RoutedEventArgs e)
        //{
        //    tbElapsedTime.Text = "0";
        //    StartTestAnimation();
        //    var startTime = DateTime.Now;
        //    pbTestProgress.Value = 0;
        //    Random rand = new Random();
        //    Steps.SelectionMode = DataGridSelectionMode.Single;
        //    Steps.SelectionUnit = DataGridSelectionUnit.FullRow;
        //    for (int i = 0; i < Model.Steps.Count; i++)
        //    {
        //        pbTestProgress.Value = (int)(i / ((float)Model.Steps.Count - 1) * 100);
        //        AnimationTest();
        //        Steps.SelectedItem = Steps.Items[i];
        //        Steps.ScrollIntoView(Steps.Items[i]);
        //        DataGridRow dgrow = (DataGridRow)Steps.ItemContainerGenerator.ContainerFromItem(Steps.Items[i]);
        //        dgrow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

        //        if (Model.Steps[i].SKIP == "TRUE")
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            await Task.Delay(rand.Next(100));
        //        }

        //        tbElapsedTime.Text = (DateTime.Now.Subtract(startTime).TotalSeconds).ToString("F2");

        //    }
        //    StopTestAnimation();
        //}

        //#region Testting Animation

        //Double testAnimationPanelOrigin;
        //private void StartTestAnimation()
        //{
        //    pnStatusFAIL.Visibility = Visibility.Collapsed;
        //    pnStatusGood.Visibility = Visibility.Collapsed;
        //    pnStatusReady.Visibility = Visibility.Collapsed;
        //    pnStatusTestting.Visibility = Visibility.Visible;
        //    testAnimationPanelOrigin = pnStatusTestting.Margin.Top;
        //}

        //double animationOffset = 0;
        //bool isBottomAnimation = false;
        //private void AnimationTest()
        //{
        //    pnStatusTestting.Margin = new Thickness(3, testAnimationPanelOrigin + animationOffset, 3, testAnimationPanelOrigin + pnStatusTestting.ActualHeight - animationOffset);
        //    Task.Delay(1);
        //    if (isBottomAnimation)
        //    {
        //        animationOffset--;
        //        if (animationOffset < -5)
        //        {
        //            isBottomAnimation = false;
        //        }
        //    }
        //    else
        //    {
        //        animationOffset++;
        //        if (animationOffset > 5)
        //        {
        //            isBottomAnimation = true;
        //        }
        //    }

        //}
        //private void StopTestAnimation()
        //{
        //    pnStatusFAIL.Visibility = Visibility.Collapsed;
        //    pnStatusGood.Visibility = Visibility.Collapsed;
        //    pnStatusReady.Visibility = Visibility.Collapsed;
        //    pnStatusTestting.Visibility = Visibility.Visible;
        //    pnStatusTestting.Margin = new Thickness(3);
        //}

        //#endregion


        #region Camera
        // Camera
        VideoCaptureDevice LocalWebCam;
        public FilterInfoCollection LoadWebCamsCollection;
        public bool IsLastPreviewState = true;
        public bool IsPreviewing = false;
        public DispatcherTimer CameraCheckConnect = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
        void CameraInit()
        {
            CameraCheckConnect.Tick -= CameraCheckConnect_Tick;
            LoadWebCamsCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (LoadWebCamsCollection.Count >= 1)
            {
                CameraCheckConnect.Stop();

                LocalWebCam = new VideoCaptureDevice(LoadWebCamsCollection[0].MonikerString);
                LocalWebCam.NewFrame -= new NewFrameEventHandler(Cam_NewFrame);
                LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);

                LocalWebCam.VideoResolution = LocalWebCam.VideoCapabilities[15];
                //for (int i = 0; i < LocalWebCam.VideoCapabilities.Length; i++)
                //{
                //    Support.WriteLine("Resolution Number " + i.ToString() + LocalWebCam.VideoCapabilities[i].FrameSize.ToString());
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

        void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                var bitmap = (Bitmap)eventArgs.Frame.Clone();
                var filter = new Mirror(false, true);
                filter.ApplyInPlace(bitmap);
                System.Drawing.Image img = bitmap;

                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                bi.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    autoPage.cameraView.Source = null;
                    autoPage.cameraView.Source = bi;
                }));

                if (!IsLastPreviewState)
                {
                    Support.WriteLine("Camera started.");
                    CameraCheckConnect.Start();
                    IsLastPreviewState = IsPreviewing;
                }
            }
            catch (Exception ex)
            {
                Support.WriteLine("Camera get frame error " + ex.GetHashCode().ToString());
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

        //private void btPanelControl_Switch(object sender, RoutedEventArgs e)
        //{
        //    //Support.WriteLine((sender as System.Windows.Controls.Button).Name);
        //    PanelAuto.Visibility = (sender as System.Windows.Controls.Button) == btPageAuto ? Visibility.Visible : Visibility.Collapsed;
        //    PanelManual.Visibility = (sender as System.Windows.Controls.Button) == btPageManual ? Visibility.Visible : Visibility.Collapsed;
        //    PanelModelEdit.Visibility = (sender as System.Windows.Controls.Button) == btPageModel ? Visibility.Visible : Visibility.Collapsed;
        //    PanelVision.Visibility = (sender as System.Windows.Controls.Button) == btPageVision ? Visibility.Visible : Visibility.Collapsed; PanelAuto.Visibility = (sender as System.Windows.Controls.Button) == btPageAuto ? Visibility.Visible : Visibility.Collapsed;
        //    PanelDataLog.Visibility = (sender as System.Windows.Controls.Button) == btPageLog ? Visibility.Visible : Visibility.Collapsed; PanelAuto.Visibility = (sender as System.Windows.Controls.Button) == btPageAuto ? Visibility.Visible : Visibility.Collapsed;
        //    PanelSetup.Visibility = (sender as System.Windows.Controls.Button) == btPageSetup ? Visibility.Visible : Visibility.Collapsed;

        //    //switch ((sender as System.Windows.Controls.Button).Name)
        //    //{
        //    //    case "btPageAuto":
        //    //        PanelAuto.Visibility = Visibility.Visible;
        //    //        PanelManual.Visibility = Visibility.Collapsed;
        //    //        break;
        //    //    default:
        //    //        break;
        //    //}
        //}

        //private void BtOpenModel_Click(object sender, RoutedEventArgs e)
        //{
        //    OpenFileDialog openFile = new OpenFileDialog();
        //    openFile.DefaultExt = "model";
        //    openFile.FileOk += LoadModelFile;
        //    openFile.ShowDialog();
        //}

        //private async void LoadModelFile(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    var fileInfor = new FileInfo((sender as OpenFileDialog).FileName);
        //    tbModelName.Text = System.IO.Path.GetFileNameWithoutExtension((sender as OpenFileDialog).FileName);
        //    Support.WriteLine("Load model : " + tbModelName.Text);
        //    string[] lines = System.IO.File.ReadAllLines((sender as OpenFileDialog).FileName);
        //    string StepStart = "";

        //    foreach (string line in lines)
        //    {
        //        if (line.Contains('['))
        //        {
        //            StepStart = line;
        //        }

        //        if (StepStart == "[STEP]")
        //        {
        //            if (line == StepStart) continue;
        //            ModelProgram.Add(line.Split(','));
        //            Support.AppentLine(line);
        //        }

        //    }
        //    Model = new Model(ModelProgram, Steps);
        //    //Steps.DataContext = ModelProgram;
        //}

        //private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    Support.WriteLine(e.Key.ToString());
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void Steps_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        //{

        //}
    }
}