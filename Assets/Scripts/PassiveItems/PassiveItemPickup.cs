using Player_.PlayerSFM;
using UnityEngine;

namespace PassiveItems
{
    public class PassiveItemPickup : Pickup
    {
        [SerializeField] private PassiveItem item;

        protected override void Awake()
        {
            base.Awake();
            SpriteRenderer.sprite = item.Sprite;
        }

        protected override void OnPickup(Player entity)
        {
            gameObject.SetActive(false);
            item.EquipItem(entity.Properties);
        }
    }
}