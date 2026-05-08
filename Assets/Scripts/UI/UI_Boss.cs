using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Boss : MonoBehaviour
{
    [SerializeField] private Slider bossHpSlider;
    [SerializeField] private TMP_Text bossName;

    private EnemyBase enemyBase;
    private void Start()
    {
        GameEvents.OnBossRoomEnter += HandleBossRoomEnter;
        GameEvents.OnBossHpChanged += HandleBossHpChanged;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameEvents.OnBossRoomEnter -= HandleBossRoomEnter;
        GameEvents.OnBossHpChanged -= HandleBossHpChanged;
    }

    private void HandleBossRoomEnter(EnemyBase boss)
    {
        bossHpSlider.maxValue = boss.MaxHp;
        bossHpSlider.value = boss.CurrentHp;
        bossName.text = boss.BossName;
        gameObject.SetActive(true);
    }

    private void HandleBossHpChanged(int currentHp)
    {
        bossHpSlider.value = currentHp;
    }

}
