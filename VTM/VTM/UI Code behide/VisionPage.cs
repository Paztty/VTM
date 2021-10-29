
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System;
using HVT.VTM.Base;

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
                //cameraload.Visibility = Visibility.Collapsed;
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

        #region vistion funtion button work
        private void btDraw_Click(object sender, RoutedEventArgs e)
        {

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
        }
        #endregion
    }
}
