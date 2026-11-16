using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Reward Settings")]
    [SerializeField] private int coinReward = 20;

    [Header("Health Bar")]
    [SerializeField] private Image healthFill;

    [Header("Position")]
    [SerializeField]
    private Vector3 healthBarOffset =
        new Vector3(0f, 1f, 0f);

    [Header("Death Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathTrigger = "Die";

    private float currentHealth;
    private bool isDead;

    private RectTransform healthBar;
    private Canvas canvas;
    private Camera mainCamera;

    private bool cameraWarned = false;

    private void Awake()
    {
        // Spawner SetHealthBar'ni Start'dan oldin chaqirishi mumkin,
        // shuning uchun HP shu yerda to'ldiriladi
        currentHealth = maxHealth;
    }

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (canvas == null)
            canvas = FindFirstObjectByType<Canvas>();

        mainCamera = Camera.main;

        if (mainCamera == null)
            Debug.LogError("Main Camera topilmadi! Kameraga 'MainCamera' tagini qo'ying.");

        UpdateHealthBar();
    }

    private void LateUpdate()
    {
        UpdateHealthBarPosition();
    }

    private void UpdateHealthBarPosition()
    {
        if (healthBar == null || isDead)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (canvas == null)
            canvas = healthBar.GetComponentInParent<Canvas>();

        if (mainCamera == null || canvas == null)
        {
            if (!cameraWarned)
            {
                cameraWarned = true;
                Debug.LogWarning(name + ": Camera yoki Canvas topilmadi, HP bar joylashmaydi!");
            }
            return;
        }

        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(
                transform.position + healthBarOffset
            );

        if (screenPosition.z <= 0)
        {
            healthBar.gameObject.SetActive(false);
            return;
        }

        healthBar.gameObject.SetActive(true);

        // Joylashuv barning ota obyektiga (konteynerga) nisbatan hisoblanadi
        RectTransform parentRect = healthBar.parent as RectTransform;

        if (parentRect == null)
            return;

        Camera uiCamera =
            canvas.renderMode == RenderMode.ScreenSpaceOverlay
            ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            screenPosition,
            uiCamera,
            out Vector2 localPoint))
        {
            healthBar.anchoredPosition = localPoint;
        }
    }

    // EnemySpawner chaqiradi
    public void SetHealthBar(
        RectTransform bar,
        Image fill)
    {
        healthBar = bar;
        healthFill = fill;

        if (healthBar != null)
        {
            canvas = healthBar.GetComponentInParent<Canvas>();
            healthBar.gameObject.SetActive(true);
        }

        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }

    private void UpdateHealthBar()
    {
        if (healthFill != null && maxHealth > 0)
        {
            healthFill.fillAmount =
                Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        EnemyMovement movement =
            GetComponent<EnemyMovement>();

        if (movement != null)
            movement.enabled = false;

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
            col.enabled = false;

        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        if (animator != null)
            animator.SetTrigger(deathTrigger);
        else
            DestroyEnemy();
    }

    public void DestroyEnemy()
    {
        if (!isDead)
            return;

        if (UIManager.Instance != null)
            UIManager.Instance.AddCoins(coinReward);

        Destroy(gameObject);
    }

    // Enemy yo'qolganda HP bar ham Canvas'dan o'chiriladi
    private void OnDestroy()
    {
        if (healthBar != null)
            Destroy(healthBar.gameObject);
    }
}