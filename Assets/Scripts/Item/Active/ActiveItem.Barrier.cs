using UnityEngine;

namespace ActiveItem
{
    [CreateAssetMenu(fileName = "Barrier", menuName = "Item/Active/Barrier")]
    public class Barrier : ActiveItemBase
    {
        public override void Use()
        {
            if (_currentCharge < _maxCharge) return;

            Debug.Log("베리어 사용!");
            var playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
            playerHealth.SetProtected(true);
            _currentCharge = 0;
        }
    }
}