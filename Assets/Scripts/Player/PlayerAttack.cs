using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform firePoint;

    [SerializeField] private int poolSize = 10;
    [SerializeField] private float attackInterval = 0.3f;
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private float attackSpeed = 5f;

    private float fireTimer;

    private void Awake()
    {
        bulletPool.Initialize(bulletPrefab, poolSize);
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= attackInterval / attackSpeed)
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
            firePoint.forward * bulletSpeed
        );
    }

    public void ChangeBullet(Bullet newBulletPrefab)
    {
        bulletPool.ChangeBullet(newBulletPrefab, poolSize);
    }
}
