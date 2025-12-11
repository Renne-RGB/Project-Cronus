using UnityEngine;

public class Enemy_IdleState : EnemyState
{
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //待機状態の移動速度常に0にする
        enemy.rb.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();

        enemy.GetPlayerTransform();     //プレイヤーの位置を取る

        //プレイヤーが範囲内にいる
        if(enemy.playerTransform != null)
        {
            //攻撃距離より大きいであれば 追撃状態に入る
            if(enemy.distance > enemy.cqbDistance)
            {
                stateMachine.ChangeState(enemy.chaseState);
                Debug.Log("Enemy:Chase State");
            }
            else
            {
                stateMachine.ChangeState(enemy.cqbState);
                Debug.Log("Enemy:Cqb State");
            }
        }
    }
}
