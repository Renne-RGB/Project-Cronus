using UnityEngine;

public class Enemy_CqbState : EnemyState
{
    public Enemy_CqbState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetAlert(true);
    }

    public override void Update()
    {
        base.Update();

        //攻撃状態の移動速度常に0にする
        enemy.rb.linearVelocity = Vector2.zero;

        enemy.GetPlayerTransform();     //プレイヤーの位置を取る

        if (triggerCalled)
            stateMachine.ChangeState(enemy.idleState);
    }
}
