using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Grammar", menuName = "ScriptableObjects/Grammar", order = 1)]
public class Grammar : ScriptableObject
{
    public char[] V; 
    public char[] S;
    public string axiom;
    public string[] P;
}
