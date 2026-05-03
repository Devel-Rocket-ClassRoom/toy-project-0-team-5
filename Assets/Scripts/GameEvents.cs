using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<Transform> OnRoomTransition;
    public static Action OnTransitionStart;
    public static Action OnTransitionEnd;
    public static Action OnRoomClear;
    public static Action<int> OnPlayerHpChanged;
    public static Action<int> OnPlayerMaxHpChanged;
    public static Action OnPlayerHit;
    public static Action OnPlayerDie;
    public static Action OnPlayerUseActive;
    public static Action OnPlayerUseBomb;
}
