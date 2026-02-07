using UnityEngine;

public class Enemy_ChaseState : EnemyState
{
    //プレイヤーへの追跡を失ったかどうかを記録する
    private bool hasLostPlayer = false;

    public Enemy_ChaseState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetAlert(true);
        hasLostPlayer = false;
    }

    public override void Update()
    {
        base.Update();
        enemy.GetPlayerTransform();

        if (enemy.targetPlayer == null)
            return;

        float distToPlayer = Vector2.Distance(enemy.transform.position, enemy.targetPlayer.transform.position);
        bool isVisible = enemy.playerTransform != null && !enemy.targetPlayer.IsHidden();
        bool hasSearchRing = SearchRingManager.Instance.HasActiveRing();

        // プレイヤーを完全に失った
        if (isVisible)
        {
            hasLostPlayer = false;
        }

        //まだプレイヤーを見失っていない場合のみ、感知距離内であれば隠れているプレイヤーも追跡する
        if (!hasLostPlayer && (isVisible || distToPlayer <= enemy.suspiciousDistance))
        {
            if (enemy.hasDirectLineOfSight)
            {
                SearchRingManager.Instance.LastTargetPosition = enemy.targetPlayer.transform.position;

                enemy.ClearSearchRing();
            }
            
            enemy.currentChaseTimer = enemy.chaseDuration;

            if (CheckAttackConditions(distToPlayer, true))
                return;

            if (distToPlayer > enemy.minChaseMoveDistance)
            {
                MoveTowardsTarget();
            }
            else
            {
                enemy.MovementInput = Vector2.zero;
                enemy.Dash();
            }
        }
        else
        {
            // プレイヤーを見失ったため、searchモードへ移行
            hasLostPlayer = true;
            Vector3 lastSeenPos = SearchRingManager.Instance.LastTargetPosition;
            float distToLastPos = Vector2.Distance(enemy.transform.position, lastSeenPos);

            //赤い円が時間経過で自然に消滅した場合は、ここで再生成されない
            if (enemy.GetAlert() && !hasSearchRing && !isVisible)
            {
                if (distToLastPos > enemy.minSearchRingDistance)
                {
                    enemy.UpdateSharedSearchRing(lastSeenPos);
                    enemy.SetCanAssassed(true);// 暗殺可能な状態に設定
                }
            }

            MoveTowardsTarget();

            // Vector3 targetPos = SearchRingManager.Instance.LastTargetPosition;
            // float distanceToRing = Vector2.Distance(enemy.transform.position, targetPos);

            //リングの位置に到達したらSearchStateに切り替え
            if (distToLastPos <= 1.2f)
            {
                stateMachine.ChangeState(enemy.searchState);
                return;
            }
        }

        // MoveTowardsTarget();
    }

    private bool CheckAttackConditions(float distance, bool ignoreHidden)
    {
        //攻撃対象有効か判定
        bool canBeTargeted = (enemy.playerTransform != null &&
                             !enemy.targetPlayer.IsHidden()) || ignoreHidden;

        if (!canBeTargeted)
            return false;

        if (enemy.canMelee && distance <= enemy.cqbDistance)
        {
            stateMachine.ChangeState(enemy.cqbState);
            return true;
        }
        if (enemy.canShoot && distance <= enemy.shootRange && enemy.currentShootCooldown <= 0)
        {
            stateMachine.ChangeState(enemy.shootState);
            return true;
        }

        return false;
    }

    private void MoveTowardsTarget()
    {
        enemy.AutoPath();

        if (enemy.pathPointList != null && enemy.currentIndex < enemy.pathPointList.Count)
        {
            enemy.MovementInput = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
            enemy.Dash();
        }
        else
        {
            enemy.MovementInput = Vector2.zero;
            enemy.Dash();
        }
    }
}