using UnityEngine;

public class ItemStand : MonoBehaviour
{
    [SerializeField] private ItemTable _itemTable;
    [SerializeField] private SpriteRenderer _renderer; // 현재 단상 위 아이템 스프라이트

    private ICollectible _currentItem;
    private bool _collectFlag;

    public ICollectible CurrentItem => _currentItem;

    private void Start()
    {
        _currentItem = _itemTable.GetRandomItem();
        _currentItem.Init();
        _renderer.sprite = _currentItem.Sprite;
    }

    private void OnDestroy()
    {
        if (_currentItem == null) return;

        _itemTable.AddItem(_currentItem);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player") || _currentItem == null || _collectFlag) return;

        _collectFlag = true;
        _currentItem = _currentItem.Collect(other.gameObject);
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