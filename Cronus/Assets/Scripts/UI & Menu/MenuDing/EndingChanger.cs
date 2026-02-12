using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening; // 使用你已有的 DOTween

public class EndingChanger : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    void Start()
    {
        fadeImage.color = Color.black;
        fadeImage.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            fadeImage.gameObject.SetActive(false);
        });
    }

    public void TransitionToScene(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);

        fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }
}