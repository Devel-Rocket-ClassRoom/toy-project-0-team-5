using UnityEngine;

public enum BulletType
{

}

public enum BulletFlags
{

}

public class BulletConfig
{
    public float Damage;
    public float Speed;
    public float Range;
    public BulletType Type;
    public BulletFlags Flags;
    public Vector3 Direction;
}