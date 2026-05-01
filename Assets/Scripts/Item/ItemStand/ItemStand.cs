using UnityEngine;

public class ItemStand : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer; // 현재 단상 위 아이템 스프라이트

    private ICollectible _currentItem;
    private bool _collectFlag;

    public ICollectible CurrentItem => _currentItem;

    private void Awake()
    {
        // TODO: 아이템 테이블 정의되면 구현 예정
        // table.GetRandomItem(out _currentItem);
        // _currentItem.Init();
        // _renderer.sprite = _currentItem.Sprite;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player") || _currentItem == null || _collectFlag) return;

        _collectFlag = true;
        _currentItem = _currentItem.Collect(other.gameObject); // TODO: 캐릭터 구현되면 수정 예정
        if (_currentItem == null)
        {
            _renderer.sprite = null;
        }
        else
        {
            _renderer.sprite = _currentItem.Sprite;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        _collectFlag = false;
    }
}