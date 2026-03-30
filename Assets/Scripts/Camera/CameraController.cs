using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private Vector3 cameraOffset;

    private void LateUpdate()
    {
        LateTick();
    }

    private void LateTick()
    {
        if (playerTransform != null && cameraTransform != null)
        {
            Vector3 newPosition = playerTransform.position + cameraOffset;
            cameraTransform.position = newPosition;
        }
    }
}
