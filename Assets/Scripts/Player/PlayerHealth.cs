using System;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHp { get; private set; } = 6;
    public int CurrentHp { get; private set; }

    private PlayerAnimator playerAnimator;

    public event Action<int> OnHpChanged;
    public event Action<int> OnMaxHpChanged;

    private void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        CurrentHp = MaxHp;
    }

    public void SetMaxHp(int newMax, bool healFull = false)
    {
        MaxHp = newMax;
        OnMaxHpChanged?.Invoke(MaxHp);

        if (healFull)
        {
            CurrentHp = MaxHp;
            OnHpChanged?.Invoke(CurrentHp);
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHp -= damage;

        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            playerAnimator.PlayDie();
            Die();
        }
        playerAnimator.PlayDamage();

        OnHpChanged?.Invoke(CurrentHp);
    }

    private void Die()
    {
        Debug.Log("Die");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            TakeDamage(1);
        }
    }
}
