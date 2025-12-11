using UnityEngine;

public class Player_MoveState : Player_BasicState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Update()
    {
        base.Update();

        //壁にぶつかったら移動状態停止
        if (player.moveInput.x == 0 && player.moveInput.y == 0)
            stateMachine.ChangeState(player.idleState);

        if ((player.moveInput.x != 0 && player.wallDetectedX) || (player.moveInput.y != 0 && player.wallDetectedY))
            stateMachine.ChangeState(player.idleState);

        Vector2 input = player.moveInput.normalized;
        player.SetVelocity(input.x * player.moveSpeed, input.y * player.moveSpeed);

    }
}
