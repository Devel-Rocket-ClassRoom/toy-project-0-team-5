using UnityEngine;

public class SkeletonAttack : EnemyAttackBase
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float _attackRadius = 1.2f;
    [SerializeField] private float _attackOffset = 0.8f;
    [SerializeField] private LayerMask _playerLayerMask;

    public override void Execute(Transform self, Transform target)
    {
        Vector3 attackCenter = self.position + self.forward * _attackOffset;
        Collider[] hits = Physics.OverlapSphere(attackCenter, _attackRadius, _playerLayerMask);

        foreach (var hit in hits)
        {
            Debug.Log("SkeletonAttack hit: " + hit.name);
            if (hit.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(damage);
        }
    }
}
