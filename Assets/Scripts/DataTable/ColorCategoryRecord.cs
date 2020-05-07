using UnityEngine;
using UniRx;

[System.Serializable]
public class ColorCategoryRecord
{
    public string name = "ColorCategory"; // フラグ保存に使用するのでリリース後は変更しないこと
    public Color[] colors;
    public bool gloss;
    public string[] keywords;

    public ReactiveProperty<bool> saveFlag = new ReactiveProperty<bool>(false);

    public bool IsShow {
        get { return !saveFlag.Value; }
    }
}
