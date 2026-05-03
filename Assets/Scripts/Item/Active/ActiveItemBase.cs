using NUnit.Framework;
using UnityEngine;

namespace ActiveItem
{
    public enum ChargeType
    {
        OnRoomClear,
        OnHit,
    }

    public abstract class ActiveItemBase : ScriptableObject, ICollectible
    {
        [SerializeField] protected int _id;
        [SerializeField] protected ChargeType _chargeType;
        [SerializeField] protected int _maxCharge;
        [SerializeField] protected Sprite _sprite;

        protected int _currentCharge;

        public Sprite Sprite => _sprite;

        public abstract void Use();

        public void Init()
        {
            _currentCharge = 0;
        }

        private void OnEquip()
        {
            switch (_chargeType)
            {
                case ChargeType.OnRoomClear:
                    GameEvents.OnRoomClear += OnCharge;
                    break;
                case ChargeType.OnHit:
                    GameEvents.OnPlayerHit += OnCharge;
                    break;
            }
        }

        private void OnUnequip()
        {
            switch (_chargeType)
            {
                case ChargeType.OnRoomClear:
                    GameEvents.OnRoomClear -= OnCharge;
                    break;
                case ChargeType.OnHit:
                    GameEvents.OnPlayerHit -= OnCharge;
                    break;
            }
        }

        private void OnCharge()
        {
            _currentCharge = Mathf.Min(_currentCharge + 1, _maxCharge);
        }

        public ICollectible Collect(GameObject collector)
        {
            // TODO: 캐릭터 필드에 아이템 상태에 따라 분기
            // if null => set this => return null
            // else => swap this <-> other => return other
            //      => this.OnEquip(), other.OnUnequip();
            return null;
        }
    }
}