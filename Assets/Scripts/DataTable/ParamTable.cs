using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/ParamTable", fileName = "ParamTable")]
public class ParamTable : ScriptableObject
{
    public string colorName = "_Color";
    public string subColorName = "_SubColor";
    public string textureName = "_Texture";
    public string normalTextureName = "_NormalTexture";
    public float duration = 0.3f;
}
