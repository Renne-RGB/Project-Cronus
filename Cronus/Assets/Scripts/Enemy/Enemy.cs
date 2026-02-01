using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_ChaseState chaseState;
    public Enemy_CqbState cqbState;
    public Enemy_ShootState shootState;
    public Enemy_AlertState alertState;
    public Enemy_SearchState searchState;
    public Enemy_KnockbackState knockbackState;
    public Enemy_FaintState faintState;
    public Enemy_BlockState blockState;
    public Enemy_SuspiciousState susState;
    public Enemy_HearState hearState;
    public Enemy_InvestigateState investigateState;

    public SpriteRenderer sr;
    [Header("Vision")]
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private float fov = 90f;
    [SerializeField] private float viewDistance = 8f;
    [SerializeField] private float fovRotationSpeed = 10f;  //転向速度
    [SerializeField] public Vector3 aimDirection { get; set; }

    [Header("Patrol details")]
    public float idleDuration = 2;      //待機時間
    public float moveSpeed = 1.4f;
    public Transform[] patroPoints;     //全てのパトロール座標
    public int targetPointIndex = 0;    //パトロール目標番号

    [Header("Target")]
    public Transform playerTransform;
    public Player targetPlayer;
    [SerializeField] public LayerMask playerAndObstacleMask;
    [Header("Suspicious Settings")]
    public float suspiciousDistance = 15f; // 疑惑距離
    public Image questionMarkImage;
    [Header("Chase")]
    public float currentSpeed = 0;
    public float chaseDuration = 3f;
    public float currentChaseTimer = 0f;
    [SerializeField] private bool AlertFlag = false;

    //（金）キャンバスの宣言
    Canvas canvas;
    //（金）アラート用プレファブ
    public GameObject alertPrefab;

    public Vector2 MovementInput { get; set; }
    [SerializeField] protected float chaseDistance = 20f;       //追撃距離
    [Header("Detection Settings")]
    public float loseTargetDelay = 2.0f; //プレイヤーが消えて何秒から赤い円を生成する
    private float loseTargetTimer = 0f;


    private Seeker seeker;
    public List<Vector3> pathPointList;        //ルーティングリスト
    public bool pathReady = false;
    public int currentIndex = 0;
    private float pathGenerateInterval = 0.5f;      //0.5秒毎にルーティング生成
    private float pathGenerateTimer = 0f;       //ルーティング生成Timer
    [Header("Attack")]
    public float distance;      //プレイヤーとの距離
    public float shootRange;      //射程距離
    public float cqbDistance;         //接近戦距離
    public float cqbAttackRadius = 3.0f;     //接近戦判定半径
    public float cqbStunDuration = 2.0f;     //stun時間
    public float cqbKnockbackForce = 20.0f;  //飛ばされる距離
    public float cqbDamage = 2.0f;
    private bool canAssassed = true;     //暗殺できるか否か

    public LayerMask playerLayer;
    [Header("Weapon Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float timeBetweenShots = 0.2f; //三連発の間隔
    public float spreadAngle = 30f;     //弾丸のランダム角度
    public float bulletSpeed = 20f;     //弾丸の速度
    public float shootCooldownDuration = 2.0f;
    public float currentShootCooldown = 0f;
    [Header("Knockback & Faint Settings")]
    public float faintDuration = 5.0f;           // 気絶時間
    public float knockbackForceEnemy = 10.0f;    // 敵が受けるノックバック力
    public float wallBounceForce = 5.0f;         // 壁にぶつかった時の跳ね返り力
    public float faintDrag = 7.0f;         //気絶時の摩擦
    [Header("UI Settings")]
    public InteractionIcon interactionIcon; //Icon

    public bool isDead = false;

    protected override void Awake()
    {
        base.Awake();
        seeker = GetComponent<Seeker>();
        sr = GetComponentInChildren<SpriteRenderer>();

        MovementInput = Vector2.right;
        fieldOfView = GetComponentInChildren<FieldOfView>();
        fieldOfView.SetFov(fov);
        fieldOfView.SetViewDistance(viewDistance);

        //（金）キャンバスの取得
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

    }

    protected override void Update()
    {
        if (isDead)
            return;

        base.Update();

        GetPlayerTransform();

        bool isViewLocked = (stateMachine.currentState == knockbackState || stateMachine.currentState == faintState
         || stateMachine.currentState == searchState || stateMachine.currentState == susState);

        //敵が飛ばせられ状態であれば視野はプレイヤーに追従しない
        if (!isViewLocked)
        {

            Vector3 targetDirection = aimDirection;     //向きの初期設定

            if (playerTransform != null)
            {
                SearchRingManager.Instance.LastTargetPosition = playerTransform.position;
                targetDirection = (playerTransform.position - transform.position).normalized;
            }
            else
            {
                //プレイヤーに見つけなかったら 視野方向は移動方向と同じようにする
                if (MovementInput.sqrMagnitude > 0.01f)
                {
                    targetDirection = MovementInput.normalized;
                }
            }

            if (targetDirection != Vector3.zero)
            {
                aimDirection = Vector3.Slerp(aimDirection, targetDirection, fovRotationSpeed * Time.deltaTime);
            }

            if (Mathf.Abs(aimDirection.x) > 0.1f)
            {
                sr.flipX = aimDirection.x < 0;
            }
        }


        fieldOfView.SetAimDirection(aimDirection);
        fieldOfView.SetOrigin(Vector3.zero);

        FindTargetPlayer();

        if (GetAlert())
        {
            if (SearchRingManager.Instance.HasActiveRing()) currentChaseTimer = chaseDuration;
            else currentChaseTimer -= Time.deltaTime;

            if (currentChaseTimer <= 0)
            {
                SetAlert(false);
                stateMachine.ChangeState(idleState);
            }
        }

        if (currentShootCooldown > 0)
        {
            currentShootCooldown -= Time.deltaTime;
        }
    }

    public EntityState GetCurrentState()
    {
        return stateMachine.currentState;
    }

    public void GetPlayerTransform()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, suspiciousDistance, playerLayer);

        if (colliders.Length > 0)
        {
            Transform target = colliders[0].transform;
            targetPlayer = target.GetComponent<Player>();

            //プレイヤーはHide状態であれば無視する
            if (targetPlayer != null)
            {
                if (targetPlayer.IsHidden() && !GetAlert())
                {
                    return;
                }
            }

            Vector3 dirToPlayer = (target.position - transform.position).normalized;
            float distToPlayer = Vector2.Distance(transform.position, target.position);

            //障害物チェック
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, suspiciousDistance, playerAndObstacleMask);
            bool hasLineOfSight = hit.collider != null && ((1 << hit.collider.gameObject.layer) & playerLayer) != 0;

            if (hasLineOfSight)
            {
                //角度のチェック
                float angle = Vector3.Angle(aimDirection, dirToPlayer);
                if (angle < fov / 2f)
                {
                    playerTransform = target;
                    distance = distToPlayer;
                    loseTargetTimer = 0f;

                    if (stateMachine.currentState == susState)
                    {
                        SearchRingManager.Instance.LastTargetPosition = target.position;
                    }
                    return;
                }
            }
        }

        if (playerTransform != null)
        {
            loseTargetTimer += Time.deltaTime;
            if (loseTargetTimer >= loseTargetDelay)
            {
                playerTransform = null;
            }
        }
    }

    #region ルーティング生成
    public void AutoPath()
    {
        pathGenerateTimer += Time.deltaTime;

        // ターゲットの決定：プレイヤーが見えていればプレイヤー、いなければ共有の最後目撃地点
        Vector3 targetPos = (playerTransform != null) ? playerTransform.position : SearchRingManager.Instance.LastTargetPosition;

        if (pathGenerateTimer >= pathGenerateInterval)
        {
            GeneratePath(targetPos);
            pathGenerateTimer = 0f;
        }

        if (pathPointList != null && currentIndex < pathPointList.Count)
        {
            // 経路の補正：次のポイントが現在地より近い場合はスキップ
            if (currentIndex + 1 < pathPointList.Count)
            {
                if (Vector2.Distance(transform.position, pathPointList[currentIndex + 1]) <
                    Vector2.Distance(transform.position, pathPointList[currentIndex]))
                {
                    currentIndex++;
                }
            }

            // 到達判定
            if (Vector2.Distance(transform.position, pathPointList[currentIndex]) <= 0.4f)
            {
                currentIndex++;
            }
        }
    }

    //ルーティング生成
    public void GeneratePath(Vector3 target)
    {
        pathReady = false;
        //引数（1：始点(プレイヤー位置)　2：終点(敵位置)　3：コールバック関数）
        seeker.StartPath(transform.position, target, Path =>
        {
            if (Path.error) return;

            pathPointList = Path.vectorPath;

            //パース逆転して、敵からプレイヤーになるように
            //pathPointList.Reverse();

            currentIndex = 0;

            pathReady = true;

        });

    }
    #endregion

    public void Dash()
    {
        if (MovementInput.sqrMagnitude > 0.01f && currentSpeed > 0f)
        {
            Vector2 newPos = rb.position + MovementInput * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);

            //プレイヤーに見えない時のみ視野方向は移動方向と同じようにする
            if (playerTransform == null)
            {
                if (Mathf.Abs(MovementInput.x) > 0.1f)
                {
                    sr.flipX = MovementInput.x < 0;
                }
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void FindTargetPlayer()
    {
        //既にAlert状態であれば実行しない
        if (AlertFlag || stateMachine.currentState == alertState)
            return;
        if (stateMachine.currentState == susState && distance > viewDistance)
            return;

        if (playerTransform != null)
        {
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;

            if (Vector3.Angle(aimDirection, dirToPlayer) < fov / 2)
            {
                //敵との距离 < 視野範囲距离 ->　Alert状態に入る
                if (distance <= viewDistance)
                {
                    if (stateMachine.currentState != faintState && stateMachine.currentState != hearState)
                        stateMachine.ChangeState(alertState);
                }
                //疑惑距离
                else if (distance <= suspiciousDistance)
                {
                    if (stateMachine.currentState != faintState && stateMachine.currentState != susState)
                    {
                        stateMachine.ChangeState(susState);
                    }
                }
            }
        }

    }

    public void SetAlert(bool alert)
    {
        AlertFlag = alert;
        if (alert)
        {
            currentChaseTimer = chaseDuration;

            // (金)アラートプレファブを複製しキャンバスの親子関係にし、SendMessageでキャンバスに登録
            if (canvas.GetComponent<UIManager>().alert == null)
            {
                Vector2 pos = canvas.transform.position;
                Vector3 rot = new Vector3(0.0f, 0.0f, 0.0f);
                GameObject newAlert = Instantiate(alertPrefab, pos, Quaternion.Euler(rot), canvas.transform);
                canvas.SendMessage("GetAlertPrefab", newAlert);
            }
            else if (canvas.GetComponent<UIManager>().alert != null)
            {

            }



        }


    }

    public bool GetAlert()
    {
        return AlertFlag;
    }

    // 共有検索赤い円の生成・更新
    public void UpdateSharedSearchRing(Vector3 position)
    {
        SearchRingManager.Instance.GenerateSearchRing(position);
    }

    // 赤い円とタイマーの強制クリア（プレイヤー発見時に使用）
    public void ClearSearchRing()
    {
        if (SearchRingManager.Instance.HasActiveRing())
        {
            SearchRingManager.Instance.DestroySearchRing();

            //リングが消えた瞬間、プレイヤーが見えていなければ Idle へ戻る
            if (playerTransform == null && (stateMachine.currentState == chaseState || stateMachine.currentState == alertState))
            {
                SetAlert(false);
                stateMachine.ChangeState(idleState);
            }
        }
    }

    public void OnHearSound(Vector3 soundPosition)
    {
        if (isDead)
            return;
        //すでにプレイヤーを目視している場合は、視覚優先のため音を無視する
        if (playerTransform != null)
            return;

        //最後に確認されたターゲット座標を音の発生源に更新
        SearchRingManager.Instance.LastTargetPosition = soundPosition;

        //赤い円（SearchRing）を生成
        if (!SearchRingManager.Instance.HasActiveRing())
        {
            UpdateSharedSearchRing(soundPosition);
        }

        //警戒状態（AlertState）に移行して、音の場所へ移動を開始する
        if (stateMachine.currentState != alertState && stateMachine.currentState != chaseState && stateMachine.currentState != faintState)
        {
            //SetAlert(true);
            stateMachine.ChangeState(hearState);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        // 1. プレイヤーとの衝突判定
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            // プレイヤーがチャージ中
            if (player != null && player.GetCurrentState() == player.chargeActionState)
            {
                Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

                Vector2 playerChargeDir = player.chargeDir.normalized;
                if (playerChargeDir == Vector2.zero)
                    playerChargeDir = (transform.position - player.transform.position).normalized;

                // プレイヤーへの反動
                if (playerRb != null)
                {
                    //Idle状態戻る
                    player.ResetState();
                    playerRb.linearVelocity = Vector2.zero;
                    playerRb.AddForce(-playerChargeDir * player.chargeRecoilForce, ForceMode2D.Impulse);
                }

                // 敵へのノックバック処理
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(playerChargeDir * knockbackForceEnemy, ForceMode2D.Impulse);

                //まずは KnockbackState に移行する
                stateMachine.ChangeState(knockbackState);
            }
            else if (player != null && player.GetCurrentState() == player.witchActionState)
            {
                TriggerAssassinationDeath(0);
                return;
            }
        }

        //壁との衝突判定
        if (other.CompareTag("Wall") && stateMachine.currentState == knockbackState)
        {
            //壁の法線を計算
            Vector2 closestPoint = other.ClosestPoint(transform.position);
            Vector2 normal = ((Vector2)transform.position - closestPoint).normalized;

            if (normal == Vector2.zero) normal = -rb.linearVelocity.normalized;

            //壁からの跳ね返り
            rb.linearVelocity = Vector2.zero; // 既存の速度を消して、純粋な跳ね返りにする
            rb.AddForce(normal * wallBounceForce, ForceMode2D.Impulse);

            //壁に当たったら FaintState へ移行
            stateMachine.ChangeState(faintState);
        }
    }

    public void SetCanAssassed(bool killFlag)
    {
        canAssassed = killFlag;
    }

    public bool GetCanAssassed()
    {
        return canAssassed;
    }

    public void TriggerAssassinationDeath(int deathType)
    {
        if (isDead)
            return;
        isDead = true;

        ToggleInteractionIcon(false);

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        MovementInput = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        anim.SetInteger("deathIndex", deathType);
        anim.SetTrigger("die");
        CloseFieldOfView(false);
    }

    public void TriggerBlockOrAlert()
    {
        if (stateMachine.currentState != faintState && stateMachine.currentState != knockbackState)
        {
            stateMachine.ChangeState(blockState);
        }
    }

    public void CloseFieldOfView(bool isActive)
    {
        if (fieldOfView != null)
        {
            fieldOfView.gameObject.SetActive(isActive);
        }
    }

    public float GetViewDistance()
    {
        return viewDistance;
    }

    public void ToggleInteractionIcon(bool show)
    {
        if (interactionIcon == null)
            return;


        //暗殺目標になる時表示する
        bool canAssassinate = GetCanAssassed();
        //Alert状態になる時表示しない
        bool isAlerted = GetAlert();
        //Faint状態になる時表示する
        bool isFainted = stateMachine.currentState == faintState;

        bool shouldShow = isFainted || (canAssassinate && !isAlerted);

        if (show && shouldShow)
        {
            interactionIcon.Show();
        }
        else
        {
            interactionIcon.Hide();
        }
    }
}