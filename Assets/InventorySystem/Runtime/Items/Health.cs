using System;
using UnityEngine;

namespace InventorySystem.Items
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Min(1)] private int maxHealth = 100;
        [SerializeField, Min(0)] private int startingHealth = 100;

        public int MaxHealth => maxHealth;
        public int CurrentHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0;

        public event Action<int, int> HealthChanged;

        private void Awake()
        {
            maxHealth = Mathf.Max(1, maxHealth);
            CurrentHealth = Mathf.Clamp(startingHealth, 0, maxHealth);
            Debug.Log($"[{name}] Health initialized: {CurrentHealth}/{maxHealth}");
        }

        public int TakeDamage(int amount)
        {
            if (amount <= 0 || !IsAlive) return 0;
            return SetHealth(CurrentHealth - amount);
        }

        public int Heal(int amount)
        {
            if (amount <= 0 || !IsAlive) return 0;
            return SetHealth(CurrentHealth + amount);
        }

        private int SetHealth(int value)
        {
            int previous = CurrentHealth;
            CurrentHealth = Mathf.Clamp(value, 0, maxHealth);
            int difference = CurrentHealth - previous;

            if (difference != 0)
            {
                HealthChanged?.Invoke(CurrentHealth, maxHealth);
                Debug.Log($"[{name}] Health: {previous} -> {CurrentHealth}/{maxHealth}");
            }

            return Mathf.Abs(difference);
        }
    }
}
