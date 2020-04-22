using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using SuperScrollView;

public class CommonList : MonoBehaviour
{
    public ItemType itemType;
    public int initialIndex = 0;

    public ReactiveProperty<int> itemIndex = new ReactiveProperty<int>(-1);

    private string itemName = "CommonItem";
    private ScrollRect scrollRect;
    private LoopListView2 listView;

    void Awake()
    {
        itemIndex.Value = initialIndex;
        scrollRect = GetComponent<ScrollRect>();
        listView = scrollRect.GetComponent<LoopListView2>();

        if (listView) {
            // LoopListView2を使う
            listView.InitListView(
                itemType.GetCount(),
                OnGetItemByIndex);
        } else {
            // LoopListView2を使わない
            // normalizedPositionが特殊でSnapがうまく動かなくなるため
            var prefab = scrollRect.content.GetChild(0);
            for (var i = 0; i < itemType.GetCount(); i++) {
                var obj = i == 0 ? prefab : Instantiate(prefab, scrollRect.content);
                var item = obj.GetComponent<RectTransform>();
                var itemScript = obj.GetComponent<CommonItem>();
                itemType.SetSize(scrollRect.GetComponent<RectTransform>(), item);
                itemScript.UpdateContext(itemType, this, i);
            }
        }

        // プレファブはcontent内にある最初のものを使う
        if (scrollRect.content.childCount > 0) {
            itemName = scrollRect.content.GetChild(0).name;
        }
    }

    private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
    {
        if (index < 0 || index >= listView.ItemTotalCount)
        {
            return null;
        }

        // var itemData = DataTable.Tutorial.list[index];
        // if (itemData == null)
        // {
        //     return null;
        // }
        LoopListViewItem2 item = listView.NewListViewItem(itemName);
        itemType.SetSize(listView, item);
        var itemScript = item.GetComponent<CommonItem>();
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            // itemScript.Init();
        }

        // itemScript.SetItemData(itemData, index);
        itemScript.UpdateContext(itemType, this, index);
        return item;
    }

    public void Reset()
    {
        if (listView) {
            listView.SetListItemCount(itemType.GetCount());
            listView.RefreshAllShownItem();
        }
    }
}
