
using UnityEngine;

public class StoneProjectile : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator stoneAnimator;
    [SerializeField] private string idleStateName = "Idle";
    [SerializeField] private string explodeTrigger = "Explode";

    [Header("Movement")]
    [SerializeField] private float hitDistance = 0.15f;

    private Transform target;

    private float damage;
    private float speed;

    private bool initialized;
    private bool hasHit;

    // PROJECTILE INITIALIZATION
    public void Initialize(
        Transform enemyTarget,
        float projectileDamage,
        float projectileSpeed)
    {
        target = enemyTarget;
        damage = projectileDamage;
        speed = projectileSpeed;

        initialized = true;
        hasHit = false;

        if (stoneAnimator != null)
        {
            stoneAnimator.Play(idleStateName, 0, 0f);
        }
    }

    private void Update()
    {
        if (!initialized || hasHit)
            return;

        // Enemy yo'qolgan bo'lsa
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Toshning enemy tomon yo'nalishi
        Vector3 direction = target.position - transform.position;

        // Tosh harakati
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // Toshni enemy tomonga burish
        if (direction.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(
                direction.y,
                direction.x
            ) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(
                0f,
                0f,
                angle
            );
        }

        // Enemyga yetib borish
        float distance = Vector2.Distance(
            transform.position,
            target.position
        );

        if (distance <= hitDistance)
        {
            HitTarget();
        }
    }

    // ENEMYGA TEGISH
    private void HitTarget()
    {
        if (hasHit)
            return;

        hasHit = true;

        // Enemyga damage berish
        IDamageable enemyHealth =
            target.GetComponentInParent<IDamageable>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning(
                "Enemy da IDamageable topilmadi!"
            );
        }

        // Toshning Explode animatsiyasi
        if (stoneAnimator != null)
        {
            stoneAnimator.SetTrigger(explodeTrigger);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ANIMATION EVENT
    // Explode animatsiyasining oxirgi frame'ida chaqiriladi
    public void OnExplodeAnimationFinished()
    {
        Destroy(gameObject);
    }
}
