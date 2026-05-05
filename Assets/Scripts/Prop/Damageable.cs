using System;
using UnityEngine;

/// <summary>
/// HP를 가지며 피격 처리를 담당하는 컴포넌트.
/// TakeDamage()를 호출하면 HP가 감소하고, 0 이하가 되면 OnDeath 이벤트를 발생시킨다.
/// Destructible과 함께 사용하면 파괴 가능한 Prop을 만들 수 있다.
/// </summary>
public class Damageable : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHp = 1;

    private int _currentHp;

    /// <summary>HP가 0이 됐을 때 발생. Destructible 등이 구독해서 처리한다.</summary>
    public event Action OnDeath;

    private void Awake()
    {
        _currentHp = _maxHp;
    }

    /// <summary>외부(총알, 폭발 등)에서 호출. 이미 사망 상태면 무시한다.</summary>
    public void TakeDamage(int amount)
    {
        if (_currentHp <= 0) return;

        _currentHp -= amount;

        if (_currentHp <= 0)
        {
            _currentHp = 0;
            OnDeath?.Invoke();
        }
    }
}
