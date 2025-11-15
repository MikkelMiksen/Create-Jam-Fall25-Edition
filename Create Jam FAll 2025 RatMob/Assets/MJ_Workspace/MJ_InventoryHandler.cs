using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MJ_InventoryHandler : MonoBehaviour
{
    //Used to reference the inventory globally
    public static MJ_InventoryHandler Instance; void Awake() { Instance = this; }

    [SerializeField] private TextMeshProUGUI textMesh;
    public ItemType itemToDisplay;

    //carriedItems by the player
    public Dictionary<ItemType, int> carriedItems = new();

    public int GetResourceAmount(ItemType itemType) => carriedItems.TryGetValue(itemType, out int amount) ? amount : 0;

    public void ClearInventory()
    {
        carriedItems.Clear();
    }



    public bool RemoveItem(ItemType itemType, int amount)
    {
        int totalAvailable = GetResourceAmount(itemType);
        if (totalAvailable < amount)
        {
            Debug.Log($"\u274c Not enough {itemType} to remove {amount}");
            return false;
        }

        carriedItems[itemType] -= amount;
        UpdateCarriedItemsText();
        return true;
    }

    public void AddItem(ItemType itemType, int amount)
    {
        if(carriedItems.ContainsKey(itemType))
            carriedItems[itemType] += amount;
        else
            carriedItems.Add(itemType, amount);
        UpdateCarriedItemsText();
    }

    public void UpdateCarriedItemsText()
    {
        textMesh.text = "";

        foreach (var entry in carriedItems)
        {
            // Skip items with 0 amount
            if (entry.Value <= 0)
                continue;

            textMesh.text += $"• {entry.Key} — {entry.Value}\n";
        }
    }
}
