using UnityEngine;

namespace PassiveItem
{
    [CreateAssetMenu(fileName = "Meat", menuName = "Item/Passive/Meat")]
    public class Meat : PassiveItemBase
    {
        [SerializeField] private int _MaxHPAmount = 1;

        protected override void OnCollect(GameObject collector)
        {
            var playerHealth = collector.GetComponent<PlayerHealth>();
            playerHealth.AddMaxHp(_MaxHPAmount);

            Modifier modifier = new(
                PlayerStatType.Damage,
                ModifierType.Additive,
                _value,
                this
            );
            var playerStats = collector.GetComponent<PlayerStats>();
            playerStats.AddModifier(modifier);
        }
    }
}