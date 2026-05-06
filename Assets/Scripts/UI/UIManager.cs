using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UI_Window_Ingame ingameWindowUI;
    [SerializeField] private UI_Window_Pause pauseWindowUI;
    [SerializeField] private UI_Window_Gameover gameoverWindowUI;
    [SerializeField] private UI_Boss bossUI;
    [SerializeField] private InputManager inputManager;

    private bool isPaused;

    private void Start()
    {

        pauseWindowUI.Hide();
        gameoverWindowUI.Hide();
        bossUI.Hide();
    }

    private void Update()
    {
        if (inputManager.PausePressed)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            
            ShowGameOver();
        }
    }


    public void PauseGame()
    {
        isPaused = true;
        pauseWindowUI.Show();

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseWindowUI.Hide();
        gameoverWindowUI.Hide();

        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        isPaused = true;
        gameoverWindowUI.Show();

        Time.timeScale = 0f;
    }

    public void BossTime()
    {
        bossUI.Show();
    }
}
