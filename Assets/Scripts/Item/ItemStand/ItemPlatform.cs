using UnityEngine;

public class ItemPlatform : MonoBehaviour
{
    [SerializeField] private ItemStand _itemStand;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")
            || _itemStand == null
            || _itemStand.CurrentItem == null
            || _itemStand.CollectFlag) return;

        _itemStand.OnChangeItem(other.gameObject);
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        _itemStand.OnExitFlag();
    }
}