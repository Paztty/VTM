﻿using HVT.Utility;
using HVT.VTM.Base;
using HVT.VTM.Program;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

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
            Program.TestState = Model.RunTestState.STOP;
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
                    item.Result1 = Step.DontCare;
                    item.Result2 = Step.DontCare;
                    item.Result3 = Step.DontCare;
                    item.Result4 = Step.DontCare;
                }
                TestStepsGrid.Items.Refresh();
                Program.StepTesting = 0;
                Program.IsTestting = false;
                Program.TestState = Model.RunTestState.TESTTING;
                EscapTimer.Elapsed -= EscapTimer_Elapsed;
                EscapTimer.Elapsed += EscapTimer_Elapsed;
                EscapTimer.Start();
                Program.RUN_TEST();
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
            if (Program.StepTesting >= 0)
            {
                this.Dispatcher.Invoke(new Action(delegate
                {
                    TestStepsGrid.SelectedItem = Program.RootModel.Steps[(int)sender];
                    TestStepsGrid.ScrollIntoView(TestStepsGrid.SelectedItem);
                    TestStepsGridManual.SelectedItem = Program.RootModel.Steps[(int)sender];
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
            if (Program.IsTestting)
            {
                ProgressTestSlider.Value = (int)((double)Program.StepTesting / (Program.RootModel.Steps.Count - 2) * 100);
            }
        }

        #endregion
        
        #region Model Action

        //Event 
        private void Model_LoadFinish_AutoPage(object sender, EventArgs e)
        {
            tbModelName.Text = Program.RootModel.Name;
            TestStepsGrid.ItemsSource = null;
            TestStepsGrid.ItemsSource = new ObservableCollection<Step>(Program.RootModel.Steps);

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

    }
}