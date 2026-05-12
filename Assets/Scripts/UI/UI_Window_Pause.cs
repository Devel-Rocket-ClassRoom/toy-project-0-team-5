using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Window_Pause : MonoBehaviour
{
    [Header("[Button]")]
    [SerializeField]
    private Button resumeButton;

    [SerializeField]
    private Button quiteButton;

    [Header("[Text]")]
    [SerializeField]
    private TMP_Text attackStatText;

    [SerializeField]
    private TMP_Text attackSpeedStatText;

    [SerializeField]
    private TMP_Text rangeStatText;

    [SerializeField]
    private TMP_Text SpeedStatText;

    [SerializeField]
    private PlayerStats playerStats;

    [Header("[Passive Items]")]
    [SerializeField]
    private UI_PassiveItemSlot passiveItemSlotPrefab;

    [SerializeField]
    private Transform passiveItemContainer;

    [SerializeField]
    private PlayerPassiveItems playerPassiveItems;

    private UIManager uiManager;

    public void Init(UIManager manager)
    {
        uiManager = manager;

        resumeButton.onClick.AddListener(uiManager.ResumeGame);
        quiteButton.onClick.AddListener(uiManager.ResumeGame);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Refresh();
        RefreshPassiveItems();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Refresh()
    {
        attackStatText.text = $"{playerStats.Damage:F2}";
        attackSpeedStatText.text = $"{playerStats.ShotSpeed:F2}";
        rangeStatText.text = $"{playerStats.Range:F2}";
        SpeedStatText.text = $"{playerStats.MoveSpeed:F2}";
    }

    private void RefreshPassiveItems()
    {
        if (passiveItemContainer == null) return;

        foreach (Transform child in passiveItemContainer)
            Destroy(child.gameObject);

        foreach (var item in playerPassiveItems.Items)
        {
            var slot = Instantiate(passiveItemSlotPrefab, passiveItemContainer);
            slot.Init(item.Sprite);
        }
    }
}
