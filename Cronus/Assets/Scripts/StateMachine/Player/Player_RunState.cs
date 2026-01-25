using UnityEngine;

public class Player_RunState : Player_MoveState
{
    private float moveSpeedMul = 2.0f; // 跑步速度倍率
    private float playerSpeedTemp = 0f;
    private float noiseInterval = 0.4f;

    public Player_RunState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerSpeedTemp = player.moveSpeed;
        player.moveSpeed *= moveSpeedMul;
    }

    public override void Update()
    {
        if (player.input.Player.Dash.WasPressedThisFrame())
        {
            if (player.dashCooldownTimer <= 0)
            {
                player.dashCooldownTimer = player.dashCooldown;
                player.SetInvincibleFlash(true);
                stateMachine.ChangeState(player.dashState);
                return;
            }
        }

        if (TryUseCharge())
            return;

        //runキー押していないならmoveに戻る
        if (!player.input.Player.Run.IsPressed())
        {
            stateMachine.ChangeState(player.moveState);
            return;
        }

        //移動入力がなければidleに戻る
        if (player.moveInput.x == 0 && player.moveInput.y == 0)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        player.anim.SetFloat("x", player.moveInput.x);
        player.anim.SetFloat("y", player.moveInput.y);

        //壁チェック
        bool isBlockedX = player.wallDetectedX && player.moveInput.x != 0 && Mathf.Sign(player.moveInput.x) == Mathf.Sign(player.facingDirX);
        bool isBlockedY = player.wallDetectedY && player.moveInput.y != 0 && Mathf.Sign(player.moveInput.y) == Mathf.Sign(player.facingDirY);

        float vx = isBlockedX ? 0 : player.moveInput.x * player.moveSpeed;
        float vy = isBlockedY ? 0 : player.moveInput.y * player.moveSpeed;

        Vector2 finalVelocity = new Vector2(vx, vy);
        if (finalVelocity.magnitude > player.moveSpeed)
        {
            finalVelocity = finalVelocity.normalized * player.moveSpeed;
        }

        player.SetVelocity(finalVelocity.x, finalVelocity.y);

        if (player.noiseCooldownTimer <= 0)
        {
            player.EmitRunNoise();
            player.noiseCooldownTimer = noiseInterval;
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.moveSpeed = playerSpeedTemp;
    }
}