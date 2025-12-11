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

        Dash();
    }

    void Dash()
    {
        if (enemy.MovementInput.sqrMagnitude > 0.01f && enemy.currentSpeed > 0f)
        {
            Vector2 newPos = enemy.rb.position + enemy.MovementInput * enemy.currentSpeed * Time.fixedDeltaTime;
            enemy.rb.MovePosition(newPos);

            if (Mathf.Abs(enemy.MovementInput.x) > 0.1f)
            {
                enemy.sr.flipX = enemy.MovementInput.x < 0;
            }

            // if (enemy.pathPointList != null && enemy.pathPointList.Count > enemy.currentIndex)
            // {
            //     Vector2 nextNode = enemy.pathPointList[enemy.currentIndex];
            //     float dx = nextNode.x - enemy.transform.position.x;

            //     if (Mathf.Abs(dx) > 0.05f) 
            //         enemy.sr.flipX = dx < 0;
            // }
        }
        else
        {
            enemy.rb.linearVelocity = Vector2.zero;
        }
    }
}
