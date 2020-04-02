using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class NailItem : MonoBehaviour
{
    public Text numberText;
    public Text titleText;
    public Button button;

    public void UpdateContext(NailSelector selector, int n)
    {
        // // 座標のセット
        // var rectTransform = GetComponent<RectTransform>();
        // rectTransform.sizeDelta = new Vector2(
        //     selector.itemWidth,
        //     selector.scrollRect.content.sizeDelta.y);
        // rectTransform.anchoredPosition = new Vector2(
        //     (float)n * selector.itemWidth,
        //     0);

        var data = DataTable.Nail.list[n];
        numberText.text = "No." + (n + 1).ToString();
        titleText.text = data.name;

        // var material = Resources.Load<Material>("Materials/" + data.materialName);

        // ボタンタップ時の挙動
        button.OnClickAsObservable()
            .Subscribe(b => {
                foreach (Transform t in selector.main.nailDetection.transform) {
                    t.GetComponent<NailGroup>().UpdateData(DataTable.Nail.list[n]);
                }
                // Debug.Log("Click: " + data.name);
            })
            .AddTo(gameObject);
    }
}
