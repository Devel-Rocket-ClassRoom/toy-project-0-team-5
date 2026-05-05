using UnityEngine;
using UnityEngine.UI;

public class UI_Window_Gameover : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Button exitButton;

    private UIManager uiManager;

    private void Init(UIManager manager)
    {
        uiManager = manager;

        exitButton.onClick.AddListener(uiManager.ShowGameOver);
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

