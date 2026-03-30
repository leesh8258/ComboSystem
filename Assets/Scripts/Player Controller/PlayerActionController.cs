using TMPro;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputController inputController;
    [SerializeField] private PlayerWeaponEquipController weaponEquipController;
    [SerializeField] private PlayerAnimationController animationController;

    [Header("Debug UI")]
    [SerializeField] private TextMeshProUGUI currentWeaponNameText;
    [SerializeField] private TextMeshProUGUI currentComboStep;
    [SerializeField] private TextMeshProUGUI currentNextComboStep;
    
    private PlayerComboRuntimeState runtimeState = new PlayerComboRuntimeState();
    private bool waitingForAttackStateEnter;

    public bool IsAttacking => runtimeState.IsAttacking;

    private void Reset()
    {
        inputController = GetComponent<PlayerInputController>();
        weaponEquipController = GetComponent<PlayerWeaponEquipController>();
        animationController = GetComponent<PlayerAnimationController>();
    }

    private void OnEnable()
    {
        if (inputController != null)
        {
            inputController.OnAttackInput += HandleAttackInput;
        }

        if (weaponEquipController != null)
        {
            weaponEquipController.OnWeaponEquipped += HandleWeaponEquipped;
            weaponEquipController.OnWeaponCleared += HandleWeaponCleared;
        }
    }

    private void OnDisable()
    {
        if (inputController != null)
        {
            inputController.OnAttackInput -= HandleAttackInput;
        }

        if (weaponEquipController != null)
        {
            weaponEquipController.OnWeaponEquipped -= HandleWeaponEquipped;
            weaponEquipController.OnWeaponCleared -= HandleWeaponCleared;
        }
    }

    private void Update()
    {
        UpdateComboFlow();
        UpdateDebugRuntimeState();
    }

    public bool TryGetCurrentAttackStepData(out GameComboStepData stepData)
    {
        return GetCurrentStepData(out stepData);
    }

    private void HandleAttackInput(GameAttackInputType inputType)
    {
        if (inputType == GameAttackInputType.None)
        {
            return;
        }

        if (!runtimeState.IsAttacking)
        {
            TryStartCombo(inputType);
            return;
        }

        TryBufferNextStep(inputType);
    }

    private void HandleWeaponEquipped(GameWeaponSO weapon)
    {
        FinishCombo();
    }

    private void HandleWeaponCleared()
    {
        FinishCombo();
    }

    private void TryStartCombo(GameAttackInputType inputType)
    {
        GameComboData comboData = GetCurrentComboData();
        if (comboData == null) return;
        if (!comboData.TryGetEntryStepId(inputType, out string stepId)) return;
        if (!comboData.TryGetStep(stepId, out GameComboStepData stepData)) return;

        PlayStep(stepData);
    }

    private void TryBufferNextStep(GameAttackInputType inputType)
    {
        if (runtimeState.HasBufferedStep) return;

        GameComboData comboData = GetCurrentComboData();
        if (comboData == null) return;
        if (!comboData.TryGetStep(runtimeState.CurrentStepId, out GameComboStepData currentStepData) || !animationController.IsPlayingAttackState()) return;

        float normalizedTime = animationController.GetAttackNormalizedTime();
        if (!currentStepData.IsBufferWindowOpen(normalizedTime) || !currentStepData.TryGetNextStepId(inputType, out string nextStepId)) return;

        runtimeState.SetBufferedNextStep(nextStepId);
    }

    private void UpdateComboFlow()
    {
        if (!runtimeState.IsAttacking) return;

        if (animationController == null)
        {
            FinishCombo();
            return;
        }

        if (waitingForAttackStateEnter)
        {
            if (animationController.IsPlayingAttackState())
            {
                waitingForAttackStateEnter = false;
            }

            return;
        }

        if (!GetCurrentStepData(out GameComboStepData currentStepData))
        {
            FinishCombo();
            return;
        }

        if (!animationController.IsPlayingAttackState())
        {
            ResolveAttackFinished();
            return;
        }

        float normalizedTime = animationController.GetAttackNormalizedTime();

        if (runtimeState.HasBufferedStep && normalizedTime >= currentStepData.BufferCloseNormalizedTime)
        {
            ResolveBufferedStep();
            return;
        }

        if (normalizedTime >= 1f)
        {
            ResolveAttackFinished();
        }
    }

    private void ResolveBufferedStep()
    {
        GameComboData comboData = GetCurrentComboData();
        if (comboData == null)
        {
            FinishCombo();
            return;
        }

        string nextStepId = runtimeState.BufferedNextStepId;
        if (!comboData.TryGetStep(nextStepId, out GameComboStepData nextStepData))
        {
            FinishCombo();
            return;
        }

        PlayStep(nextStepData);
    }

    private void ResolveAttackFinished()
    {
        if (runtimeState.HasBufferedStep)
        {
            ResolveBufferedStep();
            return;
        }

        FinishCombo();
    }

    private void PlayStep(GameComboStepData stepData)
    {
        if (stepData == null || animationController == null || stepData.AnimationClip == null) return;

        runtimeState.BeginStep(stepData);
        waitingForAttackStateEnter = true;
        animationController.PlayAttackClip(stepData.AnimationClip);
    }

    private void FinishCombo()
    {
        runtimeState.Clear();
        waitingForAttackStateEnter = false;
    }

    private GameComboData GetCurrentComboData()
    {
        if (weaponEquipController == null) return null;

        GameWeaponSO currentWeapon = weaponEquipController.CurrentWeapon;
        if (currentWeapon == null) return null;

        return currentWeapon.ComboData;
    }

    private bool GetCurrentStepData(out GameComboStepData stepData)
    {
        stepData = null;

        GameComboData comboData = GetCurrentComboData();
        if (comboData == null) return false;
        if (string.IsNullOrEmpty(runtimeState.CurrentStepId)) return false;

        return comboData.TryGetStep(runtimeState.CurrentStepId, out stepData);
    }

    #region Debug
    private void UpdateDebugRuntimeState()
    {
        currentComboStep.text = $"Current Combo: {runtimeState.CurrentStepId}";
        currentNextComboStep.text = $"Next Combo: {runtimeState.BufferedNextStepId}";
        currentWeaponNameText.text = $"Weapon: {(weaponEquipController.HasWeapon ? weaponEquipController.CurrentWeapon.WeaponName : "None")}";
    }
    #endregion
}