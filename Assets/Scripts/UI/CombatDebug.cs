using TMPro;
using UnityEngine;

public class CombatDebug : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerActionController actionController;
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private PlayerWeaponEquipController weaponEquipController;
    [SerializeField] private PlayerDamageController damageController;

    [Header("Debug UI")]
    [SerializeField] private TextMeshProUGUI currentWeaponNameText;
    [SerializeField] private TextMeshProUGUI currentComboStepText;
    [SerializeField] private TextMeshProUGUI currentNextComboStepText;
    [SerializeField] private TextMeshProUGUI currentAttackClipNameText;
    [SerializeField] private TextMeshProUGUI currentDamageText;

    private void Reset()
    {
        actionController = GetComponent<PlayerActionController>();
        animationController = GetComponent<PlayerAnimationController>();
        weaponEquipController = GetComponent<PlayerWeaponEquipController>();
        damageController = GetComponent<PlayerDamageController>();
    }

    private void LateUpdate()
    {
        RefreshDebugUI();
    }

    private void RefreshDebugUI()
    {
        UpdateWeaponText();
        UpdateComboTexts();
        UpdateAttackClipText();
        UpdateDamageText();
    }

    private void UpdateWeaponText()
    {
        if (currentWeaponNameText == null) return;

        string weaponName = "None";

        if (weaponEquipController != null && weaponEquipController.HasWeapon && weaponEquipController.CurrentWeapon != null)
        {
            weaponName = weaponEquipController.CurrentWeapon.WeaponName;
        }

        currentWeaponNameText.text = $"Weapon: {weaponName}";
    }

    private void UpdateComboTexts()
    {
        if (actionController != null)
        {
            if (currentComboStepText != null)
            {
                currentComboStepText.text = $"Current Combo: {actionController.CurrentComboStepId}";
            }

            if (currentNextComboStepText != null)
            {
                currentNextComboStepText.text = $"Next Combo: {actionController.BufferedNextComboStepId}";
            }

            return;
        }

        if (currentComboStepText != null)
        {
            currentComboStepText.text = "Current Combo: ";
        }

        if (currentNextComboStepText != null)
        {
            currentNextComboStepText.text = "Next Combo: ";
        }
    }

    private void UpdateAttackClipText()
    {
        if (currentAttackClipNameText == null) return;

        string clipName = string.Empty;

        if (animationController != null)
        {
            AnimationClip currentClip = animationController.GetCurrentAttackClip();
            if (currentClip != null)
            {
                clipName = currentClip.name;
            }
        }

        currentAttackClipNameText.text = $"Current Animation Clip: {clipName}";
    }

    private void UpdateDamageText()
    {
        if (currentDamageText == null) return;

        float damage = 0f;

        if (damageController != null)
        {
            damage = damageController.GetCurrentDamage();
        }

        currentDamageText.text = $"Damage: {damage:0.##}";
    }
}