using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

// ページスクロールビュー.
public class PageScrollViewHorizontal : ScrollRect
{
    // 1ページの幅.
    // private float pageWidth;
    private float pageWidth;
    // 前回のページIndex. 最も左を0とする.
    private int prevPageIndex = 0;
    private bool firstDrag = true;

    protected override void Awake()
    {
        base.Awake();

        GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
        // 1ページの幅を取得.
        // pageWidth = grid.cellSize.x + grid.spacing.x;
        pageWidth = grid.cellSize.x + grid.spacing.x;
    }

    // ドラッグを開始したとき.
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    // ドラッグを終了したとき.
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        // ドラッグを終了したとき、スクロールをとめます.
        // スナップさせるページが決まった後も慣性が効いてしまうので.
        StopMovement();

        // スナップさせるページを決定する.
        // スナップさせるページのインデックスを決定する.
        int pageIndex = Mathf.RoundToInt(content.anchoredPosition.x / pageWidth);
        // ページが変わっていない且つ、素早くドラッグした場合.
        // ドラッグ量の具合は適宜調整してください.
        if (firstDrag) {
            firstDrag = false;
            prevPageIndex = pageIndex;
        }
        // Debug.Log("pageIndex: " + pageIndex + "," + prevPageIndex);
        if (pageIndex == prevPageIndex && Mathf.Abs(eventData.delta.x) >= 5)
        {
            pageIndex += (int)Mathf.Sign(eventData.delta.x);
        }

        // Contentをスクロール位置を決定する.
        // 必ずページにスナップさせるような位置になるところがポイント.
        float destX = pageIndex * pageWidth;
        content.DOAnchorPosX(destX,0.3f);

        // 「ページが変わっていない」の判定を行うため、前回スナップされていたページを記憶しておく.
        prevPageIndex = pageIndex;
    }
}