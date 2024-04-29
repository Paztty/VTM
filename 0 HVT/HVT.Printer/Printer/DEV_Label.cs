using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Text.Json;
using System.Text.Json.Serialization;
using HVT.Utility;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Controls;
using System.Windows.Controls;

namespace HVT.Printer
{
    public class DEV_Label : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public const string LabelConfigPath = "LabelConfig.cfg";

        [JsonIgnore]
        public LabelPreview Preview { get; set; } = new LabelPreview();
        public Viewbox viewbox = new Viewbox();

        /// <summary>
        /// Label size
        /// </summary>


        private int _HomeX;
        public int HomeX
        {
            get { return _HomeX; }
            set
            {
                if (value != _HomeX)
                {
                    _HomeX = value;
                    NotifyPropertyChanged("HomeX");
                }
            }
        }

        private int _HomeY;
        public int HomeY
        {
            get { return _HomeY; }
            set
            {
                if (value != _HomeY)
                {
                    _HomeY = value;
                    NotifyPropertyChanged("HomeY");
                }
            }
        }

        private int _Width = 300;
        public int Width
        {
            get { return _Width; }
            set
            {
                if (value != _Width)
                {
                    _Width = value;
                    NotifyPropertyChanged("Width");
                    NotifyPropertyChanged("RWidth");
                }
            }
        }

        private int _Height = 300;
        public int Height
        {
            get { return _Height; }
            set
            {
                if (value != _Height)
                {
                    _Height = value;
                    NotifyPropertyChanged("Height");
                    NotifyPropertyChanged("RHeight");
                }
            }
        }

        private int _Speed;
        public int Speed
        {
            get { return _Speed; }
            set
            {
                if (value != _Speed)
                {
                    _Speed = value;
                    NotifyPropertyChanged("Speed");
                }
            }
        }

        private int _Darkness;
        public int Darkness
        {
            get { return _Darkness; }
            set
            {
                if (value != _Darkness)
                {
                    _Darkness = value;
                    NotifyPropertyChanged("Darkness");
                }
            }
        }

        private int _Pad;
        public int Pad
        {
            get { return _Pad; }
            set
            {
                if (value != _Pad)
                {
                    _Pad = value;
                    NotifyPropertyChanged("Pad");
                    NotifyPropertyChanged("RWidth");
                    NotifyPropertyChanged("RHeight");
                }
            }
        }

        public int RWidth
        {
            get { return Width + HomeX * 2  + Pad; }
        }

        public int RHeight
        {
            get { return Height + HomeY * 2 + Pad; }
        }

        private int _NumberLabel = 1;
        public int NumberLabel
        {
            get { return _NumberLabel; }
            set
            {
                if (value != _NumberLabel)
                {
                    _NumberLabel = value;
                    NotifyPropertyChanged("NumberLabel");
                }
            }
        }


        private LabelTextItem _SN1 = new LabelTextItem();
        public LabelTextItem SN1
        {
            get { return _SN1; }
            set
            {
                if (value != null && value != _SN1)
                {
                    _SN1 = value;
                    NotifyPropertyChanged("SN1");
                }
            }
        }

        private LabelImageItem _QR = new LabelImageItem();
        public LabelImageItem QR
        {
            get { return _QR; }
            set
            {
                if (value != null && value != _QR)
                {
                    _QR = value;
                    NotifyPropertyChanged("QR");
                }
            }
        }

        private LabelTextItem _SN2 = new LabelTextItem();
        public LabelTextItem SN2
        {
            get { return _SN2; }
            set
            {
                if (value != null && value != _SN2)
                {
                    _SN2 = value;
                    NotifyPropertyChanged("SN2");
                }
            }
        }

        private LabelTextItem _detail1 = new LabelTextItem();
        public LabelTextItem detail1
        {
            get { return _detail1; }
            set
            {
                if (value != null && value != _detail1)
                {
                    _detail1 = value;
                    NotifyPropertyChanged("detail1");
                }
            }
        }

        private LabelTextItem _detail2 = new LabelTextItem();
        public LabelTextItem detail2
        {
            get { return _detail2; }
            set
            {
                if (value != null && value != _detail2)
                {
                    _detail2 = value;
                    NotifyPropertyChanged("detail2");
                }
            }
        }

        private LabelTextItem _ArrayText = new LabelTextItem() { Width = 30, Height = 30 };
        public LabelTextItem ArrayText
        {
            get { return _ArrayText; }
            set
            {
                if (value != null && value != _ArrayText)
                {
                    _ArrayText = value;
                    NotifyPropertyChanged("ArrayText");
                }
            }
        }



        public DEV_Label()
        {}

        public void Save()
        {
            Extensions.SaveToFile(this, LabelConfigPath);
        }

        public string MakeLabel(string SN1, string SN2, string QR, string detail1 = "", string detail2 = "", string array = "")
        {
            this.SN1.Content = SN1;
            this.SN2.Content = SN2;
            this.QR.ContentString = QR;
            this.detail1.Content = detail1;
            this.detail2.Content = detail2;
            this.ArrayText.Content = array;

            string str = "I8,A,001\n";
            str += "\n";
            str += "V00,8,L\n";
            str += "Q" + Width + "," + Pad + "\n";
            str += "q" + RWidth + "\n";
            str += "rN\n";
            str += "S" + Speed + "\n"; 
            str += "D" + Darkness + "\n";
            str += "ZT\n";
            str += "JF\n";
            str += "OC\n";
            str += "R" + HomeX + "," + HomeY + "\n";
            str += "f100\n";
            str += "N\n";

            Preview.DataContext = this;
            if (Preview.Parent == null)
            {
                viewbox.Child = Preview;
                viewbox.Measure(Preview.RenderSize);
                viewbox.Arrange(new Rect(new System.Windows.Point(0, 0), Preview.RenderSize));
                viewbox.UpdateLayout();
            }

            System.Windows.Point relativePoint = Preview.labelCanvasViewer.TransformToAncestor(Preview.CanvasRinbol)
              .Transform(new System.Windows.Point(0, 0));

            RenderTargetBitmap renderTargetBitmap =
                new RenderTargetBitmap((int)(RWidth + relativePoint.X), (int)(RHeight + relativePoint.Y), 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(Preview);
            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            encoder.Save(stream);
            System.Drawing.Bitmap bitmap = new Bitmap(stream);
            bitmap.Save("D:/bitmap.png");
            var realLabel = bitmap.Clone(new System.Drawing.Rectangle((int)relativePoint.X, (int)relativePoint.Y, Width, Height), bitmap.PixelFormat);


            str += BitmapToString(HomeX, HomeY, realLabel);
            str += "P" + NumberLabel + "\n";
            stream.Dispose();

            return str;
        }

        private static string BitmapToString(int top, int left, System.Drawing.Bitmap bitmap)
        {
            string data = "";
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms, Encoding.ASCII))
                {
                    //we set p3 parameter, remember it is Width of Graphic in bytes,
                    //so we divive the width of image and round up of it
                    int P3 = (int)Math.Ceiling((double)bitmap.Width / 8);
                    bw.Write(Encoding.ASCII.GetBytes(string.Format
                    ("GW{0},{1},{2},{3},", top, left, P3, bitmap.Height)));
                    //the width of matrix is rounded up multi of 8
                    int canvasWidth = P3 * 8;
                    //Now we convert image into 2 dimension binary matrix by 2 for loops below,
                    //in the range of image, we get colour of pixel of image,
                    //calculate the luminance in order to set value of 1 or 0
                    //otherwise we set value to 1
                    //Because P3 is set to byte (8 bits), so we gather 8 dots of this matrix,
                    //convert into a byte then write it to memory by using shift left operator <<
                    //e,g 1 << 7  ---> 10000000
                    //    1 << 6  ---> 01000000
                    //    1 << 3  ---> 00001000
                    for (int y = 0; y < bitmap.Height; ++y)     //loop from top to bottom
                    {
                        for (int x = 0; x < canvasWidth;)       //from left to right
                        {
                            byte abyte = 0;
                            for (int b = 0; b < 8; ++b, ++x)     //get 8 bits together and write to memory
                            {
                                int dot = 1;                     //set 1 for white,0 for black
                                                                 //pixel still in width of bitmap,
                                                                 //check luminance for white or black, out of bitmap set to white
                                if (x < bitmap.Width)
                                {
                                    System.Drawing.Color color = bitmap.GetPixel(x, y);
                                    int luminance = (int)((color.R * 0.3) + (color.G * 0.59) + (color.B * 0.11));
                                    dot = luminance > 127 ? 1 : 0;
                                }
                                abyte |= (byte)(dot << (7 - b)); //shift left,
                                                                 //then OR together to get 8 bits into a byte
                            }
                            bw.Write(abyte);
                        }
                    }
                    bw.Write("\n");
                    bw.Flush();
                    //reset memory
                    ms.Position = 0;
                    //get encoding, I have no idea why encode page of 1252 works and fails for others
                    data = Encoding.GetEncoding(1252).GetString(ms.ToArray());
                    ms.Dispose();
                    bw.Dispose();
                    bitmap.Dispose();
                }
            }
            return data;
        }

    }


    public class LabelTextItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private int _Top = 5;
        public int Top
        {
            get { return _Top; }
            set
            {
                if (value != _Top)
                {
                    _Top = value;
                    NotifyPropertyChanged("Top");
                }
            }
        }

        private int _Left = 5;
        public int Left
        {
            get { return _Left; }
            set
            {
                if (value != _Left)
                {
                    _Left = value;
                    NotifyPropertyChanged("Left");
                }
            }
        }

        private int _Width = 300;
        public int Width
        {
            get { return _Width; }
            set
            {
                if (value != _Width)
                {
                    _Width = value;
                    NotifyPropertyChanged("Width");
                }
            }
        }


        private int _Height = 300;
        public int Height
        {
            get { return _Height; }
            set
            {
                if (value != _Height)
                {
                    _Height = value;
                    NotifyPropertyChanged("Height");
                }
            }
        }


        private int _Size = 13;
        public int Size
        {
            get { return _Size; }
            set
            {
                if (value != _Size)
                {
                    _Size = value;
                    NotifyPropertyChanged("Size");
                }
            }
        }


        private bool _Print;
        public bool Print
        {
            get { return _Print; }
            set
            {
                if (value != _Print)
                {
                    _Print = value;
                    NotifyPropertyChanged("Print");
                    NotifyPropertyChanged("Visibility");
                }
            }
        }

        public Visibility Visibility
        {
            get { return Print ? Visibility.Visible : Visibility.Hidden; }
        }


        private string _Content;
        public string Content
        {
            get { return _Content; }
            set
            {
                if (value != null && value != _Content)
                {
                    _Content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }
    }

    public class LabelImageItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private int _Top;
        public int Top
        {
            get { return _Top; }
            set
            {
                if (value != _Top)
                {
                    _Top = value;
                    NotifyPropertyChanged("Top");
                }
            }
        }

        private int _Left;
        public int Left
        {
            get { return _Left; }
            set
            {
                if (value != _Left)
                {
                    _Left = value;
                    NotifyPropertyChanged("Left");
                }
            }
        }

        public int Width
        {
            get { return Size * 10; }
        }

        public int Height
        {
            get { return Size * 10; }
        }
        private int _Size = 3;
        public int Size
        {
            get { return _Size; }
            set
            {
                if (value != _Size)
                {
                    _Size = value;
                    NotifyPropertyChanged("Size");
                    NotifyPropertyChanged("Width");
                    NotifyPropertyChanged("Height");
                    NotifyPropertyChanged("Content");
                }
            }
        }


        private bool _Print;
        public bool Print
        {
            get { return _Print; }
            set
            {
                if (value != _Print)
                {
                    _Print = value;
                    NotifyPropertyChanged("Print");
                }
            }
        }

        [JsonIgnore]
        public BitmapImage Content
        {
            get
            {
                var image = GenerateQR(this.Width, this.Height, ContentString);
                return image;
            }
        }


        private string _ContentString = "QR LABEL";
        public string ContentString
        {
            get { return _ContentString; }
            set
            {
                if (value != null && value != _ContentString)
                {
                    _ContentString = value;
                    NotifyPropertyChanged("ContentString");
                    NotifyPropertyChanged("Content");
                }
            }
        }

        private BitmapImage GenerateQR(int width, int height, string text)
        {
            if (text == "")
            {
                text = "No content";
            }
            var bw = new ZXing.BarcodeWriter();
            var encOptions = new ZXing.Common.EncodingOptions() { Width = width, Height = height, Margin = 0 };
            bw.Options = encOptions;
            bw.Format = ZXing.BarcodeFormat.QR_CODE;
            var _bitmap = new Bitmap(bw.Write(text));
            MemoryStream ms = new MemoryStream();
            _bitmap.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            bi.Freeze();
            return bi;
        }

    }

}
