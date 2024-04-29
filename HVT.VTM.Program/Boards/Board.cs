using HVT.Controls;
using HVT.Utility;
using HVT.VTM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HVT.VTM.Program
{
    public class Board : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int No { get; set; }

        private string _ModelName;
        public string ModelName
        {
            get { return _ModelName; }
            set
            {
                if (value != null || value != _ModelName) _ModelName = value;
                OnPropertyChanged("ModelName");
            }
        }

        private string _SiteName;
        public string SiteName
        {
            get { return _SiteName; }
            set
            {
                if (value != null || value != _SiteName) _SiteName = value;
                OnPropertyChanged("SiteName");
            }
        }
        private string _ModelSource;
        public string ModelSource
        { 
            get { return _ModelSource; }
            set
            {
                if (value != null || value != _ModelSource) _ModelSource = value;
                OnPropertyChanged("ModelSource");
            }
        }

        [JsonIgnore]
        private bool _Skip = false;
        [JsonIgnore]
        public bool Skip
        {
            get { return _Skip; }
            internal set
            {
                if (value != _Skip)
                {
                    _Skip = value;
                }
                OnPropertyChanged("Skip");
            }
        }
        [JsonIgnore]
        private bool _UserSkip = false;
        [JsonIgnore]
        public bool UserSkip
        {
            get { return _UserSkip; }
            set
            {
                if (value != _UserSkip)
                {
                    _UserSkip = value;
                }
                OnPropertyChanged("UserSkip");
                System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
                Console.WriteLine(DateTime.Now.ToString() + "->" + t.ToString());
            }
        }

        public bool BarcodeReady = false;
        [JsonIgnore]
        private string _Barcode = "";
        public string Barcode
        {
            get { return _Barcode; }
            set
            {
                if (value != null || value != _Barcode)
                {
                    _Barcode = value;
                    if (value.Length > 5)
                    {
                        BarcodeReady = true;
                    }
                    else
                    {
                        BarcodeReady = false;
                    }
                    OnPropertyChanged("Barcode");
                }
            }
        }

        [JsonIgnore]
        private string _BarcodeNext = "";
        public string BarcodeNext
        {
            get { return _BarcodeNext; }
            set
            {
                if (value != null || value != _BarcodeNext) _BarcodeNext = value;
                OnPropertyChanged("BarcodeNext");
            }
        }

        private string _BoardDetail;
        public string BoardDetail
        {
            get { return _BoardDetail; }
            set
            {
                if (value != null || value != _BoardDetail) _BoardDetail = value;
                OnPropertyChanged("BoardDetail");
            }
        }


        private List<Step> _TestStep;
        public List<Step> TestStep
        {
            get { return _TestStep; }
            set
            {
                if (value != null || value != _TestStep)
                {

                    _TestStep = value;
                    switch (SiteName)
                    {
                        case "A":
                            foreach (var item in _TestStep)
                            {
                                item.Value = item.ValueGet1.ToString();
                                item.ResultValue = item.Result1.ToString();
                            }
                            break;
                        case "B":
                            foreach (var item in _TestStep)
                            {
                                item.Value = item.ValueGet2.ToString();
                                item.ResultValue = item.Result2.ToString();
                            }
                            break;
                        case "C":
                            foreach (var item in _TestStep)
                            {
                                item.Value = item.ValueGet3.ToString();
                                item.ResultValue = item.Result3.ToString();
                            }
                            break;
                        case "D":
                            foreach (var item in _TestStep)
                            {
                                item.Value = item.ValueGet4.ToString();
                                item.ResultValue = item.Result4.ToString();
                            }
                            break;
                        default:
                            break;
                    }
                    OnPropertyChanged("TestStep");
                }
            }
        }


        private string _QRout;
        public string QRout
        {
            get { return _QRout; }
            set
            {
                if (value != null || value != _QRout) _QRout = value;
                OnPropertyChanged("QRout");
            }
        }


        private DateTime _StartTest;
        public DateTime StartTest
        {
            get { return _StartTest; }
            set
            {
                if (value != null || value != _StartTest) _StartTest = value;
                OnPropertyChanged("StartTest");
            }
        }


        private DateTime _EndTest;
        public DateTime EndTest
        {
            get { return _EndTest; }
            set
            {
                if (value != null || value != _EndTest) _EndTest = value;
                OnPropertyChanged("EndTest");
            }
        }

        [JsonIgnore]
        private Step _FailStep;
        public Step FailStep
        {
            get { return _FailStep; }
            set
            {
                if (value != null || value != _FailStep) _FailStep = value;
                OnPropertyChanged("FailStep");
            }
        }


        private List<LevelChannel> _LevelChannels = new List<LevelChannel>();
        [JsonIgnore]
        public List<LevelChannel> LevelChannels
        {
            get { return _LevelChannels; }
            set
            {
                if (value != null || value != _LevelChannels) _LevelChannels = value;
                OnPropertyChanged("LevelChannels");
            }
        }

        public int LEVEL_COUNT(bool IsHightLevel, int channel, int skipSamples)
        {
            int skipcount = skipSamples;
            if (channel >= LevelChannels.Count) return -1;
            if (LevelChannels[channel].Samples.Count <= 1) return -1;
            else
            {
                int countEdge = 0;
                for (int i = 1; i < LevelChannels[channel].Samples.Count; i++)
                {

                    // bybass edge
                    if (skipcount > 0 )
                    {
                        skipcount--;
                        if (LevelChannels[channel].Samples[i - 1].Level != LevelChannels[channel].Samples[i].Level)
                        {
                            skipcount++;
                        }
                    }

                    // count edge
                    if (LevelChannels[channel].Samples[i - 1].Level == !IsHightLevel &&
                        LevelChannels[channel].Samples[i].Level == IsHightLevel)
                    {
                        if (skipcount <= 0) countEdge++;
                    }
                }
                return countEdge;
            }
        }

        private List<MuxChannel> _MuxChannels = new List<MuxChannel>();
        [JsonIgnore]
        public List<MuxChannel> MuxChannels
        {
            get { return _MuxChannels; }
            set
            {
                if (value != null || value != _MuxChannels) _MuxChannels = value;
                OnPropertyChanged("MuxChannels");
            }
        }


        private string _Result;
        public string Result
        {
            get { return _Result; }
            set
            {
                if (value != null || value != _Result) _Result = value;
                OnPropertyChanged("Result");
            }
        }
    }
}
