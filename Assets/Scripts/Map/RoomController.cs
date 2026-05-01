using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public Vector2Int GridPosition;
    public bool IsCleared;

    [SerializeField] private DoorController _doorNorth;
    [SerializeField] private DoorController _doorSouth;
    [SerializeField] private DoorController _doorEast;
    [SerializeField] private DoorController _doorWest;

    private List<GameObject> _enemies = new();

    public void Init(Vector2Int gridPosition, List<GameObject> enemies)
    {
        GridPosition = gridPosition;
        _enemies = enemies;
        IsCleared = false;
    }

    public void OnRoomEnter()
    {
        SetDoorsLocked(true);
        foreach (var enemy in _enemies)
            enemy.SetActive(true);
    }

    public void OnRoomClear()
    {
        IsCleared = true;
        SetDoorsLocked(false);
    }

    public Transform GetSpawnPoint(DoorFlags direction)
    {
        return GetDoor(direction)?.SpawnPoint;
    }

    public DoorController GetDoor(DoorFlags direction)
    {
        return direction switch
        {
            DoorFlags.North => _doorNorth,
            DoorFlags.South => _doorSouth,
            DoorFlags.East  => _doorEast,
            DoorFlags.West  => _doorWest,
            _               => null,
        };
    }

    private void SetDoorsLocked(bool locked)
    {
        _doorNorth?.SetLocked(locked);
        _doorSouth?.SetLocked(locked);
        _doorEast?.SetLocked(locked);
        _doorWest?.SetLocked(locked);
    }
}
