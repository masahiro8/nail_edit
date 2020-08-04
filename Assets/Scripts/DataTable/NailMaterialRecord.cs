using UnityEngine;
using UnityEditor;
using System;
using UniRx;

[System.Serializable]
public class NailMaterialRecord
{
    // マテリアル用
    // public string materialName = "NailBase";
    public NailMaterialType materialType = NailMaterialType.Base;
    public Color baseColor = Vector4.one;
    [HeaderAttribute("Sub ColorはLameFlatの時はハイライトとして使用")]
    public Color subColor = Vector4.zero;
    public Color shadowColor = Vector4.zero;
    public float normalHeight = 0;
    public float normalSize = 0.7f;
    [Header("NoiseはBaseでのみ有効")]
    public float noise = 0;
    [Header("以下3つはLameFlatで未使用")]
    public float emissionRate = 0;
    public float metallic = 0;
    public float smoothness = 0.8f;

    // [SeparatorAttribute("Separator test")]

    // テクスチャ用
    // [Space(10)]
    [Header("ラメの模様のパターン")]
    public NailTextureType textureType = NailTextureType.None;
    [Header("以下Texture TypeがNoneの時は未使用")]
    public float minSize = 0.1f;
    public float maxSize = 0.1f;
    public int randomSeed = 12345;
    public int randomCount = 32;

    private const string colorName = "_Color";
    private const string subColorName = "_SubColor";
    private const string textureName = "_Texture";
    private const string normalTextureName = "_NormalTexture";

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
        renderer.material = Resources.Load<Material>("Materials/" + materialType.GetFileType());

        renderer.material.SetColor(colorName, baseColor);
        if (subColor.r == 0 && subColor.g == 0 && subColor.b == 0) {
            renderer.material.SetColor(subColorName, baseColor);
        } else {
            renderer.material.SetColor(subColorName, subColor);
        }
        if (shadowColor.r == 0 && shadowColor.g == 0 && shadowColor.b == 0) {
            renderer.material.SetColor("_ShadowColor", baseColor * emissionRate);
        } else {
            renderer.material.SetColor("_ShadowColor", shadowColor);
        }
        renderer.material.SetFloat("_Noise", noise);
        renderer.material.SetFloat("_NormalHeight", normalHeight);
        renderer.material.SetFloat("_NormalSize", normalSize);
        // renderer.material.SetFloat("_EmissionRate", emissionRate);
        // renderer.material.SetFloat("_Metallic", metallic);
        // renderer.material.SetFloat("_Smoothness", smoothness);
        DataTable.Param.topcoatType
            .Subscribe(type => {
                renderer.material.SetFloat("_Metallic", metallic * type.Metallic());
            })
            .AddTo(renderer.gameObject);
        DataTable.Param.topcoatType
            .Subscribe(type => {
                renderer.material.SetFloat("_Smoothness", smoothness * type.Smoothness());
            })
            .AddTo(renderer.gameObject);
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
                    renderer.material.SetTexture(textureName, nailTexture.patternTexture);
                }
                if (nailTexture.normalTexture) {
                    renderer.material.SetTexture(normalTextureName, nailTexture.normalTexture);
                }
                break;
        }
    }
}
