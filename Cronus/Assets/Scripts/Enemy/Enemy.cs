using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

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

    public SpriteRenderer sr;
    [Header("Vision")]
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private float fov = 90f;
    [SerializeField] private float viewDistance = 8f;
    [SerializeField] public Vector3 aimDirection { get; set; }

    [Header("Patrol details")]
    public float idleDuration = 2;      //待機時間
    public float moveSpeed = 1.4f;
    public Transform[] patroPoints;     //全てのパトロール座標
    public int targetPointIndex = 0;    //パトロール目標番号

    [Header("Target")]
    public Transform playerTransform;
    public Player player;
    [SerializeField] public LayerMask playerAndObstacleMask;
    [Header("Chase")]
    public float currentSpeed = 0;
    public float chaseDuration = 3f;
    public float currentChaseTimer = 0f;
    [SerializeField] private bool AlertFlag = false;
    public Vector2 MovementInput { get; set; }
    [SerializeField] protected float chaseDistance = 20f;       //追撃距離
    [Header("Detection Settings")]
    public float loseTargetDelay = 2.0f; //プレイヤーが消えて何秒から赤い円を生成する
    private float loseTargetTimer = 0f;

    private bool hasGeneratedRingThisTime = false;

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

    protected override void Awake()
    {
        base.Awake();
        seeker = GetComponent<Seeker>();
        sr = GetComponentInChildren<SpriteRenderer>();

        MovementInput = Vector2.right;
        fieldOfView = GetComponentInChildren<FieldOfView>();
        fieldOfView.SetFov(fov);
        fieldOfView.SetViewDistance(viewDistance);
    }

    protected override void Update()
    {
        base.Update();

        GetPlayerTransform();

        bool isViewLocked = (stateMachine.currentState == knockbackState || stateMachine.currentState == faintState);

        //敵が飛ばせる状態であれば視野はプレイヤーに追従しない
        if (!isViewLocked)
        {
            if (playerTransform != null)
            {
                SearchRingManager.Instance.LastTargetPosition = playerTransform.position;
                aimDirection = (playerTransform.position - transform.position).normalized;

                if (Mathf.Abs(aimDirection.x) > 0.1f)
                {
                    sr.flipX = aimDirection.x < 0;
                }
            }
            else
            {
                //プレイヤーに見つけなかったら 視野方向は移動方向と同じようにする
                if (MovementInput.sqrMagnitude > 0.01f)
                {
                    aimDirection = MovementInput.normalized;
                }
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

    public void GetPlayerTransform()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, chaseDistance, playerLayer);

        if (colliders.Length > 0)
        {
            Transform target = colliders[0].transform;
            Vector3 dirToPlayer = (target.position - transform.position).normalized;
            float distToPlayer = Vector2.Distance(transform.position, target.position);

            //障害物チェック
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, chaseDistance, playerAndObstacleMask);
            bool hasLineOfSight = hit.collider != null && ((1 << hit.collider.gameObject.layer) & playerLayer) != 0;

            if (hasLineOfSight)
            {
                if (playerTransform != null)
                {
                    //もしchase状態である かつ chaseDistance内 かつ 障害物なし
                    //視野角度がプレイヤーにLockONする
                    loseTargetTimer = 0f;
                    distance = distToPlayer;
                    return;
                }
                else
                {
                    float angle = Vector3.Angle(aimDirection, dirToPlayer);
                    if (distToPlayer <= viewDistance && angle < fov / 2f)
                    {
                        playerTransform = target;
                        distance = distToPlayer;
                        loseTargetTimer = 0f;
                        ClearSearchRing(); //プレイヤーに見つけなかったら 前の赤い円を消す
                        return;
                    }
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

        if (playerTransform != null)
        {
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, viewDistance, playerAndObstacleMask);
            bool isVisible = hit.collider != null && hit.collider.gameObject.GetComponent<Player>() != null;
            if (isVisible)
            {
                //視野内に入ったらalert状態に切り替える
                if (Vector3.Angle(MovementInput, dirToPlayer) < fov / 2 || distance < cqbDistance)
                {
                    stateMachine.ChangeState(alertState);
                }
            }
        }

    }

    public void SetAlert(bool alert)
    {
        AlertFlag = alert;
        if (alert)
            currentChaseTimer = chaseDuration;
    }

    public bool GetAlert()
    {
        return AlertFlag;
    }

    // 共有検索赤い円の生成・更新
    public void UpdateSharedSearchRing(Vector3 position)
    {
        if (hasGeneratedRingThisTime) return;

        SearchRingManager.Instance.GenerateSearchRing(position);
        hasGeneratedRingThisTime = true;
    }

    public void ResetSearchStatus()
    {
        hasGeneratedRingThisTime = false;
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

    public void ResetSearchRingFlag()
    {
        hasGeneratedRingThisTime = false;
    }

    public void OnHearSound(Vector3 soundPosition)
    {
        //すでにプレイヤーを目視している場合は、視覚優先のため音を無視する
        if (playerTransform != null)
            return;

        //最後に確認されたターゲット座標を音の発生源に更新
        SearchRingManager.Instance.LastTargetPosition = soundPosition;

        //赤い円（SearchRing）を生成
        if (!SearchRingManager.Instance.HasActiveRing())
        {
            ResetSearchStatus();
            UpdateSharedSearchRing(soundPosition);
        }

        //警戒状態（AlertState）に移行して、音の場所へ移動を開始する
        if (stateMachine.currentState != alertState && stateMachine.currentState != chaseState)
        {
            SetAlert(true);
            stateMachine.ChangeState(alertState);
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
        anim.SetInteger("deathIndex", deathType);
        anim.SetTrigger("die");
    }

    public void TriggerBlockOrAlert()
    {
        if (stateMachine.currentState != faintState && stateMachine.currentState != knockbackState)
        {
            stateMachine.ChangeState(blockState);
        }
    }
}