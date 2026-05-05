using TMPro;
using UnityEngine;

public class UI_Window_Ingame : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text bombText;
    [SerializeField] private TMP_Text keyText;

    private int coinCount;
    private int bombCount;
    private int keyCount;

    public void AddCoin(int amount)
    {
        coinCount += amount;
        Refresh();
    }

    public void AddBomb(int amount)
    {
        bombCount += amount;
        Refresh();
    }

    public void AddKey(int amount)
    {
        keyCount += amount;
        Refresh();
    }

    public void Refresh()
    {
        coinText.text = coinCount.ToString();
        bombText.text = bombCount.ToString();
        keyText.text = keyCount.ToString();
    }
}
