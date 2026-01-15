using UnityEngine;

public class Enemy_ChaseState : EnemyState
{
    public Enemy_ChaseState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetAlert(true);
        enemy.moveSpeed = 3.0f;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        enemy.GetPlayerTransform();

        if (enemy.playerTransform != null)
        {
            // プレイヤーを視認している場合
            SearchRingManager.Instance.LastTargetPosition = enemy.playerTransform.position;

            enemy.currentChaseTimer = enemy.chaseDuration;
            enemy.ResetSearchRingFlag();

            if (enemy.distance <= enemy.cqbDistance)
            {
                stateMachine.ChangeState(enemy.cqbState);
                return;
            }
            else if (enemy.distance <= enemy.shootRange && enemy.currentShootCooldown <= 0)
            {
                stateMachine.ChangeState(enemy.shootState);
                return;
            }
        }
        else
        {
            // プレイヤーを見失った場合
            // UpdateSharedSearchRing内部で「一度だけ生成する」
            if (enemy.GetAlert())
            {
                enemy.UpdateSharedSearchRing(SearchRingManager.Instance.LastTargetPosition);
                //プレイヤー位置失ったら暗殺状態になる
                enemy.SetCanAssassed(true);
            }
        }

        enemy.AutoPath();

        if (enemy.pathPointList != null && enemy.currentIndex < enemy.pathPointList.Count)
        {
            enemy.MovementInput = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
            enemy.Dash();
        }
        else
        {
            enemy.MovementInput = Vector2.zero;
            enemy.Dash();
        }
    }
}