using UnityEngine;

namespace PassiveItem
{
    [CreateAssetMenu(fileName = "Meat", menuName = "Item/Passive/Meat")]
    public class Meat : PassiveItemBase
    {
        [SerializeField] private int _healAmount = 1;
        [SerializeField] private float _damageValue = 1f;

        protected override void OnCollect(GameObject collector)
        {
            var playerHealth = collector.GetComponent<PlayerHealth>();
            playerHealth.AddMaxHp(_healAmount);

            Modifier modifier = new(
                PlayerStatType.Damage,
                ModifierType.Additive,
                _damageValue,
                this
            );
            var playerStats = collector.GetComponent<PlayerStats>();
            playerStats.AddModifier(modifier);
        }
    }
}