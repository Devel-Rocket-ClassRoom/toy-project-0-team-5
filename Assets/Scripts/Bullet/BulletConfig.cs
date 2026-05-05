using UnityEngine;

public enum BulletFlags
{
    NONE = 0,
    HOMING = 1 << 0,
    PIERCING = 1 << 1
}

public class BulletConfig
{
    public readonly float Damage;
    public readonly float Speed;
    public readonly float Range;
    public readonly float Luck;
    public readonly BulletFlags Flags;

    public BulletConfig(float damage, float speed, float range, float luck, BulletFlags flags)
    {
        Damage = damage;
        Speed = speed;
        Range = range;
        Luck = luck;
        Flags = flags;
    }
}