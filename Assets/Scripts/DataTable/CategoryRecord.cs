using UnityEngine;
using UniRx;

[System.Serializable]
public class CategoryRecord
{
    [Multiline] public string name = "Category";
    public bool show = true;
    // public CategoryType type = CategoryType.SoulfulColor;
    public int categoryId = 0;

    public ReactiveProperty<bool> saveFlag = new ReactiveProperty<bool>(false);
    public ReactiveProperty<NailInfoRecord[]> filter = new ReactiveProperty<NailInfoRecord[]>(new NailInfoRecord[]{});

    // 表示リストの更新
    public void UpdateShowList()
    {

    }
}
