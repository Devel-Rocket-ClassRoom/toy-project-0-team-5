using UnityEngine;

// 기본 근접 공격형 적. BT(Behavior Tree) 구조로 행동을 결정한다.
//
// [BT 우선순위 — 위에서부터 순서대로 평가]
//   1. 사망     : 체력 <= 0 → Die()
//   2. 피격경직 : 피격 직후 일정 시간 경직
//   3. 공격     : 공격 범위 안 + 쿨다운 완료 → 총알 발사
//   4. 추적     : 탐색 범위 안 → 플레이어 방향으로 이동
//   5. 순찰     : 위 조건 모두 불만족 → 랜덤 방향 이동 (기본 행동)
//
// FSM과의 차이: 매 프레임 루트(Selector)부터 다시 평가하므로
// 별도의 상태 전이 코드 없이 우선순위만으로 행동이 자동 결정된다.
public class TestEnemy : EnemyBase
{
    private bool isAttacking;
    private float attackTimer;
    private float hitStunTimer;

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
                .AddChild(new ActionNode(() => { Attacker?.TickCooldown(); Chase(); return NodeState.Running; })))
            .AddChild(new ActionNode(() => { Patrol(); return NodeState.Running; }));
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
        Die();
        return NodeState.Running;
    }

    // 피격 시 hitStunDuration 동안 경직. 타이머가 끝나면 isHit을 해제한다.
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

    // 공격 시작 시 총알을 발사하고, attackDuration 동안 Running을 반환해 행동을 점유한다.
    private NodeState AttackAction()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = Attacker.AttackDuration;
            Attack();
        }

        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return NodeState.Running;

        isAttacking = false;
        Attacker.StartCooldown();
        return NodeState.Success;
    }
}
