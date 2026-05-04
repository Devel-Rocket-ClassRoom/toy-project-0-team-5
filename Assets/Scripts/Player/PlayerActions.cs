using ActiveItem;
using UnityEngine;

/// <summary>
/// Player가 환경과의 상호작용을 위한 컴포넌트
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerActions : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private ActiveItemBase _equippedItem;
    [SerializeField] private IBulletStrategy _strategy;
    private PlayerStats _playerStats;

    private Rigidbody _rigidBody;
    private InputManager _inputManager;

    private Vector3 _currentDirection = Vector3.forward;
    private BulletFlags _flags;
    private int _attackFrameTimer;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _inputManager = GetComponent<InputManager>();
        _playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        _attackFrameTimer++;

        if (_inputManager.UseActivePressed) UseCurrentItem();
        // if (_inputManager.BombPressed) CreateBomb();
        if (_inputManager.ShootInput != Vector2.zero) OnShoot();
    }

    private void FixedUpdate()
    {
        Quaternion targetRot = Quaternion.LookRotation(_currentDirection);
        _rigidBody.MoveRotation(Quaternion.Slerp(_rigidBody.rotation, targetRot, _rotationSpeed * Time.fixedDeltaTime));
    }

    private void UseCurrentItem()
    {
        if (_equippedItem == null) return;

        _equippedItem.Use();
        GameEvents.OnActiveUsed?.Invoke();
    }

    public ICollectible OnChangeActiveItem(ActiveItemBase item)
    {
        var currentItem = _equippedItem;
        _equippedItem = item;
        Debug.Log($"장착 완료!: {_equippedItem.GetType().Name}");
        return currentItem;
    }

    // private void CreateBomb() { }

    private void OnShoot()
    {
        if (_attackFrameTimer < _playerStats.Delay) return;

        _currentDirection = new Vector3(_inputManager.ShootInput.x, 0f, _inputManager.ShootInput.y);
        // BulletConfig config = new(
        //     _currentDirection,
        //     _playerStats.Damage,
        //     _playerStats.ShotSpeed,
        //     _playerStats.Range,
        //     _playerStats.Luck,
        //     _flags
        // );
        // _strategy.Fire(null);
        Debug.Log("Attack!!!");
        _attackFrameTimer = 0;
    }
}