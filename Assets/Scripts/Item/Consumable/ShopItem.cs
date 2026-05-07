using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private ItemPickup itemPickup;
    [SerializeField] private int price;
    [SerializeField] private TMP_Text priceText;


    private void Awake()
    {
        itemPickup.enabled = false;

        priceText.text = $"{price:D1}개";
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
