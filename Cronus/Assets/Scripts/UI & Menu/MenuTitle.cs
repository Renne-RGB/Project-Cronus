using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Splines;

public class MenuTitle : MenuBase
{
    void Start()
    {
        selectMax = 2;
        ChangeTextColor();
    }

    void Update()
    {
        GetUp();
        GetDown();
        GetSpace();
        Select();
    }

    // ‘I‘ð‚·‚éˆ—
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
