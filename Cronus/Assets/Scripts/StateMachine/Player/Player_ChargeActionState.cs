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
        HandleCost();

        //無敵時間
        player.invincibleTimer = player.chargeActionDuration;
        player.SetInvincibleFlash(false);

        if (player.noiseCooldownTimer <= 0)
        {
            player.EmitRunNoise();
        }
    }

    public override void Update()
    {
        base.Update();
        stateTimer += Time.unscaledDeltaTime;

        player.SetVelocity(player.chargeDir.x * player.chargeSpeed, player.chargeDir.y * player.chargeSpeed);

        if (player.CheckAttackInput())
        {
            Enemy target = player.GetCankillEnemy();
            if (target != null)
            {
                if (!target.canBlock || target.GetCanAssassed() || player.witchTimeManager.IsWitchTimeActive)
                {
                    stateMachine.ChangeState(player.attackState);
                    return;
                }
                else
                {
                    stateMachine.ChangeState(player.failedAttackState);
                    target.TriggerBlockOrAlert();
                    return;
                }
            }
        }

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

    protected virtual void HandleCost()
    {
        player.ChangeHP(-1);
    }
}