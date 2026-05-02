using UnityEditor.EditorTools;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerStats stats;

    [SerializeField] private int poolSize = 10;
    [SerializeField] private float attackInterval = 0.3f;

    private float fireTimer;

    private void Awake()
    {
        GameObject poolObj = new GameObject("BulletPool");
        bulletPool = poolObj.AddComponent<BulletPool>();

        bulletPool.Initialize(bulletPrefab, poolSize);
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= attackInterval / stats.attackSpeed)
        {
            Fire();
            fireTimer = 0;
        }
    }

    private void Fire()
    {
        Bullet bullet = bulletPool.GetBullet();

        bullet.Fire(
            firePoint.position,
            firePoint.rotation,
            firePoint.forward * stats.attackSpeed
        );
    }

    public void ChangeBullet(Bullet newBulletPrefab)
    {
        bulletPool.ChangeBullet(newBulletPrefab, poolSize);
    }
}
