using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExpItemDataBase", menuName = "Scriptable Objects/ExpItemDataBase")]
public class ExpItemDataBase : ScriptableObject 
{
    public List<ExpItemEntry> entryList;
}