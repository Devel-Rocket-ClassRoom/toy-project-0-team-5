using UnityEngine;

// 비행 몬스터. isTrigger 상태로 Prop 등 환경 오브젝트와 물리 충돌 없음.
//
// BT 우선순위:
//   1. 사망     : 체력 <= 0 → Die()
//   2. 피격경직 : 피격 직후 경직
//   3. 공격     : 공격 범위 내 + 쿨다운 완료 → 정지 후 총알 발사
//   4. 추적     : 탐지 범위 내 → 플레이어 방향으로 비행
//   5. 순찰     : 랜덤 방향으로 비행 (벽 감지 시 방향 재선택)
public class BatEnemy : EnemyBase
{
    [Header("Patrol Wall Avoidance")]
    [SerializeField] private LayerMask _wallLayerMask;
    [SerializeField] private float _wallCheckDistance = 2f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 8f;

    private Animator _animator;
    private static readonly int AttackHash    = Animator.StringToHash("Attack");
    private static readonly int TakeDamageHash = Animator.StringToHash("TakeDamage");
    private static readonly int DieHash       = Animator.StringToHash("Die");

    private Vector3 _batPatrolDir;
    private float _batPatrolDirTimer;

    private bool isAttacking;
    private float attackTimer;
    private float hitStunTimer;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponentInChildren<Animator>();
    }

    protected override BehaviorNode BuildTree()
    {
        return new SelectorNode()
            .AddChild(new SequenceNode()
                .AddChild(new ConditionNode(() => IsDead))
                .AddChild(new ActionNode(DeadAction)))
            .AddChild(new SequenceNode()
                .AddChild(new ConditionNode(() => isHit))
                .AddChild(new ActionNode(HitStunAction)))
            .AddChild(new SequenceNode()
                .AddChild(new ConditionNode(ShouldAttack))
                .AddChild(new ActionNode(AttackAction)))
            .AddChild(new SequenceNode()
                .AddChild(new ConditionNode(InSearchRange))
                .AddChild(new ActionNode(() => { Attacker?.TickCooldown(); ChaseAndFace(); return NodeState.Running; })))
            .AddChild(new ActionNode(() => { BatPatrol(); return NodeState.Running; }));
    }

    private void BatPatrol()
    {
        _batPatrolDirTimer -= Time.deltaTime;

        bool wallAhead = _batPatrolDir != Vector3.zero
                      && Physics.Raycast(transform.position, _batPatrolDir, _wallCheckDistance, _wallLayerMask);

        if (_batPatrolDirTimer <= 0f || wallAhead)
        {
            _batPatrolDir = PickSafePatrolDirection();
            _batPatrolDirTimer = patrolDirectionInterval;
        }

        Rb.linearVelocity = _batPatrolDir * MoveSpeed * patrolSpeedMultiplier;
        FaceDirection(_batPatrolDir);
    }

    // 수평 방향으로만 후보를 뽑아 벽 없는 방향을 반환 (Environment 레이어가 바닥+벽 공유이므로 수평 ray는 벽만 감지)
    private Vector3 PickSafePatrolDirection()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 candidate = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            if (!Physics.Raycast(transform.position, candidate, _wallCheckDistance, _wallLayerMask))
                return candidate;
        }
        return -_batPatrolDir;
    }

    private void ChaseAndFace()
    {
        Chase();
        if (PlayerTransform != null)
        {
            Vector3 dir = PlayerTransform.position - transform.position;
            dir.y = 0f;
            FaceDirection(dir.normalized);
        }
    }

    private void FaceDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.01f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotationSpeed);
    }

    private bool InSearchRange()
    {
        if (PlayerTransform == null) return false;
        return Vector3.Distance(transform.position, PlayerTransform.position) <= SearchRange;
    }

    private bool ShouldAttack()
    {
        if (isAttacking) return true;
        if (Attacker == null || PlayerTransform == null || !Attacker.IsReady) return false;
        return Vector3.Distance(transform.position, PlayerTransform.position) <= Attacker.AttackRange;
    }

    private NodeState DeadAction()
    {
        _animator?.SetTrigger(DieHash);
        Die();
        return NodeState.Running;
    }

    private NodeState HitStunAction()
    {
        if (hitStunTimer <= 0f)
        {
            hitStunTimer = HitStunDuration;
            _animator?.SetTrigger(TakeDamageHash);
            Hit();
        }

        hitStunTimer -= Time.deltaTime;
        if (hitStunTimer > 0f) return NodeState.Running;

        isHit = false;
        hitStunTimer = 0f;
        return NodeState.Success;
    }

    // 공격 시 정지 후 발사 → attackDuration 동안 Running 유지
    private NodeState AttackAction()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = Attacker.AttackDuration;
            Rb.linearVelocity = Vector3.zero;
            _animator?.SetTrigger(AttackHash);
            Attack();
        }

        Rb.linearVelocity = Vector3.zero;
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return NodeState.Running;

        isAttacking = false;
        Attacker.StartCooldown();
        return NodeState.Success;
    }
}
