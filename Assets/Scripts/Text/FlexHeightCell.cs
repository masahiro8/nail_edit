using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexHeightCell : MonoBehaviour
{

    public Text txt;
    private RectTransform txtRect;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        txtRect = txt.gameObject.GetComponent<RectTransform>();
        var h = txtRect.rect.size.y + 28.0f + 12.0f;
        var rect = gameObject.GetComponent<RectTransform>();
        rect.sizeDelta= new Vector2(rect.sizeDelta.x, h);
    }
}
