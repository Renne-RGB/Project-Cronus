using UnityEngine;

public class DestroyFinishedState : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(animator.transform.root.gameObject, stateInfo.length);
    }
}