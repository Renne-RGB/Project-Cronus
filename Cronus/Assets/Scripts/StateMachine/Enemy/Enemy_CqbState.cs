using UnityEngine;

public class Enemy_CqbState : EnemyState
{
    public Enemy_CqbState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetAlert(true);
    }

    public override void Update()
    {
        base.Update();

        //攻撃状態の移動速度常に0にする
        enemy.rb.linearVelocity = Vector2.zero;

        enemy.GetPlayerTransform();     //プレイヤーの位置を取る

        if (triggerCalled)
        {
            // --- 在切换状态前执行攻击判定 ---
            PerformCqbAttack();

            stateMachine.ChangeState(enemy.idleState);
        }
    }

    private void PerformCqbAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(enemy.transform.position, enemy.cqbAttackRadius, enemy.playerLayer);

        foreach (var hit in hits)
        {
            Player player = hit.GetComponent<Player>();
            if (player != null)
            {
                //撃退方向
                Vector2 dirToPlayer = (player.transform.position - enemy.transform.position).normalized;

                player.TakeDamageByMelee(dirToPlayer, enemy.cqbStunDuration, enemy.cqbKnockbackForce);

                break; 
            }
        }
    }

}
