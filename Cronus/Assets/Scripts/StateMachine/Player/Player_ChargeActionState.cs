using UnityEngine;

public class Player_ChargeActionState : PlayerState
{
    private float stateTimer;

    public Player_ChargeActionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = 0f;

        //発動成功したらhp消費する
        player.ChangeHP(-1);

        //無敵時間
        player.invincibleTimer = player.chargeActionDuration;
        player.SetInvincibleFlash(false);
    }

    public override void Update()
    {
        base.Update();
        stateTimer += Time.unscaledDeltaTime;

        player.SetVelocity(player.chargeDir.x * player.chargeSpeed, player.chargeDir.y * player.chargeSpeed);

        if (stateTimer >= player.chargeActionDuration)
        {
            player.SetVelocity(0, 0);
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, 0);
        player.SetInvincibleFlash(true);
    }
}