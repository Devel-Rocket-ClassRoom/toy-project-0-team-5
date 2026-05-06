using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerConsumableItem : MonoBehaviour
{
    private int coinCount;
    private int bombCount;
    private int keyCount;

    public int CoinCount => coinCount;
    public int BombCount => bombCount;
    public int KeyCount => keyCount;

    public event Action OnItemChanged;


    public void AddItem(ItemType itemType, int amount)
    {
        switch (itemType)
        {
            case ItemType.Coin:
                coinCount += amount;
                break;

            case ItemType.Bomb:
                bombCount += amount;
                break;

            case ItemType.Key:
                keyCount += amount;
                break;
        }
        OnItemChanged?.Invoke();
    }

    public bool UseBomb(int amount = 1)
    {
        if (bombCount < amount)
            return false;

        bombCount -= amount;

        OnItemChanged?.Invoke();

        return true;
    }

    public bool UseKey(int amount = 1)
    {
        if (keyCount < amount)
            return false;

        keyCount -= amount;

        OnItemChanged?.Invoke();

        return true;
    }
}
