using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerHealth Health { get; private set;  }
    public PlayerStats Stats  { get; private set; }
    public PlayerAttack Attack { get; private set; }
    public PlayerMovement Movement { get; private set; }

    private void Awake()
    {
        Health = GetComponent<PlayerHealth>();
        Stats = GetComponent<PlayerStats>();
        Attack = GetComponent<PlayerAttack>();
        Movement = GetComponent<PlayerMovement>(); 
    }
}
