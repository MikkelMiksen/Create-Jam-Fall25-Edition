using UnityEngine;

public interface Iinteractable
{
    public void Interact();
    public string GetPrompt();
    
}

public interface IPickupAble
{
    public object PickUp();
}
