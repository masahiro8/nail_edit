using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using SuperScrollView;

public partial class MenuMain : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject itemPrefab;

    private void CreateMenu()
    {
        // // スクロールバーのアイテム
        // for (var i = 0; i < DataTable.Menu.list.Length; i++) {
        //     var obj = Instantiate(itemPrefab, scrollRect.content);
        //     var item = obj.GetComponent<MenuMainItem>();
        //     item.UpdateContext(this, i);
        // }

        var listView = scrollRect.GetComponent<LoopListView2>();
        listView.InitListView(
            DataTable.Menu.list.Length,
            OnGetItemByIndex);
    }

    LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
    {
        if (index < 0 || index >= DataTable.Menu.list.Length)
        {
            return null;
        }

        var itemData = DataTable.Menu.list[index];
        if (itemData == null)
        {
            return null;
        }
        LoopListViewItem2 item = listView.NewListViewItem("MainMenuItem");
        var itemScript = item.GetComponent<MenuMainItem>();
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            // itemScript.Init();
        }

        // itemScript.SetItemData(itemData, index);
        itemScript.UpdateContext(this, index);
        return item;
    }
}
