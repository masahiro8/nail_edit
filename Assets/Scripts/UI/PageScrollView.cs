using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

// ページスクロールビュー
public class PageScrollView : ScrollRect
{
    public GameObject parentList;
    public bool closeToDestroy = false;

    // 1ページの幅
    // private float pageWidth;
    private float pageHeight;
    // 前回のページIndex
    private int prevPageIndex = 0;
    private bool firstDrag = true;

    protected override void Awake()
    {
        base.Awake();

        GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();
        // 1ページの幅を取得.
        // pageWidth = grid.cellSize.x + grid.spacing.x;
        pageHeight = grid.cellSize.y + grid.spacing.y;

        //位置を初期化
        content.anchoredPosition = new Vector2(content.anchoredPosition.x, -1453);
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

        // ドラッグを終了したとき、スクロールを停止
        // スナップさせるページが決まった後も慣性が効いてしまうので.
        StopMovement();

        // スナップさせるページを決定
        // スナップさせるページのインデックスを決定
        int pageIndex = Mathf.RoundToInt(content.anchoredPosition.y / pageHeight);
        // ページが変わっていない且つ、素早くドラッグした場合
        // ドラッグ量の具合は適宜調整
        if (firstDrag) {
            firstDrag = false;
            prevPageIndex = pageIndex;
        }
        // Debug.Log("pageIndex: " + pageIndex + "," + prevPageIndex);
        if (pageIndex == prevPageIndex && Mathf.Abs(eventData.delta.y) >= 5)
        {
            pageIndex += (int)Mathf.Sign(eventData.delta.y);
        }

        // Contentをスクロール位置を決定
        // 必ずページにスナップさせるような位置になるところがポイント
        float destY = pageIndex * pageHeight;
        // content.DOAnchorPosY(destY,0.3f);

        var sequence = DOTween.Sequence()
            .OnStart(() => {})
            .Append(content.DOAnchorPosY(destY,0.3f))
            .OnComplete(() => {
                // 閉じたときに親のGameObjectを非アクティブにする
                // 色選択画面のタッチ判定を消すため
                if (pageIndex == -2 && parentList) {
                    if (closeToDestroy) {
                        Destroy(parentList);
                    } else {
                        parentList.SetActive(false);
                    }
                }
            });
        sequence.Play();

        // 「ページが変わっていない」の判定を行うため、前回スナップされていたページを記憶しておく
        prevPageIndex = pageIndex;
    }
}