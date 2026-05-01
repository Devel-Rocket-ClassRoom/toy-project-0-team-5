using UnityEngine;

public class DoorController : MonoBehaviour
{
    public DoorFlags Direction;
    public Transform SpawnPoint;

    private Collider _doorCollider;

    private void Awake()
    {
        _doorCollider = GetComponent<Collider>();
    }

    public void SetLocked(bool locked)
    {
        if (_doorCollider != null)
            _doorCollider.enabled = locked;
    }
}
