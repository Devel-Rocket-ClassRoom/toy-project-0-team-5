using UnityEngine;

public class MonstroSpreadAttack : EnemyAttackBase
{
    [SerializeField] private BulletSpawner _spawner;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float bulletRange = 15f;
    [SerializeField] private int bulletCount = 10;
    [SerializeField] private float spreadAngle = 45f;
    [SerializeField] private float verticalSpread = 25f;

    public override void Execute(Transform self, Transform target)
    {
        if (target == null || _spawner == null) return;

        Vector3 diff = target.position - self.position;
        Vector3 baseDir = new Vector3(diff.x, 0f, diff.z).normalized;

        BulletConfig config = new BulletConfig(damage, bulletSpeed, bulletRange / 60, 0f, BulletFlags.NONE);

        float angleStep = bulletCount > 1 ? spreadAngle / (bulletCount - 1) : 0f;
        float startAngle = -(spreadAngle / 2f);

        for (int i = 0; i < bulletCount; i++)
        {
            float yaw = startAngle + angleStep * i;
            float pitch = Random.Range(-verticalSpread / 2f, verticalSpread / 2f);
            Vector3 dir = Quaternion.Euler(-pitch, yaw, 0f) * baseDir;
            _spawner.SpawnBullet(dir, config);
        }
    }
}
