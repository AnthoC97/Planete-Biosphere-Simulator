using MoonSharp.Interpreter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent, MoonSharpUserData]
public class PBSNoiseScript : MonoBehaviour
{
    Planet planet;

    private void Start()
    {
        planet = GetComponent<Planet>();
    }

    private void OnValidate()
    {
        if (planet && !planet.GetIstUsingNoiseGenetic())
            planet.UpdateNoiseGenerator(GetNoiseGenerator());
    }

    public virtual PBSNoiseGenerator GetNoiseGenerator()
    {
        return new PBSNoiseGenerator();
    }

    public virtual Color GetColor(Vector3 pointOnUnitSphere)
    {
        return Color.white;
    }
}
