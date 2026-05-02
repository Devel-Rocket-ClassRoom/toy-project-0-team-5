using UnityEngine;

namespace ConsumableItem
{
    /// <summary>
    /// 골드나 폭탄, 기타 소모품들을 대량 지급하는 아이템
    /// </summary>
    public abstract class ConsumableItemBase : ICollectible
    {
        protected int _id;
        protected Sprite _sprite;

        public Sprite Sprite => _sprite;

        public ConsumableItemBase(int id)
        {
            _id = id;
        }

        public void Init()
        {
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