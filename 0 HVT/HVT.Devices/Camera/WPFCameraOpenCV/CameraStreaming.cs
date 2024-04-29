using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DirectShowLib;
using System.Threading;
using System.Windows.Controls;
using System.Drawing;
using Tesseract;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using OpenCvSharp.Extensions;
using ImageProcessor;
using System.IO;
using ImageProcessor.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HVT.VTM.Base
{
    public sealed class CameraStreaming : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Bitmap lastFrame;
        public System.Drawing.Bitmap _lastFrame {
            get { return lastFrame; }
            set {
                lastFrame = value;
                NotifyPropertyChanged("_lastFrame");
            }
        }

        private System.Drawing.Bitmap _lastCropFrame;
        private Task _previewTask;

        private CancellationTokenSource _cancellationTokenSource;
        private readonly System.Windows.Controls.Image _imageControlForRendering;
        private readonly System.Windows.Controls.Image _imageControlForCropRendering;

        System.Windows.Media.Imaging.BitmapSource lastFrameBitmapImage
        {
            get { return lastFrameBitmapImage; }
            set {
                lastFrameBitmapImage = value;
                NotifyPropertyChanged();
            }
        }

        private readonly int _frameWidth;
        private readonly int _frameHeight;



        public int CameraDeviceId { get; private set; }
        public byte[] LastPngFrame { get; private set; }

        public VideoCapture videoCapture = new VideoCapture();
        public enum VideoProperties
        { 
            Exposure,
            Brightness,
            Contrast,
            Satuation,
            WhiteBalance,
            Sharpness,
            Focus
        
        
        }

        public CameraStreaming(
            System.Windows.Controls.Image imageControlForRendering,
            System.Windows.Controls.Image imageControlForCrop,
            int frameWidth,
            int frameHeight,
            int cameraDeviceId)
        {
            _imageControlForRendering = imageControlForRendering;
            _imageControlForCropRendering = imageControlForCrop;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            CameraDeviceId = cameraDeviceId;
        }

        public async Task Start()
        {
            // Never run two parallel tasks for the webcam streaming
            if (_previewTask != null && !_previewTask.IsCompleted)
                return;

            var initializationSemaphore = new SemaphoreSlim(0, 1);

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
                        throw new ApplicationException("Cannot connect to camera");
                    }
                    Console.WriteLine("Set frame width:" + videoCapture.Set(VideoCaptureProperties.FrameWidth, _frameWidth));
                    Console.WriteLine("Set frame height:" + videoCapture.Set(VideoCaptureProperties.FrameHeight, _frameHeight));
                    //Console.WriteLine("Set FPS" + videoCapture.Set(VideoCaptureProperties.Fps, 60));
                    Console.WriteLine("Set Brightness" + videoCapture.Set(VideoCaptureProperties.Brightness, 100));
                    Console.WriteLine("Set Exposure" + videoCapture.Set(VideoCaptureProperties.Exposure, -5));
                    //Console.WriteLine("Set Sharpness" + videoCapture.Set(VideoCaptureProperties.Sharpness, 100));
                    //Console.WriteLine("Set Contrast" + videoCapture.Set(VideoCaptureProperties.Contrast, 500));
                    using (Mat frame = new Mat())
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            videoCapture.Read(frame);
                            //Console.WriteLine(videoCapture.FrameCount);
                            if (!frame.Empty())
                            {
                                // Releases the lock on first not empty frame
                                if (initializationSemaphore != null)
                                    initializationSemaphore.Release();
                                _lastFrame = BitmapConverter.ToBitmap(frame);
                                _lastCropFrame = CropBitmap(_lastFrame);
                               // Console.WriteLine(OCR(_lastCropFrame));
                                System.Windows.Media.Imaging.BitmapSource lastFrameBitmapImage = _lastFrame.ToBitmapSource();
                                System.Windows.Media.Imaging.BitmapSource lastFrameCropBitmapImage = _lastCropFrame.ToBitmapSource();
                                lastFrameBitmapImage.Freeze();
                                lastFrameCropBitmapImage.Freeze();
                                _imageControlForRendering.Dispatcher.Invoke(() => _imageControlForRendering.Source = lastFrameBitmapImage);
                                _imageControlForCropRendering.Dispatcher.Invoke(() => _imageControlForCropRendering.Source = lastFrameCropBitmapImage);
                            }

                            // 30 FPS
                            //await Task.Delay();
                        }
                    }
                    Console.WriteLine("Set frame width:" + videoCapture.Get(VideoCaptureProperties.FrameWidth));
                    Console.WriteLine("Set frame height:" + videoCapture.Get(VideoCaptureProperties.FrameHeight));
                    Console.WriteLine("Set FPS" + videoCapture.Get(VideoCaptureProperties.Fps));

                    videoCapture?.Dispose();
                }
                finally
                {
                    if (initializationSemaphore != null)
                        initializationSemaphore.Release();
                }

            }, _cancellationTokenSource.Token);

            // Async initialization to have the possibility to show an animated loader without freezing the GUI
            // The alternative was the long polling. (while !variable) await Task.Delay
            await initializationSemaphore.WaitAsync();
            initializationSemaphore.Dispose();
            initializationSemaphore = null;

            if (_previewTask.IsFaulted)
            {
                // To let the exceptions exit
                await _previewTask;
            }
        }

        private Bitmap CropBitmap(Bitmap b)
        {
            ImageProcessor.Imaging.CropLayer crop = new CropLayer(780, 160, 250, 60, CropMode.Pixels);
            

            Bitmap src = new Bitmap(b, b.Width, b.Height);
            Rectangle cropRect = new Rectangle(780, 155, 250, 55);
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height, b.PixelFormat);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new RectangleF(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            Mat mat = target.ToMat();
            Mat returnmat = new Mat();
            mat = mat.CvtColor(ColorConversionCodes.RGB2GRAY);

            mat = mat.Blur(new OpenCvSharp.Size(1, 1));
            //mat = mat.Threshold(110, 255, ThresholdTypes.Binary);
            //mat = mat.AdaptiveThreshold(255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.Binary, 31, -11);
            mat = mat.CvtColor(ColorConversionCodes.GRAY2RGB);
            return mat.ToBitmap(System.Drawing.Imaging.PixelFormat.Format24bppRgb);
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
            return _lastFrame.Clone(new Rectangle(0, 0, _frameWidth,_frameHeight), _lastFrame.PixelFormat);
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
                    videoCapture.Set(VideoCaptureProperties.WhiteBalanceBlueU, Value);
                    break;
                case VideoProperties.Sharpness:
                    videoCapture.Set(VideoCaptureProperties.Sharpness, Value);
                    break;
                case VideoProperties.Focus:
                    videoCapture.Set(VideoCaptureProperties.Focus, Value);
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
                default:
                    return 0;
            }
        }


        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _lastFrame?.Dispose();
        }

    }
}
