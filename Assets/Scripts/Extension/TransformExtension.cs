using UnityEngine;
using Unity.Mathematics;

public static class TransformExtension
{
    // エディット用のサイズを元に戻す
    public static void RestoreFillBound(this Transform t)
    {
        var panels = t.GetComponentsInChildren<SetCanvasBounds>();
        foreach (var panel in panels) {
            var rectTransform = panel.GetComponent<RectTransform>();
            if (rectTransform) {
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
        }
    }
}
