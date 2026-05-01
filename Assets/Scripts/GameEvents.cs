using System;
using UnityEngine;

public static class GameEvents
{
    public static Action<Transform> OnRoomTransition;
    public static Action OnRoomClear;
    public static Action OnPlayerHit;
}
