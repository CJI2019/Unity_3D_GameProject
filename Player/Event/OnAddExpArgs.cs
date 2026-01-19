using System;
using UnityEngine;

public class OnAddExpArgs :EventArgs
{
    public int CurrentExp { get; }
    public int MaxExp { get; }

    public OnAddExpArgs(int currentExp, int maxExp)
    { 
        CurrentExp = currentExp;
        MaxExp = maxExp;
    }
}
