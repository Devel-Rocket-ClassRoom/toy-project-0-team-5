using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody rb;
    private float knockbackForce;

    private void Awake() => rb = GetComponent<Rigidbody>();

    public void Fire(Vector3 velocity, float knockbackForce)
    {
        this.knockbackForce = knockbackForce;
        rb.linearVelocity = velocity;
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerHealth>(out var health))
        {
            health.TakeDamage(damage);
            ApplyKnockback(collision.rigidbody);
        }

        // Enemy 레이어는 Physics Matrix에서 충돌 제외 → 여기까지 오면 플레이어나 벽
        Destroy(gameObject);
    }

    private void ApplyKnockback(Rigidbody targetRb)
    {
        if (targetRb == null || knockbackForce <= 0f) return;

        Vector3 dir = (targetRb.transform.position - transform.position).normalized;
        targetRb.AddForce(dir * knockbackForce, ForceMode.Impulse);
    }
}
