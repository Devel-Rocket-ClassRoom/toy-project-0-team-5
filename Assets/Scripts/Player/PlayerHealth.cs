using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHp = 6;
    private int _currentHp;
    private bool _isProtected = false;
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

    public void AddMaxHp(int amount, bool heal = true)
    {
        _maxHp += amount;
        GameEvents.OnPlayerMaxHpChanged?.Invoke(_maxHp);

        if (heal)
        {
            _currentHp += amount;
            GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);
        }
    }

    public void TakeDamage(int amount)
    {
        if (_isProtected)
        {
            _isProtected = false;
        }
        else
        {
            _currentHp -= amount;

            if (_currentHp <= 0)
            {
                _currentHp = 0;
                playerAnimator.PlayDie();
                Die();
            }

            playerAnimator.PlayDamage();
            GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);
        }
        GameEvents.OnPlayerHit?.Invoke();
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
            Debug.Log("Hit");
        }
    }

    public void SetProtected(bool isProtected) => _isProtected = isProtected;
}
