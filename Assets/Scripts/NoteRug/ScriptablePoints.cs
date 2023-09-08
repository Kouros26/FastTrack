using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Score", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class ScriptablePoints : ScriptableObject
{
    public int mScore = 0;
}
