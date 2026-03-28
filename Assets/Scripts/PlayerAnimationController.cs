using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameWeaponEquipController weaponEquipController;

    [Header("Attack States")]
    [SerializeField] private string[] attackStateNames = { "Attack_01", "Attack_02", "Attack_03" };
    [SerializeField] private AnimationClip[] attackPlaceholderClips = new AnimationClip[3];

    private AnimatorOverrideController runtimeOverrideController;
    private int[] attackStateHashes;

    #region Debug
    [Header("Debug")]
    [SerializeField] private bool enableDebugRuntimeState = true;
    [SerializeField] private bool debugIsPlayingAttackState;
    [SerializeField] private string debugCurrentAttackStateName;
    [SerializeField] private float debugCurrentAttackNormalizedTime;
    #endregion

    private void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        weaponEquipController = GetComponent<GameWeaponEquipController>();
    }

    private void Awake()
    {
        CacheAttackStateHashes();
        CreateRuntimeOverrideController();
    }

    private void OnEnable()
    {
        if (weaponEquipController != null)
        {
            weaponEquipController.OnWeaponEquipped += HandleWeaponEquipped;
        }
    }

    private void OnDisable()
    {
        if (weaponEquipController != null)
        {
            weaponEquipController.OnWeaponEquipped -= HandleWeaponEquipped;
        }
    }

    private void LateUpdate()
    {
        UpdateDebugRuntimeState();
    }

    public void ApplyWeaponComboOverride(GameWeaponSO weapon)
    {
        CreateRuntimeOverrideController();
        ResetAttackOverridesToPlaceholders();

        GameComboData comboData = weapon.ComboData;
        int maxCount = Mathf.Min(comboData.StepCount, attackPlaceholderClips.Length);

        for (int i = 0; i < maxCount; i++)
        {
            GameComboStepData stepData = comboData.GetStep(i);
            if (stepData == null)
            {
                continue;
            }

            AnimationClip placeholderClip = attackPlaceholderClips[i];
            AnimationClip overrideClip = stepData.AnimationClip;

            if (placeholderClip == null || overrideClip == null)
            {
                continue;
            }

            runtimeOverrideController[placeholderClip] = overrideClip;
        }
    }

    public void PlayAttackStep(int stepIndex)
    {
        if (animator == null) return;

        animator.Play(attackStateNames[stepIndex], 0, 0f);
    }

    public bool IsPlayingAttackState()
    {
        if (animator == null || attackStateHashes == null)
        {
            return false;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        int shortNameHash = stateInfo.shortNameHash;

        for (int i = 0; i < attackStateHashes.Length; i++)
        {
            if (shortNameHash == attackStateHashes[i])
            {
                return true;
            }
        }

        return false;
    }

    public float GetCurrentAttackNormalizedTime()
    {
        if (animator == null)
        {
            return 0f;
        }

        if (!IsPlayingAttackState())
        {
            return 0f;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime;
    }

    public int GetMaxAttackStepCount()
    {
        return attackStateNames != null ? attackStateNames.Length : 0;
    }

    private void HandleWeaponEquipped(GameWeaponSO weapon)
    {
        ApplyWeaponComboOverride(weapon);
    }

    private void CreateRuntimeOverrideController()
    {
        if (animator == null || runtimeOverrideController != null) return;

        RuntimeAnimatorController baseController = animator.runtimeAnimatorController;
        if (baseController == null) return;

        runtimeOverrideController = new AnimatorOverrideController(baseController);
        animator.runtimeAnimatorController = runtimeOverrideController;
    }

    private void ResetAttackOverridesToPlaceholders()
    {
        if (runtimeOverrideController == null || attackPlaceholderClips == null) return;


        for (int i = 0; i < attackPlaceholderClips.Length; i++)
        {
            AnimationClip placeholderClip = attackPlaceholderClips[i];
            if (placeholderClip == null)
            {
                continue;
            }

            runtimeOverrideController[placeholderClip] = placeholderClip;
        }
    }

    private void CacheAttackStateHashes()
    {
        if (attackStateNames == null)
        {
            attackStateHashes = new int[0];
            return;
        }

        attackStateHashes = new int[attackStateNames.Length];

        for (int i = 0; i < attackStateNames.Length; i++)
        {
            string stateName = attackStateNames[i];
            attackStateHashes[i] = string.IsNullOrEmpty(stateName) ? 0 : Animator.StringToHash(stateName);
        }
    }

    #region Debug
    private void UpdateDebugRuntimeState()
    {
        if (!enableDebugRuntimeState)
        {
            return;
        }

        if (animator == null)
        {
            debugIsPlayingAttackState = false;
            debugCurrentAttackStateName = string.Empty;
            debugCurrentAttackNormalizedTime = 0f;
            return;
        }

        debugIsPlayingAttackState = IsPlayingAttackState();

        if (!debugIsPlayingAttackState)
        {
            debugCurrentAttackStateName = string.Empty;
            debugCurrentAttackNormalizedTime = 0f;
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        debugCurrentAttackNormalizedTime = stateInfo.normalizedTime;
        debugCurrentAttackStateName = GetDebugAttackStateName(stateInfo.shortNameHash);
    }

    private string GetDebugAttackStateName(int shortNameHash)
    {
        if (attackStateHashes == null || attackStateNames == null)
        {
            return string.Empty;
        }

        for (int i = 0; i < attackStateHashes.Length; i++)
        {
            if (attackStateHashes[i] == shortNameHash)
            {
                return attackStateNames[i];
            }
        }

        return string.Empty;
    }
    #endregion
}