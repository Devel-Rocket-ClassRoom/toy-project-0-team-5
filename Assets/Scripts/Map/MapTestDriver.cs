using UnityEngine;

public class MapTestDriver : MonoBehaviour
{
    [SerializeField] private FloorManager _floorManager;

    private void Update()
    {
        var room = _floorManager.CurrentRoom;
        if (room == null) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Testing player entering north door");
            _floorManager.OnPlayerEnterDoor(room, DoorFlags.North);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Testing player entering south door");
            _floorManager.OnPlayerEnterDoor(room, DoorFlags.South);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Testing player entering east door");
            _floorManager.OnPlayerEnterDoor(room, DoorFlags.East);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Testing player entering west door");
            _floorManager.OnPlayerEnterDoor(room, DoorFlags.West);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Testing room clear");
            _floorManager.CurrentRoom.OnRoomClear();
        }
    }
}
