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

public class ShootModeView : MonoBehaviour
{
    public DrawMain main;
    public Button openButton;
    public Button closeButton;
    // public Image backgroundImage; // 画面全体を暗くするための背景
    public RectTransform frameTransform; // メニュー用の黒い背景
    public RectTransform menuTransform; // メニュー内部
    public PageScrollView pageScrollView; // 閉じる時に消すため

    public Button[] handSideButtons;
    public Button[] shootModeButtons;

    // void Awake()
    // {
    //     // エディット用のサイズを元に戻す
    //     transform.RestoreFillBound();
    // }

    // Start is called before the first frame update
    void Start()
    {
        pageScrollView.parentList = gameObject;
        pageScrollView.closeToDestroy = true;

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

        // 右手左手選択
        ShootHandType[] shootHandTable = {
            ShootHandType.Left,
            ShootHandType.Right,
        };
        for (var i = 0; i < handSideButtons.Length; i++) {
            var button = handSideButtons[i];
            var type = shootHandTable[(i / 2)];
            var flag = i % 2 == 0;
            if (button) {
                // ボタンを押した時
                button.OnClickAsObservable()
                    .Select(_ => flag)
                    .Subscribe(_ => DataTable.Param.shootHand.Value = type)
                    .AddTo(gameObject);

                // 状態が変わった時にActiveを変更
                DataTable.Param.shootHand
                    .Subscribe(t => button.gameObject.SetActive((type == t) == flag))
                    .AddTo(gameObject);
            }
        }

        // 撮影モード選択
        // SaveName.FreeModeAlertDone.SetBool(false); // デバッグ時に起動ごとにアラートをリセット
        ShootModeType[] shootModeTable = {
            ShootModeType.HandPaa,
            ShootModeType.HandGoo,
            ShootModeType.Foot,
            ShootModeType.Free,
        };
        for (var i = 0; i < shootModeButtons.Length; i++) {
            var button = shootModeButtons[i];
            var type = shootModeTable[(i / 2)];
            var flag = i % 2 == 0;
            if (button) {
                // ボタンを押した時
                button.OnClickAsObservable()
                    .Select(_ => flag)
                    .Subscribe(_ => SetShootModeWithAlert(type))
                    .AddTo(gameObject);

                // 状態が変わった時にActiveを変更
                DataTable.Param.shootMode
                    .Subscribe(t => button.gameObject.SetActive((type == t) == flag))
                    .AddTo(gameObject);
            }
        }

        // デバッグ表示
        DataTable.Param.shootHand
            .Subscribe(t => Debug.Log("ShootHandType" + t))
            .AddTo(gameObject);
        DataTable.Param.shootMode
            .Subscribe(t => Debug.Log("ShootModeType" + t))
            .AddTo(gameObject);

        // ウィンドウを開く
        CloseMenu(false);
        OpenMenu();
    }

    // フリーモード選択時の警告アラート
    void SetShootModeWithAlert(ShootModeType type) {
        // DataTable.Param.shootMode.Value.SetWithAlert(type);
        if (DataTable.Param.shootMode.Value == type) {
            return;
        }

        // アラートを出さない
        if (SaveName.FreeModeAlertDone.GetBool() || !type.IsUseFreeModeAlert()) {
            DataTable.Param.shootMode.Value = type;
            return;
        }

        NPBinding.UI.ShowAlertDialogWithMultipleButtons(
            null,
            "FreeModeAlert: Title".Localized(),
            new string[] {
                "FreeModeAlert: Off".Localized(),
                "OK".Localized(),
            },
            (value) => {
                Debug.Log("ShowAlertDialogWithMultipleButtons: " + value);
                if (value == "FreeModeAlert: Off".Localized()) {
                    SaveName.FreeModeAlertDone.SetBool(true);
                }
                DataTable.Param.shootMode.Value = type;
            });
    }

    // 開く
    public void OpenMenu()
    {
        CompleteAnimation();

        // backgroundImage.raycastTarget = true;
        // backgroundImage
        //     .DOColor(orgColor, DataTable.Param.duration);
        frameTransform
            .DOAnchorPosY(-667, DataTable.Param.duration);
        menuTransform
            .DOAnchorPosY(-32, DataTable.Param.duration);
    }

    // 閉じる
    public void CloseMenu(bool anim)
    {
        CompleteAnimation();

        // backgroundImage.raycastTarget = false;
        // backgroundImage
        //     .DOColor(Color.clear, DataTable.Param.duration);
        // frameTransform
        //     .DOAnchorPosY(-Screen.height, DataTable.Param.duration);
        menuTransform
            .DOAnchorPosY(-Screen.height, DataTable.Param.duration);
            // .OnComplete(() => {
            //     if (anim) {
            //         Destroy(gameObject);
            //     }
            // });
    }

    // アニメーションを完了させる
    private void CompleteAnimation()
    {
        // if (DOTween.IsTweening(backgroundImage)) {
        //     backgroundImage.DOComplete();
        // }
        if (DOTween.IsTweening(frameTransform)) {
            frameTransform.DOComplete();
        }
        if (DOTween.IsTweening(menuTransform)) {
            menuTransform.DOComplete();
        }
    }
}
