using System;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerAbility : MonoBehaviour
{
    public event EventHandler<OnAddExpArgs> OnAddExp;
    public event Action<int> OnLevelUp;
    int playerLevel = 1;
    int playerEXP = 0;
    int playerLevelMaxEXP = 20;

    public void AddExp(int exp)
    {
        playerEXP += exp;
        if (playerLevelMaxEXP <= playerEXP)
        {
            playerEXP = 0;
            playerLevel += 1;
            OnLevelUp?.Invoke(playerLevel);
        }

        OnAddExp?.Invoke(this,new OnAddExpArgs(playerEXP,playerLevelMaxEXP));
        // Debug.Log($"LV : {playerLevel} EXP : {playerEXP}");
    }
}
