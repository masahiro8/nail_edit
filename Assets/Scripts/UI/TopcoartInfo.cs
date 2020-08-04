using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using System;

public class TopcoartInfo : MonoBehaviour
{
    public NailDetailList nailDetail;
    // public TopcoartSelect topcoartSelect;
    public List<NailInfoRecord> items = new List<NailInfoRecord>();
    
    public string[] ClearNumbers;
    public string[] MatNumbers;

    //Enumがエラーになるので、2を入れる
    //Assets/Scripts/UI/TopcoartInfo.cs(15,72): error CS0103: The name 'Enum' does not exist in the current context
    public ReactiveProperty<int>[] favFlag = new ReactiveProperty<int>[2];

    // private int selected = 0;
    public ReactiveProperty<int> selected = new ReactiveProperty<int>(0);

    void Awake()
    {
        for (var i = 0; i < favFlag.Length; i++) {
            favFlag[i] = new ReactiveProperty<int>(-1);
        }
    }

    void Start()
    {
        CommonItem topcoartItem = gameObject.GetComponent<CommonItem>();

        var BtnImage = topcoartItem.button[6];
        BtnImage.OnClickAsObservable()
            .Subscribe(_ => {
                this.onClick();
            })
            .AddTo(BtnImage.gameObject);
        
        var BtnImage2 = topcoartItem.button[7];
        BtnImage2.OnClickAsObservable()
            .Subscribe(_ => {
                this.onClick();
            })
            .AddTo(BtnImage2.gameObject);

        //変更した情報
        DataTable.Param.topcoatType
            .Subscribe( type => {
                string[] infos = null;
                switch (type) {
                    case NailTopcoatType.Clear:
                        infos = ClearNumbers;
                        break;
                    case NailTopcoatType.Mat:
                        infos = MatNumbers;
                        break;
                    case NailTopcoatType.None:
                    default:
                        break;
                }
                if( infos != null ) {
                    selected.Value = 0;
                    items = new List<NailInfoRecord>();
                    foreach (string info in infos) {
                        var detail = DataTable.NailInfo.list
                            .Where( x => x.colorNumber == info )
                            .FirstOrDefault();
                        items.Add(detail);
                    }
                    this.setImageScale();
                    this.setItems();
                    this.SetFavoriteInit();
                    this.SetFavoriteButton();
                }
            })
            .AddTo(gameObject);

        //トップコート表示判定
        nailDetail.OnViewTopcoart
            .Subscribe( b => {
                if( b == true ) {
                    Invoke("setImageScale", 1.0f);
                    Invoke("SetFavoriteInit", 1.0f);
                    Invoke("SetFavoriteButton", 1.0f);
                }
            })
            .AddTo(gameObject);
    }

    private void setItems () {
        CommonItem topcoartItem = gameObject.GetComponent<CommonItem>();
        topcoartItem.text[0].text = items[selected.Value].productName;
        topcoartItem.text[1].text = items[selected.Value].subName;
        topcoartItem.text[2].text = items[selected.Value].productCode;
        for ( var i = 0; i < items.Count; i++ ) {
            items[i].SetBottleTexture(topcoartItem.image[i]);
        }
    }

    private void onClick(){
        if( items.Count == 2 ) {
            selected.Value = selected.Value == 0 ? 1:0;
            this.setImageScale();
            this.setItems();
            this.SetFavoriteInit();
            this.SetFavoriteButton();
        }
    }

    private void setImageScale () {
        CommonItem topcoartItem = gameObject.GetComponent<CommonItem>();

        //ドット
        if( items.Count < 2 ) {
            topcoartItem.extra[2].gameObject.SetActive(false);
            topcoartItem.extra[3].gameObject.SetActive(false);
        } else {
            topcoartItem.extra[2].gameObject.SetActive(true);
            topcoartItem.extra[3].gameObject.SetActive(true);
            if(selected.Value == 0) {
                topcoartItem.extra[2].gameObject.transform.DOLocalMoveX(-116.0f,0);
                topcoartItem.extra[3].gameObject.transform.DOLocalMoveX(-100.0f,0);
            }else{
                topcoartItem.extra[2].gameObject.transform.DOLocalMoveX(-100.0f,0);
                topcoartItem.extra[3].gameObject.transform.DOLocalMoveX(-116.0f,0);
            }
        }        

        //画像
        if( items.Count == 2 ) {
            topcoartItem.image[0].transform.DOLocalMoveX(-130.0f , 0.3f);        
            topcoartItem.image[selected.Value].DOFade(1.0F, 0.3f);
            topcoartItem.image[selected.Value].transform.DOScale(Vector3.one * 2.1f, 0.3f);
            topcoartItem.image[selected.Value].transform.DOLocalMoveY(20.0f , 0.3f);
            topcoartItem.image[1].gameObject.SetActive(true);
            topcoartItem.image[1].transform.DOLocalMoveX(-100.0f , 0.3f);
            var notSelected = selected.Value == 0 ? 1 : 0;
            topcoartItem.image[notSelected].DOFade(0.5F, 0.3f);
            topcoartItem.image[notSelected].transform.DOScale(Vector3.one * 1.6f , 0.3f);
            topcoartItem.image[notSelected].transform.DOLocalMoveY(35.0f , 0.3f);
        }else{
            topcoartItem.image[0].transform.DOScale(Vector3.one * 2.1f, 0.3f);
            topcoartItem.image[selected.Value].transform.DOLocalMoveY(20.0f , 0.3f);
            topcoartItem.image[1].gameObject.SetActive(false);
        }
    }

    private void SetFavoriteInit(){
        foreach (MyListType type in Enum.GetValues(typeof(MyListType))) {
            var i = (int)type;
            favFlag[i].Value = items[selected.Value].IsMyList(type) ? 1 : 0;
        }
    }

    private void SetFavoriteButton(){
        CommonItem topcoartItem = gameObject.GetComponent<CommonItem>();
        // // お気に入りボタン
        for (var i = 0; i < favFlag.Length; i++) {
            var useFlag = favFlag[i];
            var type = (MyListType)i;
            for (var j = 0; j < 2; j++) { // オンオフ
                var orgFlag = j;
                var button = topcoartItem.button[j + 2 + i * 2];
                // ボタンを押した時にお気に入りフラグを更新
                button.OnClickAsObservable()
                    .Subscribe(_ => {
                        useFlag.Value = orgFlag;
                        SetFavorite(type, orgFlag == 1);
                        DataTable.MyList.Reset(type);
                    })
                    .AddTo(button.gameObject);

                // お気に入りフラグ更新時に表示状態を更新
                useFlag
                    .Subscribe(b => {
                        // Debug.Log("type: " + i +"," + type+ " ("+ orgFlag + "!=" + b +")" );
                        button.gameObject.SetActive(b != orgFlag);
                    })
                    .AddTo(button.gameObject);
            }
        }
    }

    // お気に入り、持っているフラグを更新
    private void SetFavorite(MyListType type, bool flag)
    {
        //itemsから取り出す
        var key = type.ToString() + items[selected.Value].productCode;
        SaveName.MyListItem.SetBool(key, flag);
        // Debug.Log("SetFavorite -> " + key + ": " + SaveName.MyListItem.GetBool(key));
        
        // //ここからapiにリクエスト
        // // string app_token = SaveName.AppToken.GetString("");
        // JsonMyNailParam favorite = new JsonMyNailParam();
        // favorite.nail_code = items[selected.Value].productCode;
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
