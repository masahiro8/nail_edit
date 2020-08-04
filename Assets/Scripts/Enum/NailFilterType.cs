using System;
using UnityEngine;

public enum NailFilterType
{
    None,

    All,
    New,
    Limited,
    Have,
    Favotite,
}

public static partial class EnumExtensions
{
    // 操作可能
    public static bool IsShow(this NailFilterType type, NailInfoRecord v)
    {
        switch (type) {
            case NailFilterType.New:
                // 情報公開日から発売日の30日後までの商品に、NEWマークをつける。
                return v.IsNew();
            case NailFilterType.Limited:
                return v.IsLimited();
            case NailFilterType.Have:
                return v.IsMyList(MyListType.Have);
            case NailFilterType.Favotite:
                return v.IsMyList(MyListType.Favorite);
            default:
            case NailFilterType.None:
            case NailFilterType.All:
                return true;
        }
    }
}
