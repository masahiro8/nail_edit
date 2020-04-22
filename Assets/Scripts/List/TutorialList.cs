using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using SuperScrollView;

public class TutorialList : MonoBehaviour
{
    public ScrollRect scrollRect;
    public ScrollRect dotScrollRect;

    // Start is called before the first frame update
    void Start()
    {
        // すでに表示済みの場合は消しておく
        if (SaveName.TutorialDone.GetBool()) {
            gameObject.SetActive(false);
        }

        // ドットの表示をスクロールと同期させる
        var dotCommonList = dotScrollRect.GetComponent<CommonList>();
        var listView = scrollRect.GetComponent<LoopListView2>();
        scrollRect.OnValueChangedAsObservable()
            .Subscribe(value => {
                var n = (int)(value.x * (float)DataTable.Tutorial.list.Length);
                // Debug.Log(value.x + ", " + scrollRect.normalizedPosition.x + ", " + n + ", " + listView.CurSnapNearestItemIndex);
                dotCommonList.itemIndex.Value = Mathf.Clamp(n, 0, DataTable.Tutorial.list.Length - 1);
                // commonList.itemIndex.Value = listView.CurSnapNearestItemIndex;
            })
            .AddTo(gameObject);

        // ドットの枠の大きさを個数に合わせてフィットさせる
        dotScrollRect.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            DataTable.Tutorial.list.Length * dotScrollRect.content.GetChild(0).GetComponent<RectTransform>().rect.width);

        // ボタンが押されたときにチュートリアルを非表示
        var commonList = dotScrollRect.GetComponent<CommonList>();
        commonList.itemIndex
            .Where(value => value == DataTable.Tutorial.list.Length - 1)
            .Subscribe(value => {
                gameObject.SetActive(false);
                SaveName.TutorialDone.SetBool(true);
            })
            .AddTo(gameObject);
    }
}
