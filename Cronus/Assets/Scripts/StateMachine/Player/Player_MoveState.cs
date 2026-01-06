using UnityEngine;

public class Player_MoveState : Player_BasicState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

    public override void Update()
    {
        base.Update();

        // inputがなければIdle状態に戻る
        if (player.moveInput.x == 0 && player.moveInput.y == 0)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        bool isBlockedX = player.wallDetectedX && player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) == Mathf.Sign(player.facingDirX);
        bool isBlockedY = player.wallDetectedY && player.moveInput.y != 0 && Mathf.Sign(player.moveInput.y) == Mathf.Sign(player.facingDirY);

        //全ての方向は移動できないならidle状態に戻る
        bool stopMovingX = isBlockedX && player.moveInput.y == 0;
        bool stopMovingY = isBlockedY && player.moveInput.x == 0;
        bool stopMovingBoth = isBlockedX && isBlockedY;

        if (stopMovingX || stopMovingY || stopMovingBoth)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        if (player.input.Player.Run.IsPressed())
            stateMachine.ChangeState(player.runState);

        //遮られていない方向にのみ速度を与える
        float vx = isBlockedX ? 0 : player.moveInput.x * player.moveSpeed;
        float vy = isBlockedY ? 0 : player.moveInput.y * player.moveSpeed;

        //斜め移動速度の修正
        Vector2 finalVelocity = new Vector2(vx, vy);
        if (finalVelocity.magnitude > player.moveSpeed)
        {
            finalVelocity = finalVelocity.normalized * player.moveSpeed;
        }

        player.SetVelocity(finalVelocity.x, finalVelocity.y);
    }
}
