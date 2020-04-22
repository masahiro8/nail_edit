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
    // public float itemWidth = 400;
    // public ScrollRect scrollRect;
    // public GameObject itemPrefab;
    // public GameObject modelPrefab;
    public DrawMain main;
    public CommonList nailList;
    public CommonList categoryList;
    public CommonItem detailItem;
    // public Button[] favButton;
    // public ReactiveProperty<int> favFlag = new ReactiveProperty<int>(-1);

    private bool detailShow = true;

    // Start is called before the first frame update
    void Start()
    {
        // var favFlag = Enumerable
        //     .Repeat<ReactiveProperty<int>>(new ReactiveProperty<int>(-1), Enum.GetValues(typeof(MyListType)).Length)
        //     .ToArray();
        var max = Enum.GetValues(typeof(MyListType)).Length;
        ReactiveProperty<int>[] favFlag = new ReactiveProperty<int>[max];
        //     new ReactiveProperty<int>(-1),
        //     new ReactiveProperty<int>(-1),
        // };

        // お気に入りボタン
        // foreach (MyListType type in Enum.GetValues(typeof(MyListType))) {
        //     var i = (int)type;
        for (var i = 0; i < max; i++) {
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
                        button.gameObject.SetActive(b != orgFlag && nailList.itemIndex.Value > -1);
                    })
                    .AddTo(button.gameObject);
            }
        }

        // カテゴリ選択時に変更
        categoryList.itemIndex
            .Subscribe(n => {
                var index = DataTable.Category.showList[n];
                var data = DataTable.Category.list[index];
                DataTable.NailInfo.UpdateCategory(data.type);
                nailList.Reset();
            })
            .AddTo(gameObject);

        // ネイル選択時にモデルも変更
        nailList.itemIndex
            .Where(n => n > -1)
            .Subscribe(n => {
                UpdateDetail();
                // Debug.Log("Click: " + data.name);
                foreach (MyListType type in Enum.GetValues(typeof(MyListType))) {
                    var i = (int)type;
                    var index = DataTable.NailInfo.showList[nailList.itemIndex.Value];
                    var data = DataTable.NailInfo.list[index];
                    favFlag[i].Value = data.IsMyList(type) ? 1 : 0;
                }
            })
            .AddTo(gameObject);

        // 詳細ビューを拡大縮小させる
        detailItem.button[0].OnClickAsObservable()
            .Subscribe(_ => UpdateDetailShow())
            .AddTo(detailItem.gameObject);

        UpdateDetailShow();
    }

    private void UpdateDetail()
    {
        var index = DataTable.NailInfo.showList[nailList.itemIndex.Value];
        var data = DataTable.NailInfo.list[index];

        foreach (Transform t in main.nailDetection.transform) {
            t.GetComponent<NailGroup>().UpdateData(DataTable.NailInfo.list[index]);
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

    private void UpdateDetailShow()
    {
        detailShow = !detailShow;
        if (DOTween.IsTweening(detailItem)) {
            detailItem.DOComplete();
        }
        detailItem.transform
            .DOScale(Vector3.one * (detailShow ? 1f : 0.3f), DataTable.Param.duration);
    }

    private void SetFavorite(MyListType type, bool flag)
    {
        var index = DataTable.NailInfo.showList[nailList.itemIndex.Value];
        var data = DataTable.NailInfo.list[index];

        var key = type.ToString() + data.productCode;
        // SaveName.FavoriteItem.SetBool(data.name, !SaveName.FavoriteItem.GetBool(data.name));
        SaveName.MyListItem.SetBool(key, flag);
        Debug.Log("SetFavorite -> " + key + ": " + SaveName.MyListItem.GetBool(key));
    }
}
