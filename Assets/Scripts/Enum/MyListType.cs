using System;
using UnityEngine;

public enum MyListType
{
    Favorite,
    Have,
}

public static partial class EnumExtensions
{
    // アイテムタイプの取得
    public static ItemType GetItemType(this MyListType type)
    {
        switch (type) {
            case MyListType.Favorite:
                return ItemType.MyList1;
            case MyListType.Have:
                return ItemType.MyList2;
            default:
                return (ItemType)0;
        }
    }
}
