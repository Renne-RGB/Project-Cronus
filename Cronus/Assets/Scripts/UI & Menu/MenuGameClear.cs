using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Splines;

public class MenuGameClear : MenuBase
{
    void Update()
    {
        GetSpace();
        Select();
    }

    // ‘I‘ğ‚·‚éˆ—
    void Select()
    {
        if (isSelected)
        {
            SceneManager.LoadScene("Scenes/GameScene");
        }
    }

}
