using System;
using UnityEngine;
using Physics;

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
        
        abstract public void EquipItem(Pickupper pickupper);
    }
}