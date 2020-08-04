using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
 
public class SyncTransform : MonoBehaviour {
 
    public RectTransform targetTransform;
    public float top = 0;
    private RectTransform myTransform;
 
    void Start() {
        myTransform = GetComponent<RectTransform>();
    }
 
    void Update() {
        myTransform.anchoredPosition = new Vector2(
            targetTransform.anchoredPosition.x,
            targetTransform.anchoredPosition.y + this.top
        );
    }
}