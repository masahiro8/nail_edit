using System;
using System.Linq;
using UnityEngine;

public enum MainViewType
{
    Movie,
    FixedCamera,
    FixedPreview,
    FixedEdit,
}

public static partial class EnumExtensions
{
    // アイテム数取得
    public static bool IsShowNail(this MainViewType type)
    {
        switch (type) {
            case MainViewType.FixedCamera:
            case MainViewType.FixedPreview:
                return false;
            default:
                return true;
        }
    }
}
