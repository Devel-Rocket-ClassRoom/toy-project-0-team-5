using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float playerSpeed = 8;
    [SerializeField] private float rotateSpeed = 15f;

    public enum PlayerState
    {
        Idle,
        Move,
        Damage,
        Die
    }

    private PlayerState state;
    private Rigidbody rb;
    private float idleTimer;

    private Quaternion targetRotation;
    private Vector3 currentDirection;
    private List<KeyCode> moveKeys = new List<KeyCode>();


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        targetRotation = transform.rotation;
    }

    private void Update()
    {
        CheckKey(KeyCode.W);
        CheckKey(KeyCode.UpArrow);

        CheckKey(KeyCode.S);
        CheckKey(KeyCode.DownArrow);

        CheckKey(KeyCode.A);
        CheckKey(KeyCode.LeftArrow);

        CheckKey(KeyCode.D);
        CheckKey(KeyCode.RightArrow);

        if (moveKeys.Count > 0)
        {
            KeyCode lastKey = moveKeys[moveKeys.Count - 1];
            currentDirection = KeyToDirection(lastKey);
        }
        else
        {
            currentDirection = Vector3.zero;
        }

        // Move 애니메이션 전환
        if (currentDirection == Vector3.zero)
        {
            SetState(PlayerState.Idle);
        }
        else
        {
            SetState(PlayerState.Move);
        }

        // Idle2 애니메이션 출력
        if (state == PlayerState.Idle)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

            if (animState.IsName("Idle2"))
            {
                idleTimer = 0f;
            }
            else
            {
                idleTimer += Time.deltaTime;
            }
        }
        else
        {
            idleTimer = 0;
        }
        animator.SetFloat("IdleTime", idleTimer);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = currentDirection * playerSpeed;

        if (currentDirection != Vector3.zero)
            Rotate(currentDirection);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.fixedDeltaTime);
    }

    private void CheckKey(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            moveKeys.Remove(key);
            moveKeys.Add(key);
        }

        if (Input.GetKeyUp(key))
        {
            moveKeys.Remove(key);
        }
    }

    private Vector3 KeyToDirection(KeyCode key)
    {
        if (key == KeyCode.W || key == KeyCode.UpArrow)
            return Vector3.forward;

        if (key == KeyCode.S || key == KeyCode.DownArrow)
            return Vector3.back;

        if (key == KeyCode.A || key == KeyCode.LeftArrow)
            return Vector3.left;

        if (key == KeyCode.D || key == KeyCode.RightArrow)
            return Vector3.right;

        return Vector3.zero;
    }

    private void Rotate(Vector3 direction)
    {
        if (direction == Vector3.zero)
            return;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            if (direction.x > 0f)
                targetRotation = Quaternion.Euler(0, 90, 0);
            else
                targetRotation = Quaternion.Euler(0, -90, 0);
        }
        else
        {
            if (direction.z > 0f)
                targetRotation = Quaternion.Euler(0, 0, 0);
            else
                targetRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void SetState(PlayerState newState)
    {
        if (state == newState)
            return;

        state = newState;

        switch (state)
        {
            case PlayerState.Idle:
                animator.SetBool("IsMoving", false);
                break;

            case PlayerState.Move:
                animator.SetBool("IsMoving", true);
                break;

            case PlayerState.Damage:
                animator.SetTrigger("Damage");
                break;

            case PlayerState.Die:
                animator.SetTrigger("Die");
                break;
        }
    }
}
