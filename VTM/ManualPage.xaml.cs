using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using HVT.VTM.Program;
using HVT.VTM.Base;
using System.Windows.Controls.Primitives;
using System.Timers;
using System.Threading;
using Timer = System.Timers.Timer;

namespace VTM
{
    /// <summary>
    /// Interaction logic for ManualPage.xaml
    /// </summary>
    public partial class ManualPage : Page
    {

        public CancellationTokenSource _shutDown = new CancellationTokenSource();
        private Program _Program;
        public Program Program
        {
            get { return _Program; }
            set
            {
                if (value != null || value != _Program)
                {
                    _Program = value;
                    TestModel = Program.TestModel;
                    DMM_Holder.Child = Program._DMM;
                    MuxControl_Holder.Child = Program.MuxCard;
                    RelayControl_Holder.Child = Program.RELAY;
                    SolenoidControl_Holder.Child = Program.Solenoid;
                    LevelViewer_Holder.Child = Program.LEVEL;
                    SYSTEM_Holder.Child = Program.SYSTEM;
                    StepDetailHolder.Child = Program.StepViewer;
                    VisionTest_Holder.Child = Program.VisionTester;
                    Program.StepTestChange += Program_StepTestChange;
                    Program.TestRunFinish += Program_TestRunFinish;

                    UUT1Com.Content = Program.UUTs[0].LogBox;
                    UUT2Com.Content = Program.UUTs[1].LogBox;
                    UUT3Com.Content = Program.UUTs[2].LogBox;
                    UUT4Com.Content = Program.UUTs[3].LogBox;
                }

            }
        }

        private void Program_TestRunFinish(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                btTest.IsChecked = false;
                btStop.IsChecked = true;
            }));
        }

        private Model _TestModel;
        public Model TestModel
        {
            get { return _TestModel; }
            set
            {
                if (value != null || value != _TestModel)
                {
                    _TestModel = value;
                    Program.MuxCard.Card = _TestModel.MuxCard;
                    Program.LEVEL.Card = _TestModel.LevelCard;
                    dgModelSteps.ItemsSource = _TestModel.Steps;

                    dtttSite1.Visibility = TestModel.Layout.PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
                    dtttSite2.Visibility = TestModel.Layout.PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
                    dtttSite3.Visibility = TestModel.Layout.PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
                    dtttSite4.Visibility = TestModel.Layout.PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;

                    Program.VisionTester.Models = TestModel.VisionModels;
                    VisionTest_Holder.Child = Program.VisionTester;

                    camera.SetParammeter(TestModel.CameraSetting);
                    cbbTxnaming_Manual.ItemsSource = _TestModel.Naming.TxDatas.Select(x => x.Name).ToList();
                }
            }
        }

        Timer GetFNDImageSampleTimer = new Timer
        {
            Interval = 100,
        };

        public ManualPage()
        {
            InitializeComponent();
            GetFNDImageSampleTimer.Elapsed += GetImageSampleTimer_Elapsed;
            GetFNDImageSampleTimer.Start();
        }

        private void dgModelSteps_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dgModelSteps.SelectedItem != null)
            {
                Program.StepViewer.StepToGet = (Step)dgModelSteps.SelectedItem;
            }
        }


        private void Program_StepTestChange(object sender, EventArgs e)
        {
            if (!this._shutDown.IsCancellationRequested)
            {
                pgbTestProgress.Dispatcher.Invoke(new Action(() =>
                {
                    var progressPercent = Math.Round((((int)sender + 1) / (double)TestModel.Steps.Count) * 100.0, 2);
                    pgbTestProgress.Value = (int)sender > 0 ? pgbTestProgress.Value < progressPercent ? progressPercent : pgbTestProgress.Value : progressPercent;
                    dgModelSteps.SelectedIndex = (int)sender;
                    dgModelSteps.ScrollIntoView(dgModelSteps.SelectedItem);
                }));
            }
        }
        private void dgModelSteps_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgModelSteps.SelectedItem != null)
            {
                Program.currentStep = (Step)dgModelSteps.SelectedItem;
                Program.RunStep();
            }
        }

        private void btTest_Checked(object sender, RoutedEventArgs e)
        {
            btStop.IsChecked = false;
            if (!Program.IsTestting)
            {
                Program.RUN_MANUAL_TEST();
            }
            else {
                Program.TestState = Program.RunTestState.MANUALTEST;
            }
        }

        private void btTest_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Program.IsTestting)
            {
                Program.TestState = Program.RunTestState.PAUSE;
            }
        }

        private void btStop_Checked(object sender, RoutedEventArgs e)
        {
            btTest.IsChecked = false;
            Program.IsTestting = false;
            if (Program.IsTestting)
            {
                Program.TestState = Program.RunTestState.STOP;
            }
        }

        private void waitCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            //if (Program.IsTestting) return;
            //DependencyObject dep = (DependencyObject)e.OriginalSource;
            //while ((dep != null) && !(dep is DataGridColumnHeader))
            //{
            //    dep = VisualTreeHelper.GetParent(dep);
            //}

            //if (dep == null)
            //    return;
            //if (dep is DataGridColumnHeader)
            //{
            //    DataGridColumnHeader columnHeader = dep as DataGridColumnHeader;
            //    columnHeader.Background = new SolidColorBrush(Color.FromRgb(21, 21, 21));
            //    Console.WriteLine(columnHeader.Content);
            //    string columnEnable = columnHeader.Content.ToString();
            //    switch (columnEnable)
            //    {
            //        case "A":
            //            if (Program.Boards.Count >= 1) Program.Boards[0].UserSkip = false;
            //            break;
            //        case "B":
            //            if (Program.Boards.Count >= 2) Program.Boards[1].UserSkip = false;
            //            break;
            //        case "C":
            //            if (Program.Boards.Count >= 3) Program.Boards[2].UserSkip = false;
            //            break;
            //        case "D":
            //            if (Program.Boards.Count >= 4) Program.Boards[3].UserSkip = false;
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        private void waitCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            //if (Program.IsTestting) return;
            //DependencyObject dep = (DependencyObject)e.OriginalSource;
            //while ((dep != null) && !(dep is DataGridColumnHeader))
            //{
            //    dep = VisualTreeHelper.GetParent(dep);
            //}

            //if (dep == null)
            //    return;
            //if (dep is DataGridColumnHeader)
            //{
            //    DataGridColumnHeader columnHeader = dep as DataGridColumnHeader;
            //    columnHeader.Background = new SolidColorBrush(Colors.Gray);
            //    Console.WriteLine(columnHeader.Content);
            //    string columnEnable = columnHeader.Content.ToString();
            //    switch (columnEnable)
            //    {
            //        case "A":
            //            if (Program.Boards.Count >= 1) Program.Boards[0].UserSkip = true;
            //            break;
            //        case "B":
            //            if (Program.Boards.Count >= 2) Program.Boards[1].UserSkip = true;
            //            break;
            //        case "C":
            //            if (Program.Boards.Count >= 3) Program.Boards[2].UserSkip = true;
            //            break;
            //        case "D":
            //            if (Program.Boards.Count >= 4) Program.Boards[3].UserSkip = true;
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }
        private void waitCheckbox_Click(object sender, RoutedEventArgs e)
        {
            if (Program.IsTestting) return;
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;
            if (dep is DataGridColumnHeader && (sender as CheckBox).IsChecked == true)
            {
                DataGridColumnHeader columnHeader = dep as DataGridColumnHeader;
                columnHeader.Background = new SolidColorBrush(Color.FromRgb(21, 21, 21));
                Console.WriteLine(columnHeader.Content);
                string columnEnable = columnHeader.Content.ToString();
                switch (columnEnable)
                {
                    case "A":
                        if (Program.Boards.Count >= 1) Program.Boards[0].UserSkip = false;
                        break;
                    case "B":
                        if (Program.Boards.Count >= 2) Program.Boards[1].UserSkip = false;
                        break;
                    case "C":
                        if (Program.Boards.Count >= 3) Program.Boards[2].UserSkip = false;
                        break;
                    case "D":
                        if (Program.Boards.Count >= 4) Program.Boards[3].UserSkip = false;
                        break;
                    default:
                        break;
                }
            }

            if (dep is DataGridColumnHeader && (sender as CheckBox).IsChecked == false)
            {
                DataGridColumnHeader columnHeader = dep as DataGridColumnHeader;
                columnHeader.Background = new SolidColorBrush(Colors.Gray);
                Console.WriteLine(columnHeader.Content);
                string columnEnable = columnHeader.Content.ToString();
                switch (columnEnable)
                {
                    case "A":
                        if (Program.Boards.Count >= 1) Program.Boards[0].UserSkip = true;
                        break;
                    case "B":
                        if (Program.Boards.Count >= 2) Program.Boards[1].UserSkip = true;
                        break;
                    case "C":
                        if (Program.Boards.Count >= 3) Program.Boards[2].UserSkip = true;
                        break;
                    case "D":
                        if (Program.Boards.Count >= 4) Program.Boards[3].UserSkip = true;
                        break;
                    default:
                        break;
                }

            }
        }

        private void GetImageSampleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                var lastFrameToTest = camera.LastMatFrame;
                if (lastFrameToTest == null)
                {
                    return;
                }
                Program.VisionTester.Models.GetFNDSampleImage(lastFrameToTest);
                Program.VisionTester.Models.GetGLEDSampleImage(lastFrameToTest);
                Program.VisionTester.Models.GetLEDSampleImage(lastFrameToTest);
            }));
        }

        public void EnableLive()
        {
            GetFNDImageSampleTimer.Start();
        }

        public void DisableLive()
        {
            GetFNDImageSampleTimer.Stop();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            Program.ResetTest();
            if(dgModelSteps.Items.Count > 1) dgModelSteps.SelectedIndex = 0;
        }

        private void btNextStep_Click(object sender, RoutedEventArgs e)
        {
            if (dgModelSteps.SelectedIndex > -1 && dgModelSteps.SelectedIndex + 1 < dgModelSteps.Items.Count)
            {
                Program.currentStep = Program.TestModel.Steps[dgModelSteps.SelectedIndex + 1];
                dgModelSteps.SelectedIndex = dgModelSteps.SelectedIndex + 1;
                Program.RunStep();
            }
        }

        private void cbbTxnaming_Mainual_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbTxnaming_Manual.SelectedItem != null)
            {
                var txData = TestModel.Naming.TxDatas.Where(o => o.Name == (string)cbbTxnaming_Manual.SelectedItem).First();
                if (txData != null)
                {
                    var data = cbbUUTconfig_Manual.Text == "P1" ? TestModel.P1_Config.GetFrame(txData.Data) : TestModel.P2_Config.GetFrame(txData.Data);
                    string dataStr = "";
                    foreach (var item in data)
                    {
                        dataStr += item.ToString("X2") + " ";
                    }
                    lbTxData.Content = dataStr;
                }
            }
        }

        private void ButtonSendUUT_Click(object sender, RoutedEventArgs e)
        {
            if (cbbTxnaming_Manual.SelectedItem != null)
            {
                var txData = TestModel.Naming.TxDatas.Where(o => o.Name == (string)cbbTxnaming_Manual.SelectedItem).First();
                foreach (var item in Program.UUTs)
                {
                    if (cbbUUTconfig_Manual.Text == "P1" && item.Config != TestModel.P1_Config)
                    {
                        item.Config = TestModel.P1_Config;
                    }
                    else if (cbbUUTconfig_Manual.Text == "P2")
                    {
                        item.Config = TestModel.P1_Config;
                    }
                }
                if (txData != null)
                {
                    if (TestModel.Layout.PCB_Count >= 1) Program.UUTs[0].Send(txData);
                    if (TestModel.Layout.PCB_Count >= 2) Program.UUTs[1].Send(txData);
                    if (TestModel.Layout.PCB_Count >= 3) Program.UUTs[2].Send(txData);
                    if (TestModel.Layout.PCB_Count >= 4) Program.UUTs[3].Send(txData);
                }
            }
        }
    }
}
