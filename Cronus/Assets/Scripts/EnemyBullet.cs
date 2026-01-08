using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Settings")]
    public GameObject hitEffect;
    public float effectDuration = 0.5f;
    public float damage = 1f;
    public LayerMask targetLayers;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            return;

        if (((1 << collision.gameObject.layer) & targetLayers) != 0)
        {
            HitTarget(collision.gameObject);
        }
    }

    private void HitTarget(GameObject target)
    {
        Player player = target.GetComponent<Player>();

        //無敵時間プレイヤーに当たれない
        if (player != null)
        {
            if (player.invincibleTimer > 0)
            {
                return;
            }
        }

        // Effect生成
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, effectDuration);
        }

        // Damage处理
        if (player != null)
        {
            Rigidbody2D bulletRb = GetComponent<Rigidbody2D>();
            Vector2 bulletDir = bulletRb != null ? bulletRb.linearVelocity.normalized : transform.right;

            player.TakeDamage(bulletDir);
        }
        else
        {
            //Debug.Log("HitWall");
        }

        Destroy(gameObject);
    }
}