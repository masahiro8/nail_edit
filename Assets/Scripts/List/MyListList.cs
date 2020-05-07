using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using SuperScrollView;
using DanielLochner.Assets.SimpleScrollSnap;

public class MyListList : MonoBehaviour
{
    public CommonList frameList;
    public CommonList selectList;
    public Button closeButton;
    public Image backgroundImage; // 画面全体を暗くするための背景
    public RectTransform frameTransform; // メニュー用の黒い背景
    public RectTransform menuTransform; // メニュー内部

    // private SimpleScrollSnap scrollSnap;
    private Color orgColor; // 画面全体を暗くするための背景の元の色

    void OnDestroy()
    {
        backgroundImage.DOKill();
        frameTransform.DOKill();
        menuTransform.DOKill();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 編集用に横にずらしているのを元に戻す
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // frameList.updateItem = UpdateSelect;
        frameList.itemCount.Value = DataTable.MyList.list.Length;
        selectList.updateItem = UpdateSelect;
        selectList.itemCount.Value = DataTable.MyList.list.Length;

        // ボタン押し
        if (closeButton) {
            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseMenu())
                .AddTo(gameObject);
        }

        // アニメーション用に色の保存
        orgColor = backgroundImage.color;
        // CloseMenu();

        // 閉じておく
        CloseMenu();

        // お気に入りと持っているの切り替え時
        selectList.itemIndex
            .Zip(selectList.itemIndex.Skip(1), (x, y) => new System.Tuple<int, int>(x, y))
            .Subscribe(value => {
                // var scrollRect = frameList.GetComponent<ScrollRect>();
                // // scrollRect.ani.MovePanelToItemIndex(value.Item2, 0);
                // float f = (float)value.Item2 / (float)(scrollRect.content.transform.childCount - 1);
                // scrollRect.horizontalNormalizedPosition = f;
                var scrollSnap = frameList.GetComponent<SimpleScrollSnap>();
                // scrollRect.ani.MovePanelToItemIndex(value.Item2, 0);
                scrollSnap.GoToPanel(value.Item2);
            })
            .AddTo(gameObject);

        // お気に入りが更新されたので表示リストを更新する
        foreach (MyListType type in Enum.GetValues(typeof(MyListType))) {
            var i = (int)type;
            var scrollRect = frameList.GetComponent<ScrollRect>();
            var commonList = scrollRect.content.GetChild(i).GetChild(0).GetComponent<CommonList>();
            // commonList.itemType = type.GetItemType();
            commonList.updateItem = (item2, index2) => UpdateMyList(item2, index2, type);
            commonList.itemCount.Value = DataTable.MyList.filterdList[(int)type].Value.Length;
            DataTable.MyList.filterdList[i]
                .Subscribe(l => {
                    // var commonList = frameList.GetComponent<CommonList>();
                    commonList.Reset();
                })
                .AddTo(gameObject);
        }
    }

    private void UpdateSelect(CommonItem item, int index)
    {
        var data = DataTable.MyList.list[index];
        item.text[0].text = data.name.Localized();
    }

    private void UpdateMyList(CommonItem item, int index, MyListType mlType)
    {
        var data = DataTable.NailInfo.list[DataTable.MyList.filterdList[(int)mlType].Value[index]];
        item.text[0].text = data.productCode;
    }

    // 開く
    public void OpenMenu(int n)
    {
        CompleteAnimation();

        // リストを更新
        DataTable.MyList.Reset((MyListType)n);

        // 選択状態を更新
        selectList.itemIndex.Value = n;

        // アニメーションを見せないために強制移動
        var scrollRect = frameList.GetComponent<ScrollRect>();
        float f = (float)n / (float)(scrollRect.content.transform.childCount - 1);
        scrollRect.horizontalNormalizedPosition = f;

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

    // 閉じる
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
