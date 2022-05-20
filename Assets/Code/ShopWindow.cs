using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public sealed class ShopWindow : MonoBehaviour
{
    public event Action CatalogLoaded;
    public event Action InventoryUpdated;
    public Dictionary<string, CatalogItem> CatalogItems => _catalogItems;

    [SerializeField] private Transform _shopRoot;
    [SerializeField] private ShopItem _shopItemPrefab;

    private const string _STORE = "Main";
    private Dictionary<string, CatalogItem> _catalogItems = new Dictionary<string, CatalogItem>();
    private List<StoreItem> _storeItems;

    private void Start()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), ProcessCatalog, LogError);
    }

    private void ProcessCatalog(GetCatalogItemsResult result)
    {
        for(var i = 0; i < result.Catalog.Count; i++)
        {
            _catalogItems.Add(result.Catalog[i].ItemId, result.Catalog[i]);
        }

        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest {
            StoreId = _STORE
        }, ProcessStore, LogError);

        CatalogLoaded?.Invoke();
    }

    private void ProcessStore(GetStoreItemsResult result)
    {
        _storeItems = result.Store;

        for(var i = _shopRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(_shopRoot.GetChild(i).gameObject);
        }

        for(var i = 0; i < _storeItems.Count; i++)
        {
            var index = i;
            ShopItem currentItem = Instantiate<ShopItem>(_shopItemPrefab);
            currentItem.transform.SetParent(_shopRoot, false);
            string itemTitle = _catalogItems[_storeItems[i].ItemId].DisplayName;
            GetPrice(_storeItems[i].VirtualCurrencyPrices, out string currency, out int price);
            string itemPrice = $"{price} {currency}";
            currentItem.SetData(itemTitle, itemPrice, () => {
                PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest {
                    StoreId = _STORE,
                    ItemId = _storeItems[index].ItemId,
                    Price = price,
                    VirtualCurrency = currency
                }, ProcessPurchasedItem, LogError);
            });
        }
    }

    private void ProcessPurchasedItem(PurchaseItemResult result)
    {
        InventoryUpdated?.Invoke();
    }

    private void GetPrice(Dictionary<string, uint> prices, out string currency, out int value)
    {
        currency = "";
        value = 0;

        foreach(var price in prices)
        {
            currency = price.Key;
            value = (int) price.Value;
        }
    }

    private void LogError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
}
