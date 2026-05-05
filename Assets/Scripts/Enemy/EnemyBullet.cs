using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    public void Fire(Vector3 velocity)
    {
        rb.linearVelocity = velocity;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy")) return;

        if (other.TryGetComponent<PlayerHealth>(out var health))
            health.TakeDamage(damage);

        Destroy(gameObject);
    }
}
