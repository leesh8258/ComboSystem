using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private InputActionReference moveForwardAction;
    [SerializeField] private InputActionReference moveBackwardAction;

    private bool isForwardPressed;
    private bool isBackwardPressed;

    public float MoveDirection { get; private set; }

    private void OnEnable()
    {
        if (moveForwardAction != null && moveForwardAction.action != null)
        {
            moveForwardAction.action.Enable();
            moveForwardAction.action.performed += OnForwardPerformed;
            moveForwardAction.action.canceled += OnForwardCanceled;
        }

        if (moveBackwardAction != null && moveBackwardAction.action != null)
        {
            moveBackwardAction.action.Enable();
            moveBackwardAction.action.performed += OnBackwardPerformed;
            moveBackwardAction.action.canceled += OnBackwardCanceled;
        }

        RefreshMoveDirection();
    }

    private void OnDisable()
    {
        if (moveForwardAction != null && moveForwardAction.action != null)
        {
            moveForwardAction.action.performed -= OnForwardPerformed;
            moveForwardAction.action.canceled -= OnForwardCanceled;
            moveForwardAction.action.Disable();
        }

        if (moveBackwardAction != null && moveBackwardAction.action != null)
        {
            moveBackwardAction.action.performed -= OnBackwardPerformed;
            moveBackwardAction.action.canceled -= OnBackwardCanceled;
            moveBackwardAction.action.Disable();
        }

        isForwardPressed = false;
        isBackwardPressed = false;
        MoveDirection = 0f;
    }

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
}
