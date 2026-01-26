using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Settings")]
    public GameObject hitEffect;
    public float effectDuration = 0.5f;
    public float damage = 1f;
    public LayerMask targetLayers;

    private float stunDuration = 1.0f;
    private float knockbackForce = 10.0f;

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

        bool shouldDestroy = true;

        if (player != null)
        {
            Rigidbody2D bulletRb = GetComponent<Rigidbody2D>();
            Vector2 bulletDir = bulletRb != null ? bulletRb.linearVelocity.normalized : (Vector2)transform.right;

            //無敵時間プレイヤーに当たれない(shouldDestroy->false)
            shouldDestroy = player.TakeDamageByBullet(bulletDir, stunDuration, knockbackForce);
        }

        if (shouldDestroy)
        {
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect, effectDuration);
            }
            Destroy(gameObject);
        }
    }
}