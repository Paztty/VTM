using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HVT.VTM.Core
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

        public string Name { get; set; }
        public bool IsWait { get; set; }
        /// <summary>
        /// PCB barcode
        /// </summary>
        private string barcodeInput;
        public string BarcodeInput
        {
            get { return barcodeInput; }
            set
            {
                if (value != this.barcodeInput)
                {
                    this.barcodeInput = value;
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Steps list
        /// </summary>
        public ObservableCollection<Model.Step> Steps = new ObservableCollection<Model.Step>();


        private SerialNaming.TxData txData;
        public SerialNaming.TxData TxData
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




    }
}
