using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public partial class NailObject : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    private ReactiveProperty<NailMaterialRecord> materialData = new ReactiveProperty<NailMaterialRecord>(null);

    // Start is called before the first frame update
    void Start()
    {
        materialData
            .Subscribe(data => {
                Debug.Log(data);
                ResetData(data);
            })
            .AddTo(gameObject);
    }

    // テクスチャを更新
    public void ResetData(NailMaterialRecord data)
    {
        var material = Resources.Load<Material>("Materials/" + data.materialName);
        meshRenderer.material = material;
        meshRenderer.material.SetColor(GlobalParam.colorName, data.baseColor);
        if (data.subColor == Color.clear) {
            meshRenderer.material.SetColor(GlobalParam.subColorName, data.baseColor);
        } else {
            meshRenderer.material.SetColor(GlobalParam.subColorName, data.subColor);
        }

        switch (data.textureType) {
            case NailTextureType.None:
            case NailTextureType.Light:
                break;
            default:
                seed = data.randomSeed;
                count = data.randomCount;
                UpdateTexture(data);
                meshRenderer.material.SetTexture(GlobalParam.textureName, patternTexture);
                meshRenderer.material.SetTexture(GlobalParam.normalTextureName, normalTexture);
                break;
        }
    }

    // テクスチャを更新
    public void UpdateData(NailGroup group, NailMaterialRecord data)
    {
        materialData.Value = data;
        meshFilter.mesh = group.mesh;
        name = data.materialName;
        if (data.textureType == NailTextureType.Light) {
            // meshRenderer.material.mainTexture = group.lightTexture;
            meshRenderer.material.SetTexture(GlobalParam.textureName, group.lightTexture);
        }
    }
}
