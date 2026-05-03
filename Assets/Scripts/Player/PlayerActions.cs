using UnityEngine;

/// <summary>
/// Player가 환경과의 상호작용을 위한 컴포넌트
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(InputManager))]
public class PlayerActions : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private InputManager _inputManager;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
    }
}