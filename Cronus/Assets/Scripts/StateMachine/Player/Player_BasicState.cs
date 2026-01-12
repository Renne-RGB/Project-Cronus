using UnityEngine;

public class Player_BasicState : PlayerState
{
    public Player_BasicState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        // 攻撃入力の確認
        if (player.CheckAttackInput())
        {
            stateMachine.ChangeState(player.attackState);
            return;
        }

        //Dash Input
        if (player.input.Player.Dash.WasPressedThisFrame())
        {
            //cooldownの確認
            if (player.dashCooldownTimer <= 0)
            {
                player.dashCooldownTimer = player.dashCooldown; //cooldown設定
                player.invincibleFlashEnabled = false;
                stateMachine.ChangeState(player.dashState);
                return;
            }
            else
            {
                // Debug.Log("Dash is on Cooldown");
            }
        }

        if (player.input.Player.Charge.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.prepareChargeState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}