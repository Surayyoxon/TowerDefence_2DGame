using UnityEngine;
using UnityEngine.UI;

public class BaseHealth : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Health UI (ixtiyoriy - asosiy bar UIManager'da)")]
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

        currentHealth = Mathf.Max(0f, currentHealth - damage);

        UpdateHealthUI();

        if (currentHealth <= 0f)
            DestroyBase();
    }

    private void UpdateHealthUI()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.SetBaseHealth(currentHealth, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void DestroyBase()
    {
        if (isDestroyed)
            return;

        isDestroyed = true;

        // Time.timeScale = 0 ni UIManager o'zi qo'yadi
        if (UIManager.Instance != null)
            UIManager.Instance.ShowDiedPanel();
        else
            Time.timeScale = 0f;
    }
}