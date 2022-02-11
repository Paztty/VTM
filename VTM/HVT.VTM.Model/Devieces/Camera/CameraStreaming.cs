using ImageProcessor;
using ImageProcessor.Imaging;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{
    public sealed class CameraStreaming : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler ImageUpdate;

        private Bitmap lastFrame;
        public System.Drawing.Bitmap _lastFrame
        {
            get { return lastFrame; }
            set
            {
                lastFrame = value;
                NotifyPropertyChanged("_lastFrame");
            }
        }

        public  System.Windows.Media.Imaging.BitmapSource LastFrameBitmapImage;
        public  System.Windows.Media.Imaging.BitmapSource LastFrame;
        private Task _previewTask;

        private CancellationTokenSource _cancellationTokenSource;
        private readonly System.Windows.Controls.Image _imageControlForRendering;
        //private readonly System.Windows.Controls.Image _imageControlForCropRendering;

        public ConcurrentStack<System.Windows.Media.Imaging.BitmapSource> Camera_Frame_Collection = new ConcurrentStack<System.Windows.Media.Imaging.BitmapSource>();

        public System.Windows.Media.Imaging.BitmapSource lastFrameBitmapImage
        {
            get { return LastFrameBitmapImage; }
            set
            {
                LastFrameBitmapImage = value;
                if (value != null)
                        LastFrame = value;
            }
        }

        private readonly int _frameWidth;
        private readonly int _frameHeight;

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

        public CameraStreaming(
            System.Windows.Controls.Image imageControlForRendering,
            int frameWidth,
            int frameHeight,
            int cameraDeviceId)
        {
            _imageControlForRendering = imageControlForRendering;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            CameraDeviceId = cameraDeviceId;
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

                    videoCapture = new VideoCapture();


                    if (!videoCapture.Open(CameraDeviceId))
                    {
                        HVT.Utility.Debug.Write("Cannot connect to camera", Utility.Debug.ContentType.Error);
                    }
                    Console.WriteLine("Set frame width:" + videoCapture.Set(VideoCaptureProperties.FrameWidth, _frameWidth));
                    Console.WriteLine("Set frame height:" + videoCapture.Set(VideoCaptureProperties.FrameHeight, _frameHeight));
                    Console.WriteLine("frame width:" + videoCapture.Get(VideoCaptureProperties.FrameWidth));
                    Console.WriteLine("frame height:" + videoCapture.Get(VideoCaptureProperties.FrameHeight));
                    Console.WriteLine("Set FPS" + videoCapture.Set(VideoCaptureProperties.Fps, 30));
                    Console.WriteLine("FPS " + videoCapture.Get(VideoCaptureProperties.Fps));
                    //Console.WriteLine("Set Brightness" + videoCapture.Set(VideoCaptureProperties.Brightness, 100));
                    //Console.WriteLine("Set Exposure" + videoCapture.Set(VideoCaptureProperties.Exposure, -5));
                    //Console.WriteLine("Set Sharpness" + videoCapture.Set(VideoCaptureProperties.Sharpness, 100));
                    //Console.WriteLine("Set Contrast" + videoCapture.Set(VideoCaptureProperties.Contrast, 500));
                    using (Mat frame = new Mat())
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            //var timeStart = DateTime.Now;
                            videoCapture.Read(frame);
                            //Console.WriteLine(videoCapture.FrameCount);
                            if (!frame.Empty())
                            {
                                // Releases the lock on first not empty frame
                                //if (initializationSemaphore != null)
                                //    initializationSemaphore.Release();
                                //_lastCropFrame = CropBitmap(_lastFrame);

                                var frameSource = frame.ToBitmapSource();
                                //System.Windows.Media.Imaging.BitmapSource lastFrameCropBitmapImage = _lastCropFrame.ToBitmapSource();
                                frameSource.Freeze();
                                //lastFrameCropBitmapImage.Freeze();
                                _imageControlForRendering.Dispatcher.Invoke(() => _imageControlForRendering.Source = frameSource);
                                //_imageControlForCropRendering.Dispatcher.Invoke(() => _imageControlForCropRendering.Source = lastFrameCropBitmapImage);
                                 Camera_Frame_Collection.Push(frameSource);
                                ImageUpdate?.Invoke(null, null);
                            }
                            // 30 FPS
                            await Task.Delay(15);
                            //Console.WriteLine(DateTime.Now - timeStart);
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

            if (_lastFrame != null)
            {
                using (var imageFactory = new ImageFactory())
                using (var stream = new MemoryStream())
                {
                    imageFactory
                        .Load(_lastFrame)
                        .Resize(new ResizeLayer(
                            size: new System.Drawing.Size(_frameWidth, _frameHeight),
                            resizeMode: ResizeMode.Crop,
                            anchorPosition: AnchorPosition.Center))
                        .Save(stream);
                    LastPngFrame = stream.ToArray();
                }
            }
            else
            {
                LastPngFrame = null;
            }
        }

        public Bitmap Capture()
        {
            return _lastFrame.Clone(new Rectangle(0, 0, _frameWidth, _frameHeight), _lastFrame.PixelFormat);
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
                    return (int)videoCapture.Get(VideoCaptureProperties.Exposure);
                case VideoProperties.Brightness:
                    return (int)videoCapture.Get(VideoCaptureProperties.Brightness);
                case VideoProperties.Contrast:
                    return (int)videoCapture.Get(VideoCaptureProperties.Contrast);
                case VideoProperties.Satuation:
                    return (int)videoCapture.Get(VideoCaptureProperties.Saturation);
                case VideoProperties.WhiteBalance:
                    return (int)videoCapture.Get(VideoCaptureProperties.WBTemperature);
                case VideoProperties.Sharpness:
                    return (int)videoCapture.Get(VideoCaptureProperties.Sharpness);
                case VideoProperties.Focus:
                    return (int)videoCapture.Get(VideoCaptureProperties.Focus);
                case VideoProperties.Zoom:
                    return (int)videoCapture.Get(VideoCaptureProperties.Zoom);
                default:
                    return 0;
            }
        }

        public bool SetParammeter(CameraSetting cameraSetting)
        {
            if (cameraSetting != null)
            {

                videoCapture.Set(VideoCaptureProperties.Exposure, cameraSetting.Exposure);

                videoCapture.Set(VideoCaptureProperties.Brightness, cameraSetting.Brightness);

                videoCapture.Set(VideoCaptureProperties.Contrast, cameraSetting.Contrast);

                videoCapture.Set(VideoCaptureProperties.Saturation, cameraSetting.Saturation);

                videoCapture.Set(VideoCaptureProperties.WhiteBalanceBlueU, cameraSetting.WBTemperature);

                videoCapture.Set(VideoCaptureProperties.Sharpness, cameraSetting.Sharpness);

                videoCapture.Set(VideoCaptureProperties.Focus, cameraSetting.Focus);

                videoCapture.Set(VideoCaptureProperties.Zoom, cameraSetting.Zoom);
            }
            return true;
        }

        public CameraSetting GetParammeter()
        {
            return cameraSetting;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _lastFrame?.Dispose();
        }
    }
}
