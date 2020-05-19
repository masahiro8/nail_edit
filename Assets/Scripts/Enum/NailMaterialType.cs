using System;
using UnityEngine;

public enum NailMaterialType
{
    Base,
    Lame,
    LameFlat,
}

public static partial class EnumExtensions
{
    // アイテムタイプの取得
    public static string GetFileType(this NailMaterialType type)
    {
        switch (type) {
            case NailMaterialType.Base:
                return "NailBase";
            case NailMaterialType.Lame:
                return "NailLame";
            case NailMaterialType.LameFlat:
                return "NailLame";
            default:
                return "";
        }
    }
}
