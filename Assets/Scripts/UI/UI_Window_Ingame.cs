using TMPro;
using UnityEngine;

public class UI_Window_Ingame : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text bombText;
    [SerializeField] private TMP_Text keyText;

    [SerializeField] private PlayerConsumableItem playerConsumableItem;

    private void Update()
    {
        playerConsumableItem.OnItemChanged += Refresh;
        Refresh();
    }

    public void Refresh()
    {
        coinText.text = $"{playerConsumableItem.CoinCount:D2}";
        bombText.text = $"{playerConsumableItem.BombCount:D2}";
        keyText.text = $"{playerConsumableItem.KeyCount:D2}";
    }
}
