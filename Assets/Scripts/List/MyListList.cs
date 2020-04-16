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
        // ボタン押し
        if (closeButton) {
            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseMenu())
                .AddTo(gameObject);
        }

        // アニメーション用に色の保存
        orgColor = backgroundImage.color;
        // CloseMenu();

        // メニュー作成
        CloseMenu();

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
    }

    // 開く
    public void OpenMenu(int n)
    {
        CompleteAnimation();

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
