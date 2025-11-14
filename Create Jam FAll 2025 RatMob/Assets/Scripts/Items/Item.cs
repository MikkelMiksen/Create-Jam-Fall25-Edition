using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IUsable
{
   [SerializeField] public Image image;
   [SerializeField] public ItemType type;
    
        
    

    public void Use()
    {
        
    }
}


public enum ItemType
{
    // name all items here
    Cheese, Example1, Example2, Example3, Example4, Example5, Example6, Example7, Example8, Example9
}
