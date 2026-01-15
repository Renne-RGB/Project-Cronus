using UnityEngine;

public class Player_DashState : PlayerState
{
    private float dashStartTime;
    private Vector2 savedDashDir;
    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        dashStartTime = Time.time;

        player.invincibleTimer = player.dashDuration;

        savedDashDir = player.moveInput;
        if (savedDashDir == Vector2.zero)
        {
            savedDashDir = new Vector2(player.anim.GetFloat("x"), player.anim.GetFloat("y"));

            if (savedDashDir == Vector2.zero)
                savedDashDir = Vector2.right;
        }
        savedDashDir.Normalize();
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(savedDashDir.x * player.dashSpeed, savedDashDir.y * player.dashSpeed);
        player.anim.SetFloat("x", savedDashDir.x);
        player.anim.SetFloat("y", savedDashDir.y);

        if (Time.time >= dashStartTime + player.dashDuration)
        {
            player.SetVelocity(0, 0);
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, 0);

        player.SetInvincible(true);
    }
}