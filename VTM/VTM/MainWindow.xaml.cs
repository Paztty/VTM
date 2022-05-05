﻿using HVT.Utility;
using HVT.VTM.Base;
using HVT.VTM.Program;
using System;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;

namespace VTM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            Program.RootModel.UpdateCommand();
        }

        private void TestStepsGridData_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Keyboard.Focus(TestStepsGridData);
        }

        private void ModelPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Program.SaveModel();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.O && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Program.LoadModel();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.I && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Program.ImportModel();
                e.Handled = true;
                return;
            }
        }

        private void btAutoRun_Click(object sender, RoutedEventArgs e)
        {
            Runtest();
        }

        private void pnResult_MouseDown(object sender, MouseButtonEventArgs e)
        {
            pnResult.Visibility = Visibility.Hidden;

            for (int i = 0; i < Program.RootModel.contruction.PCB_Count; i++)
            {
                if (Program.RootModel.contruction.PBAs[i].Barcode != "")
                {
                    if (i == Program.RootModel.contruction.PCB_Count - 1)
                    {
                        Program.TestState = Program.RunTestState.READY;
                        return;
                    }
                }
                else
                {
                    Program.TestState = Program.RunTestState.WAIT;
                    return;
                }
            }
            
        }

        private void TestStepsGridManual_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TestStepsGridManual.SelectedItem != null)
            {
                Step step = (Step)TestStepsGridManual.SelectedItem;
                Program.currentStep = step;
                Program.RunStep();
            }
        }

        private void manualBoardSelect_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var Ctr = (sender as System.Windows.Controls.Primitives.ToggleButton);
            Ctr.IsChecked = Ctr.Visibility != Visibility.Visible ? false: Ctr.IsChecked;
        }

        private void btMuxSelectAll_Click(object sender, RoutedEventArgs e)
        {
            Program.MUX_CARD.SelectAll();
        }

        private void btMuxClearAll_Click(object sender, RoutedEventArgs e)
        {
            Program.MUX_CARD.ClearAll();
        }

        private void MuxSelecCard_Select(object sender, RoutedEventArgs e)
        {
            MuxSelecCard2.IsChecked = false;
            MuxSelecCard1.IsChecked = false;

            (sender as ToggleButton).IsChecked = true;

            pnMux1.Visibility = (bool)MuxSelecCard1.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            pnMux2.Visibility = (bool)MuxSelecCard2.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RelaySelecCard_Select(object sender, RoutedEventArgs e)
        {
            RelaySelecCard2.IsChecked = false;
            RelaySelecCard1.IsChecked = false;

            (sender as ToggleButton).IsChecked = true;

            pnRelay1.Visibility = (bool)RelaySelecCard1.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            pnRelay2.Visibility = (bool)RelaySelecCard2.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void pnMux_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Program.SendCardStatus();
        }

        private void pnRelay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Program.SendRLCardStatus();
        }

        private void btRelaySelectAll_Click(object sender, RoutedEventArgs e)
        {
            Program.LEVEL_CARD.SelectAll();
        }

        private void btRelayClearAll_Click(object sender, RoutedEventArgs e)
        {
            Program.LEVEL_CARD.ClearAll();
        }

        private void btPageSetup_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow(Program);
            settingWindow.ShowDialog();
            Program.appSetting = Extensions.OpenFromFile<AppSettingParam>("Config.cfg");
            Console.WriteLine();
        }

        private void cbbVisionTxnaming_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbTxpakage.Text = ((TxData)cbbVisionTxnaming.SelectedItem).Data;
        }

        private void btPageLog_Click(object sender, RoutedEventArgs e)
        {
            VTM_Report.MainWindow ReportWindow = new VTM_Report.MainWindow();
            ReportWindow.Show();

            //VTM_Report.App ReportApp = new VTM_Report.App();
            //ReportApp.Run();
        }

        private void btModelReset_Click(object sender, RoutedEventArgs e)
        {
            Program.TestState = Program.RunTestState.WAIT;
            Program.IsTestting = false;
            Program.RootModel.CleanSteps();
            Program.StepTesting = 0;
        }

        private void btSamplingStart_Checked(object sender, RoutedEventArgs e)
        {
            if (Program != null)
            {
                if (Program.LEVEL_CARD != null)
                {
                    Program.LEVEL_CARD.ClearChart();
                    Program.LEVEL_CARD.Sampling.Interval = TimeSpan.FromMilliseconds((int)nudSampleTime.Value);
                    Program.LEVEL_CARD.SampleCount = 0;
                    Program.LEVEL_CARD.TotalSample = (int)nudlevelTotalSample.Value;
                    Program.LEVEL_CARD.Sampling.Start();
                }
            }
        }

        private void btLastOpenModels_Click(object sender, RoutedEventArgs e)
        {
            panellastloaded.Children.Clear();
            foreach (var item in FolderMap.ModelLoadeds)
            {
                //item.FileExited = File.Exists(item.Path);
                panellastloaded.Children.Add(item.FileLabel);
            }
            panellastloaded.Visibility = panellastloaded.Visibility != Visibility.Visible ? Visibility.Visible: Visibility.Collapsed;
        }

        private void TestStepsGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Program.IsTestting)
            {
                var rowScroll = 3;
                var currentRow = TestStepsGrid.SelectedIndex;
                if (currentRow - rowScroll > 0 && currentRow - rowScroll > 0)
                {
                    TestStepsGrid.SelectedIndex = e.Delta < 0 ? currentRow + rowScroll : currentRow - rowScroll;
                }
            }
        }

        private void tgPower220V_Checked(object sender, RoutedEventArgs e)
        {
            tgPower110V.IsChecked = false;
        }

        private void tgPower110V_Checked(object sender, RoutedEventArgs e)
        {
            tgPower220V.IsChecked = false;
        }










        //////      /////    ////////////////////    ////////        ////
        /////     ////             /////           ////// ///      ////   
        ////    ///              /////           //////  ///    // //
        ///   ///              /////           //////   ///  //  //
        //  //               /////           //////     ////   //
        ///                /////           //////       //   //


        //Main window code  : MainWindow.cs
        //Auto page code    : AutoPage.cs
        //Manual page code  : ManualPage.cs
        //Model page code   : ModelPage.cs
        //Vision page code  : VisionPage.cs


        //Machine: PPS x2 , DMM x2, Main control x2, Mux x2, Level x1, Relay x1, Sol x1

        //Vision: Camera x1, 4 site {LED x30, GLED x30, FND x1, LCD x1}

        //PBA: list max 4 site:
        //      Site Name
        //      Site Skip
        //      List test steps
        //      Image
        //      Datetime
        //      QR in
        //      QR out
        //      PCB array config

        //Model program:
        //  - Model name
        //  - PCB array config
        //  - Camera setting
        //  - TX naming, RX naming (link file)
        //  - UUT serial config
        //  - QR naming (link file)
        //  - Barcode setting 
        //  - PPS DC Power setting
        //  - Discharge setting
        //  - Vision list steps test ()
        //  - PCB image
        //  - Channel setting
        //  - Mux use
        //  - Level use

        // Step test

        // Vision test

    }
}