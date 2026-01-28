using UnityEngine;

public class UIManager : MonoBehaviour
{
    //（金）アラートのプレファブを入れる変数
    public GameObject alert;

    //（金）敵からSendMessageで受け取り、アラートのプレファブを登録（重複させないため）
    void GetAlertPrefab(GameObject ob)
    {
        if (alert == null)
        {
            alert = ob;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
