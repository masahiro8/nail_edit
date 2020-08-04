using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SortMenu : MonoBehaviour
{
    public Button MainButton;
    public GameObject Menu;
    public SortStore Store;
    public ScrollRect scrollRect;

    public NailFilterType initialId = NailFilterType.All;
    // public string initialLabel = "FAVORITE";

    // Start is called before the first frame update
    void Start()
    {
        Store.id.Value = (NailFilterType)SaveName.NailItemFilter.GetInt((int)initialId);
        var res = scrollRect.content.Cast<Transform>() // スクロールビューの子供を全取得
            .Select(v => v.GetComponent<SortMenuButton>()) // 型の変換
            .Where(v => v.id == Store.id.Value) // IDでフィルター
            .ToArray(); // 配列として取得
        Store.label.Value = res.Length > 0 ? res[0].TitleLabel : "";
        showSortMenu(false);

        //ストアからソートのidの変更を受け取り
        Store.OnIdChanged.Subscribe(id =>
        {
            showSortMenu(false);
        });

        MainButton.OnClickAsObservable()
            .Subscribe( _=>{
                showSortMenu(true);
            })
            .AddTo(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void showSortMenu(bool b){

        if( b ) {
            Menu.SetActive(true);
            MainButton.gameObject.SetActive(false);
        } else {
            Menu.SetActive(false);
            MainButton.gameObject.SetActive(true);
        }
    }
}
