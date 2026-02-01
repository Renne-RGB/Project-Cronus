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
        // 退出状态时不需特殊处理，因为 Enter 下一个状态（如 Chase）会重新设置速度
    }

    private void DetermineNextState()
    {
        //プレイヤーに見つけったら
        if (enemy.playerTransform != null)
        {
            stateMachine.ChangeState(enemy.alertState);
        }
        //赤い円があれば
        else if (SearchRingManager.Instance.HasActiveRing())
        {
            stateMachine.ChangeState(enemy.investigateState);
        }
        //なんだねこか
        else
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }
}