using UnityEngine;

public class Enemy_FleeState : EnemyState
{
    private float noiseTimer;
    private const float noiseInterval = 1.5f;

    private float fleeTimer;
    private const float fleePhaseDuration = 10.0f;

    private const float fleeDistance = 10f;
    private const float playerDistanceThreshold = 8f;

    private Vector3 currentFleeTarget;
    private float pathUpdateTimer;
    private const float pathUpdateInterval = 0.5f;

    public Enemy_FleeState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.SetAlert(true);

        noiseTimer = 0;
        fleeTimer = noiseInterval;
        pathUpdateTimer = 0;

        PickRandomFleePoint();
    }

    public override void Update()
    {
        base.Update();

        noiseTimer -= Time.deltaTime;
        if (noiseTimer <= 0)
        {
            if (enemy.playerTransform != null)
            {
                enemy.UpdateSharedSearchRing(enemy.playerTransform.position);
            }

            enemy.EmitNoise(20.0f);
            noiseTimer = noiseInterval;
        }

        fleeTimer -= Time.deltaTime;
        if (fleeTimer <= 0)
        {
            if (enemy.targetPlayer == null)
            {
                EndFlee();
                return;
            }

            float realDist = Vector2.Distance(enemy.transform.position, enemy.targetPlayer.transform.position);

            if (realDist > playerDistanceThreshold)
            {
                EndFlee();
                return;
            }
            else
            {
                fleeTimer = fleePhaseDuration;
                PickRandomFleePoint();
            }
        }

        MoveAlongPath();
    }

    private void EndFlee()
    {
        enemy.SetAlert(false);
        stateMachine.ChangeState(enemy.idleState);
    }

    private void PickRandomFleePoint()
    {
        //ランダムの逃げる方向
        float randomAngle = Random.Range(0f, 360f);
        Vector3 direction = Quaternion.Euler(0, 0, randomAngle) * Vector3.right;

        currentFleeTarget = enemy.transform.position + direction * fleeDistance;

        enemy.GeneratePath(currentFleeTarget);
    }

    private void MoveAlongPath()
    {
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= pathUpdateInterval)
        {
            enemy.GeneratePath(currentFleeTarget);
            pathUpdateTimer = 0;
        }

        if (enemy.pathPointList != null && enemy.currentIndex < enemy.pathPointList.Count)
        {
            if (Vector2.Distance(enemy.transform.position, enemy.pathPointList[enemy.currentIndex]) <= 0.4f)
            {
                enemy.currentIndex++;
            }

            if (enemy.currentIndex < enemy.pathPointList.Count)
            {
                Vector2 direction = (enemy.pathPointList[enemy.currentIndex] - enemy.transform.position).normalized;
                enemy.MovementInput = direction;
                enemy.Dash();
            }
        }
        else
        {
            enemy.MovementInput = Vector2.zero;
        }
    }
}