using UnityEngine;

public class Player_BasicState : PlayerState
{
    public PlayerFeedBack playerFeedBack;
    public Player_BasicState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        // 攻撃入力の確認
        if (player.CheckAttackInput())
        {
            Enemy target = player.GetCankillEnemy();
            if (target != null)
            {
                if (target.GetCanAssassed())
                {
                    stateMachine.ChangeState(player.attackState);
                }
                else
                {
                    //暗殺失敗状態に入る
                    stateMachine.ChangeState(player.failedAttackState);

                    //敵はガード状態
                    target.TriggerBlockOrAlert();
                }
            }
            return;
        }

        //Dash Input
        if (player.input.Player.Dash.WasPressedThisFrame())
        {
            //cooldownの確認
            if (player.dashCooldownTimer <= 0)
            {
                player.dashCooldownTimer = player.dashCooldown; //cooldown設定
                player.SetInvincibleFlash(false);
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
            if (player.GetHP() > 1)
            {
                stateMachine.ChangeState(player.prepareChargeState);
            }
            else
            {
                player.PlayErrorShake();

                if (stateMachine.currentState != player.idleState)
                {
                    stateMachine.ChangeState(player.idleState);
                }
            }

            return;
        }

        if (player.input.Player.Active.WasPressedThisFrame())
    {
        if (player.currentHideSpot != null)
        {
            stateMachine.ChangeState(player.hiddenState);
            return;
        }
    }
    }

    public override void Exit()
    {
        base.Exit();
    }
}