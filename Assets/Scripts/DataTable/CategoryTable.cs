using System.Linq;
using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/CategoryTable", fileName = "CategoryTable")]
public class CategoryTable : ScriptableObject
{
    public CategoryRecord[] list;

    [System.NonSerialized] public int[] showList;

    // 表示用インデックスの作成
    public void Reset()
    {
        showList = list
            .Select((v, i) => new { Value = v, Index = i })
            .Where(v => v.Value.show)
            .Select(v => v.Index)
            .ToArray();
    }

    // カテゴリタイプの取得
    public CategoryType GetTypeFromString(string str)
    {
        var res = list.Where(value => value.compareText == str).ToArray();
        if (res.Length > 0) {
            return res[0].type;
        } else {
            return CategoryType.None;
        }
    }
}
