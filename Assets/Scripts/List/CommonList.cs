using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SuperScrollView;

public class CommonList : MonoBehaviour
{
    public ItemType itemType;
    public int initialIndex = 0;

    public ReactiveProperty<int> itemIndex = new ReactiveProperty<int>(-1);
    public ReactiveProperty<int> itemCount = new ReactiveProperty<int>(0);
    public System.Action<CommonItem, int> updateItem;
    public System.Func<int, int> itemPrefabIndex;

    private string[] itemNames;
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
                itemCount.Value,
                OnGetItemByIndex);
        }

        // 総数変化時にリセット
        itemCount
            .SkipLatestValueOnSubscribe()
            .Subscribe(_ => Reset())
            .AddTo(gameObject);
            
        // プレファブはcontent内にある最初のものを使う
        itemPrefabIndex = (n) => 0;
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
        LoopListViewItem2 item = listView.NewListViewItem(listView.ItemPrefabDataList[itemPrefabIndex(index)].mItemPrefab.name);
        itemType.SetSize(listView, item);
        var itemScript = item.GetComponent<CommonItem>();
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            // itemScript.Init();
        }

        // itemScript.SetItemData(itemData, index);
        itemScript.UpdateContext(this, index);
        if (updateItem != null) {
            updateItem(itemScript, index);
        }
        return item;
    }

    public void Reset()
    {
        if (listView) {
            listView.SetListItemCount(itemCount.Value);
            listView.RefreshAllShownItem();
        } else if (scrollRect) {
            // LoopListView2を使わない
            // normalizedPositionが特殊でSnapがうまく動かなくなるため
            itemCount.Value = itemCount.Value;
            var prefab = scrollRect.content.GetChild(0);
            for (var i = 0; i < itemCount.Value; i++) {
                var obj = i == 0 ? prefab : Instantiate(prefab, scrollRect.content);
                var item = obj.GetComponent<RectTransform>();
                var itemScript = obj.GetComponent<CommonItem>();
                itemType.SetSize(scrollRect.GetComponent<RectTransform>(), item);
                itemScript.UpdateContext(this, i);
                if (updateItem != null) {
                    updateItem(itemScript, i);
                }
            }
        }
    }
}
