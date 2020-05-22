using System.ComponentModel;
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

    [Category("表示")]
    [DisplayName("デバッグテキスト")]
    public bool _DispTextView {
        get { return DispTextView.Value; }
        set { DispTextView.Value = value; }
    }
    public ReactiveProperty<bool> DispTextView = new ReactiveProperty<bool>();

    [Category("表示")]
    [DisplayName("手のモデル")]
    public bool _DispHandModel {
        get { return DispHandModel.Value; }
        set { DispHandModel.Value = value; }
    }
    public ReactiveProperty<bool> DispHandModel = new ReactiveProperty<bool>();

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
}
