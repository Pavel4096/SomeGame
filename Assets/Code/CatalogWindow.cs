using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public sealed class CatalogWindow : MonoBehaviour
{
    [SerializeField] private Transform _catalogWindowRoot;
    [SerializeField] private GameObject _itemPrefab;
    private List<CatalogItem> _items;
    private const string _catalogName = "Main";

    private void Start()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest {
            CatalogVersion = _catalogName
        }, CatalogReceived, CatalogReceiveFail);
    }

    private void CatalogReceived(GetCatalogItemsResult result)
    {
        _items = result.Catalog;

        for(var i = 0; i < _catalogWindowRoot.childCount; i++)
        {
            Transform currentItem = _catalogWindowRoot.GetChild(i).parent;
            Object.Destroy(currentItem);
        }

        AddItemsFromCatalog(_catalogWindowRoot, _items);
    }

    private void CatalogReceiveFail(PlayFabError error)
    {
        Debug.Log($"{error.GenerateErrorReport()}");
    }

    private void AddItemsFromCatalog(Transform catalogWindowRoot, List<CatalogItem> items)
    {
        Item itemPrefab = _itemPrefab.GetComponent<Item>();
        for(var i = 0; i < items.Count; i++)
        {
            CatalogItem currentItem = items[i];

            Item currentItemObject = Object.Instantiate<Item>(itemPrefab);
            currentItemObject.transform.SetParent(catalogWindowRoot, false);
            currentItemObject.SetData(currentItem.DisplayName, GetPrice(currentItem.VirtualCurrencyPrices));
        }
    }

    private string GetPrice(Dictionary<string, uint> prices)
    {
        foreach(var price in prices)
        {
            return $"{price.Value} {price.Key}";
        }

        return "";
    }
}
