using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class MainMenuList : MonoBehaviour
{
    public Button openButton;
    public Button closeButton;
    public Image backgroundImage; // 画面全体を暗くするための背景
    public RectTransform frameTransform; // メニュー用の黒い背景
    public RectTransform menuTransform; // メニュー内部
    public MyListList favoriteList; // メニュー内部
    public GameObject webViewPrefab;

    private Color orgColor; // 画面全体を暗くするための背景の元の色

    void OnDestroy()
    {
        backgroundImage.DOKill();
        frameTransform.DOKill();
        menuTransform.DOKill();
    }

    // Start is called before the first frame update
    void Start()
    {
        // ボタン押し
        if (openButton) {
            openButton.OnClickAsObservable()
                .Subscribe(_ => OpenMenu())
                .AddTo(gameObject);
        }

        if (closeButton) {
            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseMenu())
                .AddTo(gameObject);
        }

        // アニメーション用に色の保存
        orgColor = backgroundImage.color;
        // CloseMenu();

        // メニュー作成
        CloseMenu();
    }

    // 開く
    private void OpenMenu()
    {
        CompleteAnimation();

        backgroundImage.raycastTarget = true;
        backgroundImage
            .DOColor(orgColor, DataTable.Param.duration);
        frameTransform
            .DOAnchorPosX(0, DataTable.Param.duration);
        menuTransform
            .DOAnchorPosX(0, DataTable.Param.duration);
    }

    // 閉じる
    private void CloseMenu()
    {
        CompleteAnimation();

        backgroundImage.raycastTarget = false;
        backgroundImage
            .DOColor(Color.clear, DataTable.Param.duration);
        frameTransform
            .DOAnchorPosX(-Screen.width, DataTable.Param.duration);
        menuTransform
            .DOAnchorPosX(-Screen.width, DataTable.Param.duration);
    }

    // 閉じる
    private void CompleteAnimation()
    {
        if (DOTween.IsTweening(backgroundImage)) {
            backgroundImage.DOComplete();
        }
        if (DOTween.IsTweening(frameTransform)) {
            frameTransform.DOComplete();
        }
        if (DOTween.IsTweening(menuTransform)) {
            menuTransform.DOComplete();
        }
    }

    // WebViewを開く
    public void OpenWebView(string url)
    {
        var webView = Instantiate(webViewPrefab).GetComponent<UniWebView>();
        webView.Load(url);
        webView.Show();
        webView.OnShouldClose += (view) => {
            Destroy(view.gameObject);
            return true;
        };
        // Debug.Log("UniWebView: " + GameObject.FindObjectsOfType<Transform>().Where(c => c.name == "UniWebView(Clone)").ToArray().Length);
    }
}
