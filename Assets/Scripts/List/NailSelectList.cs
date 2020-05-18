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
    public NailDetailList detailFrame;

    // Start is called before the first frame update
    void Start()
    {
        nailList.updateItem = UpdateNail;
        nailList.itemCount.Value = DataTable.Category.list[0].filter.Value.Length;
        categoryList.updateItem = UpdateCategory;
        categoryList.itemCount.Value = DataTable.Category.showList.Length;

        // カテゴリ選択時に変更
        categoryList.itemIndex
            .Subscribe(n => {
                var data = DataTable.Category.showList[n];
                data.UpdateShowList();
                // DataTable.NailInfo.UpdateCategory(data.type);
                // nailList.itemCount.Value = DataTable.NailInfo.showList.Length;
                nailList.itemCount.Value = data.filter.Value.Length;
            })
            .AddTo(gameObject);

        // ネイル選択時にモデルも変更
        nailList.itemIndex
            .Where(n => n > -1)
            .Subscribe(n => {
                detailFrame.UpdateDetailIndex();
                // Debug.Log("Click: " + data.name);
                // ここは下のでループしたい
                // DataTable.MyList.list
                var data = DataTable.Category.showList[categoryList.itemIndex.Value].filter.Value[n];
                foreach (MyListType type in Enum.GetValues(typeof(MyListType))) {
                    var i = (int)type;
                    // var index = DataTable.NailInfo.showList[nailList.itemIndex.Value];
                    // var data = DataTable.NailInfo.list[index];
                    detailFrame.favFlag[i].Value = data.IsMyList(type) ? 1 : 0;
                }
            })
            .AddTo(gameObject);
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
        if (index >= DataTable.Category.showList[categoryList.itemIndex.Value].filter.Value.Length) {
            return;
        }

        // var data = DataTable.NailInfo.list[DataTable.NailInfo.showList[index]];
        var data = DataTable.Category.showList[categoryList.itemIndex.Value].filter.Value[index];
        // text[0].text = "No." + data.index;
        item.text[0].text = data.colorNumber;

        // テスト
        var nailData = Resources.Load<NailMaterialTable>("Data/NailMaterial/" + data.fileName2);
        if (nailData) {
            item.svgImage[0].enabled = nailData.list.Length > 0; // new
        } else {
            item.svgImage[0].enabled = false; // new
        }
        item.svgImage[1].enabled = index % 10 == 0; // dot

        // var key = data.productCode.Substring(0, 4) + "_" + data.colorNumber;
        // var texBottle = Resources.Load("Textures/NailBottle/" + data.fileName) as Texture2D;
        data.SetSampleTexture(item.image[0]);
    }

    private void UpdateCategory(CommonItem item, int index)
    {
        var data = DataTable.Category.showList[index];
        item.text[0].text = data.name.Localized();
    }
}
