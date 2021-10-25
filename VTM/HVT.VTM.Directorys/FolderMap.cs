using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVT.VTM.Directorys
{
    public class FolderMap
    {
        private string rootFolder;
        public string RootFolder
        {
            get { return rootFolder; }
            set {
                rootFolder = value;
                CheckFolderMap();
            }
        }

        public const string SettingFolder = @"\Setting";
        public const string ModelFolder = @"\Model";
        public string HistoryFolder = @"\History\" + DateTime.Now.ToString("yyyy\\MM\\dd");
        public const string MESFolder = @"\MES";
        public const string PCBFolder = @"\PCB";

        private void CheckFolderMap()
        {
            if (!Directory.Exists(RootFolder)) Directory.CreateDirectory(RootFolder);
            if (!Directory.Exists(RootFolder+SettingFolder)) Directory.CreateDirectory(RootFolder);
            if (!Directory.Exists(RootFolder+ModelFolder)) Directory.CreateDirectory(RootFolder);
            if (!Directory.Exists(RootFolder+HistoryFolder)) Directory.CreateDirectory(RootFolder);
            if (!Directory.Exists(RootFolder+MESFolder)) Directory.CreateDirectory(RootFolder);
            if (!Directory.Exists(RootFolder+PCBFolder)) Directory.CreateDirectory(RootFolder);
        }
    }
}
