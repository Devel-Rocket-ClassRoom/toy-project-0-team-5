using UnityEngine;
using System.Collections.Generic;

public class UI_HeartPanel : MonoBehaviour
{
    [SerializeField] private UI_HeartSlot heartPrefab;
    [SerializeField] private Transform parent;

    private List<UI_HeartSlot> hearts = new List<UI_HeartSlot>();

    private void Awake()
    {
        GameEvents.OnPlayerMaxHpChanged += UpdateMaxHearts;
        GameEvents.OnPlayerHpChanged += UpdateHearts;
    }

    private void OnDestroy()
    {
        GameEvents.OnPlayerMaxHpChanged -= UpdateMaxHearts;
        GameEvents.OnPlayerHpChanged -= UpdateHearts;
    }

    private void UpdateMaxHearts(int maxHealth)
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

    private void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            int value = Mathf.Clamp(currentHealth - (i * 2), 0, 2);
            hearts[i].SetState(value);
        }
    }
}
