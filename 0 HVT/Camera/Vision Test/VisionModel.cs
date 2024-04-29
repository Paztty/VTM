using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Camera
{
    public class VisionModel
    {

        private List<FND> _FNDs = new List<FND>();
        public List<FND> FNDs
        {
            get { return _FNDs; }
            set
            {
                if (value != null || value != _FNDs)
                {
                    _FNDs = value;
                    for (int i = 0; i < 4; i++)
                    {
                        FNDs[i].Sellected += VisionModel_Sellected;
                    }
                }
            }
        }


        private List<LCD> _LCDs = new List<LCD>();
        public List<LCD> LCDs
        {
            get { return _LCDs; }
            set
            {
                if (value != null || value != _LCDs)
                {
                    _LCDs = value;
                    for (int i = 0; i < 4; i++)
                    {
                        LCDs[i].Sellected += VisionModel_Sellected;
                    }
                }
            }
        }


        private List<LED> _LED = new List<LED>();
        public List<LED> LED
        {
            get { return _LED; }
            set
            {
                if (value != null || value != _LED) _LED = value;
            }
        }

        private List<GLED> _GLED = new List<GLED>();
        public List<GLED> GLED
        {
            get { return _GLED; }
            set
            {
                if (value != null || value != _GLED) _GLED = value;
            }
        }

        public VisionModelOption Option = new VisionModelOption();

        public VisionModel()
        {
            for (int i = 0; i < 4; i++)
            {
                FNDs.Add(new FND(i));
                FNDs[i].Sellected += VisionModel_Sellected;
                LCDs.Add(new LCD(i));
                LCDs[i].Sellected += VisionModel_Sellected;
                GLED.Add(new GLED(new System.Windows.Point(5, 5 + 25 * i)));
                LED.Add(new LED(new System.Windows.Point(5, 100 + 25 * i)));
            }
            Option.DataContext = FNDs[0];
        }

        private void VisionModel_Sellected(object sender, EventArgs e)
        {
            FND tryCatchFND = (sender as FND);
            if (tryCatchFND != null)
            {
                Option.DataContext = tryCatchFND;
            }
            LCD tryCatchLCD = (sender as LCD);
            if (tryCatchLCD != null)
            {
                Option.DataContext = tryCatchLCD;
            }
        }

        public void UpdateLayout(int PCB_Count)
        {
            for (int i = 0; i < 4; i++)
            {
                FNDs[i].Visibility = PCB_Count > i ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                LCDs[i].Visibility = PCB_Count > i ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                GLED[i].Visibility = PCB_Count > i ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                LED[i].Visibility = PCB_Count > i ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
        }

        public void GetFNDSampleImage(Mat mat)
        {

            foreach (var item in FNDs)
            {
                if (item.Visibility == Visibility.Visible)
                {
                    item.TestImage(mat);
                }
            }
        }

        public void GetLCDSampleImage(Mat mat)
        {
            if (LCDs.Count < 4)
            {
                return;
            }
            foreach (var item in LCDs)
            {
                if (item.Visibility == Visibility.Visible)
                {
                   if(!LCD.Processing)
                            item.TestImage(mat, item.SpectString);
                }
            }
        }

        public void GetGLEDSampleImage(Mat mat)
        {
            foreach (var item in GLED)
            {
                item.GetValue(mat);
            }
        }
        public void GetLEDSampleImage(Mat mat)
        {
            foreach (var item in LED)
            {
                item.GetValue(mat);
            }
        }

        public void GetGLEDSample()
        {
            foreach (var item in GLED)
            {
                item.GetValue();
            }
        }
        public void GetLEDSample()
        {
            foreach (var item in LED)
            {
                item.GetValue();
            }
        }
    }
}
