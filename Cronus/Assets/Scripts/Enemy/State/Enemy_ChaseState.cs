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
    public override void Update()
    {
        base.Update();
        Dash();

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
                if (enemy.currentIndex == 0)
                {
                    Debug.Log("DIR: " + direction);
                    Debug.Log("pathPointList[currentIndex]: " + enemy.pathPointList[enemy.currentIndex]);
                    Debug.Log("enemy.transform: " + enemy.transform.position);
                    Debug.Log("-------------------------");
                }
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
    }

    void Dash()
    {
        if (enemy.MovementInput.magnitude > 0.1f && enemy.currentSpeed >= 0)
        {
            enemy.rb.linearVelocity = enemy.MovementInput * enemy.currentSpeed;
            //Flip
            if (enemy.MovementInput.x < 0)      //右
                enemy.sr.flipX = true;
            if (enemy.MovementInput.x > 0)      //左
                enemy.sr.flipX = false;
        }
        else
            enemy.rb.linearVelocity = Vector2.zero;
    }
}
