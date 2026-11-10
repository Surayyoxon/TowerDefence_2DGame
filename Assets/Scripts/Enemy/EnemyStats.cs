using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Statistics")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int baseDamage = 1;
    [SerializeField] private int reward = 10;

    public float MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public int BaseDamage => baseDamage;
    public int Reward => reward;
}
