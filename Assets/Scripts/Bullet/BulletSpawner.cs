using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private DefaultBullet _bulletPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private LayerMask _targetLayer;

    public void SpawnBullet(Vector3 direction, BulletConfig config)
    {
        DefaultBullet bullet = Instantiate(_bulletPrefab, _spawnPoint.position, _spawnPoint.rotation);
        bullet.Init(_targetLayer, direction, config);
        Debug.Log("Bullet Spawned");
    }
}