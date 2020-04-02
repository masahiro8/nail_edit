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
    [DisplayName("オリジナル")]
    public bool _DispOrgImage {
        get { return DispOrgImage.Value; }
        set { DispOrgImage.Value = value; }
    }
    public ReactiveProperty<bool> DispOrgImage = new ReactiveProperty<bool>();

    [Category("画像")]
    [DisplayName("手のひらの検出")]
    public bool _DispPalmImage {
        get { return DispPalmImage.Value; }
        set { DispPalmImage.Value = value; }
    }
    public ReactiveProperty<bool> DispPalmImage = new ReactiveProperty<bool>();

    [Category("画像")]
    [DisplayName("手の検出")]
    public bool _DispHandImage {
        get { return DispHandImage.Value; }
        set { DispHandImage.Value = value; }
    }
    public ReactiveProperty<bool> DispHandImage = new ReactiveProperty<bool>();
}
