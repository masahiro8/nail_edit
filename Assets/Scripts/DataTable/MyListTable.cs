using System;
using System.Linq;
using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/MyListTable", fileName = "MyListTable")]
public class MyListTable : ScriptableObject
{
    public MyListRecord[] list;

    public ReactiveProperty<int[]>[] filterdList = new ReactiveProperty<int[]>[Enum.GetValues(typeof(MyListType)).Length];

    public void Reset(MyListType type)
    {
        if (filterdList[(int)type] == null) {
            filterdList[(int)type] = new ReactiveProperty<int[]>(new int[]{});
        }
        filterdList[(int)type].Value = DataTable.NailInfo.list
            .Select((v, i) => new { Value = v, Index = i })
            .Where(v => SaveName.MyListItem.GetBool(type.ToString() + v.Value.productCode))
            .Select(v => v.Index)
            .ToArray();
    }
}
