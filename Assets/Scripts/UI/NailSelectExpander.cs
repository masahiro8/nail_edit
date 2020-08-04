using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;


//Camera
//http://resetoter.cn/UnityDoc/ScriptReference/Camera.html

public class NailSelectExpander : MonoBehaviour
{
    public Button ExpandBtn;
    public Button CloseBtn;

    public GameObject NailSelecterPanel;
    public GameObject ShootPanel;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Camera.main.ScreenToViewportPoint(new Vector3(Screen.width,Screen.height,1)));
        Debug.Log(Screen.width +" /"+Screen.height);
        expand(false);
        
        ExpandBtn.OnClickAsObservable()
            .Subscribe( _=>{
                expand(true);
            })
            .AddTo(this.gameObject);

        CloseBtn.OnClickAsObservable()
            .Subscribe( _=>{
                expand(false);
            })
            .AddTo(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void expand (bool b) {
        if ( b ) {
            ExpandBtn.gameObject.SetActive(false);
            CloseBtn.gameObject.SetActive(true);
            RectTransform transform = NailSelecterPanel.gameObject.GetComponent<RectTransform>();
            transform.DOAnchorPosY(-260, .2f);
        } else {
            ExpandBtn.gameObject.SetActive(true);
            CloseBtn.gameObject.SetActive(false);
            RectTransform transform = NailSelecterPanel.gameObject.GetComponent<RectTransform>();
            transform.DOAnchorPosY(0, .2f);
        }
    }
}
