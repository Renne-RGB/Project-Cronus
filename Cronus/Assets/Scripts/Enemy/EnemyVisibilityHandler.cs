using UnityEngine;

// クラスの外に定義することでアクセスを容易にする
public enum VisibilityState { Visible, RecentlyHidden, Hidden }

public class EnemyVisibilityHandler : MonoBehaviour
{
    [Header("Component References")]
    public Animator actionAnimator; 

    [Header("Visibility Settings")]
    public float ghostDuration = 0.5f; 

    private VisibilityState currentState = VisibilityState.Hidden;
    private float timer;

    // Animatorのレイヤーインデックス（0:Base, 1:Outline, 2:Ripple）
    private const int LAYER_NORMAL = 0;
    private const int LAYER_OUTLINE = 1;
    private const int LAYER_RIPPLE = 2;

    void Start()
    {
        // 初期状態の適用
        ApplyState(VisibilityState.Hidden);
    }

    void Update()
    {
        // 「最近非表示になった」カウントダウン処理
        if (currentState == VisibilityState.RecentlyHidden)
        {
            timer -= Time.deltaTime;
            if (timer <= 0) ApplyState(VisibilityState.Hidden);
        }
    }

    public void UpdateVisibility(bool isVisibleNow)
    {
        if (isVisibleNow)
        {
            if (currentState != VisibilityState.Visible) ApplyState(VisibilityState.Visible);
        }
        else if (currentState == VisibilityState.Visible)
        {
            ApplyState(VisibilityState.RecentlyHidden);
        }
    }

    private void ApplyState(VisibilityState newState)
    {
        currentState = newState;

        // 全てのオーバーレイレイヤーの重みを一旦リセット（0にする）
        // これにより、Base Layerのアニメーション状態を壊さずに見た目だけを切り替える
        actionAnimator.SetLayerWeight(LAYER_OUTLINE, 0f);
        actionAnimator.SetLayerWeight(LAYER_RIPPLE, 0f);

        switch (newState)
        {
            case VisibilityState.Visible:
                // Base Layerのみが表示される（他のWeightが0のため）
                break;

            case VisibilityState.RecentlyHidden:
                // Outlineレイヤーを最前面に表示
                actionAnimator.SetLayerWeight(LAYER_OUTLINE, 1f);
                timer = ghostDuration;
                break;

            case VisibilityState.Hidden:
                // Rippleレイヤーを最前面に表示
                actionAnimator.SetLayerWeight(LAYER_RIPPLE, 1f);
                break;
        }
    }
}