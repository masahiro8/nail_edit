using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SuperScrollView;
using DG.Tweening;

public class NailSelectList : MonoBehaviour
{
    public CommonList nailList;
    public CommonList categoryList;
    // public NailDetailList detailFrame;
    public Text noItemText;

    public GameObject nailListPrefab;
    public GameObject categoryListPrefab;

    public GameObject Panel;

    private ReactiveProperty<NailInfoRecord[]> nailArray = new ReactiveProperty<NailInfoRecord[]>(new NailInfoRecord[]{});
    private ReactiveProperty<CategoryRecord[]> categoryArray = new ReactiveProperty<CategoryRecord[]>(new CategoryRecord[]{});

    // Start is called before the first frame update
    void Start()
    {
        DataTable.Instance.addCallback( json => {
            Debug.Log("■■■■■■■■■■■■ callback" + json);
        });

        // ネイルのリストを配置
        if (nailListPrefab) {
            //カテゴリ選択とネイル選択をPanel内に配置する
            var nailObj = Instantiate(nailListPrefab, Panel.transform);
            nailList = nailObj.GetComponent<CommonList>();
            // noItemTextを探す
            foreach (Transform t in nailObj.transform) {
                var text = t.GetComponent<Text>();
                noItemText = text != null ? text : noItemText;
            }
        }

        // カテゴリーのリストを配置
        if (categoryListPrefab) {
            //カテゴリ選択とネイル選択をPanel内に配置する
            var cateObj = Instantiate(categoryListPrefab, Panel.transform);
            categoryList = cateObj.GetComponent<CommonList>();
        }

        // カテゴリ選択時に表示リストを更新
        foreach (var category in DataTable.Category.showList) {
            category.saveFlag
                .Subscribe(b => UpdateCategoryList())
                .AddTo(gameObject);
        }

        // 色カテゴリ選択時に表示リストを更新
        foreach (var category in DataTable.ColorCategory.showList) {
            category.saveFlag
                .Subscribe(b => UpdateNailList(categoryList.itemIndex.Value, DataTable.Param.filterType.Value))
                .AddTo(gameObject);
        }

        // カテゴリ変更時に表示リストを更新
        categoryList.itemIndex
            .SkipLatestValueOnSubscribe()
            .Subscribe(n => UpdateNailList(n, DataTable.Param.filterType.Value))
            .AddTo(gameObject);

        // フィルター変更時に表示リストを更新
        DataTable.Param.filterType
            // .SkipLatestValueOnSubscribe() // 一個は通して作成しておく
            .Subscribe(type => UpdateNailList(categoryList.itemIndex.Value, type))
            .AddTo(gameObject);

        nailList.updateItem = UpdateNail;
        // nailList.itemCount.Value = list.Value.Length;
        categoryList.updateItem = UpdateCategory;
        // categoryList.itemCount.Value = DataTable.Category.showList.Length;

        // リスト変更時に表示リストを更新
        nailArray
            .Subscribe(list => {
                // アイテムなしのテキスト表示
                noItemText.enabled = list.Length == 0;
                nailList.itemCount.SetValueAndForceNotify(list.Length);
                // 初回は最初のものを取得
                if (DataTable.Param.selectedNail.Value == null && nailList.itemIndex.Value < list.Length) {
                    DataTable.Param.selectedNail.Value = list[nailList.itemIndex.Value];
                }
                // 選択済みの項目がリスト内にあるかどうか調べる
                var res = list
                    .Select((v, i) => new { Value = v, Index = i })
                    .Where(v => v.Value == DataTable.Param.selectedNail.Value)
                    .ToArray();
                // 選択済みの項目がリスト内にある場合は選択状態の変更
                nailList.itemIndex.SetValueAndForceNotify(res.Length > 0 ? res[0].Index : -1);
            })
            .AddTo(gameObject);

        // カテゴリリスト変更時に表示リストを更新
        categoryArray
            .Subscribe(v => {
                categoryList.itemCount.SetValueAndForceNotify(v.Length);
            })
            .AddTo(gameObject);

        // ネイル選択時にモデルも変更
        nailList.itemIndex
            .Where(n => n > -1)
            .Where(n => n < nailArray.Value.Length)
            .Subscribe(n => {
                // detailFrame.UpdateDetailIndex();
                // Debug.Log("Click: " + data.name);
                // ここは下のでループしたい
                // DataTable.MyList.list
                // var data = nailArray.Value[n];
                // foreach (MyListType type in Enum.GetValues(typeof(MyListType))) {
                //     var i = (int)type;
                //     // var index = DataTable.NailInfo.showList[nailList.itemIndex.Value];
                //     // var data = DataTable.NailInfo.list[index];
                //     detailFrame.favFlag[i].Value = data.IsMyList(type) ? 1 : 0;
                // }
                DataTable.Param.selectedNail.Value = nailArray.Value[n];
            })
            .AddTo(gameObject);

        noItemText.text = "NoNailToChoose".Localized();
        noItemText.enabled = false;
    }

    // void Update()
    // {
    //     // 初回のみ
    //     if (nailList.itemIndex.Value < 0) {
    //         nailList.itemIndex.Value = 0;
    //     }
    // }

    private void UpdateNail(CommonItem item, int index)
    {
        if (index >= nailArray.Value.Length) {
            return;
        }

        // var data = DataTable.NailInfo.list[DataTable.NailInfo.showList[index]];
        var data = nailArray.Value[index];
        // text[0].text = "No." + data.index;
        item.text[0].text = data.colorNumber;

        item.svgImage[0].enabled = data.IsNew(); // new
        item.svgImage[1].enabled = data.IsMyList(MyListType.Favorite); // favorite
        item.image[1].enabled = data.IsLimited(); // limited
        // item.image[1].enabled = data.IsMyList(MyListType.Have); // have（仮）

        // var key = data.productCode.Substring(0, 4) + "_" + data.colorNumber;
        // var texBottle = Resources.Load("Textures/NailBottle/" + data.fileName) as Texture2D;
        data.SetSampleTexture(item.image[0]);
    }

    private void UpdateCategory(CommonItem item, int index)
    {
        var data = categoryArray.Value[index];
        item.text[0].text = data.name.Localized();
    }

    // ネイル表示リストを更新
    private void UpdateNailList(int n, NailFilterType fType)
    {
        nailArray.SetValueAndForceNotify(DataTable.NailInfo.list
            // カテゴリーでフィルター
            .Where(v => v.category == categoryArray.Value[n]
                // 公開日から終了日まで
                && v.IsDisp()
                // さらに色カテゴリでフィルター
                && v.colorCategory.IsShow
                // さらにNailFilterTypeでフィルター
                && fType.IsShow(v))
            .ToArray());

        // Debug.Log("UpdateNailList(" + n + "," + fType + "): " + list.Value.Length);
    }

    // カテゴリ表示リストを更新
    private void UpdateCategoryList()
    {
        categoryArray.SetValueAndForceNotify(DataTable.Category.showList
            // カテゴリーでフィルター
            .Where(v => !v.saveFlag.Value)
            .ToArray());

        UpdateNailList(categoryList.itemIndex.Value, DataTable.Param.filterType.Value);

        // Debug.Log("UpdateNailList(" + n + "," + fType + "): " + list.Value.Length);
    }

    // カテゴリ表示リストを更新
    public NailInfoRecord GetCurrentNail()
    {
        return nailArray.Value[nailList.itemIndex.Value];
    }
}
