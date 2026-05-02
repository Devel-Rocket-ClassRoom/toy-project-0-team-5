using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private float delayTime = 0.5f;
    public float damage = 10f;

    private BulletPool pool;
    private Rigidbody rb;
    private Coroutine lifeCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetPool(BulletPool bulletPool)
    {
        pool = bulletPool;
    }

    public void Fire(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        transform.position = position;
        transform.rotation = rotation;

        rb.linearVelocity = velocity;
        rb.useGravity = false;

        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
        }

        lifeCoroutine = StartCoroutine(LifeTimer());
        StartCoroutine(GravityDelay());
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifeTime);

        ReturnToPool();
    }

    private void OnCollisionEnter(Collision collision)
    {
        int otherLayer = collision.gameObject.layer;

        if (otherLayer == LayerMask.NameToLayer("Enemy") ||
            otherLayer == LayerMask.NameToLayer("Ground"))
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (lifeCoroutine != null)
        {
            StopCoroutine(lifeCoroutine);
            lifeCoroutine = null;
        }

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;

        pool.ReturnBullet(this);
    }

    private IEnumerator GravityDelay()
    {
        yield return new WaitForSeconds(delayTime);

        rb.useGravity = true;
    }    





}
