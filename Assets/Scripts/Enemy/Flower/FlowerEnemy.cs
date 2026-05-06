using UnityEngine;

// 고정형 몬스터. 이동하지 않으며 플레이어가 탐지 범위에 들어오면 총알을 발사한다.
//
// BT 우선순위:
//   1. 사망     : 체력 <= 0 → Die()
//   2. 피격경직 : 피격 직후 경직 (넉백 없음 — Rigidbody position 고정)
//   3. 공격     : 탐지 범위 내 + 쿨다운 완료 → 총알 발사
//   4. 대기     : 그 외 아무것도 하지 않음
public class FlowerEnemy : EnemyBase
{
    [SerializeField] private float _rotationSpeed = 8f;

    private Animator _animator;
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int TakeDamageHash = Animator.StringToHash("TakeDamage");

    private bool isAttacking;
    private float attackTimer;
    private float hitStunTimer;

    protected override void Awake()
    {
        base.Awake();
        _animator = GetComponentInChildren<Animator>();
        if (Rb != null)
            Rb.constraints = RigidbodyConstraints.FreezePosition
                           | RigidbodyConstraints.FreezeRotationX
                           | RigidbodyConstraints.FreezeRotationZ;
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
                .AddChild(new ConditionNode(ShouldAttack))
                .AddChild(new ActionNode(AttackAction)))
            .AddChild(new ActionNode(() => { Attacker?.TickCooldown(); return NodeState.Running; }));
    }

    private bool ShouldAttack()
    {
        if (isAttacking) return true;
        if (Attacker == null || PlayerTransform == null || !Attacker.IsReady) return false;
        return Vector3.Distance(transform.position, PlayerTransform.position) <= SearchRange;
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

    private NodeState AttackAction()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = Attacker.AttackDuration;
            _animator?.SetTrigger(AttackHash);
            Attack();
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return NodeState.Running;

        isAttacking = false;
        Attacker.StartCooldown();
        return NodeState.Success;
    }
}
