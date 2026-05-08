using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<RoomData> _roomDataList;
    [SerializeField] private float _roomSpacing = 20f;
    [SerializeField] private int _totalRooms = 10;

    private Dictionary<Vector2Int, RoomController> _rooms = new();
    private RoomController _currentRoom;
    public RoomController CurrentRoom => _currentRoom;

    private EnemyBase _boss;
    public EnemyBase Boss => _boss;

    private void Start()
    {
        GenerateFloor();
    }

    private void GenerateFloor()
    {
        var generator = new MapGenerator(_totalRooms);
        var nodes = generator.Generate();
        var nodeDict = new Dictionary<Vector2Int, RoomNode>();
        foreach (var node in nodes) nodeDict[node.GridPosition] = node;

        var directions = new (Vector2Int offset, DoorFlags flag)[]
        {
            (Vector2Int.up,    DoorFlags.North),
            (Vector2Int.down,  DoorFlags.South),
            (Vector2Int.right, DoorFlags.East),
            (Vector2Int.left,  DoorFlags.West),
        };

        foreach (var node in nodes)
        {
            var data = GetRoomData(node.RoomType);
            if (data?.Prefab == null) continue;

            var worldPos = new Vector3(node.GridPosition.x, 0, node.GridPosition.y) * _roomSpacing;
            var instance = Instantiate(data.Prefab, worldPos, Quaternion.identity, transform);

            var controller = instance.GetComponent<RoomController>();
            if (controller == null) continue;

            var neighborTypes = new Dictionary<DoorFlags, RoomType>();
            foreach (var (offset, flag) in directions)
            {
                if (nodeDict.TryGetValue(node.GridPosition + offset, out var neighbor))
                    neighborTypes[flag] = neighbor.RoomType;
            }

            controller.Init(node.GridPosition, node.DoorFlags, this, neighborTypes);
            _rooms[node.GridPosition] = controller;

            if (node.RoomType == RoomType.Boss)
                _boss = controller.Boss;

            if (node.RoomType == RoomType.Start)
                ActivateRoom(controller);
        }
    }

    public void OnPlayerEnterDoor(RoomController currentRoom, DoorFlags enteredDoor)
    {
        var direction = DoorFlagToVector(enteredDoor);
        var nextPos = currentRoom.GridPosition + direction;

        if (!_rooms.TryGetValue(nextPos, out var nextRoom)) return;

        ActivateRoom(nextRoom);

        var oppositeFlag = GetOppositeFlag(enteredDoor);
        var spawnPoint = nextRoom.GetSpawnPoint(oppositeFlag);

        if (spawnPoint != null)
            GameEvents.OnRoomTransition?.Invoke(spawnPoint, nextRoom.transform);

        currentRoom.gameObject.SetActive(false);
    }

    private void ActivateRoom(RoomController room)
    {
        room.gameObject.SetActive(true);
        _currentRoom = room;
        room.OnRoomEnter();
    }

    public RoomController GetRoom(Vector2Int gridPosition)
    {
        _rooms.TryGetValue(gridPosition, out var room);
        return room;
    }

    public static Vector2Int DoorFlagToVector(DoorFlags flag)
    {
        return flag switch
        {
            DoorFlags.North => Vector2Int.up,
            DoorFlags.South => Vector2Int.down,
            DoorFlags.East => Vector2Int.right,
            DoorFlags.West => Vector2Int.left,
            _ => Vector2Int.zero,
        };
    }

    public static DoorFlags GetOppositeFlag(DoorFlags flag)
    {
        return flag switch
        {
            DoorFlags.North => DoorFlags.South,
            DoorFlags.South => DoorFlags.North,
            DoorFlags.East => DoorFlags.West,
            DoorFlags.West => DoorFlags.East,
            _ => DoorFlags.None,
        };
    }

    private RoomData GetRoomData(RoomType type)
    {
        var candidates = _roomDataList.Where(d => d.RoomType == type).ToList();
        if (candidates.Count == 0)
            candidates = _roomDataList.Where(d => d.RoomType == RoomType.Normal).ToList();
        if (candidates.Count == 0) return null;

        int totalWeight = candidates.Sum(d => d.Weight);
        if (totalWeight <= 0) return candidates[0];

        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;
        foreach (var data in candidates)
        {
            cumulative += data.Weight;
            if (roll < cumulative) return data;
        }
        return candidates[^1];
    }
}
