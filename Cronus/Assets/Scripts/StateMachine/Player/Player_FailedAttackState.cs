using UnityEngine;

public class Player_FailedAttackState : PlayerState
{
    public Player_FailedAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocity(0, 0);

        player.MovePlayerToEnemy();
    }

    public override void Update()
    {
        base.Update();
        
        player.SetVelocity(0, 0);

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
            Vector2 backDir = new Vector2(-player.facingDirX, -player.facingDirY);
            player.TakeDamageByMelee(backDir, 1.0f, 20.0f, 0);
        }
    }
}