using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<Transform> OnRoomTransition;
    public static Action OnTransitionStart;
    public static Action OnTransitionEnd;
    public static Action OnRoomClear;
    public static Action OnPlayerHit;
}
