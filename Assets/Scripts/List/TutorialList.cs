#define NAIL_EDIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using SuperScrollView;
using DanielLochner.Assets.SimpleScrollSnap;
#if !NAIL_EDIT
using Firebase;
#endif

public class TutorialList : MonoBehaviour
{
    public CommonList tutorialList;
    public ScrollRect scrollRect;
    public ScrollRect dotScrollRect;
    public DotsView dotView;
    public Button BtnSkip;
    public Button BtnStart;
    public Button BtnNext;
    public DrawMain drawMain;
    public PermissionAuthorization permission;

    private int prevIndex;

    void Awake()
    {
        // 読み込みのため
        var _ = DataTable.Instance;

        if (DataTable.Param.useDummyImage) {
            SaveName.TutorialDone.SetBool(true);
            SaveName.NavigationDone.SetBool(true);
            SaveName.NavigationGuideDone.SetBool(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // パーミッションの結果カメラを開く
        permission.done
            .SkipLatestValueOnSubscribe()
            .Subscribe(b => {
                if (b) {
                    drawMain.gameObject.SetActive(true);
                }
            })
            .AddTo(gameObject);

        // 編集用に横にずらしているのを元に戻す
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        tutorialList.updateItem = UpdateTutorial;
        tutorialList.itemCount.Value = DataTable.Tutorial.list.Length;
        dotView.itemCount.Value = DataTable.Tutorial.list.Length;

        tutorialList.GetComponent<SimpleScrollSnap>().Setup(true);

        //初期化
        prevIndex = 0;
        dotView.itemIndex.Value = prevIndex;

        // ドットの表示をスクロールと同期させる
        var listView = scrollRect.GetComponent<LoopListView2>();
        scrollRect.OnValueChangedAsObservable()
            .Subscribe(value => {
               
                var n = (int)(value.x * (float)DataTable.Tutorial.list.Length);
                var _v =  Mathf.Clamp(n, 0, DataTable.Tutorial.list.Length);
                var len = DataTable.Tutorial.list.Length - 1;

                //ページ変更
                //たぶんdotListのOnGetItemByIndexはスクロールしないので変化を検出しない
                //自前でページの変更を監視
                if( prevIndex != _v ) {
                    dotView.itemIndex.Value = _v;
                    prevIndex = _v;
                }

                //最後のページ
                BtnSkip.gameObject.SetActive(_v != len);
            })
            .AddTo(gameObject);

        //開始ボタン
        BtnStart.OnClickAsObservable()
            .Subscribe( _=>{
                this.StartApp();
            })
            .AddTo(this.gameObject);

        //スキップボタン
        BtnSkip.OnClickAsObservable()
            .Subscribe( _=>{
                this.StartApp();
            })
            .AddTo(this.gameObject);
        //次へ
        BtnNext.OnClickAsObservable()
            .Subscribe(_ => {
                var scrollSnap = scrollRect.GetComponent<SimpleScrollSnap>();
                scrollSnap.GoToPanel(scrollSnap.TargetPanel + 1);
            })
            .AddTo(gameObject);

        // すでに表示済みの場合は消しておく
        if (SaveName.TutorialDone.GetBool()) {
            gameObject.SetActive(false);
            // カメラを起動
            // drawMain.gameObject.SetActive(true);
            RequestCamera();

            // 通知の初期化
#if !NAIL_EDIT
            FirebaseManager.Instance.SetupMessaging();
#endif
        } else {
#if !NAIL_EDIT
            Firebase.Analytics.FirebaseAnalytics
                .LogEvent(Firebase.Analytics.FirebaseAnalytics.EventTutorialBegin);
#endif
        }
    }

    private void StartApp()
    {
        gameObject.SetActive(false);
        SaveName.TutorialDone.SetBool(true);
        // カメラを起動
        // drawMain.gameObject.SetActive(true);
        RequestCamera();

#if !NAIL_EDIT
        // チュートリアル完了を送信
        Firebase.Analytics.FirebaseAnalytics
            .LogEvent(Firebase.Analytics.FirebaseAnalytics.EventTutorialComplete);

        // 通知の初期化
        FirebaseManager.Instance.SetupMessaging();
#endif
    }

    private void UpdateTutorial(CommonItem item, int index)
    {
        var data = DataTable.Tutorial.list[index];
        item.image[0].texture = data.image;
    }

    private void UpdateDot(CommonItem item, int index)
    {
        var data = DataTable.Tutorial.list[index];
        var disposable = tutorialList.itemIndex
            .Subscribe(value => {
                item.svgImage[0].enabled = index != value;
                item.svgImage[1].enabled = index == value;
            });
        item.disposableBag.Add(disposable);
    }

    private void RequestCamera()
    {
        // StartCoroutine("RequestCameraCoroutine");
        permission.gameObject.SetActive(true);
    }
}
