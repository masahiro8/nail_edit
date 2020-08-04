using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//TODO 
// 1.最大３行
// 2.文章が切れる時は末尾に...
// 3.文章の高さに合わせてセル自体の高さを変える

public class TableText : MonoBehaviour
{
    public string value;

    void Update()
    {
        this.GetComponent<Text>().SetTextWithEllipsis(value);
    }
}
