using System;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class NewCharacterWindow : MonoBehaviour
{
    public event Action NewCharacterAdded;

    [SerializeField] private Transform _newCharactersRoot;
    [SerializeField] private NewCharacterItem _newCharacterItemPrefab;
    [SerializeField] private ShopWindow _shopWindow;
    [SerializeField] private TMP_InputField _name;
    [SerializeField] private Button _closeButton;

    private const string _STORE = "Characters";

    private void Awake()
    {
        _shopWindow.CatalogLoaded += UpdateAvailableCharacters;
        _closeButton.onClick.AddListener(() => CloseWindow());
    }

    private void OnDestroy()
    {
        _shopWindow.CatalogLoaded -= UpdateAvailableCharacters;
        _closeButton.onClick.RemoveAllListeners();
    }

    public void OpenWindow()
    {
        gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }

    private void UpdateAvailableCharacters()
    {
        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest {
            StoreId = _STORE
        }, ProcessStoreItems, LogError);
    }

    private void ProcessStoreItems(GetStoreItemsResult result)
    {
        var storeItems = result.Store;
        for(var i = 0; i < _newCharactersRoot.childCount; i++)
        {
            Destroy(_newCharactersRoot.GetChild(i));
        }

        for(var i = 0; i < storeItems.Count; i++)
        {
            int index = i;
            NewCharacterItem currentItem = Instantiate(_newCharacterItemPrefab);
            currentItem.transform.SetParent(_newCharactersRoot, false);
            string name = _shopWindow.CatalogItems[storeItems[i].ItemId].DisplayName;
            string itemId = storeItems[i].ItemId;
            GetPrice(storeItems[i].VirtualCurrencyPrices, out string currency, out int price);
            currentItem.SetData(name, () => {
                PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest {
                    StoreId = _STORE,
                    ItemId = itemId,
                    Price = price,
                    VirtualCurrency = currency
                }, ProcessPurchasedItem, LogError);
            });
        }
    }

    private void ProcessPurchasedItem(PurchaseItemResult result)
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest {
            CharacterName = _name.text,
            ItemId = result.Items[0].ItemId
        }, ProcessNewCharacter, LogError);
    }

    private void ProcessNewCharacter(GrantCharacterToUserResult result)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest {
            CharacterId = result.CharacterId,
            CharacterStatistics = new Dictionary<string, int> {
                ["XP"] = 0
            }
        }, null, LogError);
        NewCharacterAdded?.Invoke();
        gameObject.SetActive(false);
    }

    private void LogError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
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
}
