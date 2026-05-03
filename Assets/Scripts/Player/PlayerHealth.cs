using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHp = 6;
    private int _currentHp;
    private PlayerAnimator playerAnimator;

    public int MaxHp => _maxHp;
    public int CurrentHp => _currentHp;


    private void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();

        SetMaxHp(_maxHp, true);
    }

    public void SetMaxHp(int newMax, bool healFull = false)
    {
        _maxHp = newMax;
        GameEvents.OnPlayerMaxHpChanged?.Invoke(_maxHp);

        if (healFull)
        {
            _currentHp = _maxHp;
            GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHp -= damage;

        if (_currentHp <= 0)
        {
            _currentHp = 0;
            playerAnimator.PlayDie();
            Die();
        }

        playerAnimator.PlayDamage();
        GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);
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
