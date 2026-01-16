using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class CheckGameClear : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        // 衝突したのがプレイヤーなら
        if (col.gameObject.tag == "Player")
        {
            // ゲームクリア画面に遷移
            SceneManager.LoadScene("Scenes/GameClear");
        }
    }

}
