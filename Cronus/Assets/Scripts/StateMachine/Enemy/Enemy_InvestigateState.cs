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

        enemy.pathPointList = null;
        enemy.currentIndex = 0;

        Vector3 targetPos = SearchRingManager.Instance.LastTargetPosition;
        enemy.GeneratePath(targetPos);
    }

    public override void Update()
    {
        base.Update();

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

        // 3. 执行移动逻辑
        MoveAndCheckArrival();
    }

    private void MoveAndCheckArrival()
    {
        enemy.AutoPath();

        if (enemy.pathPointList != null && enemy.currentIndex < enemy.pathPointList.Count)
        {
            enemy.MovementInput = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
            enemy.Dash();

            //赤い円に到達しているか
            float distToTarget = Vector2.Distance(enemy.transform.position, SearchRingManager.Instance.LastTargetPosition);
            if (distToTarget <= 1.2f)
            {
                ArriveAtDestination();
            }
        }
        else
        {
            enemy.MovementInput = Vector2.zero;
            enemy.Dash();
        }
    }

    private void ArriveAtDestination()
    {
        if (hasReachedDestination)
            return;
        hasReachedDestination = true;

        enemy.MovementInput = Vector2.zero;
        enemy.rb.linearVelocity = Vector2.zero;

        stateMachine.ChangeState(enemy.searchState);
    }
}