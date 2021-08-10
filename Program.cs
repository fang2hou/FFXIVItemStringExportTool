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
    class Program
    {
        private static readonly Dictionary<int, Dictionary<string, string>> ItemDatabase = new Dictionary<int,
            Dictionary<string, string>>();

        static string CleanupName(string name)
        {
            name = name.Replace("<Indent/>", "");
            name = name.Replace("<SoftHyphen/>", "");
            return name;
        }

        static void ExportItemString(string gameDirectory, Language lang, string shortName)
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

        static void Main(string[] args)
        {
            const string outputDirectory = @"C:\Users\fang2hou\Desktop";
            const string globalGameDirectory = @"C:\Games\Square Enix\FFXIV GL";
            const string chineseGameDirectory = @"C:\Games\Square Enix\FFXIV CN";

            ExportItemString(globalGameDirectory, Language.Japanese, "ja");
            ExportItemString(globalGameDirectory, Language.English, "en");
            ExportItemString(globalGameDirectory, Language.French, "fr");
            ExportItemString(globalGameDirectory, Language.German, "de");
            ExportItemString(chineseGameDirectory, Language.ChineseSimplified, "cn");

            var jsonString = JsonConvert.SerializeObject(ItemDatabase);

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(outputDirectory, "ItemStrings.json")))
            {
                outputFile.WriteLine(jsonString);
            }
        }
    }
}
