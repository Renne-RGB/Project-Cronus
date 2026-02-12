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
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

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