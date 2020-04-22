using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using SubjectNerd.Utilities;

public class NailInfoRecord
{
    public int index = -1; // No: 1
    public string productName; // 商品名: ネイルホリック
    public string subName; // サブ: Classic
    public string colorNumber; // 色番: WT005
    // 爪サンプル
    // 質感
    // 塗った回数
    public int orderCode; // 発注コード: 44513
    public string productCode; // 商品符号: DLMI005
    // チップ画像
    // 商品画像
    public int price; // 税抜価格: ¥300
    public DateTime releaseDate; // 発売年月: 2015/02
    public string janCode; // JANコード: 4971710445138
    public string url; // MaisonKOSE: https://maison.kose.co.jp/site/nailholic/g/gDLMI005/
    // Amazon
    // 楽天
    // ロハコ
    // ＠コスメ
    // 流通EC
    public CategoryType category;

    public string fileName {
        get { return productCode.Substring(0, 4) + "_" + colorNumber; }
    }

    public NailInfoRecord(string str)
    {
        var lines = str.Split(',');
        if (lines.Length < 15) {
            return;
        }
        index = int.TryParse(lines[0], out int n0) ? n0 : -1;
        productName = lines[1];
        subName = lines[2];
        colorNumber = lines[3];
        orderCode = int.TryParse(lines[7], out int n7) ? n7 : -1;
        productCode = lines[8];
        price = int.TryParse(lines[11].Replace("¥", ""), out int n11) ? n11 : -1;
        releaseDate = DateTime.TryParse(lines[12], out DateTime n12) ? n12 : DateTime.Now;
        janCode = lines[13];
        url = lines[14];

        category = DataTable.Category.GetTypeFromString(subName);
        if (index > -1 && category == CategoryType.None) {
            Debug.Log("Error: " + str);
        }
    }

    public bool IsMyList(MyListType type)
    {
        return SaveName.MyListItem.GetBool(type.ToString() + productCode);
    }
}
