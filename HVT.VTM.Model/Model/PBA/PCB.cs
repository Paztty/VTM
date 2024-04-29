using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HVT.VTM.Base
{
    public class PCB : INotifyPropertyChanged
    {
        /// <summary>
        /// Apply change to binding UI Element automatically 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler PCB_WaitChange;

        public string Name { get; set; }
        private bool isWait;
        public bool IsWait
        {
            get { return isWait; }
            set
            {
                if (isWait != value)
                {
                    isWait = value;
                    NotifyPropertyChanged("IsWait");
                    PCB_WaitChange?.Invoke(value, new EventArgs());
                }
            }
        }
        private bool isManualSelect;
        public bool IsManualSelect
        {
            get { return isManualSelect; }
            set
            {
                if (isManualSelect != value)
                {
                    isManualSelect = value;
                    NotifyPropertyChanged("IsManualSelect");
                }
            }
        }
        /// <summary>
        /// PCB barcode
        /// </summary>
        /// 
        private string barcodeInput;
        public string BarcodeInput
        {
            get { return barcodeInput; }
            set
            {
                if (value != this.barcodeInput)
                {
                    this.barcodeInput = value;
                    NotifyPropertyChanged(nameof(BarcodeInput));
                }
            }
        }
        private string barcodeOutput;
        public string BarcodeOutput
        {
            get { return barcodeOutput; }
            set
            {
                if (value != this.barcodeOutput)
                {
                    this.barcodeOutput = value;
                    NotifyPropertyChanged(nameof(BarcodeOutput));
                }
            }
        }

        /// <summary>
        /// Steps list
        /// </summary>
        public ObservableCollection<Step> Steps = new ObservableCollection<Step>();

        private TxData txData;
        public TxData TxData
        {
            get { return txData; }
            set 
            {
                if (value != this.txData)
                {
                    this.txData = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private RxData rxData;
        public RxData RxData
        {
            get { return rxData; }
            set
            {
                if (value != this.rxData)
                {
                    this.rxData = value;
                    NotifyPropertyChanged();
                }
            }
        }

    }
}
