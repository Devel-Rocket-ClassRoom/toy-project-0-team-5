using UnityEngine;

namespace ActiveItem
{
    [CreateAssetMenu(fileName = "YumHeart", menuName = "Item/Active/YumHeart")]
    public class YumHeart : ActiveItemBase
    {
        [SerializeField] private int _healAmount = 2; // 1칸

        public override void Use()
        {
            if (_currentCharge < _maxCharge) return;

            var playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
            if (playerHealth.OnHeal(_healAmount))
            {
                _currentCharge = 0;
                GameEvents.OnActiveUsed?.Invoke();
            }
        }
    }
}