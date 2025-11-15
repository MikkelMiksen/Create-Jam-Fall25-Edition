using TMPro;
using UnityEngine;

public class MJ_PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField]
    private float hp = 100f, maxHP = 100f;

    [SerializeField]
    private TextMeshProUGUI hpText;

    private Color[] colors = new Color[3] { Color.cyan, Color.grey, Color.red };


    private string hpDispText => "HP " + (int)hp + " / " + (int)maxHP;
    

    void Update()
    {
        hpText.text = hpDispText;
        if (hp <= 0)
        {
            hpText.text = "player has died";
        }
    }
}
