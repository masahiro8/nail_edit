using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class Progress : MonoBehaviour
{
    public GameObject Bar;

    // Start is called before the first frame update
    void Start()
    {
        Bar.GetComponent<Image>().transform.DOScaleX(0.0f,0.0f);
        // タイマー
        Observable
            .Timer(System.TimeSpan.FromMilliseconds(100))
            .Subscribe(_ => {
                Bar.GetComponent<Image>().transform.DOScaleX(0.5f,0.5f);
            })
            .AddTo(gameObject);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setProgress ( float val ) {
         Bar.GetComponent<Image>().transform.DOScaleX( val, 0.5f);
    }
}
