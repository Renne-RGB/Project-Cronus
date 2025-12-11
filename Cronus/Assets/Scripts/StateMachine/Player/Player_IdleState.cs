using UnityEngine;

public class Player_IdleState : Player_BasicState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        
    }
    public override void Update()
    {
        base.Update();
        HandleIdleVelocity();

        //壁にぶつかったら移動状態に切り替えないようにする
        if ((player.moveInput.x == player.facingDirX && player.wallDetectedX) ||
            (player.moveInput.y == player.facingDirY && player.wallDetectedY))
            return;

        if (player.moveInput.x != 0 || player.moveInput.y != 0)
            stateMachine.ChangeState(player.moveState);

    }

    private void HandleIdleVelocity()
    {
        //待機状態の移動速度常に0にする
        player.SetVelocity(0, 0);
    }

}
