using UnityEngine;

/// <summary>
/// Prop 컴포넌트 에디터 테스트용 드라이버.
/// [Space] → _damageTarget에 TakeDamage(1) 호출
/// [F] → _interactTarget 근처에서 OnInteract 강제 발생
/// </summary>
public class PropTestDriver : MonoBehaviour
{
    [SerializeField] private Damageable _damageTarget;
    [SerializeField] private Interactable _interactTarget;

    private void OnEnable()
    {
        GameEvents.OnPlayerHit += OnPlayerHit;

        if (_interactTarget != null)
            _interactTarget.OnInteract += OnInteract;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerHit -= OnPlayerHit;

        if (_interactTarget != null)
            _interactTarget.OnInteract -= OnInteract;
    }

    private void Update()
    {
        // Space → Damageable에 1 데미지
        if (Input.GetKeyDown(KeyCode.Space) && _damageTarget != null)
        {
            Debug.Log("[PropTest] TakeDamage(1) 호출");
            _damageTarget.TakeDamage(1);
        }
    }

    private void OnPlayerHit()
    {
        Debug.Log("[PropTest] OnPlayerHit 발생 — DamageOnContact 작동 확인");
    }

    private void OnInteract(GameObject player)
    {
        Debug.Log($"[PropTest] OnInteract 발생 — 플레이어: {player.name}");
    }
}
