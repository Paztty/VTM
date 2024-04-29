using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HVT.Controls
{
    /// <summary>
    /// Interaction logic for GWIN_TECH_DMM.xaml
    /// </summary>
    public partial class GWIN_TECH_DMM : UserControl
    {
        private DMM dmm1 = new DMM("DMM#1");
        private DMM dmm2 = new DMM("DMM#2");

        public DMM DMM1
        {
            get { return dmm1; }
            set
            {
                dmm1 = value;
            }
        }
        public DMM DMM2
        {
            get { return dmm2; }
            set
            {
                dmm2 = value;
            }
        }


        private DMM.DMM_Mode _Mode;
        public DMM.DMM_Mode Mode
        {
            get { return _Mode; }
            set
            {
                if (value != _Mode)
                {
                    _Mode = value;
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        DMM_Mode_DC.IsChecked = Mode == DMM.DMM_Mode.DCV;
                        DMM_Mode_AC.IsChecked = Mode == DMM.DMM_Mode.ACV;
                        DMM_Mode_FRQ.IsChecked = Mode == DMM.DMM_Mode.FREQ;
                        DMM_Mode_DIODE.IsChecked = Mode == DMM.DMM_Mode.DIODE;
                        cbbDMM_range.Visibility = Visibility.Visible;
                        DMM1.Mode = Mode;
                        DMM2.Mode = Mode;
                        switch (Mode)
                        {
                            case DMM.DMM_Mode.NONE:
                                break;
                            case DMM.DMM_Mode.DCV:
                                cbbDMM_range.ItemsSource = Enum.GetValues(typeof(DMM.DMM_DCV_Range)).Cast<DMM.DMM_DCV_Range>();
                                break;
                            case DMM.DMM_Mode.ACV:
                                cbbDMM_range.ItemsSource = Enum.GetValues(typeof(DMM.DMM_ACV_Range)).Cast<DMM.DMM_ACV_Range>();
                                break;
                            case DMM.DMM_Mode.FREQ:
                                cbbDMM_range.ItemsSource = Enum.GetValues(typeof(DMM.DMM_ACV_Range)).Cast<DMM.DMM_ACV_Range>();
                                break;
                            case DMM.DMM_Mode.RES:
                                cbbDMM_range.ItemsSource = Enum.GetValues(typeof(DMM.DMM_RES_Range)).Cast<DMM.DMM_RES_Range>();
                                break;
                            case DMM.DMM_Mode.DIODE:
                                cbbDMM_range.Visibility = Visibility.Hidden;
                                break;
                            default:
                                break;
                        }
                    }));
                }
            }
        }


        private DMM.DMM_Rate _Rate;
        public DMM.DMM_Rate Rate
        {
            get { return _Rate; }
            set
            {
                if (value != _Rate)
                {
                    _Rate = value;
                    this.Dispatcher.Invoke(
                        new Action(() =>
                        {
                            btDMMfastRate.IsChecked = Rate == DMM.DMM_Rate.FAST;
                            btDMMmidRate.IsChecked = Rate == DMM.DMM_Rate.MID;
                            btDMMslowRate.IsChecked = Rate == DMM.DMM_Rate.SLOW;
                    }));
                }
            }
        }

        public GWIN_TECH_DMM()
        {
            InitializeComponent();
            DMM1_Value.DataContext = dmm1;
            DMM2_Value.DataContext = dmm2;
        }

        private void cbDMM_AutoRead_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)cbDMM_AutoRead.IsChecked == false)
            {
                DMM1.UpdateValue((bool)cbDMM_AutoRead.IsChecked, (int)nudPeriod.Value);
                DMM2.UpdateValue((bool)cbDMM_AutoRead.IsChecked, (int)nudPeriod.Value);
            }
        }

        private void btMeasureCH1_Click(object sender, RoutedEventArgs e)
        {
            DMM1.UpdateValue((bool)cbDMM_AutoRead.IsChecked, (int)nudPeriod.Value);
            DMM1.GetValue();
        }

        private void btMeasureAll_Click(object sender, RoutedEventArgs e)
        {
            DMM1.GetValue();
            DMM2.GetValue();
            DMM1.UpdateValue((bool)cbDMM_AutoRead.IsChecked, (int)nudPeriod.Value);
            DMM2.UpdateValue((bool)cbDMM_AutoRead.IsChecked, (int)nudPeriod.Value);
        }

        private void btMeasureCH2_Click(object sender, RoutedEventArgs e)
        {
            DMM2.GetValue();
            DMM2.UpdateValue((bool)cbDMM_AutoRead.IsChecked, (int)nudPeriod.Value);
        }

        private void DMM_Mode_Click(object sender, RoutedEventArgs e)
        {
            cbDMM_AutoRead.IsChecked = false;
            DMM1.IsAutoUpdate = false;
            DMM2.IsAutoUpdate = false;

            cbbDMM_range.SelectionChanged -= cbbDMM_range_SelectionChanged;
            nudPeriod.Minimum = 100;

            DMM_Mode_DC.IsChecked = (sender as ToggleButton) == DMM_Mode_DC;
            DMM_Mode_AC.IsChecked = (sender as ToggleButton) == DMM_Mode_AC;
            DMM_Mode_FRQ.IsChecked = (sender as ToggleButton) == DMM_Mode_FRQ;
            DMM_Mode_RES.IsChecked = (sender as ToggleButton) == DMM_Mode_RES;
            DMM_Mode_DIODE.IsChecked = (sender as ToggleButton) == DMM_Mode_DIODE;

            bool TheLastIsDiodeMode = Mode == DMM.DMM_Mode.DIODE;

            cbbDMM_range.Visibility = Visibility.Visible;
            cbbDMM_range.ItemsSource = null;
            switch ((sender as ToggleButton).Name)
            {
                case "DMM_Mode_DC":
                    DMM1.SetMode(DMM.DMM_Mode.DCV);
                    DMM2.SetMode(DMM.DMM_Mode.DCV);
                    var _enumval = Enum.GetValues(typeof(DMM.DMM_DCV_Range)).Cast<DMM.DMM_DCV_Range>();
                    cbbDMM_range.ItemsSource = _enumval;
                    cbbDMM_range.SelectedIndex = 3;
                    Mode = DMM.DMM_Mode.DCV;
                    btDMMfastRate.IsChecked = DMM1.DCrate == DMM.DMM_Rate.FAST;
                    btDMMmidRate.IsChecked = DMM1.DCrate == DMM.DMM_Rate.MID;
                    btDMMslowRate.IsChecked = DMM1.DCrate == DMM.DMM_Rate.SLOW;

                    break;
                case "DMM_Mode_AC":
                    nudPeriod.Minimum = 500;
                    if ((int)nudPeriod.Value > 500)
                    {
                        nudPeriod.Value = 500;
                    }
                    DMM1.SetMode(DMM.DMM_Mode.ACV);
                    DMM2.SetMode(DMM.DMM_Mode.ACV);
                    var _enumval1 = Enum.GetValues(typeof(DMM.DMM_ACV_Range)).Cast<DMM.DMM_ACV_Range>();
                    cbbDMM_range.ItemsSource = _enumval1;
                    cbbDMM_range.SelectedIndex = 4;
                    Mode = DMM.DMM_Mode.ACV;
                    btDMMfastRate.IsChecked = DMM1.ACrate == DMM.DMM_Rate.FAST;
                    btDMMmidRate.IsChecked = DMM1.ACrate == DMM.DMM_Rate.MID;
                    btDMMslowRate.IsChecked = DMM1.ACrate == DMM.DMM_Rate.SLOW;

                    break;
                case "DMM_Mode_FRQ":
                    DMM1.SetMode(DMM.DMM_Mode.FREQ);
                    DMM2.SetMode(DMM.DMM_Mode.FREQ);
                    var _enumval2 = Enum.GetValues(typeof(DMM.DMM_ACV_Range)).Cast<DMM.DMM_ACV_Range>();
                    cbbDMM_range.ItemsSource = _enumval2;
                    cbbDMM_range.SelectedIndex = 4;
                    Mode = DMM.DMM_Mode.FREQ;
                    btDMMfastRate.IsChecked = DMM1.FREQrate == DMM.DMM_Rate.FAST;
                    btDMMmidRate.IsChecked = DMM1.FREQrate == DMM.DMM_Rate.MID;
                    btDMMslowRate.IsChecked = DMM1.FREQrate == DMM.DMM_Rate.SLOW;
                    break;
                case "DMM_Mode_RES":
                    DMM1.SetMode(DMM.DMM_Mode.RES);
                    DMM2.SetMode(DMM.DMM_Mode.RES);
                    var _enumval3 = Enum.GetValues(typeof(DMM.DMM_RES_Range)).Cast<DMM.DMM_RES_Range>();
                    cbbDMM_range.ItemsSource = _enumval3;
                    cbbDMM_range.SelectedIndex = 4;
                    Mode = DMM.DMM_Mode.RES;
                    btDMMfastRate.IsChecked = DMM1.RESrate == DMM.DMM_Rate.FAST;
                    btDMMmidRate.IsChecked = DMM1.RESrate == DMM.DMM_Rate.MID;
                    btDMMslowRate.IsChecked = DMM1.RESrate == DMM.DMM_Rate.SLOW;
                    break;

                case "DMM_Mode_DIODE":
                    DMM1.SetMode(DMM.DMM_Mode.DIODE);
                    DMM2.SetMode(DMM.DMM_Mode.DIODE);
                    btDMMfastRate.IsChecked = DMM1.DIODErate == DMM.DMM_Rate.FAST;
                    btDMMmidRate.IsChecked = DMM1.DIODErate == DMM.DMM_Rate.MID;
                    btDMMslowRate.IsChecked = DMM1.DIODErate == DMM.DMM_Rate.SLOW;
                    cbbDMM_range.Visibility = Visibility.Hidden;
                    if(TheLastIsDiodeMode) Task.Delay(500).Wait();
                    Mode = DMM.DMM_Mode.DIODE;
                    break;
                default:
                    break;
            }
            cbbDMM_range.SelectionChanged += cbbDMM_range_SelectionChanged;
            Task.Delay(100).Wait();
        }

        private void btDMMRate_Click(object sender, RoutedEventArgs e)
        {
            cbDMM_AutoRead.IsChecked = false;
            DMM1.IsAutoUpdate = false;
            DMM2.IsAutoUpdate = false;

            btDMMfastRate.IsChecked = (sender as ToggleButton) == btDMMfastRate;
            btDMMmidRate.IsChecked = (sender as ToggleButton) == btDMMmidRate;
            btDMMslowRate.IsChecked = (sender as ToggleButton) == btDMMslowRate;

            switch ((sender as ToggleButton).Content)
            {
                case "Fast":
                    DMM1.ChangeRate(DMM.DMM_Rate.FAST);
                    DMM2.ChangeRate(DMM.DMM_Rate.FAST);
                    break;
                case "Mid":
                    DMM1.ChangeRate(DMM.DMM_Rate.MID);
                    DMM2.ChangeRate(DMM.DMM_Rate.MID);
                    break;
                case "Slow":
                    DMM1.ChangeRate(DMM.DMM_Rate.SLOW);
                    DMM2.ChangeRate(DMM.DMM_Rate.SLOW);
                    break;
                default:
                    break;
            }
        }

        private void cbbDMM_range_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbDMM_AutoRead.IsChecked = false;
            DMM1.IsAutoUpdate = false;
            DMM2.IsAutoUpdate = false;
            if ((sender as ComboBox).SelectedIndex != -1)
            {
                DMM1.ChangeRange((sender as ComboBox).SelectedIndex);
                DMM2.ChangeRange((sender as ComboBox).SelectedIndex);
            }
        }

        public void SetModeDC(DMM.DMM_DCV_Range range, DMM.DMM_Rate rate)
        {
            
            DMM1.ChangeRange((int)range, DMM.DMM_Mode.DCV);
            DMM2.ChangeRange((int)range, DMM.DMM_Mode.DCV);
            if (Mode != DMM.DMM_Mode.DCV)
            {
                Mode = DMM.DMM_Mode.DCV;
                Task.Delay(1000).Wait();
            }
            Rate = rate;
            DMM1.ChangeRate(rate);
            DMM2.ChangeRate(rate);

            this.Dispatcher.Invoke(
                new Action(() =>
                {
                    cbbDMM_range.SelectedIndex = (int)range;
                    DMM_Mode_DC.IsChecked = Mode == DMM.DMM_Mode.DCV;
                    DMM_Mode_AC.IsChecked = Mode == DMM.DMM_Mode.ACV;
                    DMM_Mode_FRQ.IsChecked = Mode == DMM.DMM_Mode.FREQ;
                    DMM_Mode_RES.IsChecked = Mode == DMM.DMM_Mode.RES;
                    DMM_Mode_DIODE.IsChecked = Mode == DMM.DMM_Mode.DIODE;
                    btDMMfastRate.IsChecked = DMM1.DCrate == DMM.DMM_Rate.FAST;
                    btDMMmidRate.IsChecked = DMM1.DCrate == DMM.DMM_Rate.MID;
                    btDMMslowRate.IsChecked = DMM1.DCrate == DMM.DMM_Rate.SLOW;
                }));

        }
        
        public void SetModeAC(DMM.DMM_ACV_Range range, DMM.DMM_Rate rate)
        {

            DMM1.ChangeRange((int)range, DMM.DMM_Mode.ACV);
            DMM2.ChangeRange((int)range, DMM.DMM_Mode.ACV);
            if (Mode != DMM.DMM_Mode.ACV)
            {
                Mode = DMM.DMM_Mode.ACV;
                Task.Delay(1000).Wait();
            }

            Rate = rate;
            DMM1.ChangeRate(rate);
            DMM2.ChangeRate(rate);

            this.Dispatcher.Invoke(
                new Action(() =>
                {
                    cbbDMM_range.SelectedIndex = (int)range;
                    DMM_Mode_DC.IsChecked = Mode == DMM.DMM_Mode.DCV;
                    DMM_Mode_AC.IsChecked = Mode == DMM.DMM_Mode.ACV;
                    DMM_Mode_FRQ.IsChecked = Mode == DMM.DMM_Mode.FREQ;
                    DMM_Mode_RES.IsChecked = Mode == DMM.DMM_Mode.RES;
                    DMM_Mode_DIODE.IsChecked = Mode == DMM.DMM_Mode.DIODE;
                    btDMMfastRate.IsChecked = DMM1.ACrate == DMM.DMM_Rate.FAST;
                    btDMMmidRate.IsChecked = DMM1.ACrate == DMM.DMM_Rate.MID;
                    btDMMslowRate.IsChecked = DMM1.ACrate == DMM.DMM_Rate.SLOW;
                }));
        }

        public void SetModeFREQ(DMM.DMM_ACV_Range range, DMM.DMM_Rate rate)
        {

            DMM1.ChangeRange((int)range, DMM.DMM_Mode.FREQ);
            DMM2.ChangeRange((int)range, DMM.DMM_Mode.FREQ);
            if (Mode != DMM.DMM_Mode.FREQ)
            {
                Mode = DMM.DMM_Mode.FREQ;
                Task.Delay(1000).Wait();
            }
            Rate = rate;
            DMM1.ChangeRate(rate);
            DMM2.ChangeRate(rate);

            this.Dispatcher.Invoke(
                new Action(() =>
                {
                    cbbDMM_range.SelectedIndex = (int)range;
                    DMM_Mode_DC.IsChecked = Mode == DMM.DMM_Mode.DCV;
                    DMM_Mode_AC.IsChecked = Mode == DMM.DMM_Mode.ACV;
                    DMM_Mode_FRQ.IsChecked = Mode == DMM.DMM_Mode.FREQ;
                    DMM_Mode_RES.IsChecked = Mode == DMM.DMM_Mode.RES;
                    DMM_Mode_DIODE.IsChecked = Mode == DMM.DMM_Mode.DIODE;
                    btDMMfastRate.IsChecked = DMM1.FREQrate == DMM.DMM_Rate.FAST;
                    btDMMmidRate.IsChecked = DMM1.FREQrate == DMM.DMM_Rate.MID;
                    btDMMslowRate.IsChecked = DMM1.FREQrate == DMM.DMM_Rate.SLOW;
                }));
        }

        public void SetModeRES(DMM.DMM_RES_Range range, DMM.DMM_Rate rate)
        {

            DMM1.ChangeRange((int)range, DMM.DMM_Mode.RES);
            DMM2.ChangeRange((int)range, DMM.DMM_Mode.RES);
            Mode = DMM.DMM_Mode.RES;

            Rate = rate;
            DMM1.ChangeRate(rate);
            DMM2.ChangeRate(rate);

            this.Dispatcher.Invoke(
                new Action(() =>
                {
                    cbbDMM_range.SelectedIndex = (int)range;
                    DMM_Mode_DC.IsChecked = Mode == DMM.DMM_Mode.DCV;
                    DMM_Mode_AC.IsChecked = Mode == DMM.DMM_Mode.ACV;
                    DMM_Mode_FRQ.IsChecked = Mode == DMM.DMM_Mode.FREQ;
                    DMM_Mode_RES.IsChecked = Mode == DMM.DMM_Mode.RES;
                    DMM_Mode_DIODE.IsChecked = Mode == DMM.DMM_Mode.DIODE;
                    btDMMfastRate.IsChecked = DMM1.RESrate == DMM.DMM_Rate.FAST;
                    btDMMmidRate.IsChecked = DMM1.RESrate == DMM.DMM_Rate.MID;
                    btDMMslowRate.IsChecked = DMM1.RESrate == DMM.DMM_Rate.SLOW;
                }));
        }

        public void SetModeDiode(DMM.DMM_Rate rate)
        {
            DMM1.SetMode(Mode);
            DMM2.SetMode(Mode);

            if (Mode != DMM.DMM_Mode.DIODE)
            {
                Mode = DMM.DMM_Mode.DIODE;
                Task.Delay(1000).Wait();
            }
            Rate = rate;
            DMM1.ChangeRate(rate);
            DMM2.ChangeRate(rate);

            this.Dispatcher.Invoke(
                new Action(() =>
                {
                    DMM_Mode_DC.IsChecked = Mode == DMM.DMM_Mode.DCV;
                    DMM_Mode_AC.IsChecked = Mode == DMM.DMM_Mode.ACV;
                    DMM_Mode_FRQ.IsChecked = Mode == DMM.DMM_Mode.FREQ;
                    DMM_Mode_RES.IsChecked = Mode == DMM.DMM_Mode.RES;
                    DMM_Mode_DIODE.IsChecked = Mode == DMM.DMM_Mode.DIODE;

                    btDMMfastRate.IsChecked = DMM1.DIODErate == DMM.DMM_Rate.FAST;
                    btDMMmidRate.IsChecked = DMM1.DIODErate == DMM.DMM_Rate.MID;
                    btDMMslowRate.IsChecked = DMM1.DIODErate == DMM.DMM_Rate.SLOW;
                }));
        }
    }
}
