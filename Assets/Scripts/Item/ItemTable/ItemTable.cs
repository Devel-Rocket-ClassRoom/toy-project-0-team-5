using System.Collections.Generic;
using ActiveItem;
using ConsumableItem;
using UnityEngine;

public class ItemTable : MonoBehaviour
{
    [SerializeField] private List<ActiveItemBase> _actives;
    // [SerializeField] private List<PassiveItemBase> _passives;
    // [SerializeField] private List<ConsumableItemBase> _consumables;

    private readonly List<ICollectible> _table = new();

    private void Awake()
    {
        // 액티브 아이템 추가
        foreach (var active in _actives)
        {
            _table.Add(active);
        }

        // 패시브 아이템 추가
        // foreach (var passive in _passives)
        // {
        //     _table.Add(passive);
        // }

        // 소비관련템 추가
        // foreach (var consumable in _consumables)
        // {
        //     _table.Add(consumable);
        // }
    }

    public ICollectible GetRandomItem()
    {
        int i = Random.Range(0, _table.Count);
        var item = _table[i];
        _table[i] = _table[_table.Count - 1];
        _table.RemoveAt(_table.Count - 1);
        return item;
    }

    public void AddItem(ICollectible item)
    {
        _table.Add(item);
    }
}