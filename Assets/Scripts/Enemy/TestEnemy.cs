using UnityEngine;

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
