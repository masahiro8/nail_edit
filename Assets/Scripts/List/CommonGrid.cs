using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SuperScrollView;

public class CommonGrid : MonoBehaviour
{
    public int initialIndex = 0;

    public ReactiveProperty<int> itemIndex = new ReactiveProperty<int>(-1);
    public ReactiveProperty<int> itemCount = new ReactiveProperty<int>(0);
    public System.Action<CommonItem, int> updateItem;

    private string itemName = "CommonItem";
    private ScrollRect scrollRect;
    private LoopGridView gridView;

    void Awake()
    {
        itemIndex.Value = initialIndex;
        scrollRect = GetComponent<ScrollRect>();
        gridView = scrollRect.GetComponent<LoopGridView>();

        if (gridView) {
            // LoopListView2を使う
            gridView.InitGridView(
                itemCount.Value,
                OnGetItemByIndex);
        }

        // 総数変化時にリセット
        itemCount
            .Subscribe(_ => Reset())
            .AddTo(gameObject);

        // プレファブはcontent内にある最初のものを使う
        if (scrollRect.content.childCount > 0) {
            itemName = scrollRect.content.GetChild(0).name;
        }
    }

    private LoopGridViewItem OnGetItemByIndex(LoopGridView gridView, int index, int row,int column)
    {
        if (index < 0 || index >= gridView.ItemTotalCount)
        {
            return null;
        }

        // var itemData = DataTable.Tutorial.list[index];
        // if (itemData == null)
        // {
        //     return null;
        // }
        LoopGridViewItem item = gridView.NewListViewItem(itemName);
        // itemType.SetSize(gridView, item);
        var itemScript = item.GetComponent<CommonItem>();
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            // itemScript.Init();
        }

        // itemScript.SetItemData(itemData, index);
        // itemScript.UpdateContext(this, index);
        itemScript.disposableBag.Clear();
        if (updateItem != null) {
            updateItem(itemScript, index);
        }
        return item;
    }

    public void Reset()
    {
        if (gridView) {
            gridView.SetListItemCount(itemCount.Value);
            gridView.RefreshAllShownItem();
        }
    }
}
