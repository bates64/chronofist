using System;
using Player_;
using UnityEngine;

namespace PassiveItems
{
    [CreateAssetMenu(fileName = "Passive Item", menuName = "Scriptable Objects/Passive Items/Stats Increase/Health")]
    public class HealthItem : PassiveItem
    {
        [SerializeField] private int amount;
        [SerializeField] private bool isHeal;
        
        public override void EquipItem(PlayerProperties player)
        {
            player.PassiveItems.Add(this);
            player.Health.maxHealth += amount;
            if (isHeal) player.Health.CurrentHealth += amount;
        }
    }
}