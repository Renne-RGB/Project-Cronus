using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    //（金）アラートのプレファブを入れる変数
    public GameObject alert;
    //（金）ライフのプレファブを入れるリスト
    public List<GameObject> lifeList;

    //（金）敵からSendMessageで受け取り、アラートのプレファブを登録（重複させないため）
    void GetAlertPrefab(GameObject ob)
    {
        if (alert == null) { alert = ob; }
    }
    //（金）アラートのプレファブを削除
    void DestroyAlert() { alert = null; }

    //（金）追尾のタイマーを更新
    void UpdateChaseTimer(float n)
    {
        if (alert == null) { return; }
        if (alert != null) { alert.GetComponent<UIAlert>().alertTimer = n; }
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
