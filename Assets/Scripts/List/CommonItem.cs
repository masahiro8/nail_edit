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

    private RectTransform textRectTransform = null;
    private Vector2 textOffsetMin1;
    private Vector2 textOffsetMin2;

    public void UpdateContext(ItemType itemType, CommonList selector, int n)
    {
        switch (itemType) {
            case ItemType.NailSelect:
                UpdateNail(selector, n);
                break;
            case ItemType.MainMenu:
                UpdateMenu(selector, n);
                break;
            case ItemType.MyList:
                UpdateFavorite(selector, n);
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
            default:
                break;
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
        text[0].text = data.name;
        image[0].texture = data.icon;
        image[0].enabled = data.icon != null;

        textRectTransform.offsetMin = data.icon == null ? textOffsetMin1 : textOffsetMin2;

        // ボタンタップ時の挙動
        button[0].OnClickAsObservable()
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
                        // var webViewGameObject = new GameObject("UniWebView");
                        // // webViewGameObject.transform.parent = transform;
                        // var webView = webViewGameObject.AddComponent<UniWebView>();
                        // webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
                        selector.transform.parent.parent.GetComponent<MainMenuList>().OpenWebView(data.url);
                        break;
                    case 8:
                        SaveName.TutorialDone.SetBool(false);
                        break;
                    default:
                        break;
                }
            })
            .AddTo(gameObject);
    }

    public void UpdateFavorite(CommonList selector, int n)
    {
        var data = DataTable.Nail.list[n];
        text[0].text = data.name;
        // image[0].texture = data.icon;

        // ボタンタップ時の挙動
        button[0].OnClickAsObservable()
            .Subscribe(b => {
                Debug.Log(text[0].text + ": " + b);
            })
            .AddTo(gameObject);
    }

    public void UpdateFavoriteSelect(CommonList selector, int n)
    {
        var data = DataTable.MyList.list[n];
        text[0].text = data.name;
        // image[0].texture = data.icon;

        image[0].enabled = n == selector.itemIndex.Value;
        selector.itemIndex
            .Zip(selector.itemIndex.Skip(1), (x, y) => new System.Tuple<int, int>(x, y))
            .Subscribe(value => {
                // Debug.Log(n + ": " + value.Item1 + "->" + value.Item2);
                // image[0].enabled = n != value;
                image[0].enabled = n == value.Item2;
                if (n == value.Item2) {
                    transform.SetAsLastSibling(); // 最前面に移動
                    image[0].rectTransform.DOAnchorPosX((value.Item1 - value.Item2) * image[0].rectTransform.rect.width, 0);
                    image[0].rectTransform.DOAnchorPosX(0, DataTable.Param.duration);
                }
            })
            .AddTo(gameObject);

        // ボタンタップ時の挙動
        button[0].OnClickAsObservable()
            .Subscribe(b => {
                Debug.Log(text[0].text + ": " + b);
                selector.itemIndex.Value = n;
            })
            .AddTo(gameObject);
    }

    public void UpdateTutorial(CommonList selector, int n)
    {
        var data = DataTable.Tutorial.list[n];
        image[0].texture = data.image;

        // ボタンタップ時の挙動
        button[0].OnClickAsObservable()
            .Subscribe(b => {
                if (n == 3) {
                    selector.transform.parent.gameObject.SetActive(false);
                    SaveName.TutorialDone.SetBool(true);
                }
            })
            .AddTo(gameObject);
    }

    public void UpdateDot(CommonList selector, int n)
    {
        var data = DataTable.Tutorial.list[n];
        selector.itemIndex
            .Subscribe(value => {
                svgImage[0].enabled = n != value;
                svgImage[1].enabled = n == value;
            })
            .AddTo(gameObject);
    }

    public void UpdateNail(CommonList selector, int n)
    {
        var data = DataTable.Nail.list[n];
        text[0].text = "No." + (n + 1).ToString();
        text[1].text = data.name;
        if (data.materials.Length > 0) {
            image[0].color = data.materials[0].baseColor;
        }

        // ボタンタップ時の挙動
        var nailSelector = selector.GetComponent<NailSelectList>();
        button[0].OnClickAsObservable()
            .Subscribe(b => {
                foreach (Transform t in nailSelector.main.nailDetection.transform) {
                    t.GetComponent<NailGroup>().UpdateData(DataTable.Nail.list[n]);
                }
                // Debug.Log("Click: " + data.name);
            })
            .AddTo(gameObject);
    }
}
