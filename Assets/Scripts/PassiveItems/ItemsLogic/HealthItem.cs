using UnityEngine;

namespace PassiveItems
{
    [CreateAssetMenu(fileName = "Passive Item", menuName = "Scriptable Objects/Passive Items/Stats Increase/Health")]
    public class HealthItem : PassiveItem
    {
        [SerializeField] private int amount;
        [SerializeField] private bool isHeal;
        
        public override void EquipItem(Pickupper pickupper)
        {
            var health = pickupper.gameObject.GetComponent<Health.Health>();

            if (health == null) {
                Debug.LogError($"Object {pickupper.gameObject.name} missing Health component");
                return;
            }

            health.maxHealth += amount;
            if (isHeal) health.Heal(amount, false);
        }
    }
}