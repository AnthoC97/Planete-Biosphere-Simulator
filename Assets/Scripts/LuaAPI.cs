using UnityEngine;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class LuaAPI
{
    private Planet planet;
    private PBSNoiseScript planetNoiseScript;

    public LuaAPI()
    {
        planet = GameObject.Find("Planet").GetComponent<Planet>();
        planetNoiseScript =
            GameObject.Find("Planet").GetComponent<PBSNoiseScript>();
    }

    public static void Register(Script script)
    {
        script.Globals["API"] = new LuaAPI();
    }

    public Vector3 GetGroundPositionWithElevation(Vector3 normalizedPosition,
            float addedElevation)
    {
        float elevation = planetNoiseScript
            .GetNoiseGenerator().GetNoise3D(normalizedPosition);
        return normalizedPosition * (1 + elevation) * planet.radius;
    }
}
