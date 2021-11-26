using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Base
{

    // Step test 
    public class Step : INotifyPropertyChanged
    {
        public const string DontCare = "0";
        public const string Ok= "Ok";
        public const string Ng = "Ng";



        public int No { get; set; }
        public string IMQSCode { get; set; }

        public string testContent { get; set; }
        public string TestContent
        {
            get { return testContent; }
            set
            {
                if (testContent != value && value.Length > 0)
                {
                    testContent = value;
                    NotifyPropertyChanged(nameof(TestContent));
                    //IMQSCode = Naming.QRDatas.Where(x => x.Context == value).Select(x => x.Code).DefaultIfEmpty("").First();
                    NotifyPropertyChanged(nameof(IMQSCode));
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
                }
            }
        }
        public CMDs cmd
        {
            get;
            set;
        }

        public CMDs SetCMD
        {
            get { return cmd; }
            set
            {
                if (cmd != value)
                {
                    cmd = value;
                    NotifyPropertyChanged("CMD");
                    CommandDescriptions = Command.Commands.SingleOrDefault(x => x.CMD == cmd);
                    NotifyPropertyChanged("CommandDescriptions");
                }
            }
        }

        private CommandDescriptions commandDescriptions = new CommandDescriptions();
        public CommandDescriptions CommandDescriptions
        {
            get { return commandDescriptions; }
            set
            {
                if (value != this.commandDescriptions)
                {
                    this.commandDescriptions = value;
                }
            }
        }

        public string Condition1 { get; set; }
        public string Oper { get; set; }

        public string Condition2 { get; set; }
        public string Spect { get; set; }
        public string Min_Max { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string ValueGet1data { get; set; }
        public string ValueGet2data { get; set; }
        public string ValueGet3data { get; set; }
        public string ValueGet4data { get; set; }
        public string Result1data { get; set; } = "";
        public string Result2data { get; set; } = "";
        public string Result3data { get; set; } = "";
        public string Result4data { get; set; } = "";

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
        public string Count {
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
        public string Remark { get; set; }
        public string ELoc { get; set; }
        public bool Skipdata { get; set; }
        public bool Result { get; set; }

        public string ValueGet1
        {
            get { return ValueGet1data; }
            set
            {
                if (value != this.ValueGet1data)
                {
                    this.ValueGet1data = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ValueGet2
        {
            get { return ValueGet2data; }
            set
            {
                if (value != this.ValueGet2data)
                {
                    this.ValueGet2data = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ValueGet3
        {
            get { return ValueGet3data; }
            set
            {
                if (value != this.ValueGet3data)
                {
                    this.ValueGet3data = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ValueGet4
        {
            get { return ValueGet4data; }
            set
            {
                if (value != this.ValueGet4data)
                {
                    this.ValueGet4data = value;
                    NotifyPropertyChanged();
                }
            }
        }
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
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public Step() { }
    }
}
