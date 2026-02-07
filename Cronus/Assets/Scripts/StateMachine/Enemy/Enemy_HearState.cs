using UnityEngine;

public class Enemy_HearState : EnemyState
{
    public Enemy_HearState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.rb.linearVelocity = Vector2.zero;

        if (SearchRingManager.Instance.HasActiveRing())
        {
            Vector3 target = SearchRingManager.Instance.LastTargetPosition;

            Vector2 dir = (target - enemy.transform.position).normalized;
            if (dir.x != 0) enemy.sr.flipX = dir.x < 0;
        }
    }

    public override void Update()
    {
        base.Update();

        enemy.GetPlayerTransform();

        enemy.FindTargetPlayer();

        enemy.rb.linearVelocity = Vector2.zero;

        if (triggerCalled)
        {
            DetermineNextState();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    private void DetermineNextState()
    {
        //プレイヤーに見つかったら
        if (enemy.playerTransform != null)
        {
            stateMachine.ChangeState(enemy.alertState);
        }
        //赤い円をチェック
        else if (SearchRingManager.Instance.HasActiveRing())
        {
            stateMachine.ChangeState(enemy.investigateState);
        }
        else
        {
            float distToLastPos = Vector2.Distance(enemy.transform.position, SearchRingManager.Instance.LastTargetPosition);
            if (distToLastPos > 2f)
                stateMachine.ChangeState(enemy.investigateState);
            else
                stateMachine.ChangeState(enemy.idleState);
        }
    }
}