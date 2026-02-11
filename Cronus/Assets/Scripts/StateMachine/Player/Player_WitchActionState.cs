using UnityEngine;

public class Player_WitchActionState : Player_ChargeActionState
{
    public Player_WitchActionState(Player player, StateMachine stateMachine, string animBoolName)
        : base(player, stateMachine, animBoolName) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        player.witchTimeManager.DeactivateWitchTime();
    }

    protected override void HandleCost()
    {

    }
}