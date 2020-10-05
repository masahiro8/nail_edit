using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DrawDebug : MonoBehaviour
{
    public Text outputTextView = null;
    public Text shapeTextView = null;
    public Button tfliteChangeButton = null;
    public RawImage[] texView1 = null;
    public RawImage[] texView2 = null;
    public RawImage[] texView3 = null;

    private int antiAliasingOrg = 2;

    void Start()
    {
// #if !UNITY_EDITOR
        // デバッグテキスト
        SROptions.Current.DispTextView
            .Subscribe(flag => outputTextView.gameObject.SetActive(flag));
        // 上の画像4つ
        SROptions.Current.DispDebugImage1
            .Subscribe(flag => {
                foreach (var view in texView1) {
                    view.gameObject.SetActive(flag);
                }
            })
            .AddTo(gameObject);
        // 下の合成画像2つ
        SROptions.Current.DispDebugImage2
            .Subscribe(flag => {
                foreach (var view in texView2) {
                    view.gameObject.SetActive(flag);
                }
            })
            .AddTo(gameObject);
        // 爪の画像5つ
        SROptions.Current.DispDebugImage3
            .Subscribe(flag => {
                foreach (var view in texView3) {
                    view.gameObject.SetActive(flag);
                }
            })
            .AddTo(gameObject);
        // 使用するTFLiteを変更
        SROptions.Current.DispTfliteChangeButton
            .Subscribe(flag => {
                if (tfliteChangeButton) {
                    tfliteChangeButton.gameObject.SetActive(flag);
                }
            })
            .AddTo(gameObject);
        // アンチエイリアス
        antiAliasingOrg = QualitySettings.antiAliasing;
        SROptions.Current.QualityAntiAlias
            .Subscribe(n => {
                switch (n) {
                    case 1:
                        QualitySettings.antiAliasing = 2;
                        break;
                    case 2:
                        QualitySettings.antiAliasing = 4;
                        break;
                    case 3:
                        QualitySettings.antiAliasing = 8;
                        break;
                    case 0:
                    default:
                        QualitySettings.antiAliasing = antiAliasingOrg;
                        break;
                }
            })
            .AddTo(gameObject);
// #endif

        // SROptions.Current.DispDebugImage1.Value = true;
        // SROptions.Current.DispDebugImage2.Value = true;
        // SROptions.Current.DispDebugImage3.Value = true;
        // SROptions.Current.CreateDebugTexture = true;

        // SROptions.Current.CameraDegreeOffset = 90;
        // ライトはLightMainで

        // SROptions.Current.DispNailMeshCorner = true;
        // SROptions.Current.DispDebugImage1.Value = true;
        // SROptions.Current.DispDebugImage2.Value = true;
        SROptions.Current.NailCheckMode = true;

        // ネイル
        SROptions.Current.NailEdgeTransparent = true;
        SROptions.Current.NailEdge1TransparentPer = 100; // 100で割って使用
        SROptions.Current.NailEdge2TransparentPer = 120; // 100で割って使用
        SROptions.Current.NailMeshRoundX = 100;
        SROptions.Current.NailMeshRoundY = 25;
        SROptions.Current.NailRotateAdjust = true;

        SROptions.Current.NailCombineDetect = true;
        SROptions.Current.IgnoreObjectDetection = true;
        SROptions.Current.UseSquareObjectDetection = true;
        // SROptions.Current.NoUseGooDeco = true;
        // SROptions.Current.UseAllGooDeco = true;
        // SROptions.Current.IgnoreShapeDetection = true;
        // SROptions.Current.ColorSelectFilterAndRefresh = true;
        // SROptions.Current.DispShapeTextView = true;

        // 静止画モード
        SROptions.Current.ChangeFixedMode.Value = true;
        SROptions.Current.PreviewBrightnessLimit1 = -20;
        SROptions.Current.PreviewBrightnessLimit2 = 30;

        // デバッグボタン
        // SROptions.Current.NailRotateAdjustButton.Value = true;

        outputTextView.text = "";
        shapeTextView.text = "";
    }
}
