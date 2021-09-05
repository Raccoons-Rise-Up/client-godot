using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KRU.IO
{
    public static class AppData
    {
        public static void SaveJsonWebToken(string token, string username)
        {
            var folder = System.Environment.SpecialFolder.LocalApplicationData;
            var appDataPath = Path.Combine(System.Environment.GetFolderPath(folder), "Kittens Rise Up");
            var tokenPath = Path.Combine(appDataPath, "token.json");

            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            if (!File.Exists(tokenPath))
            {
                var fs = File.Create(tokenPath);
                fs.Close();
            }

            var dict = new Dictionary<string, string>{
                { "NOTICE", "DO NOT SHARE THIS TOKEN WITH ANYONE!!!"},
                { "username", username },
                { "token", token }
            };

            var contents = JsonConvert.SerializeObject(dict, Formatting.Indented);

            File.WriteAllText(tokenPath, contents);
        }

        public static Godot.Collections.Dictionary<string, string> GetJsonWebToken()
        {
            var folder = System.Environment.SpecialFolder.LocalApplicationData;
            var appDataPath = Path.Combine(System.Environment.GetFolderPath(folder), "Kittens Rise Up");
            var tokenPath = Path.Combine(appDataPath, "token.json");

            string contents;
            try
            {
                contents = File.ReadAllText(tokenPath);
            }
            catch (Exception)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<Godot.Collections.Dictionary<string, string>>(contents);
        }
    }
}
