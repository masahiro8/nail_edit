using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class LightMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 初期値
        SROptions.Current.LightRotateX.Value = (int)Mathf.Round(transform.eulerAngles.x);
        SROptions.Current.LightRotateY.Value = (int)Mathf.Round(transform.eulerAngles.y);
        SROptions.Current.LightRotateZ.Value = (int)Mathf.Round(transform.eulerAngles.z);

        SROptions.Current.LightRotateX
            .SkipLatestValueOnSubscribe()
            .Subscribe(_ => UpdateRotate())
            .AddTo(gameObject);
        SROptions.Current.LightRotateY
            .SkipLatestValueOnSubscribe()
            .Subscribe(_ => UpdateRotate())
            .AddTo(gameObject);
        SROptions.Current.LightRotateZ
            .SkipLatestValueOnSubscribe()
            .Subscribe(_ => UpdateRotate())
            .AddTo(gameObject);
    }

    // ライトの角度を更新
    void UpdateRotate()
    {
        transform.localRotation = Quaternion.Euler(
            (float)SROptions.Current.LightRotateX.Value,
            (float)SROptions.Current.LightRotateY.Value,
            (float)SROptions.Current.LightRotateZ.Value
        );
    }
}
