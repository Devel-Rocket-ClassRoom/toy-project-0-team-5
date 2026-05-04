using UnityEngine;

public enum PlayerStatType
{
    Damage,
    Delay,
    Range,
    ShotSpeed,
    Speed,
    Luck
}

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Player/StatData")]
public class StatData : ScriptableObject
{
    [Header("초기 시작 값")]

    [Tooltip("1발당 데미지")]
    [SerializeField] private float _damage = 3.5f;

    [Tooltip("프레임당 발사 빈도 (낮을 수록 빠름)")]
    [SerializeField] private float _delay = 16f;

    [Tooltip("거리 (눈물이 살아 있을 시간)")]
    [SerializeField] private float _range = 3f;

    [Tooltip("탄속")]
    [SerializeField] private float _shotSpeed = 1f;

    [Tooltip("이동 속도")]
    [SerializeField] private float _speed = 1f;

    [Tooltip("아이템 확률과 특수효과에 영향")]
    [SerializeField] private float _luck = 0f;

    public float Damage => _damage;
    public float Delay => _delay;
    public float Range => _range;
    public float ShotSpeed => _shotSpeed;
    public float Speed => _speed;
    public float Luck => _luck;
}