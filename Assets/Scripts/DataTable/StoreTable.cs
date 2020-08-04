using System.Linq;
using UnityEngine;
using UniRx;
// using SubjectNerd.Utilities;

[CreateAssetMenu(menuName = "Data Tables/StoreTable", fileName = "StoreTable")]
public class StoreTable : ScriptableObject
{
    // [Reorderable]
    public StoreRecord[] list;

    [System.NonSerialized] public StoreRecord[] showList;

    public void Reset()
    {
        // 表示用のインデックスリストを作成
        showList = list
            .Where(v => v.show)
            .ToArray();
    }
}
