using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Window_Pause : MonoBehaviour
{
[Header("[Button]")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quiteButton;

    [Header("[Text]")]
    [SerializeField] private TMP_Text attackStatText;
    [SerializeField] private TMP_Text attackSpeedStatText;
    [SerializeField] private TMP_Text rangeStatText;
    [SerializeField] private TMP_Text SpeedStatText;

    [SerializeField] private PlayerStats playerStats;

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
        Refesh();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Refesh()
    {
        attackStatText.text = $"{playerStats.Damage:F2}";
        attackSpeedStatText.text = $"{playerStats.ShotSpeed:F2}";
        rangeStatText.text = $"{playerStats.Range:F2}";
        SpeedStatText.text = $"{playerStats.MoveSpeed:F2}";
    }
}
