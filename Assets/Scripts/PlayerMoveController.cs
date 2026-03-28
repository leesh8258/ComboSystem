using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private const string MOVE_PARAM = "Move";
    private const string MOVE_DIRECTION_PARAM = "MoveDirection";

    [SerializeField] private PlayerInputController inputController;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 3f;

    private void Reset()
    {
        inputController = GetComponent<PlayerInputController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (inputController == null) return;

        float moveDirection = inputController.MoveDirection;

        UpdateMovement(moveDirection);
        UpdateAnimation(moveDirection);
    }

    private void UpdateMovement(float moveDirection)
    {
        if (Mathf.Approximately(moveDirection, 0f)) return;

        Vector3 moveVector = Vector3.forward * moveDirection;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void UpdateAnimation(float moveDirection)
    {
        if (animator == null) return;

        bool isMoving = !Mathf.Approximately(moveDirection, 0f);

        animator.SetBool(MOVE_PARAM, isMoving);
        animator.SetFloat(MOVE_DIRECTION_PARAM, moveDirection);
    }
}
