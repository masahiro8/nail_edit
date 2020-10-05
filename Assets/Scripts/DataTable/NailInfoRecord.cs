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
    public DateTime openDate; // 公開日: 2020/7/22
    public DateTime releaseDate; // 発売年月: 2015/02/01
    public DateTime endDate; // 販売終了日: 2099/12/31
    public int limited; // 限定フラグ

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
    public string[] url = new string[7];

    // フィルター用
    [System.NonSerialized] public CategoryRecord category;
    [System.NonSerialized] public ColorCategoryRecord colorCategory;

    // 追加データ
    public DateTime newDispDate; // NEWを表示する期間

    public string fileName {
        get { return productCode.Substring(0, 4) + "_" + colorNumber; }
    }

    public string fileName2 {
        get { return colorNumber; }
        // get { return category.categoryId + "_" + colorNumber; }
    }

    public Texture2D sampleTexture {
        get { return Resources.Load<Texture2D>("Textures/NailSample/" + fileName); }
    }

    public string useURL {
        get {
            var n = Math.Min(SaveName.StoreSelect.GetInt(), DataTable.Store.showList.Length - 1);
            var data = DataTable.Store.showList[n];
            var res = data.index < url.Length ? url[data.index] : null;
            return res != null ? res : "";
        }
    }

    public bool existUseURL {
        get { return useURL.Length > 0; }
    }

    //Csvからの値
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
        limited = int.TryParse(lines[11], out int n11) ? n11 : 0;
        openDate = DateTime.TryParse(lines[12], out DateTime n12) ? n12 : DateTime.Now;

        newDispDate = releaseDate.AddDays(DataTable.Param.newInfoDays);
    }

    //APiからの値
    public void AddProductFromApi(string[] lines)
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
        limited = int.TryParse(lines[11], out int n11) ? n11 : 0;
        openDate = DateTime.TryParse(lines[12], out DateTime n12) ? n12 : DateTime.Now;
    }

    // カテゴリマスタの読み込み
    public void AddCategory(string[] lines)
    {
        if (lines.Length < 7) {
            return;
        }
        var categoryId = int.TryParse(lines[4], out int n4) ? n4 : 9999;

        // カテゴリIDからカテゴリを取得
        // var res = DataTable.Category.list.Where(value => value.categoryId == categoryId).ToArray();
        // category = res.Length > 0 ? res[0] : DataTable.Category.list[0];
        category = DataTable.Category.GetCategory(categoryId, lines[5]);

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

    public bool IsDisp()
    {
        return openDate <= System.DateTime.Now && !IsSaleDone();
    }

    public bool IsNew()
    {
        return newDispDate > System.DateTime.Now;
    }

    public bool IsLimited()
    {
        return limited > 0;
    }

    public bool IsSaleDone()
    {
        return endDate < System.DateTime.Now;
    }

    public bool IsMyList(MyListType type)
    {
        return SaveName.MyListItem.GetBool(type.ToString() + productCode);
    }

    public void SetBottleTexture(RawImage image)
    {
        var tex = Resources.Load<Texture2D>("Textures/NailBottle/" + fileName);
        if (tex) {
            image.texture = tex;
            image.enabled = true;
        } else {
            image.enabled = false;
        }
    }

    public void SetSampleTexture(RawImage image)
    {
        var tex = Resources.Load<Texture2D>("Textures/NailSample/" + fileName);
        if (tex) {
            image.texture = tex;
            // ネイルのサムネイルをRect Transofrom側で設定した値を使う
            // image.SetNativeSize();
            image.enabled = true;
        } else {
            image.enabled = false;
        }
    }

    public void SetFavoriteTexture(SVGImage svg)
    {
       svg.sprite = Resources.Load<Sprite>("Textures/Button/Have_2");
    }
}
