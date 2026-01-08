using UnityEngine;

public class Player_HitState : PlayerState
{
    private float stunTimer;
    private float stunDuration = 1.0f;
    private Vector2 knockbackDir;
    private float knockbackForce = 10.0f;

    public Player_HitState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stunTimer = stunDuration;

        player.SetVelocity(knockbackDir.x * knockbackForce, knockbackDir.y * knockbackForce);

        player.anim.SetFloat("x", knockbackDir.x);
        player.anim.SetFloat("y", knockbackDir.y);
    }

    public override void Update()
    {
        base.Update();

        stunTimer -= Time.deltaTime;

        if (player.rb.linearVelocity.magnitude > 0.1f)
        {
            player.SetVelocity(player.rb.linearVelocity.x * 0.95f, player.rb.linearVelocity.y * 0.95f);
        }

        if (stunTimer <= 0)
        {
            player.SetVelocity(0, 0);
            stateMachine.ChangeState(player.idleState);
        }
    }

    // 用于在进入状态前设置击退方向
    public void SetKnockbackDirection(Vector2 direction)
    {
        knockbackDir = direction.normalized;
    }
}