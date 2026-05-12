using UnityEngine;

namespace PassiveItem
{
    public abstract class PassiveItemBase : ScriptableObject, ICollectible
    {
        [SerializeField]
        protected int _id;

        [SerializeField]
        protected PlayerStatType _statType;

        [SerializeField]
        protected ModifierType _modifierType;

        [SerializeField]
        protected float _value;

        [SerializeField]
        protected Sprite _sprite;

        public Sprite Sprite => _sprite;

        public void Init() { }

        protected abstract void OnCollect(GameObject collector);

        public ICollectible Collect(GameObject collector)
        {
            OnCollect(collector);
            collector.GetComponent<PlayerPassiveItems>()?.Add(this);
            return null;
        }
    }
}
