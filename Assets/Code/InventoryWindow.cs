using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public sealed class InventoryWindow : MonoBehaviour
{
    [SerializeField] private Transform _inventoryRoot;
    [SerializeField] private InventoryItem _inventoryItemPrefab;
    [SerializeField] private ShopWindow _shopWindow;

    private void Start()
    {
        _shopWindow.CatalogLoaded += UpdateUserInventory;
        _shopWindow.InventoryUpdated += UpdateUserInventory;
    }

    private void OnDestroy()
    {
        _shopWindow.CatalogLoaded -= UpdateUserInventory;
        _shopWindow.InventoryUpdated -= UpdateUserInventory;
    }

    private void UpdateUserInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), ProcessUserInventory, LogError);
    }

    private void ProcessUserInventory(GetUserInventoryResult result)
    {
        for(var i = _inventoryRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(_inventoryRoot.GetChild(i).gameObject);
        }

        for(var i = 0; i < result.Inventory.Count; i++)
        {
            InventoryItem currentItem = Instantiate<InventoryItem>(_inventoryItemPrefab);
            currentItem.transform.SetParent(_inventoryRoot, false);
            CatalogItem catalogItem = _shopWindow.CatalogItems[result.Inventory[i].ItemId];
            string itemTitle = catalogItem.DisplayName;
            currentItem.SetData(itemTitle, null);
        }
    }

    private void LogError(PlayFabError error)
    {}
}
