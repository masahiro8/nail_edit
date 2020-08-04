using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BtnNvi : MonoBehaviour
{
    public DotsView dotView;
    public GameObject btnNext;
    public GameObject btnStart;

    private int pageLen;

    void Start()
    {
        this.pageLen = DataTable.Tutorial.list.Length;
        this.showButton(0);

        //indexの変更を受け取る
        dotView.OnIndexChanged.Subscribe(index =>
        {
            this.showButton(index);
        });
    }

    void Update()
    {
        
    }

    private void showButton (int index ) {
        if(this.pageLen - 1 == index) {
            btnNext.SetActive(false);
            btnStart.SetActive(true);
        }else{
            btnNext.SetActive(true);
            btnStart.SetActive(false);
        }
    }
}
