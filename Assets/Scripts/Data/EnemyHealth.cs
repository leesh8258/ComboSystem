using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private const string HIT_TRIGGER = "Hit";
    private const string DEAD_TRIGGER = "Dead";

    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;

    [Header("References")]
    [SerializeField] private Animator animator;

    [SerializeField] private float currentHealth;
    private bool isDead;

    private void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (damage <= 0f) return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            HandleDeath();
            return;
        }

        Debug.Log("??");
        PlayHitReaction();
    }

    private void PlayHitReaction()
    {
        if (animator == null) return;
        animator.SetTrigger(HIT_TRIGGER);
    }

    private void HandleDeath()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger(DEAD_TRIGGER);
        }
    }
}