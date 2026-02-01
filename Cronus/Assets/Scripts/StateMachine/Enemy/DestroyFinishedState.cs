using UnityEngine;

public class DestroyFinishedState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.transform.parent.gameObject, stateInfo.length);
    }
}