using UnityEngine;

/// <summary>
/// 파괴 시 이펙트/아이템 드롭을 처리하는 컴포넌트.
/// Damageable.OnDeath를 구독하고 있다가 사망 이벤트가 오면 동작한다.
/// Damageable 없이는 동작하지 않으므로 RequireComponent로 강제한다.
/// </summary>
[RequireComponent(typeof(Damageable))]
public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject _dropPrefab;          // 파괴 시 스폰할 드롭 아이템
    [SerializeField] private GameObject _destroyEffectPrefab; // 파괴 시 재생할 이펙트

    private void Awake()
    {
        GetComponent<Damageable>().OnDeath += OnDestroyed;
    }

    private void OnDestroyed()
    {
        if (_destroyEffectPrefab != null)
            Instantiate(_destroyEffectPrefab, transform.position, Quaternion.identity);

        if (_dropPrefab != null)
            Instantiate(_dropPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
