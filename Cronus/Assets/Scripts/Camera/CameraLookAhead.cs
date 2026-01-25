using UnityEngine;

public class CameraLookAhead : MonoBehaviour
{
    [Header("PlayerInfo")]
    public Transform playerTransform;
    public Rigidbody2D playerRb;

    [Header("parameter")]
    public float lookAheadDistance = 13.0f;
    public float smoothSpeed = 3.0f;

    private Vector3 currentOffset;

    void Update()
    {
        if (playerTransform == null || playerRb == null)
            return;

        Vector2 moveDir = playerRb.linearVelocity;

        Vector3 targetOffset = Vector3.zero;

        if (moveDir.sqrMagnitude > 0.1f)
        {
            targetOffset = moveDir.normalized * lookAheadDistance;
        }
        else
        {
            targetOffset = Vector3.zero;
        }

        currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothSpeed);

        transform.position = playerTransform.position + currentOffset;
    }
}