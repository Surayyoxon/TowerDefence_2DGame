
using UnityEngine;

public class CatapultTower : MonoBehaviour
{
    [Header("Enemy Detection")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Catapult Animation")]
    [SerializeField] private Animator catapultAnimator;
    [SerializeField] private string attackTrigger = "Attack";

    [Header("Loaded Stone")]
    [SerializeField] private GameObject loadedStone;
    [SerializeField] private Transform firePoint;

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileDamage = 20f;
    [SerializeField] private float projectileSpeed = 7f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;

    private Transform currentTarget;
    private bool isAttacking;
    private float nextAttackTime;

    private void Start()
    {
        if (loadedStone != null)
            loadedStone.SetActive(false);
    }

    private void Update()
    {
        if (isAttacking)
            return;

        currentTarget = FindTarget();

        if (currentTarget == null)
            return;

        if (Time.time >= nextAttackTime)
        {
            StartAttack();
        }
    }

    // ENEMY DETECTION
    private Transform FindTarget()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange,
            enemyLayer
        );

        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            if (enemy == null)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    // ATTACK START
    private void StartAttack()
    {
        if (currentTarget == null)
            return;

        isAttacking = true;

        // Tosh katapultada paydo bo'ladi
        if (loadedStone != null)
            loadedStone.SetActive(true);

        // Attack animatsiyasi
        catapultAnimator.SetTrigger(attackTrigger);
    }

    // ANIMATION EVENT
    // Attack animatsiyasining oxirgi frame'ida chaqiriladi
    public void OnAttackAnimationFinished()
    {
        // Enemy hali mavjudligini tekshirish
        if (currentTarget != null && projectilePrefab != null)
        {
            GameObject projectile = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity
            );

            StoneProjectile stone =
                projectile.GetComponent<StoneProjectile>();

            if (stone != null)
            {
                stone.Initialize(
                    currentTarget,
                    projectileDamage,
                    projectileSpeed
                );
            }
        }

        // Katapultadagi tosh yo'qoladi
        if (loadedStone != null)
            loadedStone.SetActive(false);

        // Keyingi hujumga tayyorlanish
        nextAttackTime = Time.time + attackCooldown;
        isAttacking = false;
    }

    // DETECTION RADIUS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}
