using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class AdjustBrightnessView : MainBaseView
{
    public Slider slider;
    public Text descriptionText; // 画面上部の説明文
    public Text decideText; // 決定ボタンのテキスト
    public Button backButton; // 戻るボタン
    public Button decideButton; // 決定ボタン

    public string descriptionString; // 画面上部の説明文用文言
    public string decideString; // 決定ボタンのテキスト用文言

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // 文言のローカライズ
        descriptionText.text = descriptionString.Localized();
        decideText.text = decideString.Localized();

        // スライダーが動いた
        slider.OnValueChangedAsObservable()
            .Subscribe(v => {
                // Debug.Log("Slider: " + Mathf.Round(v * 100));
                // -1〜1を変換する
                if (v < 0) {
                    v *= (float)SROptions.Current.PreviewBrightnessLimit1 / -100f;
                } else {
                    v *= (float)SROptions.Current.PreviewBrightnessLimit2 / 100f;
                }
                viewManager.drawMain.delayView.material.SetFloat("Brightness", v);
            })
            .AddTo(gameObject);

        // 戻る
        if (backButton) {
            backButton.OnClickAsObservable()
                .Where(_ => !viewManager.drawMain.IsProcessing)
                .Subscribe(_ => {
                    DataTable.Param.mainView.Value = MainViewType.FixedCamera;
                })
                .AddTo(gameObject);
        }

        // 決定
        if (decideButton) {
            decideButton.OnClickAsObservable()
                .Subscribe(_ => {
                    DataTable.Param.mainView.Value = MainViewType.FixedEdit;
                })
                .AddTo(gameObject);
        }
    }
}
