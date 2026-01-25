using UnityEngine;

public class CameraLookAhead : MonoBehaviour
{
    [Header("PlayerInfo")]
    public Transform playerTransform;
    public Rigidbody2D playerRb;
    
    [Header("parameter")]
    public float lookAheadDistance = 13.0f;
    public float smoothSpeed = 5.0f;

    void Update()
    {
        if (playerTransform == null || playerRb == null) return;

        Vector2 moveDir = playerRb.linearVelocity;

        if (moveDir.sqrMagnitude > 0.1f)
        {
            Vector3 offset = moveDir.normalized * lookAheadDistance;
            
            //目标位置 = プレイヤー位置 + 偏移
            Vector3 desiredPos = playerTransform.position + offset;
            
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);
        }
        // else
        // {
        //     //移動停止したら カメラがプレイヤー位置に戻る
        //     transform.position = Vector3.Lerp(transform.position, playerTransform.position, Time.deltaTime * smoothSpeed);
        // }
    }
}