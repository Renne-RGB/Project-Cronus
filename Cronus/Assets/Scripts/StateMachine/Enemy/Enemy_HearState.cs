using UnityEngine;

public class Enemy_HearState : EnemyState
{
    private float hearTimer;
    private const float HEAR_DURATION = 2.0f;
    private float originalSpeed; // 用来保存原来的速度
    
    public Enemy_HearState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        hearTimer = HEAR_DURATION;
        enemy.rb.linearVelocity = Vector2.zero; 

        // 【关键修复 1】设置移动速度
        // 如果没有这行，currentSpeed 可能是 0，导致 Dash() 不执行移动
        //originalSpeed = enemy.currentSpeed; // 备份一下（如果需要）
        //enemy.currentSpeed = enemy.moveSpeed; // 或者设置一个特定的值，比如 2.0f

        // 如果是听到声音进来的，可以在这里立刻更新一次朝向
        if (SearchRingManager.Instance.HasActiveRing())
        {
             Vector3 target = SearchRingManager.Instance.LastTargetPosition;
             enemy.MovementInput = (target - enemy.transform.position).normalized;
        }
    }

    public override void Update()
    {
        base.Update();

        // 1. 视觉更新
        enemy.GetPlayerTransform();

        // 2. 状态判定 (Sus/Alert)
        enemy.FindTargetPlayer();
        if (stateMachine.currentState != this) return;

        // 3. 移动逻辑
        if (enemy.playerTransform != null)
        {
            // 看到玩家，面向玩家
            Vector2 dir = (enemy.playerTransform.position - enemy.transform.position).normalized;
            enemy.MovementInput = dir;
            // 看到玩家时通常停下
            enemy.rb.linearVelocity = Vector2.zero; 
        }
        else if (SearchRingManager.Instance.HasActiveRing())
        {
            // 看不到玩家，但有声源：走向 SearchRing
            Vector3 target = SearchRingManager.Instance.LastTargetPosition;
            Vector2 dir = (target - enemy.transform.position).normalized;
            
            enemy.MovementInput = dir;
            
            // 【关键调用】Dash 依赖于 currentSpeed > 0
            enemy.Dash();
            
            // 【可选优化】如果你希望在移动时播放 Move 动画而不是 Hear 动画
            // 可以在这里参考 SuspiciousState 的做法手动 SetBool("Move", true)
            // 但如果你想保持警觉移动的姿态（如果有对应动画），则不需要改
            if(enemy.anim != null) enemy.anim.SetBool("move", true);
        }
        else
        {
            // 没目标，停下
            enemy.MovementInput = Vector2.zero;
            enemy.rb.linearVelocity = Vector2.zero;
            if(enemy.anim != null) enemy.anim.SetBool("move", false);
        }

        // 4. 计时器
        hearTimer -= Time.deltaTime;

        if (hearTimer <= 0)
            DetermineNextState();
    }

    public override void Exit()
    {
        base.Exit();
        // 退出时记得关掉 move 动画（如果你上面开启了的话）
        if(enemy.anim != null) enemy.anim.SetBool("move", false);
        
        // 速度归零或还原
        //enemy.currentSpeed = 0f; 
        enemy.rb.linearVelocity = Vector2.zero;
    }

    private void DetermineNextState()
    {
        if (enemy.playerTransform != null)
        {
            if (enemy.distance <= enemy.cqbDistance)
                stateMachine.ChangeState(enemy.cqbState);
            else if (enemy.distance <= enemy.shootRange)
                stateMachine.ChangeState(enemy.shootState);
            else
                stateMachine.ChangeState(enemy.chaseState);
        }
        else
        {
            if (SearchRingManager.Instance.HasActiveRing())
            {
                enemy.SetAlert(true);
                stateMachine.ChangeState(enemy.chaseState); 
            }
            else
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }
    }
}