using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HVT.Utility;

namespace HVT.VTM.Base
{
    public enum RxDataKind
    {
        [EnumMember(Value = "1) bit")]
        [Description("1) bit")]
        bit,
        [EnumMember(Value = "2) Byte")]
        [Description("2) Byte")]
        Byte,
        [EnumMember(Value = "3) range")]
        [Description("3) range")]
        Range,
    }

    public enum RxDataType
    {
        DEC,
        HEX,
        ASCII,
    }

    public class RxData
    {

        public RxDataKind dataKind { get; set; } = RxDataKind.bit;
        public RxDataType dataType { get; set; } = RxDataType.DEC;
        public int No { get; set; }
        public string Name { get; set; }
        public string ModeLoc { get; set; }
        public string Mode { get; set; }
        public string DataKind
        {
            get => Extensions.ToEnumString(dataKind);
            set
            {
                if (value.Contains(')'))
                {
                    dataKind = Extensions.ToEnum<RxDataKind>(value);
                }
                else
                {
                    RxDataKind outvalue = dataKind;
                    if (Enum.TryParse<RxDataKind>(value, out outvalue)) dataKind = outvalue;
                }
            }
        }

        public string MByte { get; set; }
        public string M_Mbit { get; set; }
        public string M_Lbit { get; set; }
        public string LByte { get; set; }
        public string L_Mbit { get; set; }
        public string L_Lbit { get; set; }

        public string Type {
            get { return dataType.ToString(); }
            set {

                RxDataType outvalue = RxDataType.DEC;

                if (Enum.TryParse<RxDataType>(value, out outvalue)) dataType = outvalue;
            }
        }
        public string Remark { get; set; }

        public override string ToString()
        {
            string strReturn = No + "," + Name + "," + ModeLoc + "," + Mode + "," + DataKind + "," + MByte + "," + M_Mbit + "," + M_Lbit + "," + LByte + "," + L_Mbit + "," + L_Lbit + "," + Type +"," + Remark;
            return strReturn;
        }
    }
}
