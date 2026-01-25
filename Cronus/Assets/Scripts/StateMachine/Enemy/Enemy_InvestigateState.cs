using UnityEngine;

public class Enemy_InvestigateState : EnemyState
{
    private Vector2 direction;
    private bool hasReachedDestination = false;

    public Enemy_InvestigateState(Enemy enemy, StateMachine stateMachine, string animBoolName) 
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.SetAlert(false);
        hasReachedDestination = false;

        // 【关键修复 1】进入状态时，先把旧路径清空！
        // 否则 Update 会在下一帧立刻读取旧的 pathPointList，导致误判为“已到达”
        enemy.pathPointList = null; 
        enemy.currentIndex = 0;

        Vector3 targetPos = SearchRingManager.Instance.LastTargetPosition;
        enemy.GeneratePath(targetPos);
    }

    public override void Update()
    {
        base.Update();

        // ... (视觉检测和SearchRing检查代码保持不变) ...
        enemy.GetPlayerTransform();
        if (enemy.playerTransform != null)
        {
            enemy.FindTargetPlayer();
            return;
        }

        if (!SearchRingManager.Instance.HasActiveRing())
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        // 【关键修复 2】在路径还没算出来之前，不要执行移动逻辑
        // 只有当 list 不为空 且 确实有数据时才开始走
        if (enemy.pathPointList == null || enemy.pathPointList.Count == 0)
        {
            // 可以在这里让速度归零，等待寻路结果
            enemy.rb.linearVelocity = Vector2.zero; 
            return;
        }

        // --- 以下是原本的移动逻辑 ---
        
        // 检查是否到达终点
        float distanceToTarget = Vector2.Distance(enemy.transform.position, SearchRingManager.Instance.LastTargetPosition);
            
        // 这里有一个小优化：如果路径被堵死导致寻路终点无法到达目标点，依靠 index 判断是可以的
        // 但为了防止过早停下，建议严格检查 index 是否真的跑完了
        if (distanceToTarget <= 0.5f || enemy.currentIndex >= enemy.pathPointList.Count)
        {
            ArriveAtDestination();
            return;
        }

        Vector3 currentWaypoint = enemy.pathPointList[enemy.currentIndex];
        float distanceToWaypoint = Vector2.Distance(enemy.transform.position, currentWaypoint);

        if (distanceToWaypoint <= 0.5f)
        {
            enemy.currentIndex++;
            if (enemy.currentIndex >= enemy.pathPointList.Count)
            {
                ArriveAtDestination();
                return;
            }
        }

        direction = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
        enemy.MovementInput = direction;
        enemy.Dash();
    }

    private void ArriveAtDestination()
    {
        if (hasReachedDestination) return;
        hasReachedDestination = true;

        enemy.MovementInput = Vector2.zero;
        enemy.rb.linearVelocity = Vector2.zero;
        
        stateMachine.ChangeState(enemy.searchState);
    }
}