using System.Linq;
using UnityEngine;
using UniRx;

public class CategoryRecord
{
    [Multiline] public string name = "Category";
    public bool show = true;
    // public CategoryType type = CategoryType.SoulfulColor;
    public int categoryId = 0;

    public ReactiveProperty<bool> saveFlag = new ReactiveProperty<bool>(false);
    public ReactiveProperty<NailInfoRecord[]> filter = new ReactiveProperty<NailInfoRecord[]>(new NailInfoRecord[]{});

    // 表示リストの更新
    public void UpdateShowList(NailFilterType fType)
    {
        var b = !SROptions.Current.ColorSelectFilterAndRefresh;
        filter.Value = DataTable.NailInfo.list
            // カテゴリーでフィルター
            .Where(v => v.category == this
                && v.IsDisp()
                // さらに色カテゴリでフィルター
                && (b || v.colorCategory.IsShow))
            .ToArray();
        // Debug.Log("CategoryTable." + data.name + ": " + data.filter.Value.Length);
    }

    // 表示リストの更新
    public bool ContainsNail(NailInfoRecord nailData)
    {
        return filter.Value.Where(v => v == nailData).Count() > 0;
    }
}
