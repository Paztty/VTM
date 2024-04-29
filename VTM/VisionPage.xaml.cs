using HVT.Utility;
using HVT.VTM.Base;
using HVT.VTM.Program;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

namespace VTM
{
    /// <summary>
    /// Interaction logic for VisionPage.xaml
    /// </summary>
    public partial class VisionPage : Page
    {        //Variable
        private Model editModel = new Model();
        public Model EditModel
        {
            get { return editModel; }
            set
            {
                if (value != editModel)
                {
                    Program.EditModel = value;
                    editModel = value;

                    this.DataContext = EditModel;
                    if (this._contentLoaded)
                    {
                        if (EditModel.VisionModels != null)
                        {
                            VisionBuider.Models = EditModel.VisionModels;
                            componentOptionHolder.Child = EditModel.VisionModels.Option;
                        }
                        else
                        {
                            //EditModel.VisionModels = new Camera.VisionModel();
                            VisionBuider.Models = EditModel.VisionModels;
                            componentOptionHolder.Child = VisionBuider.Models.Option;
                        }
                        GLEDsData.ItemsSource = VisionBuider.Models.GLED[0].GLEDs;
                        LEDsData.ItemsSource = VisionBuider.Models.LED[0].LEDs;
                        componentOptionHolder.Child = VisionBuider.Models.Option;
                        EditModel.VisionModels.UpdateLayout(EditModel.Layout.PCB_Count);
                        EditModel.Layout.PCB_COUNT_CHANGE += Layout_PCB_COUNT_CHANGE;
                        UpdateLayout(EditModel.Layout.PCB_Count);
                        cbbTxnaming_Manual.ItemsSource = editModel.Naming.TxDatas.Select(x => x.Name).ToList();
                        FND_lookupTable.Update(editModel.ModelSegmentLookup);
                    }
                }
            }
        }



        private Program program = new Program();
        public Program Program
        {
            get { return program; }
            set
            {
                program = value;
                Program.EditModel = editModel;
                SolenoidControl.SerialPort.Port = program.Solenoid.SerialPort.Port;
                RelayControl.SerialPort.Port = program.RELAY.SerialPort.Port;
                SystemControl.System_Board = program.SYSTEM.System_Board;
                UUT1Com.Content = Program.UUTs[0].LogBoxVision;
                UUT2Com.Content = Program.UUTs[1].LogBoxVision;
                UUT3Com.Content = Program.UUTs[2].LogBoxVision;
                UUT4Com.Content = Program.UUTs[3].LogBoxVision;
            }
        }

        Timer GetFNDImageSampleTimer = new Timer
        {
            Interval = 100,
        };

        Timer GetLCDImageSampleTimer = new Timer
        {
            Interval = 1000,
        };

        public VisionPage()
        {
            InitializeComponent();

            // Timer get image for test
            GetFNDImageSampleTimer.Elapsed += GetImageSampleTimer_Elapsed;

            GetLCDImageSampleTimer.Elapsed += GetLCDImageSampleTimer_Elapsed;

            EditModel.VisionModels = VisionBuider.Models;

            componentOptionHolder.Child = VisionBuider.Models.Option;

        }

        public void EnableLive()
        {
            GetFNDImageSampleTimer.Start();
            GetLCDImageSampleTimer.Start();
        }

        public void DisableLive()
        {
            GetFNDImageSampleTimer.Stop();
            GetLCDImageSampleTimer.Stop();
        }

        private void GetLCDImageSampleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Program.EditModel.VisionModels.GetLCDSampleImage(Program.Capture?.LastMatFrame);
            }));
        }

        private void GetImageSampleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Program.EditModel.VisionModels.GetFNDSampleImage(Program.Capture?.LastMatFrame);
                Program.EditModel.VisionModels.GetLEDSampleImage(Program.Capture?.LastMatFrame);
                Program.EditModel.VisionModels.GetGLEDSampleImage(Program.Capture?.LastMatFrame);

                if (tgbSelectA.IsChecked == true)
                {
                    lbGLEDvalue.Content = Program.EditModel.VisionModels.GLED[0].CalculatorOutputString;
                    lbLEDvalue.Content = Program.EditModel.VisionModels.LED[0].CalculatorOutputString;
                }
                if (tgbSelectB.IsChecked == true)
                {
                    lbGLEDvalue.Content = Program.EditModel.VisionModels.GLED[1].CalculatorOutputString;
                    lbLEDvalue.Content = Program.EditModel.VisionModels.LED[1].CalculatorOutputString;
                }
                if (tgbSelectC.IsChecked == true)
                {
                    lbGLEDvalue.Content = Program.EditModel.VisionModels.GLED[2].CalculatorOutputString;
                    lbLEDvalue.Content = Program.EditModel.VisionModels.LED[2].CalculatorOutputString;
                }
                if (tgbSelectD.IsChecked == true)
                {
                    lbGLEDvalue.Content = Program.EditModel.VisionModels.GLED[3].CalculatorOutputString;
                    lbLEDvalue.Content = Program.EditModel.VisionModels.LED[3].CalculatorOutputString;
                }
            }));
        }

        private void btOpenModel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                DefaultExt = ".model",
                Title = "Open model",
            };
            openFile.Filter = "VTM model files (*.vmdl)|*.vmdl";
            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() == true)
            {
                HVT.Utility.Debug.Write("Load model:" + openFile.FileName, HVT.Utility.Debug.ContentType.Notify);
                //var fileInfor = new FileInfo(openFile.FileName);
                string modelStr = System.IO.File.ReadAllText(openFile.FileName);
                try
                {
                    string modelString = HVT.Utility.Extensions.Decoder(modelStr, System.Text.Encoding.UTF7);
                    //TestModel = HVT.Utility.Extensions.ConvertFromJson<Model>(modelString);
                    EditModel = HVT.Utility.Extensions.ConvertFromJson<Model>(modelString);
                    EditModel.Path = openFile.FileName;
                    foreach (var item in EditModel.Steps)
                    {
                        item.ValueGet1 = "";
                        item.ValueGet2 = "";
                        item.ValueGet3 = "";
                        item.ValueGet4 = "";
                    }
                    Program.OnEditModelLoaded();
                }
                catch (Exception)
                {
                    HVT.Utility.Debug.Write("Load model fail, file not correct format. \n" +
                        "Model folder: " + openFile.FileName, HVT.Utility.Debug.ContentType.Error);
                }
            }
        }

        private async void btSaveModel_Click(object sender, RoutedEventArgs e)
        {
            EditModel.CameraSetting = cameraSetting.GetParammeter();
            EditModel.ModelSegmentLookup = Camera.FND.SEG_LOOKUP.Clone();
            if (File.Exists(EditModel.Path))
            {
                saveLabel.Visibility = Visibility.Visible;
                await Task.Delay(100);
                HVT.Utility.Extensions.SaveToFile(EditModel, EditModel.Path);
                //Program.OnEditModelSave();
                await Task.Delay(100);
                saveLabel.Visibility = Visibility.Hidden;
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = HVT.VTM.Program.FolderMap.RootFolder;
                saveFileDialog.AddExtension = true;
                saveFileDialog.DefaultExt = FolderMap.DefaultModelFileExt;
                if ((bool)saveFileDialog.ShowDialog())
                {
                    saveLabel.Visibility = Visibility.Visible;
                    await Task.Delay(100);
                    EditModel.Name = saveFileDialog.SafeFileName;
                    EditModel.Path = saveFileDialog.FileName;
                    HVT.Utility.Extensions.SaveToFile(EditModel, saveFileDialog.FileName);
                    await Task.Delay(100);
                    saveLabel.Visibility = Visibility.Hidden;
                }
            }
            Program.OnEditModelSave();

        }

        private async void btSaveAsModel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = HVT.VTM.Program.FolderMap.RootFolder;
            saveFileDialog.AddExtension = true;
            saveFileDialog.DefaultExt = FolderMap.DefaultModelFileExt;
            if ((bool)saveFileDialog.ShowDialog())
            {
                saveLabel.Visibility = Visibility.Visible;
                await Task.Delay(100);
                EditModel.Name = saveFileDialog.SafeFileName;
                EditModel.Path = saveFileDialog.FileName;
                EditModel.CameraSetting = cameraSetting.GetParammeter();
                EditModel.ModelSegmentLookup = Camera.FND.SEG_LOOKUP.Clone();
                HVT.Utility.Extensions.SaveToFile(EditModel, saveFileDialog.FileName);
                saveLabel.Visibility = Visibility.Hidden;
                await Task.Delay(100);
            }
            Program.OnEditModelSave();

        }

        private void LEDsData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (LEDsData.SelectedItem != null)
            {
                LEDsData.ScrollIntoView(LEDsData.SelectedItem);
            }
        }

        private void waitCheckboxLED_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in LEDsData.Items)
            {
                (item as Camera.SingleLED).Use = false;
            }
        }

        private void waitCheckboxLED_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in LEDsData.Items)
            {
                (item as Camera.SingleLED).Use = true;
            }
        }

        private void waitCheckboxGLED_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in GLEDsData.Items)
            {
                (item as Camera.SingleGLED).Use = false;
            }
        }

        private void waitCheckboxGLED_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in GLEDsData.Items)
            {
                (item as Camera.SingleGLED).Use = true;
            }
        }

        private void UpdateLayout(int PCB_Count)
        {
            tgbSelectA.Visibility = PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed; 
            tgbSelectB.Visibility = PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
            tgbSelectC.Visibility = PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
            tgbSelectD.Visibility = PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;

        }

        private void BoardSellect_Click(object sender, RoutedEventArgs e)
        {
            var bt = (sender as ToggleButton);
            tgbSelectA.IsChecked = bt == tgbSelectA;
            tgbSelectB.IsChecked = bt == tgbSelectB;
            tgbSelectC.IsChecked = bt == tgbSelectC;
            tgbSelectD.IsChecked = bt == tgbSelectD;
            switch (bt.Name)
            {
                case "tgbSelectA" :
                    GLEDsData.ItemsSource = VisionBuider.Models.GLED[0].GLEDs;
                    LEDsData.ItemsSource = VisionBuider.Models.LED[0].LEDs;
                    break;
                case "tgbSelectB":
                    GLEDsData.ItemsSource = VisionBuider.Models.GLED[1].GLEDs;
                    LEDsData.ItemsSource = VisionBuider.Models.LED[1].LEDs;
                    break;
                case "tgbSelectC":
                    GLEDsData.ItemsSource = VisionBuider.Models.GLED[2].GLEDs;
                    LEDsData.ItemsSource = VisionBuider.Models.LED[2].LEDs;
                    break;
                case "tgbSelectD":
                    GLEDsData.ItemsSource = VisionBuider.Models.GLED[3].GLEDs;
                    LEDsData.ItemsSource = VisionBuider.Models.LED[3].LEDs;
                    break;
                default:
                    break;
            }
        }

        private void Layout_PCB_COUNT_CHANGE(object sender, EventArgs e)
        {
            UpdateLayout(EditModel.Layout.PCB_Count);
        }

        private void btGetValue_Click(object sender, RoutedEventArgs e)
        {
            Program.EditModel.VisionModels.GetLEDSampleImage(Program.Capture?.LastMatFrame);
            if (tgbSelectA.IsChecked == true)
            {
                lbLEDvalue.Content = Program.EditModel.VisionModels.LED[0].CalculatorOutputString;
            }
            if (tgbSelectB.IsChecked == true)
            {
                lbLEDvalue.Content = Program.EditModel.VisionModels.LED[1].CalculatorOutputString;
            }
            if (tgbSelectC.IsChecked == true)
            {
                lbLEDvalue.Content = Program.EditModel.VisionModels.LED[2].CalculatorOutputString;
            }
            if (tgbSelectD.IsChecked == true)
            {
                lbLEDvalue.Content = Program.EditModel.VisionModels.LED[3].CalculatorOutputString;
            }
        }

        private void btGetGLEDValue_Click(object sender, RoutedEventArgs e)
        {
            Program.EditModel.VisionModels.GetGLEDSampleImage(Program.Capture?.LastMatFrame);
            if (tgbSelectA.IsChecked == true)
            {
                lbGLEDvalue.Content = Program.EditModel.VisionModels.GLED[0].CalculatorOutputString;
            }
            if (tgbSelectB.IsChecked == true)
            {
                lbGLEDvalue.Content = Program.EditModel.VisionModels.GLED[1].CalculatorOutputString;
            }
            if (tgbSelectC.IsChecked == true)
            {
                lbGLEDvalue.Content = Program.EditModel.VisionModels.GLED[2].CalculatorOutputString;
            }
            if (tgbSelectD.IsChecked == true)
            {
                lbGLEDvalue.Content = Program.EditModel.VisionModels.GLED[3].CalculatorOutputString;
            }
        }

        private void btThresholdCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (tgbSelectA.IsChecked == true)
            {
                Program.EditModel.VisionModels.LED[0].CALC_THRESH();
            }
            if (tgbSelectB.IsChecked == true)
            {
                Program.EditModel.VisionModels.LED[1].CALC_THRESH();
            }
            if (tgbSelectC.IsChecked == true)
            {
                Program.EditModel.VisionModels.LED[2].CALC_THRESH();
            }
            if (tgbSelectD.IsChecked == true)
            {
                Program.EditModel.VisionModels.LED[3].CALC_THRESH();
            }
        }

        private void btGLEDThresholdCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (tgbSelectA.IsChecked == true)
            {
                Program.EditModel.VisionModels.GLED[0].CALC_THRESH();
            }
            if (tgbSelectB.IsChecked == true)
            {
                Program.EditModel.VisionModels.GLED[1].CALC_THRESH();
            }
            if (tgbSelectC.IsChecked == true)
            {
                Program.EditModel.VisionModels.GLED[2].CALC_THRESH();
            }
            if (tgbSelectD.IsChecked == true)
            {
                Program.EditModel.VisionModels.GLED[3].CALC_THRESH();
            }
        }

        private void cbbTxnaming_Mainual_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbTxnaming_Manual.SelectedItem != null)
            {
                var txData = EditModel.Naming.TxDatas.Where(o => o.Name == (string)cbbTxnaming_Manual.SelectedItem).First();
                if (txData != null)
                {
                    var data = cbbUUTconfig_Manual.Text == "P1" ? EditModel.P1_Config.GetFrame(txData.Data) : EditModel.P2_Config.GetFrame(txData.Data);
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
                var txData = EditModel.Naming.TxDatas.Where(o => o.Name == (string)cbbTxnaming_Manual.SelectedItem).First();
                foreach (var item in Program.UUTs)
                {
                    if (cbbUUTconfig_Manual.Text == "P1" && item.Config != EditModel.P1_Config)
                    {
                        item.Config = EditModel.P1_Config;
                    }
                    else if (cbbUUTconfig_Manual.Text == "P2")
                    {
                        item.Config = EditModel.P1_Config;
                    }
                }
                if (txData != null)
                {
                    if (EditModel.Layout.PCB_Count >= 1) Program.UUTs[0].Send(txData);
                    if (EditModel.Layout.PCB_Count >= 2) Program.UUTs[1].Send(txData);
                    if (EditModel.Layout.PCB_Count >= 3) Program.UUTs[2].Send(txData);
                    if (EditModel.Layout.PCB_Count >= 4) Program.UUTs[3].Send(txData);
                }
            }
        }
    }
}
