using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class EndingSceneManager : MonoBehaviour
{
    public Image fadeImage; 
    public float fadeDuration = 1.5f;
    public string mainMenuSceneName = "MainMenu";

    private PlayerInputSet input;
    private bool canExit = false;

    void Awake()
    {
        input = new PlayerInputSet();
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();

    void Start()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.color = Color.black;

        fadeImage.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            canExit = true;
            fadeImage.gameObject.SetActive(false); 
        });
    }

    void Update()
    {
        if (canExit && input.Player.Attack.WasPressedThisFrame())
        {
            canExit = false;
            ReturnToMainMenu();
        }
    }

    void ReturnToMainMenu()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            SceneManager.LoadSceneAsync(mainMenuSceneName);
        });
    }
}