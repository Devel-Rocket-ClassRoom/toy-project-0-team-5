using UnityEngine;

public class MonstroSpreadAttack : EnemyAttackBase
{
    [SerializeField] private BulletSpawner _spawner;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float bulletRange = 15f;
    [SerializeField] private int bulletCount = 5;
    [SerializeField] private float spreadAngle = 45f;

    public override void Execute(Transform self, Transform target)
    {
        Debug.Log("executing spread attack");
        if (target == null || _spawner == null) return;

        Vector3 diff = target.position - self.position;
        Vector3 baseDir = new Vector3(diff.x, 0f, diff.z).normalized;

        BulletConfig config = new BulletConfig(damage, bulletSpeed, bulletRange / 60, 0f, BulletFlags.NONE);

        float angleStep = bulletCount > 1 ? spreadAngle / (bulletCount - 1) : 0f;
        float startAngle = -(spreadAngle / 2f);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * baseDir;
            _spawner.SpawnBullet(dir, config);
        }
    }
}
