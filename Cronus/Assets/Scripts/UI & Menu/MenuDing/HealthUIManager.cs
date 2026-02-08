using UnityEngine;
using System.Collections.Generic;

public class HealthUIManager : MonoBehaviour
{
    public List<Animator> heartAnims;
    private int lastHP;

    public void InitHealth(int hp)
    {
        lastHP = hp;
        for (int i = 0; i < heartAnims.Count; i++)
        {
            heartAnims[i].Play(i < hp ? "Full" : "Empty");
        }
    }

    public void UpdateUI(int currentHP)
    {
        if (currentHP == lastHP)
            return;

        if (currentHP < lastHP)
        {
            for (int i = currentHP; i < lastHP; i++)
            {
                if (i >= 0 && i < heartAnims.Count)
                    heartAnims[i].SetTrigger("lose");
            }
        }
        else
        {
            for (int i = lastHP; i < currentHP; i++)
            {
                if (i >= 0 && i < heartAnims.Count)
                    heartAnims[i].SetTrigger("gain");
            }
        }
        lastHP = currentHP;
    }
}