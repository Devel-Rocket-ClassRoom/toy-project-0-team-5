using System;
using UnityEngine;

public static class GameEvents
{
    // --- 플레이어 관련 ---
    public static Action<int> OnPlayerHpChanged;
    public static Action<int> OnPlayerMaxHpChanged;
    public static Action OnPlayerHit;
    public static Action OnPlayerDie;
    public static Action OnPlayerUseBomb;

    // --- 아이템 관련 ---
    public static Action OnActiveCharged;
    public static Action OnActiveUsed;

    // --- 방 관련 ---
    public static Action<Transform, Transform> OnRoomTransition;
    public static Action OnTransitionStart;
    public static Action OnTransitionEnd;
    public static Action OnRoomClear;
    public static Action<GameObject> OnEnemyDead;
}
