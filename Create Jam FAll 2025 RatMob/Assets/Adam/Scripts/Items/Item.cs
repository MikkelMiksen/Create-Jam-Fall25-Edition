using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, Iinteractable, IPickupAble
{
   [SerializeField] public Image image;
   [SerializeField] public ItemType type;

   public string GetPrompt() => "Press [insert button] to steal";

   public void Interact()
   {
       
   }

   public object PickUp()
   {
       return this;
   }
}

[System.Serializable]
public enum ItemType
{
    // name all items here
    Cheese, Example1, Example2, Example3, Example4, Example5, Example6, Example7, Example8, Example9
}
