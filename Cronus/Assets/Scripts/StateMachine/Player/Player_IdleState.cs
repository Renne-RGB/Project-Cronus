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

       // 1. 计算阻挡情况（确保输入方向与墙的方向一致）
        bool isBlockedX = player.wallDetectedX && (player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) == Mathf.Sign(player.facingDirX));
        bool isBlockedY = player.wallDetectedY && (player.moveInput.y != 0 && Mathf.Sign(player.moveInput.y) == Mathf.Sign(player.facingDirY));

        // 2. 只有当“所有当前按下的方向”都被挡住时，才拦截切换，留在 Idle
        // 这样可以确保：如果你撞着墙角但同时按着空旷的方向，能顺利切入 Move 状态
        bool moveForbiddenX = isBlockedX && player.moveInput.y == 0;
        bool moveForbiddenY = isBlockedY && player.moveInput.x == 0;
        bool moveForbiddenBoth = isBlockedX && isBlockedY;

        if (moveForbiddenX || moveForbiddenY || moveForbiddenBoth)
            return;

        // 3. 只有存在未被阻挡的效输入时，才切换到 Move 状态
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
