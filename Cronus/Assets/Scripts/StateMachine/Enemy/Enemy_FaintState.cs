using UnityEngine;

public class Enemy_FaintState : EnemyState
{
    private float faintTimer;
    private float defaultDrag; // 元の抵抗値を保存する変数

    public Enemy_FaintState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //気絶時間設定
        faintTimer = enemy.faintDuration;

        //移動入力を無効化
        enemy.MovementInput = Vector2.zero;

        //摩擦を増やして、反動で滑った後に素早く停止させる
        defaultDrag = enemy.rb.linearDamping;
        enemy.rb.linearDamping = enemy.faintDrag;
    }

    public override void Update()
    {
        base.Update();

        faintTimer -= Time.deltaTime;

        if (faintTimer <= 0)
        {
            //復帰してIdleへ
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        //終了時は抵抗値と速度リセット
        enemy.rb.linearDamping = defaultDrag;
        enemy.rb.linearVelocity = Vector2.zero;
        
    }
}