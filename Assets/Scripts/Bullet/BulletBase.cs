using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class BulletBase : MonoBehaviour
{
    public abstract void Init(LayerMask targetLayer, Vector3 direction, BulletConfig config);
}
