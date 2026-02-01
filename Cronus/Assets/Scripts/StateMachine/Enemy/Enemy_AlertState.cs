using Unity.VisualScripting;
using UnityEngine;

public class Enemy_AlertState : EnemyState
{
    private float alertTimer;
    private const float ALERT_DURATION = 1.0f;
    public Enemy_AlertState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.SetAlert(true);
        alertTimer = ALERT_DURATION;
        enemy.SetVelocity(0, 0);
    }

    public override void Update()
    {
        base.Update();

        //ずっとプレイヤーに向けて
        enemy.GetPlayerTransform();
        if (enemy.playerTransform != null)
        {
            Vector2 dir = (enemy.playerTransform.position - enemy.transform.position).normalized;
            enemy.MovementInput = dir;
        }

        alertTimer -= Time.deltaTime;

        if (alertTimer <= 0)
            DetermineNextState();

        if (triggerCalled)
            enemy.SetCanAssassed(false);
    }

    private void DetermineNextState()
    {
        if (enemy.playerTransform == null)
        {
            stateMachine.ChangeState(enemy.idleState);
            return;
        }
        //距離によってStateを切り替え
        if (enemy.canMelee && enemy.distance <= enemy.cqbDistance)
        {
            stateMachine.ChangeState(enemy.cqbState);
        }
        else if (enemy.canShoot && enemy.distance <= enemy.shootRange)
        {
            stateMachine.ChangeState(enemy.shootState);
        }
        else if(enemy.canChase)
        {
            stateMachine.ChangeState(enemy.chaseState);
        }
        else
        {
            //ビビる
        }
    }
}
