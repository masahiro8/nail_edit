using UnityEngine;

[System.Serializable]
public class CategoryRecord
{
    [Multiline] public string name = "Category";
    public bool show = true;
    public CategoryType type = CategoryType.SoulfulColor;
    [Multiline] public string compareText = "";
}
