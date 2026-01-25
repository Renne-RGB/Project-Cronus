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

        enemy.SetAlert(false);

        GeneratePatroPoint();
    }

    public override void Update()
    {
        base.Update();

        enemy.GetPlayerTransform();
        if (enemy.playerTransform != null)
        {
            enemy.FindTargetPlayer();
        }

        // ルーティングポイントがなかったら生成する
        if (enemy.pathPointList == null || enemy.pathPointList.Count <= 0)
        {
            GeneratePatroPoint();
        }
        else
        {
            if (enemy.currentIndex >= enemy.pathPointList.Count)
            {
                stateMachine.ChangeState(enemy.idleState);
                return;
            }

            float distanceToWaypoint = Vector2.Distance(enemy.transform.position, enemy.pathPointList[enemy.currentIndex]);

            if (distanceToWaypoint <= 0.4f)
            {
                enemy.currentIndex++;

                if (enemy.currentIndex >= enemy.pathPointList.Count)
                {
                    enemy.MovementInput = Vector2.zero; // 移動停止
                    stateMachine.ChangeState(enemy.idleState);
                    return;
                }
            }
            direction = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
            enemy.MovementInput = direction;
        }

        enemy.Dash();
    }

    public void GeneratePatroPoint()
    {
        while (true)
        {
            if (enemy.patroPoints.Length == 1)
            {
                enemy.targetPointIndex = 0;
                break;
            }
            else
            {
                int i = Random.Range(0, enemy.patroPoints.Length);

                if (enemy.targetPointIndex != i)
                {
                    enemy.targetPointIndex = i;
                    break;
                }
            }

        }

        enemy.GeneratePath(enemy.patroPoints[enemy.targetPointIndex].position);
    }
}
