using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Camera.CameraControl;

namespace Camera
{
    /// <summary>
    /// Interaction logic for CameraSettingPanel.xaml
    /// </summary>
    public partial class CameraSettingPanel : UserControl
    {
        private CameraControl _Capture;
        public CameraControl Capture
        {
            get { return _Capture; }
            set
            {
                if (value != null || value != _Capture) _Capture = value;
                this.DataContext = Capture.cameraSetting;
            }
        }

        private bool _AllowControl = true;
        public bool AllowControl
        {
            get { return _AllowControl; }
            set
            {
                if (value != _AllowControl) _AllowControl = value;
            }
        }

        public CameraSettingPanel()
        {
            InitializeComponent();
        }

        private void CameraSetting_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Capture?.CameraSetting_ValueChanged(sender, e);
        }

        public CameraSetting GetParammeter()
        {
            return Capture.cameraSetting;
        }

        private void GetcameraSettingValue()
        {
            if (Capture.cameraSetting != null)
            {
                GetCameraProperties();
                slExporsure.Value = Capture.cameraSetting.Exposure;
                slBrightness.Value = Capture.cameraSetting.Brightness;
                slContrast.Value = Capture.cameraSetting.Contrast;
                slFocus.Value = Capture.cameraSetting.Focus;
                slWhite.Value = Capture.cameraSetting.WBTemperature;
                slSharpness.Value = Capture.cameraSetting.Sharpness;
                slZoom.Value = Capture.cameraSetting.Zoom;
            }
        }

        private void GetCameraProperties()
        {
            Capture.cameraSetting.Exposure = (int)Capture.videoCapture.Get(VideoCaptureProperties.Exposure);

            Capture.cameraSetting.Brightness = (int)Capture.videoCapture.Get(VideoCaptureProperties.Brightness);

            Capture.cameraSetting.Contrast = (int)Capture.videoCapture.Get(VideoCaptureProperties.Contrast);

            Capture.cameraSetting.Saturation = (int)Capture.videoCapture.Get(VideoCaptureProperties.Saturation);

            Capture.cameraSetting.WBTemperature = (int)Capture.videoCapture.Get(VideoCaptureProperties.WhiteBalanceBlueU);

            Capture.cameraSetting.Sharpness = (int)Capture.videoCapture.Get(VideoCaptureProperties.Sharpness);

            Capture.cameraSetting.Focus = (int)Capture.videoCapture.Get(VideoCaptureProperties.Focus);

            Capture.cameraSetting.Zoom = (int)Capture.videoCapture.Get(VideoCaptureProperties.Zoom);
        }

    }
}
