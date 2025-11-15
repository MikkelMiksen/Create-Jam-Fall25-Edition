 using UnityEngine;

public class MJ_Items : MonoBehaviour, Iinteractable
{
    [SerializeField]
    private ItemType itemType;

    [SerializeField]
    private int amount;

    [SerializeField]
    bool destroyOnPickUp = false;

    public void Interact()
    {
        MJ_InventoryHandler.Instance.AddItem(itemType, amount);
        if (destroyOnPickUp)
            Destroy(gameObject);
    }

    public string GetPrompt() => "Press E to pick up";
}
