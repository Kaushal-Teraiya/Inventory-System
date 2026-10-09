namespace InventorySystem.Player
{
    public sealed class PlayerHealth
    {
        public int MaxHealth { get; }
        public int CurrentHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0;

        public event System.Action HealthChanged;

        public PlayerHealth(int maxHealth = 100)
        {
            MaxHealth = System.Math.Max(1, maxHealth);
            CurrentHealth = MaxHealth;
        }

        public int TakeDamage(int amount)
        {
            if (amount <= 0 || !IsAlive)
                return 0;

            int previous = CurrentHealth;
            CurrentHealth = System.Math.Max(0, CurrentHealth - amount);
            HealthChanged?.Invoke();
            return previous - CurrentHealth;
        }

        public int Heal(int amount)
        {
            if (amount <= 0 || !IsAlive)
                return 0;

            int previous = CurrentHealth;
            CurrentHealth = System.Math.Min(MaxHealth, CurrentHealth + amount);
            int restored = CurrentHealth - previous;

            if (restored > 0)
                HealthChanged?.Invoke();

            return restored;
        }
    }
}
