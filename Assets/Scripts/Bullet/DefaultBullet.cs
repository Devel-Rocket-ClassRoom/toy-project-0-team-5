using UnityEngine;

public class DefaultBullet : IBulletStrategy
{
    [SerializeField] private GameObject _bulletPrefab;

    public void Fire(BulletConfig config)
    {
        throw new System.NotImplementedException();
    }
}