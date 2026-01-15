using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Splines;

public class MenuBase : MonoBehaviour
{
    public List<TextMeshProUGUI> textList;      // 選択項目。現在はテキストを入れてある

    protected int selectNum = 0;                          // 現在選択中の番号
    protected int selectMax = 0;                          // 選択項目の最大数
    protected bool isSelected = false;                    // 選択されたかどうか
    protected float timer = 0.0f;                         // 

    // メニュー選択　上方向
    protected void GetUp()
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
    protected void GetDown()
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

    // メニュー選択　スペースバー
    protected void GetSpace()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isSelected)
        {
            isSelected = true;
        }
    }

    // 選択項目の色を変える
    protected void ChangeTextColor()
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
