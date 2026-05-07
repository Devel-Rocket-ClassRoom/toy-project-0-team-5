using System.Collections.Generic;
using UnityEngine;

// Bee King 보스 BT 우선순위:
//   1. 사망      : 체력 <= 0 → Die()
//   2. 패턴 실행 : 범위 내 + 쿨다운 완료 → 가중치 랜덤 패턴 선택
//      - 패턴 0  : Bee 다수 소환 → 소환된 Bee들은 Bee King 주위를 공전 (75% 확률)
//      - 패턴 1  : 공전 중인 Bee 전부 플레이어 방향으로 전환 (25% 확률, 최소 4마리 이상일 때만)
//   3. 부유      : 느리게 랜덤 방향으로 이동 (벽 회피)
//
// 피격 시 패턴 중단 없음. TakeDamage에서 hit 애니메이션만 재생.
public class BeeKingEnemy : EnemyBase, IKnockbackImmune
{
    [Header("Pattern Settings")]
    [SerializeField] private float patternCooldown = 3f;
    [SerializeField] private float attackRange = 14f;

    [Header("Summon")]
    [SerializeField] private GameObject _beePrefab;
    [SerializeField] private int _summonCount = 4;
    [SerializeField] private int _maxSummonedBees = 12;
    [SerializeField] private float _summonRadius = 2.5f;

    [Header("Wall Avoidance")]
    [SerializeField] private LayerMask _wallLayerMask;
    [SerializeField] private float _wallCheckDistance = 2f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 8f;

    private Animator _animator;
    private static readonly int HitId    = Animator.StringToHash("hit");
    private static readonly int DieId    = Animator.StringToHash("die");
    private static readonly int AttackId = Animator.StringToHash("attack");

    private readonly List<BeeEnemy> _summonedBees = new();
    private float patternCooldownTimer;
    private bool isExecutingPattern;
    private int currentPattern;
    private bool _dieAnimTriggered;

    private Vector3 _floatDir;
    private float _floatDirTimer;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
        Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        hitStunImmune = true;
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (!IsDead && _animator != null)
            _animator.SetTrigger(HitId);
    }

    protected override void Update()
    {
        base.Update();
        FacePlayer();
    }

    private void FacePlayer()
    {
        if (PlayerTransform == null || IsDead) return;
        Vector3 dir = PlayerTransform.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotationSpeed);
    }

    protected override BehaviorNode BuildTree()
    {
        return new SelectorNode()
            .AddChild(new SequenceNode()
                .AddChild(new ConditionNode(() => IsDead))
                .AddChild(new ActionNode(DeadAction)))
            .AddChild(new SequenceNode()
                .AddChild(new ConditionNode(ShouldExecutePattern))
                .AddChild(new ActionNode(PatternAction)))
            .AddChild(new ActionNode(FloatAction));
    }

    private bool ShouldExecutePattern()
    {
        if (isExecutingPattern) return true;
        if (patternCooldownTimer > 0f || PlayerTransform == null) return false;
        return Vector3.Distance(transform.position, PlayerTransform.position) <= attackRange;
    }

    private NodeState PatternAction()
    {
        if (!isExecutingPattern)
        {
            isExecutingPattern = true;
            CleanDeadBees();

            // 살아있는 bee가 3마리 이하면 무조건 소환 패턴
            // 그 이상이면 75% 소환 / 25% 릴리즈
            if (_summonedBees.Count <= 2)
                currentPattern = 0;
            else
                currentPattern = (Random.Range(0, 3) == 0) ? 1 : 0;
        }

        return currentPattern switch
        {
            0 => SummonBeesPattern(),
            1 => ReleaseBeesPattern(),
            _ => FinishPattern()
        };
    }

    // 패턴 0: Bee King 주위에 Bee 소환 → 공전 시작
    private NodeState SummonBeesPattern()
    {
        if (_beePrefab == null) return FinishPattern();

        int canSummon = Mathf.Min(_summonCount, _maxSummonedBees - _summonedBees.Count);
        if (canSummon <= 0) return FinishPattern();

        for (int i = 0; i < canSummon; i++)
        {
            float angle = (360f / _summonCount) * i * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * _summonRadius;
            Vector3 spawnPos = GetSafeSpawnPosition(transform.position + offset);

            GameObject go = Instantiate(_beePrefab, spawnPos, Quaternion.identity);
            if (go.TryGetComponent<BeeEnemy>(out var bee))
            {
                bee.ForceOrbit(transform, _summonRadius);
                _summonedBees.Add(bee);
            }
        }

        return FinishPattern();
    }

    // 패턴 1: 공전 중인 Bee 전부 플레이어로 전환
    private NodeState ReleaseBeesPattern()
    {
        if (PlayerTransform == null) return FinishPattern();

        if (_animator != null) _animator.SetTrigger(AttackId);

        foreach (var bee in _summonedBees)
        {
            if (bee != null) bee.ForceChase(PlayerTransform);
        }
        _summonedBees.Clear();

        return FinishPattern();
    }

    // 느린 랜덤 부유 (벽 회피)
    private NodeState FloatAction()
    {
        patternCooldownTimer = Mathf.Max(0f, patternCooldownTimer - Time.deltaTime);

        _floatDirTimer -= Time.deltaTime;
        bool wallAhead = _floatDir != Vector3.zero
                      && Physics.Raycast(transform.position, _floatDir, _wallCheckDistance, _wallLayerMask);

        if (_floatDirTimer <= 0f || wallAhead)
        {
            _floatDir = PickSafeDirection();
            _floatDirTimer = patrolDirectionInterval;
        }

        Rb.linearVelocity = _floatDir * MoveSpeed * patrolSpeedMultiplier;
        return NodeState.Running;
    }

    private Vector3 PickSafeDirection()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 candidate = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            if (!Physics.Raycast(transform.position, candidate, _wallCheckDistance, _wallLayerMask))
                return candidate;
        }
        return -_floatDir;
    }

    private Vector3 GetSafeSpawnPosition(Vector3 desiredPos)
    {
        const float checkRadius = 0.4f;
        if (!Physics.CheckSphere(desiredPos, checkRadius, _wallLayerMask))
            return desiredPos;

        for (int i = 0; i < 16; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 candidate = transform.position + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * _summonRadius;
            if (!Physics.CheckSphere(candidate, checkRadius, _wallLayerMask))
                return candidate;
        }
        return transform.position;
    }

    private void CleanDeadBees()
    {
        _summonedBees.RemoveAll(b => b == null);
    }

    private NodeState FinishPattern()
    {
        isExecutingPattern = false;
        patternCooldownTimer = patternCooldown;
        return NodeState.Success;
    }

    public override void Hit()
    {
        Rb.linearVelocity = Vector3.zero;
    }

    private NodeState DeadAction()
    {
        if (!_dieAnimTriggered)
        {
            _dieAnimTriggered = true;
            if (_animator != null) _animator.SetTrigger(DieId);
        }
        Die();
        return NodeState.Running;
    }

}
