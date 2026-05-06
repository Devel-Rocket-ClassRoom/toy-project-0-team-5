using System;
using UnityEngine;

namespace ActiveItem
{
    [CreateAssetMenu(fileName = "Belial", menuName = "Item/Active/Belial")]
    public class Belial : ActiveItemBase
    {
        [SerializeField] private int _roomCount = 1;
        [SerializeField] private float _damageValue = 2f;

        public override void Use()
        {
            if (_currentCharge < _maxCharge) return;

            Modifier modifier = new(
                PlayerStatType.Damage,
                ModifierType.Additive,
                _damageValue,
                this
            );
            var playerStats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();
            playerStats.AddModifier(modifier);

            int count = _roomCount;
            Action expire = null;
            expire = () =>
            {
                count--;
                if (count == 0)
                {
                    playerStats.RemoveModifiersFromSource(this);
                    GameEvents.OnRoomClear -= expire;
                }
            };
            GameEvents.OnRoomClear += expire;

            _currentCharge = 0;
            GameEvents.OnActiveUsed?.Invoke();
        }
    }
}