using UnityEngine;

public enum ItemType
{
    Coin,
    Bomb,
    Key,
    Heart,
}

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private int amount = 1;


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("부딪힘");

        if (itemType == ItemType.Heart)
        {
            PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
            if (playerHealth == null)
                return;

            bool healed = playerHealth.OnHeal(amount);

            if (healed)
            {
                Destroy(gameObject);
            }
            return;

        }
        PlayerConsumableItem playerItem = other.GetComponentInParent<PlayerConsumableItem>();

        if (playerItem == null)
            return;

        playerItem.AddItem(itemType, amount);

        Destroy(gameObject);
    }
}
