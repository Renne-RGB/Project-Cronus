using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Splines;

public class TitleScreen : MonoBehaviour
{
    public List<TextMeshProUGUI> textList;      // 選択項目。現在はテキストを入れてある

    int selectNum = 0;                          // 現在選択中の番号
    int selectMax = 2;                          // 選択項目の最大数
    bool isSelected = false;                    // 選択されたかどうか
    float timer = 0.0f;                         // 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeTextColor();
    }

    void Update()
    {
        GetUp();
        GetDown();
        GetEnter();
        Select();
    }

    // メニュー選択　上方向
    void GetUp()
    {
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !isSelected)
        {
            if (selectNum == 0)
            {
                selectNum = selectMax;
            }
            else
            {
                selectNum--;
            }

            ChangeTextColor();
        }
    }

    // メニュー選択　下方向
    void GetDown()
    {
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !isSelected)
        {
            if (selectNum == selectMax)
            {
                selectNum = 0;
            }
            else
            {
                selectNum++;
            }

            ChangeTextColor();
        }
    }

    // メニュー選択　エンターキー
    void GetEnter()
    {
        if(Input.GetKeyDown(KeyCode.Return) && !isSelected)
        {
            isSelected = true;

        }
    }

    // 選択項目の色を変える
    void ChangeTextColor()
    {
        for (int i = 0; i < textList.Count; i++)
        {
            if (i == selectNum)
            {
                textList[i].color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            }
            else
            {
                textList[i].color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
            }

        }
    }

    // 選択する処理
    void Select()
    {
        if (isSelected)
        {
            timer += (Time.deltaTime * 3);

            if (((int)timer % 2) == 0)
            {
                textList[selectNum].color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
            }
            else
            {
                textList[selectNum].color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
            }

            if (timer >= 3.0f)
            {
                isSelected = false;
                timer = 0.0f;

                if(selectNum == 0)
                {
                    SceneManager.LoadScene("Scenes/VisibleTest");
                }
                else if (selectNum == 1)
                {
                    SceneManager.LoadScene("Scenes/GameScene");
                }
                else if (selectNum == 2)
                {
                    Application.Quit();
                }
            }
        }
    }



}
