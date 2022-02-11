using HVT.Utility;
using HVT.VTM.Base;
using HVT.VTM.Program;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace VTM
{
    /// <summary>
    /// Auto page source
    /// </summary>
    public partial class MainWindow : Window
    {
        #region AutoPage
        public void LoadAutopage()
        {


            Program.RootModel.contruction.PCB_resultGrid = pnResult;
            Program.RootModel.contruction._resultGrid.Add(pnResultA);
            Program.RootModel.contruction._resultGrid.Add(pnResultB);
            Program.RootModel.contruction._resultGrid.Add(pnResultC);
            Program.RootModel.contruction._resultGrid.Add(pnResultD);

            waitSiteA.Child = Program.RootModel.contruction.PBAs[0].CbWait;
            waitSiteB.Child = Program.RootModel.contruction.PBAs[1].CbWait;
            waitSiteC.Child = Program.RootModel.contruction.PBAs[2].CbWait;
            waitSiteD.Child = Program.RootModel.contruction.PBAs[3].CbWait;

            lbwaitSiteA.Child = Program.RootModel.contruction.PBAs[0].lbIsWaiting;
            lbwaitSiteB.Child = Program.RootModel.contruction.PBAs[1].lbIsWaiting;
            lbwaitSiteC.Child = Program.RootModel.contruction.PBAs[2].lbIsWaiting;
            lbwaitSiteD.Child = Program.RootModel.contruction.PBAs[3].lbIsWaiting;

            BacodesTestingList.Dispatcher.Invoke(new Action(delegate
            {
                BacodesTestingList.ItemsSource = null;
                BacodesTestingList.ItemsSource = Program.RootModel.contruction.PBAs;
            }));
            BacodesWaitingList.Dispatcher.Invoke(new Action(delegate
            {
                BacodesWaitingList.ItemsSource = null;
                BacodesWaitingList.ItemsSource = Program.RootModel.contruction.PBAs;
            }));


        }


        #endregion

        #region Button Event
        private void btClearLog_Click(object sender, RoutedEventArgs e)
        {
            Debug.ClearLog();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lbTestResultTesting.Visibility = Visibility.Visible;
            lbTestResultGood.Visibility = Visibility.Hidden;
            Runtest();
        }
        private void btTestManualStop_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)btTestManual.IsChecked == true)
            {
                btTestManual.IsChecked = false;
            }
            Program.TestState = Program.RunTestState.STOP;
            EscapTimer.Stop();
        }
        #endregion

        #region Testting
        double EscapTime;
        System.Timers.Timer EscapTimer = new System.Timers.Timer()
        {
            Interval = 100
        };

        public void Runtest()
        {
            if (Program.RootModel.Steps.Count < 1)
            {
                return;
            }
            if (!Program.IsTestting)
            {
                bool barcodeCheck = true;
                if (Program.RootModel.UseBarcodeInput)
                {

                    for (int i = 0; i < Program.RootModel.contruction.PCB_Count; i++)
                    {
                        barcodeCheck = barcodeCheck && Program.RootModel.contruction.PBAs[i].Barcode != "";
                        if (!barcodeCheck)
                        {
                            Debug.Write("PCB " + Program.RootModel.contruction.PBAs[i].SiteName + " barcode empty.", Debug.ContentType.Error);
                            return;
                        }

                        lbbarcodeA.Content = Program.RootModel.contruction.PBAs[0].Barcode;
                        lbbarcodeB.Content = Program.RootModel.contruction.PBAs[1].Barcode;
                        lbbarcodeC.Content = Program.RootModel.contruction.PBAs[2].Barcode;
                        lbbarcodeD.Content = Program.RootModel.contruction.PBAs[3].Barcode;

                        foreach (var item in Program.RootModel.contruction.PBAs)
                        {
                            item.TestResult = false;
                            item.barcodeResult = item.Barcode;
                        }
                    }
                }

                if (barcodeCheck)
                {
                    pnResult.Visibility = Visibility.Hidden;
                    EscapTime = 0;
                    foreach (var item in Program.RootModel.Steps)
                    {
                        item.ValueGet1 = "";
                        item.ValueGet2 = "";
                        item.ValueGet3 = "";
                        item.ValueGet4 = "";
                        item.Result1 = Step.DontCare;
                        item.Result2 = Step.DontCare;
                        item.Result3 = Step.DontCare;
                        item.Result4 = Step.DontCare;
                    }
                    TestStepsGrid.Items.Refresh();
                    Program.StepTesting = 0;
                    Program.IsTestting = false;
                    Program.TestState = Program.RunTestState.TESTTING;
                    EscapTimer.Elapsed -= EscapTimer_Elapsed;
                    EscapTimer.Elapsed += EscapTimer_Elapsed;
                    EscapTimer.Start();
                    Program.RUN_TEST();
                }
            }
            else if (Program.TestState != Program.RunTestState.PAUSE)
            {
                Program.TestState = Program.RunTestState.PAUSE;
                EscapTimer.Stop();
            }
            else
            {
                Program.TestState = Program.RunTestState.TESTTING;
                EscapTimer.Start();
            }
        }
        private void EscapTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                EscapTime += 0.1;
                lbEscapTime.Content = (EscapTime).ToString("F1") + "s";
            }));
        }
        private void Model_StepTestChangeAsync(object sender, EventArgs e)
        {
            if (Program.StepTesting >= 0)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    TestStepsGrid.SelectedIndex = (int)sender;
                    TestStepsGrid.ScrollIntoView(TestStepsGrid.SelectedItem);
                    //TestStepsGridManual.SelectedItem = Program.RootModel.Steps[(int)sender];
                    TestStepsGridManual.ScrollIntoView(TestStepsGrid.SelectedItem);
                }), DispatcherPriority.DataBind);
            }
        }
        private void Model_TestRunFinish(object sender, EventArgs e)
        {
            EscapTimer.Stop();
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                lbResultA.Content = Program.RootModel.contruction.PBAs[0].TestResult ? "OK" : "NG";
                lbResultB.Content = Program.RootModel.contruction.PBAs[1].TestResult ? "OK" : "NG";
                lbResultC.Content = Program.RootModel.contruction.PBAs[2].TestResult ? "OK" : "NG";
                lbResultD.Content = Program.RootModel.contruction.PBAs[3].TestResult ? "OK" : "NG";

                pnResult.Visibility = Visibility.Visible;

                for (int i = 0; i < Program.RootModel.contruction.PCB_Count; i++)
                {
                    var item = Program.RootModel.contruction.PBAs[i];
                    if (item.TestResult)
                    {
                        Debug.Write("Site " + item.SiteName + ": " + item.barcodeResult + " - OK", Debug.ContentType.Notify);
                    }
                    else
                    {
                        Debug.Write("Site " + item.SiteName + ": " + item.barcodeResult + " - FAIL", Debug.ContentType.Error);
                    }
                }
                Program.UpdateBarcodeList();
            }));



        }
        private void TestStepsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Program.IsTestting)
            {
                ProgressTestSlider.Value = (int)((double)Program.StepTesting / (Program.RootModel.Steps.Count - 1) * 100);
            }
        }

        public void Autopanel_UpdateLayout(HVT.VTM.Base.Contruction contruction)
        {
            waitSiteA.Child = Program.RootModel.contruction.PBAs[0].CbWait;
            waitSiteB.Child = Program.RootModel.contruction.PBAs[1].CbWait;
            waitSiteC.Child = Program.RootModel.contruction.PBAs[2].CbWait;
            waitSiteD.Child = Program.RootModel.contruction.PBAs[3].CbWait;

            lbwaitSiteA.Child = Program.RootModel.contruction.PBAs[0].lbIsWaiting;
            lbwaitSiteB.Child = Program.RootModel.contruction.PBAs[1].lbIsWaiting;
            lbwaitSiteC.Child = Program.RootModel.contruction.PBAs[2].lbIsWaiting;
            lbwaitSiteD.Child = Program.RootModel.contruction.PBAs[3].lbIsWaiting;

            grAutoStep.ColumnDefinitions[5].Width = contruction.PCB_Count >= 1 ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Star);
            grAutoStep.ColumnDefinitions[6].Width = contruction.PCB_Count >= 2 ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Star);
            grAutoStep.ColumnDefinitions[7].Width = contruction.PCB_Count >= 3 ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Star);
            grAutoStep.ColumnDefinitions[8].Width = contruction.PCB_Count >= 4 ? new GridLength(1, GridUnitType.Star) : new GridLength(0, GridUnitType.Star);

            dtSite1.Visibility = contruction.PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
            dtSite2.Visibility = contruction.PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
            dtSite3.Visibility = contruction.PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
            dtSite4.Visibility = contruction.PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;

            dttSite1.Visibility = contruction.PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
            dttSite2.Visibility = contruction.PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
            dttSite3.Visibility = contruction.PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
            dttSite4.Visibility = contruction.PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;

            dtttSite1.Visibility = contruction.PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
            dtttSite2.Visibility = contruction.PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
            dtttSite3.Visibility = contruction.PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
            dtttSite4.Visibility = contruction.PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;


            Program.RootModel.Contruction_ContructionChanged();
            BacodesTestingList.Dispatcher.Invoke(new Action(delegate
            {
                BacodesTestingList.ItemsSource = null;
                BacodesTestingList.ItemsSource = Program.RootModel.contruction.PBAs;
            }));
            BacodesWaitingList.Dispatcher.Invoke(new Action(delegate
            {
                BacodesWaitingList.ItemsSource = null;
                BacodesWaitingList.ItemsSource = Program.RootModel.contruction.PBAs;
            }));


        }

        #endregion

        #region Model Action
        //Event 
        private void Model_LoadFinish_AutoPage(object sender, EventArgs e)
        {

            var modelLoadedpath = FolderMap.ModelLoadeds.Select(x => x.Path).ToList();
            if (!modelLoadedpath.Contains(Program.RootModel.Path))
            {
                FolderMap.ModelLoadeds.Insert(0, new ModelLoaded() { Path = Program.RootModel.Path });
                panellastloaded.Children.Clear();
                foreach (var item in FolderMap.ModelLoadeds)
                {
                    panellastloaded.Children.Add(item.FileLabel);
                    item.FileLabel.Click -= OpenLastModel;
                    item.FileLabel.Click += OpenLastModel;
                }
            }


            waitSiteA.Child = Program.RootModel.contruction.PBAs[0].CbWait;
            waitSiteB.Child = Program.RootModel.contruction.PBAs[1].CbWait;
            waitSiteC.Child = Program.RootModel.contruction.PBAs[2].CbWait;
            waitSiteD.Child = Program.RootModel.contruction.PBAs[3].CbWait;

            lbwaitSiteA.Child = Program.RootModel.contruction.PBAs[0].lbIsWaiting;
            lbwaitSiteB.Child = Program.RootModel.contruction.PBAs[1].lbIsWaiting;
            lbwaitSiteC.Child = Program.RootModel.contruction.PBAs[2].lbIsWaiting;
            lbwaitSiteD.Child = Program.RootModel.contruction.PBAs[3].lbIsWaiting;

            tbModelName.Text = Program.RootModel.Name;
            TestStepsGrid.ItemsSource = null;
            TestStepsGrid.ItemsSource = new ObservableCollection<Step>(Program.RootModel.Steps);

            ModelName.Text = Program.RootModel.Name;
            tbModelNamePath.Text = Program.RootModel.Path;
            TestStepsGridData.ItemsSource = null;
            TestStepsGridData.ItemsSource = Program.RootModel.Steps;
            Error_Positions_Table.ItemsSource = Program.RootModel.ErrorPositions;

            Program.RootModel.contruction.PCB_resultGrid = pnResult;
            Program.RootModel.contruction._resultGrid.Clear();
            Program.RootModel.contruction._resultGrid.Add(pnResultA);
            Program.RootModel.contruction._resultGrid.Add(pnResultB);
            Program.RootModel.contruction._resultGrid.Add(pnResultC);
            Program.RootModel.contruction._resultGrid.Add(pnResultD);

            BacodesTestingList.Dispatcher.Invoke(new Action(delegate
            {
                BacodesTestingList.ItemsSource = null;
                BacodesTestingList.ItemsSource = Program.RootModel.contruction.PBAs;
            }));
            BacodesWaitingList.Dispatcher.Invoke(new Action(delegate
            {
                BacodesWaitingList.ItemsSource = null;
                BacodesWaitingList.ItemsSource = Program.RootModel.contruction.PBAs;
            }));
        }

        private void OpenLastModel(object sender, RoutedEventArgs e)
        {
            panellastloaded.Visibility = Visibility.Collapsed;

                string path = (string)(sender as Button).Content;
                if (File.Exists(path))
                {
                    Program.LoadModel(path);
                }

        }

        private void Model_StateChange(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action((
                delegate
                {
                    lbTestResultTesting.Visibility = Visibility.Hidden;
                    lbTestResultStop.Visibility = Visibility.Hidden;
                    lbTestResultPause.Visibility = Visibility.Hidden;
                    lbTestResultGood.Visibility = Visibility.Hidden;
                    lbTestResultFail.Visibility = Visibility.Hidden;
                    lbTestBusy.Visibility = Visibility.Hidden;
                    lbTestResultWait.Visibility = Visibility.Hidden;
                    lbTestReady.Visibility = Visibility.Hidden;
                }

                )));
            switch (Program.TestState)
            {
                case Program.RunTestState.WAIT:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultWait.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestResultWait);
                    }), DispatcherPriority.Send);
                    break;
                case Program.RunTestState.TESTTING:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultTesting.Visibility = Visibility.Visible;
                    }), DispatcherPriority.Send);
                    break;
                case Program.RunTestState.PAUSE:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultPause.Visibility = Visibility.Visible;
                    }), DispatcherPriority.Send);
                    break;
                case Program.RunTestState.STOP:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultStop.Visibility = Visibility.Visible;
                    }), DispatcherPriority.Send);
                    break;
                case Program.RunTestState.GOOD:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultGood.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestResultGood);
                    }), DispatcherPriority.Send);
                    break;
                case Program.RunTestState.FAIL:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestResultFail.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestResultFail);
                    }), DispatcherPriority.Send);
                    break;
                case Program.RunTestState.BUSY:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestBusy.Visibility = Visibility.Visible;
                    }), DispatcherPriority.Send);
                    break;
                case Program.RunTestState.READY:
                    Dispatcher.Invoke(new Action(delegate
                    {
                        lbTestReady.Visibility = Visibility.Visible;
                        Storyboard sb = (Storyboard)TryFindResource("LabelSlide");
                        if (sb != null) sb.Begin(lbTestReady);
                    }), DispatcherPriority.Send);
                    break;
                case Program.RunTestState.DONE:
                    btTestManual.Dispatcher.Invoke(new Action(() => btTestManual.IsChecked = false));
                    EscapTimer.Stop();
                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}
