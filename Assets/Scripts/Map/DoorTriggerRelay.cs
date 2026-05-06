using UnityEngine;

// DoorTrigger 자식 오브젝트에 붙이는 스크립트.
// 트리거 이벤트를 부모 DoorController로 전달한다.
public class DoorTriggerRelay : MonoBehaviour
{
    [SerializeField] private DoorController _door;

    private void Awake()
    {
        _door = GetComponentInParent<DoorController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("DoorTriggerRelay.OnTriggerEnter");
        _door?.OnPlayerEnter(other);
    }
}
