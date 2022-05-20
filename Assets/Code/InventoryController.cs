using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public sealed class InventoryController
{
    private List<ItemInstance> _inventoryItems;

    public InventoryController()
    {
        UpdateInventory();
    }

    public bool UseInventoryItem(string itemId)
    {
        bool isItemUsed;
        int itemIndex = -1;
        for(var i = 0; i < _inventoryItems.Count; i++)
        {
            if(itemId.Equals(_inventoryItems[i].ItemId))
            {
                itemIndex = i;
                break;
            }
        }

        if(itemIndex != -1)
        {
            PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest {
                ConsumeCount = 1,
                ItemInstanceId = _inventoryItems[itemIndex].ItemInstanceId
            }, ProcessItemUsed, null);
            isItemUsed = true;
        }
        else
            isItemUsed = false;
        
        return isItemUsed;
    }

    private void ProcessItemUsed(ConsumeItemResult result)
    {
        if(result.RemainingUses > 0)
            return;

        for(var i = 0; i < _inventoryItems.Count; i++)
        {
            if(result.ItemInstanceId.Equals(_inventoryItems[i].ItemInstanceId))
            {
                _inventoryItems.RemoveAt(i);
                break;
            }
        }
    }

    private void UpdateInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), ProcessUserInventory, null);
    }

    private void ProcessUserInventory(GetUserInventoryResult result)
    {
        _inventoryItems = result.Inventory;
    }
}
