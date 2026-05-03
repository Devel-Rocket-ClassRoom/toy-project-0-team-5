using UnityEngine;

/// <summary>
/// Player의 Stat를 관리하는 컴포넌트<br/>
/// Modifier로 변동되는 Stats 정의
/// </summary>
[RequireComponent(typeof(PlayerActions))]
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerSO _stats;
    [SerializeField] private float _MaxSpeed = 10f;

    public float Damage => _stats.Damage;
    public float MoveSpeed => Mathf.Min(_stats.Speed, _MaxSpeed); // 상한

    private PlayerActions _playerActions;

    private void Awake()
    {
        _playerActions = GetComponent<PlayerActions>();
    }
}

public enum PlayerStatType
{
    Damage,
    MoveSpeed,
}