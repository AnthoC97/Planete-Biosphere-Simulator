using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PBSNoiseScript : MonoBehaviour
{
    Planet planet;

    private void Start()
    {
        planet = GetComponent<Planet>();
    }

    private void OnValidate()
    {
        if (planet)
            planet.UpdateNoiseGenerator(GetNoiseGenerator());
    }

    public virtual PBSNoiseGenerator GetNoiseGenerator()
    {
        return new PBSNoiseGenerator();
    }

    public virtual Color GetColor()
    {
        return Color.clear;
    }
}
