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

        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;

        player.SetVelocity(Vector2.zero);
        player.rb.linearVelocity = Vector2.zero;

        player.SetHidden(true);
        if (player.currentHideSpot != null)
        {
            player.transform.position = player.currentHideSpot.GetHidePosition();
        }
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