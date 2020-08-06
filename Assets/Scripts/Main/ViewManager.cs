using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ViewManager : MonoBehaviour
{
    public DrawMain drawMain;
    public RectTransform canvasRectTransform; // Canvas
    public GameObject[] mainViewPrefabs;
    public MainViewType mainViewType = MainViewType.Movie;

    private GameObject mainView = null;

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Assertions.Assert.IsTrue(mainViewPrefabs.Length == System.Enum.GetValues(typeof(MainViewType)).Length);

        // デバッグから
        SROptions.Current.ChangeFixedMode
            .Subscribe(flag => {
                DataTable.Param.mainView.Value = flag
                    ? (DataTable.Param.useDummyImage
                            ? MainViewType.FixedEdit
                            : MainViewType.FixedCamera)
                    : MainViewType.Movie;
            })
            .AddTo(gameObject);

        // モード切り替えでのビュー変更
        DataTable.Param.mainView
            .Subscribe(type => {
                if (mainView != null) {
                    Destroy(mainView);
                    mainView = null;
                }

                switch (type) {
                    case MainViewType.FixedCamera:
                        drawMain.ChangeStopMode(false);
                        break;
                    default:
                        break;
                }

                // 爪の表示制御
                // Activeで操作すると爪が変になるので横にずらしておく
                drawMain.nailProcessing.transform.position = Vector3.left * (type.IsShowNail() ? 0 : 100);

                // インスタンスを作成
                mainView = Instantiate(mainViewPrefabs[(int)type], canvasRectTransform);

                // 最背面に移動させる
                mainView.transform.SetAsFirstSibling();

                // ボタン類をアタッチ
                var baseView = mainView.GetComponent<MainBaseView>();
                baseView.viewManager = this;
                drawMain.AttachButtons(baseView);
            })
            .AddTo(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
