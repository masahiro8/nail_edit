using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using SuperScrollView;
using DanielLochner.Assets.SimpleScrollSnap;

public class MyListList : MonoBehaviour
{
    public CommonList selectList;
    public CommonList frameList;
    public Button closeButton;
    public Image backgroundImage; // 画面全体を暗くするための背景
    public RectTransform frameTransform; // メニュー用の黒い背景
    public RectTransform menuTransform; // メニュー内部

    // private SimpleScrollSnap scrollSnap;
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

        selectList.updateItem = UpdateSelect;
        selectList.itemCount.Value = DataTable.MyList.list.Length;
        frameList.updateItem = UpdateFrame;
        frameList.itemCount.Value = DataTable.MyList.list.Length + 4; // 個数が少ないとなぜかスクロールしないので

        // ボタン押し
        if (closeButton) {
            closeButton.OnClickAsObservable()
                .Subscribe(_ => CloseMenu(true))
                .AddTo(gameObject);
        }

        // アニメーション用に色の保存
        orgColor = backgroundImage.color;
        // CloseMenu();

        // 閉じておく
        CloseMenu(false);

        // お気に入りと持っているの切り替え時
        selectList.itemIndex
            // .Zip(selectList.itemIndex.Skip(1), (x, y) => new System.Tuple<int, int>(x, y))
            .Subscribe(value => {
                var listView2 = frameList.GetComponent<LoopListView2>();
                if (listView2) {
                    // リスト表示をスクロールさせる
                    // listView2.MovePanelToItemIndex(value, 100);
                    listView2.SetSnapTargetItemIndex(value);
                    // カートの表示状態の更新など
                    listView2.GetComponent<CommonList>().Reset();
                }
            })
            .AddTo(gameObject);

        // // お気に入りが更新されたので表示リストを更新する
        // foreach (MyListType type in Enum.GetValues(typeof(MyListType))) {
        //     var i = (int)type;
        //     var scrollRect = frameList.GetComponent<ScrollRect>();
        //     var commonList = scrollRect.content.GetChild(i).GetChild(0).GetComponent<CommonList>();
        //     // commonList.itemType = type.GetItemType();
        //     commonList.updateItem = (item2, index2) => UpdateMyList(item2, index2, type);
        //     commonList.itemCount.Value = DataTable.MyList.filterdList[(int)type].Value.Length;
        //     DataTable.MyList.filterdList[i]
        //         .Subscribe(l => {
        //             // var commonList = frameList.GetComponent<CommonList>();
        //             commonList.Reset();
        //         })
        //         .AddTo(gameObject);
        // }
    }

    // リストの選択セル
    private void UpdateSelect(CommonItem item, int index)
    {
        var data = DataTable.MyList.list[index];
        item.text[0].text = data.name.Localized();
        item.text[1].text = item.text[0].text;

        // ボタンが押されたときに文字色の切り替え
        var disposable = selectList.itemIndex
            .Subscribe(n => {
                item.text[0].enabled = n == index;
                item.text[1].enabled = n != index;
            });
        item.disposableBag.Add(disposable);
    }

    // フレームが変わった
    private void UpdateFrame(CommonItem item, int index)
    {
        if (index >= Enum.GetValues(typeof(MyListType)).Length) {
            return;
        }

        item.text[0].text = "NoNailInList".Localized();
        var commonList = item.extra[0].GetComponent<CommonList>();
        commonList.updateItem = (item2, index2) => UpdateMyList(item2, index2, (MyListType)index);;
        // commonList.itemCount.Value = DataTable.MyList.filterdList[index].Value.Length;

        // お気に入りや持っているリストが変更されたときにリアルタイムで要素を変更
        var disposable = DataTable.MyList.filterdList[index]
            .Subscribe(list => {
                item.text[0].enabled = list.Length == 0;
                commonList.itemCount.SetValueAndForceNotify(list.Length);
                commonList.Reset();
            });
        item.disposableBag.Add(disposable);
    }

    // リストの表示セル
    private void UpdateMyList(CommonItem item, int index, MyListType mlType)
    {
        var data = DataTable.NailInfo.list[DataTable.MyList.filterdList[(int)mlType].Value[index]];
        item.text[0].text = data.productCode;
        item.text[3].text = "SaleDone".Localized();

        data.SetBottleTexture(item.image[0]);
        data.SetSampleTexture(item.image[1]);
        // item.svgImage[0].gameObject.SetActive(mlType==MyListType.Favorite?true:false);
        // item.svgImage[1].gameObject.SetActive(mlType==MyListType.Have?true:false);

        if (data.IsSaleDone()) {
            // 販売終了
            item.button[1].gameObject.SetActive(false);
            item.svgImage[0].enabled = true;
            item.text[3].enabled = true;
        } else if (data.existUseURL) {
            // 販売中
            item.button[1].gameObject.SetActive(true);
            item.svgImage[0].enabled = false;
            item.text[3].enabled = false;
        } else {
            // URLなし
            item.button[1].gameObject.SetActive(false);
            item.svgImage[0].enabled = false;
            item.text[3].enabled = false;
        }

        // urlを開く
        var disposable = item.button[1].OnClickAsObservable()
            .Subscribe(_ => {
                OpenUrl.productCode(data);
                // Debug.Log(">> data.productCode " + data.productCode );
            });
        item.disposableBag.Add(disposable);

        for (var i = 2; i < 6; i++) { // オンオフ
            item.button[i].gameObject.SetActive(false);
        }

        // お気に入りボタン
        var useFlag = new ReactiveProperty<int>(data.IsMyList(mlType) ? 1 : 0);
        // var type = (MyListType)i;
        for (var j = 0; j < 2; j++) { // オンオフ
            var orgFlag = j;
            var button = item.button[j + 2 + (int)mlType * 2];
            // ボタンを押した時にお気に入りフラグを更新
            disposable = button.OnClickAsObservable()
                .Subscribe(_ => {
                    if (orgFlag == 0 && data.IsSaleDone()) {
                        NPBinding.UI.ShowAlertDialogWithMultipleButtons(
                            "",
                            "SaleDoneAlert".Localized(),
                            new string[]{"Cancel".Localized(), "DeleteFav".Localized()},
                            (str) => {
                                if (str == "DeleteFav".Localized()) {
                                    useFlag.Value = orgFlag;
                                    SetFavorite(data, mlType, orgFlag == 1);
                                }
                            });
                    } else {
                        useFlag.Value = orgFlag;
                        SetFavorite(data, mlType, orgFlag == 1);
                    }
                    // DataTable.MyList.Reset(type);
                });
            item.disposableBag.Add(disposable);

            // お気に入りフラグ更新時に表示状態を更新
            disposable = useFlag
                .Subscribe(b => {
                    // Debug.Log("■■■■■ type: " + i + "," + type);
                    button.gameObject.SetActive(b != orgFlag);
                });
            item.disposableBag.Add(disposable);
        }
    }

    // 開く
    public void OpenMenu(int n)
    {
        CompleteAnimation();

        // リストを更新
        foreach (MyListType type in Enum.GetValues(typeof(MyListType))) {
            DataTable.MyList.Reset(type);
        }

        // 選択状態を更新
        selectList.itemIndex.Value = n;

        // アニメーションを見せないために強制移動
        var scrollRect = frameList.GetComponent<ScrollRect>();
        float f = (float)n / (float)(scrollRect.content.transform.childCount - 1);
        scrollRect.horizontalNormalizedPosition = f;

        backgroundImage.raycastTarget = true;
        backgroundImage
            .DOColor(orgColor, DataTable.Param.duration);
        frameTransform
            .DOAnchorPosY(0, DataTable.Param.duration);
        menuTransform
            .DOAnchorPosY(0, DataTable.Param.duration);
    }

    // 閉じる
    public void CloseMenu(bool anim)
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

    // お気に入り、持っているフラグを更新
    private void SetFavorite(NailInfoRecord data, MyListType type, bool flag)
    {
        // var index = DataTable.NailInfo.showList[list.nailList.itemIndex.Value];
        // var data = DataTable.NailInfo.list[index];
        // var data = DataTable.Category.showList[list.categoryList.itemIndex.Value].filter.Value[list.nailList.itemIndex.Value];

        var key = type.ToString() + data.productCode;
        // SaveName.FavoriteItem.SetBool(data.name, !SaveName.FavoriteItem.GetBool(data.name));
        SaveName.MyListItem.SetBool(key, flag);
        Debug.Log("SetFavorite -> " + key + ": " + SaveName.MyListItem.GetBool(key));
        
        // //ここからapiにリクエスト
        // // string app_token = SaveName.AppToken.GetString("");
        // JsonMyNailParam favorite = new JsonMyNailParam();
        // favorite.nail_code = data.productCode;
        // string _favorite = JsonUtility.ToJson(favorite);
        // //登録・削除
        // string method = flag==true?"POST":"DELETE";
        // switch (type) {
        //     case MyListType.Favorite:
        //         apiUsers.instance.FavoriteNail(method ,_favorite);
        //         return;
        //     case MyListType.Have:
        //         apiUsers.instance.OwnedNail(method ,_favorite);
        //         return;
        //     default:
        //         return;
        // }
    }
}
