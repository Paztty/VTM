
using HVT.VTM.Program;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace VTM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Task list
        private Task setCamTask;
        public void VisionPageInit()
        {
            Program.Camstarted += Program_Camstarted;
            Program.Camstopped += Program_Camstopped;

            Program.CameraPropertiesChange += Program_CameraPropertiesChange;

            Program.VisionInit(
                                drawingTable,
                                atDisplayCanvas,
                                manualDisplayCanvas,

                                pnLCDA,
                                pnLCDB,
                                pnLCDC,
                                pnLCDD,

                                pnFNDA,
                                pnFNDB,
                                pnFNDC,
                                pnFNDD,

                                lbGLEDsHexa,
                                lbLEDsHexa

                                );
            rdbtSelectAll.IsChecked = true;
            //atDisplayCanvas.Source = Program.CaptureCanvasLayout(drawingTable);
        }

        private void Program_CameraPropertiesChange(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => GetCameraSettingValue()));
        }

        private void Program_Camstopped(object sender, System.EventArgs e)
        {

        }

        private void Program_Camstarted(object sender, System.EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                GetCameraSettingValue();
                cameraView.Visibility = Visibility.Visible;
                cameraload.Visibility = Visibility.Hidden;
                cameraload.Source = null;
            }));

        }

        private void functionsView_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void functionsView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }

        private void functionsView_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void Image_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {


        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Program.BoardSelected selected;
            Enum.TryParse<Program.BoardSelected>(((RadioButton)sender).Content.ToString(), out selected);
            Program.boardSelected = selected;
            //foreach (var item in Program.RootModel.GLEDs)item.Visibility = Visibility.Collapsed;
            //foreach (var item in Program.RootModel.FNDs) item.Visibility = Visibility.Collapsed;


            Program.RootModel.FNDs[0].Visibility = Visibility.Collapsed;
            Program.RootModel.FNDs[1].Visibility = Visibility.Collapsed;
            Program.RootModel.FNDs[2].Visibility = Visibility.Collapsed;
            Program.RootModel.FNDs[3].Visibility = Visibility.Collapsed;

            Program.RootModel.LCDs[0].Visibility = Visibility.Collapsed;
            Program.RootModel.LCDs[1].Visibility = Visibility.Collapsed;
            Program.RootModel.LCDs[2].Visibility = Visibility.Collapsed;
            Program.RootModel.LCDs[3].Visibility = Visibility.Collapsed;

            Program.RootModel.GLEDs[0].Visibility = Visibility.Collapsed;
            Program.RootModel.GLEDs[1].Visibility = Visibility.Collapsed;
            Program.RootModel.GLEDs[2].Visibility = Visibility.Collapsed;
            Program.RootModel.GLEDs[3].Visibility = Visibility.Collapsed;

            Program.RootModel.LEDs[0].Visibility = Visibility.Collapsed;
            Program.RootModel.LEDs[1].Visibility = Visibility.Collapsed;
            Program.RootModel.LEDs[2].Visibility = Visibility.Collapsed;
            Program.RootModel.LEDs[3].Visibility = Visibility.Collapsed;

            GLEDsData.ItemsSource = null;
            GLEDsData.Items.Clear();
            GLEDsData.IsReadOnly = false;

            LEDsData.ItemsSource = null;
            LEDsData.Items.Clear();
            LEDsData.IsReadOnly = false;

            switch (Program.boardSelected)
            {
                case Program.BoardSelected.All:
                    GLEDsData.IsReadOnly = true;
                    for (int i = 0; i < Program.RootModel.contruction.PCB_Count; i++)
                    {
                        var item = Program.RootModel.GLEDs[i];
                        item.Visibility = Visibility.Visible;
                        foreach (var item2 in item.GLEDs)
                        {
                            GLEDsData.Items.Add(item2);
                        }
                    }
                    LEDsData.IsReadOnly = true;
                    for (int i = 0; i < Program.RootModel.contruction.PCB_Count; i++)
                    {
                        var item = Program.RootModel.LEDs[i];
                        item.Visibility = Visibility.Visible;
                        foreach (var item2 in item.LEDs)
                        {
                            LEDsData.Items.Add(item2);
                        }
                    }
                    Vision_ContructionlayoutUpdate(Program.RootModel.contruction);

                    break;
                case Program.BoardSelected.A:
                    GLEDsData.ItemsSource = Program.RootModel.GLEDs[0].GLEDs;
                    Program.RootModel.GLEDs[0].Visibility = Visibility.Visible;

                    LEDsData.ItemsSource = Program.RootModel.LEDs[0].LEDs;
                    Program.RootModel.LEDs[0].Visibility = Visibility.Visible;

                    Program.RootModel.FNDs[0].Visibility = Visibility.Visible;
                    Program.RootModel.LCDs[0].Visibility = Visibility.Visible;
                    break;
                case Program.BoardSelected.B:
                    GLEDsData.ItemsSource = Program.RootModel.GLEDs[1].GLEDs;
                    Program.RootModel.GLEDs[1].Visibility = Visibility.Visible;

                    LEDsData.ItemsSource = Program.RootModel.LEDs[1].LEDs;
                    Program.RootModel.LEDs[1].Visibility = Visibility.Visible;

                    Program.RootModel.FNDs[1].Visibility = Visibility.Visible;
                    Program.RootModel.LCDs[1].Visibility = Visibility.Visible;
                    break;
                case Program.BoardSelected.C:
                    GLEDsData.ItemsSource = Program.RootModel.GLEDs[2].GLEDs;
                    Program.RootModel.GLEDs[2].Visibility = Visibility.Visible;

                    LEDsData.ItemsSource = Program.RootModel.LEDs[2].LEDs;
                    Program.RootModel.LEDs[2].Visibility = Visibility.Visible;

                    Program.RootModel.FNDs[2].Visibility = Visibility.Visible;
                    Program.RootModel.LCDs[2].Visibility = Visibility.Visible;
                    break;
                case Program.BoardSelected.D:
                    GLEDsData.ItemsSource = Program.RootModel.GLEDs[3].GLEDs;
                    Program.RootModel.GLEDs[3].Visibility = Visibility.Visible;

                    LEDsData.ItemsSource = Program.RootModel.LEDs[3].LEDs;
                    Program.RootModel.LEDs[3].Visibility = Visibility.Visible;

                    Program.RootModel.FNDs[3].Visibility = Visibility.Visible;
                    Program.RootModel.LCDs[3].Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }

        }

        private void sldFND_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string ctr = (sender as Slider).Name;
            switch (ctr)
            {
                case "sldFNDA":
                    Program.RootModel.FNDs[0].Threshold = e.NewValue;
                    break;
                case "sldFNDB":
                    Program.RootModel.FNDs[1].Threshold = e.NewValue;
                    break;
                case "sldFNDC":
                    Program.RootModel.FNDs[2].Threshold = e.NewValue;
                    break;
                case "sldFNDD":
                    Program.RootModel.FNDs[3].Threshold = e.NewValue;
                    break;
                default:
                    break;
            }
        }

        private void sldLCD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string ctr = (sender as Slider).Name;
            switch (ctr)
            {
                case "sldLCDA":
                    Program.RootModel.LCDs[0].Threshold = (int)e.NewValue;
                    break;
                case "sldLCDB":
                    Program.RootModel.LCDs[1].Threshold = (int)e.NewValue;
                    break;
                case "sldLCDC":
                    Program.RootModel.LCDs[2].Threshold = (int)e.NewValue;
                    break;
                case "sldLCDD":
                    Program.RootModel.LCDs[3].Threshold = (int)e.NewValue;
                    break;
                default:
                    break;
            }
        }



        private void Detect_Click(object sender, RoutedEventArgs e)
        {
            Program.StartUpdateLCD();
        }

        private void DetectFND_Click(object sender, RoutedEventArgs e)
        {
            Program.StartUpdateFND();
        }

        private void btThresholdCalculator_Click(object sender, RoutedEventArgs e)
        {
            Program.Calc_GLED_Thresh();
        }

        #region vistion funtion button work
        private void btDraw_Click(object sender, RoutedEventArgs e)
        {
            DrawingImageExample();
        }

        private void btMove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btSellect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Camera settup

        private void CameraSetting_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (setCamTask != null && setCamTask.Status == TaskStatus.Running)
            {
                return;
            }
            string paramSettup = (sender as Slider).Name;
            switch (paramSettup)
            {
                case "slExporsure":
                    setCamTask = Task.Run(() => { Program.cameraStreaming?.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Exposure, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slBrightness":
                    setCamTask = Task.Run(() => { Program.cameraStreaming?.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Brightness, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slContrast":
                    setCamTask = Task.Run(() => { Program.cameraStreaming?.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Contrast, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slFocus":
                    setCamTask = Task.Run(() => { Program.cameraStreaming?.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Focus, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slWhite":
                    setCamTask = Task.Run(() => { Program.cameraStreaming?.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.WhiteBalance, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slSharpness":
                    setCamTask = Task.Run(() => { Program.cameraStreaming?.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Sharpness, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slZoom":
                    setCamTask = Task.Run(() => { Program.cameraStreaming?.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Zoom, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slSatuation":
                    setCamTask = Task.Run(() => { Program.cameraStreaming?.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Satuation, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                default:
                    break;
            }
        }


        private void GetCameraSettingValue()
        {
            if (Program.RootModel.CameraSetting != null)
            {
                slExporsure.Value = Program.RootModel.CameraSetting.Exposure;
                slBrightness.Value = Program.RootModel.CameraSetting.Brightness;
                slContrast.Value = Program.RootModel.CameraSetting.Contrast;
                slFocus.Value = Program.RootModel.CameraSetting.Focus;
                slWhite.Value = Program.RootModel.CameraSetting.WBTemperature;
                slSharpness.Value = Program.RootModel.CameraSetting.Sharpness;
                slZoom.Value = Program.RootModel.CameraSetting.Zoom;

            }

        }

        private void Model_LoadFinish_VisionPage(object sender, EventArgs e)
        {
            sldFNDA.Value = Program.RootModel.FNDs[0].Threshold;
            sldFNDB.Value = Program.RootModel.FNDs[1].Threshold;
            sldFNDC.Value = Program.RootModel.FNDs[2].Threshold;
            sldFNDD.Value = Program.RootModel.FNDs[3].Threshold;

            sldLCDA.Value = Program.RootModel.LCDs[0].Threshold;
            sldLCDB.Value = Program.RootModel.LCDs[1].Threshold;
            sldLCDC.Value = Program.RootModel.LCDs[2].Threshold;
            sldLCDD.Value = Program.RootModel.LCDs[3].Threshold;

            Program.RootModel.ReplaceComponent(
                                drawingTable,
                                atDisplayCanvas,
                                manualDisplayCanvas,

                                pnLCDA,
                                pnLCDB,
                                pnLCDC,
                                pnLCDD,

                                pnFNDA,
                                pnFNDB,
                                pnFNDC,
                                pnFNDD);
        }
        #endregion

        #region FunctionsDrawing 


        public void DrawingImageExample()
        {

            //
            // Create the Geometry to draw.
            //
            GeometryGroup ellipses = new GeometryGroup();
            ellipses.Children.Add(
                new EllipseGeometry(new Point(50, 50), 45, 20)
                );
            ellipses.Children.Add(
                new EllipseGeometry(new Point(50, 50), 20, 45)
                );

            //
            // Create a GeometryDrawing.
            //
            GeometryDrawing aGeometryDrawing = new GeometryDrawing();
            aGeometryDrawing.Geometry = ellipses;

            // Paint the drawing with a gradient.

            // Outline the drawing with a solid color.
            aGeometryDrawing.Pen = new Pen(Brushes.Black, 10);

            //
            // Use a DrawingImage and an Image control
            // to display the drawing.
            //
            DrawingImage geometryImage = new DrawingImage(aGeometryDrawing);

            // Freeze the DrawingImage for performance benefits.
            geometryImage.Freeze();

            //drawingTable.Source = geometryImage;
        }
        #endregion

        #region layout update
        private void VistionTestGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            Program.IsVisionWorking = true;
        }

        private void VistionTestGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            Program.IsVisionWorking = false;
        }
        /// <summary>
        /// Ascess image test update when focus vision tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisionPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Program.IsVisionWorking = VisionPanel.Visibility == Visibility.Visible;
        }

        public void Vision_ContructionlayoutUpdate(HVT.VTM.Base.Contruction contruction)
        {
            rdbtSelectA.Visibility = contruction.PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
            rdbtSelectB.Visibility = contruction.PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
            rdbtSelectC.Visibility = contruction.PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
            rdbtSelectD.Visibility = contruction.PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;

            pnLCDA.Visibility = contruction.PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
            pnLCDB.Visibility = contruction.PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
            pnLCDC.Visibility = contruction.PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
            pnLCDD.Visibility = contruction.PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;

            pnFNDA.Visibility = contruction.PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
            pnFNDB.Visibility = contruction.PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
            pnFNDC.Visibility = contruction.PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
            pnFNDD.Visibility = contruction.PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;

            Program.RootModel.FNDs[0].Visibility = contruction.PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.FNDs[1].Visibility = contruction.PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.FNDs[2].Visibility = contruction.PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.FNDs[3].Visibility = contruction.PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;

            Program.RootModel.LCDs[0].Visibility = contruction.PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.LCDs[1].Visibility = contruction.PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.LCDs[2].Visibility = contruction.PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.LCDs[3].Visibility = contruction.PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;

            Program.RootModel.GLEDs[0].Visibility = contruction.PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.GLEDs[1].Visibility = contruction.PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.GLEDs[2].Visibility = contruction.PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.GLEDs[3].Visibility = contruction.PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;

            Program.RootModel.LEDs[0].Visibility = contruction.PCB_Count >= 1 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.LEDs[1].Visibility = contruction.PCB_Count >= 2 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.LEDs[2].Visibility = contruction.PCB_Count >= 3 ? Visibility.Visible : Visibility.Collapsed;
            Program.RootModel.LEDs[3].Visibility = contruction.PCB_Count >= 4 ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion
    }
}
