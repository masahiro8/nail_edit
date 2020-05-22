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

    private TextAsset data;
    private NailInfoRecord infoData = null;
    private Vector3 baseScale;

    // Start is called before the first frame update
    void Start()
    {
        // JsonUtilityが二次元配列をデコードできないので一時的に手動で分解
        data = Resources.Load("sample_result") as TextAsset;
        baseScale = transform.localScale;
    }

    public void Invoke(WebCamTexture webcam, NailDotToArea d2a)
    {
        // 各爪をモデル化
        for (int i = 0; i < d2a.result.Length; i++) {
            // 爪オブジェクトのインスタンス作成
            var obj = i < meshFolder.childCount
                ? meshFolder.GetChild(i).gameObject // すでにある場合はそれを使う
                : Instantiate(prefab, meshFolder);
            var nailGroup = obj.GetComponent<NailGroup>();

            nailGroup.UpdateMesh("Finger-" + i, d2a.result[i].v, webcam);
            nailGroup.UpdateDataFirst(infoData);

            // テスト用
            if (sphereTest && nailGroup.transform.childCount > 1) {
                sphereTest.material = nailGroup.transform.GetChild(1).GetComponent<NailObject>().meshRenderer.material;
            }
        }
        // print(modelData);

        // スケール
        var size = new Vector3(webcam.width, webcam.height, 0);
        size = Quaternion.Euler(0, 0, webcam.videoRotationAngle + SROptions.Current.CameraDegreeOffset) * size;
        transform.localScale = baseScale * Mathf.Abs(size.x / size.y);
    }

    public void Invoke2(Texture webcam)
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
            nailGroup.orgTexWidth = 960;
            nailGroup.orgTexHeight = 1280;

            nailGroup.UpdateMesh("Finger-" + i, record.data, webcam);
            nailGroup.UpdateDataFirst(infoData);

            // テスト用
            if (sphereTest && nailGroup.transform.childCount > 1) {
                sphereTest.material = nailGroup.transform.GetChild(1).GetComponent<NailObject>().meshRenderer.material;
            }
        }
        // print(modelData);
    }

    public void SetInfoRecord(NailInfoRecord data)
    {
        infoData = data;
        foreach (Transform t in transform) {
            t.GetComponent<NailGroup>().UpdateData(data);
        }
    }
}

[System.Serializable]
public class ModelRecord
{
    // public ModelRecord2[] segmentations;
    // public int[5][22] segmentations;
    public int[] data;
}
