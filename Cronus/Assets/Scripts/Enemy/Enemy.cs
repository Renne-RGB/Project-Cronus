using Pathfinding;
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

    public SpriteRenderer sr;
    [Header("Vision")]
    [SerializeField] private FieldOfView fieldOfView;
    [SerializeField] private float fov = 90f;
    [SerializeField] private float viewDistance = 8f;
    [SerializeField] private bool AlertFlag = false;
    [SerializeField] public Vector3 aimDirection { get; set; }

    [Header("Patrol details")]
    public float idleDuration = 2;      //待機時間
    public float moveSpeed = 1.4f;
    public Transform[] patroPoints;     //全てのパトロール座標
    public int targetPointIndex = 0;    //パトロール目標番号

    [Header("Target")]
    public Transform playerTransform;
    public Player player;
    [SerializeField] private LayerMask playerAndObstacleMask;
    [Header("Chase")]
    public float currentSpeed = 0;
    public Vector2 MovementInput { get; set; }
    [SerializeField] protected float chaseDistance = 20f;       //追撃距離

    private Seeker seeker;
    public List<Vector3> pathPointList;        //ルーティングリスト
    public bool pathReady = false;
    public int currentIndex = 0;
    private float pathGenerateInterval = 0.5f;      //0.5秒毎にルーティング生成
    private float pathGenerateTimer = 0f;       //ルーティング生成Timer
    [Header("Attack")]
    public float distance;      //プレイヤーとの距離
    public float cqbDistance;         //接近戦距離
    public float shootRange;      //射程距離
    public LayerMask playerLayer;

    protected override void Awake()
    {
        base.Awake();
        seeker = GetComponent<Seeker>();
        sr = GetComponentInChildren<SpriteRenderer>();

        fieldOfView.SetFov(fov);
        fieldOfView.SetViewDistance(viewDistance);
    }

    protected override void Update()
    {
        base.Update();

        //!!!原因がわからないけど取り敢えずMovementInputの反時計回りはちょうど敵の移動方向である
        aimDirection = new Vector3(-MovementInput.y, MovementInput.x, 0f);
        fieldOfView.SetAimDirection(aimDirection);
        fieldOfView.SetOrigin(transform.position);

        FindTargetPlayer();
    }

    public void GetPlayerTransform()
    {
        Collider2D[] chaseColliders = Physics2D.OverlapCircleAll(transform.position, chaseDistance, playerLayer);

        //プレイヤーは追撃範囲内にいれば 距離を求める
        if (chaseColliders.Length > 0)
        {
            playerTransform = chaseColliders[0].transform;
            distance = Vector2.Distance(playerTransform.position, transform.position);
        }
        else
            playerTransform = null;
    }

    #region ルーティング生成
    public void AutoPath()
    {
        // if (playerTransform == null)
        //     return;

        pathGenerateTimer += Time.deltaTime;
        //一定の時間に経ったらルーティングを生成する
        if (pathGenerateTimer >= pathGenerateInterval)
        {
            GeneratePath(playerTransform.position);
            pathGenerateTimer = 0f;
        }

        if (!pathReady)
            return;

        //ルーティングリストがなければプレイヤーの位置によって生成する
        if (pathPointList == null || pathPointList.Count <= 0 || currentIndex >= pathPointList.Count)
            GeneratePath(playerTransform.position);
        //敵が現在のパースポイントに着いたら、currentIndex順でルーティング計算する
        else if (Vector2.Distance(transform.position, pathPointList[currentIndex]) <= 0.4f)
        {
            currentIndex++;
            if (currentIndex >= pathPointList.Count)
                GeneratePath(playerTransform.position);
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

            if (Mathf.Abs(MovementInput.x) > 0.1f)
            {
                sr.flipX = MovementInput.x < 0;
            }

        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void FindTargetPlayer()
    {
        if (playerTransform != null)
        {
            Vector3 dirToPlayer = (playerTransform.position - transform.position).normalized;
            if (distance < viewDistance)
            {
                //視野角に入るか否か
                if (Vector3.Angle(MovementInput, dirToPlayer) < fov / 2)
                {
                    //プレイヤーに向けてRaycastを出す
                    RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, dirToPlayer, viewDistance, playerAndObstacleMask);
                    if (raycastHit2D.collider != null)
                    {
                        //プレイヤーに当たったら 攻撃状態に入る
                        if (raycastHit2D.collider.gameObject.GetComponent<Player>() != null)
                        {
                            if (distance <= cqbDistance)
                            {
                                stateMachine.ChangeState(cqbState);
                            }
                            //射程距離内であれば shoot状態に入る
                            else if (distance <= shootRange)
                            {
                                stateMachine.ChangeState(shootState);
                            }
                        }
                        //他の何かを当たったら
                        else
                        {

                        }
                    }

                }
            }
            else if (distance < cqbDistance)
            {
                stateMachine.ChangeState(cqbState);
            }
        }

    }

    public void SwitchStateByDistance()
    {
        // // //接近戦距離内であれば cqb状態に入る
        // if (distance <= cqbDistance)
        // {
        //     stateMachine.ChangeState(cqbState);
        // }
        // //射程距離内であれば shoot状態に入る
        // else if (distance <= shootRange)
        // {
        //     stateMachine.ChangeState(shootState);
        // }
        // //射程より大きいであれば 追撃状態に入る
        // else
        // {
        //     stateMachine.ChangeState(chaseState);
        // }
    }

    public void SetAlert(bool alert)
    {
        AlertFlag = alert;
    }

    public bool GetAlert()
    {
        return AlertFlag;
    }
}
