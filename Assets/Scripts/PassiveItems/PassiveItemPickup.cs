using UnityEngine;

namespace PassiveItems {
    public class PassiveItemPickup : Pickup {
        [SerializeField] private PassiveItem item;

        protected override void Awake() {
            base.Awake();
            SpriteRenderer.sprite = item.Sprite;
        }

        protected override void OnPickup(Pickupper pickupper) {
            gameObject.SetActive(false);
            item.EquipItem(pickupper);
        }
    }
}
