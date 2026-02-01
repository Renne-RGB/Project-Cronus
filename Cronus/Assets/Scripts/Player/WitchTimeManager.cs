using UnityEngine;
using System.Collections;

public class WitchTimeManager : MonoBehaviour
{
    private Player player;
    public bool IsWitchTimeActive { get; private set; }

    [Header("Settings")]
    public float slowMotionFactor = 0.05f;  //時間の流れの倍率
    public float duration = 3f;
    public float witchChargeDuration = 0.5f; //スキルチャージ時間

    public void Init(Player player)
    {
        this.player = player;
    }

    public void ActivateWitchTime()
    {
        if (IsWitchTimeActive)
            return;

        player.invincibleTimer = player.dashDuration + 1.0f;
        player.SetInvincibleFlash(false);

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

        StopAllCoroutines();

        player.ToggleInteractionIcon();
    }
}