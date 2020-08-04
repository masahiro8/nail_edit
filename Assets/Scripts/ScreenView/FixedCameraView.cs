using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class FixedCameraView : MainBaseView
{
    public Button modeButton;
    public Text descriptionText; // 画面上部の説明文
    public Text iconText; // アイコンのテキスト

    public GameObject frameBaseObj; // モードごとのビューの親
    public GameObject[] frameObj; // モードごとのビュー
    public string descriptionString; // 画面上部の説明文用文言
    public string iconString; // アイコンのテキスト用文言

    public GameObject modeViewPrefab; // 撮影モードPrefab
    public GameObject brightnessPrefab; // 明るさPrefab

    public GameObject navigationView;

    //監視
    public string selectType = "Left";
    public string selectMode = "HandPaa";

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        UnityEngine.Assertions.Assert.IsTrue(frameObj.Length == System.Enum.GetValues(typeof(ShootModeType)).Length);

        // ナビゲーションガイドを出す
        if (!SaveName.NavigationGuideDone.GetBool()) {
            Instantiate(navigationView, transform.parent.GetComponent<RectTransform>());
        }

        // シャッターボタンを押した
        shutterButton.OnClickAsObservable()
            .Subscribe(_ => {
                DataTable.Param.mainView.Value = DataTable.Param.useDummyImage
                    ? MainViewType.FixedEdit
                    : MainViewType.FixedPreview;
            })
            .AddTo(gameObject);

        // モード選択ボタンを押した
        if (modeButton) {
            modeButton.OnClickAsObservable()
                .SubscribeOnMainThread()
                .Subscribe(_ => {
                    Instantiate(modeViewPrefab, transform.parent.GetComponent<RectTransform>());
                })
                .AddTo(gameObject);
        }

        // 手の左右が変わった
        DataTable.Param.shootHand
            .Subscribe(hand => {
                selectType = hand.ToString();
                frameBaseObj.transform.localScale = new Vector3(
                    hand == ShootHandType.Right ? -1f : 1f,
                    1f,
                    1f);
                UpdateModeIconRawImage();
            })
            .AddTo(gameObject);

        // 撮影モードが変わった
        DataTable.Param.shootMode
            .Subscribe(mode => {
                selectMode = mode.ToString();
                var n = (int)mode;
                for (var i = 0; i < frameObj.Length; i++) {
                    frameObj[i].SetActive(i == n);
                }
                Debug.Log(descriptionString + mode);
                descriptionText.text = (descriptionString + mode).Localized();
                // iconText.text = (iconString + mode).Localized();//画像内に文字を表示
                UpdateModeIconRawImage();
            })
            .AddTo(gameObject);
    }

    public void UpdateModeIconRawImage(){
        string tex = "";
        if( selectMode == ShootModeType.Free.ToString()) {
            tex = string.Format("Textures/ModeButton/mode-free");
        }else {
            tex = string.Format("Textures/ModeButton/mode-{0}-{1}", selectType, selectMode);
        }
        modeButton.GetComponent<RawImage>().texture = Resources.Load<Texture2D>(tex);
    }
}
