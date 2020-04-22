using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/ParamTable", fileName = "ParamTable")]
public class ParamTable : ScriptableObject
{
    [SerializeField, HeaderAttribute("共通のアニメーション時間")]
    public float duration = 0.3f;

    [SerializeField, HeaderAttribute("選択バー移動時のアニメーション時間")]
    public float selectedBarDuration = 0.2f;
}
