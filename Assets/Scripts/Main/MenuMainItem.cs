using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SuperScrollView;

public class MenuMainItem : LoopListViewItem2
{
    public Text titleText;
    public RawImage iconImage;
    public Button button;

    private RectTransform textRectTransform = null;
    private Vector2 textOffsetMin1;
    private Vector2 textOffsetMin2;

    public void UpdateContext(MenuMain selector, int n)
    {
        // 初期化
        if (textRectTransform == null) {
            textRectTransform = titleText.rectTransform;
            textOffsetMin1 = textRectTransform.offsetMin;
            textOffsetMin2 = textOffsetMin1;
            textOffsetMin2.x += iconImage.rectTransform.sizeDelta.x;
            textOffsetMin2.x += iconImage.rectTransform.offsetMin.x;
        }

        var data = DataTable.Menu.list[n];
        titleText.text = data.name;
        iconImage.texture = data.icon;
        iconImage.enabled = data.icon != null;

        textRectTransform.offsetMin = data.icon == null ? textOffsetMin1 : textOffsetMin2;

        // ボタンタップ時の挙動
        button.OnClickAsObservable()
            .Subscribe(b => {
                Debug.Log(titleText.text + ": " + b);
            })
            .AddTo(gameObject);
    }
}
