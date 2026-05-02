using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    private Queue<Bullet> pool = new Queue<Bullet>();

    private Bullet bulletPrefab;

    public void Initialize(Bullet prefab, int size)
    {
        bulletPrefab = prefab;

        CreateBullets(size);
    }

    public Bullet GetBullet()
    {
        if (pool.Count > 0)
        {
            Bullet bullet = pool.Dequeue();
            bullet.gameObject.SetActive(true);
            return bullet;
        }

        Bullet newBullet = Instantiate(bulletPrefab, transform);
        newBullet.SetPool(this);
        return newBullet;
    }

    public void ReturnBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        pool.Enqueue(bullet);
    }

    public void ChangeBullet(Bullet newPrefab, int size)
    {
        ClearPool();

        bulletPrefab = newPrefab;

        CreateBullets(size);
    }

    private void CreateBullets(int size)
    {
        for (int i = 0; i < size; i++)
        {
            Bullet bullet = Instantiate(bulletPrefab, transform);
            bullet.gameObject.SetActive(false);
            bullet.SetPool(this);
            pool.Enqueue(bullet);
        }
    }

    private void ClearPool()
    {
        while (pool.Count > 0)
        {
            Bullet bullet = pool.Dequeue();

            if (bullet != null)
            {
                Destroy(bullet.gameObject);
            }
        }
    }
}
