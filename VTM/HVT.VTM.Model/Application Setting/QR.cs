using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Collections.ObjectModel;

namespace HVT.VTM.Base
{
    public class QR_Code
    {
        public static string ConfigPath = Environment.CurrentDirectory + @"\QRformat.config";

        public bool TestPCBPrintAll { get; set; } = false;
        public bool TestPCBPassPrint { get; set; } = true;
        public bool ArrayPCBPrint { get; set; } = true;
        public int PrintMaxStepCount { get; set; } = 3;

        public static String[] SEHC_YearCode = { "N", "R", "T", "W" };
        public static String[] SEHC_DayCode =
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
        public static String[] SEHC_MonthCode =
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

        public string UnitCode { get; set; } = "";
        public string MaterialCode { get; set; } = "";
        public string SupplierCode { get; set; } = "";
        public string ProductionDate { get; set; } = "";

        public string MainSerialNumber { get; set; } = "";
        public string SubSerialNumber { get; set; } = "";

        public string QRCode { get; set; } = "";
        public string CountryCode { get; set; } = "";
        public string ProductionLine { get; set; } = "";
        public string InspectionEquipment { get; set; } = "";

        public string ThenumberofInspectionitem { get; set; } = "01";
        public string InspectionStart { get; set; } = "";
        public string InspectionEnd { get; set; } = "";
        public string InspectionItem { get; set; } = "";
        public string MeasuredValue { get; set; } = "";
        public string UpperLimitSpecificationValu { get; set; } = "";
        public string LowerLimitSpecificationValue { get; set; } = "";
        public string ClassificationSymbol { get; set; } = "/";
        public string Separator { get; set; } = "-";

        public int Size { get; set; } = 4;
        public int Mode { get; set; } = 2;

        public string TestResult = "";

        public class Label
        {
            public int Lenght { get; set; } = 200;

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

            public string QrCodeData = "";
            public string SerialCode = "";
            public string ModelCode = "";
        }

        public Label label { get; set; } = new Label();

        public List<string> parameter = new List<string>();



        public QR_Code() { }

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

        public void printTestQR(GT800_Printer printer)
        {
            //A2          DYSCRA50055QRVNL01AP0101/20211005134505/20211005134514/D011-1184-1291-1169/
            string qr = "A3";
            qr += SupplierCode;
            qr += SEHC_YearCode[1];
            qr += SEHC_MonthCode[11];
            qr += SEHC_DayCode[21];
            qr += 0021;
            qr += QRCode;
            qr += CountryCode;
            qr += ProductionLine;
            qr += InspectionEquipment;
            qr += ThenumberofInspectionitem;
            qr += ClassificationSymbol;
            qr += "20211221080506";
            qr += ClassificationSymbol;
            qr += "20211221090506";
            qr += ClassificationSymbol;
            //if (query.Count > 0)
            //{
            //    foreach (var realTestitem in siteTester.Steps)
            //    {
            //        if (query.Where(o => o.Content == realTestitem.Content)
            //                .Select(o => o.Code)
            //                .DefaultIfEmpty("@")
            //                .First() != "@")
            //        {
            //            qr += query.Where(o => o.Content == realTestitem.Content)
            //                    .Select(o => o.Code)
            //                    .DefaultIfEmpty()
            //                    .First();
            //            qr += "-";
            //            qr += realTestitem.Value;
            //            qr += "-";
            //            qr += new string(realTestitem.Max.Where(c => char.IsDigit(c)).ToArray());
            //            qr += "-";
            //            qr += new string(realTestitem.Min.Where(c => char.IsDigit(c)).ToArray());
            //            qr += ClassificationSymbol;
            //        }
            //    }
            // string print built
            string str = "I8,A,001\n";
            str += "\n";
            str += "V00,8,L\n";
            str += "Q"+ label.Lenght +",015\n";
            str += "q1228\n";
            str += "rN\n";
            str += "S1" + "\n";
            str += "D15" + "\n";
            str += "ZB\n";
            str += "JF\n";
            str += "OC\n";
            str += "R" + label.home_x + "," + label.home_y + "\n";
            str += "f100\n";
            str += "N\n";
            str += "b" + label.qr_x + "," + label.qr_y + ",Q,m" + Mode + ",s" + Size + ",eQ,iA\"" + qr + "\"\n"; // Nội dung QR code

            str += "A" + label.SN1_X + "," + label.SN1_Y + ",4,"+ label.SN1_Font +",1,1,N,\"DYELC\"\n";
            str += "A" + label.MainCodeVersion_X + "," + label.MainCodeVersion_Y + ",4," + label.MainCodeVersion_Font + ",1,1,N,\"DYELC\"\n";

            str += "A" + label.SN2_X + "," + label.SN2_Y + ",5," + label.SN2_Font + ",1,1,N,\"DYELC\"\n";
            str += "A" + label.InvCodeVersion_X + "," + label.InvCodeVersion_Y + ",5," + label.InvCodeVersion_Font + ",1,1,N,\"DYELC\"\n";

            str += "A" + label.PCBArrayText_X + "," + (label.PCBArrayText_Y + 30) + ",4," + label.PCBArrayText_Font + ",1,1,N,\"" + "A" + "\"\n";
            str += "LE" + (label.PCBArrayText_X - 10) + "," + (label.PCBArrayText_Y + 30) + ",35,35" + "\n";
            str += "P1\n";




            Console.WriteLine(str);

            printer.SendStringToPrinter(str);

            //saveQRFormat();
            //}
            //else
            //{
            //    HVT.Utility.Debug.Write("No QR code sellected to print.", HVT.Utility.Debug.ContentType.Warning);
            //}
        }

        public void saveQRFormat()
        {
            string QRformat = HVT.Utility.Extensions.ConvertToJson(this);
            File.WriteAllText(ConfigPath, QRformat);
        }

    }
}
