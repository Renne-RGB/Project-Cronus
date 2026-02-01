using UnityEngine;

public class SoundWave : MonoBehaviour
{
    private SpriteRenderer sr;
    private float targetScale;
    private float currentScale = 0f;

    [Header("Settings")]
    // 建议在 Inspector 中将 expandSpeed 设为 40-60，fadeSpeed 设为 2-3
    public float expandSpeed = 100f; // 大幅提升扩散速度
    public float fadeSpeed = 2f;   // 淡出速度

    public void Setup(float radius)
    {
        sr = GetComponent<SpriteRenderer>();
        
        // 目标直径 = 半径 * 2
        targetScale = radius * 2f; 
        
        currentScale = 0f;
        transform.localScale = Vector3.zero;

        // 初始化颜色，确保 Alpha 是满的 (1.0)
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
            // 确保在顶层显示
            sr.sortingOrder = 100;
        }
    }

    void Update()
    {
        // 1. 快速扩散逻辑
        if (currentScale < targetScale)
        {
            // 使用 MoveTowards 保证在 1 秒内迅速达到目标大小
            currentScale = Mathf.MoveTowards(currentScale, targetScale, expandSpeed * Time.deltaTime);
            transform.localScale = new Vector3(currentScale, currentScale, 1f);
        }

        // 2. 颜色变淡逻辑
        if (sr != null)
        {
            Color c = sr.color;
            c.a -= fadeSpeed * Time.deltaTime;
            sr.color = c;

            // 当透明度扣完且已经完成扩散（或接近完成）时销毁
            if (c.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}