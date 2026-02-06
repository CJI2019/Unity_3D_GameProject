using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "ExpItemDataBase", menuName = "Scriptable Objects/ExpItemDataBase")]
public class ExpItemDataBase : ScriptableObject 
{
    public List<ExpItemEntry> entryList;
}