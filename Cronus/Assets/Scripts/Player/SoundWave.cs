using UnityEngine;

public class SoundWave : MonoBehaviour
{
    private SpriteRenderer sr;
    private float targetScale;
    private float currentScale = 0f;

    [Header("Settings")]
    public float expandSpeed = 100f; //拡散速度
    public float fadeSpeed = 2f;

    public void Setup(float radius)
    {
        sr = GetComponent<SpriteRenderer>();
    
        targetScale = radius * 2f; 
        
        currentScale = 0f;
        transform.localScale = Vector3.zero;

        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;

            sr.sortingOrder = 100;
        }
    }

    void Update()
    {
        if (currentScale < targetScale)
        {
            //一秒内最大まで拡散
            currentScale = Mathf.MoveTowards(currentScale, targetScale, expandSpeed * Time.deltaTime);
            transform.localScale = new Vector3(currentScale, currentScale, 1f);
        }

        if (sr != null)
        {
            Color c = sr.color;
            c.a -= fadeSpeed * Time.deltaTime;
            sr.color = c;

            //完全に透明になったら削除
            if (c.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}