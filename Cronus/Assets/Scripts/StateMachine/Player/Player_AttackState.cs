using UnityEngine;

public class Player_AttackState : PlayerState
{
    public Player_AttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        Enemy targetEnemy = player.GetCankillEnemy();

        if (targetEnemy != null)
        {
            //WitchTime終わる
            if (player.witchTimeManager.IsWitchTimeActive)
                player.witchTimeManager.DeactivateWitchTime();

            targetEnemy.TriggerAssassinationDeath();

            player.MovePlayerToEnemy();
            player.SetAttackStandby(false);
            player.SetInvincibleFlash(true);
        }

        base.Enter();

    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);

            //HP回復
            player.ChangeHP(1);

            player.SetAttackStandby(false);

            player.SetInvincibleFlash(false);
        }
    }

    private void HandleAttackVelocity()
    {
        //攻撃する時移動速度を0にする
        player.SetVelocity(0, 0);
    }
}
