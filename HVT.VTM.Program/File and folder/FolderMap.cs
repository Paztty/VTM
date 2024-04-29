using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace HVT.VTM.Program
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
        public string HistoryFolder = @"\History\" + DateTime.Now.ToString(@"yyyy\\MM");
        public const string MESFolder = @"\MES";
        public const string PCBFolder = @"\PCB";

        public const string DefaultModelFileExt = ".vmdl";
        public const string DefaultTxFileExt = ".vtx";
        public const string DefaultRxFileExt = ".vrx";
        public const string DefaultQrFileExt = ".vqr";
        public const string DefaultLogFileExt = ".vlog";

        public void TryCreatFolderMap()
        {
            RootFolder = "D:\\Daeyoung Electronis Vina\\VTM";
            if (!Directory.Exists(RootFolder)) Directory.CreateDirectory(RootFolder);
            if (!Directory.Exists(RootFolder + SettingFolder)) Directory.CreateDirectory(RootFolder + SettingFolder);
            if (!Directory.Exists(RootFolder + ModelFolder)) Directory.CreateDirectory(RootFolder + ModelFolder);
            if (!Directory.Exists(RootFolder + HistoryFolder)) Directory.CreateDirectory(RootFolder + HistoryFolder);
            if (!Directory.Exists(RootFolder + MESFolder)) Directory.CreateDirectory(RootFolder + MESFolder);
            if (!Directory.Exists(RootFolder + PCBFolder)) Directory.CreateDirectory(RootFolder + PCBFolder);
            Console.WriteLine(String.Format("{0}\\{1}", HistoryFolder, DateTime.Now.Date.ToString("dd") + ".vtmh"));

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

        public void SaveHistory(object HistoryObject)
        {
            Console.WriteLine(String.Format("{0}\\{1}",RootFolder + HistoryFolder, DateTime.Now.Date.ToString("dd") + ".vtmh"));
            File.AppendAllText(String.Format("{0}\\{1}", RootFolder + HistoryFolder, DateTime.Now.Date.ToString("dd") + ".vtmh"), HVT.Utility.Extensions.ConvertToJson(HistoryObject) + Environment.NewLine);
        }

    }
}
