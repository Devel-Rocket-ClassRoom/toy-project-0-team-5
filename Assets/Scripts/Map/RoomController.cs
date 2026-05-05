using System.Collections.Generic;
using System.Linq;
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
    private bool _isActive;

    private void OnEnable() => GameEvents.OnEnemyDead += HandleEnemyDead;
    private void OnDisable() => GameEvents.OnEnemyDead -= HandleEnemyDead;

    public void Init(Vector2Int gridPosition, DoorFlags doorFlags)
    {
        GridPosition = gridPosition;
        IsCleared = false;

        _enemies = GetComponentsInChildren<EnemyBase>(includeInactive: true)
            .Select(e => e.gameObject)
            .ToList();

        _doorNorth?.gameObject.SetActive((doorFlags & DoorFlags.North) != 0);
        _doorSouth?.gameObject.SetActive((doorFlags & DoorFlags.South) != 0);
        _doorEast?.gameObject.SetActive((doorFlags & DoorFlags.East) != 0);
        _doorWest?.gameObject.SetActive((doorFlags & DoorFlags.West) != 0);
    }

    public void OnRoomEnter()
    {
        if (_enemies.Count == 0)
        {
            IsCleared = true;
            SetDoorsLocked(false);
            return;
        }

        _isActive = true;
        SetDoorsLocked(true);

        foreach (var enemy in _enemies)
            enemy.SetActive(true);
    }

    public void OnRoomClear()
    {
        _isActive = false;
        IsCleared = true;
        SetDoorsLocked(false);
        GameEvents.OnRoomClear?.Invoke();
    }

    private void HandleEnemyDead(GameObject deadEnemy)
    {
        if (!_isActive) return;
        if (!_enemies.Remove(deadEnemy)) return;

        if (_enemies.Count == 0)
            OnRoomClear();
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
            DoorFlags.East => _doorEast,
            DoorFlags.West => _doorWest,
            _ => null,
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
