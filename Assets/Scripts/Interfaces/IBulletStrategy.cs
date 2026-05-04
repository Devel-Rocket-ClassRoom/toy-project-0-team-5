using System.Numerics;

public interface IBulletStrategy
{
    void Fire(Vector3 direction, BulletConfig config);
}