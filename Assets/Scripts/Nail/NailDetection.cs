using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NailDetection : MonoBehaviour
{
    public Transform meshFolder;
    public GameObject prefab;
    public MeshRenderer sphereTest;

    // private TextAsset data;

    // // Start is called before the first frame update
    // void Start()
    // {
    //     // JsonUtilityが二次元配列をデコードできないので一時的に手動で分解
    //     data = Resources.Load("sample_result") as TextAsset;
    // }

    public void Invoke(Texture webcam, WebCamDevice device)
    {
        var matches = Regex.Matches(DebugPhoto.Instance.PhotoFileJson,
            @"\[[^\[]*?\]",
            RegexOptions.Singleline);

        // 各爪をモデル化
        for (int i = 0; i < matches.Count; i++) {
            var str = "{\"data\":" + matches[i].Value + "}";
            // print(str);
            var record = JsonUtility.FromJson<ModelRecord>(str);

            // 爪オブジェクトのインスタンス作成
            var obj = i < meshFolder.childCount
                ? meshFolder.GetChild(i).gameObject // すでにある場合はそれを使う
                : Instantiate(prefab, meshFolder);
            var nailGroup = obj.GetComponent<NailGroup>();

            nailGroup.UpdateMesh("Finger-" + i, record.data, webcam);

            // テスト用
            if (sphereTest && nailGroup.transform.childCount > 1) {
                sphereTest.material = nailGroup.transform.GetChild(1).GetComponent<NailObject>().meshRenderer.material;
            }
        }
        // print(modelData);
    }
}

[System.Serializable]
public class ModelRecord
{
    // public ModelRecord2[] segmentations;
    // public int[5][22] segmentations;
    public int[] data;
}
