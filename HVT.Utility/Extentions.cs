using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace HVT.Utility
{
    public static class Extensions
    {
        public const string LogDefine = "Dev Program Log File";
        public const string LogExt = ".elog";
        private static string LogFile = "LOG_PROGRAM" + DateTime.Now.ToString("ddMMyyyy") + ".elog";


        // JSON clone oject
        public static T Clone<T>(this T source)
        {
            var serialized = JsonSerializer.Serialize(source);
            Console.WriteLine(serialized);
            return JsonSerializer.Deserialize<T>(serialized);
        }


        // JSON convert opject to String
        public static string ConvertToJson<T>(this T source)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                MaxDepth = 1024,
                //WriteIndented = true
            };
            return JsonSerializer.Serialize(source, options);
        }

        public static T ConvertFromJson<T>(string jsonStr)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                MaxDepth = 1024,
                //WriteIndented = true
            };
            return JsonSerializer.Deserialize<T>(jsonStr, options);
        }

        //Encoder string
        public static string Encoder(string plainText, Encoding encodingCode)
        {
            var plainTextBytes = encodingCode.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        //Decoder string
        public static string Decoder(string base64EncodedData, Encoding encodingCode)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return encodingCode.GetString(base64EncodedBytes);
        }

        public static T OpenFromFile<T>(string FileName)
        {
            if (File.Exists(FileName))
            {
                try
                {
                    JsonSerializerOptions options = new JsonSerializerOptions()
                    {
                        MaxDepth = 1024,
                        WriteIndented = true
                    };

                    var serialized = File.ReadAllText(FileName);
                    serialized = Decoder(serialized, Encoding.UTF7);
                    Console.WriteLine(serialized);
                    File.AppendAllText(LogFile, DateTime.Now.ToString() + "Extension : Open from file SUCCESS " + FileName + Environment.NewLine + serialized + Environment.NewLine);
                    return JsonSerializer.Deserialize<T>(serialized, options);

                }
                catch (Exception err)
                {
                    MessageBox.Show(err.StackTrace);
                    File.AppendAllText(LogFile, DateTime.Now.ToString() + "Extension : Open from file FAIL - " + FileName + err.Message + Environment.NewLine);
                }
            }
            else
            {
                //MessageBox.Show( Resource.ProgramContext_en_US.FileNotFound + ": " + FileName);
            }
            return default;
        }

        public static bool SaveToFile<T>(this T source, string FileName)
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    MaxDepth = 1024,
                    WriteIndented = true
                };

                var strToSave = JsonSerializer.Serialize(source, options);

                File.WriteAllText("modelText.txt", strToSave);

                Console.WriteLine(strToSave);
                strToSave = Encoder(strToSave, Encoding.UTF7);
                File.WriteAllText(FileName, strToSave);
                return true;
            }
            catch (Exception err)
            {
                File.AppendAllText(LogFile, DateTime.Now.ToString() + " Extension : Save to file FAIL -" + err.Message + Environment.NewLine);
                return false;
            }

        }
        public static void LogErr(string errMessage)
        {
            File.AppendAllText(LogFile, DateTime.Now.ToString() + " Extension : " + errMessage + Environment.NewLine);
        }

        public static void DataGrid2CSV(DataGrid comparisonGrid, string Title, string FileExit, string FileType)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = FileType + " (*." + FileExit + "|*." + FileExit;
            saveDlg.FilterIndex = 0;
            saveDlg.RestoreDirectory = true;
            saveDlg.Title = Title;
            if ((bool)saveDlg.ShowDialog())
            {
                string CsvFpath = saveDlg.FileName;
                System.IO.StreamWriter csvFileWriter = new StreamWriter(CsvFpath, false);
                string columnHeaderText = "";

                int countColumn = comparisonGrid.Columns.Count - 1;
                if (countColumn >= 0)
                {
                    columnHeaderText = comparisonGrid.Columns[0].Header.ToString();
                }

                // Writing column headers
                for (int i = 1; i <= countColumn; i++)
                {
                    columnHeaderText = columnHeaderText + ',' + (comparisonGrid.Columns[i].Header).ToString();
                }
                csvFileWriter.WriteLine(columnHeaderText);

                // Writing values row by row
                for (int i = 0; i <= comparisonGrid.Items.Count - 2; i++)
                {
                    string dataFromGrid = "";
                    for (int j = 0; j <= comparisonGrid.Columns.Count - 1; j++)
                    {
                        if (j == 0)
                        {
                            dataFromGrid = ((DataRowView)comparisonGrid.Items[i]).Row.ItemArray[j].ToString();
                        }
                        else
                        {
                            dataFromGrid = dataFromGrid + ',' + ((DataRowView)comparisonGrid.Items[i]).Row.ItemArray[j].ToString();
                        }
                    }
                    csvFileWriter.WriteLine(dataFromGrid);
                }
                csvFileWriter.Flush();
                csvFileWriter.Close();
            }
        }

        public static string ToEnumString<T>(T type)
        {
            var enumType = typeof(T);
            var name = Enum.GetName(enumType, type);
            var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
            return enumMemberAttribute.Value;
        }

        public static T ToEnum<T>(string str)
        {
            var enumType = typeof(T);
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
                if (enumMemberAttribute.Value == str) return (T)Enum.Parse(enumType, name);
            }
            //throw exception or whatever handling you want or
            return default(T);
        }
    }
}
