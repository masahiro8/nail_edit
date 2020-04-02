using UnityEngine;

[System.Serializable]
public class NailMaterialRecord
{
    public string materialName = "Mirror";
    public NailTextureType textureType = NailTextureType.None;
    public Color baseColor = Vector4.one;
    public Color subColor = Vector4.one;
    public float minSize = 0.1f;
    public float maxSize = 0.1f;
    public int randomSeed = 12345;
    public int randomCount = 32;
}
