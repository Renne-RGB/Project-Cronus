using UnityEngine;

public class Enemy_IdleState : EnemyState
{
    private float Timer = 0f;
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Timer = 0f;
        //待機状態の移動速度常に0にする
        enemy.rb.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();

        enemy.GetPlayerTransform();     //プレイヤーの位置を取る

        if (enemy.GetAlert())
        {
            stateMachine.ChangeState(enemy.chaseState);
        }

        //プレイヤーが範囲内にいる
        // if (enemy.playerTransform != null)
        // {
        //     enemy.FindTargetPlayer();
        // }
        //プレイヤーがなかったらパトロール状態に入る
        // else
        // {
        if (Timer <= enemy.idleDuration)
        {
            Timer += Time.deltaTime;
        }
        else
        {
            Timer = 0f;
            stateMachine.ChangeState(enemy.moveState);
        }
        // }
    }
}
