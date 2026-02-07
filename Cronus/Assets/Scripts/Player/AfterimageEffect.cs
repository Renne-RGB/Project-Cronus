using UnityEngine;
using System.Collections;

public class AfterimageEffect : MonoBehaviour
{
    public SpriteRenderer targetSR;

    [Header("settings")]
    public float ghostDelay = 0.07f;
    public float ghostLifeTime = 2.0f;
    public Color ghostColor = new Color(0f, 0.2f, 0.8f, 0.6f);

    private float ghostDelayTimer;
    private bool isEmitting = false;

    public void SetTarget(SpriteRenderer sr)
    {
        targetSR = sr;
    }

    void Update()
    {
        if (isEmitting && targetSR != null)
        {
            if (ghostDelayTimer > 0)
            {
                ghostDelayTimer -= Time.unscaledDeltaTime;
            }
            else
            {
                GenerateGhost();
                ghostDelayTimer = ghostDelay;
            }
        }
    }

    public void StartEffect() => isEmitting = true;
    public void StopEffect() => isEmitting = false;

    private void GenerateGhost()
    {
        GameObject ghost = new GameObject("Afterimage_Ghost");

        ghost.transform.SetPositionAndRotation(targetSR.transform.position, targetSR.transform.rotation);
        ghost.transform.localScale = targetSR.transform.lossyScale;

        SpriteRenderer sr = ghost.AddComponent<SpriteRenderer>();
        sr.sprite = targetSR.sprite;
        sr.color = ghostColor;
        sr.flipX = targetSR.flipX;
        sr.flipY = targetSR.flipY;
        sr.sortingOrder = targetSR.sortingOrder - 1;

        StartCoroutine(FadeOutAndDestroy(ghost, sr));
    }

    private IEnumerator FadeOutAndDestroy(GameObject obj, SpriteRenderer sr)
    {
        float timer = 0;
        Color startColor = ghostColor;
        while (timer < ghostLifeTime)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, timer / ghostLifeTime);
            if (sr != null) sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        Destroy(obj);
    }
}