using TMPro;
using UnityEngine;

public class UI_Window_Ingame : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text bombText;
    [SerializeField] private TMP_Text keyText;

    [SerializeField] private PlayerItem playerItem;

    private void Update()
    {
        playerItem.OnItemChanged += Refresh;
        Refresh();
    }

    public void Refresh()
    {
        coinText.text = $"{playerItem.Coin:D2}";
        bombText.text = $"{playerItem.Bomb:D2}";
        keyText.text = $"{playerItem.Key:D2}";
    }
}
