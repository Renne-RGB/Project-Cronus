using UnityEngine;

public class Enemy_SearchState : EnemyState
{
    public Enemy_SearchState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // 1. 生成前往红圈（最后已知位置）的路径
        Vector3 targetPos = SearchRingManager.Instance.LastTargetPosition;
        enemy.GeneratePath(targetPos);
        
        enemy.currentSpeed = enemy.moveSpeed; // 使用普通移动速度
    }

    public override void Update()
    {
        base.Update();

        // --- 1. 优先检查是否重新发现了玩家 ---
        enemy.GetPlayerTransform();
        if (enemy.playerTransform != null)
        {
            // 如果在搜索途中看见了玩家，直接进 Chase（跳过 Alert，因为已经是警戒状态了）
            stateMachine.ChangeState(enemy.chaseState);
            return;
        }

        // --- 2. 寻路逻辑 (沿用 MoveState 的逻辑) ---
        if (enemy.pathPointList != null && enemy.currentIndex < enemy.pathPointList.Count)
        {
            // 移动方向计算
            Vector2 direction = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
            enemy.MovementInput = direction;
            enemy.Dash();

            // 到达当前路点
            if (Vector2.Distance(enemy.transform.position, enemy.pathPointList[enemy.currentIndex]) <= 0.4f)
            {
                enemy.currentIndex++;
            }
        }
        else
        {
            // --- 3. 抵达终点（红圈位置）---
            enemy.rb.linearVelocity = Vector2.zero;
            
            // 到了红圈还没看到人，说明跟丢了 -> 解除警戒，回 Idle
            enemy.SetAlert(false);
            enemy.ClearSearchRing(); // 清除红圈
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}