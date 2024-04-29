using HVT.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Drawing;
using System.Security.Policy;

namespace HVT.Printer
{
    public class GT800_Printer
    {
        public const string PrinterConfigPath = "Printer.cfg";

        #region Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }
        public static bool SendMemoryToPrinter(string szPrinterName, MemoryStream ms)
        {
            BinaryReader br = new BinaryReader(ms);
            Byte[] bytes = new Byte[ms.Length];
            bool bSuccess = false;
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(ms.Length);
            bytes = br.ReadBytes(nLength);
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }
        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }

        public static string SendImageToPrinter(int top, int left, System.Drawing.Bitmap bitmap)
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
        private static System.Drawing.Bitmap RotateImg(System.Drawing.Bitmap bmp, float angle)
        {
            angle = angle % 360;
            if (angle > 180) angle -= 360;
            float sin = (float)Math.Abs(Math.Sin(angle *
            Math.PI / 180.0)); // this function takes radians
            float cos = (float)Math.Abs(Math.Cos(angle * Math.PI / 180.0)); // this one too
            float newImgWidth = sin * bmp.Height + cos * bmp.Width;
            float newImgHeight = sin * bmp.Width + cos * bmp.Height;
            float originX = 0f;
            float originY = 0f;
            if (angle > 0)
            {
                if (angle <= 90)
                    originX = sin * bmp.Height;
                else
                {
                    originX = newImgWidth;
                    originY = newImgHeight - sin * bmp.Width;
                }
            }
            else
            {
                if (angle >= -90)
                    originY = sin * bmp.Width;
                else
                {
                    originX = newImgWidth - sin * bmp.Height;
                    originY = newImgHeight;
                }
            }
            System.Drawing.Bitmap newImg =
            new System.Drawing.Bitmap((int)newImgWidth, (int)newImgHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImg);
            g.Clear(System.Drawing.Color.White);
            g.TranslateTransform(originX, originY); // offset the origin to our calculated values
            g.RotateTransform(angle); // set up rotate
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.DrawImageUnscaled(bmp, 0, 0); // draw the image at 0, 0
            g.Dispose();
            return newImg;
        }
        public static string SendImageToPrinter(int top, int left, string source, float angle)
        {
            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(source);
            System.Drawing.Bitmap newbitmap = RotateImg(bitmap, angle);
            return SendImageToPrinter(top, left, newbitmap);
        }
        public static string SendImageToPrinter(int top, int left, string source)
        {
            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)FromFile(source);
            File.Delete(source);
            return SendImageToPrinter(top, left, bitmap);
        }

        public static Image FromFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var ms = new MemoryStream(bytes);
            var img = Image.FromStream(ms);
            return img;
        }

        public void Print(string dataToPrint)
        {
            System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
            pd.PrinterSettings = new PrinterSettings();
            GT800_Printer.SendStringToPrinter(pd.PrinterSettings.PrinterName, dataToPrint);
        }
        #endregion

        // Serial com
        public bool IsSerialPrint { get; set; }
        public string serialPortName { get; set; }
        public SerialPort serialPort = new SerialPort();
        public int ScanTimeOut = 1000;
        private string dataReciverBuffer { get; set; } = "";

        public event EventHandler SerialSend;

        public void SerialInit()
        {
            if (SerialPort.GetPortNames().Contains(serialPortName))
            {
                if (serialPort.IsOpen) serialPort.Close();

                serialPort.PortName = serialPortName;
                serialPort.BaudRate = 9600;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Parity = Parity.None;


                serialPort.DataReceived -= SerialPort_DataReceived;
                serialPort.DataReceived += SerialPort_DataReceived;

                dataReciverBuffer = "";
                try
                {
                    serialPort.Open();
                }
                catch (Exception)
                {

                }
                if (serialPort.IsOpen)
                {
                    //serialPort.Write("UQ\r\n" + Environment.NewLine);
                    SerialSend?.Invoke(this, null);
                }
            }
        }

        public void PortChange(string PortName)
        {
            if (serialPort.IsOpen) serialPort.Close();

            var PortList = SerialPort.GetPortNames();
            if (PortList.Contains(PortName))
            {
                serialPortName = PortName;
                serialPort.PortName = PortName;
                serialPort.BaudRate = 9600;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Parity = Parity.None;

                serialPort.DataReceived -= SerialPort_DataReceived;
                serialPort.DataReceived += SerialPort_DataReceived;

                dataReciverBuffer = "";
                try
                {
                    serialPort.Open();
                }
                catch (Exception err)
                {
                    System.Windows.MessageBox.Show("Printer port open error: " + err.Message);
                }
                if (serialPort.IsOpen)
                {
                    //serialPort.Write("U\n" + Environment.NewLine);
                    SerialSend?.Invoke(this, null);
                }

            }
            else
            {
            }
        }

        public void SendStringToPrinter(string str)
        {
            if (str == null)
            {
                return;
            }
            if (IsSerialPrint)
            {
                SerialSend?.Invoke(this, null);

                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine(str);
                    Debug.Write("Print a lablel via Serial.", Debug.ContentType.Log);
                    Console.WriteLine("Print a lablel via Serial.");
                }
            }
            else
            {
                System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
                pd.PrinterSettings = new PrinterSettings();
                GT800_Printer.SendStringToPrinter(pd.PrinterSettings.PrinterName, str);
                Console.WriteLine("Print a lablel via USB.");
            }
            Console.WriteLine(str);
        }

        public void SendFileToPrinter(string path)
        {
            if (path == null)
            {
                return;
            }
            if (IsSerialPrint)
            {
                SerialSend?.Invoke(this, null);

                if (serialPort.IsOpen)
                {
                    serialPort.WriteLine(path);
                    Debug.Write("Print a lablel via Serial.", Debug.ContentType.Log);
                    Console.WriteLine("Print a lablel via Serial.");
                }
            }
            else
            {
                System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
                pd.PrinterSettings = new PrinterSettings();
                string dataPrint = "I8,A,001\r\n\r\n\r\nQ203,024\r\nq831\r\nrN\r\nS5\r\nD10\r\nZT\r\nJF\r\nO\r\nR314,0\r\nf100\r\nN\r\n";
                dataPrint += SendImageToPrinter(0, 0, path);
                dataPrint += "\r\nP1";
                GT800_Printer.SendStringToPrinter(pd.PrinterSettings.PrinterName, dataPrint);

            }
            Console.WriteLine(path);
        }

        public void SendImageToPrinter(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return;
            }
            if (IsSerialPrint)
            {
                SerialSend?.Invoke(this, null);

                if (serialPort.IsOpen)
                {
                    Debug.Write("Print a lablel via Serial.", Debug.ContentType.Log);
                    Console.WriteLine("Print a lablel via Serial.");
                }
            }
            else
            {
                System.Windows.Forms.PrintDialog pd = new System.Windows.Forms.PrintDialog();
                pd.PrinterSettings = new PrinterSettings();
                string str = "I8,A,001\n";
                str += "\n";
                str += "V00,8,L\n";
                str += "Q" + 210 + "," + 024 + "\n";
                str += "q300\n";
                str += "rN\n";
                str += "S1" + "\n";
                str += "D15" + "\n";
                str += "ZT\n";
                str += "JF\n";
                str += "OC\n";
                str += "R" + 5 + "," + 5 + "\n";
                str += "f100\n";
                str += "N\n";
                str += SendImageToPrinter(0, 0, bitmap);
                str += "P1\n";
                SendStringToPrinter(pd.PrinterSettings.PrinterName, str);

            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            dataReciverBuffer = serialPort.ReadExisting();
            Console.WriteLine(dataReciverBuffer);
        }

        public void saveConfig()
        {
            Extensions.SaveToFile(this, PrinterConfigPath);
        }
    }
}

