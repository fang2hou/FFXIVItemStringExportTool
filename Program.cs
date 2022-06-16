using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SaintCoinach.Ex;
using Newtonsoft.Json;

namespace FFXIVItemStringExportTool
{
    internal class Program
    {
        private static readonly Dictionary<int, Dictionary<string, string>> ItemDatabase = new Dictionary<int,
            Dictionary<string, string>>();

        private static string CleanupName(string name)
        {
            name = name.Replace("<Indent/>", "");
            name = name.Replace("<SoftHyphen/>", "");
            return name;
        }

        private static void ExportItemString(string gameDirectory, Language lang, string shortName)
        {
            var realm = new SaintCoinach.ARealmReversed(gameDirectory, lang);
            var items = realm.GameData.GetSheet<SaintCoinach.Xiv.Item>();

            foreach (var item in items)
            {
                if (!ItemDatabase.ContainsKey(item.Key))
                {
                    ItemDatabase.Add(item.Key, new Dictionary<string, string>());
                }

                ItemDatabase[item.Key].Add(shortName, CleanupName(item.Name.ToString()));
            }
        }

        private static void Main(string[] args)
        {
            const string outputDirectory = @"C:\Users\fang2hou\Desktop";
            const string globalGameDirectory = @"C:\Games\Square Enix\FFXIV GL";
            const string chineseGameDirectory = @"C:\Games\Square Enix\FFXIV CN";

            ExportItemString(globalGameDirectory, Language.Japanese, "ja");
            ExportItemString(globalGameDirectory, Language.English, "en");
            ExportItemString(globalGameDirectory, Language.French, "fr");
            ExportItemString(globalGameDirectory, Language.German, "de");
            ExportItemString(chineseGameDirectory, Language.ChineseSimplified, "cn");

            var cleanedDb = ItemDatabase.Where(kvp => kvp.Value["ja"] != "")
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var jsonString = JsonConvert.SerializeObject(cleanedDb);

            using (var outputFile = new StreamWriter(Path.Combine(outputDirectory, "ItemStrings.json")))
            {
                outputFile.WriteLine(jsonString);
            }
        }
    }
}