using UnityEngine;
using UnityEngine.UI;

public class Enemy_SuspiciousState : EnemyState
{
    private float timer;
    private float duration = 5.0f;
    private bool halfWayTriggered = false; //50%以上フラグ

    private string susBlendParam = "susBlend";

    public Enemy_SuspiciousState(Enemy enemy, StateMachine stateMachine, string animBoolName)
        : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timer = 0f;
        halfWayTriggered = false;

        enemy.rb.linearVelocity = Vector2.zero;
        enemy.MovementInput = Vector2.zero;

        enemy.anim.SetFloat(susBlendParam, 0f);

        if (enemy.questionMarkImage != null)
        {
            enemy.questionMarkImage.gameObject.SetActive(true);
            enemy.questionMarkImage.fillAmount = 0f;
        }
    }

    public override void Update()
    {
        base.Update();

        enemy.GetPlayerTransform();

        //プレイヤー位置によってprogress増加するか否か
        bool shouldIncrease = false;

        if (enemy.playerTransform != null && enemy.targetPlayer != null && !enemy.targetPlayer.IsHidden())
        {
            float dist = Vector2.Distance(enemy.transform.position, enemy.playerTransform.position);

            //もし視野内にいれば直接Alert状態に入る
            if (dist <= enemy.GetViewDistance())
            {
                stateMachine.ChangeState(enemy.alertState);
                return;
            }

            //疑惑範囲内にいればprogress増加
            if (dist <= enemy.suspiciousDistance)
            {
                shouldIncrease = true;
            }
        }

        if (shouldIncrease)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
        }

        //timer范围を制御 [0, duration]
        timer = Mathf.Clamp(timer, 0f, duration);

        //progressを更新する
        float progress = timer / duration;
        if (enemy.questionMarkImage != null)
            enemy.questionMarkImage.fillAmount = progress;

        //MAXになったらAlert状態に入る
        if (timer >= duration)
        {
            //enemy.SetAlert(true);
            stateMachine.ChangeState(enemy.alertState);
            return;
        }
        else if (timer <= 0f)
        {
            //progressは０になったらIdle状態に戻る
            stateMachine.ChangeState(enemy.idleState);
            return;
        }

        //progressは50％超えたらプレイヤー位置に移動する
        if (progress >= 0.5f)
        {
            if (enemy.playerTransform != null)
            {
                //プレイヤーがいなければRingの位置に移動する
                if (!halfWayTriggered)
                {
                    TriggerHalfWayAction();
                }
                MoveTowardsRing();
            }
            else
            {
                stateMachine.ChangeState(enemy.investigateState);
                return;
            }
        }
        //50%以下になったら移動停止
        else if (progress < 0.5f && halfWayTriggered)
        {
            CancelHalfWayAction();
        }

        // if (halfWayTriggered)
        // {
        //     MoveTowardsRing();
        // }
    }

    //SearchRingに移动する
    private void TriggerHalfWayAction()
    {
        halfWayTriggered = true;

        if (enemy.playerTransform != null)
            enemy.UpdateSharedSearchRing(enemy.playerTransform.position);

        //Moveアニメーションを再生する
        enemy.anim.SetFloat(susBlendParam, 1.0f);
    }

    //移動停止
    private void CancelHalfWayAction()
    {
        halfWayTriggered = false;

        enemy.MovementInput = Vector2.zero;
        enemy.rb.linearVelocity = Vector2.zero;

        //Blend Treeの値を0にしてIdleアニメーションに戻す
        enemy.anim.SetFloat(susBlendParam, 0f);
    }

    private void MoveTowardsRing()
    {
        Vector3 targetPos = SearchRingManager.Instance.LastTargetPosition;
        Vector3 dir = (targetPos - enemy.transform.position).normalized;

        enemy.MovementInput = dir;
        enemy.Dash();
    }

    public override void Exit()
    {
        //終了時にパラメータをリセット
        enemy.anim.SetFloat(susBlendParam, 0f);

        enemy.MovementInput = Vector2.zero;
        enemy.rb.linearVelocity = Vector2.zero;

        if (enemy.questionMarkImage != null)
        {
            enemy.questionMarkImage.fillAmount = 0f;
            enemy.questionMarkImage.gameObject.SetActive(false);
        }

        base.Exit();
    }
}