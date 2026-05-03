using UnityEngine;

/// <summary>
/// 접촉 중인 플레이어에게 지속적으로 데미지를 주는 컴포넌트.
/// Spike, Fire처럼 닿으면 플레이어가 피해를 받는 Prop에 붙인다.
/// Trigger/Collision 둘 다 지원하므로 콜라이더 설정에 맞게 동작한다.
/// </summary>
public class DamageOnContact : MonoBehaviour
{
    /// <summary>연속 데미지 방지를 위한 최소 간격(초).</summary>
    [SerializeField] private float _damageCooldown = 0.5f;

    private float _lastDamageTime = float.MinValue;

    private void OnTriggerStay(Collider other)
    {
        TryDamage(other.gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        TryDamage(collision.gameObject);
    }

    private void TryDamage(GameObject target)
    {
        if (!target.CompareTag("Player")) return;
        if (Time.time - _lastDamageTime < _damageCooldown) return;

        _lastDamageTime = Time.time;
        GameEvents.OnPlayerHit?.Invoke();
    }
}
