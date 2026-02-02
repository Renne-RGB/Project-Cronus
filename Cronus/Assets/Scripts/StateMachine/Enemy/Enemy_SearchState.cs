using UnityEngine;

public class Enemy_SearchState : EnemyState
{
    private float timer;
    private int phaseIndex;
    // 0: 移動停止 -> 1: 後ろ向き移動 -> 2: 移動停止 -> 3: 前向き移動 -> 4: 移動停止 -> 5: Idle状態に戻る

    private float observationTime = 3.0f;
    private float turnMoveDist = 1.0f;

    private bool isMovingToTurn;
    private Vector3 moveTargetPos;

    private float moveTimeoutTimer; //OverTimeバッファ
    private float maxMoveTime = 3.0f; //3秒以上到着しなかったら移動停止

    public Enemy_SearchState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.rb.linearVelocity = Vector2.zero;
        enemy.MovementInput = Vector2.zero;
        enemy.SetAlert(false);

        phaseIndex = 0;
        timer = observationTime;
        isMovingToTurn = false;
    }

    public override void Update()
    {
        base.Update();

        enemy.GetPlayerTransform();
        if (enemy.playerTransform != null && enemy.targetPlayer != null && !enemy.targetPlayer.IsHidden())
        {
            stateMachine.ChangeState(enemy.alertState);
            return;
        }

        if (isMovingToTurn)
        {
            HandleTurningMovement();
        }
        else
        {
            HandleObservation();
        }
    }

    private void HandleObservation()
    {
        enemy.MovementInput = Vector2.zero;

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            NextPhase();
        }
    }

    private void HandleTurningMovement()
    {
        float distance = Vector2.Distance(enemy.transform.position, moveTargetPos);
        Vector2 direction = (moveTargetPos - enemy.transform.position).normalized;

        //後ろ向きの移動
        enemy.MovementInput = direction;

        if (direction != Vector2.zero)
        {
            enemy.aimDirection = direction;
            if (Mathf.Abs(direction.x) > 0.1f)
                enemy.sr.flipX = direction.x < 0;
        }

        enemy.Dash();

        if (distance <= 0.2f)
        {
            StopTurning();
            return;
        }

        moveTimeoutTimer -= Time.deltaTime;
        if (moveTimeoutTimer <= 0)
        {
            StopTurning();
        }
    }

    private void NextPhase()
    {
        phaseIndex++;

        switch (phaseIndex)
        {
            case 1: //後ろ向けて移動
                StartTurnMove(GetPositionBehind());
                break;

            case 2: //移動停止
                timer = observationTime;
                break;

            case 3: //前向き移動
                StartTurnMove(SearchRingManager.Instance.LastTargetPosition);
                break;

            case 4: //移動停止
                timer = observationTime;
                break;

            case 5: //Over
                stateMachine.ChangeState(enemy.idleState);
                break;
        }
    }

    private void StartTurnMove(Vector3 targetPos)
    {
        isMovingToTurn = true;
        moveTargetPos = targetPos;

        moveTimeoutTimer = maxMoveTime;
    }

    private void StopTurning()
    {
        isMovingToTurn = false;
        enemy.MovementInput = Vector2.zero;
        enemy.rb.linearVelocity = Vector2.zero;

        NextPhase();
    }

    private Vector3 GetPositionBehind()
    {
        //後ろ向きの移動先座標を計算する
        //Vector3 toCenter = (SearchRingManager.Instance.LastTargetPosition - enemy.transform.position).normalized;
        Vector3 currentFacing = enemy.aimDirection.normalized;
        
        if (currentFacing == Vector3.zero)
        {
             currentFacing = enemy.sr.flipX ? Vector2.left : Vector2.right;
        }

        Vector3 backwardPos = enemy.transform.position - (currentFacing * turnMoveDist);
        
        return backwardPos;
    }
}