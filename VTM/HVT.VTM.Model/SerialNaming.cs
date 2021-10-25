using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Core
{
    public class SerialNaming
    {
        public class TxData
        { 
            public int No { get; set; }
            public string Name { get; set; }
            public string Data { get; set; }
            public string Blank { get; set; }
            public string Remark { get; set; }
        }
        public List<TxData> TxDatas = new List<TxData>();

        public class RxData
        {
            public int No { get; set; }
            public string Name { get; set; }
            public string ModeLoc { get; set; }
            public string Mode { get; set; }
            public string DataKind { get; set; }
            public string MByte { get; set; }
            public string M_Mbit { get; set; }
            public string M_Lbit { get; set; }
            public string LByte { get; set; }
            public string L_Mbit { get; set; }
            public string L_Lbit { get; set; }
            public string Type { get; set; }
            public string Remark { get; set; }
        }
        public List<RxData> RxDatas = new List<RxData>();

        public SerialNaming()
        {
            if (File.Exists("TxNaming.CTN"))
            {
                var TxDataList = File.ReadAllLines("TxNaming.CTN");
                foreach (var item in TxDataList)
                {
                    var dataItem = item.Split(',');
                    TxDatas.Add(new TxData()
                    {
                        No = Convert.ToInt32(dataItem[0]),
                        Name = dataItem[1],
                        Data = dataItem[2],
                        Blank = dataItem[3],
                        Remark = dataItem[4]
                    });
                }
            }

            if (File.Exists("RxNaming.CRN"))
            {
                var TxDataList = File.ReadAllLines("RxNaming.CRN");
                foreach (var item in TxDataList)
                {
                    var dataItem = item.Split(',');
                    RxDatas.Add(new RxData()
                    {
                        No = Convert.ToInt32(dataItem[0]),
                        Name = dataItem[1],
                        ModeLoc = dataItem[2],
                        Mode = dataItem[3],
                        DataKind = dataItem[4],
                        MByte = dataItem[5],
                        M_Mbit = dataItem[6],
                        M_Lbit = dataItem[7],
                        LByte = dataItem[8],
                        L_Mbit = dataItem[9],
                        L_Lbit = dataItem[10],
                        Type = dataItem[11],
                        Remark = dataItem[12]
                    });
                }
            }
        }



    }
}
