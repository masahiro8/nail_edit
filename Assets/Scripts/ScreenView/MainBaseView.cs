using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBaseView : MonoBehaviour
{
    public ViewManager viewManager;

    public Button shutterButton;
    public Button changeCameraButton;
    public Button flashButton;
    public Button colorSelectButton;
    public Button menuButton;
    public Button debugButton;

    void Awake()
    {
        // エディット用のサイズを元に戻す
        transform.RestoreFillBound();
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
    }
}
