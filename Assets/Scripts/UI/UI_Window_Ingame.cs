using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ActiveItem;

public class UI_Window_Ingame : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text bombText;
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private Image activeIcon;
    [SerializeField] private Slider energySlider;

    [SerializeField] private PlayerConsumableItem playerConsumableItem;
    [SerializeField] private PlayerActions playerActions;

    private void Update()
    {
        playerConsumableItem.OnItemChanged += Refresh;
        playerActions.OnActiveItemChanged += Refresh;
        energySlider.gameObject.SetActive(true);

        Refresh();
    }

    private void OnDestroy()
    {
        playerConsumableItem.OnItemChanged -= Refresh;
        playerActions.OnActiveItemChanged -= Refresh;
        energySlider.value = 0;
    }

    public void Refresh()
    {
        coinText.text = $"{playerConsumableItem.CoinCount:D2}";
        bombText.text = $"{playerConsumableItem.BombCount:D2}";
        keyText.text = $"{playerConsumableItem.KeyCount:D2}";

        ActiveItemBase item = playerActions.EquippedItem;

        if (item != null )
        {
            activeIcon.enabled = true;
            activeIcon.sprite = item.Sprite;

            energySlider.maxValue = item.MaxCharge;
            energySlider.value = item.CurrentCharge;
        }
        else
        {
            activeIcon.enabled = false;
            energySlider.value = 0;
        }
    }
}
