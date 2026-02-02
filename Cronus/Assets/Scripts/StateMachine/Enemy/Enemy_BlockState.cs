using UnityEngine;

public class Enemy_BlockState : EnemyState
{
    public Enemy_BlockState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.SetVelocity(0, 0);

        enemy.SetAlert(true);

    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled)
        {
            enemy.anim.SetBool("block", false);
            stateMachine.ChangeState(enemy.chaseState);
        }
    }
}