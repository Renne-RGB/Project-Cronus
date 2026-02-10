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
        if (canvasGroup != null)
            canvasGroup.alpha = 0;
        infoPanel.transform.localScale = Vector3.one * 0.8f;
    }

    void Start()
    {
        if (videoPlayer != null && signVideoClip != null)
        {
            videoPlayer.clip = signVideoClip;
            videoPlayer.playOnAwake = false;
            videoPlayer.Prepare();
        }

        infoPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
        infoPanel.transform.DOKill();
        canvasGroup.DOKill();

        infoPanel.SetActive(true);
        descText.text = contentDescription;

        if (videoPlayer.isPrepared)
        {
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Play();
        }

        canvasGroup.DOFade(1, duration);
        infoPanel.transform.DOScale(1f, duration).SetEase(Ease.OutBack); // OutBack 会有一个微小的回弹效果，更生动
    }

    private void HidePanel()
    {
        infoPanel.transform.DOKill();
        canvasGroup.DOKill();

        canvasGroup.DOFade(0, duration);
        infoPanel.transform.DOScale(0.8f, duration).SetEase(Ease.InBack).OnComplete(() =>
        {
            videoPlayer.Pause();
            infoPanel.SetActive(false);
        });
    }
}