using System.Collections;
using UnityEngine;

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

        enemy.SetCanAssassed(false);

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

        enemy.rb.linearVelocity = Vector2.zero;
        enemy.MovementInput = Vector2.zero;

        enemy.GetPlayerTransform();

        //射撃する時常にプレイヤー方向に向いている
        if (enemy.playerTransform != null)
        {
            Vector3 dir = (enemy.playerTransform.position - enemy.transform.position).normalized;
            if (Mathf.Abs(dir.x) > 0.1f)
                enemy.sr.flipX = dir.x < 0;
        }

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

    private IEnumerator ShootBurstRoutine()
    {
        for (int i = 0; i < enemy.shootNum; i++)
        {
            if (enemy.playerTransform == null)
            {
                stateMachine.ChangeState(enemy.idleState);
                yield break;
            }

            FireCircularBullet();

            yield return new WaitForSeconds(enemy.timeBetweenShots);
        }

        enemy.currentShootCooldown = enemy.shootCooldownDuration;

        stateMachine.ChangeState(enemy.chaseState);
    }

    private void FireCircularBullet()
    {
        if (enemy.isDead)
            return;

        if (enemy.bulletPrefab == null || enemy.firePoint == null) return;

        //プレイヤー方向ベクトル
        Vector2 directionToPlayer = (enemy.playerTransform.position - enemy.firePoint.position).normalized;

        //射撃のランダム角度
        float randomAngle = Random.Range(-enemy.spreadAngle / 2f, enemy.spreadAngle / 2f);

        Vector2 finalDirection = Quaternion.Euler(0, 0, randomAngle) * directionToPlayer;

        //Bullet生成する
        GameObject bullet = Object.Instantiate(enemy.bulletPrefab, enemy.firePoint.position, Quaternion.identity);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = finalDirection * enemy.bulletSpeed;
        }

        Object.Destroy(bullet, 5f);
    }
}