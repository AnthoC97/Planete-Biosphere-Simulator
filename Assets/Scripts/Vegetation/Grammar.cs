using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Grammar : ScriptableObject
{
    public char[] S;
    public string axiom;
    public List<string> P;
    public char[] V;

    public string tree_name;
    public float trunc_height;
    public float trunc_radius;
    public int delta_angle;
    //rate of each tree nutrimetns-----------------
    public float glucid; //between 0 and  1
    public float water; //between 0 and  1
    public float minerals; //between 0 and  1
    //---------------------------------------------
}
