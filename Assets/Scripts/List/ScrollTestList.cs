using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using DG.Tweening;
using SuperScrollView;
using DanielLochner.Assets.SimpleScrollSnap;

public class ScrollTestList : MonoBehaviour
{
    public CommonList frameList;
    public Button closeButton;
    int index = 0;
    int max = 4;

    // Start is called before the first frame update
    void Start()
    {
        // 編集用に横にずらしているのを元に戻す
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        frameList.updateItem = UpdateFrame;
        frameList.itemCount.Value = max;

        // ボタン押し
        if (closeButton) {
            closeButton.OnClickAsObservable()
                .Subscribe(_ => {
                    var listView2 = frameList.GetComponent<LoopListView2>();
                    if (listView2) {
                        // リスト表示をスクロールさせる
                        index = (index + 1) % frameList.itemCount.Value;
                        // listView2.MovePanelToItemIndex(index, 0);
                        listView2.SetSnapTargetItemIndex(index);
                    }
                    // SceneManager.LoadScene("Main");
                })
                .AddTo(gameObject);
        }

        var tmp = DataTable.Instance;
    }

    // フレームが変わった
    private void UpdateFrame(CommonItem item, int index)
    {
        item.text[0].text = index.ToString();
        // commonList.itemCount.Value = DataTable.MyList.filterdList[index].Value.Length;
    }
}
