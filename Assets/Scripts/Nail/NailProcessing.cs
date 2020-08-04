#define NAIL_EDIT

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class NailProcessing : MonoBehaviour
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

#if !NAIL_EDIT
    public void Invoke(NailDetection ndTest1, Texture tex)
    {
        // 各爪をモデル化
        for (int i = 0; i < ndTest1.ioDataAll.dot2Area.result.Length; i++) {
            var d2a = ndTest1.ioDataAll.dot2Area.result[i];
            // 爪オブジェクトのインスタンス作成
            var obj = i < meshFolder.childCount
                ? meshFolder.GetChild(i).gameObject // すでにある場合はそれを使う
                : Instantiate(prefab, meshFolder);
            if (!obj.activeSelf) {
                obj.SetActive(true);
            }
            var nailGroup = obj.GetComponent<NailGroup>();
            nailGroup.orgTexWidth = ndTest1.inputWidth;
            nailGroup.orgTexHeight = ndTest1.inputHeight;
            nailGroup.rz = d2a.Rotate;

            if (infoData != null) {
                nailGroup.UpdateDataFirst(infoData);
            }
            nailGroup.UpdateMesh((d2a.thumb ? "Thumb" : "Finger") + "-" + i, d2a.v, tex);

            obj.transform.localRotation = Quaternion.Euler(0, 0, nailGroup.rz);

            // テスト用
            if (sphereTest && nailGroup.transform.childCount > 1) {
                sphereTest.material = nailGroup.transform.GetChild(1).GetComponent<NailObject>().meshRenderer.material;
            }
        }
        // print(modelData);

        for (int i = ndTest1.ioDataAll.dot2Area.result.Length; i < meshFolder.childCount; i++) {
            var obj = meshFolder.GetChild(i).gameObject;
            obj.SetActive(false);
            // obj.transform.position = Vector3.zero;
        }

        // リセット時に通る
        if (tex == null) {
            return;
        }

        // スケール
        var size = new Vector3(tex.width, tex.height, 0);
        var bs = baseScale;
        var webCam = tex as WebCamTexture;
        if (webCam) {
            size = Quaternion.Euler(0, 0, webCam.videoRotationAngle + SROptions.Current.CameraDegreeOffset) * size;
            if (SaveName.NailMeshReverse.GetBool()) {
                bs.x = -bs.x;
            }
        }
        // 画像が横向き（スケールが1以上）の時だけ上下幅を詰めるために拡大
        var scale = Mathf.Max(1, Mathf.Abs(size.x / size.y));
        transform.localScale = bs * scale;
        // transform.localScale = bs * Mathf.Abs(size.x / size.y);
    }

    public void Invoke3(NailDetectionQuant ndTest3, Texture tex)
    {
        // 各爪をモデル化
        for (int i = 0; i < ndTest3.ioDataAll.dot2Area.result.Length; i++) {
            // 爪オブジェクトのインスタンス作成
            var obj = i < meshFolder.childCount
                ? meshFolder.GetChild(i).gameObject // すでにある場合はそれを使う
                : Instantiate(prefab, meshFolder);
            if (!obj.activeSelf) {
                obj.SetActive(true);
            }
            var nailGroup = obj.GetComponent<NailGroup>();
            nailGroup.orgTexWidth = ndTest3.inputWidth;
            nailGroup.orgTexHeight = ndTest3.inputHeight;

            if (infoData != null) {
                nailGroup.UpdateDataFirst(infoData);
            }
            nailGroup.UpdateMesh("Finger-" + i, ndTest3.ioDataAll.dot2Area.result[i].v, tex);

            // テスト用
            if (sphereTest && nailGroup.transform.childCount > 1) {
                sphereTest.material = nailGroup.transform.GetChild(1).GetComponent<NailObject>().meshRenderer.material;
            }
        }
        // print(modelData);

        for (int i = ndTest3.ioDataAll.dot2Area.result.Length; i < meshFolder.childCount; i++) {
            var obj = meshFolder.GetChild(i).gameObject;
            obj.SetActive(false);
            // obj.transform.position = Vector3.zero;
        }

        // スケール
        var size = new Vector3(tex.width, tex.height, 0);
        var bs = baseScale;
        var webCam = tex as WebCamTexture;
        if (webCam) {
            size = Quaternion.Euler(0, 0, webCam.videoRotationAngle + SROptions.Current.CameraDegreeOffset) * size;
            if (SaveName.NailMeshReverse.GetBool()) {
                bs.x = -bs.x;
            }
        }
        // 画像が横向き（スケールが1以上）の時だけ上下幅を詰めるために拡大
        var scale = Mathf.Max(1, Mathf.Abs(size.x / size.y));
        transform.localScale = bs * scale;
        // transform.localScale = bs * Mathf.Abs(size.x / size.y);
    }
#endif

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
            nailGroup.rz = DebugPhoto.Instance.GetRotate(i);

            if (infoData != null) {
                nailGroup.UpdateDataFirst(infoData);
            }
            nailGroup.UpdateMesh("Finger-" + i, record.data, webcam);

            obj.transform.localRotation = Quaternion.Euler(0, 0, nailGroup.rz);

            // テスト用
            if (sphereTest && nailGroup.transform.childCount > 1) {
                sphereTest.material = nailGroup.transform.GetChild(1).GetComponent<NailObject>().meshRenderer.material;
            }
        }
        // print(modelData);
    }

    // public void UpdateInfoRecord()
    // {
    //     foreach (Transform t in transform) {
    //         t.GetComponent<NailGroup>().UpdateMaterialAll();
    //     }
    // }

    public void SetInfoRecord(NailInfoRecord data)
    {
        infoData = data;
        foreach (Transform t in transform) {
            var group = t.GetComponent<NailGroup>();
            if (group) {
                group.UpdateData(data);
                group.UpdateMaterialAll();
            }
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
