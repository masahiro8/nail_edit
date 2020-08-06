using System.ComponentModel;
using UnityEngine;
using UniRx;

public partial class SROptions {

    [Category("カメラ")]
    [DisplayName("表示サイズへの倍率")]
    [NumberRange(0, 1)]
    [Increment(0.1f)]
    public float CameraDispSizeScale {
        get;
        set;
    }

    [Category("カメラ")]
    [DisplayName("角度へのオフセット")]
    [NumberRange(0, 270)]
    [Increment(90)]
    public float CameraDegreeOffset {
        get;
        set;
    }

    [Category("カメラ")]
    [DisplayName("NatDeviceを使う")]
    public bool UseCameraNatDevice {
        get;
        set;
    }

    [Category("ライト")]
    [DisplayName("並行光源の角度X")]
    [NumberRange(-180, 180)]
    [Increment(5)]
    public int _LightRotateX {
        get { return LightRotateX.Value; }
        set { LightRotateX.Value = value; }
    }
    public ReactiveProperty<int> LightRotateX = new ReactiveProperty<int>();

    [Category("ライト")]
    [DisplayName("並行光源の角度Y")]
    [NumberRange(-180, 180)]
    [Increment(5)]
    public int _LightRotateY {
        get { return LightRotateY.Value; }
        set { LightRotateY.Value = value; }
    }
    public ReactiveProperty<int> LightRotateY = new ReactiveProperty<int>();

    [Category("ライト")]
    [DisplayName("並行光源の角度Z")]
    [NumberRange(-180, 180)]
    [Increment(5)]
    public int _LightRotateZ {
        get { return LightRotateZ.Value; }
        set { LightRotateZ.Value = value; }
    }
    public ReactiveProperty<int> LightRotateZ = new ReactiveProperty<int>();

    [Category("表示")]
    [DisplayName("デバッグテキスト")]
    public bool _DispTextView {
        get { return DispTextView.Value; }
        set { DispTextView.Value = value; }
    }
    public ReactiveProperty<bool> DispTextView = new ReactiveProperty<bool>();

    [Category("表示")]
    [DisplayName("グーパーの結果を表示")]
    public bool DispShapeTextView {
        get;
        set;
    }

    [Category("表示")]
    [DisplayName("ネイルメッシュの4角確認")]
    public bool DispNailMeshCorner {
        get;
        set;
    }

    [Category("ネイル")]
    [DisplayName("エッジ透過有効化")]
    public bool NailEdgeTransparent {
        get;
        set;
    }

    [Category("ネイル")]
    [DisplayName("エッジ透過境界1")]
    [NumberRange(0, 200)]
    [Increment(5)]
    public int NailEdge1TransparentPer {
        get;
        set;
    }

    [Category("ネイル")]
    [DisplayName("エッジ透過境界2")]
    [NumberRange(0, 200)]
    [Increment(5)]
    public int NailEdge2TransparentPer {
        get;
        set;
    }

    [Category("ネイル")]
    [DisplayName("メッシュの曲がりX")]
    [NumberRange(5, 100)]
    [Increment(5)]
    public int NailMeshRoundX {
        get;
        set;
    }

    [Category("ネイル")]
    [DisplayName("メッシュの曲がりY")]
    [NumberRange(5, 100)]
    [Increment(5)]
    public int NailMeshRoundY {
        get;
        set;
    }

    [Category("ネイル")]
    [DisplayName("方向補正有効化")]
    public bool NailRotateAdjust {
        get;
        set;
    }

    [Category("色選択画面")]
    [DisplayName("色選択時に表示ネイルを更新")]
    public bool ColorSelectFilterAndRefresh {
        get;
        set;
    }

    [Category("画像")]
    [DisplayName("デバッグ表示1")]
    public bool _DispDebugImage1 {
        get { return DispDebugImage1.Value; }
        set { DispDebugImage1.Value = value; }
    }
    public ReactiveProperty<bool> DispDebugImage1 = new ReactiveProperty<bool>();

    [Category("画像")]
    [DisplayName("デバッグ表示2")]
    public bool _DispDebugImage2 {
        get { return DispDebugImage2.Value; }
        set { DispDebugImage2.Value = value; }
    }
    public ReactiveProperty<bool> DispDebugImage2 = new ReactiveProperty<bool>();

    [Category("画像")]
    [DisplayName("デバッグ表示3")]
    public bool _DispDebugImage3 {
        get { return DispDebugImage3.Value; }
        set { DispDebugImage3.Value = value; }
    }
    public ReactiveProperty<bool> DispDebugImage3 = new ReactiveProperty<bool>();

    [Category("画像")]
    [DisplayName("デバッグテクスチャ生成")]
    public bool CreateDebugTexture {
        get;
        set;
    }

    [Category("クオリティ")]
    [DisplayName("アンチエイリアス")]
    [NumberRange(0, 3)]
    public int _QualityAntiAlias {
        get { return QualityAntiAlias.Value; }
        set { QualityAntiAlias.Value = value; }
    }
    public ReactiveProperty<int> QualityAntiAlias = new ReactiveProperty<int>();

    [Category("チェックモード")]
    [DisplayName("ネイルチェックモード")]
    public bool NailCheckMode {
        get;
        set;
    }

    [Category("チェックモード")]
    [DisplayName("ネイルを合成して検出")]
    public bool NailCombineDetect {
        get;
        set;
    }

    [Category("チェックモード")]
    [DisplayName("物体検出を使わない")]
    public bool IgnoreObjectDetection {
        get;
        set;
    }

    [Category("チェックモード")]
    [DisplayName("カメラ画像を正方形で抜き出す")]
    public bool UseSquareObjectDetection {
        get;
        set;
    }

    [Category("チェックモード")]
    [DisplayName("グーパー検出を使わない")]
    public bool IgnoreShapeDetection {
        get;
        set;
    }

    [Category("チェックモード")]
    [DisplayName("グーは強制的にデコなしにする")]
    public bool NoUseGooDeco {
        get;
        set;
    }

    [Category("チェックモード")]
    [DisplayName("すべてグーのデコなしにする")]
    public bool UseAllGooDeco {
        get;
        set;
    }

    [Category("チェックモード")]
    [DisplayName("tflite変更ボタンの表示")]
    public bool _DispTfliteChangeButton {
        get { return DispTfliteChangeButton.Value; }
        set { DispTfliteChangeButton.Value = value; }
    }
    public ReactiveProperty<bool> DispTfliteChangeButton = new ReactiveProperty<bool>();

    [Category("アプリモード")]
    [DisplayName("静止画モード切り替え")]
    public bool _ChangeFixedMode {
        get { return ChangeFixedMode.Value; }
        set { ChangeFixedMode.Value = value; }
    }
    public ReactiveProperty<bool> ChangeFixedMode = new ReactiveProperty<bool>();

    // [Category("アプリモード")]
    // [DisplayName("エディタモード切り替え")]
    // public bool ChangeEditorMode {
    //     get;
    //     set;
    // }

    [Category("データ操作")]
    [DisplayName("強制的にアプリをクラッシュ")]
    public void ForceCrash()
    {
        // Firebase.Crashlytics.Crashlytics.Crassh();
        UnityEngine.Diagnostics.Utils.ForceCrash(0);
    }

    [Category("データ操作")]
    [DisplayName("チュートリアルフラグを削除")]
    public void RemoveTutorialFlag()
    {
        SaveName.TutorialDone.SetBool(false);
    }

    [Category("データ操作")]
    [DisplayName("ナビゲーションフラグを削除")]
    public void RemoveNavigationFlag()
    {
        SaveName.NavigationDone.SetBool(false);
    }

    [Category("データ操作")]
    [DisplayName("ナビゲーションガイドフラグを削除")]
    public void RemoveNavigationGuideFlag()
    {
        SaveName.NavigationGuideDone.SetBool(false);
    }

    [Category("データ操作")]
    [DisplayName("フリーモード警告フラグを削除")]
    public void RemoveFreeModeAlertFlag()
    {
        SaveName.FreeModeAlertDone.SetBool(false);
    }

    [Category("プレビュー画面")]
    [DisplayName("明度の最小値")]
    [NumberRange(-200, 0)]
    [Increment(5)]
    public int PreviewBrightnessLimit1 {
        get;
        set;
    }

    [Category("プレビュー画面")]
    [DisplayName("明度の最大値")]
    [NumberRange(0, 200)]
    [Increment(5)]
    public int PreviewBrightnessLimit2 {
        get;
        set;
    }

    [Category("デバッグボタン")]
    [DisplayName("ネイル回転補正ボタン")]
    public bool _NailRotateAdjustButton {
        get { return NailRotateAdjustButton.Value; }
        set { NailRotateAdjustButton.Value = value; }
    }
    public ReactiveProperty<bool> NailRotateAdjustButton = new ReactiveProperty<bool>();
}
