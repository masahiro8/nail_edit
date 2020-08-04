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

public class NailDetailList : MonoBehaviour
{
    public DrawMain main;
    // public NailSelectList list;
    public TopcoartSelect topcoartSelect;
    public CommonList detailList;
    public CommonItem detailItem;
    public CommonItem topcoartItem;
    public RectTransform[] animObj;
    public ReactiveProperty<int>[] favFlag = new ReactiveProperty<int>[Enum.GetValues(typeof(MyListType)).Length];
    public ReactiveProperty<bool> OnViewTopcoart = new ReactiveProperty<bool>(false);

    private NailTopcoatType topcoartStatus = NailTopcoatType.None;
    private Vector2 scrollVector = Vector2.zero;

    void Awake()
    {
        for (var i = 0; i < favFlag.Length; i++) {
            favFlag[i] = new ReactiveProperty<int>(-1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //変更を受け取る
        DataTable.Param.selectedNail
            .Subscribe(v => {
                UpdateDetail(detailItem, 0);
            })
            .AddTo(gameObject);

        //変更を受け取る
        DataTable.Param.topcoatType
            .SkipLatestValueOnSubscribe()
            .Subscribe( s => {
                topcoartStatus = s;
                showTopcoart(true);
            })
            .AddTo(gameObject);
        showTopcoart(false);

        detailList.updateItem = UpdateDetail;
        detailList.itemCount.Value = 2;
        detailList.GetComponent<SimpleScrollSnap>().Setup(true);

        var scrollRect = detailList.GetComponent<ScrollRect>();
        // var obj = scrollRect.content.GetChild(0);
        // var cItem = obj.GetComponent<CommonItem>();
        var scrollSnap = detailList.GetComponent<SimpleScrollSnap>();
        // Debug.Log(scrollRect.content.childCount);

        // detailList.updateItem = UpdateNail;
        detailList.itemCount.Value = 2;

        // ボタンサイズを変更
        // var listView = detailList.GetComponent<LoopListView2>();
        scrollRect.OnValueChangedAsObservable()
            .Subscribe(value => UpdateScroll(value))
            .AddTo(gameObject);
        UpdateScroll(Vector2.zero);

        // スクロールさせる
        detailItem.button[0].OnClickAsObservable()
            .Subscribe(_ => {
                scrollSnap.GoToPanel(1 - scrollSnap.TargetPanel);
            })
            .AddTo(gameObject);

        // urlを開く
        detailItem.button[1].OnClickAsObservable()
            .Subscribe(_ => {
                OpenUrl.productCode(DataTable.Param.selectedNail.Value);
                // Debug.Log(">> data.productCode " + data.productCode );
            })
            .AddTo(gameObject);
        
        // お気に入りボタン
        for (var i = 0; i < favFlag.Length; i++) {
            var useFlag = favFlag[i];
            var type = (MyListType)i;
            for (var j = 0; j < 2; j++) { // オンオフ
                var orgFlag = j;
                var button = detailItem.button[j + 2 + i * 2];
                // ボタンを押した時にお気に入りフラグを更新
                button.OnClickAsObservable()
                    .Subscribe(_ => {
                        useFlag.Value = orgFlag;
                        SetFavorite(type, orgFlag == 1);
                        // DataTable.MyList.Reset(type);
                    })
                    .AddTo(button.gameObject);

                // お気に入りフラグ更新時に表示状態を更新
                useFlag
                    .Subscribe(b => {
                        // Debug.Log("■■■■■ type: " + i + "," + type);
                        button.gameObject.SetActive(b != orgFlag);
                    })
                    .AddTo(button.gameObject);
            }
        }

        // モード切り替えでの表示位置制御
        DataTable.Param.mainView
            .Subscribe(type => {
                // 爪の表示制御
                gameObject.SetActive(type.IsShowNail());
            })
            .AddTo(gameObject);
    }

    private Vector3 GetBgScale ( Vector2 value ) {
        return new Vector3( 
            1.2f + value.x * 1.1f,//figma > 60px -> 230px
            1.2f + value.x * 2.6f,//figma > 50px -> 190px
            1
        );
    }

    private Vector3 GetImageScale ( Vector2 value ) {
        var s2 = 1.0f + value.x * 1.5f;
        return Vector3.one * s2;
    }

    private Vector3 GetImagePosition ( Vector2 value ) {
        return new Vector3(
            0.0f + ( -90.0f * value.x ) ,
            20.0f - ( 15.0f * value.x ),
            0
        );
    }

    private Vector2 GetInfoPosition ( Vector2 value ) {
        return new Vector2 ( 70.0f + ( -130.0f * value.x ), -30.0f);
    }

    private void UpdateScroll(Vector2 value)
    {
        scrollVector = value;

        //背景サイズ
        detailItem.button[0].transform.localScale = GetBgScale(value);
        
        //写真
        detailItem.image[0].transform.localScale = GetImageScale(value);
        var img_rect = detailItem.image[0].transform;
        img_rect.localPosition =  GetImagePosition(value);

        //info座標
        var rect = detailItem.extra[0].transform.GetComponent<RectTransform>();
        rect.offsetMin = GetInfoPosition(value);

        // 2つ目のアイテムはダミーなので消す
        var scrollRect = detailList.GetComponent<ScrollRect>();
        if (scrollRect.content.childCount > 1) {
            // var item = scrollRect.content.GetChild(1).GetComponent<CommonItem>();
            scrollRect.content.GetChild(1).gameObject.SetActive(false);
        }

        //トップコート
        showTopcoart(true);
    }

    private void showTopcoart (bool anim) {
        var f = anim ? 0.3f : 0;
        //トップコートを表示
        if( !topcoartStatus.IsNone() && scrollVector.x >= 0.9f ) {
            topcoartItem.gameObject.GetComponent<CanvasGroup>().DOFade(1.0F, f);

            Vector2 value = new Vector2(1.0f,1.0f);
            var trans = topcoartItem.gameObject.GetComponent<RectTransform>();

            //背景
            topcoartItem.button[0].transform.localScale = GetBgScale(value);

            //画像1
            topcoartItem.image[0].transform.localScale = GetImageScale(value);
            var img_rect_topcoart = topcoartItem.image[0].transform;
            Vector2 image_vec = GetImagePosition(value);
            img_rect_topcoart.localPosition =  new Vector2(image_vec.x - 30.0f , image_vec.y);

             //画像2
            topcoartItem.image[1].transform.localScale = GetImageScale(value);
            var img_rect_topcoart_2 = topcoartItem.image[1].transform;
            Vector2 image_vec_2 = GetImagePosition(value);
            img_rect_topcoart_2.localPosition =  new Vector2(image_vec_2.x - 30.0f , image_vec_2.y);

            //info
            Vector2 info_vec = GetInfoPosition(value);
            var rect_topcoart = topcoartItem.extra[0].transform.GetComponent<RectTransform>();
            rect_topcoart.offsetMin = new Vector2(info_vec.x - 60.0f,info_vec.y);

            if( OnViewTopcoart.Value == false) {
                OnViewTopcoart.Value = true;
                Debug.Log("■■■■■■■ NailDetaulList OnView true");
            }
        }else{
            topcoartItem.gameObject.GetComponent<CanvasGroup>().DOFade(0.0F, f);
            var trans = topcoartItem.gameObject.GetComponent<RectTransform>();
            var _rect_topcoart = topcoartItem.extra[0].transform.GetComponent<RectTransform>();
            _rect_topcoart.offsetMin = new Vector2 ( 0 , -30.0f);
            
            if( OnViewTopcoart.Value == true) {
                OnViewTopcoart.Value = false;
                Debug.Log("■■■■■■■ NailDetaulList OnView false");
            }
        }

        
    }

    private void UpdateDetail(CommonItem item, int index)
    {
        var data = DataTable.Param.selectedNail.Value;

        // 初期化時に-1が入ってくるので
        if (data == null) {
            return;
        }

        // お気に入りボタンの初期化
        foreach (MyListType type in Enum.GetValues(typeof(MyListType))) {
            favFlag[(int)type].Value = data.IsMyList(type) ? 1 : 0;
        }

        main.nailProcessing.SetInfoRecord(data);

        detailItem.text[0].text = data.productName;
        detailItem.text[1].text = data.subName;
        detailItem.text[2].text = data.productCode;

        var texBottle = Resources.Load("Textures/NailBottle/" + data.fileName) as Texture2D;
        if (texBottle) {
            detailItem.image[0].texture = texBottle;
            // detailItem.image[0].SetNativeSize();
            detailItem.image[0].enabled = true;
        } else {
            detailItem.image[0].enabled = false;
        }

        detailItem.button[1].gameObject.SetActive(data.existUseURL);
    }

    // お気に入り、持っているフラグを更新
    private void SetFavorite(MyListType type, bool flag)
    {
        // var index = DataTable.NailInfo.showList[list.nailList.itemIndex.Value];
        // var data = DataTable.NailInfo.list[index];
        var data = DataTable.Param.selectedNail.Value;

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
