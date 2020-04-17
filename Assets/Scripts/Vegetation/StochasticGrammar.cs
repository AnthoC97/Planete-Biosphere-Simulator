using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stochastic Grammar", menuName = "ScriptableObjects/Grammar/Stochastic", order = 2)]
public class StochasticGrammar : Grammar
{
    [SerializeField] public List<char> V_Keys;
    [SerializeField] public List<int> V_Values;
}
