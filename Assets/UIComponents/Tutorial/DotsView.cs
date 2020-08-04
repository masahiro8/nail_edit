using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class DotsView : MonoBehaviour
{
    public ReactiveProperty<int> itemIndex = new ReactiveProperty<int>(-1);
    public ReactiveProperty<int> itemCount = new ReactiveProperty<int>(0);
    public GameObject dot;
    public GameObject Content;

    private List<GameObject> items = new List<GameObject>();

    //ページの状態を配信する >> 本当はTutorialListでやった方がいいかも
    private Subject<int> indexSubject = new Subject<int>();
    public IObservable<int> OnIndexChanged
    {
        get { return indexSubject; }
    } 

    // Start is called before the first frame update
    void Start()
    {
        //作成
        this.generateDots(DataTable.Tutorial.list.Length);

        itemCount
            .Subscribe( val => {} )
            .AddTo(gameObject);

        //インデックス変更
        itemIndex
            .Subscribe( val => {
                if( val >= 0){
                    updateIndex(val);
                    indexSubject.OnNext(val);
                }
            })
            .AddTo(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void generateDots ( int len ) {
        for ( int i = 0; i < len; i++) {
            GameObject instance = (GameObject)Instantiate(dot);
            instance.transform.SetParent(Content.transform, false);
            items.Add(instance);
        }
    }

    private void updateIndex (int index) {
        foreach (GameObject _dot in items)
        {
           CommonItem _item = _dot.GetComponent<CommonItem>();
           _item.svgImage[1].enabled = false;
        }
        CommonItem item = items[index].GetComponent<CommonItem>();
        item.svgImage[1].enabled = true;
    }

}
