using UnityEngine;
using System.Collections;

public class Enemy_ShootState : EnemyState
{
    private Coroutine shootingCoroutine;

    public Enemy_ShootState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetAlert(true);

        // 进状态时立刻清零速度，防止滑行
        enemy.MovementInput = Vector2.zero;
        enemy.rb.linearVelocity = Vector2.zero;

        if (enemy.playerTransform != null)
        {
            shootingCoroutine = enemy.StartCoroutine(ShootBurstRoutine());
        }
    }

    public override void Update()
    {
        base.Update();

        // 1. 持续强制停止移动
        enemy.rb.linearVelocity = Vector2.zero;
        enemy.MovementInput = Vector2.zero;

        // 2. 更新玩家位置
        enemy.GetPlayerTransform();

        // 3. 简单的面向逻辑：射击时让敌人看着玩家
        if (enemy.playerTransform != null)
        {
            Vector3 dir = (enemy.playerTransform.position - enemy.transform.position).normalized;
            if (Mathf.Abs(dir.x) > 0.1f)
                enemy.sr.flipX = dir.x < 0;
        }

        // 4. 退出条件 (由动画事件触发)
        if (triggerCalled)
            stateMachine.ChangeState(enemy.idleState);
    }

    public override void Exit()
    {
        base.Exit();
        if (shootingCoroutine != null)
        {
            enemy.StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }
    }

    // --- 核心：三连发协程 ---
    private IEnumerator ShootBurstRoutine()
    {
        for (int i = 0; i < 3; i++)
        {
            // 检查发射条件 (如果玩家突然消失或跑太远，可以提前中断)
            if (enemy.playerTransform == null)
            {
                stateMachine.ChangeState(enemy.idleState);
                yield break;
            }

            FireCircularBullet();

            // 等待下一发间隔
            yield return new WaitForSeconds(enemy.timeBetweenShots);
        }

        // --- 三连发结束 ---

        // 1. 重置冷却时间 (设定为 2秒)
        enemy.currentShootCooldown = enemy.shootCooldownDuration;

        // 2. 【核心】手动切换回 ChaseState
        // 退出 ShootState 后，EntityState 会自动处理动画参数，射击动画停止。
        // 回到 ChaseState 后，因为有 CD 存在，它会执行追逐/移动逻辑，直到 2秒 CD 结束。
        stateMachine.ChangeState(enemy.chaseState);
    }

    // --- 核心：发射逻辑 ---
    private void FireCircularBullet()
    {
        if (enemy.bulletPrefab == null || enemy.firePoint == null) return;

        // 1. 获取准确的朝向玩家的向量
        Vector2 directionToPlayer = (enemy.playerTransform.position - enemy.firePoint.position).normalized;

        // 2. 计算随机偏移角度 (-15度 到 +15度)
        float randomAngle = Random.Range(-enemy.spreadAngle / 2f, enemy.spreadAngle / 2f);

        // 3. 使用四元数旋转向量，得到最终的【飞行速度方向】
        Vector2 finalDirection = Quaternion.Euler(0, 0, randomAngle) * directionToPlayer;

        // 4. 生成子弹
        // 因为是圆形，Rotation 这里直接用 Quaternion.identity (不旋转) 即可
        GameObject bullet = Object.Instantiate(enemy.bulletPrefab, enemy.firePoint.position, Quaternion.identity);

        // 5. 赋予速度
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = finalDirection * enemy.bulletSpeed;
        }

        Object.Destroy(bullet, 5f);
    }
}