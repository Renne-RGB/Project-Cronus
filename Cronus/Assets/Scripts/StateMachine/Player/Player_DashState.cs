using UnityEngine;

public class Player_DashState : PlayerState
{
    public float dashStartTime;
    private Vector2 savedDashDir;
    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        dashStartTime = Time.unscaledTime;

        player.invincibleTimer = player.dashDuration;

        savedDashDir = player.moveInput;
        if (savedDashDir == Vector2.zero)
        {
            savedDashDir = new Vector2(player.anim.GetFloat("x"), player.anim.GetFloat("y"));

            if (savedDashDir == Vector2.zero)
                savedDashDir = Vector2.right;
        }
        savedDashDir.Normalize();
        player.SetInvincibleFlash(false);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(savedDashDir.x * player.dashSpeed, savedDashDir.y * player.dashSpeed);

        if (Time.unscaledTime - dashStartTime >= player.dashDuration)
        {
            player.SetVelocity(0, 0);
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, 0);

        player.SetInvincibleFlash(true);
    }
}