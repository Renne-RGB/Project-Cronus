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

        //Timer = 0f;
        //待機状態の移動速度常に0にする
        enemy.rb.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();

        enemy.GetPlayerTransform();     //プレイヤーの位置を取る

        if (enemy.GetAlert() && enemy.canChase)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }

        if (enemy.patroPoints.Length == 1)
        {
            float distanceToPoint = Vector2.Distance(enemy.transform.position, enemy.patroPoints[0].position);

            if (distanceToPoint > 0.4f)
            {
                Timer -= Time.deltaTime;
                if (Timer <= 0f)
                {
                    Timer = enemy.idleDuration;
                    stateMachine.ChangeState(enemy.moveState);
                }
            }
            else
            {
                Timer = enemy.idleDuration;
            }
        }
        else
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0f)
            {
                Timer = enemy.idleDuration;
                stateMachine.ChangeState(enemy.moveState);
            }
        }

    }
}
