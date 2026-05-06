using UnityEngine;

public class ItemStand : MonoBehaviour
{
    [SerializeField] private ItemTable _itemTable;
    [SerializeField] private SpriteRenderer _renderer; // 현재 단상 위 아이템 스프라이트

    private ICollectible _currentItem;
    private bool _collectFlag;

    public ICollectible CurrentItem => _currentItem;
    public bool CollectFlag => _collectFlag;

    private void Start()
    {
        _currentItem = _itemTable.GetRandomItem();
        _currentItem.Init();
        _renderer.sprite = _currentItem.Sprite;
        _renderer.transform.rotation = Camera.main.transform.rotation;
    }

    // private void Update()
    // {
    //     if (_currentItem == null) return;

    // }

    private void OnDestroy()
    {
        if (_currentItem == null) return;

        _itemTable.AddItem(_currentItem);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player") || _currentItem == null || _collectFlag) return;

        OnChangeItem(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        OnExitFlag();
    }

    public void OnChangeItem(GameObject other)
    {
        _collectFlag = true;
        _currentItem = _currentItem.Collect(other);
        if (_currentItem == null)
        {
            _renderer.sprite = null;
        }
        else
        {
            _renderer.sprite = _currentItem.Sprite;
        }
    }

    public void OnExitFlag()
    {
        _collectFlag = false;
    }
}