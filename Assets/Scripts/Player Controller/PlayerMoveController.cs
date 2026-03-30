using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveController : MonoBehaviour
{
    private const string MOVE_PARAM = "Move";
    private const string MOVE_DIRECTION_PARAM = "MoveDirection";

    [SerializeField] private PlayerInputController inputController;
    [SerializeField] private PlayerActionController actionController;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 3f;

    private void Reset()
    {
        inputController = GetComponent<PlayerInputController>();
        actionController = GetComponent<PlayerActionController>();
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (inputController == null || characterController == null) return;

        float moveDirection = GetMoveDirection();

        UpdateMovement(moveDirection);
        UpdateAnimation(moveDirection);
    }

    private float GetMoveDirection()
    {
        if (actionController != null && actionController.IsAttacking) return 0f;

        return inputController.MoveDirection;
    }

    private void UpdateMovement(float moveDirection)
    {
        if (Mathf.Approximately(moveDirection, 0f)) return;

        Vector3 moveVector = Vector3.forward * moveDirection;
        characterController.Move(moveVector * moveSpeed * Time.deltaTime);
    }

    private void UpdateAnimation(float moveDirection)
    {
        if (animator == null) return;

        bool isMoving = !Mathf.Approximately(moveDirection, 0f);

        animator.SetBool(MOVE_PARAM, isMoving);
        animator.SetFloat(MOVE_DIRECTION_PARAM, moveDirection);
    }

    private void OnAnimatorMove()
    {
        if (animator == null || characterController == null) return;
        if (actionController == null || !actionController.IsAttacking) return;

        Vector3 delta = animator.deltaPosition;
        delta.x = 0f;
        delta.y = 0f;

        if (delta.sqrMagnitude <= 0f) return;

        characterController.Move(delta);
    }
}