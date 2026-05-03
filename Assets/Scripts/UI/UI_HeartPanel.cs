using UnityEngine;
using System.Collections.Generic;

public class UI_HeartPanel : MonoBehaviour
{
    [SerializeField] private UI_HeartSlot heartPrefab;
    [SerializeField] private Transform parent;
    [SerializeField] private Player player;

    private List<UI_HeartSlot> hearts = new List<UI_HeartSlot>();

    private void Start()
    {
        player.Health.OnMaxHpChanged += HandleMaxHpChanged;
        player.Health.OnHpChanged += UpdateHearts;

        Init(player.Health.MaxHp);
        UpdateHearts(player.Health.CurrentHp);

    }

    private void HandleMaxHpChanged(int newMax)
    {
        Init(newMax);
        UpdateHearts(player.Health.CurrentHp);
    }

    private void OnDestroy()
    {
        if (player.Health != null)
        {
            player.Health.OnHpChanged -= UpdateHearts;
            player.Health.OnMaxHpChanged -= HandleMaxHpChanged;
        }
    }

    public void Init(int maxHealth)
    {
        foreach (var heart in hearts)
        {
            Destroy(heart.gameObject);
        }
        hearts.Clear();

        int heartCount = maxHealth / 2;

        for (int i = 0; i < heartCount; i++)
        {
            UI_HeartSlot slot = Instantiate(heartPrefab, parent);
            hearts.Add(slot);
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            int value = Mathf.Clamp(currentHealth - (i * 2), 0, 2);
            hearts[i].SetState(value);
        }
    }
}
