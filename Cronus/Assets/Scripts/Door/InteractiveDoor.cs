using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractiveDoor : MonoBehaviour
{
    public enum DoorType { Horizontal4, Vertical2 }

    [Header("Component")]
    public Transform doorVisual;
    public BoxCollider2D obstacleCollider;
    public InteractionIcon interactionIcon;

    [Header("Settings")]
    public DoorType doorType;
    public float slideSpeed = 5f;
    [SerializeField] private LayerMask detectionLayer;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen = false;

    private HashSet<GameObject> entitiesInRange = new HashSet<GameObject>();
    private Player player;

    void Start()
    {
        closedPos = doorVisual.localPosition;

        //格数で開ける方法を決める
        if (doorType == DoorType.Horizontal4)
            openPos = closedPos + Vector3.left * 4f; //右から左
        else
            openPos = closedPos + Vector3.up * 2f;   //下から上

        if (interactionIcon != null)
            interactionIcon.Hide();

        if (detectionLayer == 0)
            detectionLayer = LayerMask.GetMask("Player", "Enemy");
    }

    void Update()
    {
        HandleDoorLogic();

        Vector3 targetPos = isOpen ? openPos : closedPos;
        doorVisual.localPosition = Vector3.Lerp(doorVisual.localPosition, targetPos, Time.deltaTime * slideSpeed);

        if (!isOpen)
        {
            float dist = Vector3.Distance(doorVisual.localPosition, closedPos);
            if (dist < 0.02f) //完全に閉じたら
            {
                if (!obstacleCollider.enabled)
                {
                    doorVisual.localPosition = closedPos;
                    obstacleCollider.enabled = true;
                    UpdateNavigation();
                }
            }
            else
            {
                //スライド中見えないとする
                if (obstacleCollider.enabled)
                    obstacleCollider.enabled = false;
            }
        }
    }

    private void HandleDoorLogic()
    {
        bool enemyIn = HasEnemyInRange();
        bool playerIn = HasPlayerInRange();

        //範囲内敵がいるならOpen
        if (enemyIn)
        {
            if (!isOpen) SetDoorState(true);
        }
        //範囲内誰でもいなかったらClose
        else if (!playerIn && !enemyIn)
        {
            if (isOpen && !IsPathBlocked())
                SetDoorState(false);
        }
        //プレイヤーのOpen操作
        else if (playerIn && player != null)
        {
            if (player.input.Player.Active.WasPressedThisFrame())
            {
                bool targetState = !isOpen;

                if (!targetState && IsPathBlocked())
                {
                    return;
                }

                SetDoorState(targetState);
            }
        }
    }

    private void SetDoorState(bool open)
    {
        isOpen = open;
        obstacleCollider.enabled = !open;

        if (isOpen)
        {
            obstacleCollider.enabled = false;
            UpdateNavigation();
        }
    }

    private void UpdateNavigation()
    {
        if (AstarPath.active != null && obstacleCollider != null)
        {
            AstarPath.active.UpdateGraphs(obstacleCollider.bounds);
        }
    }

    private bool HasEnemyInRange()
    {
        foreach (var obj in entitiesInRange)
        {
            if (obj != null && obj.CompareTag("Enemy"))
                return true;
        }
        return false;
    }

    private bool HasPlayerInRange()
    {
        return player != null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();

            if (interactionIcon != null)
                interactionIcon.Show();

            entitiesInRange.Add(other.gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            entitiesInRange.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (interactionIcon != null)
                interactionIcon.Hide();

            player = null;
            entitiesInRange.Remove(other.gameObject);
        }
        else if (other.CompareTag("Enemy"))
        {
            entitiesInRange.Remove(other.gameObject);
        }
    }

    private bool IsPathBlocked()
    {
        //Door閉じる状態のWorld座標
        Vector3 worldClosedPos = transform.TransformPoint(closedPos);

        //obstacleColliderのサイズでチェック
        Vector2 checkSize = obstacleCollider.size * 0.9f;

        Collider2D hit = Physics2D.OverlapBox(worldClosedPos, checkSize, 0f, detectionLayer);

        return hit != null;
    }
}