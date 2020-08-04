using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;

public class NoticeList : MonoBehaviour
{
    public class JsonNews
    {
        public int news_id;
        public string title;
        public string date;
        public string url;
    }

    public class NewsInfo {
        public int id;
        public string title;
        public string date;
        public string url;
    }

    public CommonList noticeList;
    public Button closeButton;
    public Image backgroundImage; // 画面全体を暗くするための背景
    public RectTransform frameTransform; // メニュー用の黒い背景
    public RectTransform menuTransform; // メニュー内部
    public List<JsonNews> listOrg;
    public ReactiveProperty<JsonNews[]> list = new ReactiveProperty<JsonNews[]>(new JsonNews[0]);

    private Color orgColor; // 画面全体を暗くするための背景の元の色

    

    private string[] debugText = {
        "新機能「持ってるリスト」がリリースされました。",
        "新色「春恋色」が登場しました！",
    };

    private string[] debugDate = {
        "2020/05/25",
        "2020/03/12",
    };

    void OnDestroy()
    {
        backgroundImage.DOKill();
        frameTransform.DOKill();
        menuTransform.DOKill();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (DataTable.Param.noUseAPI) {
            // APIを使わない場合
        } else {
            //TODO: API 接続
            ExportToCsv.instance.ImportEachLine(@"News.csv" , (index,line) => {
                // var data = new List<JsonNews>();
                if( index > -1 ) {
                    var news = new JsonNews();
                    // news.id = int.Parse(line[0]);
                    news.news_id = index;
                    news.title = line[1];
                    news.date = line[2];
                    news.url = line[3];
                    listOrg.Add(news);
                }

                // news_idで並び替えて表示
                list.Value = listOrg.OrderBy(v => v.news_id).ToArray();
                // 降順であればこちら
                // list.Value = listOrg.OrderByDescending(v => v.news_id).ToArray();
            });
        }

        // 編集用に横にずらしているのを元に戻す
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        noticeList.updateItem = UpdateNotice;
        list
            .Subscribe(v => noticeList.itemCount.SetValueAndForceNotify(v.Length))
            .AddTo(gameObject);

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
        // TODO: API
        if( list.Value.Length - 1 <= index) {
            return;
        }
        Debug.Log("■■■■■■■" + index + "■" +list.Value[index].date);
        item.text[0].text = list.Value[index].title;
        item.text[1].text = list.Value[index].date;

        // item.text[0].text = debugText[index % debugText.Length];
        // item.text[1].text = debugDate[index % debugDate.Length];
        

        // ボタンタップ時の挙動
        var disposable = item.button[0].OnClickAsObservable()
            .Subscribe(b => {
                Debug.Log(item.text[0].text + ": " + b);
                Application.OpenURL(list.Value[index].url);
            });
        item.disposableBag.Add(disposable);
    }

    // 開く
    public void OpenWindow()
    {
        CompleteAnimation();

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
