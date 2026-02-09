using UnityEngine;
using UnityEngine.Video;
using TMPro;
using DG.Tweening; // 必须引用

public class SignInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject infoPanel;
    public TextMeshProUGUI descText;
    public VideoPlayer videoPlayer;
    public CanvasGroup canvasGroup;

    [Header("setting")]
    [TextArea(3, 5)]
    public string contentDescription;
    public VideoClip signVideoClip;

    [Header("Anim setting")]
    public float duration = 0.5f;

    private void Awake()
    {
        // 初始状态：透明度0，缩放较小
        if (canvasGroup != null) canvasGroup.alpha = 0;
        infoPanel.transform.localScale = Vector3.one * 0.8f;
        infoPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 参考项目中通用的标签检测
        {
            ShowPanel();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HidePanel();
        }
    }

    private void ShowPanel()
    {
        // 停止之前的动画防止冲突
        infoPanel.transform.DOKill();
        canvasGroup.DOKill();

        infoPanel.SetActive(true);
        descText.text = contentDescription;
        videoPlayer.clip = signVideoClip;
        videoPlayer.Play();

        // DOTween 动画：淡入并放大到正常大小
        canvasGroup.DOFade(1, duration);
        infoPanel.transform.DOScale(1f, duration).SetEase(Ease.OutBack); // OutBack 会有一个微小的回弹效果，更生动
    }

    private void HidePanel()
    {
        infoPanel.transform.DOKill();
        canvasGroup.DOKill();

        // DOTween 动画：淡出并缩小
        canvasGroup.DOFade(0, duration);
        infoPanel.transform.DOScale(0.8f, duration).SetEase(Ease.InBack).OnComplete(() =>
        {
            videoPlayer.Stop();
            infoPanel.SetActive(false);
        });
    }
}