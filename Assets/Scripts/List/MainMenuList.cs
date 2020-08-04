using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class MainMenuList : MonoBehaviour
{
    public CommonList menuList;
    public Button openButton;
    public Button closeButton;
    public Image backgroundImage; // 画面全体を暗くするための背景
    public RectTransform frameTransform; // メニュー用の黒い背景
    public RectTransform menuTransform; // メニュー内部
    public MyListList favoriteList; // メニュー内部
    public NoticeList noticeList; // お知らせ
    public StoreSelectList storeSelectList; // ストア選択
    public GameObject webViewPrefab;
    public RectTransform swipeContentTransform;//スワイプビュー

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
        // 編集用に横にずらしているのを元に戻す
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        menuList.updateItem = UpdateMenu;
        menuList.itemCount.Value = DataTable.Menu.list.Length;

        // ボタン押し
        if (openButton) {
            openButton.OnClickAsObservable()
                .Subscribe(_ => OpenMenu())
                .AddTo(gameObject);
        }

        if (closeButton) {
            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseMenu(true))
                .AddTo(gameObject);
        }

        // アニメーション用に色の保存
        orgColor = backgroundImage.color;
        // CloseMenu();

        // メニュー作成
        CloseMenu(false);
    }

    private void UpdateMenu(CommonItem item, int index)
    {
        // 初期化
        if (item.textRectTransform == null) {
            item.textRectTransform = item.text[0].rectTransform;
            item.textOffsetMin1 = item.textRectTransform.offsetMin;
            item.textOffsetMin2 = item.textOffsetMin1;
            item.textOffsetMin2.x += item.svgImage[0].rectTransform.sizeDelta.x;
            item.textOffsetMin2.x += item.svgImage[0].rectTransform.offsetMin.x;
        }

        var data = DataTable.Menu.list[index];
        item.text[0].text = data.name.Localized();
        item.svgImage[0].sprite = data.icon;
        item.svgImage[0].enabled = data.icon != null;

        //item.textRectTransform.offsetMin = data.icon == null ? item.textOffsetMin1 : item.textOffsetMin2;

        // ボタンタップ時の挙動
        var disposable = item.button[0].OnClickAsObservable()
            .Subscribe(b => {
                Debug.Log(item.text[0].text + ": " + b);
                switch (index) {
                    case 0:
                        favoriteList.OpenMenu(0);
                        break;
                    case 1: // ショップ選択
                        storeSelectList.OpenWindow();
                        break;
                    case 2: // お知らせ
                        noticeList.OpenWindow();
                        break;
                    case 5:
                    case 6:
                        OpenWebView(data.url);
                        break;
                    case 7:
                        SRDebug.Instance.ShowDebugPanel();
                        break;
                    default:
                        break;
                }
            });
        item.disposableBag.Add(disposable);
    }

    // 開く
    public void OpenMenu()
    {
        CompleteAnimation();

        // backgroundImage.raycastTarget = true;
        // backgroundImage
        //     .DOColor(orgColor, DataTable.Param.duration);
        // frameTransform
        //     .DOAnchorPosX(0, DataTable.Param.duration);
        // menuTransform
        //     .DOAnchorPosX(0, DataTable.Param.duration);
        swipeContentTransform.DOAnchorPosX(0, 0.3f);
    }

    // 閉じる
    private void CloseMenu(bool anim)
    {
        CompleteAnimation();

        // backgroundImage.raycastTarget = false;
        // backgroundImage
        //     .DOColor(Color.clear, DataTable.Param.duration);
        // frameTransform
        //     .DOAnchorPosX(-Screen.width, DataTable.Param.duration);
        // menuTransform
        //     .DOAnchorPosX(-Screen.width, DataTable.Param.duration);
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
