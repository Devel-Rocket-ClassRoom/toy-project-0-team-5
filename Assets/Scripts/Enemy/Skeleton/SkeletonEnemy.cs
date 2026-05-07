using UnityEngine;

// 근접 공격형 몬스터. 칼 휘두르기는 Physics.OverlapSphere로 전방 범위 판정.
//
// BT 우선순위:
//   1. 사망     : 체력 <= 0 → Die()
//   2. 피격경직 : 피격 직후 경직
//   3. 공격     : 공격 범위 내 + 쿨다운 완료 → 정지 후 OverlapSphere 판정
//   4. 대기     : 공격 범위 내 + 쿨다운 중 → 정지 후 플레이어 방향 주시
//   5. 추적     : 탐지 범위 내 → 플레이어 방향으로 이동 + 시선 추적
//   6. 순찰     : 랜덤 방향으로 이동
public class SkeletonEnemy : EnemyBase
{
    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 10f;

    [Header("Wall Avoidance")]
    [SerializeField] private LayerMask _wallLayerMask;
    [SerializeField] private float _wallCheckDistance = 1.5f;

    [Header("Attack Stop Distance")]
    [SerializeField] private float _chaseStopDistance = 1.5f;

    private Vector3 _patrolDir;
    private float _patrolDirTimer;

    private Animator _animator;
    private static readonly int AttackHash     = Animator.StringToHash("Attack");
    private static readonly int TakeDamageHash = Animator.StringToHash("TakeDamage");
    private static readonly int DieHash        = Animator.StringToHash("Die");
    private static readonly int IsWalkingHash  = Animator.StringToHash("IsWalking");
    private static readonly int IsRunningHash  = Animator.StringToHash("IsRunning");

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
                .AddChild(new ConditionNode(InAttackRange))
                .AddChild(new ActionNode(StandAndFaceAction)))
            .AddChild(new SequenceNode()
                .AddChild(new ConditionNode(InSearchRange))
                .AddChild(new ActionNode(() => { Attacker?.TickCooldown(); ChaseAndFace(); SetRunning(); return NodeState.Running; })))
            .AddChild(new ActionNode(() => { WallAwarePatrol(); SetWalking(); return NodeState.Running; }));
    }

    private bool InAttackRange()
    {
        if (PlayerTransform == null) return false;
        return Vector3.Distance(transform.position, PlayerTransform.position) <= _chaseStopDistance;
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

    // 공격 범위 내에 있지만 쿨다운 중 → 정지 + 플레이어 주시
    private NodeState StandAndFaceAction()
    {
        Attacker?.TickCooldown();
        Rb.linearVelocity = Vector3.zero;
        if (PlayerTransform != null)
        {
            Vector3 dir = PlayerTransform.position - transform.position;
            dir.y = 0f;
            FaceDirection(dir.normalized);
        }
        SetStopped();
        return NodeState.Running;
    }

    private void WallAwarePatrol()
    {
        _patrolDirTimer -= Time.deltaTime;

        bool wallAhead = _patrolDir != Vector3.zero
                      && Physics.Raycast(transform.position, _patrolDir, _wallCheckDistance, _wallLayerMask);

        if (_patrolDirTimer <= 0f || wallAhead)
        {
            _patrolDir = PickSafePatrolDirection();
            _patrolDirTimer = patrolDirectionInterval;
        }

        Rb.linearVelocity = _patrolDir * MoveSpeed * patrolSpeedMultiplier;
        FaceDirection(_patrolDir);
    }

    // 수평 ray만 사용하므로 바닥(수평면)은 감지 안 됨 (Environment 레이어 바닥+벽 공유 대응)
    private Vector3 PickSafePatrolDirection()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 candidate = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            if (!Physics.Raycast(transform.position, candidate, _wallCheckDistance, _wallLayerMask))
                return candidate;
        }
        return -_patrolDir;
    }

    public override void Chase()
    {
        if (PlayerTransform == null) return;

        Vector3 dir = (PlayerTransform.position - transform.position);
        dir.y = 0f;
        dir.Normalize();

        // 플레이어 방향에 벽이 있으면 벽면을 따라 슬라이딩
        if (Physics.Raycast(transform.position, dir, _wallCheckDistance, _wallLayerMask))
        {
            Vector3 right = Vector3.Cross(Vector3.up, dir);
            if (!Physics.Raycast(transform.position, right, _wallCheckDistance, _wallLayerMask))
                dir = right;
            else if (!Physics.Raycast(transform.position, -right, _wallCheckDistance, _wallLayerMask))
                dir = -right;
            else
                dir = Vector3.zero;
        }

        Rb.linearVelocity = dir * MoveSpeed;
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

    private void SetWalking()  { _animator?.SetBool(IsWalkingHash, true);  _animator?.SetBool(IsRunningHash, false); }
    private void SetRunning()  { _animator?.SetBool(IsWalkingHash, false); _animator?.SetBool(IsRunningHash, true);  }
    private void SetStopped()  { _animator?.SetBool(IsWalkingHash, false); _animator?.SetBool(IsRunningHash, false); }

    private void FaceDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.01f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _rotationSpeed);
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
            SetStopped();
            _animator?.SetTrigger(TakeDamageHash);
            Hit();
        }

        hitStunTimer -= Time.deltaTime;
        if (hitStunTimer > 0f) return NodeState.Running;

        isHit = false;
        hitStunTimer = 0f;
        return NodeState.Success;
    }

    // 공격 시 정지 후 OverlapSphere 판정 → attackDuration 동안 Running 유지
    private NodeState AttackAction()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = Attacker.AttackDuration;
            Rb.linearVelocity = Vector3.zero;
            SetStopped();
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
