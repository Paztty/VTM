using System;
using System.Collections.Generic;
using System.IO;

namespace HVT.VTM.Base
{
    public class FolderMap
    {
        private static string rootFolder;
        public static string RootFolder
        {
            get { return rootFolder; }
            set
            {
                rootFolder = value;
            }
        }

        public const string SettingFolder = @"\Setting";
        public const string ModelFolder = @"\Model";
        public string HistoryFolder = @"\History\" + DateTime.Now.ToString("yyyy/MM/dd");
        public const string MESFolder = @"\MES";
        public const string PCBFolder = @"\PCB";

        public void TryCreatFolderMap()
        {
            RootFolder = "D:\\Daeyoung Electronis Vina\\VTM";
            if (!Directory.Exists(RootFolder)) Directory.CreateDirectory(RootFolder);
            if (!Directory.Exists(RootFolder + SettingFolder)) Directory.CreateDirectory(RootFolder + SettingFolder);
            if (!Directory.Exists(RootFolder + ModelFolder)) Directory.CreateDirectory(RootFolder + ModelFolder);
            if (!Directory.Exists(RootFolder + HistoryFolder)) Directory.CreateDirectory(RootFolder + HistoryFolder);
            if (!Directory.Exists(RootFolder + MESFolder)) Directory.CreateDirectory(RootFolder + MESFolder);
            if (!Directory.Exists(RootFolder + PCBFolder)) Directory.CreateDirectory(RootFolder + PCBFolder);
            
        }
        public static List<ModelLoaded> ModelLoadeds = new List<ModelLoaded>();
        public static void GetListModelsLoaded()
        {
            if (File.Exists(RootFolder + SettingFolder + "\\models.ld"))
            {
                ModelLoadeds.Clear();
                var strModel = File.ReadAllLines(RootFolder + SettingFolder + "\\models.ld");
                foreach (var item in strModel)
                {
                    ModelLoadeds.Add(new ModelLoaded() { Path = item });
                }
            }
        }
        public static void SaveListModelLoaded()
        {
            if (File.Exists(RootFolder + SettingFolder + "\\models.ld")) File.Delete(RootFolder + SettingFolder + "\\models.ld");
            for (int i = 0; i < 10; i++)
            {
                if (i < ModelLoadeds.Count)
                {
                    using (StreamWriter writer = File.AppendText(RootFolder + SettingFolder + "\\models.ld"))
                    {
                        writer.WriteLine(ModelLoadeds[i].Path);
                    }
                }
            }
        }

    }
}
