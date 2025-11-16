using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTextManager : MonoBehaviour
{
    public TextMeshProUGUI textBox;
    public ItemType itemToCheck;   // Which item decides the text?
    public string textIfOwned;     // What the textbox should say if player has this item
    public string defaultText;     // What to say if player does NOT have it

    void Start()
    {
        UpdateTextbox();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateTextbox();
    }

    private void UpdateTextbox()
    {
        if (textBox == null || MJ_InventoryHandler.Instance == null)
            return;

        int amount = MJ_InventoryHandler.Instance.GetResourceAmount(itemToCheck);

        if (amount > 0)
        {
            textBox.text = textIfOwned.Replace("{amount}", amount.ToString());
        }
        else
        {
            textBox.text = defaultText;
        }
    }
}
