using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Tesseract;
using Rect = System.Windows.Rect;

namespace HVT.VTM.Base.VisionFunctions
{
    public class VisionWorker
    {
        public static Bitmap GetBitmap(BitmapSource source)
        {
            Bitmap bmp = new Bitmap(
              source.PixelWidth,
              source.PixelHeight,
              PixelFormat.Format24bppRgb);

            BitmapData data = bmp.LockBits(
              new Rectangle(System.Drawing.Point.Empty, bmp.Size),
              ImageLockMode.WriteOnly,
              PixelFormat.Format24bppRgb);

            source.CopyPixels(
              Int32Rect.Empty,
              data.Scan0,
              data.Height * data.Stride,
              data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }
        public static BitmapSource Convert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Rgb24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;

        }



        #region segement detecter


        public static Bitmap SeventSegmentDetect(Bitmap source, double threshold, out string detectedString)
        {
            detectedString = "";
            Mat mInput = source.ToMat();
            Mat gray = mInput.CvtColor(ColorConversionCodes.RGB2GRAY);


            gray.MinMaxIdx(out _, out double maxval);

            Mat edge = gray.Threshold(threshold, 255, ThresholdTypes.Binary);
            //Cv2.FastNlMeansDenoising(edge, edge, 0,4);
            Mat blurred = edge.GaussianBlur(new OpenCvSharp.Size(3, 9), 0, 3);
            Mat blurgreen = blurred.Threshold(maxval * 0.1 > 10 ? maxval * 0.1 : 10, 255, ThresholdTypes.Binary);
            OpenCvSharp.Point[][] contour;
            HierarchyIndex[] hierarchy;
            List<OpenCvSharp.Rect> digitContour = new List<OpenCvSharp.Rect>();

            // make corect region 
            Mat blurWhiteDown = blurgreen;
            Mat blurWhiteUp = blurgreen;
            //for (int i = 0; i < blurWhiteDown.Cols; i++)
            //{
            //    bool startWhite = false;
            //    for (int j = 0; j < blurWhiteDown.Rows; j++)
            //    {
            //        var colorVal = blurWhiteDown.At<>(i, j)
            //    }
            //}




            Cv2.FindContours(blurgreen, out contour, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            for (int i = 0; i < contour.Length; i++)
            {
                //mInput.DrawContours(contour, i, new Scalar(255, 0, 0));
                OpenCvSharp.Rect rect = Cv2.BoundingRect(contour[i]);
                if (rect.Width > 2 && rect.Height > 20 && rect.Height < 100)
                {
                    digitContour.Add(rect);
                    //blurgreen.Rectangle(rect, new Scalar(255, 0, 0));
                }
            }
            digitContour = digitContour.OrderBy(o => o.Left).ToList();
            Mat moutput = blurWhiteDown.CvtColor(ColorConversionCodes.GRAY2RGB);
            foreach (var item in digitContour)
            {
                //Console.Write(item.Left.ToString() + "->");
                var rect = item;
                detectedString += DetectSegmentChar(rect, new Mat(edge, item), moutput);
            }
            foreach (var item in digitContour)
            {
                moutput.Rectangle(item, new Scalar(255, 0, 0));
            }
            Bitmap output = BitmapConverter.ToBitmap(moutput, PixelFormat.Format24bppRgb);
            return output;
        }

        private static char DetectSegmentChar(OpenCvSharp.Rect rectg, Mat Matinput, Mat colorMat)
        {
            double threadShot = 50;
            //Matinput.MinMaxIdx(out _, out threadShot);
            //threadShot *= 0.5;
            Mat input = Matinput.Threshold(threadShot, 255, ThresholdTypes.Binary);
            SegementCharacter segement = new SegementCharacter();
            var W = input.Width;
            var H = input.Height;
            Mat matDigit;

            OpenCvSharp.Rect rect = new OpenCvSharp.Rect();


            if ((double)H / W > 3)
            {
                W = input.Width;
                H = input.Height;

                rect = new OpenCvSharp.Rect(0, 0, W, H / 2);
                matDigit = new Mat(input, rect);
                segement.digit[2] = (byte)(matDigit.Mean().Val0 > threadShot ? 1 : 0);

                rect = new OpenCvSharp.Rect(0, H / 2, W, H / 2);
                matDigit = new Mat(input, rect);
                segement.digit[5] = (byte)(matDigit.Mean().Val0 > threadShot ? 1 : 0);

            }
            else
            {
                input.Resize(new OpenCvSharp.Size(4 * W, 8 * H), 4, 8);
                W = input.Width;
                H = input.Height;

                rect = new OpenCvSharp.Rect(W / 4, 0, W / 2, H / 6);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[0] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }


                rect = new OpenCvSharp.Rect(0, H / 7, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[1] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(3 * W / 4, H / 7, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[2] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(W / 4, 2 * H / 5, W / 2, H / 5);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[3] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(0, 4 * H / 7, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[4] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(3 * W / 4, 4 * H / 7, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[5] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(W / 4, 4 * H / 5, W / 2, H / 5);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[6] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }
            }

            var output = new SegementCharacter();
            var returnVal = new SegementCharacter();
            returnVal.digit = segement.digit;
            foreach (var item in SEG_LOOKUP)
            {
                if (item.digitString == returnVal.digitString)
                {
                    returnVal = item;
                    break;
                }
            }
            return returnVal.character;
        }

        internal class SegementCharacter
        {
            public char character { get; set; } = ' ';
            private byte[] digits = new byte[7] { 0, 0, 0, 0, 0, 0, 0 };
            public byte[] digit
            {
                get { return digits; }
                set
                {
                    if (digits != value)
                    {
                        digits = value;
                        digitString = "";
                        foreach (byte b in digits)
                        {
                            digitString += b;
                        }

                    }
                }
            }
            public string digitString;
        }

        private static List<SegementCharacter> SEG_LOOKUP = new List<SegementCharacter>()
        {
            new SegementCharacter(){character = 'n', digit = new byte[7] {0,0,0,0,0,0,0}},
            new SegementCharacter(){character = '0', digit = new byte[7] {1,1,1,0,1,1,1}},
            new SegementCharacter(){character = '1', digit = new byte[7] {0,0,1,0,0,1,0}},
            new SegementCharacter(){character = '2', digit = new byte[7] {1,0,1,1,1,0,1}},
            new SegementCharacter(){character = '3', digit = new byte[7] {1,0,1,1,0,1,1}},
            new SegementCharacter(){character = '4', digit = new byte[7] {0,1,1,1,0,1,0}},
            new SegementCharacter(){character = '5', digit = new byte[7] {1,1,0,1,0,1,1}},
            new SegementCharacter(){character = '6', digit = new byte[7] {1,1,0,1,1,1,1}},
            new SegementCharacter(){character = '7', digit = new byte[7] {1,1,1,0,0,1,0}},
            new SegementCharacter(){character = '8', digit = new byte[7] {1,1,1,1,1,1,1}},
            new SegementCharacter(){character = '9', digit = new byte[7] {1,1,1,1,0,1,1}},
            new SegementCharacter(){character = 'A', digit = new byte[7] {1,1,1,1,1,1,0}},
            new SegementCharacter(){character = 'B', digit = new byte[7] {0,1,0,1,1,1,1}},
            new SegementCharacter(){character = 'C', digit = new byte[7] {1,1,0,0,1,0,1}},
            new SegementCharacter(){character = 'D', digit = new byte[7] {0,0,1,1,1,1,1}},
            new SegementCharacter(){character = 'E', digit = new byte[7] {1,1,0,1,1,0,1}},
            new SegementCharacter(){character = 'F', digit = new byte[7] {1,1,0,1,1,0,0}},
        };

        #endregion
        #region GLED
        public static int Meansure(Int32Rect rect, Bitmap bitmap, int Thresh)
        {
            Mat input = bitmap.ToMat();
            Mat crop = new Mat(input, new OpenCvSharp.Rect(rect.X, rect.Y, rect.Width, rect.Height));
            Mat gray = crop.CvtColor(ColorConversionCodes.RGB2GRAY);
            int whitePercent = (int)(gray.Mean().Val0);
            return whitePercent;
        }

        #endregion


        #region Character Detect
        public static TesseractEngine engine = new TesseractEngine(@"./TessData", "eng", EngineMode.TesseractAndLstm);

        public static Bitmap DetectString(Bitmap bitmap, out string str)
        {
            Bitmap output = (Bitmap)bitmap.Clone();

            Mat mat = bitmap.ToMat();
            Mat matInv = new Mat();
            Cv2.BitwiseNot(mat, matInv);
            matInv = matInv.Threshold(175, 255, ThresholdTypes.Binary);
            output = matInv.ToBitmap();

            var ocrtext = string.Empty;
            try
            {
                using (var img = PixConverter.ToPix(output))
                {
                    using (var page = engine.Process(img))
                    {
                        ocrtext = page.GetText();
                        var rects = page.GetSegmentedRegions(PageIteratorLevel.Word);
                        Graphics g = Graphics.FromImage(output);
                        foreach (var r in rects)
                            g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue), r);
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }

            str = ocrtext;
            return output;
        }


        //        Bitmap output = (Bitmap)bitmap.Clone();
        //        var ocrtext = string.Empty;
        //            try
        //            {
        //                using (var engine = new TesseractEngine(@"./TessData", "eng", EngineMode.Default))
        //                {
        //                    using (var img = PixConverter.ToPix(bitmap))
        //                    {
        //                        using (var page = engine.Process(img))
        //                        {
        //                            ocrtext = page.GetText();
        //                            var rects = page.GetSegmentedRegions(PageIteratorLevel.Block);
        //        Graphics g = Graphics.FromImage(output);
        //                            foreach (var r in rects)
        //                                g.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Blue), r);
        //                        }
        //}
        //                }
        //            }
        //            catch (Exception err)
        //{
        //    Console.WriteLine(err.Message);
        //}

        //str = ocrtext;
        //return output;
        #endregion
    }
}
