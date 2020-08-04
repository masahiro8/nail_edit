using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/NailMaterialTable", fileName = "NailMaterialTable")]
public class NailMaterialTable : ScriptableObject
{
    [Header("トップコート（クリア）の透明度")]
    public float reflectionIntensity = 1.6f;

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
}
