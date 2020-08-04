using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class CategoryTable
{
    public CategoryRecord[] list;

    [System.NonSerialized] public CategoryRecord[] showList;

    List<CategoryRecord> data = new List<CategoryRecord>();

    public CategoryRecord GetCategory(int categoryId, string categoryName)
    {
        var res = data.Where(value => value.categoryId == categoryId).ToArray();
        if (res.Length > 0) {
            return res[0];
        }
        var res2 = new CategoryRecord();
        res2.name = categoryName;
        res2.categoryId = categoryId;
        res2.show = categoryId < DataTable.Param.hideCategoryId;
        data.Add(res2);
        return res2;
    }

    // 表示用インデックスの作成
    public void Reset()
    {
        data.Sort((v1, v2) => v1.categoryId - v2.categoryId);
        list = data.ToArray();
        // Debug.Log("CategoryTable.list.Length: " + list.Length);

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
                .Subscribe(_ => data.UpdateShowList(DataTable.Param.filterType.Value))
                .AddTo(DataTable.Instance.gameObject);

            // フィルター変化時
            DataTable.Param.filterType
                .SkipLatestValueOnSubscribe()
                .Subscribe(type => data.UpdateShowList(type))
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
