using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private ItemPickup itemPickup;
    [SerializeField] private int price;


    private void Awake()
    {
        itemPickup.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerConsumableItem playerInven = other.GetComponent<PlayerConsumableItem>();

        if (playerInven.CoinCount >= price)
        {
            itemPickup.GetItem(other.gameObject);
            playerInven.UseCoin(price);
            
        }
    }
}
