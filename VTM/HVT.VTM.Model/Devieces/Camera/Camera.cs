
namespace HVT.VTM.Base
{
    public class CameraDevice
    {
        public int OpenCvId { get; set; }
        public string Name { get; set; }
        public string DeviceId { get; set; }
    }

    public class CameraSetting
    {
        private int _brightness = 0;
        public int Brightness { get { return _brightness; } set { _brightness = value; } }
        
        private int _contrast = 0;
        public int Contrast { get { return _contrast; } set { _contrast = value; } }
        
        private int _saturation = 0;
        public int Saturation { get { return _saturation; } set { _saturation = value; } }

        private int _exposure = -5;
        public int Exposure { get { return _exposure; } set { _exposure = value; } }

        private int _zoom = 0;
        public int Zoom { get { return _zoom; } set { _zoom = value; } }

        private int _backlight = 0;
        public int Backlight { get { return _backlight; } set { _backlight = value; } }

        private int _focus = 0;
        public int Focus { get { return _focus; } set { _focus = value; } }

        private int _sharpness = 0; 
        public int Sharpness { get { return _sharpness; } set { _sharpness = value; } }

        private int _wbTemperature = 0;
        public int WBTemperature { get { return _wbTemperature; } set { _wbTemperature = value; } }

    }
}
