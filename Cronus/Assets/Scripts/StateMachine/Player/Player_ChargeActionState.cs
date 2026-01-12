using UnityEngine;

public class Player_ChargeActionState : PlayerState
{
    private float startTime;

    public Player_ChargeActionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        startTime = Time.time;
        
        //無敵時間
        player.invincibleTimer = player.chargeActionDuration;
        player.invincibleFlashEnabled = false;
    }

    public override void Update()
    {
        base.Update();

        // アローが向いていた方向（chargeDir）へ高速移動
        player.SetVelocity(player.chargeDir.x * player.chargeSpeed, player.chargeDir.y * player.chargeSpeed);

        // 時間経過で終了
        if (Time.time >= startTime + player.chargeActionDuration)
        {
            player.SetVelocity(0, 0);
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, 0);
        player.invincibleFlashEnabled = true;
    }
}