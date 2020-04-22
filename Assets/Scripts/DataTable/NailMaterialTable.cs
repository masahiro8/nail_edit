using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/NailMaterialTable", fileName = "NailMaterialTable")]
public class NailMaterialTable : ScriptableObject
{
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
