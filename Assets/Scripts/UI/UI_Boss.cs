using UnityEngine;
using UnityEngine.UI;

public class UI_Boss : MonoBehaviour
{
    [SerializeField] private Slider bossHpSlider;

    private bool isBossTime;

    private void Update()
    {
        if (isBossTime)
        {

        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        isBossTime = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        isBossTime = false;
    }

}
