using UnityEngine;

public class BatAttack : EnemyAttackBase
{
    [SerializeField] private BulletSpawner _spawner;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float bulletRange = 12f;

    public override void Execute(Transform self, Transform target)
    {
        if (target == null || _spawner == null) return;

        Vector3 diff = target.position - self.position;
        Vector3 dir = new Vector3(diff.x, 0f, diff.z).normalized;

        BulletConfig config = new BulletConfig(damage, bulletSpeed, bulletRange, 0f, BulletFlags.NONE);
        _spawner.SpawnBullet(dir, config);
    }
}
