using UnityEngine;

public class Player_PrepareChargeState : PlayerState
{
    private float prepareTimer; 

    public Player_PrepareChargeState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(0, 0);

        prepareTimer = 0f; 

        if (player.arrowIndicator != null)
            player.arrowIndicator.SetActive(true);

        //矢印の初期方向設定
        player.currentArrowDir = new Vector3(player.facingDirX, player.facingDirY, 0).normalized;
        UpdateArrowTransform(player.currentArrowDir);
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, 0);

        prepareTimer += Time.deltaTime;

        //チャージ完了
        if (prepareTimer >= player.chargeDurationReq)
        {
            stateMachine.ChangeState(player.chargeActionState);
            return;
        }

        //矢印の回転
        Vector3 targetDir = new Vector3(player.moveInput.x, player.moveInput.y, 0);

        if (targetDir == Vector3.zero)
            targetDir = player.currentArrowDir;

        player.currentArrowDir = Vector3.Slerp(player.currentArrowDir, targetDir.normalized, player.arrowRotationSpeed * Time.deltaTime);

        UpdateArrowTransform(player.currentArrowDir);
        player.chargeDir = (Vector2)player.currentArrowDir;

        if (player.input.Player.Charge.WasReleasedThisFrame())
        {
            //チャージ未完了で離した場合はアイドル状態に戻る
            stateMachine.ChangeState(player.idleState);
        }
    }

    private void UpdateArrowTransform(Vector3 direction)
    {
        if (player.arrowIndicator == null) return;

        // ワールド座標を使用して位置を更新
        player.arrowIndicator.transform.position = player.transform.position + (direction * player.arrowOrbitRadius);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        player.arrowIndicator.transform.rotation = Quaternion.Euler(0, 0, angle);

        // プレイヤーの反転に合わせて矢印のスケールを修正
        if (player.transform.localScale.x < 0)
            player.arrowIndicator.transform.localScale = new Vector3(-1, 1, 1);
        else
            player.arrowIndicator.transform.localScale = Vector3.one;
    }

    public override void Exit()
    {
        base.Exit();
        if (player.arrowIndicator != null)
            player.arrowIndicator.SetActive(false);
    }
}