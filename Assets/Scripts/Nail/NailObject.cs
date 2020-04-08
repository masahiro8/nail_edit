﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class NailObject : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public NailTexture nailTexture;

    private ReactiveProperty<NailMaterialRecord> materialData = new ReactiveProperty<NailMaterialRecord>(null);

    // Start is called before the first frame update
    void Start()
    {
        materialData
            .Where(data => data != null)
            .Subscribe(data => {
                // Debug.Log(data);
                // ResetData(data);
                data.SetMaterial(meshRenderer);
                data.SetTexture(meshRenderer, nailTexture);
            })
            .AddTo(gameObject);

#if UNITY_EDITOR
        // エディタでは編集用に更新させるため
        DataTable.Nail.validateTime
            .Where(t => t > 0)
            .Subscribe(_ => {
                // Debug.Log(t);
                var data = materialData.Value;
                data.SetMaterial(meshRenderer);
                data.SetTexture(meshRenderer, nailTexture);
            })
            .AddTo(gameObject);
#endif
    }

    // // テクスチャを更新
    // public void ResetData(NailMaterialRecord data)
    // {

    // }

    // テクスチャを更新
    public void UpdateData(NailGroup group, NailMaterialRecord data)
    {
        materialData.Value = data;
        meshFilter.mesh = group.mesh;
        name = data.materialName;
        if (data.textureType == NailTextureType.Light) {
            // meshRenderer.material.mainTexture = group.lightTexture;
            meshRenderer.material.SetTexture(DataTable.Param.textureName, group.lightTexture);
        }
    }
}
