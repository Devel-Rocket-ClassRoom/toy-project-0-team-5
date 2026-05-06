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
                GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);

                playerAnimator.PlayDie();
                Die();
                return;
            }

            playerAnimator.PlayDamage();
            GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);
        }
        GameEvents.OnPlayerHit?.Invoke();
    }

    public bool OnHeal(int amount)
    {
        if (_currentHp >= _maxHp) return false;

        _currentHp = Mathf.Min(_currentHp + amount, _maxHp);
        GameEvents.OnPlayerHpChanged?.Invoke(_currentHp);

        return true;
    }

    private void Die()
    {
        GameEvents.OnPlayerDie?.Invoke();
    }

    public bool SetProtected(bool isProtected)
    {
        if (_isProtected == isProtected) return false;

        _isProtected = isProtected;
        return true;
    }
}
