using UnityEngine;

namespace PassiveItem
{
    [CreateAssetMenu(fileName = "Meat", menuName = "Item/Passive/Meat")]
    public class Meat : PassiveItemBase
    {
        protected override void OnCollect(GameObject collector)
        {
            var playerHealth = collector.GetComponent<PlayerHealth>();
            playerHealth.AddMaxHp(1);

            Modifier modifier = new(
                PlayerStatType.Damage,
                ModifierType.Addtive,
                1,
                this
            );
            var playerStats = collector.GetComponent<PlayerStats>();
            playerStats.AddModifier(modifier);
        }
    }
}