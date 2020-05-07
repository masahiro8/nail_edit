using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using SuperScrollView;

public class CommonItem : MonoBehaviour
{
    public Text[] text;
    public RawImage[] image;
    public SVGImage[] svgImage;
    public Button[] button;
    public RectTransform[] extra;
    public RawImage selectedBar;

    [System.NonSerialized] public CompositeDisposable disposableBag = new CompositeDisposable();
    [System.NonSerialized] public RectTransform textRectTransform = null;
    [System.NonSerialized] public Vector2 textOffsetMin1;
    [System.NonSerialized] public Vector2 textOffsetMin2;

    void OnDestroy()
    {
        disposableBag.Clear();
    }

    public void UpdateContext(CommonList selector, int n)
    {
        // それまでの講読を中止
        disposableBag.Clear();

        // ボタンタップ時の挙動
        if (button.Length > 0) {
            var disposable = button[0].OnClickAsObservable()
                .Subscribe(b => {
                    selector.itemIndex.Value = n;
                });
            disposableBag.Add(disposable);
        }

        // 選択状態バーを移動させる
        if (selectedBar) {
            selectedBar.enabled = n == selector.itemIndex.Value;
            var disposable = selector.itemIndex
                .Zip(selector.itemIndex.Skip(1), (x, y) => new System.Tuple<int, int>(x, y))
                .Subscribe(value => {
                    selectedBar.enabled = n == value.Item2;
                    if (n == value.Item2) {
                        transform.SetAsLastSibling(); // 最前面に移動
                        selectedBar.rectTransform.DOAnchorPosX((value.Item1 - value.Item2) * selectedBar.rectTransform.rect.width, 0);
                        selectedBar.rectTransform.DOAnchorPosX(0, DataTable.Param.selectedBarDuration);
                    }
                });
            disposableBag.Add(disposable);
        }
    }
}
