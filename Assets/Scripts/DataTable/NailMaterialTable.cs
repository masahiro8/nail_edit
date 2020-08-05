using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/NailMaterialTable", fileName = "NailMaterialTable")]
public class NailMaterialTable : ScriptableObject
{
    [Header("トップコート（クリア）の透明度")]
    public float reflectionIntensity = 1.6f;

    [Header("トップコート（クリア）Metallic")]
    public float topcoatClearMetallicPer = 1.05f;

    [Header("トップコート（クリア）Smoothness")]
    public float topcoatClearSmoothnessPer = 1f;

    [Header("トップコート（マット）Metallic")]
    public float topcoatMatMetallicPer = 1.5f;

    [Header("トップコート（マット）Smoothness")]
    public float topcoatMatSmoothnessPer = 0f;

    // 一個ずつにしたのであまり意味がなくなっている
    public NailMaterialRecord[] list;

    // エディット時のパラメータ変化を受け取るため
#if UNITY_EDITOR
    public ReactiveProperty<float> validateTime = new ReactiveProperty<float>(0);

    public void OnValidate()
    {
        // Debug.Log(Time.time);
        validateTime.Value = Time.time;
    }
#endif

    public float Metallic(NailTopcoatType type)
    {
        switch (type) {
            case NailTopcoatType.Clear:
                return topcoatClearMetallicPer;
            case NailTopcoatType.Mat:
                return topcoatMatMetallicPer;
            case NailTopcoatType.None:
            default:
                return 1f;
        }
    }

    public float Smoothness(NailTopcoatType type)
    {
        switch (type) {
            case NailTopcoatType.Clear:
                return topcoatClearSmoothnessPer;
            case NailTopcoatType.Mat:
                return topcoatMatSmoothnessPer;
            case NailTopcoatType.None:
            default:
                return 1f;
        }
    }
}
