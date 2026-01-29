using UnityEngine;
using DG.Tweening;

public class InteractionIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer iconSprite;
    [SerializeField] private float bobAmplitude = 0.1f;
    [SerializeField] private float bobDuration = 0.3f;

    private Tween moveTween;

    private Vector3 initialLocalPos;    //元の座標を記録する
    private bool isInitialized = false;

    void Awake()
    {
        if (iconSprite == null)
            iconSprite = GetComponent<SpriteRenderer>();

        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (gameObject.activeSelf)
            return;

        if (!isInitialized)
        {
            Initialize();
        }

        gameObject.SetActive(true);

        //アニメーション zoom+Fadein
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);

        //アニメーション 繰り返し
        moveTween = transform.DOLocalMoveY(initialLocalPos.y + bobAmplitude, bobDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(true);
    }

    private void Initialize()
    {
        if (isInitialized)
            return;

        initialLocalPos = transform.localPosition;
        isInitialized = true;
    }

    public void Hide()
    {
        if (!gameObject.activeSelf) return;

        //Fadeout
        moveTween?.Kill();
        transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
            transform.localPosition = initialLocalPos;
        });
    }
}