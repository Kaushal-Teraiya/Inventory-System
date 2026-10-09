using UnityEngine;

namespace InventorySystem.Items
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxHealth = 100;
        public int MaxHealth => maxHealth;
        public int CurrentHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0;

        private void Awake() => CurrentHealth = Mathf.Max(1, maxHealth);

        public int TakeDamage(int amount)
        {
            if (amount <= 0 || !IsAlive) return 0;
            int previous = CurrentHealth;
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            return previous - CurrentHealth;
        }

        public int Heal(int amount)
        {
            if (amount <= 0 || !IsAlive) return 0;
            int previous = CurrentHealth;
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            return CurrentHealth - previous;
        }
    }
}
