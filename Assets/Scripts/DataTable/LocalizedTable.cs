using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/LocalizedTable", fileName = "LocalizedTable")]
public class LocalizedTable : ScriptableObject
{
    // public LocalizedRecord[] list;
    public LocalizationAsset localization = new LocalizationAsset();

    public LocalizedTable()
    {
        Debug.Log("localization.localeIsoCode: " + localization.localeIsoCode);

        // var data = new List<LocalizedRecord>();
        var csvFile = Resources.Load("CSV/LocalizedText") as TextAsset;
        var reader = new StringReader(csvFile.text);
        var n = 0;
        var prev = "";
        while (reader.Peek() != -1) {
            var line = reader.ReadLine();
            var lastStr = line.Substring(line.Length - 1);
            if (lastStr != ",") {
                prev += line + "\n";
                continue;
            }
            // var record = new LocalizedRecord(prev + line);
            if (n > 0) {
                // data.Add(record);
                var lines = (prev + line).Split(',');
                if (lines.Length >= 4) {
                    var key = lines[0];
                    switch (Application.systemLanguage) {
                        case SystemLanguage.Japanese:
                            localization.SetLocalizedString(key, lines[1]);
                            break;
                        case SystemLanguage.English:
                            localization.SetLocalizedString(key, lines[2]);
                            break;
                        case SystemLanguage.Chinese:
                            localization.SetLocalizedString(key, lines[3]);
                            break;
                        default:
                            localization.SetLocalizedString(key, key);
                            break;
                    }
                }
            }
            prev = "";
            n++;
        }
        // list = data.ToArray();
    }

    // // カテゴリタイプの取得
    // public string Get(string str)
    // {
    //     return str;
    // }
}
