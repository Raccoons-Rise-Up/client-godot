using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Client.Utilities
{
    public static class AppData
    {
        public static void SaveLoginInfo(string token, string username, string password)
        {
            var folder = System.Environment.SpecialFolder.LocalApplicationData;
            var appDataPath = Path.Combine(System.Environment.GetFolderPath(folder), "Raccoons Rise Up");
            var tokenPath = Path.Combine(appDataPath, "loginInfo.json");

            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            if (!File.Exists(tokenPath))
            {
                var fs = File.Create(tokenPath);
                fs.Close();
            }

            var dict = new Dictionary<string, string>{
                { "username", EncryptionHelper.Encrypt(username) },
                { "password", EncryptionHelper.Encrypt(password) },
                { "token", EncryptionHelper.Encrypt(token) }
            };

            var contents = JsonConvert.SerializeObject(dict, Formatting.Indented);

            File.WriteAllText(tokenPath, contents);
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
            var folder = System.Environment.SpecialFolder.LocalApplicationData;
            var appDataPath = Path.Combine(System.Environment.GetFolderPath(folder), "Raccoons Rise Up");
            var tokenPath = Path.Combine(appDataPath, "loginInfo.json");

            string contents;
            try
            {
                contents = File.ReadAllText(tokenPath);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(contents).ToDictionary(x => x.Key, x => EncryptionHelper.Decrypt(x.Value));
            }
            catch (Exception)
            {
                //Godot.GD.Print($"Failed to get JWT File: {e.Message}");
                return null;
            }
        }
    }
}