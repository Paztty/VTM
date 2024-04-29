using HVT.Utility;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

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

    public class RxData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public RxDataType dataType { get; set; } = RxDataType.DEC;

        private int no;
        public int No
        {
            get { return no; }
            set
            {
                if (no != value)
                {
                    no = value;
                    NotifyPropertyChanged(nameof(No));
                }
            }
        }
        public string Name { get; set; }
        public string ModeLoc { get; set; }
        public string Mode { get; set; }
        public string DataKind
        {
            get => Extensions.ToEnumString(_dataKind);
            set
            {
                if (value.Contains(')'))
                {
                    _dataKind = Extensions.ToEnum<RxDataKind>(value);
                }
                else
                {
                    RxDataKind outvalue = dataKind;
                    if (Enum.TryParse<RxDataKind>(value, out outvalue))
                        _dataKind = outvalue;
                }
            }
        }
        public RxDataKind dataKind
        {
            get { return _dataKind; }
            set
            {
                if (_dataKind != value)
                {
                    _dataKind = value;
                    NotifyPropertyChanged(nameof(dataKind));
                }
            }
        }
        private RxDataKind _dataKind = RxDataKind.bit;

        public string MByte { get; set; }
        public string M_Mbit { get; set; }
        public string M_Lbit { get; set; }
        public string LByte { get; set; }
        public string L_Mbit { get; set; }
        public string L_Lbit { get; set; }

        public string Type
        {
            get { return dataType.ToString(); }
            set
            {

                RxDataType outvalue;
                if (Enum.TryParse<RxDataType>(value, out outvalue)) dataType = outvalue;
            }
        }
        public string Remark { get; set; }
        public override string ToString()
        {
            string strReturn = No + "," + Name + "," + ModeLoc + "," + Mode + "," + DataKind + "," + MByte + "," + M_Mbit + "," + M_Lbit + "," + LByte + "," + L_Mbit + "," + L_Lbit + "," + Type + "," + Remark;
            return strReturn;
        }
        public string ToTooltipString()
        {
            string strReturn =
                "No:\t\t" + No + Environment.NewLine +
                "Name:\t\t" + Name + Environment.NewLine +
                "Mode loc:\t" + ModeLoc +  Environment.NewLine +
                "Mode:\t\t" + Mode.Replace(" ","") + Environment.NewLine +
                "Data kind:\t" + DataKind + Environment.NewLine +
                "M Byte:\t\t" + MByte+ Environment.NewLine +
                "    |M bit:\t\t" + M_Mbit + Environment.NewLine +
                "    |L bit:\t\t" + M_Lbit + Environment.NewLine +
                "L Byte:\t\t" + LByte  + Environment.NewLine +
                "    |M bit:\t\t" + L_Mbit  + Environment.NewLine +
                "    |L bit:\t\t" + L_Lbit  + Environment.NewLine +
                "Type:\t\t" + Type;
            return strReturn;
        }
    }
}
