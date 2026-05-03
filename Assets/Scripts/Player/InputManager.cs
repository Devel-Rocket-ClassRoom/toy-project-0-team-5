using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private bool _normalizeDiagonal = true;

    public Vector2 MoveInput { get; private set; }
    public Vector2 ShootInput { get; private set; }

    public bool BombPressed { get; private set; }
    public bool UseActivePressed { get; private set; }
    public bool PausePressed { get; private set; }

    public bool IsMoving => MoveInput != Vector2.zero;
    public bool IsShooting => ShootInput != Vector2.zero;

    // 발사 방향 — 마지막에 눌린 키 우선
    private readonly List<KeyCode> _shootStack = new();
    private static readonly KeyCode[] _shootKeys =
    {
        KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow
    };

    private void Update()
    {
        MoveInput = ReadAxis(KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D);
        ShootInput = ReadShoot();

        BombPressed = Input.GetKeyDown(KeyCode.E);
        UseActivePressed = Input.GetKeyDown(KeyCode.Space);
        PausePressed = Input.GetKeyDown(KeyCode.Escape);
    }

    private Vector2 ReadShoot()
    {
        foreach (var k in _shootKeys)
        {
            if (Input.GetKeyDown(k))
            {
                _shootStack.Remove(k);
                _shootStack.Add(k);
            }
            if (Input.GetKeyUp(k))
            {
                _shootStack.Remove(k);
            }
        }

        if (_shootStack.Count == 0) return Vector2.zero;

        return KeyToDir(_shootStack[_shootStack.Count - 1]);
    }

    private static Vector2 KeyToDir(KeyCode k) => k switch
    {
        KeyCode.UpArrow => Vector2.up,
        KeyCode.DownArrow => Vector2.down,
        KeyCode.LeftArrow => Vector2.left,
        KeyCode.RightArrow => Vector2.right,
        _ => Vector2.zero,
    };

    private Vector2 ReadAxis(KeyCode up, KeyCode down, KeyCode left, KeyCode right)
    {
        Vector2 v = Vector2.zero;
        if (Input.GetKey(up)) v.y += 1f;
        if (Input.GetKey(down)) v.y -= 1f;
        if (Input.GetKey(left)) v.x -= 1f;
        if (Input.GetKey(right)) v.x += 1f;

        if (_normalizeDiagonal && v.sqrMagnitude > 1f)
            v.Normalize();

        return v;
    }
}
