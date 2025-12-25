using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class TitleScreen : MonoBehaviour
{
    public List<TextMeshProUGUI> textList;      // 選択項目。現在はテキストを入れてある

    int selectNum = 0;                          // 現在選択中の番号
    int selectMin = 0;                          // 
    int selectMax = 2;                          // 
    bool isSelected = false;                    // 選択されたかどうか

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextChangeColor();
    }

    void Update()
    {
        GetUp();
        GetDown();
    }

    // メニュー選択　上方向
    void GetUp()
    {
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !isSelected)
        {
            if (selectNum == selectMin)
            {
                selectNum = selectMax;
            }
            else
            {
                selectNum--;
            }

            TextChangeColor();
        }
    }

    // メニュー選択　下方向
    void GetDown()
    {
        if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !isSelected)
        {
            if (selectNum == selectMax)
            {
                selectNum = selectMin;
            }
            else
            {
                selectNum++;
            }

            TextChangeColor();
        }
    }


    // 選択項目の色を変える
    void TextChangeColor()
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

}
