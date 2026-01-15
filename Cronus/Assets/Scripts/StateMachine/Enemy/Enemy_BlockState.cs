using UnityEngine;

// 敌人格挡/被惊动状态
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

        // if (enemy.playerTransform != null)
        // {
        //     float direction = enemy.playerTransform.position.x - enemy.transform.position.x;
        //     if (direction > 0 && enemy.facingDirX < 0)
        //         enemy.FlipX();
        //     else if (direction < 0 && enemy.facingDirX > 0)
        //         enemy.FlipX();
        // }
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