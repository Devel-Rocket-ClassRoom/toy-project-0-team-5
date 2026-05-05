using UnityEngine;

public class NormalEnemyAttack : EnemyAttackBase
{
    [SerializeField] private EnemyBullet bulletPrefab;
    [SerializeField] private float bulletSpeed = 8f;

    public override void Execute(Transform self, Transform target)
    {
        if (target == null || bulletPrefab == null) return;

        Vector3 diff = target.position - self.position;
        Vector3 dir = new Vector3(diff.x, 0f, diff.z).normalized;
        EnemyBullet bullet = Instantiate(bulletPrefab, self.position, Quaternion.LookRotation(dir));
        bullet.Fire(dir * bulletSpeed);
    }
}
