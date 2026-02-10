using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertUIManager : MonoBehaviour
{
    public GameObject alert;            // �Ԃ��g������ϐ�

    bool isAlertActive = false;         // �A���[�g�̃I���I�t
    float redTimer = 1f;                 // �Ԃ��g���A�j���[�V���������邽�߂̕ϐ�
    bool redIsGlow = false;

    void Start()
    {
        alert = transform.GetChild(0).gameObject;
        alert.SetActive(false);
    }

    void Update()
    {
        // �v���C���[���������G�̗L���ŃA���[�g��؂�ւ���
        if (Enemy.globalAlertCount == 0) { if (isAlertActive) { AlertReset(); } }
        else if (Enemy.globalAlertCount > 0)
        {
            if (!isAlertActive) { AlertStart(); }
            AlertAnimate();
        }
    }


    // �v���C���[���������G�����Ȃ��Ȃ�����A�A���[�g�����Z�b�g����
    void AlertReset()
    {
        alert.SetActive(false);
        isAlertActive = false;
        redTimer = 1f;
        redIsGlow = false;
    }
    // �v���C���[���������G��������A�A���[�g���I���ɂ���
    void AlertStart()
    {
        alert.SetActive(true);
        isAlertActive = true;
    }

    // �A���[�g���A�j���[�V����������
    void AlertAnimate()
    {
        if (redTimer <= 0.1f) { redIsGlow = true; }
        if (redTimer >= 0.9f) { redIsGlow = false; }
        if (redIsGlow) { redTimer += Time.deltaTime; }
        else { redTimer -= Time.deltaTime; }
        alert.GetComponent<Image>().color = new Color(1f, 1f, 1f, redTimer);
    }

    //using TMPro;
    //�i���j�^�C�}�[��\�����邽�߂̃e�L�X�g
    //public TextMeshProUGUI timer;
    //�i���j�A���[�g�̃^�C�}�[
    //public float alertTimer = 16f;

    //Start�i���j�^�C�}�[�̃e�L�X�g�̎擾
    //timer = this.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
    //Update
    //if(alertTimer > 0f) { alertTimer -= Time.deltaTime; }
    //timer.text = (Mathf.Floor(alertTimer * 100f) / 100f).ToString();
    // Mathf.Floor(value * 100f) / 100f;


}
