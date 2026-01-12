using UnityEngine;

public class Enemy_KnockbackState : EnemyState
{
    private float timer;

    public Enemy_KnockbackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        timer = 1.5f;
        
        enemy.MovementInput = Vector2.zero;
    }

    public override void Update()
    {
        base.Update();

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // 壁にぶつからずに時間が経過した場合
            // AlertFlagをTrueにしてIdleに戻る
            enemy.SetAlert(true);
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();

    }
}