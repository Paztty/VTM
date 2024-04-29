using DirectShowLib;
using HVT.Utility;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

namespace Camera
{
    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    public partial class CameraControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler CamerasCollectionEmplty;
        private Task _previewTask;

        private CancellationTokenSource _cancellationTokenSource;


        private Mat _LastMatFrame;
        public Mat LastMatFrame
        {
            get { return _LastMatFrame; }
            set
            {
                if (value != null || value != _LastMatFrame)
                    _LastMatFrame = value;
            }
        }

        private BitmapSource lastFrame;

        public BitmapSource LastFrame
        {
            get
            {
                return lastFrame;
            }
            set
            {
                lastFrame = value;
                NotifyPropertyChanged(nameof(LastFrame));
            }
        }

        public int CameraDeviceId { get; private set; }
        public byte[] LastPngFrame { get; private set; }

        public CameraSetting cameraSetting = new CameraSetting();

        public VideoCapture videoCapture = new VideoCapture();
        public enum VideoProperties
        {
            Exposure,
            Brightness,
            Contrast,
            Satuation,
            WhiteBalance,
            Sharpness,
            Focus,
            Zoom,
            Reset,
        }
        public CameraControl()
        {
            InitializeComponent();
            this.DataContext = this;
            List<CameraDevice> cameras = new List<CameraDevice>();
            try
            {
                cameras = CameraDevicesEnumerator.GetAllConnectedCameras();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return;
            }
            if (cameras.Count >= 1)
            {
                var selectedCameraDeviceId = cameras[0].OpenCvId;
                if (CameraDeviceId != selectedCameraDeviceId)
                {
                    CameraDeviceId = cameras[0].OpenCvId;
                }
            }
            else
            {
                CamerasCollectionEmplty?.Invoke(null, null);
                CameraDetail.Content = "No camera detectd !";
                Debug.Write("Camera : No camera detected, check your camera device and restart this software.", Debug.ContentType.Error, 20);
            }

        }

        public void START()
        {
            try
            {
                Task.Run(Start);
            }
            catch (Exception)
            {

            }
        }


        public async Task Start()
        {
            // Never run two parallel tasks for the webcam streaming
            if (_previewTask != null && !_previewTask.IsCompleted)
                return;

            //var initializationSemaphore = new SemaphoreSlim(0, 1);

            _cancellationTokenSource = new CancellationTokenSource();
            _previewTask = Task.Run(async () =>
            {
                try
                {
                    // Creation and disposal of this object should be done in the same thread 
                    // because if not it throws disconnectedContext exception
                    videoCapture = new VideoCapture(CameraDeviceId);
                    if (!videoCapture.Open(CameraDeviceId))
                    {
                        Console.WriteLine("XXXXXXXXXXXXXXX");
                    }
                    try
                    {
                        videoCapture.FrameWidth=1920;
                        videoCapture.FrameHeight=1080;
                        videoCapture.Fps = 30;
                        videoCapture.AutoExposure = -5;
                        videoCapture.AutoFocus = false;
                        videoCapture.BufferSize = 1;
                        videoCapture.FourCC = "MJPG";
                        GetCameraProperties();
                    }
                    catch (Exception err)
                    {
                        string defaltFrame = videoCapture.Get(VideoCaptureProperties.FrameWidth) + " x " + videoCapture.Get(VideoCaptureProperties.FrameHeight);
                        MessageBox.Show("Camera start error!" + err.Message + "\n" + err.StackTrace + "\n Defaul resolution: " + defaltFrame, "Camera start error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    //GetCameraProperties();
                    using (Mat frame = new Mat())
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            if (videoCapture.Read(frame))
                            {
                                if (!frame.Empty())
                                {
                                    LastMatFrame = frame.Clone();
                                    var bi = frame.ToBitmapSource();
                                    bi.Freeze();
                                    LastFrame = bi;
                                }
                            }
                            await Task.Delay(30); // millisecond
                        }
                    }
                    videoCapture?.Dispose();
                }
                finally
                {
                    //if (initializationSemaphore != null)
                    //    initializationSemaphore.Release();
                }

            }, _cancellationTokenSource.Token);

            // Async initialization to have the possibility to show an animated loader without freezing the GUI
            // The alternative was the long polling. (while !variable) await Task.Delay
            //await initializationSemaphore.WaitAsync();
            //initializationSemaphore.Dispose();
            //initializationSemaphore = null;

            if (_previewTask.IsFaulted)
            {
                // To let the exceptions exit
                await _previewTask;
            }
        }

        public async Task Stop()
        {
            // If "Dispose" gets called before Stop
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            if (!_previewTask.IsCompleted)
            {
                _cancellationTokenSource.Cancel();

                // Wait for it, to avoid conflicts with read/write of _lastFrame
                await _previewTask;
            }
        }

        private Task setCamTask;
        public void CameraSetting_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (setCamTask != null && setCamTask.Status == TaskStatus.Running)
            {
                return;
            }
            string paramSettup = (sender as Slider).Name;
            switch (paramSettup)
            {
                case "slExporsure":
                    setCamTask = Task.Run(() => { SetParammeter(VideoProperties.Exposure, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slBrightness":
                    setCamTask = Task.Run(() => { SetParammeter(VideoProperties.Brightness, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slContrast":
                    setCamTask = Task.Run(() => { SetParammeter(VideoProperties.Contrast, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slFocus":
                    setCamTask = Task.Run(() => { SetParammeter(VideoProperties.Focus, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slWhite":
                    setCamTask = Task.Run(() => { SetParammeter(VideoProperties.WhiteBalance, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slSharpness":
                    setCamTask = Task.Run(() => { SetParammeter(VideoProperties.Sharpness, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slZoom":
                    setCamTask = Task.Run(() => { SetParammeter(VideoProperties.Zoom, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                case "slSatuation":
                    setCamTask = Task.Run(() => { SetParammeter(VideoProperties.Satuation, (int)e.NewValue); return Task.CompletedTask; });
                    break;
                default:
                    break;
            }
        }

        public void SetParammeter(VideoProperties properties, int Value, bool InTest)
        {
            if (videoCapture == null)
            {
                return;
            }
            switch (properties)
            {
                case VideoProperties.Exposure:
                    videoCapture.Set(VideoCaptureProperties.Exposure, Value);
                    break;
                case VideoProperties.Brightness:
                    videoCapture.Set(VideoCaptureProperties.Brightness, Value);
                    break;
                case VideoProperties.Contrast:
                    videoCapture.Set(VideoCaptureProperties.Contrast, Value);
                    break;
                case VideoProperties.Satuation:
                    videoCapture.Set(VideoCaptureProperties.Saturation, Value);
                    break;
                case VideoProperties.WhiteBalance:
                    //videoCapture.Set(VideoCaptureProperties.WhiteBalanceBlueU, Value);
                    videoCapture.Set(VideoCaptureProperties.WBTemperature, Value);
                    break;
                case VideoProperties.Sharpness:
                    videoCapture.Set(VideoCaptureProperties.Sharpness, Value);
                    break;
                case VideoProperties.Focus:
                    videoCapture.Set(VideoCaptureProperties.Focus, Value);
                    break;
                case VideoProperties.Zoom:
                    videoCapture.Set(VideoCaptureProperties.Zoom, Value);
                    break;
                default:
                    break;
            }
        }

        public void SetParammeter(VideoProperties properties, int Value)
        {
            if (videoCapture == null)
            {
                return;
            }
            switch (properties)
            {
                case VideoProperties.Exposure:
                    videoCapture.Set(VideoCaptureProperties.Exposure, Value);
                    cameraSetting.Exposure = Value;
                    break;
                case VideoProperties.Brightness:
                    videoCapture.Set(VideoCaptureProperties.Brightness, Value);
                    cameraSetting.Brightness = Value;
                    break;
                case VideoProperties.Contrast:
                    videoCapture.Set(VideoCaptureProperties.Contrast, Value);
                    cameraSetting.Contrast = Value;
                    break;
                case VideoProperties.Satuation:
                    videoCapture.Set(VideoCaptureProperties.Saturation, Value);
                    cameraSetting.Saturation = Value;
                    break;
                case VideoProperties.WhiteBalance:
                    videoCapture.Set(VideoCaptureProperties.WBTemperature, Value);
                    cameraSetting.WBTemperature = Value;
                    break;
                case VideoProperties.Sharpness:
                    videoCapture.Set(VideoCaptureProperties.Sharpness, Value);
                    cameraSetting.Sharpness = Value;
                    break;
                case VideoProperties.Focus:
                    videoCapture.Set(VideoCaptureProperties.Focus, Value);
                    cameraSetting.Focus = Value;
                    break;
                case VideoProperties.Zoom:
                    videoCapture.Set(VideoCaptureProperties.Zoom, Value);
                    cameraSetting.Zoom = Value;
                    break;
                default:
                    break;
            }
        }

        public int GetParammeter(VideoProperties properties)
        {
            if (videoCapture == null)
            {
                return 0;
            }
            switch (properties)
            {
                case VideoProperties.Exposure:
                    return (int)videoCapture.Exposure;
                case VideoProperties.Brightness:
                    return (int)videoCapture.Brightness;
                case VideoProperties.Contrast:
                    return (int)videoCapture.Contrast;
                case VideoProperties.Satuation:
                    return (int)videoCapture.Saturation;
                case VideoProperties.WhiteBalance:
                    return (int)videoCapture.WhiteBalanceBlueU;
                case VideoProperties.Sharpness:
                    return (int)videoCapture.Sharpness;
                case VideoProperties.Focus:
                    return (int)videoCapture.Focus;
                case VideoProperties.Zoom:
                    return (int)videoCapture.Zoom;
                default:
                    return 0;
            }
        }

        public bool SetParammeter(CameraSetting cameraSetting)
        {
            if (cameraSetting != null)
            {
                videoCapture.Exposure= cameraSetting.Exposure;

                videoCapture.Brightness=cameraSetting.Brightness;

                videoCapture.Contrast=cameraSetting.Contrast;

                videoCapture.Saturation= cameraSetting.Saturation;

                videoCapture.WhiteBalanceBlueU= cameraSetting.WBTemperature;

                videoCapture.Sharpness= cameraSetting.Sharpness;

                videoCapture.Focus= cameraSetting.Focus;

                videoCapture.Zoom= cameraSetting.Zoom;
            }
            return true;
        }

        public CameraSetting GetParammeter()
        {
            return cameraSetting;
        }

        private void GetCameraProperties()
        {
            cameraSetting.Exposure = (int)videoCapture.Exposure;

            cameraSetting.Brightness = (int)videoCapture.Brightness;

            cameraSetting.Contrast = (int)videoCapture.Contrast;

            cameraSetting.Saturation = (int)videoCapture.Saturation;

            cameraSetting.WBTemperature = (int)videoCapture.WhiteBalanceBlueU;

            cameraSetting.Sharpness = (int)videoCapture.Sharpness;

            cameraSetting.Focus = (int)videoCapture.Focus;

            cameraSetting.Zoom = (int)videoCapture.Zoom;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }

    }

    public class CameraDevice
    {
        public int OpenCvId { get; set; }
        public string Name { get; set; }
        public string DeviceId { get; set; }
    }

    public class CameraSetting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _brightness = 0;
        public int Brightness
        {
            get { return _brightness; }
            set {
                if (_brightness != value)
                {
                    _brightness = value;
                    NotifyPropertyChanged(nameof(Brightness));
                }
            }
        }

        private int _contrast = 0;
        public int Contrast { get { return _contrast; }
            set
            {
                if (_contrast != value)
                {
                    _contrast = value;
                    NotifyPropertyChanged(nameof(Contrast));
                }
            }
        }


        private int _saturation = 0;
        public int Saturation { get { return _saturation; }
            set
            {
                if (_saturation != value)
                {
                   _saturation = value;
                    NotifyPropertyChanged(nameof(Saturation));
                }
            }
        }

        private int _exposure = -5;
        public int Exposure { get { return _exposure; }
            set
            {
                if (_exposure != value)
                {
                   _exposure = value;
                    NotifyPropertyChanged(nameof(Exposure));
                }
            }
        }

        private int _zoom = 0;
        public int Zoom { get { return _zoom; }
            set
            {
                if (_zoom != value)
                {
                    _zoom = value;
                    NotifyPropertyChanged(nameof(Zoom));
                }
            }
        }

        private int _backlight = 0;
        public int Backlight { get { return _backlight; }
            set
            {
                if (_backlight != value)
                {
                    _backlight = value;
                    NotifyPropertyChanged(nameof(Backlight));
                }
            }
        }

        private int _focus = 0;
        public int Focus { get { return _focus; }
            set
            {
                if (_focus != value)
                {
                    _focus = value;
                    NotifyPropertyChanged(nameof(Focus));
                }
            }
        }

        private int _sharpness = 0;
        public int Sharpness { get { return _sharpness; }
            set
            {
                if (_sharpness != value)
                {
                    _sharpness = value;
                    NotifyPropertyChanged(nameof(Sharpness));
                }
            }
        }

        private int _wbTemperature = 0;
        public int WBTemperature { get { return _wbTemperature; }
            set
            {
                if (_wbTemperature != value)
                {
                    _wbTemperature = value;
                    NotifyPropertyChanged(nameof(WBTemperature));
                }
            }
        }
    }

    public static class CameraDevicesEnumerator
    {
        public static List<CameraDevice> GetAllConnectedCameras()
        {
            var cameras = new List<CameraDevice>();
            var videoInputDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            int openCvId = 0;
            return videoInputDevices.Select(v => new CameraDevice()
            {
                DeviceId = v.DevicePath,
                Name = v.Name,
                OpenCvId = openCvId++
            }).ToList();
        }
    }
}
