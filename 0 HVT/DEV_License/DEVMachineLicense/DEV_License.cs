using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DEVMachineLicense
{
    public class DEV_License : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private string _HelloContent = "Hay ton trong quyen tac gia...";
        public string HelloContent
        {
            get { return _HelloContent; }
        }

        public enum LicenseType
        {
            Time,
            All
        }

        private LicenseType _MachineLisenseType;
        public LicenseType MachineLisenseType
        {
            get { return _MachineLisenseType; }
            set
            {
                if (value != _MachineLisenseType)
                {
                    _MachineLisenseType = value;
                    NotifyPropertyChanged("MachineLisenseType");
                }
            }
        }

        private string _DeviceMAC;
        public string DeviceMAC
        {
            get { return _DeviceMAC; }
            set
            {
                if (value != null && value != _DeviceMAC)
                {
                    _DeviceMAC = value;
                    NotifyPropertyChanged("DeviceMAC");
                }
            }
        }

        private int _ExpiredTime;
        public int ExpiredTime
        {
            get { return _ExpiredTime; }
            set
            {
                if (value != _ExpiredTime)
                {
                    _ExpiredTime = value;
                    NotifyPropertyChanged("ExpiredTime");
                }
            }
        }

        private DateTime _StartDate;
        public DateTime StartDate
        {
            get { return _StartDate; }
            set
            {
                if (value != null && value != _StartDate)
                {
                    _StartDate = value;
                    NotifyPropertyChanged("StartDate");
                }
            }
        }


        private string _MachineName;
        public string MachineName
        {
            get { return _MachineName; }
            set
            {
                if (value != null && value != _MachineName)
                {
                    _MachineName = value;
                    NotifyPropertyChanged("MachineName");
                }
            }
        }

        private bool _IsExpired = false;
        public bool IsExpired
        {
            get { return _IsExpired; }
            set
            {
                if (value != _IsExpired)
                {
                    _IsExpired = value;
                    NotifyPropertyChanged("IsExpired");
                }
            }
        }

        public bool CheckLisence(string MachineName, out string Message)
        {
            if (File.Exists("License.lic"))
            {
                var _licenseBytesSave = File.ReadAllBytes("License.lic");
                var _licenseBytes = _licenseBytesSave;

                for (int i = 0; i < _licenseBytes.Count(); i++)
                {
                    _licenseBytes[i] = (byte)(_licenseBytesSave[i] + 40);
                }
                var _licenseStr = Encoding.ASCII.GetString(_licenseBytes);

                DEV_License _License = new DEV_License();
                try
                {
                    _License = JsonSerializer.Deserialize<DEV_License>(_licenseStr);

                }
                catch (Exception)
                {
                    Message = "License file not correct format or have modify";
                    return false;
                }
                if (MachineName == _License.MachineName)
                {
                    switch (_License.MachineLisenseType)
                    {
                        case LicenseType.Time:
                            if (DateTime.Now.Subtract(StartDate).TotalDays > _License.ExpiredTime)
                            {
                                Message = "License expired";
                                this.IsExpired = true;
                                return false;
                            }
                            else
                            {
                                var macAddr1 = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                                where nic.OperationalStatus == OperationalStatus.Up
                                                select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

                                if (_License.DeviceMAC != macAddr1)
                                {
                                    Message = "machine not match";
                                    return false;
                                }
                                else
                                {
                                    Message = "OK";
                                    return true;
                                }
                            }
                        case LicenseType.All:
                            var macAddr2 = (from nic in NetworkInterface.GetAllNetworkInterfaces()
                                            where nic.OperationalStatus == OperationalStatus.Up
                                            select nic.GetPhysicalAddress().ToString()).FirstOrDefault();

                            if (_License.DeviceMAC != macAddr2)
                            {
                                Message = "machine not match";
                                return false;
                            }
                            else
                            {
                                Message = "OK";
                                return true;
                            }
                        default:
                            Message = "License file not found";
                            return false;
                    }
                }
                else
                {
                    Message = "machine not match";
                    return false;
                }
            }
            else
            {
                Message = "License file not found";
                return false;
            }
        }

        public void UpdateLicense(DEV_License _License)
        {
            var _LicenseStr = JsonSerializer.Serialize<DEV_License>(_License);
            var _licenseBytes = Encoding.ASCII.GetBytes(_LicenseStr);
            var _licenseBytesSave = Encoding.ASCII.GetBytes(_LicenseStr);
            for (int i = 0; i < _licenseBytes.Count(); i++)
            {
                _licenseBytesSave[i] = (byte)(_licenseBytes[i] - 40);
            }
            File.WriteAllBytes("License.lic", _licenseBytesSave);
        }

        public void CreatLicense()
        {
            var _LicenseStr = JsonSerializer.Serialize<DEV_License>(this);
            var _licenseBytes = Encoding.ASCII.GetBytes(_LicenseStr);
            var _licenseBytesSave = Encoding.ASCII.GetBytes(_LicenseStr);
            for (int i = 0; i < _licenseBytes.Count(); i++)
            {
                _licenseBytesSave[i] = (byte)(_licenseBytes[i] - 40);
            }
            File.WriteAllBytes("License.lic", _licenseBytesSave);
        }
    }
}
