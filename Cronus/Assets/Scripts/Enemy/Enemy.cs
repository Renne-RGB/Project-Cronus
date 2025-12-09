using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_ChaseState chaseState;
    public Enemy_CqbState cqbState;
    public Enemy_ShootState shootState;

    public SpriteRenderer sr;

    [Header("Movement details")]
    public float idleTime = 2;
    public float moveSpeed = 1.4f;

    [Header("Target")]
    public Transform playerTransform;
    [Header("Chase")]
    public float currentSpeed = 0;
    public Vector2 MovementInput { get; set; }
    [SerializeField] protected float chaseDistance = 0.1f;       //追撃距離

    private Seeker seeker;
    public List<Vector3> pathPointList;        //ルーティングリスト
    public int currentIndex = 0;
    private float pathGenerateInterval = 2.0f;      //0.5秒毎にルーティング生成
    private float pathGenerateTimer = 0f;       //ルーティング生成Timer
    [Header("Attack")]
    public float cqbDistance = 3f;         //接近戦距離
    public float distance;      //プレイヤーとの距離
    public LayerMask playerLayer;

    protected override void Awake()
    {
        base.Awake();
        seeker = GetComponent<Seeker>();
        sr = GetComponentInChildren<SpriteRenderer>();
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

        //ルーティングリストがなければプレイヤーの位置によって生成する
        if (pathPointList == null || pathPointList.Count <= 0)
            GeneratePath(playerTransform.position);
        //敵が現在のパースポイントに着いたら、currentIndex順でルーティング計算する
        else if (Vector2.Distance(transform.position, pathPointList[currentIndex]) <= 0.1f)
        {
            currentIndex++;
            if (currentIndex >= pathPointList.Count)
                GeneratePath(playerTransform.position);
        }
    }

    //ルーティング生成
    public virtual void GeneratePath(Vector3 target)
    {
        currentIndex = 0;
        //引数（1：始点　2：終点　3：コールバック関数）
        seeker.StartPath(transform.position, target, Path =>
        {
            pathPointList = Path.vectorPath;
        });
    }
    #endregion
}
