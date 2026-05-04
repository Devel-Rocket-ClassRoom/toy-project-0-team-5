using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerStats))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private InputManager _inputManager;
    private PlayerStats _playerStats;

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
    }
}
