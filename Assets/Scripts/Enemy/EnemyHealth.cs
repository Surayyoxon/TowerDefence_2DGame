
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Death Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathTrigger = "Die";

    private float currentHealth;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log("Enemy HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // Stop enemy movement
        EnemyMovement movement =
            GetComponent<EnemyMovement>();

        if (movement != null)
            movement.enabled = false;

        // Disable collider
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger(deathTrigger);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called by Animation Event
    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
