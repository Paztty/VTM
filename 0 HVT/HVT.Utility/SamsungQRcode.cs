using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Office.Interop.Excel;
using Neodynamic.SDK.Printing;

namespace HVT.Utility
{
    public class SamsungQRcode
    {
        public static string ConfigPath = Environment.CurrentDirectory + @"\QRformat.config";

        public bool TestPCBPrintAll { get; set; } = false;
        public bool TestPCBPassPrint { get; set; } = true;
        public bool ArrayPCBPrint { get; set; } = true;
        public int PrintMaxStepCount { get; set; } = 3;
        public bool PrintUpsideDown { get; set; } = false;

        public string[] SEHC_YearCode { get; set; } = { "N", "R", "T", "W", "X", "Y", "L", "P", "Q", "S", "Z", "B", "C", "D", "F", "G", "H", "J", "K", "M", "N" };
        public string[] SEHC_DayCode { get; set; } =
               {" ",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "A",
                "B",
                "C",
                "D",
                "E",
                "F",
                "G",
                "H",
                "J",
                "K",
                "L",
                "M",
                "N",
                "O",
                "P",
                "R",
                "S",
                "T",
                "V",
                "W",
                "X",
                "Y",
                "Z"
                };
        public string[] SEHC_MonthCode { get; set; } =
               {" ",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "A",
                "B",
                "C",
        };

        public string UnitCode { get; set; } = "32";
        public string MaterialCode { get; set; } = " ";
        public string SupplierCode { get; set; } = "DYSC";
        public string ProductionDate { get; set; } = " ";

        public string MainSerialNumber { get; set; } = "";
        public string SubSerialNumber { get; set; } = " ";

        public string QRCode { get; set; } = "QR";
        public string CountryCode { get; set; } = "VN";
        public string ProductionLine { get; set; } = "L01";
        public string InspectionEquipment { get; set; } = "VTM1";

        public string ThenumberofInspectionitem { get; set; } = "01";
        public string InspectionStart { get; set; } = " ";
        public string InspectionEnd { get; set; } = " ";
        public string InspectionItem { get; set; } = " ";
        public string MeasuredValue { get; set; } = " ";
        public string UpperLimitSpecificationValue { get; set; } = " ";
        public string LowerLimitSpecificationValue { get; set; } = " ";
        public string ClassificationSymbol { get; set; } = "/";
        public string Separator { get; set; } = "-";

        public int Size { get; set; } = 3;
        public int Mode { get; set; } = 2;

        public string TestResult = " ";
        public int SerialBase { get; set; } = 0;

        public class Label
        {
            public int Lenght { get; set; } = 200;
            public int Pad { get; set; } = 18;

            public int home_x { get; set; } = 478;
            public int home_y { get; set; } = 0;

            public int qr_x { get; set; } = 50;
            public int qr_y { get; set; } = 5;

            public int SN1_X { get; set; } = 70;
            public int SN1_Y { get; set; } = 150;

            public int SN1_W { get; set; } = 70;
            public int SN1_H { get; set; } = 150;
            public string SN1_Font { get; set; } = "3";

            public int SN2_X { get; set; } = 230;
            public int SN2_Y { get; set; } = 10;

            public int SN2_W { get; set; } = 60;
            public int SN2_H { get; set; } = 150;
            public string SN2_Font { get; set; } = "3";


            public int MainCodeVersion_X { get; set; } = 60;
            public int MainCodeVersion_Y { get; set; } = 180;

            public int MainCodeVersion_W { get; set; } = 60;
            public int MainCodeVersion_H { get; set; } = 150;
            public string MainCodeVersion_Font { get; set; } = "2";

            public int InvCodeVersion_X { get; set; } = 260;
            public int InvCodeVersion_Y { get; set; } = 10;

            public int InvCodeVersion_W { get; set; } = 60;
            public int InvCodeVersion_H { get; set; } = 150;
            public string InvCodeVersion_Font { get; set; } = "2";

            public int PCBArrayText_X { get; set; } = 240;
            public int PCBArrayText_Y { get; set; } = 150;

            public int PCBArrayText_W { get; set; } = 60;
            public int PCBArrayText_H { get; set; } = 150;
            public string PCBArrayText_Font { get; set; } = "3";

            public int dark { get; set; } = 15;
            public int speed { get; set; } = 1;

            public string QrCodeData = " ";
            public string SerialCode = " ";
            public string ModelCode = " ";
        }

        public Label label { get; set; } = new Label();

        public List<string> parameter = new List<string>();

        public SamsungQRcode() { }

        public void SetDefault()
        {
            label = new Label();
        }

        public string testCode()
        {
            string DevQR = "99AA9999999ADYSCNBS9999-20201125220142.txt";
            DevQR = DevQR.Substring(0, DevQR.IndexOf('-'));

            label.SerialCode = DevQR.Substring(12, 11);
            label.ModelCode = DevQR.Substring(0, 12);

            UnitCode = DevQR.Substring(0, 2);
            MaterialCode = DevQR.Substring(2, 10);
            SupplierCode = DevQR.Substring(12, 4);
            ProductionDate = DevQR.Substring(16, 3);
            MainSerialNumber = DevQR.Substring(19, 4);

            QRCode = QRCode;
            CountryCode = CountryCode;
            ProductionLine = ProductionLine;
            InspectionEquipment = InspectionEquipment;

            parameter = new List<string>();

            string code = UnitCode
                        + MaterialCode
                        + SupplierCode
                        + ProductionDate
                        + MainSerialNumber
                        + QRCode
                        + CountryCode
                        + ProductionLine
                        + InspectionEquipment
                        + ThenumberofInspectionitem
                        + ClassificationSymbol
                        + "AAAAAAA"
                        + ClassificationSymbol
                        + "BBBBBBB"
                        + ClassificationSymbol;
            return code;
        }

        //public void SQCI_SAVE(FanSiteTester siteTester, TestHistory history, HVT.Utility.SQCI sqci)
        //{

        //    string qr = "A3";
        //    qr += SupplierCode;
        //    qr += SEHC_code.SEHC_YearCode[history.EndTime.Year - 2020];
        //    qr += SEHC_code.SEHC_MonthCode[history.EndTime.Month];
        //    qr += SEHC_code.SEHC_DayCode[history.EndTime.Day];
        //    qr += history.serial;
        //    qr += QRCode;
        //    qr += CountryCode;
        //    qr += ProductionLine;
        //    qr += InspectionEquipment;
        //    qr += ThenumberofInspectionitem;
        //    qr += ClassificationSymbol;
        //    qr += history.StartTime.ToString("yyyyMMddhhmmss");
        //    qr += ClassificationSymbol;
        //    qr += history.EndTime.ToString("yyyyMMddhhmmss");

        //    sqci.InspectionDate = history.EndTime.ToString("yyyyMMddHHmmss");
        //    sqci.PartCode = history.model;

        //    sqci.QR = qr;

        //    sqci.Items.Clear();
        //    var query = this.itemCodes.Where(x => x.Use == true).ToList();
        //    if (query.Count > 0)
        //    {
        //        foreach (var realTestitem in siteTester.Steps)
        //        {
        //            var testItem = query.Where(o => o.Content == realTestitem.Content)
        //                    .Select(o => o.SEHC_Content)
        //                    .DefaultIfEmpty(null)
        //                    .First();

        //            if ( testItem != null)
        //            {
        //                sqci.Items.Add(
        //                    new HVT.Utility.SQCI_Item()
        //                    {
        //                        Code = testItem,
        //                        Value = realTestitem.Value,
        //                        Min = realTestitem.Min,
        //                        Max = realTestitem.Max,
        //                        ResultTest = realTestitem.Result
        //                    });
        //            }
        //        }
        //    }
        //    sqci.Site = siteTester.Name;
        //    string SQCI_Str = sqci.ToString();
        //    Console.WriteLine(SQCI_Str);
        //    sqci.AppendToFile();
        //}

        public string GenerateCode(string Site, string Model, int Serial,
                                    DateTime StartTime, DateTime EndTime,
                                    List<string> Code, List<string> Min,
                                    List<string> Max, List<string> Value,
                                    List<bool> PrintItems,
                                    out string Barcode)
        {
            if (Code.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List code not have data");
            }

            if (Min.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List Min not have data");
            }

            if (Max.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List Max not have data");
            }

            if (Value.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List Value not have data");
            }

            if (PrintItems.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List Value not have data");
            }

            bool isInputSameSize = Code.Count() == Min.Count();
            isInputSameSize &= Code.Count() == Max.Count();
            isInputSameSize &= Code.Count() == Value.Count();
            isInputSameSize &= Code.Count() == PrintItems.Count();

            if (!isInputSameSize)
            {
                throw new ArgumentException("Data input need have same size.");
            }

            //Serial++;

            if (SerialBase + Serial > 9999)
            {
                MessageBox.Show("Limit number of labels per day (9999)");
                Barcode = "";
                return null ;
            }
            Model = Model.Replace("-", "");

            string qr = UnitCode;
            qr += Model;
            qr += SupplierCode;
            qr += SEHC_YearCode[DateTime.Now.Year - 2020] + SEHC_MonthCode[DateTime.Now.Month] + SEHC_DayCode[DateTime.Now.Day];
            qr += (SerialBase +Serial).ToString().PadLeft(4, '0');
            qr += QRCode;
            qr += CountryCode;
            qr += ProductionLine;
            qr += InspectionEquipment;

            Barcode = qr;

            string part2QR = "";

            part2QR += ClassificationSymbol;
            part2QR += StartTime.ToString("ssmmhhddMMyyyy");
            part2QR += ClassificationSymbol;
            part2QR += EndTime.ToString("ssmmhhddMMyyyy");
            part2QR += ClassificationSymbol;

            int StepAddedCount = 0;

            for (int i = 0; i < Code.Count(); i++)
            {
                if (StepAddedCount >= PrintMaxStepCount)
                {
                    break;
                }
                else if (Code[i].Length == 4 && PrintItems[i])
                {
                    string data = Code[i] + '-' + Value[i] + '-' + Max[i] + '-' + Min[i];
                    part2QR += data;
                    part2QR += ClassificationSymbol;
                    StepAddedCount++;
                }
            }

            qr += StepAddedCount.ToString().PadLeft(2, '0');
            qr += part2QR;


            string str = "I8,A,001\n";
            str += "\n";
            str += "V00,8,L\n";
            str += "Q" + label.Lenght + ","+ label.Pad.ToString().PadLeft(3,'0') +"\n";
            str += "q300\n";
            str += "rN\n";
            str += "S1" + "\n";
            str += "D15" + "\n";
            str += PrintUpsideDown == true ? "ZT\n" : "ZB\n";
            str += "JF\n";
            str += "OC\n";
            str += "R" + label.home_x + "," + label.home_y + "\n";
            str += "f100\n";
            str += "N\n";
            str += "b" + label.qr_x + "," + label.qr_y + ",Q,m" + Mode + ",s" + Size + ",eQ,iA\"" + qr + "\"\n"; // Nội dung QR code

            str += "A" + label.SN1_X + "," + label.SN1_Y + ",4," + label.SN1_Font + ",1,1,N,\"" + UnitCode + Model + "\"\n";
            //str += "A" + label.MainCodeVersion_X + "," + label.MainCodeVersion_Y + ",4," + label.MainCodeVersion_Font + ",1,1,N,\"" + UnitCode + Model + "\"\n";

            str += "A" + label.SN2_X + "," + label.SN2_Y + ",3," + label.SN2_Font + ",1,1,N,\"" + SupplierCode + SEHC_YearCode[DateTime.Now.Year - 2020] + SEHC_MonthCode[DateTime.Now.Month] + SEHC_DayCode[DateTime.Now.Day] + Serial.ToString().PadLeft(4, '0') + "\"\n";
            //str += "A" + label.InvCodeVersion_X + "," + label.InvCodeVersion_Y + ",5," + label.InvCodeVersion_Font + ",1,1,N,\"DYELC\"\n";

            if (ArrayPCBPrint)
            {
                str += "A" + label.PCBArrayText_X + "," + (label.PCBArrayText_Y) + ",4," + label.PCBArrayText_Font + ",1,1,N,\"" + Site + "\"\n";
                str += "LE" + (label.PCBArrayText_X - 10) + "," + (label.PCBArrayText_Y - 10) + "," + label.PCBArrayText_W + "," + label.PCBArrayText_H + "\n";
            }

            str += "P1\n";

            return str;
        }
        public string GenerateCode(string Site, string StartCode,
                            DateTime StartTime, DateTime EndTime,
                            List<string> Code, List<string> Min,
                            List<string> Max, List<string> Value,
                            string MainVersion, string SubVersion,
                            out string Barcode)
        {
            if (Code.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List code not have data");
            }

            if (Min.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List Min not have data");
            }

            if (Max.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List Max not have data");
            }

            if (Value.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List Value not have data");
            }

            bool isInputSameSize = Code.Count() == Min.Count();
            isInputSameSize &= Code.Count() == Max.Count();
            isInputSameSize &= Code.Count() == Value.Count();

            if (!isInputSameSize)
            {
                throw new ArgumentException("Data input need have same size.");
            }


            string qr = StartCode;
            qr += QRCode;
            qr += CountryCode;
            qr += ProductionLine;
            qr += InspectionEquipment;

            Barcode = qr;

            string part2QR = "";

            part2QR += ClassificationSymbol;
            part2QR += StartTime.ToString("yyyyMMddHHmmss");
            part2QR += ClassificationSymbol;
            part2QR += EndTime.ToString("yyyyMMddHHmmss");
            part2QR += ClassificationSymbol;

            int StepAddedCount = 0;

            for (int i = 0; i < Code.Count(); i++)
            {
                if (StepAddedCount >= PrintMaxStepCount)
                {
                    break;
                }

                if (Code[i] != null)
                {
                    if (Code[i].Length == 4)
                    {
                        string data = Code[i] + '-' + Value[i] + '-' + Max[i] + '-' + Min[i];
                        part2QR += data;
                        part2QR += ClassificationSymbol;
                        StepAddedCount++;
                    }
                }
            }

            qr += StepAddedCount.ToString().PadLeft(2, '0');
            qr += part2QR;


            string str = "I8,A,001\n";
            str += "\n";
            str += "V00,8,L\n";
            str += "Q" + label.Lenght + "," + label.Pad.ToString().PadLeft(3, '0') + "\n";
            str += "q300\n";
            str += "rN\n";
            str += "S1" + "\n";
            str += "D15" + "\n";
            str += PrintUpsideDown == true ? "ZT\n" : "ZB\n";
            str += "JF\n";
            str += "OC\n";
            str += "R" + label.home_x + "," + label.home_y + "\n";
            str += "f100\n";
            str += "N\n";
            str += "b" + label.qr_x + "," + label.qr_y + ",Q,m" + Mode + ",s" + Size + ",eQ,iA\"" + qr + "\"\n"; // Nội dung QR code

            str += "A" + label.SN1_X + "," + label.SN1_Y + ",4," + label.SN1_Font + ",1,1,N,\"" + StartCode.Substring(0,12) + "\"\n";
            str += "A" + label.MainCodeVersion_X + "," + label.MainCodeVersion_Y + ",4," + label.MainCodeVersion_Font + ",1,1,N,\"" + MainVersion + "\"\n";

            str += "A" + label.SN2_X + "," + label.SN2_Y + ",3," + label.SN2_Font + ",1,1,N,\"" + StartCode.Substring(12) + "\"\n";
            str += "A" + label.InvCodeVersion_X + "," + label.InvCodeVersion_Y + ",3," + label.InvCodeVersion_Font + ",1,1,N,\""+ SubVersion +"\"\n";

            if (ArrayPCBPrint)
            {
                str += "A" + label.PCBArrayText_X + "," + (label.PCBArrayText_Y) + ",4," + label.PCBArrayText_Font + ",1,1,N,\"" + Site + "\"\n";
                str += "LE" + (label.PCBArrayText_X - 10) + "," + (label.PCBArrayText_Y - 10) + "," + label.PCBArrayText_W + "," + label.PCBArrayText_H + "\n";
            }

            str += "P1\n";

            //Console.WriteLine(str);

            return str;
        }

        public string GenerateSampleCode()
        {
            string Model = "DJ9100000A";
            int Serial = 123;
            DateTime StartTime = DateTime.Now;
            DateTime EndTime = DateTime.Now;
            List<string> Code = new List<string> { "A001", "A002", "A003" };
            List<string> Min = new List<string> { "001", "002", "003" };
            List<string> Max = new List<string> { "001", "002", "003" };
            List<string> Value = new List<string> { "001", "002", "003" };



            if (Code.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List code not have data");
            }

            if (Min.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List Min not have data");
            }

            if (Max.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List Max not have data");
            }

            if (Value.Count() == 0)
            {
                throw new System.ArgumentOutOfRangeException("List Value not have data");
            }

            bool isInputSameSize = Code.Count() == Min.Count();
            isInputSameSize &= Code.Count() == Max.Count();
            isInputSameSize &= Code.Count() == Value.Count();
            if (!isInputSameSize)
            {
                throw new ArgumentException("Data input need have same size.");
            }

            string qr = UnitCode;
            qr += Model;
            qr += SupplierCode;
            qr += SEHC_YearCode[DateTime.Now.Year - 2020] + SEHC_MonthCode[DateTime.Now.Month] + SEHC_DayCode[DateTime.Now.Day];
            qr += Serial.ToString().PadLeft(4, '0');
            qr += QRCode;
            qr += CountryCode;
            qr += ProductionLine;
            qr += InspectionEquipment;
            qr += (PrintMaxStepCount < Code.Count() ? PrintMaxStepCount : Code.Count()).ToString().PadLeft(2, '0');
            qr += ClassificationSymbol;
            qr += StartTime.ToString("yyyyMMddhhmmss");
            qr += ClassificationSymbol;
            qr += EndTime.ToString("yyyyMMddhhmmss");
            qr += ClassificationSymbol;

            int StepAddedCount = 0;
            for (int i = 0; i < Code.Count(); i++)
            {
                if (StepAddedCount >= PrintMaxStepCount)
                {
                    break;
                }
                else
                {
                    string data = Code[i] + '-' + Value[i] + '-' + Max[i] + '-' + Min[i];
                    qr += data;
                    qr += ClassificationSymbol;
                    StepAddedCount++;
                }
            }

            string str = "I8,0,001\n";
            str += "\n";
            str += "V00,8,L\n";
            str += "Q" + label.Lenght + "," + label.Pad.ToString().PadLeft(3, '0') + "\n";
            str += "q300\n";
            str += "rN\n";
            str += "S"+ label.speed + "\n";
            str += "D" + label.dark + "\n";
            str += PrintUpsideDown == true ? "ZT\n" : "ZB\n";
            str += "JF\n";
            str += "OC\n";
            str += "R" + label.home_x + "," + label.home_y + "\n";
            str += "f100\n";
            str += "N\n";
            str += "b" + label.qr_x + "," + label.qr_y + ",Q,m" + Mode + ",s" + Size + ",eQ,iA\"" + qr + "\"\n"; // Nội dung QR code

            str += "A" + label.SN1_X + "," + label.SN1_Y + ",4," + label.SN1_Font + ",1,1,N,\"" + UnitCode + Model + "\"\n";
            str += "A" + label.MainCodeVersion_X + "," + label.MainCodeVersion_Y + ",4," + label.MainCodeVersion_Font + ",1,1,N,\"" + UnitCode + Model + "\"\n";

            str += "A" + label.SN2_X + "," + label.SN2_Y + ",3," + label.SN2_Font + ",1,1,N,\"" + SupplierCode + SEHC_YearCode[DateTime.Now.Year - 2020] + SEHC_MonthCode[DateTime.Now.Month] + SEHC_DayCode[DateTime.Now.Day] + Serial.ToString().PadLeft(4, '0') + "\"\n";
            str += "A" + label.InvCodeVersion_X + "," + label.InvCodeVersion_Y + ",3," + label.InvCodeVersion_Font + ",1,1,N,\"DYELC\"\n";

            if (ArrayPCBPrint)
            {
                str += "A" + label.PCBArrayText_X + "," + (label.PCBArrayText_Y) + ",4," + label.PCBArrayText_Font + ",1,1,N,\"" + "A" + "\"\n";
                str += "LE" + (label.PCBArrayText_X - 10) + "," + (label.PCBArrayText_Y - 10) + "," + label.PCBArrayText_W + "," + label.PCBArrayText_H + "\n";
            }

            str += "P1\n";

            //Console.WriteLine(str);

            return str;
        }

        public void saveQRFormat()
        {
            Extensions.SaveToFile(this, ConfigPath);
        }

    }
}

