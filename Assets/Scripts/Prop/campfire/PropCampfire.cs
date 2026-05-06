using System;
using UnityEngine;

/// <summary>
/// 모든 Prop(장애물/오브젝트)의 공통 기반 컴포넌트.
/// 이 컴포넌트 단독으로는 아무 동작도 하지 않으며,
/// Damageable / Destructible / Interactable / DamageOnContact 등을
/// 필요에 따라 조합해서 사용한다.
/// </summary>
public class PropCampfire : PropBase
{
    [SerializeField] private GameObject _fire;

    private Damageable _damageable;


    private void Awake()
    {
        _damageable = GetComponent<Damageable>();
        _damageable.OnHit += Resize;
    }

    private void Start()
    {
        Resize();
    }

    private void Resize()
    {
        // 원래 이렇게 Current 바로 박으면 안되지만... 시간 관계상...
        _fire.transform.localScale = Vector3.one * _damageable.CurrentHp;
    }
}
