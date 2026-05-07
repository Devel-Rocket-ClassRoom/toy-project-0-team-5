using UnityEngine;

public class DoorController : MonoBehaviour
{
    public DoorFlags Direction;
    public Transform SpawnPoint;

    [SerializeField] private RoomController _room;
    // 잠금 시 플레이어를 막는 벽 콜라이더 (non-trigger)
    [SerializeField] private GameObject _wallCollider;
    // 개방 시 플레이어 통과를 감지하는 트리거 콜라이더 (trigger)
    [SerializeField] private Collider _triggerCollider;

    [Header("Door Variants")]
    [SerializeField] private GameObject _normalDoor;
    [SerializeField] private GameObject _bossDoor;
    [SerializeField] private GameObject _goldenDoor;

    private FloorManager _floorManager;

    public void Init(FloorManager floorManager, RoomType neighborType)
    {
        _floorManager = floorManager;
        ActivateVariant(neighborType);
        SetLocked(true);
    }

    private void ActivateVariant(RoomType neighborType)
    {
        // 변형이 하나만 있으면 (Door_Boss, Door_Golden 등) 그냥 켜기
        bool hasMultipleVariants = _normalDoor != null && (_bossDoor != null || _goldenDoor != null);
        if (!hasMultipleVariants)
        {
            _normalDoor?.SetActive(true);
            _bossDoor?.SetActive(true);
            _goldenDoor?.SetActive(true);
            return;
        }

        bool isBoss   = neighborType == RoomType.Boss;
        bool isGolden = neighborType == RoomType.Treasure;
        _normalDoor.SetActive(!isBoss && !isGolden);
        _bossDoor?.SetActive(isBoss);
        _goldenDoor?.SetActive(isGolden);
    }

    public void SetLocked(bool locked)
    {
        if (_wallCollider != null) _wallCollider.SetActive(locked);
        if (_triggerCollider != null) _triggerCollider.enabled = !locked;
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
