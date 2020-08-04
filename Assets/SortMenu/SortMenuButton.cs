using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SortMenuButton : MonoBehaviour
{

    public string TitleLabel;
    public NailFilterType id;
    public Text Label;
    public SortStore Store;

    // Start is called before the first frame update
    void Start()
    {
        Label.text = TitleLabel;
        GetComponent<Button>().OnClickAsObservable()
            .Subscribe( _=>{
                //uniRx sortStoreのid を変更
                Store.id.SetValueAndForceNotify(id);
                Store.label.Value = TitleLabel; // こちらは同値の時は更新不要
                DataTable.Param.filterType.Value = id;
            })
            .AddTo(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
