using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(InputManager))]
public class PlayerMovement : MonoBehaviour
{
    // private PlayerStats _playerStats;
    private Rigidbody _rigidBody;
    private InputManager _inputManager;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _inputManager = GetComponent<InputManager>();
    }

    private void FixedUpdate()
    {
        _rigidBody.linearVelocity =
            new Vector3(
                _inputManager.MoveInput.x,
                _rigidBody.linearVelocity.y,
                _inputManager.MoveInput.y
            ) * 10f; // TODO: 스탯 생기면 movespeed 적용
    }
}
