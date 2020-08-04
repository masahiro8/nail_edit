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
    // public Image backgroundImage; // 画面全体を暗くするための背景
    public RectTransform frameTransform; // メニュー用の黒い背景
    public RectTransform menuTransform; // メニュー内部
    public PageScrollView pageScrollView; // 閉じる時に消すため

    private Color orgColor; // 画面全体を暗くするための背景の元の色

    // Start is called before the first frame update
    void Start()
    {
        // 全体を消しておく
        // gameObject.SetActive(false);
        pageScrollView.parentList = gameObject;
        pageScrollView.closeToDestroy = true;

        // アニメーション用に色の保存
        // orgColor = backgroundImage.color;

        // if (openButton) {
        //     openButton.OnClickAsObservable()
        //         .Subscribe(_ => OpenMenu())
        //         .AddTo(gameObject);
        // }

        if (closeButton) {
            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseMenu(true))
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

        // カテゴリのフィルター配列が変更された時に表示を更新
        foreach (var data in DataTable.Category.showList) {
            data.filter
                .SkipLatestValueOnSubscribe()
                .Subscribe(v => {
                    // Debug.Log("Color Reset -> " + data.name + ": " + data.filter.Value.Length + "," + v.Length);
                    detailList.Reset();
                })
                .AddTo(gameObject);
        }

        CloseMenu(false);
        OpenMenu();
    }

    // 更新
    public void UpdateColorCategory(CommonItem item, int index)
    {
        // カラーリストを取得
        var colorGrid = item.extra[0].GetComponent<CommonGrid>();

        colorGrid.updateItem = UpdateColorCircle;
        colorGrid.itemCount.Value = DataTable.ColorCategory.showList.Length;
    }

    // 更新
    public void UpdateColorCircle(CommonItem item, int index)
    {
        var data = DataTable.ColorCategory.showList[index];
        var flag1 = data.colors.Length > 0;
        var flag2 = data.colors.Length > 1;

        // カラー1の円を表示（常時表示）
        item.svgImage[1].enabled = flag1;
        // カラー1が白の場合に背景色と同じなので灰色の枠を表示
        // item.svgImage[2].enabled = flag1 && data.colors[0] == Color.white; // 白い画面の時
        item.svgImage[2].enabled = flag2 && data.colors[1] == Color.black;
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
                // Debug.Log(index);
                if (!data.saveFlag.Value
                    && DataTable.Param.selectedNail != null
                    && DataTable.Param.selectedNail.Value.colorCategory == data) {
                    // 使用中のネイルが入っている場合は解除不可
                    NPBinding.UI.ShowAlertDialogWithSingleButton("DialogErrorTitle".Localized(), "ColorSelectError1".Localized(), "OK".Localized(), null);
                } else {
                    data.saveFlag.Value = !data.saveFlag.Value;
                }
            });
        item.disposableBag.Add(disposable);
    }

    // 更新
    public void UpdateGroup(CommonItem item, int index)
    {
        var data = DataTable.Category.showList[index];
        item.text[0].text = data.name.Localized();

        // ネイルリストを取得
        var nailGrid = item.extra[0].GetComponent<CommonGrid>();

        // フラグの状態によってボタンの表示制御
        var disposable = data.saveFlag
            .Subscribe(flag => {
                // Debug.Log(data.name + ": " + flag);
                item.button[0].gameObject.SetActive(!flag);
                item.button[1].gameObject.SetActive(flag);
            });
        item.disposableBag.Add(disposable);

        // シリーズの表示のトグルをボタンで代用、0:Off 1:On
        for (var i = 0; i < 2; i++) {
            var flag = i == 0;
            disposable = item.button[i].OnClickAsObservable()
                .Subscribe(_ => {
                    var total = DataTable.Category.showList
                        .Where(v => !v.saveFlag.Value)
                        .ToArray();
                    if (flag && data.ContainsNail(DataTable.Param.selectedNail.Value)) {
                        // 使用中のネイルが入っている場合は解除不可
                        NPBinding.UI.ShowAlertDialogWithSingleButton("DialogErrorTitle".Localized(), "ColorSelectError1".Localized(), "OK".Localized(), null);
                    } else if (flag && total.Length <= 1) {
                        // 最後の1個の時にオフにしようとした場合はエラーを出す
                        NPBinding.UI.ShowAlertDialogWithSingleButton("DialogErrorTitle".Localized(), "ColorSelectError2".Localized(), "OK".Localized(), null);
                    } else {
                        data.saveFlag.Value = flag;
                    }
                });
            item.disposableBag.Add(disposable);
        }

        // ネイルリストの大きさを変更
        var n = (data.filter.Value.Length + 7) / 8;
        var item3 = item.GetComponent<LoopListViewItem2>();
        item3.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20 + 70 * n);
        item.extra[0].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 70 * n);

        // ネイルリストの設定
        nailGrid.updateItem = (item2, index2) => UpdateNail(item2, index2, index);
        // 先にセルのサイズを変えてから更新しないと範囲外が表示されない
        nailGrid.itemCount.SetValueAndForceNotify(data.filter.Value.Length);

        // Debug.Log(data.name + ": " + nailGrid.itemCount.Value + "," + n);
    }

    // 更新
    public void UpdateNail(CommonItem item, int index, int category)
    {
        // var index1 = DataTable.Category.showList[category];
        // var index2 = DataTable.Category.list[index1].filter.Value[index];
        // var data = DataTable.NailInfo.list[index2];
        var data = DataTable.Category.showList[category].filter.Value[index];

        item.text[0].text = data.colorNumber;
        item.image[1].enabled = data.IsLimited();
        item.svgImage[0].enabled = data.IsNew();
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
        gameObject.SetActive(true);

        CompleteAnimation();

        // backgroundImage.raycastTarget = true;
        // backgroundImage
        //     .DOColor(orgColor, DataTable.Param.duration);
        frameTransform
            .DOAnchorPosY(-667, DataTable.Param.duration);
        menuTransform
            .DOAnchorPosY(-32, DataTable.Param.duration);
    }

    // 閉じる
    public void CloseMenu(bool anim)
    {
        CompleteAnimation();

        // backgroundImage.raycastTarget = false;
        // backgroundImage
        //     .DOColor(Color.clear, DataTable.Param.duration);
        // frameTransform
        //     .DOAnchorPosY(-Screen.height, DataTable.Param.duration);
        menuTransform
            .DOAnchorPosY(-Screen.height, DataTable.Param.duration);
    }

    // アニメーションを完了させる
    private void CompleteAnimation()
    {
        // if (DOTween.IsTweening(backgroundImage)) {
        //     backgroundImage.DOComplete();
        // }
        if (DOTween.IsTweening(frameTransform)) {
            frameTransform.DOComplete();
        }
        if (DOTween.IsTweening(menuTransform)) {
            menuTransform.DOComplete();
        }
    }
}
