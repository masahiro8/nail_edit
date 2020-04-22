using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using SuperScrollView;

public class CommonItem : MonoBehaviour
{
    public Text[] text;
    public RawImage[] image;
    public SVGImage[] svgImage;
    public Button[] button;
    public RawImage selectedBar;

    private CompositeDisposable disposableBag = new CompositeDisposable();
    private RectTransform textRectTransform = null;
    private Vector2 textOffsetMin1;
    private Vector2 textOffsetMin2;

    void OnDestroy()
    {
        disposableBag.Clear();
    }

    public void UpdateContext(ItemType itemType, CommonList selector, int n)
    {
        // それまでの講読を中止
        disposableBag.Clear();

        switch (itemType) {
            case ItemType.NailSelect:
                UpdateNail(selector, n);
                break;
            case ItemType.MainMenu:
                UpdateMenu(selector, n);
                break;
            case ItemType.MyList1:
                UpdateFavorite(selector, MyListType.Favorite, n);
                break;
            case ItemType.MyList2:
                UpdateFavorite(selector, MyListType.Have, n);
                break;
            case ItemType.MyListSelect:
                UpdateFavoriteSelect(selector, n);
                break;
            case ItemType.Toturial:
                UpdateTutorial(selector, n);
                break;
            case ItemType.ToturialDot:
                UpdateDot(selector, n);
                break;
            case ItemType.NailCategory:
                UpdateCategory(selector, n);
                break;
            default:
                break;
        }

        // ボタンタップ時の挙動
        if (button.Length > 0) {
            var disposable = button[0].OnClickAsObservable()
                .Subscribe(b => {
                    selector.itemIndex.Value = n;
                });
            disposableBag.Add(disposable);
        }

        // 選択状態バーを移動させる
        if (selectedBar) {
            selectedBar.enabled = n == selector.itemIndex.Value;
            var disposable = selector.itemIndex
                .Zip(selector.itemIndex.Skip(1), (x, y) => new System.Tuple<int, int>(x, y))
                .Subscribe(value => {
                    selectedBar.enabled = n == value.Item2;
                    if (n == value.Item2) {
                        transform.SetAsLastSibling(); // 最前面に移動
                        selectedBar.rectTransform.DOAnchorPosX((value.Item1 - value.Item2) * selectedBar.rectTransform.rect.width, 0);
                        selectedBar.rectTransform.DOAnchorPosX(0, DataTable.Param.selectedBarDuration);
                    }
                });
            disposableBag.Add(disposable);
        }
    }

    public void UpdateMenu(CommonList selector, int n)
    {
        // 初期化
        if (textRectTransform == null) {
            textRectTransform = text[0].rectTransform;
            textOffsetMin1 = textRectTransform.offsetMin;
            textOffsetMin2 = textOffsetMin1;
            textOffsetMin2.x += image[0].rectTransform.sizeDelta.x;
            textOffsetMin2.x += image[0].rectTransform.offsetMin.x;
        }

        var data = DataTable.Menu.list[n];
        text[0].text = data.name.Localized();
        image[0].texture = data.icon;
        image[0].enabled = data.icon != null;

        textRectTransform.offsetMin = data.icon == null ? textOffsetMin1 : textOffsetMin2;

        // ボタンタップ時の挙動
        var disposable = button[0].OnClickAsObservable()
            .Subscribe(b => {
                Debug.Log(text[0].text + ": " + b);
                switch (n) {
                    case 0:
                        selector.transform.parent.parent.GetComponent<MainMenuList>().favoriteList.OpenMenu(0);
                        break;
                    case 1:
                        selector.transform.parent.parent.GetComponent<MainMenuList>().favoriteList.OpenMenu(1);
                        break;
                    case 6:
                    case 7:
                        selector.transform.parent.parent.GetComponent<MainMenuList>().OpenWebView(data.url);
                        break;
                    case 8:
                        SaveName.TutorialDone.SetBool(false);
                        break;
                    default:
                        break;
                }
            });
        disposableBag.Add(disposable);
    }

    public void UpdateFavorite(CommonList selector, MyListType mlType, int n)
    {
        var index = DataTable.MyList.filterdList[(int)mlType].Value[n];
        var data = DataTable.NailInfo.list[index];
        text[0].text = data.productCode;
    }

    public void UpdateFavoriteSelect(CommonList selector, int n)
    {
        var data = DataTable.MyList.list[n];
        text[0].text = data.name.Localized();
    }

    public void UpdateTutorial(CommonList selector, int n)
    {
        var data = DataTable.Tutorial.list[n];
        image[0].texture = data.image;
    }

    public void UpdateDot(CommonList selector, int n)
    {
        var data = DataTable.Tutorial.list[n];
        var disposable = selector.itemIndex
            .Subscribe(value => {
                svgImage[0].enabled = n != value;
                svgImage[1].enabled = n == value;
            });
        disposableBag.Add(disposable);
    }

    public void UpdateNail(CommonList selector, int n)
    {
        var index = DataTable.NailInfo.showList[n];
        var data = DataTable.NailInfo.list[index];
        // text[0].text = "No." + data.index;
        text[0].text = data.colorNumber;

        // テスト
        var nailData = Resources.Load<NailMaterialTable>("Data/NailMaterial/" + data.fileName);
        if (nailData) {
            svgImage[0].enabled = nailData.list.Length > 0; // new
        } else {
            svgImage[0].enabled = false; // new
        }
        svgImage[1].enabled = n % 10 == 0; // dot

        // var key = data.productCode.Substring(0, 4) + "_" + data.colorNumber;
        // var texBottle = Resources.Load("Textures/NailBottle/" + data.fileName) as Texture2D;
        var texSample = Resources.Load<Texture2D>("Textures/NailSample/" + data.fileName);
        if (texSample) {
            image[0].texture = texSample;
            image[0].SetNativeSize();
            image[0].enabled = true;
        } else {
            image[0].enabled = false;
        }
    }

    public void UpdateCategory(CommonList selector, int n)
    {
        var index = DataTable.Category.showList[n];
        var data = DataTable.Category.list[index];
        text[0].text = data.name.Localized();
    }
}
