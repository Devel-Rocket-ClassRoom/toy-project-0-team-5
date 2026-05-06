using System;
using UnityEngine;

namespace ActiveItem
{
    [CreateAssetMenu(fileName = "Bible", menuName = "Item/Active/Bible")]
    public class Bible : ActiveItemBase
    {
        [SerializeField] private int _roomCount = 1;

        public override void Use()
        {
            if (_currentCharge < _maxCharge) return;

            var player = GameObject.FindWithTag("Player");
            var playerCollider = player.GetComponent<Collider>();
            playerCollider.isTrigger = true;

            int count = _roomCount;
            Action expire = null;
            expire = () =>
            {
                count--;
                if (count == 0)
                {
                    if (playerCollider != null) playerCollider.isTrigger = false;
                    GameEvents.OnRoomClear -= expire;
                }
            };
            GameEvents.OnRoomClear += expire;

            _currentCharge = 0;
            GameEvents.OnActiveUsed?.Invoke();
        }
    }
}