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

public class ColorSelectList : MonoBehaviour
{
    public DrawMain main;
    public CommonList detailList;
    public CommonItem detailItem;
    public Button openButton;
    public Button closeButton;
    public Image backgroundImage; // 画面全体を暗くするための背景
    public RectTransform frameTransform; // メニュー用の黒い背景
    public RectTransform menuTransform; // メニュー内部

    private Color orgColor; // 画面全体を暗くするための背景の元の色

    // Start is called before the first frame update
    void Start()
    {
        // 編集用に横にずらしているのを元に戻す
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // アニメーション用に色の保存
        orgColor = backgroundImage.color;

        if (openButton) {
            openButton.OnClickAsObservable()
                .Subscribe(_ => OpenMenu())
                .AddTo(gameObject);
        }

        if (closeButton) {
            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseMenu())
                .AddTo(gameObject);
        }

        // アイテムの内容更新
        detailList.updateItem = (item, index) => {
            switch (index) {
                case 0:
                    item.text[0].text = "Color".Localized();
                    break;
                case 1:
                    UpdateColorCategory(item, index);
                    break;
                case 2:
                    item.text[0].text = "Series".Localized();
                    break;
                default:
                    UpdateGroup(item, index - 3);
                    break;
            }
        };
        detailList.itemPrefabIndex = (n) => {
            int[] res = { 0, 1, 0, 2 };
            return res[n < res.Length ? n : res.Length - 1];
        };
        detailList.itemCount.Value = DataTable.Category.showList.Length + 3;

        // カラーカテゴリのオンオフによる表示更新
        foreach (var data in DataTable.Category.showList) {
            data.filter
                .SkipLatestValueOnSubscribe()
                .Subscribe(v => {
                    // Debug.Log("Color Reset -> " + data.name + ": " + data.filter.Value.Length + "," + v.Length);
                    detailList.Reset();
                })
                .AddTo(gameObject);
        }

        CloseMenu();
    }

    // 更新
    public void UpdateColorCategory(CommonItem item, int index)
    {
        // カラーリストを取得
        var colorGrid = item.extra[0].GetComponent<CommonGrid>();

        colorGrid.updateItem = UpdateColorCircle;
        colorGrid.itemCount.Value = DataTable.ColorCategory.list.Length;
    }

    // 更新
    public void UpdateColorCircle(CommonItem item, int index)
    {
        var data = DataTable.ColorCategory.list[index];
        var flag1 = data.colors.Length > 0;
        var flag2 = data.colors.Length > 1;

        // カラー1の円を表示（常時表示）
        item.svgImage[1].enabled = flag1;
        // カラー1が白の場合に背景色と同じなので灰色の枠を表示
        item.svgImage[2].enabled = flag1 && data.colors[0] == Color.white;
        // カラー2の半円を表示
        item.svgImage[3].enabled = flag2;
        // カラー1
        item.svgImage[1].color = flag1 ? data.colors[0] : Color.white;
        // カラー2
        item.svgImage[3].color = flag2 ? data.colors[1] : Color.white;
        // ゴールドとシルバー用に白の光沢を表示
        item.image[0].enabled = data.gloss;

        // 値が変化したときに選択済み表示を更新
        var disposable = data.saveFlag
            .Subscribe(flag => item.svgImage[0].enabled = !flag);
        item.disposableBag.Add(disposable);

        // ボタンが押されたときにフラグの切り替え
        disposable = item.button[0].OnClickAsObservable()
            .Subscribe(_ => {
                data.saveFlag.Value = !data.saveFlag.Value;
                // Debug.Log(index);
            });
        item.disposableBag.Add(disposable);
    }

    // 更新
    public void UpdateGroup(CommonItem item, int index)
    {
        var data = DataTable.Category.showList[index];
        item.text[0].text = data.name.Localized();

        // シリーズの非表示スイッチを取得
        var categoryToggle = item.extra[0].GetComponent<Toggle>();
        // ネイルリストを取得
        var nailGrid = item.extra[1].GetComponent<CommonGrid>();

        // シリーズの非表示スイッチを取得
        categoryToggle.isOn = !data.saveFlag.Value;
        var disposable = categoryToggle.OnValueChangedAsObservable()
            .Subscribe(flag => data.saveFlag.Value = !flag);
        item.disposableBag.Add(disposable);

        // ネイルリストの設定
        nailGrid.updateItem = (item2, index2) => UpdateNail(item2, index2, index);
        nailGrid.itemCount.Value = data.filter.Value.Length;
        // Debug.Log(data.name + ": " + data.filter.Value.Length);

        // ネイルリストの大きさを変更
        var n = (data.filter.Value.Length + 7) / 8;
        var item3 = item.GetComponent<LoopListViewItem2>();
        item3.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80 + 180 * n);
        item.extra[1].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 180 * n);
    }

    // 更新
    public void UpdateNail(CommonItem item, int index, int category)
    {
        // var index1 = DataTable.Category.showList[category];
        // var index2 = DataTable.Category.list[index1].filter.Value[index];
        // var data = DataTable.NailInfo.list[index2];
        var data = DataTable.Category.showList[category].filter.Value[index];

        item.text[0].text = data.productCode;
        data.SetSampleTexture(item.image[0]);
        var disposable = item.button[0].OnClickAsObservable()
            .Subscribe(_ => {
                Debug.Log(index + ": " + data.productCode);
            });
        item.disposableBag.Add(disposable);
    }

    // 開く
    public void OpenMenu()
    {
        CompleteAnimation();

        backgroundImage.raycastTarget = true;
        backgroundImage
            .DOColor(orgColor, DataTable.Param.duration);
        frameTransform
            .DOAnchorPosY(0, DataTable.Param.duration);
        menuTransform
            .DOAnchorPosY(0, DataTable.Param.duration);
    }

    // 閉じる
    public void CloseMenu()
    {
        CompleteAnimation();

        backgroundImage.raycastTarget = false;
        backgroundImage
            .DOColor(Color.clear, DataTable.Param.duration);
        frameTransform
            .DOAnchorPosY(-Screen.height, DataTable.Param.duration);
        menuTransform
            .DOAnchorPosY(-Screen.height, DataTable.Param.duration);
    }

    // アニメーションを完了させる
    private void CompleteAnimation()
    {
        if (DOTween.IsTweening(backgroundImage)) {
            backgroundImage.DOComplete();
        }
        if (DOTween.IsTweening(frameTransform)) {
            frameTransform.DOComplete();
        }
        if (DOTween.IsTweening(menuTransform)) {
            menuTransform.DOComplete();
        }
    }
}
