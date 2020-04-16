using UnityEngine;
using SubjectNerd.Utilities;

[System.Serializable]
public class NailRecord
{
    public string name = "Nail-0";
    public CategoryType categoryType = CategoryType.SoulfulColor;
    [Reorderable]
    public NailMaterialRecord[] materials;
}
