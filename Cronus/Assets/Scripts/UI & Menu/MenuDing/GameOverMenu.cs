using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
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
        //Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuitClick()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}