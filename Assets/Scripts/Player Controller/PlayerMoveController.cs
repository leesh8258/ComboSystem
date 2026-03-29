using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    private const string MOVE_PARAM = "Move";
    private const string MOVE_DIRECTION_PARAM = "MoveDirection";

    [SerializeField] private PlayerInputController inputController;
    [SerializeField] private PlayerActionController actionController;
    [SerializeField] private Animator animator;
    [SerializeField] private float moveSpeed = 3f;

    private float fixedX;

    private void Start()
    {
        fixedX = transform.position.x;
    }

    private void Reset()
    {
        inputController = GetComponent<PlayerInputController>();
        actionController = GetComponent<PlayerActionController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (inputController == null) return;

        float moveDirection = GetMoveDirection();

        UpdateMovement(moveDirection);
        UpdateAnimation(moveDirection);
    }

    private void LateUpdate()
    {
        Vector3 position = transform.position;
        position.x = fixedX;
        transform.position = position;
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
