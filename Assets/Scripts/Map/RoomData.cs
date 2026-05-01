using System;
using UnityEngine;

[Serializable]
public class RoomData
{
    public int Id;
    public RoomType RoomType;
    public DoorFlags DoorFlags;
    public int Weight;
    public GameObject Prefab;
}
