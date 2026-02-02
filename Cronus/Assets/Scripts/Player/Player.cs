using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

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
    public Player_HiddenState hiddenState { get; private set; }
    public Player_WitchActionState witchActionState { get; private set; }

    public PlayerInputSet input { get; private set; }
    public Vector2 moveInput { get; private set; }

    public float moveSpeed;

    [SerializeField] private int currentHP;
    private int maxHP = 5;
    [SerializeField] private Enemy enemyCanKill;     //攻撃範囲内の敵
    private Enemy lastFrameTarget;      //最後1フレームの定期を記録する
    [SerializeField] private Enemy lockedEnemy;      //一番最初の敵を記録する
    [SerializeField] private Transform enemyTrans;     //敵死体の生成座標
    [SerializeField] public bool attackStandby = false;     //攻撃できるか
    [Header("Sound Settings")]
    [SerializeField] private float runNoiseRadius = 5.0f; // 走る時の音の範囲
    [SerializeField] private LayerMask enemyLayer;        // 敵のレイヤー
    [SerializeField] private GameObject soundWavePrefab;
    public float noiseCooldownTimer = 0f;
    private float noiseInterval = 0.4f;
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
    public GameObject chargeBarHolder;
    public Image chargeBarImage;
    public InteractionIcon interactionIcon; //Icon
    [HideInInspector] public Vector2 chargeDir; //チャージ方向を保存する
    public float arrowRotationSpeed = 30f;  //回転のスムーズさ
    public float chargeRecoilForce = 5.0f;  //チャージ衝突時のプレイヤーへの反動
    [HideInInspector] public Vector3 currentArrowDir; // 現在のアローの方向を保持
    [Header("FeedBack Settings")]
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 0.2f;
    [SerializeField] private int shakeVibrato = 20;      //振動頻度
    [SerializeField] private float shakeRandomness = 90; //ランダム角度
    [HideInInspector] public float shakeTimer;
    private List<Enemy> enemiesInRange = new List<Enemy>();
    [Header("Stealth Settings")]
    public HideSpot currentHideSpot;
    [Header("Hierarchy References")]
    public Transform visuals;
    public WitchTimeManager witchTimeManager;
    public bool isHidden = false;
    private List<HideSpot> hideSpotsInRange = new List<HideSpot>();
    private HideSpot lastFrameHideSpot;
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
        hiddenState = new Player_HiddenState(this, stateMachine, "hide");
        witchActionState = new Player_WitchActionState(this, stateMachine, "witch");

        witchActionState = new Player_WitchActionState(this, stateMachine, "witch");

        if (witchTimeManager != null)
        {
            witchTimeManager.Init(this);
        }
        else
        {

            Debug.LogError("WitchTimeManager Null");
        }
    }

    protected override void Start()
    {
        base.Start();

        currentHP = maxHP;

        stateMachine.Initialize(idleState);

        if (arrowIndicator != null)
            arrowIndicator.SetActive(false);

        if (chargeBarHolder != null)
            chargeBarHolder.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (witchTimeManager != null && witchTimeManager.IsWitchTimeActive)
        {
            anim.speed = 1f / witchTimeManager.slowMotionFactor;
        }
        else
        {
            anim.speed = 1f;
        }

        if (enemiesInRange.Count > 0)
        {
            UpdateClosestEnemy();
        }

        Invincible();

        if (noiseCooldownTimer > 0)
            noiseCooldownTimer -= Time.unscaledDeltaTime;

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.unscaledDeltaTime;

        if (shakeTimer > 0)
            shakeTimer -= Time.unscaledDeltaTime;

        if (input.Player.Test.WasPressedThisFrame())
        {
            witchTimeManager.ActivateWitchTime();
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
        base.OnTriggerEnter2D(other);

        //自分collider範囲内の敵チェック
        if (other.CompareTag("EnemyHit"))
        {
            Enemy enemyComponent = other.GetComponentInParent<Enemy>();

            if (enemyComponent != null && !enemiesInRange.Contains(enemyComponent))
            {
                enemiesInRange.Add(enemyComponent);
                UpdateClosestEnemy();
            }
        }
        //HideSpotを保存する
        else if (other.CompareTag("HideSpot"))
        {
            HideSpot spot = other.GetComponent<HideSpot>();
            if (spot != null && !hideSpotsInRange.Contains(spot))
            {
                hideSpotsInRange.Add(spot);
                UpdateClosestHideSpot();
            }
        }


    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        //自分collider範囲内の敵チェック
        if (other.CompareTag("EnemyHit") || other.CompareTag("Enemy"))
        {
            //Enemy leaveEnemy = other.GetComponent<Enemy>();
            Enemy leaveEnemy = other.GetComponentInParent<Enemy>();
            if (leaveEnemy != null && enemiesInRange.Contains(leaveEnemy))
            {
                enemiesInRange.Remove(leaveEnemy);

                UpdateClosestEnemy();
            }

        }
        else if (other.CompareTag("HideSpot"))
        {
            HideSpot spot = other.GetComponent<HideSpot>();
            if (spot != null && hideSpotsInRange.Contains(spot))
            {
                //アイコンを閉じる
                spot.ToggleInteractionIcon(false);
                hideSpotsInRange.Remove(spot);
                UpdateClosestHideSpot();
            }
        }

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
        noiseCooldownTimer = noiseInterval;

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

    //エディタ上で音の範囲を可視化する
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, runNoiseRadius);
    }

    public bool TakeDamageByBullet(Vector2 bulletDir, float duration, float force)
    {
        if (stateMachine.currentState == dashState)
        {
            if (Time.unscaledTime - dashState.dashStartTime <= 0.5f)
            {
                witchTimeManager.ActivateWitchTime();
                //stateMachine.ChangeState(idleState);
                return true;
            }
        }

        if (invincibleTimer > 0)
        {
            return false;
        }

        if (IsHidden())
        {
            SetHidden(false);
        }

        SetInvincibleFlash(true);
        invincibleTimer = invincibleDuration;

        hitState.SetupHit(bulletDir, duration, force);
        stateMachine.ChangeState(hitState);

        return true;
    }

    public void TakeDamageByMelee(Vector2 impactDir, float heavyStunDuration, float heavyKnockbackForce, float damage)
    {
        if (stateMachine.currentState == dashState)
        {
            if (Time.unscaledTime - dashState.dashStartTime <= 0.5f)
            {
                witchTimeManager.ActivateWitchTime();
                //stateMachine.ChangeState(idleState);
                return;
            }
        }

        if (invincibleTimer > 0)
            return;

        if (IsHidden())
        {
            SetHidden(false);
        }

        SetInvincibleFlash(true);
        invincibleTimer = invincibleDuration;

        hitState.SetupHit(impactDir, heavyStunDuration, heavyKnockbackForce);

        stateMachine.ChangeState(hitState);
    }

    private void Invincible()
    {
        if (invincibleTimer > 0)
        {
            invincibleTimer -= Time.unscaledDeltaTime;

            if (invincibleFlashEnabled && sr != null)
            {
                float alpha = Mathf.PingPong(Time.unscaledTime * 10.0f, 1.0f);
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
        if (count >= 0 && currentHP >= maxHP)
            return;

        currentHP += count;
    }

    public void PlayErrorShake()
    {
        if (visualTransform == null)
            return;

        shakeTimer = shakeDuration;
        //前のアニメーションを中止
        visualTransform.DOKill(complete: true);

        visualTransform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness, false, true);
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

    public bool IsHidden()
    {
        return isHidden;
    }

    public void SetHidden(bool hidden)
    {
        isHidden = hidden;
        UpdateClosestHideSpot();
    }

    public new void SetVelocity(float xVelocity, float yVelocity)
    {

        float compensation = (Time.timeScale < 1f && Time.timeScale > 0) ? (1f / Time.timeScale) : 1f;

        base.SetVelocity(xVelocity * compensation, yVelocity * compensation);
    }

    public void SetVelocity(Vector2 velocity)
    {
        SetVelocity(velocity.x, velocity.y);
    }

    private void UpdateClosestEnemy()
    {
        enemiesInRange.RemoveAll(e => e == null);

        if (enemiesInRange.Count == 0)
        {
            //暗殺範囲内敵がいなかったら Iconを閉じる
            if (enemyCanKill != null)
                enemyCanKill.ToggleInteractionIcon(false);

            enemyCanKill = null;
            lockedEnemy = null;
            enemyTrans = null;
            SetAttackStandby(false);
            lastFrameTarget = null;
            return;
        }

        //最も近い敵を探す
        Enemy closestForIcon = null;
        Enemy closestForAttack = null;
        float minIconDist = float.MaxValue;
        float minAttackDist = float.MaxValue;
        Vector3 currentPos = transform.position;

        foreach (var enemy in enemiesInRange)
        {
            if (enemy.isDead)
                continue;

            float dist = Vector2.Distance(currentPos, enemy.transform.position);

            if (dist < minAttackDist)
            {
                minAttackDist = dist;
                closestForAttack = enemy;
            }

            bool shouldShowIcon = (enemy.GetCanAssassed() && !enemy.GetAlert()) || (enemy.GetCurrentState() == enemy.faintState);

            if (shouldShowIcon)
            {
                if (dist < minIconDist)
                {
                    minIconDist = dist;
                    closestForIcon = enemy;
                }
            }
        }

        //表示アイコンの目標を入り替える
        if (closestForIcon != lastFrameTarget)
        {
            //前の敵のアイコンを閉じる
            if (lastFrameTarget != null)
            {
                lastFrameTarget.ToggleInteractionIcon(false);
            }

            //今の敵のアイコンを開く
            if (closestForIcon != null)
            {
                closestForIcon.ToggleInteractionIcon(true);
            }

            lastFrameTarget = closestForIcon;
        }
        else if (closestForIcon != null)
        {
            //アイコンを表示
            closestForIcon.ToggleInteractionIcon(true);
        }

        //暗殺目標を最も近い敵にする
        if (closestForAttack != null)
        {
            enemyCanKill = closestForAttack;
            lockedEnemy = closestForAttack;
            enemyTrans = closestForAttack.transform;
            SetAttackStandby(true);
        }
        else
        {
            enemyCanKill = null;
            SetAttackStandby(false);
        }
    }

    public override void FlipX()
    {
        facingRight = !facingRight;
        facingDirX *= -1;

        visuals.Rotate(0f, 180f, 0f);
    }

    private void UpdateClosestHideSpot()
    {
        hideSpotsInRange.RemoveAll(s => s == null);

        if (hideSpotsInRange.Count == 0)
        {
            if (lastFrameHideSpot != null)
                lastFrameHideSpot.ToggleInteractionIcon(false);

            currentHideSpot = null;
            lastFrameHideSpot = null;
            return;
        }

        HideSpot closestSpot = null;
        float minDistance = float.MaxValue;
        Vector3 currentPos = transform.position;

        foreach (var spot in hideSpotsInRange)
        {
            //プレイヤーもう入ったらアイコンを閉じる
            if (IsHidden() && spot == currentHideSpot)
                continue;

            float dist = Vector2.Distance(currentPos, spot.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestSpot = spot;
            }
        }

        if (closestSpot != lastFrameHideSpot)
        {
            if (lastFrameHideSpot != null)
                lastFrameHideSpot.ToggleInteractionIcon(false);
            if (closestSpot != null)
                closestSpot.ToggleInteractionIcon(true);
            lastFrameHideSpot = closestSpot;
        }

        currentHideSpot = closestSpot;
    }

    public void ToggleInteractionIcon()
    {
        if (interactionIcon == null)
            return;

        if (witchTimeManager.IsWitchTimeActive)
            interactionIcon.Show();
        else
            interactionIcon.Hide();
    }

}
