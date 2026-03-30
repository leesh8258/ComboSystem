using UnityEngine;

public class PlayerDamageController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerWeaponEquipController weaponEquipController;
    [SerializeField] private PlayerActionController actionController;

    private WeaponHitbox activeHitbox;
    private int currentAttackSequenceId;

    private void Reset()
    {
        weaponEquipController = GetComponent<PlayerWeaponEquipController>();
        actionController = GetComponent<PlayerActionController>();
    }

    private void OnEnable()
    {
        if (weaponEquipController != null)
        {
            weaponEquipController.OnWeaponEquipped += HandleWeaponEquipped;
            weaponEquipController.OnWeaponCleared += HandleWeaponCleared;
        }

        RefreshActiveHitbox();
        SetHitboxEnabled(false);
    }

    private void OnDisable()
    {
        if (weaponEquipController != null)
        {
            weaponEquipController.OnWeaponEquipped -= HandleWeaponEquipped;
            weaponEquipController.OnWeaponCleared -= HandleWeaponCleared;
        }

        SetHitboxEnabled(false);
    }

    public void EnableAttackHitbox()
    {
        currentAttackSequenceId++;
        RefreshActiveHitbox();

        if (activeHitbox == null) return;

        activeHitbox.BeginAttackSequence(currentAttackSequenceId);
        SetHitboxEnabled(true);
    }

    public void DisableAttackHitbox()
    {
        SetHitboxEnabled(false);
    }

    public float GetCurrentDamage()
    {
        if (weaponEquipController == null || !weaponEquipController.HasWeapon)
        {
            return 0f;
        }

        float baseDamage = weaponEquipController.CurrentWeapon.BaseDamage;
        float multiplier = 1f;

        if (actionController != null && actionController.TryGetCurrentAttackStepData(out GameComboStepData stepData) && stepData != null)
        {
            multiplier = stepData.DamageMultiplier;
        }

        return baseDamage * multiplier;
    }

    private void HandleWeaponEquipped(GameWeaponSO weapon)
    {
        RefreshActiveHitbox();
        SetHitboxEnabled(false);
    }

    private void HandleWeaponCleared()
    {
        activeHitbox = null;
    }

    private void RefreshActiveHitbox()
    {
        if (weaponEquipController == null || weaponEquipController.CurrentWeaponInstance == null)
        {
            activeHitbox = null;
            return;
        }

        activeHitbox = weaponEquipController.CurrentWeaponInstance.GetComponent<WeaponHitbox>();

        if (activeHitbox != null)
        {
            activeHitbox.Setup(this);
        }
    }

    private void SetHitboxEnabled(bool isEnabled)
    {
        if (activeHitbox == null) return;
        activeHitbox.SetHitboxActive(isEnabled);
    }
}
