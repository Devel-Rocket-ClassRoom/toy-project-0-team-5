using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected int maxHp = 3;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float hitStunDuration = 0.2f;
    [SerializeField] protected float knockbackForce = 5f;
    [SerializeField] protected float searchRange = 10f;
    [SerializeField] protected float patrolDirectionInterval = 2f;
    [SerializeField] protected float patrolSpeedMultiplier = 0.5f;

    public int CurrentHp { get; private set; }
    public bool IsDead => CurrentHp <= 0;
    public Transform PlayerTransform { get; private set; }
    public Rigidbody Rb { get; private set; }
    public EnemyAttackBase Attacker { get; private set; }

    public float MoveSpeed => moveSpeed;
    public float HitStunDuration => hitStunDuration;
    public float KnockbackForce => knockbackForce;
    public float SearchRange => searchRange;

    protected bool isHit;
    private bool isDying;

    private BehaviorNode behaviorTree;
    private Vector3 patrolDirection;
    private float patrolDirectionTimer;

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Attacker = GetComponent<EnemyAttackBase>();
        CurrentHp = maxHp;
        behaviorTree = BuildTree();
    }

    protected virtual void Start()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null) PlayerTransform = player.transform;
    }

    protected virtual void Update()
    {
        behaviorTree?.Evaluate();
    }

    protected abstract BehaviorNode BuildTree();

    public virtual void Chase()
    {
        if (PlayerTransform == null) return;
        Vector3 dir = (PlayerTransform.position - transform.position).normalized;
        Rb.linearVelocity = dir * MoveSpeed;
    }

    public virtual void Patrol()
    {
        patrolDirectionTimer -= Time.deltaTime;
        if (patrolDirectionTimer <= 0f)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            patrolDirection = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            patrolDirectionTimer = patrolDirectionInterval;
        }
        Rb.linearVelocity = patrolDirection * MoveSpeed * patrolSpeedMultiplier;
    }

    public virtual void Attack()
    {
        Attacker?.Execute(transform, PlayerTransform);
    }

    public virtual void Hit()
    {
        if (PlayerTransform == null) return;
        Vector3 dir = (transform.position - PlayerTransform.position).normalized;
        Rb.linearVelocity = Vector3.zero;
        Rb.AddForce(dir * KnockbackForce, ForceMode.Impulse);
    }

    public virtual void Die()
    {
        if (isDying) return;
        isDying = true;
        Rb.linearVelocity = Vector3.zero;
        GameEvents.OnEnemyDead?.Invoke(gameObject);
        Destroy(gameObject, 0.5f);
    }

    public virtual void TakeDamage(int amount)
    {
        if (IsDead || isDying) return;
        CurrentHp -= amount;
        if (!IsDead)
            isHit = true;
    }
}
