using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HVT.VTM.Program;
using HVT.Utility;
using HVT.VTM.Core;

namespace VTM
{
    /// <summary>
    /// Auto page source
    /// </summary>
    public partial class MainWindow : Window
    {
        #region AutoPage



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
            Program.RootModel.TestState = Model.RunTestState.STOP;
            EscapTimer.Stop();
        }
        #endregion

        #region Testting
        DateTime EscapTime;
        System.Timers.Timer EscapTimer = new System.Timers.Timer()
        {
            Interval = 100
        };

        public void Runtest()
        {
            if (Program.RootModel.TestState == Model.RunTestState.READY)
            {
                EscapTime = DateTime.Now;
                foreach (var item in Program.RootModel.Steps)
                {
                    item.ValueGet1 = "";
                    item.ValueGet2 = "";
                    item.ValueGet3 = "";
                    item.ValueGet4 = "";
                    item.Result1 = "";
                    item.Result2 = "";
                    item.Result3 = "";
                    item.Result4 = "";
                }
                TestStepsGrid.Items.Refresh();
                Program.RootModel.StepTesting = 0;
                Program.RootModel.IsTestting = false;
                Program.RootModel.TestState = Model.RunTestState.TESTTING;
                EscapTimer.Elapsed -= EscapTimer_Elapsed;
                EscapTimer.Elapsed += EscapTimer_Elapsed;
                EscapTimer.Start();
            }
        }
        private void EscapTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                lbEscapTime.Content = DateTime.Now.Subtract(EscapTime).TotalSeconds.ToString("F1") + "s";
            }));
        }
        private void Model_StepTestChangeAsync(object sender, EventArgs e)
        {
            if (Program.RootModel.StepTesting >= 0)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    TestStepsGrid.SelectedItem = Program.RootModel.Steps[(int)sender];
                    TestStepsGrid.ScrollIntoView(TestStepsGrid.SelectedItem);
                    TestStepsGridManual.ScrollIntoView(TestStepsGrid.SelectedItem);

                }), DispatcherPriority.DataBind);
            }
        }
        private void Model_TestRunFinish(object sender, EventArgs e)
        {
            EscapTimer.Stop();
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                lbTestResultTesting.Visibility = Visibility.Hidden;
                lbTestResultGood.Visibility = Visibility.Visible;
            }));

        }
        private void TestStepsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Program.RootModel.IsTestting)
            {
                ProgressTestSlider.Value = (int)((double)Program.RootModel.StepTesting / (Program.RootModel.Steps.Count - 2) * 100);
            }
        }

        #endregion

        #endregion

    }
}
