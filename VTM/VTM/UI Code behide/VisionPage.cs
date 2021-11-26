
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
            Program.VisionInit(drawingTable,atDisplayCanvas,
                imgFNDviewA,
                imgFNDviewB,
                imgFNDviewC,
                imgFNDviewD,

                imgLCDviewA,
                imgLCDviewB,
                imgLCDviewC,
                imgLCDviewD,

                lbFNDvalueA,
                lbFNDvalueB,
                lbFNDvalueC,
                lbFNDvalueD,

                lbLCDvalueA,
                lbLCDvalueB,
                lbLCDvalueC,
                lbLCDvalueD,

                lbGLEDsHexa,
                lbLEDsHexa
                );
            //atDisplayCanvas.Source = Program.CaptureCanvasLayout(drawingTable);
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
            foreach (var item in Program.GLEDs)item.Visibility = Visibility.Collapsed;
            foreach (var item in Program.FNDs) item.Visibility = Visibility.Collapsed;

            GLEDsData.ItemsSource = null;
            GLEDsData.Items.Clear();
            GLEDsData.IsReadOnly = false ;

            LEDsData.ItemsSource = null;
            LEDsData.Items.Clear();
            LEDsData.IsReadOnly = false;

            switch (Program.boardSelected)
            {
                case Program.BoardSelected.All:
                    GLEDsData.IsReadOnly = true;
                    foreach (var item in Program.GLEDs)
                    {
                        item.Visibility = Visibility.Visible;
                        foreach (var item2 in item.GLEDs)
                        {
                            GLEDsData.Items.Add(item2);
                        }
                    }
                    LEDsData.IsReadOnly = true;
                    foreach (var item in Program.LEDs)
                    {
                        item.Visibility = Visibility.Visible;
                        foreach (var item2 in item.LEDs)
                        {
                            LEDsData.Items.Add(item2);
                        }
                    }
                    foreach (var item in Program.FNDs) item.Visibility = Visibility.Visible;

                    break;
                case Program.BoardSelected.A:
                    GLEDsData.ItemsSource = Program.GLEDs[0].GLEDs;
                    Program.GLEDs[0].Visibility = Visibility.Visible;

                    LEDsData.ItemsSource = Program.LEDs[0].LEDs;
                    Program.LEDs[0].Visibility = Visibility.Visible;

                    Program.FNDs[0].Visibility = Visibility.Visible;
                    break;
                case Program.BoardSelected.B:
                    GLEDsData.ItemsSource = Program.GLEDs[1].GLEDs;
                    Program.GLEDs[1].Visibility = Visibility.Visible;

                    LEDsData.ItemsSource = Program.LEDs[1].LEDs;
                    Program.LEDs[1].Visibility = Visibility.Visible;

                    Program.FNDs[1].Visibility = Visibility.Visible;
                    break;
                case Program.BoardSelected.C:
                    GLEDsData.ItemsSource = Program.GLEDs[2].GLEDs;
                    Program.GLEDs[2].Visibility = Visibility.Visible;

                    LEDsData.ItemsSource = Program.LEDs[2].LEDs;
                    Program.LEDs[2].Visibility = Visibility.Visible;

                    Program.FNDs[2].Visibility = Visibility.Visible;
                    break;
                case Program.BoardSelected.D:
                    GLEDsData.ItemsSource = Program.GLEDs[3].GLEDs;
                    Program.GLEDs[3].Visibility = Visibility.Visible;

                    LEDsData.ItemsSource = Program.LEDs[3].LEDs;
                    Program.LEDs[3].Visibility = Visibility.Visible;

                    Program.FNDs[3].Visibility = Visibility.Visible;
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
                    Program.FNDs[0].Threshold = e.NewValue;
                    break;
                case "sldFNDB":
                    Program.FNDs[1].Threshold = e.NewValue;
                    break;
                case "sldFNDC":
                    Program.FNDs[2].Threshold = e.NewValue;
                    break;
                case "sldFNDD":
                    Program.FNDs[3].Threshold = e.NewValue;
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
                    Program.LCDs[0].Threshold = e.NewValue;
                    break;
                case "sldLCDB":
                    Program.LCDs[1].Threshold = e.NewValue;
                    break;
                case "sldLCDC":
                    Program.LCDs[2].Threshold = e.NewValue;
                    break;
                case "sldLCDD":
                    Program.LCDs[3].Threshold = e.NewValue;
                    break;
                default:
                    break;
            }
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
            if (setCamTask != null && !setCamTask.IsCompleted)
            {
                return;
            }
            string paramSettup = (sender as Slider).Name;
            switch (paramSettup)
            {
                case "slExporsure":
                    setCamTask = Task.Run(async () => { Program.cameraStreaming.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Exposure, (int)e.NewValue); });
                    break;
                case "slBrightness":
                    setCamTask = Task.Run(async () => { Program.cameraStreaming.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Brightness, (int)e.NewValue); });
                    break;
                case "slContrast":
                    setCamTask = Task.Run(async () => { Program.cameraStreaming.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Contrast, (int)e.NewValue); });
                    break;
                case "slFocus":
                    setCamTask = Task.Run(async () => { Program.cameraStreaming.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Focus, (int)e.NewValue); });
                    break;
                case "slWhite":
                    setCamTask = Task.Run(async () => { Program.cameraStreaming.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.WhiteBalance, (int)e.NewValue); });
                    break;
                case "slSharpness":
                    setCamTask = Task.Run(async () => { Program.cameraStreaming.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Sharpness, (int)e.NewValue); });
                    break;
                case "slZoom":
                    setCamTask = Task.Run(async () => { Program.cameraStreaming.SetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Zoom, (int)e.NewValue); });
                    break;
                default:
                    break;
            }
        }

        private void GetCameraSettingValue()
        {
            slExporsure.Value = Program.cameraStreaming.GetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Exposure);
            slBrightness.Value = Program.cameraStreaming.GetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Brightness);
            slContrast.Value = Program.cameraStreaming.GetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Contrast);
            slFocus.Value = Program.cameraStreaming.GetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Focus);
            slWhite.Value = Program.cameraStreaming.GetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.WhiteBalance);
            slSharpness.Value = Program.cameraStreaming.GetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Sharpness);
            slZoom.Value = Program.cameraStreaming.GetParammeter(HVT.VTM.Base.CameraStreaming.VideoProperties.Sharpness);
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
    }
}
