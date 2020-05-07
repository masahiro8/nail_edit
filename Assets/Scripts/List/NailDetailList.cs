using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SuperScrollView;
using DG.Tweening;
using DanielLochner.Assets.SimpleScrollSnap;

public class NailDetailList : MonoBehaviour
{
    public DrawMain main;
    public NailSelectList list;
    public CommonList detailList;
    public CommonItem detailItem;
    public RectTransform[] animObj;
    public ReactiveProperty<int>[] favFlag = new ReactiveProperty<int>[Enum.GetValues(typeof(MyListType)).Length];

    // Start is called before the first frame update
    void Start()
    {
        detailList.updateItem = UpdateDetail;
        detailList.itemCount.Value = 2;
        detailList.GetComponent<SimpleScrollSnap>().Setup(true);

        var scrollRect = detailList.GetComponent<ScrollRect>();
        // var obj = scrollRect.content.GetChild(0);
        // var cItem = obj.GetComponent<CommonItem>();
        var scrollSnap = detailList.GetComponent<SimpleScrollSnap>();
        // Debug.Log(scrollRect.content.childCount);

        // detailList.updateItem = UpdateNail;
        detailList.itemCount.Value = 2;

        // ボタンサイズを変更
        // var listView = detailList.GetComponent<LoopListView2>();
        scrollRect.OnValueChangedAsObservable()
            .Subscribe(value => UpdateScroll(value))
            .AddTo(gameObject);
        UpdateScroll(Vector2.zero);

        // スクロールさせる
        detailItem.button[0].OnClickAsObservable()
            .Subscribe(_ => {
                scrollSnap.GoToPanel(1 - scrollSnap.TargetPanel);
            })
            .AddTo(gameObject);

        // お気に入りボタン
        for (var i = 0; i < favFlag.Length; i++) {
            var useFlag = new ReactiveProperty<int>(-1);
            favFlag[i] = useFlag;
            var type = (MyListType)i;
            for (var j = 0; j < 2; j++) { // オンオフ
                var orgFlag = j;
                var button = detailItem.button[j + 2 + i * 2];
                // ボタンを押した時にお気に入りフラグを更新
                button.OnClickAsObservable()
                    .Subscribe(_ => {
                        useFlag.Value = orgFlag;
                        SetFavorite(type, orgFlag == 1);
                        DataTable.MyList.Reset(type);
                    })
                    .AddTo(button.gameObject);

                // お気に入りフラグ更新時に表示状態を更新
                useFlag
                    .Subscribe(b => {
                        // Debug.Log("type: " + i + "," + type);
                        button.gameObject.SetActive(b != orgFlag && list.nailList.itemIndex.Value > -1);
                    })
                    .AddTo(button.gameObject);
            }
        }
    }

    private void UpdateScroll(Vector2 value)
    {
        // var sMin = 0.3f;
        // var sMax = 1f;
        // // var item = listView.GetShownItemByIndex(0);
        // var s = Math.Max(sMin, Math.Min(sMax, sMin + value.x * (sMax - sMin)));
        var s1 = 0.3f + value.x * 0.7f;
        var s2 = 0.6f + value.x * 0.4f;
        detailItem.button[0].transform.localScale = Vector3.one * s1;
        detailItem.image[0].transform.localScale = Vector3.one * s2;
        detailItem.image[0].transform.parent.GetComponent<RectTransform>()
            .anchoredPosition = Vector3.left * (1 - value.x) * 54;

        // 2つ目のアイテムはダミーなので消す
        var scrollRect = detailList.GetComponent<ScrollRect>();
        if (scrollRect.content.childCount > 1) {
            // var item = scrollRect.content.GetChild(1).GetComponent<CommonItem>();
            scrollRect.content.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void UpdateDetailIndex()
    {
        UpdateDetail(detailItem, 0);
    }

    private void UpdateDetail(CommonItem item, int index)
    {
        // 初期化時に-1が入ってくるので
        if (list.nailList.itemIndex.Value < 0) {
            return;
        }

        // var index2 = DataTable.NailInfo.showList[list.nailList.itemIndex.Value];
        // var data = DataTable.NailInfo.list[index2];
        var data = DataTable.Category.showList[list.categoryList.itemIndex.Value].filter.Value[list.nailList.itemIndex.Value];

        foreach (Transform t in main.nailDetection.transform) {
            t.GetComponent<NailGroup>().UpdateData(data);
        }

        detailItem.text[0].text = data.productName;
        detailItem.text[1].text = data.subName;
        detailItem.text[2].text = data.productCode;

        var texBottle = Resources.Load("Textures/NailBottle/" + data.fileName) as Texture2D;
        if (texBottle) {
            detailItem.image[0].texture = texBottle;
            detailItem.image[0].SetNativeSize();
            detailItem.image[0].enabled = true;
        } else {
            detailItem.image[0].enabled = false;
        }
    }

    // お気に入り、持っているフラグを更新
    private void SetFavorite(MyListType type, bool flag)
    {
        // var index = DataTable.NailInfo.showList[list.nailList.itemIndex.Value];
        // var data = DataTable.NailInfo.list[index];
        var data = DataTable.Category.showList[list.categoryList.itemIndex.Value].filter.Value[list.nailList.itemIndex.Value];

        var key = type.ToString() + data.productCode;
        // SaveName.FavoriteItem.SetBool(data.name, !SaveName.FavoriteItem.GetBool(data.name));
        SaveName.MyListItem.SetBool(key, flag);
        Debug.Log("SetFavorite -> " + key + ": " + SaveName.MyListItem.GetBool(key));
    }
}
