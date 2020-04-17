using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deterministic Grammar", menuName = "ScriptableObjects/Grammar", order = 1)]
public class DeterministicGrammar : Grammar
{
    public char[] V;
}
