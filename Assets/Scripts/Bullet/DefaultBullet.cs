using UnityEngine;

public class DefaultBullet : IBulletStrategy
{
    [SerializeField] private GameObject _bulletPrefab;

    public void Fire(System.Numerics.Vector3 direction, BulletConfig config)
    {
        throw new System.NotImplementedException();
    }
}