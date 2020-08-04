using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SuperScrollView;
using DG.Tweening;
using DanielLochner.Assets.SimpleScrollSnap;

public class AfterSavingView : MonoBehaviour
{
    public DrawMain main;
    public Button openButton;
    public Button closeButton;
    public Image backgroundImage; // 画面全体を暗くするための背景
    public RectTransform[] frameTransform; // 遷移用の枠

    public Image doneImage;
    public Button backButton;
    public Button newButton;
    public Button shareButton;
    public Text[] textArray;

    private Color orgColor; // 画面全体を暗くするための背景の元の色

    private string[] textStr = new string[] {
        "AfterSaving: Done",
        "AfterSaving: Back",
        "AfterSaving: New",
        "AfterSaving: Share",
    };

    // Start is called before the first frame update
    void Start()
    {
        // エディット用のサイズを元に戻す
        transform.RestoreFillBound();

        // アニメーション用に色の保存
        orgColor = backgroundImage.color;

        // 消す
        frameTransform[1].gameObject.SetActive(false);

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

        // 戻る
        if (backButton) {
            backButton.OnClickAsObservable()
                .Subscribe(_ => {
                    CloseMenu(true);
                })
                .AddTo(gameObject);
        }

        // 新しい写真へ
        if (newButton) {
            newButton.OnClickAsObservable()
                .Subscribe(_ => {
                    CloseMenu(true);
                    DataTable.Param.mainView.Value = MainViewType.FixedCamera;
                })
                .AddTo(gameObject);
        }

        // シェア
        if (shareButton) {
            shareButton.OnClickAsObservable()
                .Subscribe(_ => {
                    main.renderScreenShot.SharePhoto();
                })
                .AddTo(gameObject);
        }

        // テキストを書き換える
        UnityEngine.Assertions.Assert.IsTrue(textArray.Length == textStr.Length);
        for (var i = 0; i < textArray.Length; i++) {
            textArray[i].text = textStr[i].Localized();
        }

        // ウィンドウを開く
        OpenMenu();
    }

    // 開く
    public void OpenMenu()
    {
        CompleteAnimation();

        // backgroundImage.raycastTarget = true;
        backgroundImage.color = Color.clear;
        // backgroundImage
        //     .DOColor(orgColor, DataTable.Param.duration);
        // 保存終了アニメーション
        doneImage.fillAmount = 0f;
        // doneImage
        //     .DOFillAmount(1f, DataTable.Param.duration);

        // アニメーション
        var sequence = DOTween.Sequence()
            .Append(backgroundImage
                .DOColor(orgColor, DataTable.Param.duration))
            .Append(doneImage
                .DOFillAmount(1f, DataTable.Param.duration))
            .Append(doneImage
                .DOColor(Color.clear, DataTable.Param.duration))
            .OnComplete(() => {
                frameTransform[0].gameObject.SetActive(false);
                frameTransform[1].gameObject.SetActive(true);
            });
        sequence.Play();

        // 撮影する
        var time1 = System.TimeSpan.FromSeconds(DataTable.Param.duration * 2);
        Observable.Timer(time1)
            .Subscribe(_ => main.renderScreenShot.SavePhoto())
            .AddTo(gameObject);
    }

    // 閉じる
    public void CloseMenu(bool anim)
    {
        CompleteAnimation();

        var sequence = DOTween.Sequence()
            .Append(backgroundImage
                .DOColor(Color.clear, DataTable.Param.duration))
            .OnComplete(() => {
                Destroy(gameObject);
            });
        sequence.Play();
    }

    // アニメーションを完了させる
    private void CompleteAnimation()
    {
        if (DOTween.IsTweening(backgroundImage)) {
            backgroundImage.DOComplete();
        }
        // if (DOTween.IsTweening(frameTransform)) {
        //     frameTransform.DOComplete();
        // }
        // if (DOTween.IsTweening(menuTransform)) {
        //     menuTransform.DOComplete();
        // }
    }
}
