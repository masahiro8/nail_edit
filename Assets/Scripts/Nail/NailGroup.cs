﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class NailGroup : MonoBehaviour
{
    public GameObject prefab;
    public int orgTexWidth = 257; // 元画像のテクスチャの幅
    public int orgTexHeight = 257; // 元画像のテクスチャの高さ
    public int minX;
    public int maxX;
    public int minY;
    public int maxY;
    public int texWidth; // テクスチャの幅
    public int texHeight; // テクスチャの高さ
    public float cx;
    public float cy;
    public float aspect;
    public Vector3 center;

    public float rz;

    [System.NonSerialized] public NailGroupMesh groupMesh = new NailGroupMesh();
    [System.NonSerialized] public NailGroupTexture groupTexture = new NailGroupTexture();

    private NailMaterialTable nailData;
    private CompositeDisposable disposableBag = new CompositeDisposable();

    // // Start is called before the first frame update
    // void Start()
    // {
    //     SROptions.Current.NailEdgeTransparent
    //         .Subscribe(flag => {
    //             // groupMesh.edgePer1 = flag ? 0.8f : 1.0f;
    //             groupMesh.edgePer1 = flag ? SROptions.Current.NailEdge1TransparentPer : 1f;
    //             groupMesh.edgePer2 = flag ? SROptions.Current.NailEdge2TransparentPer : 1f;
    //         })
    //         .AddTo(gameObject);
    // }

    // 作成
    public void UpdateMesh(string modelName, int[] data, Texture orgTexture)
    {
        // // 一時的に適当に入れておく
        // if (nailData == null) {
        //     // nailData = DataTable.Nail.list[0];
        //     // Debug.Log("No nail data.");
        //     return;
        // }

        name = modelName;
        // texture = tex;
        CalcRect(data);
        // groupTexture.UpdateTexture(this, data, orgTexture); // textureType: 1が未使用なので通さない
        groupMesh.CreateMesh(this, data);

        // 位置調整
        transform.localPosition = center;

        // 表示判定
        // meshRenderer.gameObject.SetActive(!isHighLight || nailData.hasHighLightTexture);
        UpdateMesh2();
    }

    public void UpdateMesh2()
    {
        if (nailData == null) {
            return;
        }

        for (var i = 0; i < nailData.list.Length; i++) {
            NailObject nailObject = null;
            if (i < transform.childCount) {
                // すでにある場合はそれを使う
                nailObject = transform.GetChild(i).GetComponent<NailObject>();
            } else {
                // 新規作成
                nailObject = Instantiate(prefab, transform).GetComponent<NailObject>();
                nailObject.transform.localPosition = Vector3.forward * i * -0.1f;
                // nailObject.gameObject.AddComponent<NormalHelper>();
            }
            nailObject.gameObject.SetActive(true);
            nailObject.UpdateData(this, nailData, nailData.list[i]);
        }

        for (var i = nailData.list.Length; i < transform.childCount; i++) {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // 中心および縦横の計算
    private void CalcRect(int[] data)
    {
        // クリッピング
        minX = data[0];
        maxX = minX;
        minY = data[1];
        maxY = minY;
        for (int i = 2; i < data.Length; i += 2) {
            if (minX > data[i + 0]) {
                minX = data[i + 0];
            }
            if (maxX < data[i + 0]) {
                maxX = data[i + 0];
            }
            if (minY > data[i + 1]) {
                minY = data[i + 1];
            }
            if (maxY < data[i + 1]) {
                maxY = data[i + 1];
            }
            // minX = Mathf.Min(minX, data[i + 0]);
            // maxX = Mathf.Min(maxX, data[i + 0]);
            // minY = Mathf.Min(minY, data[i + 1]);
            // maxY = Mathf.Min(maxY, data[i + 1]);
        }
        texWidth = maxX - minX;
        texHeight = maxY - minY;

        // アスペクト比
        aspect = (float)orgTexWidth / (float)orgTexHeight;

        // 中心
        cx = (float)(maxX + minX) / 2;
        cy = (float)(maxY + minY) / 2;

        center = new Vector3(
            (cx / (float)orgTexWidth - 0.5f) * aspect,
            -(cy / (float)orgTexHeight - 0.5f),
            0);
    }

    // 中心および縦横の計算
    public void ReCalcRect(float[] data)
    {
        // ToDo: あとで入れる
    }

    // テクスチャを更新
    public void UpdateDataFirst(NailInfoRecord data)
    {
        if (nailData == null) {
            UpdateData(data);
        }
    }

    // テクスチャを更新
    public void UpdateData(NailInfoRecord data)
    {
        nailData = Resources.Load<NailMaterialTable>("Data/NailMaterial/" + data.fileName2);
        // var material = Resources.Load<Material>("Materials/" + data.materialName);
        // meshRenderer.material = material;
        // foreach (Transform t in transform) {
        //     t.GetComponent<NailObject>().ResetData(this);
        // }
        UpdateMesh2();

#if UNITY_EDITOR
        disposableBag.Clear();
        if (nailData) {
            // エディタでは編集用に更新させるため
            var disposable = nailData.validateTime
                .Where(t => t > 0)
                .Subscribe(_ => UpdateMaterialAll());
            disposableBag.Add(disposable);

            disposable = DataTable.Param.validateTime
                .Where(t => t > 0)
                .Subscribe(_ => UpdateMaterialAll());
            disposableBag.Add(disposable);
        }
#endif
    }

    // マテリアルを更新
    public void UpdateMaterialAll()
    {
        RenderSettings.reflectionIntensity = nailData.reflectionIntensity;

        // Debug.Log(t);
        foreach (Transform t in transform) {
            var nObj = t.GetComponent<NailObject>();
            var materialData = nObj.materialData;
            materialData.SetMaterial(nObj.meshRenderer, nailData);
            materialData.SetTexture(nObj.meshRenderer, nObj.nailTexture);
        }
    }
}
