using System;
using UnityEngine;
using SuperScrollView;

public enum ItemType
{
    NailSelect,
    MainMenu,
    MyList1,
    MyListSelect,
    Toturial,
    ToturialDot,
    MyListFrame,
    NailCategory,
    MyList2,
}

public static partial class EnumExtensions
{
    // アイテム数取得
    public static int GetCount(this ItemType type)
    {
        switch (type) {
            case ItemType.NailSelect:
                return DataTable.NailInfo.showList.Length;
            case ItemType.MainMenu:
                return DataTable.Menu.list.Length;
            case ItemType.MyList1:
                return DataTable.MyList.filterdList[(int)MyListType.Favorite].Value.Length;
            case ItemType.MyList2:
                return DataTable.MyList.filterdList[(int)MyListType.Have].Value.Length;
            case ItemType.MyListSelect:
            case ItemType.MyListFrame:
                return DataTable.MyList.list.Length;
            case ItemType.Toturial:
            case ItemType.ToturialDot:
                return DataTable.Tutorial.list.Length;
            case ItemType.NailCategory:
                return DataTable.Category.showList.Length;
            default:
                return 0;
        }
    }

    // セルの大きさを変更
    public static void SetSize(this ItemType type, LoopListView2 listView, LoopListViewItem2 item)
    {
        var r = listView.GetComponent<RectTransform>().rect;
        switch (type) {
            case ItemType.MyListSelect:
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, r.width / listView.ItemTotalCount);
                break;
            case ItemType.Toturial:
            case ItemType.MyListFrame:
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, r.width);
                item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, r.height);
                break;
            default:
                break;
        }
    }

    // セルの大きさを変更
    public static void SetSize(this ItemType type, RectTransform rectTransform, RectTransform item)
    {
        switch (type) {
            case ItemType.Toturial:
            case ItemType.MyListFrame:
                // Debug.Log(type + ": " + rectTransform.rect);
                item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.rect.width);
                item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height);
                break;
            default:
                break;
        }
    }
}
