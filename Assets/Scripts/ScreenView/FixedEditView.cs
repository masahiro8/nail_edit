using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class FixedEditView : MainBaseView
{
    public Button cancelButton;
    public Button saveButton;
    public GameObject saveView;
    public GameObject navigationView;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // UnityEngine.Assertions.Assert.IsTrue(frameObj.Length == System.Enum.GetValues(typeof(ShootModeType)).Length);

        // ナビゲーションガイドを出す
        if (!SaveName.NavigationDone.GetBool()) {
            Instantiate(navigationView, transform.parent.GetComponent<RectTransform>());
        }

        // キャンセルボタンを押した
        if (cancelButton) {
            cancelButton.OnClickAsObservable()
                .Subscribe(_ => {
                    DataTable.Param.mainView.Value = MainViewType.FixedCamera;
                })
                .AddTo(gameObject);
        }

        // 保存ボタンを押した
        if (saveButton) {
            saveButton.OnClickAsObservable()
                .SubscribeOnMainThread()
                .Subscribe(_ => {
                    var obj = Instantiate(saveView, transform.parent.GetComponent<RectTransform>());
                    var afterSavingView = obj.GetComponent<AfterSavingView>();
                    afterSavingView.main = viewManager.drawMain;
                })
                .AddTo(gameObject);
        }

        // デバッグボタンを押した
        if (debugButton) {
            // ボタンの表示切り替え
            SROptions.Current.NailRotateAdjustButton
                .Subscribe(b => debugButton.gameObject.SetActive(b))
                .AddTo(gameObject);

            // // 爪方向補正の使用切り替えを行う
            // debugButton.OnClickAsObservable()
            //     .Subscribe(_ => {
            //         SROptions.Current.NailRotateAdjust = !SROptions.Current.NailRotateAdjust;
            //         // メッシュを作り直す
            //         viewManager.drawMain.nailProcessing.Invoke(viewManager.drawMain.nailDetection, viewManager.drawMain.delayView.texture);
            //     })
            //     .AddTo(gameObject);
        }

        // エディットモードではボタンを消す
        if (DataTable.Param.useDummyImage) {
            // foreach (var button in new Button[] { menuButton, cancelButton, saveButton }) {
            //     if (button) {
            //         button.gameObject.SetActive(false);
            //     }
            // }
            if (menuButton) {
                menuButton.gameObject.SetActive(false);
            }
            if (cancelButton) {
                cancelButton.gameObject.SetActive(false);
            }
            if (saveButton) {
                saveButton.gameObject.SetActive(false);
            }
        }
    }
}
