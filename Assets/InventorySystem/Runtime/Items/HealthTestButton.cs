using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.Items
{
    public sealed class HealthTestButton : MonoBehaviour
    {
        [SerializeField] private Health targetHealth;
        [SerializeField] private Button damageButton;
        [SerializeField, Min(1)] private int damageAmount = 25;

        private void Awake()
        {
            if (damageButton == null)
                damageButton = GetComponent<Button>();

            if (damageButton == null)
            {
                Debug.LogError("HealthTestButton requires a Button reference.", this);
                enabled = false;
                return;
            }

            damageButton.onClick.AddListener(ApplyDamage);
        }

        private void OnDestroy()
        {
            if (damageButton != null)
                damageButton.onClick.RemoveListener(ApplyDamage);
        }

        private void ApplyDamage()
        {
            if (targetHealth == null)
            {
                Debug.LogError("Assign the player's Health component to HealthTestButton.", this);
                return;
            }

            int dealt = targetHealth.TakeDamage(damageAmount);
            Debug.Log($"Damage test: dealt {dealt}. Health: {targetHealth.CurrentHealth}/{targetHealth.MaxHealth}", this);
        }
    }
}
