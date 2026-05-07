using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private BulletBase _bulletPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private AudioClip _fireSound;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void SpawnBullet(Vector3 direction, BulletConfig config)
    {
        BulletBase bullet = Instantiate(_bulletPrefab, _spawnPoint.position, _spawnPoint.rotation);
        bullet.Init(_targetLayer, direction, config);

        if (_fireSound != null && _audioSource != null)
            _audioSource.PlayOneShot(_fireSound);
    }
}