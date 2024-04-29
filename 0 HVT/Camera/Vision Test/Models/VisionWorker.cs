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
using OpenCvSharp.WpfExtensions;
using Rect = System.Windows.Rect;
using OpenCvSharp.Extensions;

namespace Camera
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
        public static Mat SeventSegmentDetect(Mat source,int noiseSize, double threshold, out string detectedString)
        {
            detectedString = "";
            Mat mInput = source;
            Mat gray = mInput.CvtColor(ColorConversionCodes.RGB2GRAY);


            gray.MinMaxIdx(out _, out double maxval);

            Mat edge = gray.Threshold(threshold, 255, ThresholdTypes.Binary);
            //Cv2.FastNlMeansDenoising(edge, edge, 0,4);
            Mat blurred = edge.GaussianBlur(new OpenCvSharp.Size(1, 21), 0, 3);
            Mat blurgreen = blurred.Threshold(maxval * 0.1 > 10 ? maxval * 0.1 : 10, 255, ThresholdTypes.Binary);
            OpenCvSharp.Point[][] contour;
            HierarchyIndex[] hierarchy;
            List<OpenCvSharp.Rect> digitContour = new List<OpenCvSharp.Rect>();

            // make corect region 
            Mat blurWhiteDown = blurgreen;
            Mat blurWhiteUp = blurgreen;

            Mat moutput = edge.CvtColor(ColorConversionCodes.GRAY2RGB);

            Cv2.FindContours(blurgreen, out contour, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            for (int i = 0; i < contour.Length; i++)
            {
                //mInput.DrawContours(contour, i, new Scalar(255, 0, 0));
                OpenCvSharp.Rect rect = Cv2.BoundingRect(contour[i]);
                if (Cv2.ContourArea(contour[i]) > noiseSize*noiseSize)
                {
                    digitContour.Add(rect);
                }
                else
                {
                    moutput.DrawContours(contour, i, new Scalar(0,0,255));
                }
            }
            digitContour = digitContour.OrderBy(o => o.Left).ToList();
            foreach (var item in digitContour)
            {
                //Console.Write(item.Left.ToString() + "->");
                var rect = item;
                detectedString += DetectSegmentChar(rect, new Mat(edge, item) , moutput, 3);
            }
            return moutput;
        }

        public static Mat SeventSegmentDetect(Mat source,double brightness, double blur, int noiseSize, double threshold, out string detectedString)
        {
            detectedString = "";

            if (source.Height < 50 || source.Width < 50)
            {
                return new Mat();
            }

            Mat mInput = source;
            Mat gray = mInput.CvtColor(ColorConversionCodes.RGB2GRAY);


            gray.MinMaxIdx(out _, out double maxval);

            Mat edge = gray.Threshold(brightness, 255, ThresholdTypes.Binary);
            //Cv2.FastNlMeansDenoising(edge, edge, 0,4);
            Mat blurred = edge.GaussianBlur(new OpenCvSharp.Size(1, 21), 0, blur);
            Mat blurgreen = blurred.Threshold(50, 255, ThresholdTypes.Binary);
            OpenCvSharp.Point[][] contour;
            HierarchyIndex[] hierarchy;
            List<OpenCvSharp.Rect> digitContour = new List<OpenCvSharp.Rect>();

            // make corect region 
            Mat blurWhiteDown = blurgreen;
            Mat blurWhiteUp = blurgreen;

            Mat moutput = edge.CvtColor(ColorConversionCodes.GRAY2RGB);

            Cv2.FindContours(blurgreen, out contour, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            for (int i = 0; i < contour.Length; i++)
            {
                //mInput.DrawContours(contour, i, new Scalar(255, 0, 0));
                OpenCvSharp.Rect rect = Cv2.BoundingRect(contour[i]);
                if (Cv2.ContourArea(contour[i]) > noiseSize*noiseSize && rect.Height >(blur*2 + 2))
                {
                    digitContour.Add(rect);
                }
                else
                {
                    moutput.DrawContours(contour, i, new Scalar(0, 0, 255));
                }
            }
            digitContour = digitContour.OrderBy(o => o.Left).ToList();
            foreach (var item in digitContour)
            {
                //Console.Write(item.Left.ToString() + "->");
                var rect = item;
                detectedString += DetectSegmentChar(rect, new Mat(edge, item), moutput, (int)blur);
            }
            return moutput;
        }

        public static void SeventSegmentDetect(BitmapSource source, double threshold, out string detectedString, out BitmapSource outSource)
        {
            detectedString = "";
            Mat mInput = source.ToMat();
            Mat gray = mInput.CvtColor(ColorConversionCodes.RGB2GRAY);


            gray.MinMaxIdx(out _, out double maxval);

            Mat edge = gray.Threshold(threshold, 255, ThresholdTypes.Binary);
            //Cv2.FastNlMeansDenoising(edge, edge, 0,4);
            Mat blurred = edge.GaussianBlur(new OpenCvSharp.Size(1, 21), 0, 3);
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
                detectedString += DetectSegmentChar(rect, new Mat(edge, item), moutput, 3);
            }
            foreach (var item in digitContour)
            {
                moutput.Rectangle(item, new Scalar(255, 0, 0));
            }
            outSource = moutput.ToBitmapSource();
        }

        public static Mat SeventSegmentDetect(Mat source, double threshold, out string detectedString, string Format = "7700702777")
        {
            detectedString = "";
            Mat mInput = source;
            Mat gray = mInput.CvtColor(ColorConversionCodes.RGB2GRAY);


            gray.MinMaxIdx(out _, out double maxval);

            Mat edge = gray.Threshold(threshold, 255, ThresholdTypes.Binary);
            //Cv2.FastNlMeansDenoising(edge, edge, 0,4);
            Mat blurred = edge.GaussianBlur(new OpenCvSharp.Size(1,21), 0, 3);
            Mat blurgreen = blurred.Threshold(maxval * 0.1 > 10 ? maxval * 0.1 : 10, 255, ThresholdTypes.Binary);
            OpenCvSharp.Point[][] contour;
            HierarchyIndex[] hierarchy;
            List<OpenCvSharp.Rect> digitContour = new List<OpenCvSharp.Rect>();
            Cv2.FindContours(blurgreen, out contour, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            // make corect region 
            Mat blurWhiteDown = blurgreen;
            Mat blurWhiteUp = blurgreen;

            double totalWidth = source.Width - 10;
            double lagerWidthNumber =  Format.Count(c => c == '7' || c == '0');
            double smallWidthNumber =  Format.Count(c => c == '2' || c == ':') + lagerWidthNumber * 3;
            double smallWidth = totalWidth / smallWidthNumber;
            double lagerWidth = smallWidth * 3;
            double height = source.Height;
            double widthSpace = 0;
            foreach (var character in Format)
            {
                switch (character)
                {
                    case '2':
                        digitContour.Add(new OpenCvSharp.Rect(new OpenCvSharp.Point(widthSpace, 5), new OpenCvSharp.Size(smallWidth, height - 10)));
                        widthSpace += smallWidth;
                        break;
                    case ':':
                        digitContour.Add(new OpenCvSharp.Rect(new OpenCvSharp.Point(widthSpace, 5), new OpenCvSharp.Size(smallWidth, height - 10)));
                        widthSpace += smallWidth;
                        break;
                    case '7':
                        digitContour.Add(new OpenCvSharp.Rect(new OpenCvSharp.Point(widthSpace, 5), new OpenCvSharp.Size(lagerWidth, height - 10)));
                        widthSpace += lagerWidth;
                        break;
                    case '0':
                        digitContour.Add(new OpenCvSharp.Rect(new OpenCvSharp.Point(widthSpace, 5), new OpenCvSharp.Size(lagerWidth, height - 10)));
                        widthSpace += lagerWidth;
                        break;
                    default:
                        break;
                }
            }
            Mat moutput = blurWhiteDown.CvtColor(ColorConversionCodes.GRAY2RGB);
            for (int i = 0; i < Format.Length; i++)
            {
                switch (Format[i])
                {
                    case '2':
                        detectedString += DetectSegmentChar(digitContour[i], new Mat(edge, digitContour[i]), moutput, 3);
                        break;
                    case ':':
                        if (DetectSegmentChar(digitContour[i], new Mat(edge, digitContour[i]), moutput, 3, true) == '1')
                        {
                            detectedString += ':';
                        }
                        break;
                    case '7':
                        detectedString += DetectSegmentChar(digitContour[i], new Mat(edge, digitContour[i]), moutput, 3);
                        break;
                    case '0':
                        detectedString += " ";
                        break;
                    default:
                        break;
                }
            }
            foreach (var item in digitContour)
            {
                moutput.Rectangle(item, new Scalar(255, 0, 0));
            }

            Console.WriteLine(detectedString);
            return moutput;
        }


        private static char DetectSegmentChar(OpenCvSharp.Rect rectg, Mat Matinput, Mat colorMat,int blurOffset, bool IsDoubleDot = false)
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

            if (IsDoubleDot)
            {
                W = input.Width;
                H = input.Height;

                rect = new OpenCvSharp.Rect(0, 0, W, H / 2);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > 10)
                {
                    segement.digit[2] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(0, H / 2, W, H / 2);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > 10)
                {
                    segement.digit[5] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }
            }

            else if ((double)H / W > 3)
            {
                W = input.Width;
                H = input.Height;

                rect = new OpenCvSharp.Rect(0, 0, W, H / 2);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[2] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(0, H / 2, W, H / 2);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[5] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }
            }
            else
            {
                input.Resize(new OpenCvSharp.Size(4 * W, 8 * H), 4, 8);
                W = input.Width;
                H = input.Height - blurOffset * 2;

                rect = new OpenCvSharp.Rect(W / 4, 3, W / 2, H / 6);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[0] = 1;
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                    //colorMat.PutText("1", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                }


                rect = new OpenCvSharp.Rect(0, H / 7 + blurOffset, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[1] = 1;
                   // colorMat.PutText("2", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(3 * W / 4, H / 7 + blurOffset, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[2] = 1;
                    //colorMat.PutText("3", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(W / 4, H/2 - H / 12 + blurOffset, W / 2, H / 6);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[3] = 1;
                    //colorMat.PutText("4", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(0, 4 * H / 7 + blurOffset, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[4] = 1;
                    //colorMat.PutText("5", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(3 * W / 4, 4 * H / 7 + blurOffset, W / 4, H / 3);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[5] = 1;
                    //colorMat.PutText("6", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }

                rect = new OpenCvSharp.Rect(W / 4, H - H / 6 + blurOffset, W / 2, H / 6);
                matDigit = new Mat(input, rect);
                if (matDigit.Mean().Val0 > threadShot)
                {
                    segement.digit[6] = 1;
                    //colorMat.PutText("7", new OpenCvSharp.Point(rect.X, rect.Y), HersheyFonts.HersheySimplex, 1, new Scalar(255, 0, 0));
                    colorMat.Rectangle(new OpenCvSharp.Rect(rectg.X + rect.X, rectg.Y + rect.Y, rect.Width, rect.Height), new Scalar(255, 0, 0));
                }
            }

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
            new SegementCharacter(){character = '7', digit = new byte[7] {1,0,1,0,0,1,0}},
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
        public static int Meansure(Mat input)
        {
            Mat gray = input.CvtColor(ColorConversionCodes.RGB2GRAY);
            int whitePercent = (int)gray.Mean().Val0;
            return whitePercent;
        }

        #endregion
    }
}
