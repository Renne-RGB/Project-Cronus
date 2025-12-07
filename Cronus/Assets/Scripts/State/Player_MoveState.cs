using UnityEngine;

public class Player_MoveState : EntityState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        
    }

    public override void Update()
    {
        base.Update();

        if(player.moveInput.x == 0 && player.moveInput.y == 0)
            stateMachine.ChangeState(player.idleState);

        Vector2 input = player.moveInput.normalized;
        player.SetVelocity(input.x * player.moveSpeed, input.y * player.moveSpeed);
    }
}
