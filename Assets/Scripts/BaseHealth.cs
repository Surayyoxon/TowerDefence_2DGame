using UnityEngine;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Health UI")]
    [SerializeField] private Slider healthSlider;

    private float currentHealth;
    private bool isDestroyed = false;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDestroyed || damage <= 0)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log("BASE HP: " + currentHealth + " / " + maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            DestroyBase();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider == null)
            return;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    private void DestroyBase()
    {
        if (isDestroyed)
            return;

        isDestroyed = true;

        Debug.Log("GAME OVER! BASE DESTROYED!");

        Time.timeScale = 0f;
    }
}
