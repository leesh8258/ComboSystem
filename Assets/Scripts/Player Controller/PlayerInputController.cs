using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [Header("Move Input Actions")]
    [SerializeField] private InputActionReference moveForwardAction;
    [SerializeField] private InputActionReference moveBackwardAction;

    [Header("Combat Input Actions")]
    [SerializeField] private InputActionReference attackLeftAction;
    [SerializeField] private InputActionReference attackRightAction;

    [Header("Weapon Slot Input Actions")]
    [SerializeField] private List<InputActionReference> weaponSlotActions;

    private readonly Dictionary<InputAction, int> weaponSlotActionMap = new Dictionary<InputAction, int>();

    private bool isForwardPressed;
    private bool isBackwardPressed;

    public float MoveDirection { get; private set; }

    public event Action<GameAttackInputType> OnAttackInput;
    public event Action<int> OnWeaponSlotPressed;

    private void OnEnable()
    {
        BindMoveForwardAction();
        BindMoveBackwardAction();
        BindAttackLeftAction();
        BindAttackRightAction();
        BindWeaponSlotActions();

        RefreshMoveDirection();
    }

    private void OnDisable()
    {
        UnbindMoveForwardAction();
        UnbindMoveBackwardAction();
        UnbindAttackLeftAction();
        UnbindAttackRightAction();
        UnbindWeaponSlotActions();

        isForwardPressed = false;
        isBackwardPressed = false;
        MoveDirection = 0f;
    }

    private void RefreshMoveDirection()
    {
        MoveDirection = 0f;

        if (isForwardPressed)
        {
            MoveDirection += 1f;
        }

        if (isBackwardPressed)
        {
            MoveDirection -= 1f;
        }
    }

    #region Bind and Unbind Methods
    private void BindMoveForwardAction()
    {
        if (moveForwardAction == null || moveForwardAction.action == null) return;
 
        moveForwardAction.action.Enable();
        moveForwardAction.action.performed += OnForwardPerformed;
        moveForwardAction.action.canceled += OnForwardCanceled;
    }

    private void BindMoveBackwardAction()
    {
        if (moveBackwardAction == null || moveBackwardAction.action == null) return;

        moveBackwardAction.action.Enable();
        moveBackwardAction.action.performed += OnBackwardPerformed;
        moveBackwardAction.action.canceled += OnBackwardCanceled;
    }

    private void BindAttackLeftAction()
    {
        if (attackLeftAction == null || attackLeftAction.action == null) return;

        attackLeftAction.action.Enable();
        attackLeftAction.action.performed += OnAttackLeftPerformed;
    }

    private void BindAttackRightAction()
    {
        if (attackRightAction == null || attackRightAction.action == null) return;

        attackRightAction.action.Enable();
        attackRightAction.action.performed += OnAttackRightPerformed;
    }

    private void BindWeaponSlotActions()
    {
        weaponSlotActionMap.Clear();

        for (int i = 0; i < weaponSlotActions.Count; i++)
        {
            if (weaponSlotActions[i] == null || weaponSlotActions[i].action == null) continue;

            InputAction action = weaponSlotActions[i].action;

            if (!weaponSlotActionMap.ContainsKey(action))
            {
                weaponSlotActionMap.Add(action, i);
            }

            action.Enable();
            action.performed += OnWeaponSlotPerformed;
        }
    }

    private void UnbindMoveForwardAction()
    {
        if (moveForwardAction == null || moveForwardAction.action == null) return;

        moveForwardAction.action.performed -= OnForwardPerformed;
        moveForwardAction.action.canceled -= OnForwardCanceled;
        moveForwardAction.action.Disable();
    }

    private void UnbindMoveBackwardAction()
    {
        if (moveBackwardAction == null || moveBackwardAction.action == null) return;

        moveBackwardAction.action.performed -= OnBackwardPerformed;
        moveBackwardAction.action.canceled -= OnBackwardCanceled;
        moveBackwardAction.action.Disable();
    }

    private void UnbindAttackLeftAction()
    {
        if (attackLeftAction == null || attackLeftAction.action == null) return;

        attackLeftAction.action.performed -= OnAttackLeftPerformed;
        attackLeftAction.action.Disable();
    }

    private void UnbindAttackRightAction()
    {
        if (attackRightAction == null || attackRightAction.action == null) return;

        attackRightAction.action.performed -= OnAttackRightPerformed;
        attackRightAction.action.Disable();
    }

    private void UnbindWeaponSlotActions()
    {
        for (int i = 0; i < weaponSlotActions.Count; i++)
        {
            if (weaponSlotActions[i] == null || weaponSlotActions[i].action == null) continue;

            InputAction inputAction = weaponSlotActions[i].action;
            inputAction.performed -= OnWeaponSlotPerformed;
            inputAction.Disable();
        }

        weaponSlotActionMap.Clear();
    }

    #endregion

    #region Input Action Callbacks
    private void OnForwardPerformed(InputAction.CallbackContext context)
    {
        isForwardPressed = true;
        RefreshMoveDirection();
    }

    private void OnForwardCanceled(InputAction.CallbackContext context)
    {
        isForwardPressed = false;
        RefreshMoveDirection();
    }

    private void OnBackwardPerformed(InputAction.CallbackContext context)
    {
        isBackwardPressed = true;
        RefreshMoveDirection();
    }

    private void OnBackwardCanceled(InputAction.CallbackContext context)
    {
        isBackwardPressed = false;
        RefreshMoveDirection();
    }

    private void OnAttackLeftPerformed(InputAction.CallbackContext context)
    {
        if (!context.ReadValueAsButton()) return;

        OnAttackInput?.Invoke(GameAttackInputType.LeftClick);
    }

    private void OnAttackRightPerformed(InputAction.CallbackContext context)
    {
        if (!context.ReadValueAsButton()) return;

        OnAttackInput?.Invoke(GameAttackInputType.RightClick);
    }

    private void OnWeaponSlotPerformed(InputAction.CallbackContext context)
    {
        if (!context.ReadValueAsButton()) return;

        if (weaponSlotActionMap.TryGetValue(context.action, out int slotIndex))
        {
            OnWeaponSlotPressed?.Invoke(slotIndex);
        }
    }

    #endregion
}
