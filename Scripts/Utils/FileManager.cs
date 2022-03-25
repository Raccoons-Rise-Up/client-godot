using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Client.Utilities
{
    public static class FileManager
    {
        private static string PathData = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Raccoons Rise Up");
        private static string PathLoginInfo = Path.Combine(PathData, "loginInfo.json");
        public static string PathOptions = Path.Combine(PathData, "options.json");

        public static void SaveLoginInfo(string token, string username, string password)
        {
            if (!Directory.Exists(PathData))
                Directory.CreateDirectory(PathData);

            if (!File.Exists(PathLoginInfo))
            {
                var fs = File.Create(PathLoginInfo);
                fs.Close();
            }

            var dict = new Dictionary<string, string>{
                { "username", EncryptionHelper.Encrypt(username) },
                { "password", EncryptionHelper.Encrypt(password) },
                { "token", EncryptionHelper.Encrypt(token) }
            };

            var contents = JsonConvert.SerializeObject(dict, Formatting.Indented);

            File.WriteAllText(PathLoginInfo, contents);
        }

        public static bool LoginInfoFileExist()
        {
            var folder = System.Environment.SpecialFolder.LocalApplicationData;
            var appDataPath = Path.Combine(System.Environment.GetFolderPath(folder), "Raccoons Rise Up");
            var tokenPath = Path.Combine(appDataPath, "loginInfo.json");

            if (!Directory.Exists(appDataPath))
                return false;

            if (!File.Exists(tokenPath))
                return false;

            return true;
        }

        public static Dictionary<string, string> GetLoginInfo()
        {
            string contents;
            try
            {
                contents = File.ReadAllText(PathLoginInfo);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(contents).ToDictionary(x => x.Key, x => EncryptionHelper.Decrypt(x.Value));
            }
            catch (Exception)
            {
                //Godot.GD.Print($"Failed to get JWT File: {e.Message}");
                return null;
            }
        }

        public static T WriteConfig<T>(string path) where T : new() => WriteConfig<T>(path, new T());

        public static T WriteConfig<T>(string path, T data)
        {
            var contents = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, contents);
            return data;
        }

        public static T GetConfig<T>(string path)
        {
            string contents;
            try
            {
                contents = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(contents);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }

    public class Options 
    {
        public double VolumeMusic { get; set; }
        public bool Fullscreen { get; set; }
        public bool FullscreenBorderless { get; set; }
    }
}