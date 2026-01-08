using System;
using NUnit.Framework.Interfaces;
using UnityEngine;

public class Player : Entity
{
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_AttackState attackState { get; private set; }
    public Player_RunState runState { get; private set; }
    public Player_HitState hitState { get; private set; }

    public PlayerInputSet input { get; private set; }
    public Vector2 moveInput { get; private set; }

    public float moveSpeed;

    [SerializeField] private Enemy enemyCanKill;     //攻撃範囲内の敵
    [SerializeField] private Enemy lockedEnemy;      //一番最初の敵を記録する
    [SerializeField] private Transform enemyTrans;     //敵死体の生成座標
    [SerializeField] public bool attackStandby = false;     //攻撃できるか
    [Header("Sound Settings")]
    [SerializeField] private float runNoiseRadius = 5.0f; // 走る時の音の範囲
    [SerializeField] private LayerMask enemyLayer;        // 敵のレイヤー
    [SerializeField] private GameObject soundWavePrefab;
    public float noiseCooldownTimer = 0f;
    [Header("Combat Settings")]
    [SerializeField] private float invincibleDuration = 2.0f;   //無敵時間
    private float invincibleTimer;
    private SpriteRenderer sr;    //無敵エフェクト

    protected override void Awake()
    {
        base.Awake();

        sr = GetComponentInChildren<SpriteRenderer>();

        input = new PlayerInputSet();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        attackState = new Player_AttackState(this, stateMachine, "attack");
        runState = new Player_RunState(this, stateMachine, "run");
        hitState = new Player_HitState(this, stateMachine, "hit");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        Invincible();
        if (noiseCooldownTimer > 0)
        {
            noiseCooldownTimer -= Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        input.Enable();

        //input.Player.Movement.started       //押す
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();                 //長押し
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;          //キャンセル

        //input.Player.Attack
    }

    private void OnDisable()
    {
        input.Disable();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        //自分collider範囲内の敵チェック
        if (!other.CompareTag("EnemyHit"))
            return;

        //既に保存する敵がいれば再び実行しない
        if (attackStandby)
            return;

        Enemy enemyComponent = other.GetComponentInParent<Enemy>();

        if (enemyComponent != null)
        {
            //今の敵を保存する
            enemyCanKill = enemyComponent;
            lockedEnemy = enemyCanKill;
            enemyTrans = enemyCanKill.transform;
            attackStandby = true;
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        //自分collider範囲内の敵チェック
        if (!other.CompareTag("Enemy"))
            return;

        //一番最初記録した敵じゃなければ無視する
        Enemy leaveEnemy = other.GetComponent<Enemy>();
        if (leaveEnemy != lockedEnemy)
            return;

        enemyCanKill = null;
        enemyTrans = null;
        lockedEnemy = null;
        attackStandby = false;
    }

    //暗殺できる敵を探す
    public bool CheckEnemyCanKill()
    {
        bool canKill = false;

        if (attackStandby && enemyCanKill != null)
            canKill = true;

        return canKill;
    }

    public void GenerateEnemyBody()
    {
        if (enemyTrans == null)
            return;
    }

    //プレイヤー暗殺する時座標を敵の位置に移動する
    public void MovePlayerToDeadEnemy()
    {
        if (enemyTrans == null)
            return;

        transform.position = enemyTrans.position;

        //敵objectを削除
        Destroy(enemyCanKill.gameObject);
    }

    public void EmitRunNoise()
    {
        if (soundWavePrefab != null)
        {
            GameObject wave = Instantiate(soundWavePrefab, transform.position, Quaternion.identity);

            SoundWave waveScript = wave.GetComponent<SoundWave>();
            if (waveScript != null)
            {
                waveScript.Setup(runNoiseRadius);
            }
        }

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, runNoiseRadius, enemyLayer);
        foreach (var hit in hitEnemies)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.OnHearSound(transform.position); //
            }
        }
    }

    // エディタ上で音の範囲を可視化する
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, runNoiseRadius);
    }

    public void TakeDamage(Vector2 bulletDir)
    {
        if (invincibleTimer > 0)
            return;
            
        invincibleTimer = invincibleDuration;

        hitState.SetKnockbackDirection(bulletDir);
        stateMachine.ChangeState(hitState);
    }

    private void Invincible()
    {
        if (invincibleTimer > 0)
        {
            invincibleTimer -= Time.deltaTime;

            if (sr != null)
            {
                float alpha = Mathf.PingPong(Time.time * 10.0f, 1.0f);

                Color c = sr.color;
                c.a = (alpha > 0.5f) ? 1f : 0.4f; // 硬切闪烁看起来更有“受击感”
                sr.color = c;
            }
        }
        else
        {
            if (sr != null && sr.color.a < 1f)
            {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
        }
    }
}
