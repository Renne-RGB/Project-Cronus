using UnityEngine;

public class Player_HitState : PlayerState
{
    private float stunTimer;
    private float stunDuration;     //stun継続時間
    private float knockbackForce;   //撃退され距離
    private Vector2 knockbackDir;

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

    public void SetupHit(Vector2 dir, float duration, float force)
    {
        knockbackDir = dir.normalized;
        stunDuration = duration;
        knockbackForce = force;
    }
}