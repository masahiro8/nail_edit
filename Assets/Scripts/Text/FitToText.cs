using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitToText : MonoBehaviour
{
    public GameObject text;
    private Vector2 localpos;

    // Start is called before the first frame update
    void Start()
    {
        localpos = gameObject.GetComponent<RectTransform>().localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //テキストtransform
        RectTransform tr = text.GetComponent<RectTransform>();
        //自身の座標
        var pos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        gameObject.GetComponent<RectTransform>().anchoredPosition =new Vector2(tr.anchoredPosition.x + tr.rect.width + 24,pos.y);
        
    }
}
