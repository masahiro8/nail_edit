using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/NailTable", fileName = "NailTable")]
public class NailTable : ScriptableObject
{
    public NailRecord[] list;
    public ReactiveProperty<float> validateTime = new ReactiveProperty<float>(0);

    public void OnValidate()
    {
        // Debug.Log(Time.time);
        validateTime.Value = Time.time;
    }
}
