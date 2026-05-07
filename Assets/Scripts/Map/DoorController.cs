using UnityEngine;

public class DoorController : MonoBehaviour
{
    public DoorFlags Direction;
    public Transform SpawnPoint;

    [SerializeField] private RoomController _room;
    // 잠금 시 플레이어를 막는 벽 콜라이더 (non-trigger)
    [SerializeField] private GameObject _wallCollider;
    // 개방 시 플레이어 통과를 감지하는 트리거 콜라이더 (trigger)
    [SerializeField] private GameObject _triggerCollider;

    private FloorManager _floorManager;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Init(FloorManager floorManager)
    {
        _floorManager = floorManager;
        SetLocked(true);
    }

    public void SetLocked(bool locked)
    {
        if (_wallCollider != null) _wallCollider.SetActive(locked);
        if (_triggerCollider != null) _triggerCollider.SetActive(!locked);
    }

    public void OnPlayerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _floorManager?.OnPlayerEnterDoor(_room, Direction);
    }

    // private RoomController GetNextRoom()
    // {
    //     return Direction switch
    //     {
    //         DoorFlags.North => _floorManager.GetRoom(_room.GridPosition + Vector2Int.up),
    //         DoorFlags.South => _floorManager.GetRoom(_room.GridPosition + Vector2Int.down),
    //         DoorFlags.East => _floorManager.GetRoom(_room.GridPosition + Vector2Int.right),
    //         DoorFlags.West => _floorManager.GetRoom(_room.GridPosition + Vector2Int.left),
    //         _ => null,
    //     };
    // }
}
