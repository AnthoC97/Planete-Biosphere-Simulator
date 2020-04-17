using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Grammar : ScriptableObject
{
    public char[] S;
    public string axiom;
    public List<string> P;
}
