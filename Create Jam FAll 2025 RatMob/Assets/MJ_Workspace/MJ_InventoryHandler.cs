using System.Collections.Generic;
using UnityEngine;

public class MJ_InventoryHandler : MonoBehaviour
{
    //Used to reference the inventory globally
    public static MJ_InventoryHandler instance; void Awake() { instance = this; }

    //carriedItems by the player
    public Dictionary<ItemType, int> carriedItems = new Dictionary<ItemType, int>();

    public int GetResourceAmount(ItemType itemType) => carriedItems[itemType];

    public void ClearInventory()
    {
        carriedItems.Clear();
    }

    public bool RemoveItem(ItemType itemType, int amount)
    {
        bool canRemove = carriedItems.ContainsKey(itemType);
        if(canRemove)
            if(carriedItems[itemType] >= amount)
                carriedItems[itemType] -= amount;
            else
                return false;
        return canRemove;
    }
}
