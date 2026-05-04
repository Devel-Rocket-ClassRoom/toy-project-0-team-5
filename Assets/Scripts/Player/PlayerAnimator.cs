using System.Data.SqlTypes;
using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class PlayerAnimator : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move,
        Damage,
        Die
    }

    [SerializeField] private Animator animator;
    [SerializeField] private InputManager inputManager;

    private PlayerState state;
    private float idleTimer;

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
    }

    private void Update()
    {
        if (inputManager.IsMoving)
        {
            SetState(PlayerState.Move);
        }
        else
        {
            SetState(PlayerState.Idle);
        }
        UpdateIdleTimer();
    }

    private void UpdateIdleTimer()
    {
        if (state == PlayerState.Idle)
        {
            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);

            if (animState.IsName("Idle2"))
            {
                idleTimer = 0;
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

    public void PlayDamage()
    {
        SetState(PlayerState.Damage);
    }

    public void PlayDie()
    {
        SetState(PlayerState.Die);
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
