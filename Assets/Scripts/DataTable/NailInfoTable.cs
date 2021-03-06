﻿using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SubjectNerd.Utilities;

public class NailInfoTable
{
    public NailInfoRecord[] list;
    private ParamTable _Param;

    // [System.NonSerialized] public int[] showList;

    public NailInfoTable()
    {
        _Param = Resources.Load<ParamTable>("Data/ParamTable");

        if (_Param.noUseAPI) {
            //CSVからのネイル情報
            this.ReadNailDataFromCsv();
        } else {
            //Apiからネイル情報
            this.ReadNailDataFromApi();
        }        

        // var str = "";
        // for (var i = 0; i < list.Length; i++) {
        //     var d = list[i];
        //     // var path = "Assets/Resources/Data/NailMaterial/";
        //     var path = "";
        //     var fn1 = path + d.fileName + ".asset";
        //     var fn2 = path + d.fileName2 + ".asset";
        //     str += "mv " + fn1 + " " + fn2 + "\n";
        //     // str += "mv " + fn1 + ".meta " + fn2 + ".meta\n";
        //     // UnityEditor.AssetDatabase.RenameAsset(fn1, fn2);
        // }
        // Debug.Log(str);
    }

    private void ReadNailDataFromApi() {
        // 商品マスタの読み込み
        var data = new List<NailInfoRecord>();

        //apiからのcsvを読み込み
        ReadFromTemporaryCachePath(@"NailProduct.csv", (index, lines) => {
            if (index > -1) {
                var record = new NailInfoRecord();
                record.index = index;
                record.AddProduct(lines);
                data.Add(record);
            }
        });
        list = data.ToArray();

        // Noとインデックスの整合性判定
        for (var i = 0; i < list.Length; i++) {
            UnityEngine.Assertions.Assert.IsTrue(list[i].index == i);
        }

        // カテゴリマスタの読み込み
        ReadFromTemporaryCachePath(@"NailCategory.csv", (index, lines) => {
            list[index].AddCategory(lines);
        });

        // EC遷移マスタの読み込み
        ReadFromTemporaryCachePath(@"NailECURL.csv", (index, lines) => {
            list[index].AddURL(lines);
        });
    }

    private void ReadNailDataFromCsv() {
        // 商品マスタの読み込み
        var data = new List<NailInfoRecord>();

        ReadData("CSV/NailProduct", (index, lines) => {
            if (index > -1) {
                var record = new NailInfoRecord();
                record.index = index;
                record.AddProduct(lines);
                data.Add(record);
            }
        });
        list = data.ToArray();

        // Noとインデックスの整合性判定
        for (var i = 0; i < list.Length; i++) {
            UnityEngine.Assertions.Assert.IsTrue(list[i].index == i);
        }

        // カテゴリマスタの読み込み
        ReadData("CSV/NailCategory", (index, lines) => list[index].AddCategory(lines));

        // EC遷移マスタの読み込み
        ReadData("CSV/NailECURL", (index, lines) => list[index].AddURL(lines));
    }

    //キャッシュから読み込み
    public void ReadFromTemporaryCachePath(string fileName, System.Action<int, string[]> action) {
        string path = Application.temporaryCachePath + "/" + fileName;
        StreamReader reader = new StreamReader(path); 
        using (StreamReader sr = new StreamReader(path)){
            string line;
            int index = 0;
            while ((line = sr.ReadLine()) != null){
                action(index , line.Split(','));
                index++;
            }
        }
        reader.Close();
    }

    // 表示リストの更新
    public void ReadData(string fileName, System.Action<int, string[]> action)
    {
        var csvFile = Resources.Load(fileName) as TextAsset;
        var reader = new StringReader(csvFile.text);
        // var index = 0;
        var dqCount = 0;
        var prev = "";
        while (reader.Peek() != -1) {
            var line = reader.ReadLine();
            dqCount += line.Replace("\"\"", "").Split('\"').Length - 1;
            if (dqCount % 2 != 0) {
                prev += line + "\n";
                continue;
            }
            var lines = (prev + line).Split(',');
            var index = int.TryParse(lines[0], out int n0) ? n0 : -1;
            if (index > -1) {
                action(index - 1, lines);
            }
            dqCount = 0;
            prev = "";
            // index++;
        }
    }

    // public void UpdateCategory()
    // {
    //     showList = new int[]{};
    // }

    // // 表示リストの更新
    // public void UpdateCategory(CategoryType type)
    // {
    //     switch (type) {
    //         case CategoryType.None:
    //             showList = new int[]{};
    //             break;
    //         case CategoryType.Favorite:
    //             showList = DataTable.MyList.filterdList[(int)MyListType.Favorite].Value;
    //             break;
    //         case CategoryType.Have:
    //             showList = DataTable.MyList.filterdList[(int)MyListType.Have].Value;
    //             break;
    //         default:
    //             showList = list
    //                 .Select((v, i) => new { Value = v, Index = i })
    //                 .Where(v => v.Value.category == type)
    //                 .Select(v => v.Index)
    //                 .ToArray();
    //             break;
    //     }
    // }
}
