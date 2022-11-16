using System;
using UnityEngine;

namespace PassiveItems {
    [Serializable]
    public abstract class PassiveItem : ScriptableObject {
        [SerializeField] private string itemName;
        [SerializeField] private Sprite sprite;
        [SerializeField] private string description;

        public string Name => itemName;
        public Sprite Sprite => sprite;
        public string Description => description;

        public abstract void EquipItem(Pickupper pickupper);
    }
}
