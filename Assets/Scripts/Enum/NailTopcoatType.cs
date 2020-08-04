using System;
using UnityEngine;

public enum NailTopcoatType
{
    None,

    Clear,
    Mat,
}

public static partial class EnumExtensions
{
    // None判定
    public static bool IsNone(this NailTopcoatType type)
    {
        return type == NailTopcoatType.None;
    }

    // Metallic
    public static float Metallic(this NailTopcoatType type)
    {
        switch (type) {
            case NailTopcoatType.Clear:
                return DataTable.Param.topcoatMetallicPer[1]; // 1.05f;
            case NailTopcoatType.Mat:
                return DataTable.Param.topcoatMetallicPer[2]; // 0.5f;
            case NailTopcoatType.None:
            default:
                return DataTable.Param.topcoatMetallicPer[0]; // 1f;
        }
    }

    // Smoothness
    public static float Smoothness(this NailTopcoatType type)
    {
        switch (type) {
            case NailTopcoatType.Clear:
                return DataTable.Param.topcoatSmoothnessPer[1]; // 1f;
            case NailTopcoatType.Mat:
                return DataTable.Param.topcoatSmoothnessPer[2]; // 0.2f;
            case NailTopcoatType.None:
            default:
                return DataTable.Param.topcoatSmoothnessPer[0]; // 1f;
        }
    }
}
