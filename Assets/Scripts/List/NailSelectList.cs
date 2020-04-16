using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SuperScrollView;

public class NailSelectList : MonoBehaviour
{
    // public float itemWidth = 400;
    // public ScrollRect scrollRect;
    // public GameObject itemPrefab;
    // public GameObject modelPrefab;
    public DrawMain main;

    // Start is called before the first frame update
    void Start()
    {
        UpdateContext();
    }

    // Update is called once per frame
    void UpdateContext()
    {
        // // Debug.Log("scrollRect.viewport.rect: " + scrollRect.viewport.rect);
        // Debug.Log("GetComponent<RectTransform>().sizeDelta: " + scrollRect.GetComponent<RectTransform>().sizeDelta);
        // scrollRect.content.sizeDelta = new Vector2(
        //     (float)list.Length * itemWidth - scrollRect.GetComponent<RectTransform>().sizeDelta.x,
        //     scrollRect.content.rect.height);

        // nailDetection.baseColor = nailTable.list[0].baseColor;

        // // スクロールバーのアイテム
        // for (var i = 0; i < DataTable.Nail.list.Length; i++) {
        //     var obj = Instantiate(itemPrefab, scrollRect.content);
        //     var item = obj.GetComponent<NailItem>();
        //     item.UpdateContext(this, i);
        // }

        // // 爪のモデル
        // for (var i = 0; i < DataTable.Nail.list.Length; i++) {
        //     var obj = Instantiate(modelPrefab, transform);
        //     obj.transform.localRotation = Quaternion.Euler(-30, 180, 60);
        //     obj.transform.localScale = Vector3.one * 4f;

        //     var data = DataTable.Nail.list[i];
        //     var materialData = data.materials[0];
        //     var meshRenderer = obj.GetComponent<MeshRenderer>();
        //     materialData.SetMaterial(meshRenderer);
        // }

        // scrollRect.OnValueChangedAsObservable()
        //     .Subscribe(v => UpdateLayer(v))
        //     .AddTo(gameObject);
    }

    // // スクロールしたのでアイテムを更新
    // public void UpdateLayer(Vector2 p)
    // {
    //     // Debug.Log("Scroll: " + p);
    //     // Debug.Log("Scroll1: " + scrollView.normalizedPosition);
    //     // Vector3 p2 = p;
    //     for (var i = 0; i < transform.childCount; i++) {
    //         var item = scrollRect.content.GetChild(i).GetComponent<CommonItem>();
    //         var rectTransform = item.button[0].GetComponent<RectTransform>();
    //         var p2 = rectTransform.position;
    //         p2.z -= 5;
    //         transform.GetChild(i).localPosition = p2;
    //         // var per = (float)i / (float)(transform.childCount - 1);
    //         // transform.GetChild(i).localPosition = GetPosition(p, per);
    //     }
    // }
 
    // public Vector3 GetPosition(Vector2 p, float per)
    // {
    //     Vector3 res = p;
    //     res.x = (p.x - per) * -10;
    //     res.y = 0;
    //     res.z = 0;
    //     return res;
    // }
}
