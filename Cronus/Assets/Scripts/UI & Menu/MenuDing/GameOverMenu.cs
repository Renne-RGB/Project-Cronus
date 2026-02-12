using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public static GameOverMenu Instance { get; private set; }
    public GameObject firstButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetupGameOverUI()
    {
        Player player = Object.FindFirstObjectByType<Player>();

        if (player != null)
        {
            player.FreezePlayer();
            player.GetComponent<UnityEngine.InputSystem.PlayerInput>().SwitchCurrentActionMap("UI");
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }

    public void OnReviveClick()
    {
        Player player = Object.FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.Revive();
        }
    }

    public void OnMenuClick()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitClick()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}