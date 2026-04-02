using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Collider hitTrigger;

    [Header("Runtime State")]
    [SerializeField] private bool isHitboxActive;
    [SerializeField] private int currentAttackSequenceId = -1;

    private readonly HashSet<EnemyHealth> hitTargets = new HashSet<EnemyHealth>();

    private PlayerDamageController damageController;

    public bool IsHitboxActive => isHitboxActive;

    private void Reset()
    {
        hitTrigger = GetComponent<Collider>();

        if (hitTrigger != null)
        {
            hitTrigger.isTrigger = true;
        }
    }

    private void Awake()
    {
        if (hitTrigger != null)
        {
            hitTrigger.isTrigger = true;
        }
    }

    public void Setup(PlayerDamageController owner)
    {
        damageController = owner;
        SetHitboxActive(false);
    }

    public void BeginAttackSequence(int attackSequenceId)
    {
        if (currentAttackSequenceId == attackSequenceId) return;

        currentAttackSequenceId = attackSequenceId;
        hitTargets.Clear();
    }

    public void SetHitboxActive(bool isActive)
    {
        isHitboxActive = isActive;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryApplyDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryApplyDamage(other);
    }

    private void TryApplyDamage(Collider other)
    {
        if (!isHitboxActive) return;
        if (damageController == null) return;
        if (other == null) return;
        if ((enemyLayers.value & (1 << other.gameObject.layer)) == 0) return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy == null) return;
        if (hitTargets.Contains(enemy)) return;

        float damage = damageController.GetCurrentDamage();
        if (damage <= 0f) return;

        hitTargets.Add(enemy);
        enemy.TakeDamage(damage);
    }
}