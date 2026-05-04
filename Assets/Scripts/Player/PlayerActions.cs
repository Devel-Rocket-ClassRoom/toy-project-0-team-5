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

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (_inputManager.UseActivePressed) UseCurrentItem();
    }

    private void FixedUpdate()
    {
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
}