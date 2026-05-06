using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerItem : MonoBehaviour
{
    private int coin;
    private int bomb;
    private int key;

    public int Coin => coin;
    public int Bomb => bomb;
    public int Key => key;

    public event Action OnItemChanged;
    public Player player;


    public void AddItem(ItemType itemType, int amount)
    {
        switch (itemType)
        {
            case ItemType.Coin:
                coin += amount;
                break;

            case ItemType.Bomb:
                bomb += amount;
                break;

            case ItemType.Key:
                key += amount;
                break;
            //case ItemType.Heart:
                //player.Health.Heal(amount);
               // break;
        }
        OnItemChanged?.Invoke();
    }
}
