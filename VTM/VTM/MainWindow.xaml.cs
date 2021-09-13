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
using System.Windows.Forms;
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

        List<System.Windows.Controls.Label> PCB_LABEL = new List<System.Windows.Controls.Label>();

        public MainWindow()
        {
            splashScreen.Show();
            InitializeComponent();
            Support.tbLog = autoPage.tbProgramLog;
            // Model.ViewModel = this.Steps;
            Console.WriteLine("Window Loaded");

            //CameraInit();
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
            PCB_LABEL.Add(PCB1);
            PCB_LABEL.Add(PCB2);
            PCB_LABEL.Add(PCB3);
            PCB_LABEL.Add(PCB4);
            PCB_LABEL.Add(PCB5);
            PCB_LABEL.Add(PCB6);
            PCB_LABEL.Add(PCB7);
            PCB_LABEL.Add(PCB8);
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
            ModelName.Text = model.Name;
            tbModelNamePath.Text = model.Path;
            TestStepsGridData.ItemsSource = null;
            TestStepsGridData.ItemsSource = model.Steps;
            Error_Positions_Table.ItemsSource = model.ErrorPositions;
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
            ModelPanel.Visibility = Visibility.Hidden;

            (sender as ToggleButton).IsChecked = true;
            LocalWebCam.NewFrame -= new NewFrameEventHandler(Cam_NewFrame);

            switch ((sender as ToggleButton).Name)
            {
                case "btPageAuto":
                    AutoPanel.Visibility = Visibility.Visible;
                    LocalWebCam.NewFrame -= new NewFrameEventHandler(Cam_NewFrame);
                    LocalWebCam.NewFrame += new NewFrameEventHandler(Cam_NewFrame);
                    break;
                case "btPageManual":
                    break;
                case "btPageVision":
                    break;
                case "btPageModel":
                    ModelPanel.Visibility = Visibility.Visible;
                    break;
                default:
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
                LocalWebCam.NewFrame -= new NewFrameEventHandler(Cam_NewFrame);
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
                //var source = ToBitmapSource(bitmap);
                DispatcherOperation operation = this.Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    cameraView.Source = ToBitmapSource(bitmap);
                }), DispatcherPriority.ApplicationIdle);

                operation.Wait(TimeSpan.FromMilliseconds(5));

                if (operation.Status != DispatcherOperationStatus.Completed)
                {
                    operation.Abort();
                }

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
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CameraDisponse();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            Console.WriteLine(e.Key);
            if (e.Key == Key.F5)
            {
                autoPage.Runtest();
            }
        }




        // model page
        //PCB align tab
        private void nUD_PCBcount_ValueChanged(object sender, EventArgs e)
        {
            int Count = (int)((sender as System.Windows.Forms.NumericUpDown).Value);
            if ((sender as System.Windows.Forms.NumericUpDown).Name == "nUD_PCBcount")
            {
                model.contruction.PCB_Count = Count;
            }
            if ((sender as System.Windows.Forms.NumericUpDown).Name == "nUD_X_axis_count")
            {
                model.contruction.PCB_X_axis_Count = Count;
            }
            Align_PCB();
        }

        private void Align_PCB()
        {
            if (PCBlayout != null)
            {

                for (int i = 0; i < 8; i++)
                {
                    PCBlayout.ColumnDefinitions[i].Width = new GridLength(0, GridUnitType.Star);
                    PCBlayout.RowDefinitions[i].Height = new GridLength(0, GridUnitType.Star);
                }

                int colunm = 0;
                int row = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (i < model.contruction.PCB_Count)
                    {
                        PCB_LABEL[i].Visibility = Visibility.Visible;
                        switch (model.contruction.ArrayPosition)
                        {
                            case Model.Contruction.ArrayPositions.HorizontalTopLeft:
                                colunm = i % model.contruction.PCB_X_axis_Count;
                                row = i / model.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.HorizontalTopRight:
                                colunm = model.contruction.PCB_X_axis_Count - i % model.contruction.PCB_X_axis_Count - 1;
                                row = i / model.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.HorizontalBottomLeft:
                                colunm = i % model.contruction.PCB_X_axis_Count;
                                row = (model.contruction.PCB_Count / model.contruction.PCB_X_axis_Count) - i / model.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.HorizontalBottomRight:
                                colunm = model.contruction.PCB_X_axis_Count - i % model.contruction.PCB_X_axis_Count - 1;
                                row = (model.contruction.PCB_Count / model.contruction.PCB_X_axis_Count) - i / model.contruction.PCB_X_axis_Count;
                                break;

                            case Model.Contruction.ArrayPositions.VerticalTopLeft:
                                colunm = i / model.contruction.PCB_X_axis_Count;
                                row = i % model.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.VerticalTopRight:
                                colunm = (model.contruction.PCB_Count / model.contruction.PCB_X_axis_Count) - i / model.contruction.PCB_X_axis_Count;
                                row = i % model.contruction.PCB_X_axis_Count;
                                break;
                            case Model.Contruction.ArrayPositions.VerticalBottomLeft:
                                colunm = i / model.contruction.PCB_X_axis_Count;
                                row = model.contruction.PCB_X_axis_Count - i % model.contruction.PCB_X_axis_Count - 1;
                                break;
                            case Model.Contruction.ArrayPositions.VerticalBottomRight:
                                colunm = (model.contruction.PCB_Count / model.contruction.PCB_X_axis_Count) - i / model.contruction.PCB_X_axis_Count;
                                row = model.contruction.PCB_X_axis_Count - i % model.contruction.PCB_X_axis_Count - 1;
                                break;
                            default:
                                break;
                        }

                        PCBlayout.ColumnDefinitions[colunm].Width = new GridLength(1, GridUnitType.Star);
                        PCBlayout.RowDefinitions[row].Height = new GridLength(1, GridUnitType.Star);

                        Grid.SetColumn(PCB_LABEL[i], colunm);
                        Grid.SetRow(PCB_LABEL[i], row);
                    }
                    else
                    {
                        PCB_LABEL[i].Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Align_PCB();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as System.Windows.Controls.ComboBox).SelectedIndex > -1)
            {
                model.contruction.ArrayPosition = (Model.Contruction.ArrayPositions)(sender as System.Windows.Controls.ComboBox).SelectedIndex;
                Align_PCB();
            }
        }


        //Error position Tab

        bool IsAddingErrorPosition = false;
        Model.ErrorPosition newErrorPosition = new Model.ErrorPosition();

        private void btPCB_Error_Position_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Error_Positions_Table.SelectedIndex >= 0 && Error_Positions_Table.SelectedIndex < model.ErrorPositions.Count)
            {
                int index = Error_Positions_Table.SelectedIndex;

                model.ErrorPositions.Remove((Model.ErrorPosition)Error_Positions_Table.SelectedItem);
                Canvas_PCB_Error_Mark.Children.Remove(((Model.ErrorPosition)Error_Positions_Table.SelectedItem).label);
                Canvas_PCB_Error_Mark.Children.Remove(((Model.ErrorPosition)Error_Positions_Table.SelectedItem).rect);

                for (int i = 0; i < model.ErrorPositions.Count; i++)
                {
                    model.ErrorPositions[i].No = i + 1;
                }
                Error_Positions_Table.Items.Refresh();
                Error_Positions_Table.SelectedIndex = index < model.ErrorPositions.Count ? index : index > 0 ? index - 1 : 0;
            }
        }

        private void Error_Positions_Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Select change");
        }

        private void imgPCB_Error_Position_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsAddingErrorPosition)
            {
                model.ErrorPositions.Add(new Model.ErrorPosition()
                {
                    No = model.ErrorPositions.Count + 1,
                    Y = e.GetPosition(Canvas_PCB_Error_Mark).Y,
                    X = e.GetPosition(Canvas_PCB_Error_Mark).X,
                    lbTop = e.GetPosition(Canvas_PCB_Error_Mark).Y,
                    lbLeft = e.GetPosition(Canvas_PCB_Error_Mark).X,
                    Position = e.GetPosition(imgPCB_Error_Position).X.ToString("F2") + " ~ " + e.GetPosition(imgPCB_Error_Position).Y.ToString("F2"),
                    Width = 0,
                    Height = 0
                });

                Canvas.SetTop(model.ErrorPositions[model.ErrorPositions.Count - 1].label, model.ErrorPositions[model.ErrorPositions.Count - 1].Y);
                Canvas.SetLeft(model.ErrorPositions[model.ErrorPositions.Count - 1].label, model.ErrorPositions[model.ErrorPositions.Count - 1].X);

                Canvas.SetTop(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].Y);
                Canvas.SetLeft(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].X);

                Canvas_PCB_Error_Mark.Children.Add(model.ErrorPositions[model.ErrorPositions.Count - 1].rect);
                Canvas_PCB_Error_Mark.Children.Add(model.ErrorPositions[model.ErrorPositions.Count - 1].label);

            }
        }

        private void imgPCB_Error_Position_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsAddingErrorPosition)
            {
                //model.ErrorPositions.Add(newErrorPosition);
                model.ErrorPositions[model.ErrorPositions.Count - 1].X = model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft;
                model.ErrorPositions[model.ErrorPositions.Count - 1].Y = model.ErrorPositions[model.ErrorPositions.Count - 1].lbTop;

                Canvas.SetTop(model.ErrorPositions[model.ErrorPositions.Count - 1].label, model.ErrorPositions[model.ErrorPositions.Count - 1].Y);
                Canvas.SetLeft(model.ErrorPositions[model.ErrorPositions.Count - 1].label, model.ErrorPositions[model.ErrorPositions.Count - 1].X);

                //Canvas.SetTop(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].Y);
                //Canvas.SetLeft(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].X);

                Error_Positions_Table.ItemsSource = model.ErrorPositions;
                Error_Positions_Table.Items.Refresh();
                IsAddingErrorPosition = false;
                btPCB_Error_Position_Add.IsChecked = false;
            }
        }

        public void Error_Mark_Draw()
        {

        }

        private void Canvas_PCB_Error_Mark_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (IsAddingErrorPosition)
                {
                    Console.WriteLine("X : {0} ---- Y : {1}", e.GetPosition(Canvas_PCB_Error_Mark).X, e.GetPosition(Canvas_PCB_Error_Mark).Y);

                    if (e.GetPosition(Canvas_PCB_Error_Mark).X >= model.ErrorPositions[model.ErrorPositions.Count - 1].X)
                    {
                        model.ErrorPositions[model.ErrorPositions.Count - 1].Width = e.GetPosition(Canvas_PCB_Error_Mark).X - model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft;
                    }
                    else
                    {
                        //var X_temp = model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft;

                        model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft = e.GetPosition(Canvas_PCB_Error_Mark).X;
                        model.ErrorPositions[model.ErrorPositions.Count - 1].Width = model.ErrorPositions[model.ErrorPositions.Count - 1].X - e.GetPosition(Canvas_PCB_Error_Mark).X;
                        Canvas.SetLeft(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft);
                    }

                    if (e.GetPosition(Canvas_PCB_Error_Mark).Y >= model.ErrorPositions[model.ErrorPositions.Count - 1].Y)
                    {
                        model.ErrorPositions[model.ErrorPositions.Count - 1].Height = e.GetPosition(Canvas_PCB_Error_Mark).Y - model.ErrorPositions[model.ErrorPositions.Count - 1].lbTop;
                    }
                    else
                    {
                        //var X_temp = model.ErrorPositions[model.ErrorPositions.Count - 1].lbLeft;

                        model.ErrorPositions[model.ErrorPositions.Count - 1].lbTop = e.GetPosition(Canvas_PCB_Error_Mark).Y;
                        model.ErrorPositions[model.ErrorPositions.Count - 1].Height = model.ErrorPositions[model.ErrorPositions.Count - 1].Y - e.GetPosition(Canvas_PCB_Error_Mark).Y;
                        Canvas.SetTop(model.ErrorPositions[model.ErrorPositions.Count - 1].rect, model.ErrorPositions[model.ErrorPositions.Count - 1].lbTop);
                    }
                }
            }
        }

        private void btPCB_Error_Position_Browser_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Open PCB Image";
            if (openFile.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            Uri fileUri = new Uri(openFile.FileName);
            tbPCB_Error_Position_Path.Text = openFile.FileName;
            imgPCB_Error_Position.Source = new BitmapImage(fileUri);
        }

        private void btPCB_Error_Position_Add_Checked(object sender, RoutedEventArgs e)
        {
            IsAddingErrorPosition = true;
        }

        private void btPCB_Error_Position_Add_Unchecked(object sender, RoutedEventArgs e)
        {
            IsAddingErrorPosition = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btReference_Copy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



    }

}