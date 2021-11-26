using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using Tesseract;


namespace HVT.VTM.Base
{
    public enum VisionFunctionCode
    {
        LED,
        GLED,
        FND,
    }
    public class VisionFunction
    {
        public string Name { get; set; }
        public VisionFunctionCode FunctionCode { get; set; }
        public Rectangle region { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string Result;

        public string Runfunction()
        {
            switch (FunctionCode)
            {
                case VisionFunctionCode.LED:
                    break;
                case VisionFunctionCode.GLED:
                    break;
                case VisionFunctionCode.FND:
                    break;
                default:
                    break;
            }
            return "";
        }

        private string GLED(Bitmap b)
        {
            //Bitmap segementRect = CropBitmap(b);
            string res = "";

            using (var engine = new TesseractEngine(@"TessData", "eng", EngineMode.Default))
            {
                using (var page = engine.Process(b, PageSegMode.AutoOnly))
                    res = page.GetText();
            }
            return res;
        }


        private string FND(Bitmap b)
        {
            Bitmap segementRect = CropBitmap(b);
            string res = "";

            using (var engine = new TesseractEngine(@"TessData", "eng", EngineMode.Default))
            {
                using (var page = engine.Process(b, PageSegMode.AutoOnly))
                    res = page.GetText();
                if (res.Contains(":"))
                {
                    res = res.Replace(':', '1');
                }
            }
            return res;
        }

        private string LED(Bitmap b)
        {
            //Bitmap segementRect = CropBitmap(b);
            string res = "";

            using (var engine = new TesseractEngine(@"TessData", "eng", EngineMode.Default))
            {
                using (var page = engine.Process(b, PageSegMode.AutoOnly))
                    res = page.GetText();
            }
            return res;
        }

        private Bitmap CropBitmap(Bitmap b)
        {
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
    }


}
