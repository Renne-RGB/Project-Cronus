using UnityEngine;

//プレイヤーの待機や移動の共通処理State(移動と待機が共有できる状態 例えば移動と待機状態両方攻撃やスキルを使える)
public class Player_BasicState : EntityState
{
    public Player_BasicState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.input.Player.Attack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.attackState);
        }
    }
}
