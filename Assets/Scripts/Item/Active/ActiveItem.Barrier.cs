using UnityEngine;

namespace ActiveItem
{
    [CreateAssetMenu(fileName = "Barrier", menuName = "Item/Active/Barrier")]
    public class Barrier : ActiveItemBase
    {
        public override void Use()
        {
            if (_currentCharge < _maxCharge) return;

            var playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
            if (playerHealth.SetProtected(true))
            {
                _currentCharge = 0;
                GameEvents.OnActiveUsed?.Invoke();
            }
        }
    }
}