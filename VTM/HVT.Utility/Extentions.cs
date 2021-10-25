using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Text.Json;

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
            return JsonSerializer.Serialize(source);
        }

        public static T ConvertFromJson<T>(string jsonStr)
        {
            return JsonSerializer.Deserialize<T>(jsonStr);
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
                    var serialized = File.ReadAllText(FileName);
                    serialized = Decoder(serialized, Encoding.UTF7);
                    File.AppendAllText(LogFile, DateTime.Now.ToString() + "Extension : Open from file SUCCESS " + FileName + Environment.NewLine + serialized + Environment.NewLine);
                    return JsonSerializer.Deserialize<T>(serialized);

                }
                catch (Exception err)
                { 
                    File.AppendAllText(LogFile, DateTime.Now.ToString() + "Extension : Open from file FAIL - " + FileName + err.Message + Environment.NewLine);
                }
            }
            else
            {
                //MessageBox.Show( Resource.ProgramContext_en_US.FileNotFound + ": " + FileName);
            }
            return default(T);
        }

        public static bool SaveToFile<T>(this T source, string FileName)
        {
            try
            {
                var strToSave = JsonSerializer.Serialize(source);
                strToSave = Encoder(strToSave, Encoding.UTF7);
                File.WriteAllText(FileName,strToSave);
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
    }
}
