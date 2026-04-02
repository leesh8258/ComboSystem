using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerWeaponEquipController weaponEquipController;

    [Header("Attack State")]
    [SerializeField] private string attackStateName = "Attack";
    [SerializeField] private AnimationClip attackPlaceholderClip;

    private AnimatorOverrideController runtimeOverrideController;
    private int attackStateHash;
    private AnimationClip currentAttackClip;

    private void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        weaponEquipController = GetComponent<PlayerWeaponEquipController>();
    }

    private void Awake()
    {
        attackStateHash = Animator.StringToHash(attackStateName);
        CreateRuntimeOverrideController();
    }

    private void OnEnable()
    {
        if (weaponEquipController != null)
        {
            weaponEquipController.OnWeaponEquipped += HandleWeaponEquipped;
            weaponEquipController.OnWeaponCleared += HandleWeaponCleared;
        }
    }

    private void OnDisable()
    {
        if (weaponEquipController != null)
        {
            weaponEquipController.OnWeaponEquipped -= HandleWeaponEquipped;
            weaponEquipController.OnWeaponCleared -= HandleWeaponCleared;
        }
    }

    public void PlayAttackClip(AnimationClip clip)
    {
        if (!CanPlayAttackClip(clip)) return;

        ApplyAttackClipOverride(clip);
        animator.Play(attackStateName, 0, 0f);
    }

    public bool IsPlayingAttackState()
    {
        if (animator == null) return false;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.shortNameHash == attackStateHash;
    }

    public float GetAttackNormalizedTime()
    {
        if (!IsPlayingAttackState()) return 0f;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime;
    }

    public bool IsAttackFinished()
    {
        if (!IsPlayingAttackState()) return false;

        return GetAttackNormalizedTime() >= 1f;
    }

    public AnimationClip GetCurrentAttackClip()
    {
        return currentAttackClip;
    }

    private void HandleWeaponEquipped(GameWeaponSO weapon)
    {
        ResetAttackOverrideToPlaceholder();
    }

    private void HandleWeaponCleared()
    {
        ResetAttackOverrideToPlaceholder();
    }

    private bool CanPlayAttackClip(AnimationClip clip)
    {
        if (animator == null || attackPlaceholderClip == null || clip == null) return false;

        CreateRuntimeOverrideController();
        return runtimeOverrideController != null;
    }

    private void CreateRuntimeOverrideController()
    {
        if (animator == null || runtimeOverrideController != null) return;

        RuntimeAnimatorController baseController = animator.runtimeAnimatorController;
        if (baseController == null) return;

        runtimeOverrideController = new AnimatorOverrideController(baseController);
        animator.runtimeAnimatorController = runtimeOverrideController;
    }

    private void ApplyAttackClipOverride(AnimationClip clip)
    {
        runtimeOverrideController[attackPlaceholderClip] = clip;
        currentAttackClip = clip;
    }

    private void ResetAttackOverrideToPlaceholder()
    {
        if (runtimeOverrideController == null || attackPlaceholderClip == null) return;

        runtimeOverrideController[attackPlaceholderClip] = attackPlaceholderClip;
        currentAttackClip = null;
    }
}