using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{

    // Step test 
    public class Step : INotifyPropertyChanged
    {
        public const string DontCare = "exe";
        public const string Ok = "Ok";
        public const string Ng = "Ng";

        private int no;
        public int No
        {
            get { return no; }
            set
            {
                if (no != value)
                {
                    no = value;
                    NotifyPropertyChanged("No");
                }
            }
        }
        public string IMQSCode { get; set; }

        public string testContent { get; set; }
        public string TestContent
        {
            get
            {
                CommandDescriptions.IsListTestContent = false;
                return testContent;
            }
            set
            {
                if (testContent != value)
                {
                    testContent = value;
                    NotifyPropertyChanged(nameof(TestContent));

                    if (testContent != null)
                    {
                        CommandDescriptions.IsListTestContent = Naming.qrDatas.Where(x => x.Context.StartsWith(value)).ToList().Count > 0;
                        IMQSCode = Naming.qrDatas.Where(x => x.Context == value).Select(x => x.Code).DefaultIfEmpty("").First();
                        NotifyPropertyChanged(nameof(IMQSCode));
                    }
                    else
                    {
                        IMQSCode = "";
                        NotifyPropertyChanged(nameof(IMQSCode));
                    }
                }
            }
        }
        public string CMD
        {
            get { return cmd.ToString(); }
            set
            {
                CMDs ds = CMDs.NON;
                if (Enum.TryParse<CMDs>(value, out ds))
                {
                    cmd = ds;
                    CommandDescriptions = Command.Commands.SingleOrDefault(x => x.CMD == cmd);
                    NotifyPropertyChanged("CommandDescriptions");
                    NotifyPropertyChanged("CMD");
                    if (cmd == CMDs.UTN)
                    {
                        CommandDescriptions.IsListRemark = true;
                    }
                }
            }
        }
        public CMDs cmd
        {
            get;
            set;
        }
        [JsonIgnore]
        public CMDs SetCMD
        {
            get { return cmd; }
            set
            {
                cmd = value;
                NotifyPropertyChanged("CMD");
                CommandDescriptions = Command.Commands.SingleOrDefault(x => x.CMD == cmd);
                NotifyPropertyChanged("CommandDescriptions");
            }
        }

        [JsonIgnore]
        private CommandDescriptions _CommandDescriptions = new CommandDescriptions();
        public CommandDescriptions CommandDescriptions
        {
            get { return _CommandDescriptions; }
            set
            {
                if (value != null && value != _CommandDescriptions)
                {
                    _CommandDescriptions = value;
                    NotifyPropertyChanged("CommandDescriptions");
                    Condition1 = CommandDescriptions.Condition1 == "not use" ? "" : Condition1;
                    Condition2 = CommandDescriptions.Condition2 == "not use" ? "" : Condition2;
                    Oper = CommandDescriptions.Oper == "not use" ? "" : Oper;
                    Min = CommandDescriptions.Min == "not use" ? "" : Min;
                    Max = CommandDescriptions.Max == "not use" ? "" : Max;
                    Spect = CommandDescriptions.Spect == "not use" ? "" : Spect;
                    Mode = CommandDescriptions.Mode == "not use" ? "" : Mode;
                    Count = CommandDescriptions.Count == "not use" ? "0" : Count;
                }
            }
        }

        private string _condition1;
        [JsonIgnore]
        public string Condition1Tooltip { get; set; } = "";
        public string Condition1
        {
            get
            {
                if (CommandDescriptions.Condition1 == "NAMING")
                {
                    var selected = Naming.txDatas.Where(o => o.Name == _condition1).FirstOrDefault();
                    if (selected != null)
                        Condition1Tooltip = selected.Data;
                }

                if (CommandDescriptions.Condition1 == "RX DATA NAME")
                {
                    var selected = Naming.rxDatas.Where(o => o.Name == _condition1).FirstOrDefault();
                    if (selected != null)
                    {
                        Condition1Tooltip = selected.ToTooltipString();
                    }
                }
                return _condition1;
            }
            set
            {
                _condition1 = value;
                NotifyPropertyChanged(nameof(Condition1));
            }
        }

        private string _Oper;
        public string Oper
        {
            get { return _Oper; }
            set
            {
                if (value != null && value != _Oper)
                {
                    _Oper = value;
                    NotifyPropertyChanged("Oper");
                }
            }
        }


        private string _Condition2;
        public string Condition2
        {
            get { return _Condition2; }
            set
            {
                if (value != null && value != _Condition2)
                {
                    _Condition2 = value;
                    NotifyPropertyChanged("Condition2");
                }
            }
        }

        private string _Spect;
        public string Spect
        {
            get { return _Spect; }
            set
            {
                if (value != null && value != _Spect)
                {
                    _Spect = value;
                    NotifyPropertyChanged("Spect");
                }
            }
        }

        [JsonIgnore]
        public string Min_Max
        {
            get
            {
                if (CommandDescriptions.Min == "MIN" && CommandDescriptions.Max == "MAX")
                {
                    return min + "~" + max;
                }
                else if (cmd == CMDs.UTN)
                {
                    return Condition1;
                }
                else if (CommandDescriptions.Spect != "not use")
                {
                    return Spect;
                }
                else if (CommandDescriptions.Oper != "not use")
                {
                    return Oper;
                }
                else
                {
                    return Condition2;
                }
            }
        }

        private string min;
        public string Min
        {
            get { return min; }
            set
            {
                if (value != min)
                {
                    min = value;
                    NotifyPropertyChanged(nameof(Min));
                }
            }
        }
        private string max;
        public string Max
        {
            get { return max; }
            set
            {
                if (value != max)
                {
                    max = value;
                    NotifyPropertyChanged(nameof(Max));
                }
            }
        }

        private string ValueGet1data = "";
        private string ValueGet2data = "";
        private string ValueGet3data = "";
        private string ValueGet4data = "";

        private string Result1data = "";
        private string Result2data = "";
        private string Result3data = "";
        private string Result4data = "";

        private string mode;

        public string Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
                NotifyPropertyChanged("Mode");
            }
        }

        private int count;
        public string Count
        {
            get { return count.ToString(); }
            set
            {
                if (int.TryParse(value, out int cnt))
                {
                    count = cnt;
                    NotifyPropertyChanged("Count");
                }
            }
        }

        public string Mem { get; set; }
        public int E_Jump { get; set; }
        private string remark;
        public string Remark
        {
            get { return remark; }
            set
            {
                if (value != remark)
                {
                    remark = value;
                    NotifyPropertyChanged("Remark");
                }
            }
        }
        public string ELoc { get; set; }
        public bool Skipdata { get; set; }
        public bool Result { get; set; }
        private string value_save;
        public string Value
        {
            get { return value_save; }
            set
            {
                if (value != null && value != "")
                {
                    value_save = value;
                }
            }
        }
        private string resultValue;
        public string ResultValue
        {
            get { return resultValue; }
            set
            {
                if (value != null && value != "")
                {
                    resultValue = value;
                }
            }
        }

        [JsonIgnore]
        public string ValueGet1
        {
            get { return ValueGet1data; }
            set
            {
                if (value == null) value = "-";
                if (value != this.ValueGet1data)
                {
                    this.ValueGet1data = value;
                    NotifyPropertyChanged(nameof(ValueGet1));
                }
            }
        }
        [JsonIgnore]
        public string ValueGet2
        {
            get { return ValueGet2data; }
            set
            {
                if (value == null) value = "-";
                if (value != this.ValueGet2data)
                {
                    if (value == null) value = "-";
                    this.ValueGet2data = value;
                    NotifyPropertyChanged(nameof(ValueGet2));
                }
            }
        }
        [JsonIgnore]
        public string ValueGet3
        {
            get { return ValueGet3data; }
            set
            {
                if (value == null) value = "-";
                if (value != this.ValueGet3data)
                {
                    this.ValueGet3data = value;
                    NotifyPropertyChanged(nameof(ValueGet3));
                }
            }
        }
        [JsonIgnore]
        public string ValueGet4
        {
            get { return ValueGet4data; }
            set
            {
                if (value == null) value = "-";
                if (value != this.ValueGet4data)
                {
                    this.ValueGet4data = value;
                    NotifyPropertyChanged(nameof(ValueGet4));
                }
            }
        }
        [JsonIgnore]
        public string Result1
        {
            get { return Result1data.ToString(); }
            set
            {
                if (value != this.Result1data)
                {
                    this.Result1data = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [JsonIgnore]
        public string Result2
        {
            get { return Result2data; }
            set
            {
                if (value != this.Result2data)
                {
                    this.Result2data = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [JsonIgnore]
        public string Result3
        {
            get { return Result3data; }
            set
            {
                if (value != this.Result3data)
                {
                    this.Result3data = value;
                    NotifyPropertyChanged();
                }
            }
        }
        [JsonIgnore]
        public string Result4
        {
            get { return Result4data; }
            set
            {
                if (value != this.Result4data)
                {
                    this.Result4data = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool Skip
        {
            get { return Skipdata; }
            set
            {
                if (value != this.Skipdata)
                {
                    this.Skipdata = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Step() { }
        public Step(int Index) { No = Index; }
    }
}
