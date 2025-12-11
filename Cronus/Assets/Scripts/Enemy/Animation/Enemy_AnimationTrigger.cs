using UnityEngine;

public class Enemy_AnimationTrigger : MonoBehaviour
{
   private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    private void CurrentStateTrigger()
    {
        enemy.CallAnimationTrigger();
    }
}
