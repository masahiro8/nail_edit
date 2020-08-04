using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class SortStore : MonoBehaviour
{
    public ReactiveProperty<NailFilterType> id = new ReactiveProperty<NailFilterType>(NailFilterType.All);
    public ReactiveProperty<string> label = new ReactiveProperty<string>();

    private Subject<NailFilterType> idSubject = new Subject<NailFilterType>();
    public IObservable<NailFilterType> OnIdChanged
    {
        get { return idSubject; }
    }

    private Subject<string> labelSubject = new Subject<string>();
    public IObservable<string> OnLabelChanged
    {
        get { return labelSubject; }
    } 

    // Start is called before the first frame update
    void Start()
    {
        //id変更を検知して配信
        id.Subscribe( val => {
            idSubject.OnNext(val);
        })
        .AddTo(gameObject);

        //label変更を検知して配信
        label.Subscribe( val => {
            Debug.Log("label " + val);
            labelSubject.OnNext(val);
        })
        .AddTo(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setId(NailFilterType n){
        id.Value = n;
    }
}
