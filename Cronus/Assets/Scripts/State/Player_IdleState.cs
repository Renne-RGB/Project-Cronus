using UnityEngine;

public class Player_IdleState : EntityState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        
    }

    public override void Update()
    {
        base.Update();

        if(player.moveInput.x != 0 || player.moveInput.y != 0)
            stateMachine.ChangeState(player.moveState);

        if (player.input.Player.Attack.WasPressedThisFrame())
            stateMachine.ChangeState(player.attackState);
    }

}
