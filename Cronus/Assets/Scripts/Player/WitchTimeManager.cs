using UnityEngine;
using System.Collections;

public class WitchTimeManager : MonoBehaviour
{
    private Player player;
    private AfterimageEffect afterimage;
    public bool IsWitchTimeActive { get; private set; }

    [Header("Settings")]
    public float slowMotionFactor = 0.05f;  //時間の流れの倍率
    public float duration = 3f;
    public float witchChargeDuration = 0.5f; //スキルチャージ時間

    public void Init(Player player)
    {
        this.player = player;

        afterimage = GetComponent<AfterimageEffect>();
        if (afterimage == null) afterimage = gameObject.AddComponent<AfterimageEffect>();

        SpriteRenderer playerSR = player.GetComponentInChildren<SpriteRenderer>();
        if (playerSR != null)
        {
            afterimage.SetTarget(playerSR);
        }
    }

    public void ActivateWitchTime()
    {
        if (IsWitchTimeActive)
            return;

        player.invincibleTimer = duration;
        player.SetInvincibleFlash(false);

        if (afterimage != null)
            afterimage.StartEffect();

        StartCoroutine(WitchTimeRoutine());
    }

    private IEnumerator WitchTimeRoutine()
    {
        IsWitchTimeActive = true;
        player.ToggleInteractionIcon();

        Time.timeScale = slowMotionFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (player.anim != null)
            player.anim.speed = 1f / slowMotionFactor;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        DeactivateWitchTime();
    }

    public void DeactivateWitchTime()
    {
        IsWitchTimeActive = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (player.anim != null)
            player.anim.speed = 1f;

        if (afterimage != null)
            afterimage.StopEffect();

        StopAllCoroutines();

        player.ToggleInteractionIcon();
    }
}