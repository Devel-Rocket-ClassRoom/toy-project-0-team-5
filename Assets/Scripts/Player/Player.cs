using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerHealth Health { get; private set;  }
    public PlayerAttack Attack { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerAnimator Animator { get; private set; }
    public PlayerConsumableItem Item { get; private set;  }

    private void Awake()
    {
        Health = GetComponent<PlayerHealth>();
        Attack = GetComponent<PlayerAttack>();
        Movement = GetComponent<PlayerMovement>(); 
        Animator = GetComponent<PlayerAnimator>();
        Item = GetComponent<PlayerConsumableItem>();
    }
}
