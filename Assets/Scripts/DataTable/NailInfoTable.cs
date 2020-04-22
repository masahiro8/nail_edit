using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SubjectNerd.Utilities;

public class NailInfoTable
{
    public NailInfoRecord[] list;

    [System.NonSerialized] public int[] showList;

    public NailInfoTable()
    {
        var data = new List<NailInfoRecord>();
        var csvFile = Resources.Load("CSV/NailList") as TextAsset;
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
            var record = new NailInfoRecord(prev + line);
            if (record.index > -1) {
                data.Add(record);
            }
            prev = "";
            n++;
        }
        list = data.ToArray();
    }

    // 表示リストの更新
    public void UpdateCategory(CategoryType type)
    {
        switch (type) {
            case CategoryType.None:
                showList = new int[]{};
                break;
            case CategoryType.Favorite:
                showList = DataTable.MyList.filterdList[(int)MyListType.Favorite].Value;
                break;
            case CategoryType.Have:
                showList = DataTable.MyList.filterdList[(int)MyListType.Have].Value;
                break;
            default:
                showList = list
                    .Select((v, i) => new { Value = v, Index = i })
                    .Where(v => v.Value.category == type)
                    .Select(v => v.Index)
                    .ToArray();
                break;
        }
    }
}
