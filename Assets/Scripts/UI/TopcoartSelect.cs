using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class TopcoartSelect : MonoBehaviour
{

    public Button BtnClear;
    public Button BtnMat;
    // public GameObject DetailView;

    // public string[] ClearNumbers;
    // public string[] MatNumbers;

    // public ReactiveProperty<int> status = new ReactiveProperty<int>(0); // 0: なし 1:clear 2:mat
    // public ReactiveProperty<string[]> Info = new ReactiveProperty<string[]>();

    void Start()
    {
        //ネイル情報を取得

        //状態管理
        var status = DataTable.Param.topcoatType;
        var customRef = RenderSettings.customReflection;
        // RenderSettings.reflectionIntensity = 1.6f; // 個別に指定
        status.Subscribe( v => { 
            switch (v) {
                case NailTopcoatType.None:
                    // Info.Value = null;
                    BtnClear.GetComponent<BtnStatus>().onSelect(false);
                    BtnMat.GetComponent<BtnStatus>().onSelect(false);
                    RenderSettings.customReflection = null;
                    break;
                case NailTopcoatType.Clear:
                    // Info.Value = ClearNumbers;
                    BtnClear.GetComponent<BtnStatus>().onSelect(true);
                    BtnMat.GetComponent<BtnStatus>().onSelect(false);
                    RenderSettings.customReflection = customRef;
                    break;
                case NailTopcoatType.Mat:
                    // Info.Value = MatNumbers;
                    BtnClear.GetComponent<BtnStatus>().onSelect(false);
                    BtnMat.GetComponent<BtnStatus>().onSelect(true);
                    RenderSettings.customReflection = null;
                    break;
            }
        }).AddTo(gameObject);

        BtnClear.OnClickAsObservable()
            .Subscribe(_ => {
                status.Value = (status.Value == NailTopcoatType.Clear) ? 0 : NailTopcoatType.Clear;
            })
            .AddTo(gameObject);

        BtnMat.OnClickAsObservable()
            .Subscribe(_ => {
                status.Value = (status.Value == NailTopcoatType.Mat) ? 0 : NailTopcoatType.Mat;
            })
            .AddTo(gameObject);
    }

}
