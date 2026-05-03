using UnityEngine;

/// <summary>
/// Player의 Stat를 관리하는 컴포넌트
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float _damage = 3.5f;
    private float _moveSpeed = 1f;

    public float Damage => _damage;
    public float MoveSpeed => Mathf.Min(2f, _moveSpeed); // 상한
}