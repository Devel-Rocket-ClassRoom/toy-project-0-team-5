using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DefaultBullet : MonoBehaviour
{
    [SerializeField] private AudioClip _hitSound;

    private LayerMask _targetLayers;
    private Rigidbody _rb;
    private BulletConfig _config;
    private Vector3 _currentDirection;

    private HashSet<Collider> _hitted = new();
    private float _lifeTime;

    public void Init(LayerMask targetLayer, Vector3 direction, BulletConfig config)
    {
        _targetLayers = targetLayer;
        _currentDirection = direction;
        _config = config;
        _lifeTime = config.Range;

        _hitted.Clear();

        _rb = GetComponent<Rigidbody>();
        _rb.linearVelocity = _currentDirection * _config.Speed;
    }

    private void Update()
    {
        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0f)
        {
            _rb.useGravity = true;
        }
    }

    private void FixedUpdate()
    {
        // if (_config.Flags.HasFlag(BulletFlags.HOMING))
        // {
        //     Vector3 toTarget = (target.position - transform.position).normalized;
        //     _currentDirection = Vector3.RotateTowards(_currentDirection, toTarget, turnRate * Time.fixedDeltaTime, 0f);
        //     _rb.linearVelocity = _currentDirection * _config.Speed;
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsTarget(other.gameObject.layer))
        {
            // 중복 방지
            if (_hitted.Contains(other)) return;

            // 데미지
            if (other.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(Mathf.RoundToInt(_config.Damage));
                if (!other.TryGetComponent(out IKnockbackImmune _) && other.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddForce(_currentDirection.normalized * _config.Damage * _config.Speed, ForceMode.Impulse);
                }
            }

            if (_config.Flags.HasFlag(BulletFlags.PIERCING))
            {
                _hitted.Add(other);
            }
            else
            {
                if (_hitSound != null)
                    AudioSource.PlayClipAtPoint(_hitSound, Camera.main.transform.position);
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Destroy(gameObject);
        }
    }

    bool IsTarget(int layer) => (_targetLayers.value & (1 << layer)) != 0;
}