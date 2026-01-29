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
        if (enemy.pathPointList == null || enemy.pathPointList.Count == 0)
        {
            enemy.rb.linearVelocity = Vector2.zero; 
            return;
        }

        //赤い円までの距離
        float distanceToTarget = Vector2.Distance(enemy.transform.position, SearchRingManager.Instance.LastTargetPosition);
            
        //詰まるチェック
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