using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

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

            if (OpeningManager.Instance != null)
            {
                OpeningManager.Instance.BeginIntro();
            }
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