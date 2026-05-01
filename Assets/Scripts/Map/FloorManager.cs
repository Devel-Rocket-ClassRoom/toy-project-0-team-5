using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<RoomData> _roomDataList;
    [SerializeField] private float _roomSpacing = 20f;
    [SerializeField] private int _totalRooms = 10;

    private Dictionary<Vector2Int, RoomController> _rooms = new();
    private RoomController _currentRoom;

    private void Start()
    {
        GenerateFloor();
    }

    private void GenerateFloor()
    {
        var generator = new MapGenerator(_totalRooms);
        var nodes = generator.Generate();

        foreach (var node in nodes)
        {
            var data = GetRoomData(node.RoomType);
            if (data?.Prefab == null) continue;

            var worldPos = new Vector3(node.GridPosition.x, 0, node.GridPosition.y) * _roomSpacing;
            var instance = Instantiate(data.Prefab, worldPos, Quaternion.identity, transform);

            var controller = instance.GetComponent<RoomController>();
            if (controller == null) continue;

            controller.Init(node.GridPosition, new List<GameObject>());
            _rooms[node.GridPosition] = controller;

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
            GameEvents.OnRoomTransition?.Invoke(spawnPoint);
    }

    private void ActivateRoom(RoomController room)
    {
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
            DoorFlags.East  => Vector2Int.right,
            DoorFlags.West  => Vector2Int.left,
            _               => Vector2Int.zero,
        };
    }

    public static DoorFlags GetOppositeFlag(DoorFlags flag)
    {
        return flag switch
        {
            DoorFlags.North => DoorFlags.South,
            DoorFlags.South => DoorFlags.North,
            DoorFlags.East  => DoorFlags.West,
            DoorFlags.West  => DoorFlags.East,
            _               => DoorFlags.None,
        };
    }

    private RoomData GetRoomData(RoomType type)
    {
        return _roomDataList.Find(d => d.RoomType == type);
    }
}
