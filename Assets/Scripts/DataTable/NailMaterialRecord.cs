using UnityEngine;

[System.Serializable]
public class NailMaterialRecord
{
    // マテリアル用
    public string materialName = "Mirror";
    public Color baseColor = Vector4.one;
    public Color subColor = Vector4.zero;
    public float normalHeight = 0;
    public float noise = 0;
    public float emissionRate = 0;
    public float smoothness = 0.8f;

    // テクスチャ用
    public NailTextureType textureType = NailTextureType.None;
    public float minSize = 0.1f;
    public float maxSize = 0.1f;
    public int randomSeed = 12345;
    public int randomCount = 32;

    // テクスチャが同じかどうかの判定
    public bool IsSameTexture(NailMaterialRecord record)
    {
        return textureType == record.textureType
            && minSize == record.minSize
            && maxSize == record.maxSize
            && randomSeed == record.randomSeed
            && randomCount == record.randomCount;
    }

    // テクスチャ要素のコピー
    public void CopyTexture(NailMaterialRecord record)
    {
        textureType = record.textureType;
        minSize = record.minSize;
        maxSize = record.maxSize;
        randomSeed = record.randomSeed;
        randomCount = record.randomCount;
    }

    public void SetMaterial(Renderer renderer)
    {
        // マテリアル変更
        renderer.material = Resources.Load<Material>("Materials/" + materialName);

        renderer.material.SetColor(DataTable.Param.colorName, baseColor);
        if (subColor.r == 0 && subColor.g == 0 && subColor.b == 0) {
            renderer.material.SetColor(DataTable.Param.subColorName, baseColor);
        } else {
            renderer.material.SetColor(DataTable.Param.subColorName, subColor);
        }
        renderer.material.SetFloat("_Noise", noise);
        renderer.material.SetFloat("_NormalHeight", normalHeight);
        renderer.material.SetFloat("_EmissionRate", emissionRate);
        renderer.material.SetFloat("_Smoothness", smoothness);
    }

    public void SetTexture(Renderer renderer, NailTexture nailTexture)
    {
        switch (textureType) {
            case NailTextureType.None:
            case NailTextureType.Light:
                break;
            default:
                nailTexture.UpdateTexture(this);
                if (nailTexture.patternTexture) {
                    renderer.material.SetTexture(DataTable.Param.textureName, nailTexture.patternTexture);
                }
                if (nailTexture.normalTexture) {
                    renderer.material.SetTexture(DataTable.Param.normalTextureName, nailTexture.normalTexture);
                }
                break;
        }
    }
}
