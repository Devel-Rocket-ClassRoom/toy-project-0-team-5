using System;
using UnityEngine;

/// <summary>
/// 플레이어가 E키를 눌러 상호작용할 수 있는 컴포넌트.
/// 반경 내에 "Player" 태그 오브젝트가 있을 때만 OnInteract 이벤트를 발생시킨다.
/// Chest, Pedestal 등 상호작용 로직은 OnInteract를 구독해서 각자 구현한다.
/// </summary>
public class Interactable : MonoBehaviour
{
    [SerializeField] private float _interactRadius = 1.5f;

    /// <summary>E키 입력 + 플레이어가 반경 내에 있을 때 발생. 인자는 플레이어 GameObject.</summary>
    public event Action<GameObject> OnInteract;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        // 반경 내 콜라이더를 검색해 플레이어가 있으면 이벤트 발생
        var hits = Physics.OverlapSphere(transform.position, _interactRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                OnInteract?.Invoke(hit.gameObject);
                break;
            }
        }
    }
}
