using UnityEngine;

public class Player_AttackState : EntityState
{
    public Player_AttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }

    private void HandleAttackVelocity()
    {
        //攻撃する時移動速度を0にする
        player.SetVelocity(0, 0);
    }
}
