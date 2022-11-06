using System;
using Player_;
using Player_.PlayerSFM;
using UnityEngine;


namespace PassiveItems
{
    [Serializable]
    public abstract class PassiveItem : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private Sprite sprite;
        [SerializeField] private string description;

        public string Name => itemName;
        public Sprite Sprite => sprite;
        public string Description => description;
        
        abstract public void EquipItem(PlayerProperties player);
    }
}