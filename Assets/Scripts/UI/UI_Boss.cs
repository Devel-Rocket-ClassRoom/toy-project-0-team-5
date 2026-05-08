using UnityEngine;
using UnityEngine.UI;

public class UI_Boss : MonoBehaviour
{
    [SerializeField] private Slider bossHpSlider;

    private bool isBossTime;

    private EnemyBase boss;
    public void Show(EnemyBase bossEnemy)
    {
        boss = bossEnemy;
        gameObject.SetActive(true);
        bossHpSlider.maxValue = boss.MaxHp;
        isBossTime = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        isBossTime = false;
    }

    private void Update()
    {
        if (isBossTime)
        {
            bossHpSlider.maxValue = boss.CurrentHp;

        }
    }

}
