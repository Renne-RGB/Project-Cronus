using Unity.VisualScripting;
using UnityEngine;

public abstract class EntityState
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody2D rb;
    protected PlayerInputSet input;

    protected bool triggerCalled;

    public EntityState(Player player, StateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
    }

    public virtual void Enter()
    {
        //状態に入ったたびに呼び出される
        anim.SetBool(animBoolName, true);
        triggerCalled = false;
        //Debug.Log("I enter " + animBoolName);
    }

    public void CallAnimationTrigger()
    {
        triggerCalled = true;
    }
    
    public virtual void Update()
    {
        //Debug.Log("I run update of " + animBoolName);
    }

    public virtual void Exit()
    {
        anim.SetBool(animBoolName, false);
        //Debug.Log("I exit " + animBoolName);
    }
}
