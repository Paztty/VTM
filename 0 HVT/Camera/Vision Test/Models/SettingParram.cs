using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.Models
{
    public class SettingParram
    {

        private double _Threshold;
        public double Threshold
        {
            get { return _Threshold; }
            set
            {
                if (value != _Threshold) _Threshold = value;
            }
        }


        private double _Blur;
        public double Blur
        {
            get { return _Blur; }
            set
            {
                if (value != _Blur) _Blur = value;
            }
        }


        private double _Noise;
        public double Noise
        {
            get { return _Noise; }
            set
            {
                if (value != _Noise) _Noise = value;
            }
        }


        private bool _IsPass;
        public bool IsPass
        {
            get { return _IsPass; }
            set
            {
                if (value != _IsPass) _IsPass = value;
            }
        }
    }
}
