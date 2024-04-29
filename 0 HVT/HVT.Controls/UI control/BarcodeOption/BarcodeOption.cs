using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HVT.Controls
{
    public class BarcodeOption : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool useBarcodeInput = true;
        private bool useBarcodeLenghtFixed = true;
        private int barcodeLenght = 23;
        private bool compareModelCode = false;
        private int startModelCodePosition = 3;
        private string modelCode = "ABC123456";

        public bool UseBarcodeInput {
            get { return useBarcodeInput; }
            set
            {
                if (value != useBarcodeInput)
                {
                    useBarcodeInput = value;
                    OnPropertyChanged(nameof(UseBarcodeInput));
                }
            }
        } 
        public bool UseBarcodeLenghtFixed
        {
            get { return useBarcodeLenghtFixed; }
            set
            {
                if (value != useBarcodeLenghtFixed)
                {
                    useBarcodeLenghtFixed = value;
                    OnPropertyChanged(nameof(UseBarcodeLenghtFixed));
                }
            }
        }
        public int BarcodeLenght
        {
            get { return barcodeLenght; }
            set
            {
                if (value != barcodeLenght)
                {
                    barcodeLenght = value;
                    OnPropertyChanged(nameof(BarcodeLenght));
                }
            }
        }

        public bool CompareModelCode
        {
            get { return compareModelCode; }
            set
            {
                if (value != compareModelCode)
                {
                   compareModelCode = value;
                    OnPropertyChanged(nameof(CompareModelCode));
                }
            }
        }
        public int StartModelCodePosition
        {
            get { return startModelCodePosition; }
            set
            {
                if (value != startModelCodePosition)
                {
                    startModelCodePosition = value;
                    OnPropertyChanged(nameof(StartModelCodePosition));
                }
            }
        }
        public string ModelCode
        {
            get { return modelCode; }
            set
            {
                if (value != modelCode)
                {
                    modelCode = value;
                    OnPropertyChanged(nameof(ModelCode));
                }
            }
        }

        public bool BarcodeCheck(string InputCode)
        {
            if (UseBarcodeLenghtFixed)
            {
                if (InputCode.Length != BarcodeLenght)
                {
                    return false;
                }
                else
                {
                    if (CompareModelCode)
                    {
                        if (InputCode.IndexOf(ModelCode) == StartModelCodePosition)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (CompareModelCode)
                {
                    if (InputCode.IndexOf(ModelCode) == StartModelCodePosition)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
