using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class SortMainButton : MonoBehaviour
{
    public SortStore Store;
    public Text text;
    
    // Start is called before the first frame update
    void Start()
    {
        //ストアからソートのidの変更を受け取り
        Store.OnLabelChanged.Subscribe(label =>
        {
            Debug.Log("Sort label" + label);
            text.text = label;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
