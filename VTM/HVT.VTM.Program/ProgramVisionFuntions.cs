using HVT.Utility;
using HVT.VTM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HVT.VTM.Program
{
    public partial class Program
    {
        public enum BoardSelected
        {
            All,
            A,
            B,
            C,
            D
        }
        public BoardSelected boardSelected;
        public Canvas DrawingCanvas, DisplayCanvas, ManualDisplayCanvas;

        public System.Windows.Controls.Image FND1, FND2, FND3, FND4;
        public System.Windows.Controls.Image LCD1, LCD2, LCD3, LCD4;
        public System.Windows.Controls.Label lbFND1, lbFND2, lbFND3, lbFND4;
        public System.Windows.Controls.Label GLEDsValue, LEDsValue;

        public System.Windows.Controls.Label lbLCD1, lbLCD2, lbLCD3, lbLCD4;

        private Timer UpdateVisionTest = new Timer()
        {
            Interval = 50,
        };

        public void VisionInit(
                                Canvas DrawingFunction,
                                Canvas DisplayFunction,
                                Canvas ManualDisplayFunction,

                                DockPanel LCDA,
                                DockPanel LCDB,
                                DockPanel LCDC,
                                DockPanel LCDD,

                                DockPanel FNDA,
                                DockPanel FNDB,
                                DockPanel FNDC,
                                DockPanel FNDD,

                                Label GLEDsValue,
                                Label LEDsValue
                                )
        {


            this.GLEDsValue = GLEDsValue;
            this.LEDsValue = LEDsValue;

            DrawingCanvas = DrawingFunction;

            DrawingCanvas.MouseDown += DrawingCanvas_MouseDown;
            DrawingCanvas.MouseMove += DrawingCanvas_MouseMove;
            DrawingCanvas.MouseUp += DrawingCanvas_MouseUp;

            DisplayCanvas = DisplayFunction;
            ManualDisplayCanvas = ManualDisplayFunction;

            RootModel.VisionTestInit(
                DrawingCanvas,
                DisplayCanvas,
                ManualDisplayCanvas,
                LCDA,
                LCDB,
                LCDC,
                LCDD,

                FNDA,
                FNDB,
                FNDC,
                FNDD
                );

            UpdateVisionTest.Elapsed += UpdateVisionTest_Elapsed;
            UpdateVisionTest.AutoReset = true;
            UpdateVisionTest.Start();
        }

        private void UpdateVisionTest_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (cameraStreaming == null)
            {
                return;
            }
            if (cameraStreaming.Camera_Frame_Collection.TryPop(out BitmapSource imageCatched))
            {

                if (updateLCDDataTask == null || updateLCDDataTask.Status != TaskStatus.Running)
                {
                    updateLCDDataTask = new Task(new Action(delegate { UpdateLCDData(imageCatched); }));
                    updateLCDDataTask.Start();
                }

                if (updateFNDDataTask == null || updateFNDDataTask.Status != TaskStatus.Running)
                {
                    updateFNDDataTask = new Task(new Action(delegate { UpdateFNDData(imageCatched); }));
                    updateFNDDataTask.Start();
                }

                /////////////////////
                if (updateGLEDDataTask == null || updateGLEDDataTask.Status != TaskStatus.Running)
                {
                    updateGLEDDataTask = new Task(new Action(delegate { UpdateGLEDData(imageCatched); }));
                    updateGLEDDataTask.Start();
                }
                /////////////////////
                if (updateLEDDataTask == null || updateLEDDataTask.Status != TaskStatus.Running)
                {
                    updateLEDDataTask = new Task(new Action(delegate { UpdateLEDData(imageCatched); }));
                    updateLEDDataTask.Start();
                }

            }

            cameraStreaming.Camera_Frame_Collection.Clear();
        }

        private void DrawingCanvas_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void DrawingCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.Handled == false)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var focusElement = Keyboard.FocusedElement;
                    if (focusElement != null && focusElement.GetType() == typeof(Label) && (e.Source as FrameworkElement) == DrawingCanvas)
                    {
                        Console.WriteLine(sender.ToString() + " + " + focusElement.ToString());
                        focusElement.RaiseEvent(e);
                        Console.WriteLine("Fire event to focus Element");
                        //Canvas.SetTop((Label)focusElement, e.GetPosition(DrawingCanvas).Y);
                        //Canvas.SetRight((Label)focusElement, e.GetPosition(DrawingCanvas).X);
                    }
                }
            }

        }

        private void DrawingCanvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                var focusElement = Keyboard.FocusedElement;
                if (focusElement != null) Console.WriteLine("Remover focus from " + Keyboard.FocusedElement.ToString());
                Keyboard.ClearFocus();
                FocusManager.SetFocusedElement(DrawingCanvas, null);
            }
        }




        private double[] GetConvertRaito(BitmapSource image, Canvas placeHolder)
        {
            if (placeHolder != null)
            {
                return new double[] {
                image.Width/placeHolder.ActualWidth, image.Height/placeHolder.ActualHeight,
            };
            }
            else
            {
                return new double[] { 0, 0 };
            }


        }


        public void Calc_GLED_Thresh()
        {
            switch (boardSelected)
            {
                case BoardSelected.All:
                    foreach (var item in RootModel.GLEDs)
                    {
                        item.CALC_THRESH();
                    }
                    break;
                case BoardSelected.A:
                    RootModel.GLEDs[0].CALC_THRESH();
                    break;
                case BoardSelected.B:
                    RootModel.GLEDs[1].CALC_THRESH();
                    break;
                case BoardSelected.C:
                    RootModel.GLEDs[2].CALC_THRESH();
                    break;
                case BoardSelected.D:
                    RootModel.GLEDs[3].CALC_THRESH();
                    break;
                default:
                    break;
            }
        }

        private Task updateFNDDataTask;
        private Task updateLCDDataTask;
        private Task updateGLEDDataTask;
        private Task updateLEDDataTask;

        public bool IsVisionWorking = false;

        public void StartUpdateLCD()
        {
            //if (updateLCDDataTask == null || updateLCDDataTask.Status != TaskStatus.Running)
            //{
            //    updateLCDDataTask = new Task(new Action(delegate { UpdateLCDData((sender as BitmapSource)); }));
            //    updateLCDDataTask.Start();
            //}
        }

        public void StartUpdateFND()
        {
            //if (updateFNDDataTask == null || updateFNDDataTask.Status != TaskStatus.Running)
            //{
            //    updateFNDDataTask = new Task(new Action(delegate { UpdateFNDData((sender as BitmapSource)); }));
            //    updateFNDDataTask.Start();
            //}
        }

        private void UpdateLCDData(BitmapSource source)
        {
            if (source != null)
            {
                var newimage = source;
                var raito = GetConvertRaito(newimage, DrawingCanvas);
                foreach (var item in RootModel.LCDs)
                {
                    item.raito = raito;
                }
                for (int i = 0; i < RootModel.contruction.PCB_Count; i++)
                {
                    CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[i].Area);
                    RootModel.LCDs[i].GetImage(croppedBitmap);
                }
            }
        }
        private void UpdateFNDData(BitmapSource source)
        {
            if (source == null)
            {
                return;
            }
            var newimage = source;
            var raito = GetConvertRaito(newimage, DrawingCanvas);
            foreach (var item in RootModel.FNDs)
            {
                item.raito = raito;
            }

            for (int i = 0; i < RootModel.contruction.PCB_Count; i++)
            {
                CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[i].Area);
                RootModel.FNDs[i].GetImage(croppedBitmap);
            }

        }
        private void UpdateGLEDData(BitmapSource source)
        {
            var newimage = source;
            var raito = GetConvertRaito(newimage, DrawingCanvas);
            foreach (var item in RootModel.GLEDs)
            {
                foreach (var item2 in item.GLEDs)
                    item2.raito = raito;
            }
            switch (boardSelected)
            {
                case BoardSelected.All:
                    foreach (var item in RootModel.GLEDs)
                    {
                        item.GetValue(newimage, raito);
                    }
                    break;
                case BoardSelected.A:
                    RootModel.GLEDs[0].GetValue(newimage, raito);
                    GLEDsValue.Dispatcher.Invoke(() =>
                    {
                        GLEDsValue.Content = RootModel.GLEDs[0].CalculatorOutputString;
                    });
                    break;
                case BoardSelected.B:
                    RootModel.GLEDs[1].GetValue(newimage, raito);
                    GLEDsValue.Dispatcher.Invoke(() =>
                    {
                        GLEDsValue.Content = RootModel.GLEDs[1].CalculatorOutputString;
                    });
                    break;
                case BoardSelected.C:
                    RootModel.GLEDs[2].GetValue(newimage, raito);
                    GLEDsValue.Dispatcher.Invoke(() =>
                    {
                        GLEDsValue.Content = RootModel.GLEDs[2].CalculatorOutputString;
                    });
                    break;
                case BoardSelected.D:

                    RootModel.GLEDs[3].GetValue(newimage, raito);
                    GLEDsValue.Dispatcher.Invoke(() =>
                    {
                        GLEDsValue.Content = RootModel.GLEDs[3].CalculatorOutputString;
                    });
                    break;
                default:
                    break;
            }
        }
        private void UpdateLEDData(BitmapSource source)
        {
            var newimage = source;
            var raito = GetConvertRaito(newimage, DrawingCanvas);
            foreach (var item in RootModel.LEDs)
            {
                foreach (var value in item.LEDs)
                    value.raito = raito;
            }
            switch (boardSelected)
            {
                case BoardSelected.All:
                    foreach (var item in RootModel.LEDs)
                    {
                        item.GetValue(newimage, raito);
                    }
                    break;
                case BoardSelected.A:
                    RootModel.LEDs[0].GetValue(newimage, raito);
                    LEDsValue.Dispatcher.Invoke(() =>
                    {
                        LEDsValue.Content = RootModel.LEDs[0].CalculatorOutputString;
                    });
                    break;
                case BoardSelected.B:
                    RootModel.LEDs[1].GetValue(newimage, raito);
                    LEDsValue.Dispatcher.Invoke(() =>
                    {
                        LEDsValue.Content = RootModel.LEDs[1].CalculatorOutputString;
                    });
                    break;
                case BoardSelected.C:
                    RootModel.LEDs[2].GetValue(newimage, raito);
                    LEDsValue.Dispatcher.Invoke(() =>
                    {
                        LEDsValue.Content = RootModel.LEDs[2].CalculatorOutputString;
                    });
                    break;
                case BoardSelected.D:

                    RootModel.LEDs[3].GetValue(newimage, raito);
                    LEDsValue.Dispatcher.Invoke(() =>
                    {
                        LEDsValue.Content = RootModel.LEDs[3].CalculatorOutputString;
                    });
                    break;
                default:
                    break;
            }
        }

        public event EventHandler CameraPropertiesChange;
        public void CameraPropertiesChanged()
        {
            CameraPropertiesChange?.Invoke(this, EventArgs.Empty);
        }

        private void CameraStreaming_ImageUpdate(object sender, EventArgs e)
        {
            if (!RootModel.HaveApplyCamsetting)
            {
                Debug.Write("Camera Start.....", Debug.ContentType.Notify);
                cameraStreaming.SetParammeter(RootModel.CameraSetting);
                CameraPropertiesChanged();
                RootModel.HaveApplyCamsetting = true;
            }


        }

        //cameraStreaming.Camera_Frame_Collection.Clear();
    }

}

