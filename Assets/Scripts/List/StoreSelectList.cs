using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class StoreSelectList : MonoBehaviour
{
    public CommonList storeSelectList;
    public Button closeButton;
    public Image backgroundImage; // 画面全体を暗くするための背景
    public RectTransform frameTransform; // メニュー用の黒い背景
    public RectTransform menuTransform; // メニュー内部
    // public Texture[] banners;

    private Color orgColor; // 画面全体を暗くするための背景の元の色
    private int SelectedStore = 0;

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

        storeSelectList.updateItem = UpdateNotice;
        storeSelectList.itemCount.Value = DataTable.Store.showList.Length;

        // ボタン押し
        if (closeButton) {
            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseWindow(true))
                .AddTo(gameObject);
        }

        // アニメーション用に色の保存
        orgColor = backgroundImage.color;
        // CloseMenu();

        // メニュー作成
        CloseWindow(false);
    }

    private void UpdateNotice(CommonItem item, int index)
    {
        var data = DataTable.Store.showList[index];
        // // 値が変化したときに選択済み表示を更新
        // var disposable = data.saveFlag
        //     .Subscribe(flag => item.svgImage[0].enabled = !flag);
        // item.disposableBag.Add(disposable);

        // ボタンが押されたときにフラグの切り替え
        var　disposable = item.button[0].OnClickAsObservable()
            .Subscribe(_ => {
                storeSelectList.itemIndex.Value = index;
                // Debug.Log(index);
            });
        item.disposableBag.Add(disposable);

        disposable = storeSelectList.itemIndex
            .Subscribe(n => {
                item.button[1].gameObject.SetActive(n != index);
                item.button[2].gameObject.SetActive(n == index);
            });
        item.disposableBag.Add(disposable);

        // if( index < banners.Length ) {
        //     Texture _tex = banners[index];
        //     var tex = Resources.Load<Texture2D>("Shop/"+_tex.name);
        //     item.image[0].texture = tex;
        // }
        item.image[0].texture = data.image;

        // ボタンタップ時の挙動
        disposable = item.button[0].OnClickAsObservable()
            .Subscribe(b => {
                // Debug.Log(index.ToString() + ": " + b);
                SaveName.StoreSelect.SetInt(index);
            });
        item.disposableBag.Add(disposable);
    }

    // 開く
    public void OpenWindow()
    {
        CompleteAnimation();

        //保持した値を設定
        storeSelectList.itemIndex.Value = SaveName.StoreSelect.GetInt();

        backgroundImage.raycastTarget = true;
        backgroundImage
            .DOColor(orgColor, DataTable.Param.duration);
        frameTransform
            .DOAnchorPosY(0, DataTable.Param.duration);
        menuTransform
            .DOAnchorPosY(0, DataTable.Param.duration);
    }

    // 閉じる
    public void CloseWindow(bool anim)
    {
        CompleteAnimation();

        var f = anim ? DataTable.Param.duration : 0;
        backgroundImage.raycastTarget = false;
        backgroundImage
            .DOColor(Color.clear, f);
        frameTransform
            .DOAnchorPosY(-Screen.height, f);
        menuTransform
            .DOAnchorPosY(-Screen.height, f);
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

    // // WebViewを開く
    // public void OpenWebView(string url)
    // {
    //     var webView = Instantiate(webViewPrefab).GetComponent<UniWebView>();
    //     webView.Load(url);
    //     webView.Show();
    //     webView.OnShouldClose += (view) => {
    //         Destroy(view.gameObject);
    //         return true;
    //     };
    //     // Debug.Log("UniWebView: " + GameObject.FindObjectsOfType<Transform>().Where(c => c.name == "UniWebView(Clone)").ToArray().Length);
    // }
}
