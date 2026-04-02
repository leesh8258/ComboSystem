using TMPro;
using UnityEngine;

public class CombatDebug : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController inputController;
    [SerializeField] private PlayerActionController actionController;
    [SerializeField] private PlayerAnimationController animationController;
    [SerializeField] private PlayerWeaponEquipController weaponEquipController;
    [SerializeField] private PlayerDamageController damageController;

    [Header("Weapon Debug UI")]
    [SerializeField] private TextMeshProUGUI currentWeaponNameText;
    [SerializeField] private TextMeshProUGUI hasWeaponText;

    [Header("Combo Debug UI")]
    [SerializeField] private TextMeshProUGUI currentComboStepText;
    [SerializeField] private TextMeshProUGUI currentNextComboStepText;
    [SerializeField] private TextMeshProUGUI hasBufferedStepText;
    [SerializeField] private TextMeshProUGUI isAttackingText;
    [SerializeField] private TextMeshProUGUI waitingForAttackStateEnterText;

    [Header("Animation Debug UI")]
    [SerializeField] private TextMeshProUGUI currentAttackClipNameText;
    [SerializeField] private TextMeshProUGUI isPlayingAttackStateText;
    [SerializeField] private TextMeshProUGUI currentNormalizedTimeText;

    [Header("Damage Debug UI")]
    [SerializeField] private TextMeshProUGUI currentDamageText;
    [SerializeField] private TextMeshProUGUI currentAttackSequenceText;
    [SerializeField] private TextMeshProUGUI hitboxActiveText;

    [Header("Input Debug UI")]
    [SerializeField] private TextMeshProUGUI lastInputText;
    [SerializeField] private TextMeshProUGUI moveDirectionText;

    private string currentInputLabel = "None";

    private void Reset()
    {
        inputController = GetComponent<PlayerInputController>();
        actionController = GetComponent<PlayerActionController>();
        animationController = GetComponent<PlayerAnimationController>();
        weaponEquipController = GetComponent<PlayerWeaponEquipController>();
        damageController = GetComponent<PlayerDamageController>();
    }

    private void OnEnable()
    {
        if (inputController != null)
        {
            inputController.OnAttackInput += HandleAttackInput;
            inputController.OnWeaponSlotPressed += HandleWeaponSlotPressed;
        }
    }

    private void OnDisable()
    {
        if (inputController != null)
        {
            inputController.OnAttackInput -= HandleAttackInput;
            inputController.OnWeaponSlotPressed -= HandleWeaponSlotPressed;
        }
    }

    private void LateUpdate()
    {
        RefreshDebugUI();
    }

    private void HandleAttackInput(GameAttackInputType inputType)
    {
        switch (inputType)
        {
            case GameAttackInputType.LeftClick:
                currentInputLabel = "Left Click";
                break;

            case GameAttackInputType.RightClick:
                currentInputLabel = "Right Click";
                break;

            default:
                currentInputLabel = inputType.ToString();
                break;
        }
    }

    private void HandleWeaponSlotPressed(int slotIndex)
    {
        currentInputLabel = $"Weapon Slot {slotIndex + 1}";
    }

    private void RefreshDebugUI()
    {
        UpdateWeaponTexts();
        UpdateComboTexts();
        UpdateAnimationTexts();
        UpdateDamageTexts();
        UpdateInputTexts();
    }

    private void UpdateWeaponTexts()
    {
        string weaponName = "None";
        bool hasWeapon = false;

        if (weaponEquipController != null && weaponEquipController.HasWeapon && weaponEquipController.CurrentWeapon != null)
        {
            hasWeapon = true;
            weaponName = weaponEquipController.CurrentWeapon.WeaponName;
        }

        if (currentWeaponNameText != null)
        {
            currentWeaponNameText.text = $"Weapon: {weaponName}";
        }

        if (hasWeaponText != null)
        {
            hasWeaponText.text = $"Has Weapon: {hasWeapon}";
        }
    }

    private void UpdateComboTexts()
    {
        string currentComboStepId = string.Empty;
        string bufferedNextStepId = string.Empty;
        bool isAttacking = false;
        bool hasBufferedStep = false;
        bool isWaitingForAttackStateEnter = false;

        if (actionController != null)
        {
            currentComboStepId = actionController.CurrentComboStepId;
            bufferedNextStepId = actionController.BufferedNextComboStepId;
            isAttacking = actionController.IsAttacking;
            hasBufferedStep = actionController.HasBufferedStep;
            isWaitingForAttackStateEnter = actionController.IsWaitingForAttackStateEnter;
        }

        if (currentComboStepText != null)
        {
            currentComboStepText.text = $"Current Combo: {currentComboStepId}";
        }

        if (currentNextComboStepText != null)
        {
            currentNextComboStepText.text = $"Next Combo: {bufferedNextStepId}";
        }

        if (hasBufferedStepText != null)
        {
            hasBufferedStepText.text = $"Has Buffered Step: {hasBufferedStep}";
        }

        if (isAttackingText != null)
        {
            isAttackingText.text = $"Is Attacking: {isAttacking}";
        }

        if (waitingForAttackStateEnterText != null)
        {
            waitingForAttackStateEnterText.text = $"Waiting For Attack State Enter: {isWaitingForAttackStateEnter}";
        }
    }

    private void UpdateAnimationTexts()
    {
        string clipName = string.Empty;
        bool isPlayingAttackState = false;
        float normalizedTime = 0f;

        if (animationController != null)
        {
            AnimationClip currentClip = animationController.GetCurrentAttackClip();
            if (currentClip != null)
            {
                clipName = currentClip.name;
            }

            isPlayingAttackState = animationController.IsPlayingAttackState();
            normalizedTime = animationController.GetAttackNormalizedTime();
        }

        if (currentAttackClipNameText != null)
        {
            currentAttackClipNameText.text = $"Current Animation Clip: {clipName}";
        }

        if (isPlayingAttackStateText != null)
        {
            isPlayingAttackStateText.text = $"Is Playing Attack State: {isPlayingAttackState}";
        }

        if (currentNormalizedTimeText != null)
        {
            currentNormalizedTimeText.text = $"Normalized Time: {normalizedTime:0.000}";
        }
    }

    private void UpdateDamageTexts()
    {
        float damage = 0f;
        int attackSequenceId = 0;
        bool isHitboxActive = false;

        if (damageController != null)
        {
            damage = damageController.GetCurrentDamage();
            attackSequenceId = damageController.CurrentAttackSequenceId;
            isHitboxActive = damageController.IsHitboxActive;
        }

        if (currentDamageText != null)
        {
            currentDamageText.text = $"Damage: {damage:0.##}";
        }

        if (currentAttackSequenceText != null)
        {
            currentAttackSequenceText.text = $"Attack Sequence Id: {attackSequenceId}";
        }

        if (hitboxActiveText != null)
        {
            hitboxActiveText.text = $"Hitbox Active: {isHitboxActive}";
        }
    }

    private void UpdateInputTexts()
    {
        float moveDirection = 0f;

        if (inputController != null)
        {
            moveDirection = inputController.MoveDirection;
        }

        if (lastInputText != null)
        {
            lastInputText.text = $"Last Input: {currentInputLabel}";
        }

        if (moveDirectionText != null)
        {
            moveDirectionText.text = $"Move Direction: {moveDirection:0.##}";
        }
    }
}