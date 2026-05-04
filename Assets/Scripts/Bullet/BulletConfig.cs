using UnityEngine;

public enum BulletFlags
{
    Homing = 1 << 0,
    Pierce = 1 << 1
}

public class BulletConfig
{
    public Vector3 Direction;
    public float Damage;
    public float Speed;
    public float Range;
    public float Luck;
    public BulletFlags Flags;

    public BulletConfig(Vector3 direction, float damage, float speed, float range, float luck, BulletFlags flags)
    {
        Direction = direction;
        Damage = damage;
        Speed = speed;
        Range = range;
        Luck = luck;
        Flags = flags;
    }
}