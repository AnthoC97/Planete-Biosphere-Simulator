using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProbOfRules
{
    [SerializeField]
    public char symbol;
    [SerializeField]
    public List<float> probs;
}
