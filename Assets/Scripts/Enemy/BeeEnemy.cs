using UnityEngine;

// Bee 비행 몬스터 BT 우선순위:
//   1. 사망     : 체력 <= 0 → Die()
//   2. 피격경직 : 피격 직후 경직
//   3. 추적/공전: 한 번 탐색 범위 진입 후 영구 활성
//      - 공전 모드(_orbitCenter != null): Bee King 주위를 일정 반경으로 공전
//      - 추적 모드: 지정된 타겟(기본 플레이어) XZ 추적 (벽 슬라이딩)
//   4. 순찰     : 랜덤 이동 (벽 감지 시 방향 재선택)
//
// 데미지는 BT가 아닌 DamageOnContact 컴포넌트가 OnTriggerStay로 자동 처리한다.
// Y축 고정으로 소환 위치의 비행 높이를 유지한다.
public class BeeEnemy : EnemyBase
{
    [Header("Wall Avoidance")]
    [SerializeField] private LayerMask _wallLayerMask;
    [SerializeField] private float _wallCheckDistance = 2f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 10f;

    private Animator _animator;
    private static readonly int HitId = Animator.StringToHash("hit");
    private static readonly int DieId = Animator.StringToHash("die");

    private Vector3 _patrolDir;
    private float _patrolDirTimer;
    private float hitStunTimer;
    private bool _isAggro;
    private bool _dieAnimTriggered;

    // 추적 모드
    private Transform _chaseTarget;
    private Transform ChaseTarget => _chaseTarget != null ? _chaseTarget : PlayerTransform;

    // 공전 모드 (Bee King 주위)
    private Transform _orbitCenter;
    private float _orbitRadius;
    private float _orbitAngle;

    // BeeKingEnemy 소환 직후: 일정 반경 공전
    public void ForceOrbit(Transform center, float radius)
    {
        _isAggro = true;
        _orbitCenter = center;
        _orbitRadius = radius;
        _orbitAngle = Random.Range(0f, 360f);
        _chaseTarget = null;
    }

    // BeeKingEnemy 릴리즈 시: 지정 타겟(플레이어) 직접 추적
    public void ForceChase(Transform target)
    {
        _isAggro = true;
        _orbitCenter = null;
        _chaseTarget = target;
    }

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
        Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }

    protected override void Update()
    {
        base.Update();
        UpdateFacing();
    }

    private void UpdateFacing()
    {
        if (IsDead) return;
        Vector3 targetDir;
        if (_isAggro)
        {
            if (_orbitCenter != null)
            {
                // 공전 이동 방향(접선)을 바라봄 — outward × up = CCW 접선
                Vector3 outward = transform.position - _orbitCenter.position;
                outward.y = 0f;
                targetDir = Vector3.Cross(outward.normalized, Vector3.up);
            }
            else if (ChaseTarget != null)
                targetDir = ChaseTarget.position - transform.position;
            else
                targetDir = _patrolDir;
        }
        else
        {
            targetDir = _patrolDir;
        }
        targetDir.y = 0f;
        if (targetDir.sqrMagnitude < 0.01f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * _rotationSpeed);
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
                .AddChild(new ConditionNode(ShouldChase))
                .AddChild(new ActionNode(() => { ChaseXZ(); return NodeState.Running; })))
            .AddChild(new ActionNode(() => { WallAwarePatrol(); return NodeState.Running; }));
    }

    private void ChaseXZ()
    {
        // 공전 중이라도 플레이어가 절반 탐지 범위 안에 들어오면 직접 추적으로 전환
        if (_orbitCenter != null && PlayerTransform != null &&
            Vector3.Distance(transform.position, PlayerTransform.position) <= SearchRange * 0.5f)
        {
            ForceChase(PlayerTransform);
        }

        if (_orbitCenter != null) { OrbitAround(); return; }

        if (ChaseTarget == null) return;
        Vector3 dir = ChaseTarget.position - transform.position;
        dir.y = 0f;
        dir.Normalize();

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

    private void OrbitAround()
    {
        _orbitAngle += MoveSpeed / _orbitRadius * Mathf.Rad2Deg * Time.deltaTime;

        float rad = _orbitAngle * Mathf.Deg2Rad;
        Vector3 targetPos = _orbitCenter.position + new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * _orbitRadius;

        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;

        if (dir.magnitude < 0.15f)
        {
            Rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 moveDir = dir.normalized;
        if (Physics.Raycast(transform.position, moveDir, _wallCheckDistance, _wallLayerMask))
        {
            Vector3 right = Vector3.Cross(Vector3.up, moveDir);
            if (!Physics.Raycast(transform.position, right, _wallCheckDistance, _wallLayerMask))
                Rb.linearVelocity = right * MoveSpeed;
            else if (!Physics.Raycast(transform.position, -right, _wallCheckDistance, _wallLayerMask))
                Rb.linearVelocity = -right * MoveSpeed;
            else
                Rb.linearVelocity = Vector3.zero;
            return;
        }

        Rb.linearVelocity = moveDir * MoveSpeed;
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
    }

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

    private bool ShouldChase()
    {
        if (!_isAggro && PlayerTransform != null &&
            Vector3.Distance(transform.position, PlayerTransform.position) <= SearchRange)
            _isAggro = true;
        return _isAggro;
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

    private NodeState HitStunAction()
    {
        if (hitStunTimer <= 0f)
        {
            hitStunTimer = HitStunDuration;
            if (_animator != null) _animator.SetTrigger(HitId);
            Hit();
        }
        hitStunTimer -= Time.deltaTime;
        if (hitStunTimer > 0f) return NodeState.Running;
        isHit = false;
        hitStunTimer = 0f;
        return NodeState.Success;
    }
}
