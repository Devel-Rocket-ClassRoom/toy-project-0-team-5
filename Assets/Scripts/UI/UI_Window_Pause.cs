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

    private UIManager uiManager;

    private int attackStat;
    private int sttackSpeedStat;
    private int rangeStat;
    private int speedStat;

    public void Init(UIManager manager)
    {
        uiManager = manager;

        resumeButton.onClick.AddListener(uiManager.ResumeGame);
        quiteButton.onClick.AddListener(uiManager.ResumeGame);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
