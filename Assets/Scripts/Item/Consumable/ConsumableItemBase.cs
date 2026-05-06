using UnityEngine;

namespace ConsumableItem
{
    /// <summary>
    /// 골드나 폭탄, 기타 소모품들을 대량 지급하는 아이템
    /// </summary>
    public abstract class ConsumableItemBase : ScriptableObject, ICollectible
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
            return null;
        }
    }
}