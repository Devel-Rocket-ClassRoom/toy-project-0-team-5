using UnityEngine;

public enum ItemType
{
    Coin,
    Bomb,
    Key
}

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private int amount = 1;


    private void OnTriggerEnter(Collider other)
    {
       if (!other.TryGetComponent(out PlayerItem playerItem))
        {
            return;
        }
        playerItem.AddItem(itemType, amount);

        Destroy(gameObject);
    }
}
