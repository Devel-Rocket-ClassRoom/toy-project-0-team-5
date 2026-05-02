using UnityEngine;

namespace ActiveItem
{
    [CreateAssetMenu(fileName = "Barrier", menuName = "Item/Active/Barrier")]
    public class Barrier : ActiveItemBase
    {
        public override void Use()
        {
            if (_currentCharge < _maxCharge) return;

            _currentCharge = 0;
            Debug.Log("Using Barrier");
        }
    }
}