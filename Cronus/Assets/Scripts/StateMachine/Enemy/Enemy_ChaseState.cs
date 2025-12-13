using UnityEngine;

public class Enemy_ChaseState : EnemyState
{
    public Enemy_ChaseState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.moveSpeed = 3.0f;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();

        enemy.GetPlayerTransform();     //プレイヤーの位置を取る
        enemy.AutoPath();
        //プレイヤーを見つかったら
        if (enemy.playerTransform != null)
        {
            //ルーティングリストのNULLチェック
            if (enemy.pathPointList == null || enemy.pathPointList.Count <= 0)
                return;

            if (enemy.currentIndex >= enemy.pathPointList.Count)
                return; // ルーティング既に終わっている、新しいルーティング生成を待つ

            //接近戦範囲をチェック
            if (enemy.distance <= enemy.cqbDistance)
            {
                stateMachine.ChangeState(enemy.cqbState);
            }
            else
            {
                //プレイヤーを追撃する
                Vector2 direction = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
                enemy.MovementInput = direction;
                //enemy.SetVelocity(enemy.moveSpeed * direction.x, enemy.moveSpeed * direction.y);
            }
        }
        //プレイヤーを見つけなかったら
        else
        {
            //待機状態に入る
            stateMachine.ChangeState(enemy.idleState);
        }

        enemy.Dash();
    }

}
