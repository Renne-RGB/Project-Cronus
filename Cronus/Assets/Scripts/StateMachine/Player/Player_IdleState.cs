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

        if (player.IsShaking)
        {
            //震えているときIdle状態保つ
            return;
        }

        bool isBlockedX = player.wallDetectedX && (player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) == Mathf.Sign(player.facingDirX));
        bool isBlockedY = player.wallDetectedY && (player.moveInput.y != 0 && Mathf.Sign(player.moveInput.y) == Mathf.Sign(player.facingDirY));

        bool moveForbiddenX = isBlockedX && player.moveInput.y == 0;
        bool moveForbiddenY = isBlockedY && player.moveInput.x == 0;
        bool moveForbiddenBoth = isBlockedX && isBlockedY;

        if (moveForbiddenX || moveForbiddenY || moveForbiddenBoth)
            return;

        if (player.moveInput.x != 0 || player.moveInput.y != 0)
        {
            stateMachine.ChangeState(player.moveState);
        }

    }

    private void HandleIdleVelocity()
    {
        //待機状態の移動速度常に0にする
        player.SetVelocity(0, 0);
    }

}
