using UnityEngine;
using DG.Tweening;

public class Player : Entity
{
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_AttackState attackState { get; private set; }
    public Player_FailedAttackState failedAttackState { get; private set; }
    public Player_RunState runState { get; private set; }
    public Player_HitState hitState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_PrepareChargeState prepareChargeState { get; private set; }
    public Player_ChargeActionState chargeActionState { get; private set; }

    public PlayerInputSet input { get; private set; }
    public Vector2 moveInput { get; private set; }

    public float moveSpeed;

    [SerializeField] private int currentHP;
    private int maxHP = 5;
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
    public float invincibleTimer;
    [HideInInspector] public bool invincibleFlashEnabled = true;
    private SpriteRenderer sr;    //無敵エフェクト
    [Header("Dash Settings")]
    public float dashSpeed = 30f;
    public float dashDuration = 0.7f;
    public float dashCooldown = 2.0f;
    public float dashCooldownTimer;
    [Header("Charge Settings")]
    public GameObject arrowIndicator; //アローのGameObject
    public float chargeDurationReq = 1.5f; // チャージ完了までの時間
    public float chargeSpeed = 40f; //チャージ速度
    public float chargeActionDuration = 0.5f;
    public float arrowOrbitRadius = 1.5f;   //アローが回転する外周の半径
    [HideInInspector] public Vector2 chargeDir; //チャージ方向を保存する
    public float arrowRotationSpeed = 30f;  //回転のスムーズさ
    public float chargeRecoilForce = 5.0f;  //チャージ衝突時のプレイヤーへの反動
    [HideInInspector] public Vector3 currentArrowDir; // 現在のアローの方向を保持
    [Header("FeedBack Settings")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 0.2f;
    [SerializeField] private int shakeVibrato = 20;      //振動頻度
    [SerializeField] private float shakeRandomness = 90; //ランダム角度
    [HideInInspector] public float shakeTimer;
    protected override void Awake()
    {
        base.Awake();

        sr = GetComponentInChildren<SpriteRenderer>();

        input = new PlayerInputSet();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        attackState = new Player_AttackState(this, stateMachine, "attack");
        failedAttackState = new Player_FailedAttackState(this, stateMachine, "failedAttack");

        runState = new Player_RunState(this, stateMachine, "run");
        hitState = new Player_HitState(this, stateMachine, "hit");
        dashState = new Player_DashState(this, stateMachine, "dash");

        prepareChargeState = new Player_PrepareChargeState(this, stateMachine, "precharge");
        chargeActionState = new Player_ChargeActionState(this, stateMachine, "charge");
    }

    protected override void Start()
    {
        base.Start();

        currentHP = maxHP;

        stateMachine.Initialize(idleState);

        if (arrowIndicator != null)
            arrowIndicator.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        Invincible();
        if (noiseCooldownTimer > 0)
            noiseCooldownTimer -= Time.deltaTime;

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (shakeTimer > 0)
            shakeTimer -= Time.deltaTime;
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
            SetAttackStandby(true);
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
        SetAttackStandby(false);
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
    public void MovePlayerToEnemy()
    {
        if (enemyTrans == null)
            return;

        transform.position = enemyTrans.position;
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

    public void TakeDamageByBullet(Vector2 bulletDir, float duration, float force)
    {
        if (invincibleTimer > 0)
            return;

        SetInvincibleFlash(true);
        invincibleTimer = invincibleDuration;

        hitState.SetupHit(bulletDir, duration, force);
        stateMachine.ChangeState(hitState);
    }

    public void TakeDamageByMelee(Vector2 impactDir, float heavyStunDuration, float heavyKnockbackForce, float damage)
    {
        if (invincibleTimer > 0)
            return;

        SetInvincibleFlash(true);
        invincibleTimer = invincibleDuration;

        hitState.SetupHit(impactDir, heavyStunDuration, heavyKnockbackForce);

        stateMachine.ChangeState(hitState);
    }

    private void Invincible()
    {
        if (invincibleTimer > 0)
        {
            invincibleTimer -= Time.deltaTime;

            if (invincibleFlashEnabled && sr != null)
            {
                float alpha = Mathf.PingPong(Time.time * 10.0f, 1.0f);
                Color c = sr.color;
                c.a = (alpha > 0.5f) ? 1f : 0.4f;
                sr.color = c;
            }
            else if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f;
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

    public bool CheckAttackInput()
    {
        if (input.Player.Attack.WasPressedThisFrame() && attackStandby)
            return true;
        else
            return false;
    }

    public bool CheckDashInput()
    {
        if (input.Player.Dash.WasPressedThisFrame() && dashCooldownTimer <= 0)
        {
            return true;
        }
        return false;
    }

    public EntityState GetCurrentState()
    {
        return stateMachine.currentState;
    }

    public void ResetState()
    {
        stateMachine.ChangeState(idleState);
    }

    public void SetInvincibleFlash(bool isInvincible)
    {
        invincibleFlashEnabled = isInvincible;
    }

    public int GetHP()
    {
        return currentHP;
    }

    public void ChangeHP(int count)
    {
        if (currentHP >= maxHP)
            return;

        currentHP += count;
    }

    public void PlayErrorShake()
    {
        shakeTimer = shakeDuration;
        //前のアニメーションを中止
        transform.DOKill(complete: true);

        transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, true);
    }

    public bool IsShaking => shakeTimer > 0;

    public Enemy GetCankillEnemy()
    {
        if (enemyCanKill != null)
            return enemyCanKill;
        else
            return null;
    }

    public void SetAttackStandby(bool canAttack)
    {
        attackStandby = canAttack;
    }

}
