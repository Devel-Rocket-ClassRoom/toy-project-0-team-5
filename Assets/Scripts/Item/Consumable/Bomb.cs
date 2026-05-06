using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float explodeDelay = 2f;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private SphereCollider explosionCollider;

    private void Start()
    {
        StartCoroutine(CoExplode());
    }

    private IEnumerator CoExplode()
    {
        yield return new WaitForSeconds(explodeDelay);

        Explode();
    }

    private void Explode()
    {
        // colider 반지름 구해서 폭발범위 설정 & 데미지 주기
        float radius =
        explosionCollider.radius *
        explosionCollider.transform.lossyScale.x;

        Collider[] hits = Physics.OverlapSphere(
            explosionCollider.transform.position,
            radius
        );

        foreach (Collider hit in hits)
        {
            IDamageable damageable =
                hit.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(2);
            }
        }


        if (explosionEffect != null)
        {
            ParticleSystem effect = 
                Instantiate(
                    explosionEffect,
                    transform.position,
                    Quaternion.identity
                );

            effect.Play();
            Destroy(effect.gameObject, effect.main.duration );
        }

        Destroy(gameObject);
    }
}
