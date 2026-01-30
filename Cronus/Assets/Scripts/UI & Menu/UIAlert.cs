using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAlert : MonoBehaviour
{
    //（金）UIマネージャの変数
    public UIManager uimanager;
    //（金）タイマーを表示するためのテキスト
    public TextMeshProUGUI timer;
    //（金）アラートのタイマー
    public float alertTimer = 16f;
    //（金）赤い枠をアニメーションさせるためのタイマー
    public float redTimer = 1f;
    public bool redIsGlow = false;


    void Start()
    {
        //（金）UIマネージャの取得
        uimanager = GameObject.Find("Canvas").GetComponent<UIManager>();
        //（金）タイマーのテキストの取得
        timer = this.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if(alertTimer > 0f) { alertTimer -= Time.deltaTime; }
        timer.text = (Mathf.Floor(alertTimer * 100f) / 100f).ToString();
        // Mathf.Floor(value * 100f) / 100f;

        if (redTimer <= 0.1f) { redIsGlow = true; }
        if (redTimer >= 0.9f) { redIsGlow = false; }

        if (redIsGlow) { redTimer += Time.deltaTime; }
        else { redTimer -= Time.deltaTime; }

        this.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, redTimer);


        //（金）タイマーが0になればマネージャのプレファブを削除し、自分も消す
        if (alertTimer <= 0f)
        {
            SendMessage("DestroyAlert");
            Destroy(gameObject);
        }

    }
}
