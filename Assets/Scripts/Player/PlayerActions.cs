using ActiveItem;
using UnityEngine;
using System;

/// <summary>
/// Player가 환경과의 상호작용을 위한 컴포넌트
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerActions : MonoBehaviour
{
    [SerializeField] private ActiveItemBase _equippedItem;
    [SerializeField] private BulletSpawner _spawner;

    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Transform bombSpawnPoint;

    private PlayerStats _playerStats;
    private InputManager _inputManager;
    private PlayerConsumableItem playerConsumableItem;

    private BulletFlags _flags;
    private float _attackTimer;

    public ActiveItemBase EquippedItem => _equippedItem;

    public event Action OnActiveItemChanged;

    private void Awake()
    {
        _inputManager = GetComponent<InputManager>();
        _playerStats = GetComponent<PlayerStats>();
        playerConsumableItem = GetComponent<PlayerConsumableItem>();
    }

    private void Update()
    {
        _attackTimer += Time.deltaTime;

        if (_inputManager.UseActivePressed) UseCurrentItem();
        if (_inputManager.BombPressed && playerConsumableItem.UseBomb()) CreateBomb();
        if (_inputManager.ShootInput != Vector2.zero) OnShoot();
    }

    private void UseCurrentItem()
    {
        if (_equippedItem == null) return;

        _equippedItem.Use();
    }

    public ICollectible OnChangeActiveItem(ActiveItemBase item)
    {
        var currentItem = _equippedItem;
        _equippedItem = item;
        Debug.Log($"장착 완료!: {_equippedItem.GetType().Name}");
        OnActiveItemChanged?.Invoke();
        return currentItem;
    }

    private void CreateBomb()
    {
        if (bombPrefab == null)
            return;
 
        Instantiate(
            bombPrefab,
            bombSpawnPoint.position,
            Quaternion.identity 
            );
    }

    private void OnShoot()
    {
        if (_attackTimer < _playerStats.Delay) return;

        if (_spawner != null)
        {
            var moveDir = new Vector3(_inputManager.MoveInput.x, 0f, _inputManager.MoveInput.y).normalized;
            var shotDir = new Vector3(_inputManager.ShootInput.x, 0f, _inputManager.ShootInput.y).normalized;
            BulletConfig config = new(
                _playerStats.Damage,
                _playerStats.ShotSpeed,
                _playerStats.Range,
                _playerStats.Luck,
                _flags
            );
            _spawner.SpawnBullet(shotDir + (moveDir * 0.5f), config); // TODO: 나중에 값 수정 필요하면 따로 필드화
        }

        _attackTimer = 0f;
    }
}