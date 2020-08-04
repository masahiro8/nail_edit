using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class BtnStatus : MonoBehaviour
{
    public GameObject Normal;
    public GameObject Selected;
    public bool select = false;

    // Start is called before the first frame update
    void Start()
    {
        // this.onSelect(this.select);
    }

    public void onSelect(bool b){
        if( b ) {
            Normal.SetActive(false);
            Selected.SetActive(true);
        } else {
            Normal.SetActive(true);
            Selected.SetActive(false);
        }

    }
}
