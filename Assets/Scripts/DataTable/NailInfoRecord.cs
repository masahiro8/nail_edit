using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SubjectNerd.Utilities;

public class NailInfoRecord
{
    // ======== 商品マスタ ========
    public int index = -1; // No: 1
    public string productName; // 商品名: ネイルホリック
    public string subName; // サブ: Classic
    public string colorNumber; // 色番: WT005
    // 爪サンプル
    // 質感
    // 塗った回数
    // public int orderCode; // 発注コード: 44513
    public string productCode; // 商品符号: DLMI005
    public int price; // 税抜価格: ¥300
    public DateTime releaseDate; // 発売年月: 2015/02/01
    public DateTime endDate; // 販売終了日: 2099/12/31
    // 限定フラグ

    // ======== カテゴリマスタ ========
    // No: 1
    // 商品名: ネイルホリック
    // サブ: Classic
    // 色番: WT005
    // カテゴリID: 1015
    // カテゴリ名: Classic color
    // 色カテゴリ: 白/黒

    // ======== EC遷移マスタ ========
    // No: 1
    // 商品名: ネイルホリック
    // 商品符号: DLMI005
    // MaisonKOSE: https://maison.kose.co.jp/site/nailholic/g/gDLMI005/
    // Amazon: https://www.amazon.co.jp/dp/B01CNFAN4W
    // 楽天: https://item.rakuten.co.jp/rakuten24/4971710445138/
    // ロハコ: https://lohaco.jp/product/J479944/
    // ＠コスメ: 
    // 流通EC: 
    public string[] url = new string[4];

    // フィルター用
    [System.NonSerialized] public CategoryRecord category;
    [System.NonSerialized] public ColorCategoryRecord colorCategory;

    public string fileName {
        get { return productCode.Substring(0, 4) + "_" + colorNumber; }
    }

    public string fileName2 {
        get { return category.categoryId + "_" + colorNumber; }
    }

    public Texture2D sampleTexture {
        get { return Resources.Load<Texture2D>("Textures/NailSample/" + fileName); }
    }

    public void AddProduct(string[] lines)
    {
        if (lines.Length < 12) {
            return;
        }
        productName = lines[1];
        subName = lines[2];
        colorNumber = lines[3];
        // orderCode = int.TryParse(lines[7], out int n7) ? n7 : -1;
        productCode = lines[7];
        price = int.TryParse(lines[8].Replace("¥", ""), out int n8) ? n8 : -1;
        releaseDate = DateTime.TryParse(lines[9], out DateTime n9) ? n9 : DateTime.Now;
        endDate = DateTime.TryParse(lines[10], out DateTime n10) ? n10 : DateTime.Now;
    }

    // カテゴリマスタの読み込み
    public void AddCategory(string[] lines)
    {
        if (lines.Length < 7) {
            return;
        }
        var categoryId = int.TryParse(lines[4], out int n4) ? n4 : 9999;

        // カテゴリIDからカテゴリを取得
        var res = DataTable.Category.list.Where(value => value.categoryId == categoryId).ToArray();
        category = res.Length > 0 ? res[0] : DataTable.Category.list[0];

        // カラーカテゴリIDからカラーカテゴリを取得
        colorCategory = DataTable.ColorCategory.GetRecordFromNumber(colorNumber);
    }

    // EC遷移マスタの読み込み
    public void AddURL(string[] lines)
    {
        if (lines.Length < 9) {
            return;
        }
        const string comp = "http";
        for (var i = 0; i < url.Length; i++) {
            if (lines[3 + i].Length >= comp.Length && lines[3 + i].Substring(0, comp.Length) == comp) {
                url[i] = lines[3 + i];
            }
        }
    }

    public bool IsMyList(MyListType type)
    {
        return SaveName.MyListItem.GetBool(type.ToString() + productCode);
    }

    public void SetSampleTexture(RawImage image)
    {
        var texSample = sampleTexture;
        if (texSample) {
            image.texture = texSample;
            image.SetNativeSize();
            image.enabled = true;
        } else {
            image.enabled = false;
        }
    }
}
