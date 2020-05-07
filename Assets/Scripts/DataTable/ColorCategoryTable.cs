using System.Linq;
using UnityEngine;
using UniRx;
// using SubjectNerd.Utilities;

[CreateAssetMenu(menuName = "Data Tables/ColorCategoryTable", fileName = "ColorCategoryTable")]
public class ColorCategoryTable : ScriptableObject
{
    // [Reorderable]
    public ColorCategoryRecord[] list;

    public void Reset()
    {
        foreach (var data in list) {
            var key = data.name;

            // フラグ更新時処理
            data.saveFlag.Value = SaveName.ColorHidden.GetBool(key);
            data.saveFlag
                .SkipLatestValueOnSubscribe()
                .Subscribe(flag => {
                    SaveName.ColorHidden.SetBool(key, flag);
                    // Debug.Log("ColorHidden" + key + ":" + flag);
                })
                .AddTo(DataTable.Instance.gameObject);
        }
    }

    // 色番からレコードを取得
    public ColorCategoryRecord GetRecordFromNumber(string colorNumber)
    {
        var colorCategoryId = colorNumber.Length >= 2 ? colorNumber.Substring(0, 2) : "";
        var res = list
            .Where(v => v.keywords.Contains(colorCategoryId))
            .ToArray();
        return res.Length > 0 ? res[0] : list[0];
    }
}
