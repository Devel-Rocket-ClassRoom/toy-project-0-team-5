using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 15f;
    [SerializeField] private float moveSpeed = 10f;

    private Rigidbody rb;
    private Quaternion targetRotation;
    private Vector3 currentDirection;
    private List<KeyCode> moveKeys = new List<KeyCode>();

    public Vector3 CurrentDirection => currentDirection;
    public bool IsMoving => currentDirection != Vector3.zero;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

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
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = currentDirection * moveSpeed;

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


}
