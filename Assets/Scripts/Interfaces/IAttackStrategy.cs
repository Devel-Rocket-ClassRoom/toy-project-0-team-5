using UnityEngine;

public interface IAttackStrategy
{
    void Execute(Transform origin, Vector2 direction, BulletConfig config);
}
