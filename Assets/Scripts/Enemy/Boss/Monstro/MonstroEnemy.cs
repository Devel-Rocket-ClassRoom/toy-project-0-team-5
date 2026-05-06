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
    [Header("Pattern Settings")]
    [SerializeField] private float patternCooldown = 2f;
    [SerializeField] private float attackRange = 8f;

    [Header("Pattern 1 - Small Jump")]
    [SerializeField] private float smallJumpForce = 7f;
    [SerializeField] private float smallJumpUpForce = 5f;

    [Header("Pattern 2 - Big Jump + Radial")]
    [SerializeField] private float bigJumpUpForce = 10f;
    [SerializeField] private BulletSpawner _radialSpawner;
    [SerializeField] private float radialDamage = 1f;
    [SerializeField] private float radialBulletSpeed = 6f;
    [SerializeField] private float radialBulletRange = 4f;
    [SerializeField] private int radialBulletCount = 8;

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
                bigJumpTargetPos = PlayerTransform.position;
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
        Attack();
        return FinishPattern();
    }

    // 패턴 1: 작은 점프로 플레이어에게 접근 → 착지 시 완료
    private NodeState SmallJumpPattern()
    {
        Debug.Log("small jump pattern executing");
        if (!isJumping)
        {
            isJumping = true;
            jumpAirborneTimer = JumpAirborneDelay;
            floorY = transform.position.y;
            Rb.linearVelocity = Vector3.zero;
            Rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            if (PlayerTransform != null)
            {
                Vector3 horizontal = (PlayerTransform.position - transform.position);
                horizontal.y = 0f;
                Rb.AddForce(horizontal.normalized * smallJumpForce + Vector3.up * smallJumpUpForce, ForceMode.Impulse);
            }
        }

        if (!IsGrounded()) return NodeState.Running;

        SnapToFloor();
        Rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        Rb.linearVelocity = Vector3.zero;
        return FinishPattern();
    }

    // 패턴 2: 플레이어 위치로 큰 점프 → 착지 시 방사형 발사
    private NodeState BigJumpPattern()
    {
        Debug.Log("big jump pattern executing");
        if (!isJumping)
        {
            isJumping = true;
            jumpAirborneTimer = JumpAirborneDelay;
            floorY = transform.position.y;
            Rb.linearVelocity = Vector3.zero;
            Rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

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

    private NodeState DeadAction()
    {
        Die();
        return NodeState.Running;
    }

    private NodeState HitStunAction()
    {
        if (hitStunTimer <= 0f)
        {
            hitStunTimer = HitStunDuration;
            Hit();
        }

        hitStunTimer -= Time.deltaTime;
        if (hitStunTimer > 0f) return NodeState.Running;

        isHit = false;
        hitStunTimer = 0f;
        return NodeState.Success;
    }
}
