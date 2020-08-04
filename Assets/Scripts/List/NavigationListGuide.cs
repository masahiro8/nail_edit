using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class NavigationListGuide : MonoBehaviour
{
    public Button BtnNext;
    public GameObject Win;
    public GameObject Balloon;
    public Text BallonText;

    public List<GameObject> targets;
    public List<string> texts;
    public List<GameObject> balloons;
    public List<Sprite> BalloonImages;

    private int counter = 0;
    private string[] btnTexts = {"Next","Close"};

    // private Rect lastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        // エディット用のサイズを元に戻す
        transform.RestoreFillBound();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.setBalloon();
        BtnNext.OnClickAsObservable()
            .Subscribe( _=>{
                if (counter < targets.Count) {
                    this.setBalloon();
                } else {
                    this.StartApp();
                    SaveName.NavigationGuideDone.SetBool(true);
                }
            })
            .AddTo(this.gameObject);
    }

    private void setBalloon (){
        Rect safeArea = Screen.safeArea;

        RectTransform rect = targets[counter].GetComponent<RectTransform>() ;
        Win.GetComponent<RectTransform>().anchoredPosition = rect.anchoredPosition;
        Win.GetComponent<RectTransform>().anchorMax = rect.anchorMax;
        Win.GetComponent<RectTransform>().anchorMin = rect.anchorMin;
        Win.GetComponent<RectTransform>().offsetMax = rect.offsetMax;
        Win.GetComponent<RectTransform>().offsetMin = rect.offsetMin;
        Win.GetComponent<RectTransform>().sizeDelta = rect.sizeDelta;

        RectTransform rect2 = balloons[counter].GetComponent<RectTransform>() ;            
        Balloon.GetComponent<RectTransform>().anchoredPosition = rect2.anchoredPosition;
        Balloon.GetComponent<RectTransform>().anchorMax = rect2.anchorMax;
        Balloon.GetComponent<RectTransform>().anchorMin = rect2.anchorMin;
        Balloon.GetComponent<RectTransform>().offsetMax = rect2.offsetMax;
        Balloon.GetComponent<RectTransform>().offsetMin = rect2.offsetMin;
        Balloon.GetComponent<RectTransform>().sizeDelta = rect2.sizeDelta;
        
        Debug.Log("safeArea pos.y" + safeArea.position.y + " size.y" + safeArea.size.y);

        // if (safeArea != lastSafeArea){
        //     Win.GetComponent<RectTransform>().anchorMax = safeArea.position + safeArea.size;
        //     Win.GetComponent<RectTransform>().anchorMin = safeArea.position;
        //     Balloon.GetComponent<RectTransform>().anchorMax = safeArea.position + safeArea.size;
        //     Balloon.GetComponent<RectTransform>().anchorMin = safeArea.position;
        //     lastSafeArea = safeArea;
        // }

        Balloon.GetComponent<SVGImage>().sprite = BalloonImages[counter];

        BallonText.text = texts[counter];
        BtnNext.gameObject.transform.GetChild(0).GetComponent<Text>().text = btnTexts[counter]; 
        counter++;
    }

    private void StartApp()
    {
        Destroy(gameObject);
    }
}
