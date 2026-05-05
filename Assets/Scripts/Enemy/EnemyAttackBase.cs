using UnityEngine;

public abstract class EnemyAttackBase : MonoBehaviour
{
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float attackDuration = 0.4f;

    public float AttackRange => attackRange;
    public float KnockbackForce => knockbackForce;
    public float AttackDuration => attackDuration;

    private float cooldownTimer;
    public bool IsReady => cooldownTimer <= 0f;

    public void TickCooldown() => cooldownTimer = Mathf.Max(0f, cooldownTimer - Time.deltaTime);

    public void StartCooldown() => cooldownTimer = cooldown;

    public abstract void Execute(Transform self, Transform target);
}
