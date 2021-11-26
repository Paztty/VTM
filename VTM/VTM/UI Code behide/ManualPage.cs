
using HVT.VTM.Base;
using HVT.VTM.Program;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace VTM
{
    /// <summary>
    /// Manual control page
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Load default Element
        /// </summary>
        public void LoadManualPage()
        {
            dgtUUT_TX.ItemsSource = null;
            dgtUUT_TX.ItemsSource = Program.Naming.TxDatas;
            dgtUUT_TX_Naming.ItemsSource = Program.Naming.TxDatas;

            Program.PPS_Init(ST1_CH1_V, ST1_CH2_V, ST2_CH1_V, ST2_CH2_V, ST1_CH1_A, ST1_CH2_A, ST2_CH1_A, ST2_CH2_A,
                RX_RECT_COM5, TX_RECT_COM5, CONNECTED_RECT_COM5, RX_RECT_COM6, TX_RECT_COM6, CONNECTED_RECT_COM6);

            Program.DMM_UI_Init(MinVal_DMM1, MaxVal_DMM1, Arg_DMM1, Val_DMM1, MinVal_DMM2, MaxVal_DMM2, Arg_DMM2, Val_DMM2,
                RX_RECT_COM3, TX_RECT_COM3, CONNECTED_RECT_COM3, RX_RECT_COM4, TX_RECT_COM4, CONNECTED_RECT_COM4);

            Program.ConnectCheck();

            DMM_Mode_DC.IsChecked = true;
            Program.DMM_ChangeMode(HVT.VTM.Base.DMM_Mode.DCV);
            var _enumval = Enum.GetValues(typeof(HVT.VTM.Base.DMM_DCV_Range)).Cast<HVT.VTM.Base.DMM_DCV_Range>();
            cbbDMM_range.ItemsSource = _enumval;
            cbbDMM_range.SelectedIndex = 3;
            btDMMslowRate.IsChecked = true;
        }

        #region Model Action
        //Model Init see at MainWindow.cs
        //Event 
        private void Model_LoadFinish_ManualPage(object sender, EventArgs e)
        {
            //TestStepsGridManual.ItemsSource = null;

        }


        #endregion





        private void btTestManual_Checked(object sender, RoutedEventArgs e)
        {
            if (!Program.IsTestting)
            {
                EscapTime = DateTime.Now;
                EscapTimer.Elapsed -= EscapTimer_Elapsed;
                EscapTimer.Elapsed += EscapTimer_Elapsed;
                EscapTimer.Start();
                Runtest();
            }
            else
            {
                EscapTime = DateTime.Now;
                Program.TestState = Model.RunTestState.TESTTING;
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
                EscapTimer.Start();
            }

        }

        private void btTestManual_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Program.IsTestting)
            {
                Program.TestState = Model.RunTestState.Pause;
                EscapTimer.Stop();
            }
        }

        private async void PPS1_On_Click(object sender, RoutedEventArgs e)
        {
            bool OnOK = await Program.OnSite(Program.PPS1, (bool)(sender as ToggleButton).IsChecked);
            (sender as ToggleButton).Content = (bool)(sender as ToggleButton).IsChecked ? "ON" : "OFF";
        }
        private async void PPS2_On_Click(object sender, RoutedEventArgs e)
        {
            bool OnOK = await Program.OnSite(Program.PPS2, (bool)(sender as ToggleButton).IsChecked);
            (sender as ToggleButton).Content = (bool)(sender as ToggleButton).IsChecked ? "ON" : "OFF";
        }


        #region DMM
        private void DMM_Mode_Click(object sender, RoutedEventArgs e)
        {
            cbbDMM_range.SelectionChanged -= cbbDMM_range_SelectionChanged;

            DMM_Mode_DC.IsChecked = (sender as ToggleButton) == DMM_Mode_DC;
            DMM_Mode_AC.IsChecked = (sender as ToggleButton) == DMM_Mode_AC;
            DMM_Mode_FRQ.IsChecked = (sender as ToggleButton) == DMM_Mode_FRQ;
            DMM_Mode_RES.IsChecked = (sender as ToggleButton) == DMM_Mode_RES;
            DMM_Mode_DIODE.IsChecked = (sender as ToggleButton) == DMM_Mode_DIODE;

            cbbDMM_range.Visibility = Visibility.Visible;
            cbbDMM_range.ItemsSource = null;

            switch ((sender as ToggleButton).Name)
            {
                case "DMM_Mode_DC":
                    Program.DMM_ChangeMode(HVT.VTM.Base.DMM_Mode.DCV);
                    var _enumval = Enum.GetValues(typeof(HVT.VTM.Base.DMM_DCV_Range)).Cast<HVT.VTM.Base.DMM_DCV_Range>();
                    cbbDMM_range.ItemsSource = _enumval;
                    cbbDMM_range.SelectedIndex = 3;
                    break;
                case "DMM_Mode_AC":
                    Program.DMM_ChangeMode(HVT.VTM.Base.DMM_Mode.ACV);
                    var _enumval1 = Enum.GetValues(typeof(HVT.VTM.Base.DMM_ACV_Range)).Cast<HVT.VTM.Base.DMM_ACV_Range>();
                    cbbDMM_range.ItemsSource = _enumval1;
                    cbbDMM_range.SelectedIndex = 4;
                    break;
                case "DMM_Mode_FRQ":
                    Program.DMM_ChangeMode(HVT.VTM.Base.DMM_Mode.FREQ);
                    var _enumval2 = Enum.GetValues(typeof(HVT.VTM.Base.DMM_ACV_Range)).Cast<HVT.VTM.Base.DMM_ACV_Range>();
                    cbbDMM_range.ItemsSource = _enumval2;
                    cbbDMM_range.SelectedIndex = 4;
                    break;
                case "DMM_Mode_RES":
                    Program.DMM_ChangeMode(HVT.VTM.Base.DMM_Mode.RES);
                    var _enumval3 = Enum.GetValues(typeof(HVT.VTM.Base.DMM_RES_Range)).Cast<HVT.VTM.Base.DMM_RES_Range>();
                    cbbDMM_range.ItemsSource = _enumval3;
                    cbbDMM_range.SelectedIndex = 4;
                    break;
                case "DMM_Mode_DIODE":
                    Program.DMM_ChangeMode(HVT.VTM.Base.DMM_Mode.DIODE);
                    cbbDMM_range.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
            cbbDMM_range.SelectionChanged += cbbDMM_range_SelectionChanged;
        }

        private void cbDMM_AutoRead_Click(object sender, RoutedEventArgs e)
        {
            cbDMM_MatchRead.IsChecked = !(bool)cbDMM_AutoRead.IsChecked;
            cbDMM_MatchRead.IsEnabled = !(bool)cbDMM_AutoRead.IsChecked;
            Program.DMM1.UpdateValue((bool)cbDMM_AutoRead.IsChecked, (int)nudPeriod.Value);
            Program.DMM2.UpdateValue((bool)cbDMM_AutoRead.IsChecked, (int)nudPeriod.Value);
        }

        private void cbbDMM_range_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Program.DMM_ChangeRange(cbbDMM_range.SelectedIndex);
        }

        private void btMeasureCH1_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbDMM_AutoRead.IsChecked)
            {
                Program.DMM1.UpdateValue(true, (int)nudPeriod.Value);
            }
            else
            {
                Program.DMM1.GetValue();
            }
        }

        private void btMeasureAll_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbDMM_AutoRead.IsChecked)
            {
                Program.DMM1.UpdateValue(true, (int)nudPeriod.Value);
                Program.DMM2.UpdateValue(true, (int)nudPeriod.Value);
            }
            else
            {
                Task.Run(async () =>
                {
                    Program.DMM1.GetValue();
                });
                Task.Run(async () =>
                {
                    Program.DMM2.GetValue();
                });
            }
        }

        private void btMeasureCH2_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbDMM_AutoRead.IsChecked)
            {
                Program.DMM2.UpdateValue(true, (int)nudPeriod.Value);
            }
            else
            {
                Task.Run(async () =>
                {
                    Program.DMM2.GetValue();
                });
            }
        }

        private void btDMMRate_Click(object sender, RoutedEventArgs e)
        {
            btDMMfastRate.IsChecked = (sender as ToggleButton) == btDMMfastRate;
            btDMMmidRate.IsChecked = (sender as ToggleButton) == btDMMmidRate;
            btDMMslowRate.IsChecked = (sender as ToggleButton) == btDMMslowRate;

            switch ((sender as ToggleButton).Content)
            {
                case "Fast":
                    Program.DMM_ChangeRate(HVT.VTM.Base.DMM_Rate.FAST);
                    break;
                case "Mid":
                    Program.DMM_ChangeRate(HVT.VTM.Base.DMM_Rate.MID);
                    break;
                case "Slow":
                    Program.DMM_ChangeRate(HVT.VTM.Base.DMM_Rate.SLOW);
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
