using System;
using System.Collections.Generic;
using PassiveItem;

public class PlayerPassiveItems : UnityEngine.MonoBehaviour
{
    private readonly List<PassiveItemBase> _items = new();

    public IReadOnlyList<PassiveItemBase> Items => _items;

    public event Action OnPassiveItemAdded;

    public void Add(PassiveItemBase item)
    {
        _items.Add(item);
        OnPassiveItemAdded?.Invoke();
    }
}
