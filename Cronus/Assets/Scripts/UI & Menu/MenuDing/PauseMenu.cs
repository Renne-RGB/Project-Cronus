using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }
    public GameObject pauseMenuUI;
    public GameObject firstButton;

    public PlayerInput playerInput;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GameOverMenu.Instance != null && GameOverMenu.Instance.gameObject.activeInHierarchy)
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.SetSelectedGameObject(GameOverMenu.Instance.firstButton);
                }
                return;
            }

            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        if (GameOverMenu.Instance != null && GameOverMenu.Instance.gameObject.activeInHierarchy)
        {
            isPaused = false;
            pauseMenuUI.SetActive(false);
            playerInput.SwitchCurrentActionMap("UI");
            return;
        }

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        playerInput.SwitchCurrentActionMap("Player");
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        isPaused = true;

        Player player = Object.FindFirstObjectByType<Player>();

        if (player != null)
        {
            player.FreezePlayer();
        }

        playerInput.SwitchCurrentActionMap("UI");
        Time.timeScale = 0f;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}