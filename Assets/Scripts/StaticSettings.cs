using UnityEngine;

public class StaticSettings
{
    public static bool useStaticSettings = false;

    // Planet
    public static float planetRadius = 200;
    public static float waterPercent = 0.0125f;

    // Planet Algo Gen
    public static float planetIntervalBeforeNewGeneration = 1;
    public static int planetPopulationSize = 200;
    public static int planetSelectedCount = 8;
    public static float planetProbaMutation = 0.2f;
    public static int planetIcosahedronSubDiv = 2;
    public static string planetScript = Application.dataPath + "/../scorer.lua";
}