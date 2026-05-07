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
        if (!enabled)
            return;

        Debug.Log($"ItemPickup OnTriggerEnter 발생: {gameObject.name}, enabled: {enabled}");

        if (!other.CompareTag("Player"))
            return;

        GetItem(other.gameObject);
    }

    public void GetItem(GameObject player)
    {
        if (itemType == ItemType.Heart)
        {
            PlayerHealth playerHealth = player.GetComponentInParent<PlayerHealth>();
            if (playerHealth == null)
                return;

            bool healed = playerHealth.OnHeal(amount);

            if (healed)
            {
                Destroy(gameObject);
            }
            return;

        }
        PlayerConsumableItem playerItem = player.GetComponentInParent<PlayerConsumableItem>();

        if (playerItem == null)
            return;

        playerItem.AddItem(itemType, amount);

        Destroy(gameObject);
    }


}
