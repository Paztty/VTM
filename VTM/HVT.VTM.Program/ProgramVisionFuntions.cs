using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void VisionInit(
                                Canvas DrawingFunction,
                                Canvas DisplayFunction,
                                Canvas ManualDisplayFunction,

                                System.Windows.Controls.Image FND1,
                                System.Windows.Controls.Image FND2,
                                System.Windows.Controls.Image FND3,
                                System.Windows.Controls.Image FND4,

                                System.Windows.Controls.Image LCD1,
                                System.Windows.Controls.Image LCD2,
                                System.Windows.Controls.Image LCD3,
                                System.Windows.Controls.Image LCD4,

                                Label FNDdetected1,
                                Label FNDdetected2,
                                Label FNDdetected3,
                                Label FNDdetected4,

                                Label LCDdetected1,
                                Label LCDdetected2,
                                Label LCDdetected3,
                                Label LCDdetected4,

                                Label GLEDsValue,
                                Label LEDsValue
                                )
        {
            this.FND1 = FND1;
            this.FND2 = FND2;
            this.FND3 = FND3;
            this.FND4 = FND4;

            this.LCD1 = LCD1;
            this.LCD2 = LCD2;
            this.LCD3 = LCD3;
            this.LCD4 = LCD4;

            this.lbFND1 = FNDdetected1;
            this.lbFND2 = FNDdetected2;
            this.lbFND3 = FNDdetected3;
            this.lbFND4 = FNDdetected4;


            this.lbLCD1 = LCDdetected1;
            this.lbLCD2 = LCDdetected2;
            this.lbLCD3 = LCDdetected3;
            this.lbLCD4 = LCDdetected4;

            this.GLEDsValue = GLEDsValue;
            this.LEDsValue = LEDsValue;

            DrawingCanvas = DrawingFunction;

            DrawingCanvas.MouseDown += DrawingCanvas_MouseDown;
            DrawingCanvas.MouseMove += DrawingCanvas_MouseMove;
            DrawingCanvas.MouseUp += DrawingCanvas_MouseUp;

            DisplayCanvas = DisplayFunction;
            ManualDisplayCanvas = ManualDisplayFunction;

            RootModel.VisionTestInit(DrawingCanvas, DisplayCanvas, ManualDisplayCanvas);

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
            if (updateLCDDataTask == null)
            {
                updateLCDDataTask = new Task(new Action(delegate { UpdateLCDData(cameraStreaming.LastFrame); }));
                updateLCDDataTask.Start();
            }
            else if (updateLCDDataTask.IsCompleted)
            {
                updateLCDDataTask = new Task(new Action(delegate { UpdateLCDData(cameraStreaming.LastFrame); }));
                updateLCDDataTask.Start();
            }
        }

        public void StartUpdateFND()
        {
            if (updateFNDDataTask == null)
            {
                updateFNDDataTask = new Task(new Action(delegate { UpdateFNDData(cameraStreaming.LastFrame); }));
                updateFNDDataTask.Start();
            }
            else if (updateFNDDataTask.IsCompleted)
            {
                updateFNDDataTask = new Task(new Action(delegate { UpdateFNDData(cameraStreaming.LastFrame); }));
                updateFNDDataTask.Start();
            }
        }

        public void UpdateLCDData(BitmapSource source)
        {
            if (source != null)
            {
                var newimage = source;
                var raito = GetConvertRaito(newimage, DrawingCanvas);
                foreach (var item in RootModel.LCDs)
                {
                    item.raito = raito;
                }
                if (IsVisionWorking)
                {
                    switch (boardSelected)
                    {
                        case BoardSelected.All:
                            RootModel.LCDs[0].GetImage(newimage);
                            LCD1.Dispatcher.Invoke(() =>
                            {
                                LCD1.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new Action(delegate {
                                        LCD1.Source = RootModel.LCDs[0].DetectedImage;
                                    }));

                                lbLCD1.Content = RootModel.LCDs[0].DetectString;
                            });

                            RootModel.LCDs[1].GetImage(newimage);
                            LCD2.Dispatcher.Invoke(() =>
                            {
                                LCD2.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new Action(delegate {
                                        LCD2.Source = RootModel.LCDs[1].DetectedImage;
                                    }));
                                lbLCD2.Content = RootModel.LCDs[1].DetectString;
                            });

                            RootModel.LCDs[2].GetImage(newimage);
                            LCD3.Dispatcher.Invoke(() =>
                            {
                                LCD3.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new Action(delegate {
                                        LCD3.Source = RootModel.LCDs[2].DetectedImage;
                                    }));
                                lbLCD3.Content = RootModel.LCDs[2].DetectString;
                            });

                            RootModel.LCDs[3].GetImage(newimage);
                            LCD4.Dispatcher.Invoke(() =>
                            {
                                LCD4.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                    new Action(delegate {
                                        LCD4.Source = RootModel.LCDs[3].DetectedImage;
                                    }));
                                lbLCD4.Content = RootModel.LCDs[3].DetectString;
                            });

                            //CroppedBitmap croppedBitmap1 = new CroppedBitmap(newimage, RootModel.LCDs[0].Area);
                            //RootModel.LCDs[0].GetImage(croppedBitmap1);
                            //RootModel.LCDs[0].SetImage(LCD1, lbLCD1);

                            //LCD2.Dispatcher.Invoke(() =>
                            //{
                            //    CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[1].Area);
                            //    RootModel.LCDs[1].GetImage(croppedBitmap);
                            //    RootModel.LCDs[1].SetImage(LCD2, lbLCD2);
                            //});

                            //LCD3.Dispatcher.Invoke(() =>
                            //{
                            //    CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[2].Area);
                            //    RootModel.LCDs[2].GetImage(croppedBitmap);
                            //    RootModel.LCDs[2].SetImage(LCD3, lbLCD3);
                            //});

                            //LCD4.Dispatcher.Invoke(() =>
                            //{
                            //    CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[3].Area);
                            //    RootModel.LCDs[3].GetImage(croppedBitmap);
                            //    RootModel.LCDs[3].SetImage(LCD4, lbLCD4);
                            //});
                            break;
                        case BoardSelected.A:
                            LCD1.Dispatcher.Invoke(() =>
                            {
                                CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[0].Area);
                                RootModel.LCDs[0].GetImage(croppedBitmap);
                                RootModel.LCDs[0].SetImage(LCD1, lbLCD1);
                            });
                            break;
                        case BoardSelected.B:
                            LCD2.Dispatcher.Invoke(() =>
                            {
                                CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[1].Area);
                                RootModel.LCDs[1].GetImage(croppedBitmap);
                                RootModel.LCDs[1].SetImage(LCD2, lbLCD2);
                            });
                            break;
                        case BoardSelected.C:
                            LCD3.Dispatcher.Invoke(() =>
                            {
                                CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[2].Area);
                                RootModel.LCDs[2].GetImage(croppedBitmap);
                                RootModel.LCDs[2].SetImage(LCD3, lbLCD3);
                            });
                            break;
                        case BoardSelected.D:
                            LCD4.Dispatcher.Invoke(() =>
                            {
                                CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[3].Area);
                                RootModel.LCDs[3].GetImage(croppedBitmap);
                            });
                            RootModel.LCDs[3].SetImage(LCD4, lbLCD4);
                            break;
                        default:
                            break;
                    }
                }
                else
                {

                    LCD1.Dispatcher.Invoke(() =>
                    {
                        CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[0].Area);
                        RootModel.LCDs[0].GetImage(croppedBitmap);
                    });
                    RootModel.LCDs[0].GetValue();
                    LCD2.Dispatcher.Invoke(() =>
                    {
                        CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[1].Area);
                        RootModel.LCDs[1].GetImage(croppedBitmap);
                    });
                    RootModel.LCDs[1].GetValue();
                    LCD3.Dispatcher.Invoke(() =>
                    {
                        CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[2].Area);
                        RootModel.LCDs[2].GetImage(croppedBitmap);
                    });
                    RootModel.LCDs[2].GetValue();

                    LCD4.Dispatcher.Invoke(() =>
                    {
                        CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.LCDs[3].Area);
                        RootModel.LCDs[3].GetImage(croppedBitmap);
                    });
                    RootModel.LCDs[3].GetValue();
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

            if (IsVisionWorking)
            {
                switch (boardSelected)
                {
                    case BoardSelected.All:
                        FND1.Dispatcher.Invoke(() =>
                        {
                            CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[0].Area);
                            RootModel.FNDs[0].GetImage(croppedBitmap);
                            RootModel.FNDs[0].SetImage(FND1, lbFND1);
                        });

                        FND2.Dispatcher.Invoke(() =>
                        {
                            CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[1].Area);
                            RootModel.FNDs[1].GetImage(croppedBitmap);
                            RootModel.FNDs[1].SetImage(FND2, lbFND2);
                        });

                        FND3.Dispatcher.Invoke(() =>
                        {
                            CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[2].Area);
                            RootModel.FNDs[2].GetImage(croppedBitmap);
                            RootModel.FNDs[2].SetImage(FND3, lbFND3);
                        });

                        FND4.Dispatcher.Invoke(() =>
                        {
                            CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[3].Area);
                            RootModel.FNDs[3].GetImage(croppedBitmap);
                            RootModel.FNDs[3].SetImage(FND4, lbFND4);
                        });
                        break;
                    case BoardSelected.A:
                        FND1.Dispatcher.Invoke(() =>
                        {
                            CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[0].Area);
                            RootModel.FNDs[0].GetImage(croppedBitmap);
                            RootModel.FNDs[0].SetImage(FND1, lbFND1);
                        });
                        break;
                    case BoardSelected.B:
                        FND2.Dispatcher.Invoke(() =>
                        {
                            CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[1].Area);
                            RootModel.FNDs[1].GetImage(croppedBitmap);
                            RootModel.FNDs[1].SetImage(FND2, lbFND2);
                        });
                        break;
                    case BoardSelected.C:
                        FND3.Dispatcher.Invoke(() =>
                        {
                            CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[2].Area);
                            RootModel.FNDs[2].GetImage(croppedBitmap);
                            RootModel.FNDs[2].SetImage(FND3, lbFND3);
                        });
                        break;
                    case BoardSelected.D:
                        FND4.Dispatcher.Invoke(() =>
                        {
                            CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[3].Area);
                            RootModel.FNDs[3].GetImage(croppedBitmap);
                            RootModel.FNDs[3].SetImage(FND4, lbFND4);
                        });
                        break;
                    default:
                        break;
                }
            }
            else
            {
                FND1.Dispatcher.Invoke(() =>
                {
                    CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[0].Area);
                    RootModel.FNDs[0].GetImage(croppedBitmap);
                });
                RootModel.FNDs[0].GetValue();

                FND2.Dispatcher.Invoke(() =>
                {
                    CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[1].Area);
                    RootModel.FNDs[1].GetImage(croppedBitmap);
                });
                RootModel.FNDs[1].GetValue();

                FND3.Dispatcher.Invoke(() =>
                {
                    CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[2].Area);
                    RootModel.FNDs[2].GetImage(croppedBitmap);
                });
                RootModel.FNDs[2].GetValue();

                FND4.Dispatcher.Invoke(() =>
                {
                    CroppedBitmap croppedBitmap = new CroppedBitmap(newimage, RootModel.FNDs[3].Area);
                    RootModel.FNDs[3].GetImage(croppedBitmap);
                });
                RootModel.FNDs[3].GetValue();

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
                cameraStreaming.SetParammeter(RootModel.CameraSetting);
                CameraPropertiesChanged();
                RootModel.HaveApplyCamsetting = true;
            }

            if (updateLCDDataTask == null)
            {
                updateLCDDataTask = new Task(new Action(delegate { UpdateLCDData(sender as BitmapSource); }));
                updateLCDDataTask.Start();
            }
            else if (updateLCDDataTask.IsCompleted)
            {
                updateLCDDataTask = new Task(new Action(delegate { UpdateLCDData(sender as BitmapSource); }));
                updateLCDDataTask.Start();
            }

            if (updateFNDDataTask == null)
            {
                updateFNDDataTask = new Task(new Action(delegate { UpdateFNDData(sender as BitmapSource); }));
                updateFNDDataTask.Start();
            }
            else if (updateFNDDataTask.Status != TaskStatus.Running)
            {
                updateFNDDataTask = new Task(new Action(delegate { UpdateFNDData(sender as BitmapSource); }));
                updateFNDDataTask.Start();
            }
            /////////////////////
            if (updateGLEDDataTask == null)
            {
                updateGLEDDataTask = new Task(new Action(delegate { UpdateGLEDData(sender as BitmapSource); }));
                updateGLEDDataTask.Start();
            }
            else if (updateGLEDDataTask.Status != TaskStatus.Running)
            {
                updateGLEDDataTask = new Task(new Action(delegate { UpdateGLEDData(sender as BitmapSource); }));
                updateGLEDDataTask.Start();
            }
            /////////////////////
            if (updateLEDDataTask == null)
            {
                updateLEDDataTask = new Task(new Action(delegate { UpdateLEDData(sender as BitmapSource); }));
                updateLEDDataTask.Start();
            }
            else if (updateLEDDataTask.Status != TaskStatus.Running)
            {
                updateLEDDataTask = new Task(new Action(delegate { UpdateLEDData(sender as BitmapSource); }));
                updateLEDDataTask.Start();
            }
        }

        public BitmapSource CaptureCanvasLayout(Canvas canvas)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(canvas);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(canvas);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);
            BitmapFrame bmp = BitmapFrame.Create(rtb);
            return bmp;
        }
    }
}
