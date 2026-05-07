using UnityEngine;

// Monstro 보스 BT 우선순위:
//   1. 사망     : 체력 <= 0 → Die()
//   2. 피격경직 : 피격 직후 경직
//   3. 패턴 실행: 범위 내 + 쿨다운 완료 → 매번 랜덤 패턴 선택
//      - 패턴 0: 플레이어 방향 분산 발사
//      - 패턴 1: 작은 점프로 플레이어에게 접근
//      - 패턴 2: 플레이어 위치로 큰 점프 강하 → 착지 시 방사형 발사
//   4. 대기     : 쿨다운 카운트다운 후 다음 패턴 대기
public class MonstroEnemy : EnemyBase
{
    [Header("Animation")]
    [SerializeField] private float jumpAnimSpeed = 0.3f;

    [Header("Pattern Settings")]
    [SerializeField] private float patternCooldown = 2f;
    [SerializeField] private float attackRange = 8f;

    [Header("Pattern 1 - Small Jump")]
    [SerializeField] private float smallJumpForce = 7f;
    [SerializeField] private float smallJumpUpForce = 5f;

    [Header("Pattern 2 - Big Jump + Radial")]
    [SerializeField] private float bigJumpUpForce = 10f;
    [SerializeField] private BulletSpawner _radialSpawner;

    [Header("Jump Safety")]
    [SerializeField] private LayerMask _wallLayerMask;
    [SerializeField] private float _landingCheckRadius = 1.5f;

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float radialDamage = 1f;
    [SerializeField] private float radialBulletSpeed = 6f;
    [SerializeField] private float radialBulletRange = 4f;
    [SerializeField] private int radialBulletCount = 8;

    private Animator _animator;
    private static readonly int IsJumping      = Animator.StringToHash("isJumping");
    private static readonly int SpreadAttackId = Animator.StringToHash("spreadAttack");
    private static readonly int HitId          = Animator.StringToHash("hit");
    private static readonly int DieId          = Animator.StringToHash("die");

    private bool _dieAnimTriggered;
    private float patternCooldownTimer;
    private float hitStunTimer;
    private bool isExecutingPattern;
    private int currentPattern;
    private bool isJumping;
    private float jumpAirborneTimer;
    private float floorY;
    private Vector3 bigJumpTargetPos;
    private bool bigJumpLanded;

    // 점프 직후 착지 오판정 방지용 딜레이
    private const float JumpAirborneDelay = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponent<Animator>();
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
                .AddChild(new ConditionNode(() => isHit))
                .AddChild(new ActionNode(HitStunAction)))
            .AddChild(new SequenceNode()
                .AddChild(new ConditionNode(ShouldExecutePattern))
                .AddChild(new ActionNode(PatternAction)))
            .AddChild(new ActionNode(IdleAction));
    }

    private bool ShouldExecutePattern()
    {
        if (isExecutingPattern) return true;
        if (!PatternReady || PlayerTransform == null) return false;
        return Vector3.Distance(transform.position, PlayerTransform.position) <= attackRange;
    }

    private NodeState PatternAction()
    {
        if (!isExecutingPattern)
        {
            isExecutingPattern = true;
            currentPattern = Random.Range(0, 3);
            isJumping = false;
            jumpAirborneTimer = 0f;
            bigJumpLanded = false;

            if (currentPattern == 2)
                bigJumpTargetPos = GetSafeLandingPosition(PlayerTransform.position);
        }

        return currentPattern switch
        {
            0 => SpreadShotPattern(),
            1 => SmallJumpPattern(),
            2 => BigJumpPattern(),
            _ => FinishPattern()
        };
    }

    // 패턴 0: 플레이어 방향으로 분산 발사 (MonstroSpreadAttack 컴포넌트 사용)
    private NodeState SpreadShotPattern()
    {
        Rb.linearVelocity = Vector3.zero;
        if (_animator != null) _animator.SetTrigger(SpreadAttackId);
        Attack();
        return FinishPattern();
    }

    // 패턴 1: 작은 점프로 플레이어에게 접근 → 착지 시 완료
    private NodeState SmallJumpPattern()
    {
        if (!isJumping)
        {
            isJumping = true;
            jumpAirborneTimer = JumpAirborneDelay;
            floorY = transform.position.y;
            Rb.linearVelocity = Vector3.zero;
            Rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            SetJumpAnim(true);

            if (PlayerTransform != null)
            {
                Vector3 horizontal = (PlayerTransform.position - transform.position);
                horizontal.y = 0f;
                Vector3 jumpDir = GetSafeJumpDirection(horizontal.normalized, smallJumpForce, smallJumpUpForce);
                Rb.AddForce(jumpDir * smallJumpForce + Vector3.up * smallJumpUpForce, ForceMode.Impulse);
            }
        }

        if (!IsGrounded()) return NodeState.Running;

        SetJumpAnim(false);
        SnapToFloor();
        Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        Rb.linearVelocity = Vector3.zero;
        return FinishPattern();
    }

    // 패턴 2: 플레이어 위치로 큰 점프 → 착지 시 방사형 발사
    private NodeState BigJumpPattern()
    {
        if (!isJumping)
        {
            isJumping = true;
            jumpAirborneTimer = JumpAirborneDelay;
            floorY = transform.position.y;
            Rb.linearVelocity = Vector3.zero;
            Rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            SetJumpAnim(true);

            // 체공 시간으로 역산한 수평 속도 → 목표 지점에 정확히 착지
            float gravity = -Physics.gravity.y;
            float vUp = bigJumpUpForce / Rb.mass;
            float timeOfFlight = 2f * vUp / gravity;

            Vector3 toTarget = bigJumpTargetPos - transform.position;
            toTarget.y = 0f;
            float vHorizontal = toTarget.magnitude / timeOfFlight;

            Rb.AddForce(toTarget.normalized * vHorizontal * Rb.mass + Vector3.up * bigJumpUpForce, ForceMode.Impulse);
        }

        if (!IsGrounded()) return NodeState.Running;

        if (!bigJumpLanded)
        {
            bigJumpLanded = true;
            SetJumpAnim(false);
            SnapToFloor();
            Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            Rb.linearVelocity = Vector3.zero;
            FireRadial();
        }
        return FinishPattern();
    }

    private void FireRadial()
    {
        if (_radialSpawner == null) return;

        BulletConfig config = new BulletConfig(radialDamage, radialBulletSpeed, radialBulletRange, 0f, BulletFlags.NONE);
        float angleStep = 360f / radialBulletCount;
        for (int i = 0; i < radialBulletCount; i++)
        {
            Vector3 dir = Quaternion.Euler(0f, angleStep * i, 0f) * Vector3.forward;
            _radialSpawner.SpawnBullet(dir, config);
        }
    }

    // isTrigger 상태에서는 바닥 충돌이 없으므로 Raycast 대신 Y 위치로 착지 판정
    // 점프 시작 시 저장한 floorY로 돌아오면 착지로 간주
    private bool IsGrounded()
    {
        if (jumpAirborneTimer > 0f)
        {
            jumpAirborneTimer -= Time.deltaTime;
            return false;
        }

        if (Rb.linearVelocity.y > 0f) return false;

        return transform.position.y <= floorY;
    }

    private void SnapToFloor()
    {
        Rb.MovePosition(new Vector3(transform.position.x, floorY, transform.position.z));
    }

    // 예상 착지 지점이 벽과 겹치면 안전한 방향으로 보정해 반환 (SmallJump용)
    private Vector3 GetSafeJumpDirection(Vector3 direction, float horizontalForce, float upForce)
    {
        float gravity = -Physics.gravity.y;
        float vHorizontal = horizontalForce / Rb.mass;
        float vUp = upForce / Rb.mass;
        float timeOfFlight = 2f * vUp / gravity;

        Vector3 estimatedLanding = transform.position + direction * (vHorizontal * timeOfFlight);
        estimatedLanding.y = transform.position.y;

        Vector3 safeLanding = GetSafeLandingPosition(estimatedLanding);
        Vector3 safeDir = safeLanding - transform.position;
        safeDir.y = 0f;
        return safeDir.magnitude > 0.01f ? safeDir.normalized : direction;
    }

    // 착지 예정 위치가 벽과 겹치면 벽 바깥으로 밀어낸 안전한 좌표를 반환
    private Vector3 GetSafeLandingPosition(Vector3 targetPos)
    {
        // Y를 반경만큼 올려 바닥 콜라이더가 감지되지 않도록 함 (바닥과 벽이 같은 레이어인 경우 대응)
        Vector3 checkPos = new Vector3(targetPos.x, transform.position.y + _landingCheckRadius, targetPos.z);

        for (int i = 0; i < 8; i++)
        {
            Collider[] hits = Physics.OverlapSphere(checkPos, _landingCheckRadius, _wallLayerMask);
            if (hits.Length == 0) break;

            foreach (var hit in hits)
            {
                Vector3 push = checkPos - hit.ClosestPoint(checkPos);
                push.y = 0f;
                float overlap = _landingCheckRadius - push.magnitude;
                if (overlap > 0f)
                    checkPos += (push == Vector3.zero ? (checkPos - hit.transform.position).normalized : push.normalized) * (overlap + 0.05f);
            }
        }

        return checkPos;
    }

    private NodeState IdleAction()
    {
        TickPatternCooldown();
        Rb.linearVelocity = Vector3.zero;
        return NodeState.Running;
    }

    private NodeState FinishPattern()
    {
        isExecutingPattern = false;
        isJumping = false;
        StartPatternCooldown();
        return NodeState.Success;
    }

    private bool PatternReady => patternCooldownTimer <= 0f;
    private void TickPatternCooldown() => patternCooldownTimer = Mathf.Max(0f, patternCooldownTimer - Time.deltaTime);
    private void StartPatternCooldown() => patternCooldownTimer = patternCooldown;

    private void SetJumpAnim(bool jumping)
    {
        if (_animator == null) return;
        _animator.SetBool(IsJumping, jumping);
        _animator.speed = jumping ? jumpAnimSpeed : 1f;
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
