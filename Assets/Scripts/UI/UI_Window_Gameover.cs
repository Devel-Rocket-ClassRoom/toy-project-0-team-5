using UnityEngine;
using UnityEngine.UI;

public class UI_Window_Gameover : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Button exitButton;

    private UIManager uiManager;

    public void Init(UIManager manager)
    {
        uiManager = manager;

        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(uiManager.RestartScene);
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

