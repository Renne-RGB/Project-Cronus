using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertUIManager : MonoBehaviour
{
    public GameObject alert;            // 赤い枠を入れる変数

    bool isAlertActive = false;         // アラートのオンオフ
    float redTimer = 1f;                 // 赤い枠をアニメーションさせるための変数
    bool redIsGlow = false;

    void Start()
    {
        alert = transform.GetChild(0).gameObject;
        alert.SetActive(false);
    }

    void Update()
    {
        // プレイヤーを見つけた敵の有無でアラートを切り替える
        if (Enemy.globalAlertCount == 0) { if (isAlertActive) { AlertReset(); } }
        else if (Enemy.globalAlertCount > 0)
        {
            if (!isAlertActive) { AlertStart(); }
            AlertAnimate();
        }
    }


    // プレイヤーを見つけた敵がいなくなったら、アラートをリセットする
    void AlertReset()
    {
        alert.SetActive(false);
        isAlertActive = false;
        redTimer = 1f;
        redIsGlow = false;
    }
    // プレイヤーを見つけた敵がいたら、アラートをオンにする
    void AlertStart()
    {
        alert.SetActive(true);
        isAlertActive = true;
    }

    // アラートをアニメーションさせる
    void AlertAnimate()
    {
        if (redTimer <= 0.1f) { redIsGlow = true; }
        if (redTimer >= 0.9f) { redIsGlow = false; }
        if (redIsGlow) { redTimer += Time.deltaTime; }
        else { redTimer -= Time.deltaTime; }
        alert.GetComponent<Image>().color = new Color(1f, 1f, 1f, redTimer);
    }

    //using TMPro;
    //（金）タイマーを表示するためのテキスト
    //public TextMeshProUGUI timer;
    //（金）アラートのタイマー
    //public float alertTimer = 16f;

    //Start（金）タイマーのテキストの取得
    //timer = this.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    //Update
    //if(alertTimer > 0f) { alertTimer -= Time.deltaTime; }
    //timer.text = (Mathf.Floor(alertTimer * 100f) / 100f).ToString();
    // Mathf.Floor(value * 100f) / 100f;


}
