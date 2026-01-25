using UnityEngine;

public class Player_HiddenState : PlayerState
{
    private RigidbodyConstraints2D originalConstraints;

    public Player_HiddenState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        originalConstraints = player.rb.constraints;

        // 2. 彻底冻结位置和旋转
        // FreezeAll = FreezePositionX | FreezePositionY | FreezeRotation
        // 这样玩家就像钉在地上一样，有碰撞体但不会被推走，也不会滑行
        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // 3. 强制清零当前速度 (防止惯性残留)
        player.SetVelocity(Vector2.zero);
        player.rb.linearVelocity = Vector2.zero;

        // 4. 标记状态并瞬移到柜子中心 (视觉对齐)
        player.SetHidden(true);
        if (player.currentHideSpot != null)
        {
            player.transform.position = player.currentHideSpot.GetHidePosition();
        }

        // 【关键】绝对不要关闭 Collider (playerCollider.enabled = false)
        // 只要 Collider 开着，Enemy.cs 里的射线检测就能打中，攻击判定也能触发
    }
    public override void Update()
    {
        base.Update();

        //Activeキーで出る
        if (player.input.Player.Active.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        //暗殺
        if (player.CheckAttackInput())
        {
            Enemy target = player.GetCankillEnemy();
            if (target != null)
            {
                stateMachine.ChangeState(player.attackState);
                return;
            }
        }
        
        player.rb.linearVelocity = Vector2.zero;
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.constraints = originalConstraints;

        player.SetHidden(false);
    }
}