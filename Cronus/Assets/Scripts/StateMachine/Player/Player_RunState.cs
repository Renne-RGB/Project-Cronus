using UnityEngine;

public class Player_RunState : Player_MoveState
{
    private float moveSpeedMul = 2.0f;
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
        base.Update();

        //走る入力なかったらRunstate終わる
        if (!player.input.Player.Run.IsPressed())
        {
            stateMachine.ChangeState(player.moveState);
            return;
        }
        if (player.moveInput.x == 0 && player.moveInput.y == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }

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
