using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Enemy_MoveState : EnemyState
{
    private Vector2 direction;
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        GeneratePatroPoint();
    }

    public override void Update()
    {
        base.Update();

        enemy.GetPlayerTransform();
        if (enemy.playerTransform != null)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }

        //ルーティングポイントがなかったら生成する
        if (enemy.pathPointList == null || enemy.pathPointList.Count <= 0)
        {
            GeneratePatroPoint();
        }
        else
        {
            if (Vector2.Distance(enemy.transform.position, enemy.pathPointList[enemy.currentIndex]) <= 0.4f)
            {
                enemy.currentIndex++;

                //最後のルーティングポイントに着いたら
                if(enemy.currentIndex >= enemy.pathPointList.Count)
                    stateMachine.ChangeState(enemy.idleState);
                else
                {
                    direction = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
                    enemy.MovementInput = direction;
                }
            }
            else
            {

            }
        }

        enemy.Dash();
    }

    public void GeneratePatroPoint()
    {
        while (true)
        {
            int i = Random.Range(0, enemy.patroPoints.Length);

            if (enemy.targetPointIndex != i)
            {
                enemy.targetPointIndex = i;
                break;
            }
        }

        enemy.GeneratePath(enemy.patroPoints[enemy.targetPointIndex].position);
    }
}
