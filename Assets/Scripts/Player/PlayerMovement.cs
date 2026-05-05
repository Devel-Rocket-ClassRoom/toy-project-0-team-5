using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 20f;
    private Rigidbody _rigidBody;
    private InputManager _inputManager;
    private PlayerStats _playerStats;

    private Vector3 _currentDirection = Vector3.forward;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _inputManager = GetComponent<InputManager>();
        _playerStats = GetComponent<PlayerStats>();
    }

    private void FixedUpdate()
    {
        _rigidBody.linearVelocity =
            new Vector3(
                _inputManager.MoveInput.x,
                0f,
                _inputManager.MoveInput.y
            ) * _playerStats.MoveSpeed;

        if (_inputManager.ShootInput != Vector2.zero)
            _currentDirection = new Vector3(_inputManager.ShootInput.x, 0f, _inputManager.ShootInput.y);

        Quaternion targetRot = Quaternion.LookRotation(_currentDirection);
        _rigidBody.MoveRotation(Quaternion.Slerp(_rigidBody.rotation, targetRot, _rotationSpeed * Time.fixedDeltaTime));
    }
}
