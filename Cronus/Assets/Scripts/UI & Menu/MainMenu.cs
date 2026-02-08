using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SpaceShip";

    public void NewGame()
    {
        Debug.Log("Starting New Game");
        SceneManager.LoadScene(gameSceneName);
    }

    // 2. 继续游戏 (存档载入预留)
    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("SavedScene"))
        {
            Debug.Log("Loading Saved Game");
            // string savedScene = PlayerPrefs.GetString("SavedScene");
            // SceneManager.LoadScene(savedScene);
        }
        else
        {
            Debug.LogWarning("No save data found!");
        }
    }

    // 3. 退出游戏
    public void ExitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }
}