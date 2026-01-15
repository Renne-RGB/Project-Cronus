using UnityEngine;

public class Player_AttackState : PlayerState
{
    public Player_AttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        float targetIndex = 0f;

        Enemy targetEnemy = player.GetCankillEnemy();

        if (targetEnemy != null)
        {
            if (targetEnemy.CompareTag("Enemy"))
                targetIndex = 0f;

            targetEnemy.TriggerAssassinationDeath((int)targetIndex);

            player.MovePlayerToEnemy();
            player.SetAttackStandby(false);
            player.anim.SetFloat("attackIndex", targetIndex);
            player.SetInvincibleFlash(true);
        }

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
