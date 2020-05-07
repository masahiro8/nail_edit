using System.Linq;
using UnityEngine;
using UniRx;

[CreateAssetMenu(menuName = "Data Tables/CategoryTable", fileName = "CategoryTable")]
public class CategoryTable : ScriptableObject
{
    public CategoryRecord[] list;

    [System.NonSerialized] public CategoryRecord[] showList;

    // 表示用インデックスの作成
    public void Reset()
    {
        // 表示用のインデックスリストを作成
        showList = list
            .Where(v => v.show)
            .ToArray();

        foreach (var data in list) {
            // 非表示フラグの変更
            var key = data.name;
            data.saveFlag.Value = SaveName.CategoryHidden.GetBool(key);
            data.saveFlag
                .SkipLatestValueOnSubscribe()
                .Subscribe(flag => {
                    SaveName.CategoryHidden.SetBool(key, flag);
                    // Debug.Log("CategoryHidden" + key + ":" + flag);
                })
                .AddTo(DataTable.Instance.gameObject);

            // シリーズごとのネイルのインデックスリストを作成
            data.saveFlag
                .Subscribe(b => {
                    data.filter.Value = DataTable.NailInfo.list
                        .Where(v => v.category == data && v.colorCategory.IsShow)
                        .ToArray();
                    // Debug.Log("CategoryTable." + data.name + ": " + data.filter.Value.Length);
                })
                .AddTo(DataTable.Instance.gameObject);
        }

        foreach (var colorData in DataTable.ColorCategory.list) {
            colorData.saveFlag
                .SkipLatestValueOnSubscribe()
                .Subscribe(b => {
                    foreach (var data in list) {
                        data.filter.Value = DataTable.NailInfo.list
                            .Where(v => v.category == data && v.colorCategory.IsShow)
                            .ToArray();
                        // Debug.Log("CategoryTable." + data.name + ": " + data.filter.Value.Length);
                    }
                })
                .AddTo(DataTable.Instance.gameObject);
        }
    }
}
