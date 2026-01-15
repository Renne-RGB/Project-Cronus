using UnityEngine;

public class Player_AnimationTrigger : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }
    private void CurrentStateTrigger()
    {
        player.CallAnimationTrigger();
    }
  
}
