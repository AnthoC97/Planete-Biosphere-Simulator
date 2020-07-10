using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using MoonSharp.Interpreter;
using System.IO;

public class GeneticNoise : MonoBehaviour
{
    public bool debug = false;

    [Min(0.5f)]
    public float interval = 1;
    public int populationSize = 10;
    public int selectedCount = 8;
    public float probaMutation = 0.2f;

    Planet planet;
    PBSNoiseScript noiseScript;
    List<FieldInfo> fields;

    List<List<GeneticValue>> solutions;
    List<float> notes;

    List<GeneticValue> bestSolution;
    float bestScore = Mathf.NegativeInfinity;

    public int NbSubdivision = 2;

    static Vector3[] scorerPoints;
    Script script;

    private void OnEnable()
    {
        // Store points before to speed up
        SetupScorerPoints();
    }

    private void OnDisable()
    {
        if (!planet.GetIstUsingNoiseGenetic()) return;
        StopAllCoroutines();

        // Apply best solution
        foreach (GeneticValue geneticVal in bestSolution)
        {
            geneticVal.ApplyValue(noiseScript);
        }
        planet.UpdateNoiseGenerator();
        planet.SetIsUsingNoiseGenetic(false);
    }

    void Start()
    {
        planet = GetComponent<Planet>();
        noiseScript = GetComponent<PBSNoiseScript>();
        if (!noiseScript) return;

        if(StaticSettings.useStaticSettings)
        {
            interval = StaticSettings.planetIntervalBeforeNewGeneration;
            populationSize = StaticSettings.planetPopulationSize;
            selectedCount = StaticSettings.planetSelectedCount;
            probaMutation = StaticSettings.planetProbaMutation;
            NbSubdivision = StaticSettings.planetIcosahedronSubDiv;
        }

        //Recupération de l'objet System.Type représentant le type sous-jacent de l'objet
        Type objectType = noiseScript.GetType();

        solutions = new List<List<GeneticValue>>();
        notes = new List<float>();

        // Setup script lua
        script = null;
        if (File.Exists(StaticSettings.planetScript))
        {
            UserData.RegisterAssembly();
            UserData.RegisterType<Vector3>();
            UserData.RegisterType<Color>();

            script = new Script();
            script.Options.ScriptLoader = new MoonSharp.Interpreter.Loaders.FileSystemScriptLoader();
            ((MoonSharp.Interpreter.Loaders.ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = new string[] { Application.dataPath + "/?", Application.dataPath + "/?.lua" };
            script.Options.DebugPrint = Debug.Log;

            script.Globals["noiseScript"] = planet.pbsNoiseScript;
            script.Globals["bestScore"] = float.MinValue;

            script.Globals["getPoints"] = (Func<Vector3[]>)GetScorerPoints;
            script.Globals["isNaN"] = (Func<float, bool>)IsNaN;
            script.Globals["isInfinity"] = (Func<float, bool>)IsInfinity;

            script.DoFile("scorer.lua");
            populationSize = (int)script.Globals.Get("populationSize").Number;
        }

        StartCoroutine(RunAlgoGenetic());
    }

    IEnumerator RunAlgoGenetic() {
        planet.SetIsUsingNoiseGenetic(true);
        for(int i = 0; i < populationSize; ++i)
            solutions.Add(Generate());

        while (true)
        {
            yield return new WaitForSeconds(interval);
            for (int i = 0; i < solutions.Count; ++i)
            {
                notes.Add(Evaluate(i));
            }
            if (EndCriteria())
            {
                break;
            }
            else
            {
                List<List<GeneticValue>> selections = Selector();
                solutions.Clear();
                solutions.AddRange(selections);
                notes.Clear();
                for(int i=0; i< selections.Count; ++i)
                {
                    for (int j = 0; j < selections.Count; ++j)
                    {
                        if (i == j) continue;
                        solutions.Add(MutationOperator(CrossOperator(solutions[i], solutions[j])));
                    }
                }
            }
        }

        // Apply best solution
        foreach (GeneticValue geneticVal in bestSolution)
        {
            geneticVal.ApplyValue(noiseScript);
        }
        planet.UpdateNoiseGenerator();
        planet.SetIsUsingNoiseGenetic(false);
    }

    List<GeneticValue> Generate()
    {
        List<GeneticValue> solution = new List<GeneticValue>();
        foreach(FieldInfo fieldInfo in noiseScript.GetType().GetFields())
        {
            if(fieldInfo.FieldType == typeof(int))
            {
                foreach (Attribute attribute in fieldInfo.GetCustomAttributes())
                {
                    switch (attribute)
                    {
                        case RangeAttribute ra:
                            int min = Mathf.RoundToInt(ra.min);
                            int max = Mathf.RoundToInt(ra.max);
                            solution.Add(new IntGenetic(UnityEngine.Random.Range(min, max), min, max, fieldInfo.Name));
                            break;
                    }
                }
            }
            else if (fieldInfo.FieldType == typeof(float))
            {
                foreach (Attribute attribute in fieldInfo.GetCustomAttributes())
                {
                    switch (attribute)
                    {
                        case RangeAttribute ra:
                            float min = Mathf.RoundToInt(ra.min);
                            float max = Mathf.RoundToInt(ra.max);
                            solution.Add(new FloatGenetic(UnityEngine.Random.Range(min, max), min, max, fieldInfo.Name));
                            break;
                    }
                }
            }
            else if(fieldInfo.FieldType.IsEnum)
            {
                int min = 0;
                int max = fieldInfo.FieldType.GetEnumValues().Length-1;
                foreach (Attribute attribute in fieldInfo.GetCustomAttributes())
                {
                    switch (attribute)
                    {
                        case MinAttribute mina:
                            min = (int)mina.min;
                            break;
                    }
                }
                solution.Add(new EnumGenetic(UnityEngine.Random.Range(0, max), min, max, fieldInfo.Name));
            }
        }
        return solution;
    }

    float Evaluate(int index)
    {
        List<GeneticValue> solution = solutions[index];
        float score = 0;

        // Apply solution
        foreach(GeneticValue geneticVal in solution)
        {
            geneticVal.ApplyValue(noiseScript);
        }

        if (script != null)
        {
            DynValue res = script.Call(script.Globals["getScore"]);
            score = (float)res.Number;
        }

        // Si meilleur score l'afficher
        if (score > bestScore)
        {
            bestScore = score;
            bestSolution = solution;
            if(script != null) script.Globals["bestScore"] = bestScore;
            print("new best score: "+ bestScore);
            planet.UpdateNoiseGenerator();
        }

        return score;
    }

    bool EndCriteria()
    {
        bool End = true;

        if(script != null)
        {
            DynValue res = script.Call(script.Globals["isEndCriteria"]);
            End = res.Boolean;
        }
        return End;
    }

    List<List<GeneticValue>> Selector()
    {
        int nBest = Mathf.Min(notes.Count-1, selectedCount);
        List<List<GeneticValue>> selections = new List<List<GeneticValue>>();

        for(int i=0; i<nBest; ++i)
        {
            int index = notes.IndexOf(notes.Max());
            selections.Add(solutions[index]);

            solutions.RemoveAt(index);
            notes.RemoveAt(index);
        }

        return selections;
    }

    List<GeneticValue> CrossOperator(List<GeneticValue> solution1, List<GeneticValue> solution2)
    {
        int num = solution1.Count/2;
        List<GeneticValue> newSolution = solution1.GetRange(0, num-1);
        newSolution.AddRange(solution2.GetRange(num, solution2.Count-num-1));
        return newSolution;
    }

    List<GeneticValue> MutationOperator(List<GeneticValue> solution)
    {
        if (probaMutation < UnityEngine.Random.value) return solution;
        int randIndex = UnityEngine.Random.Range(0, solution.Count - 1);
        solution[randIndex].SetValue(solution[randIndex].GetRandomValue());
        return solution;
    }

    private void OnDrawGizmos()
    {
        if (!debug) return;

        Gizmos.color = Color.green;
        PBSNoiseGenerator NoiseGenerator = planet.noiseGenerator;
        foreach (Vector3 point in scorerPoints)
        {
            float elevation = NoiseGenerator.GetNoise3D(point);
            elevation = Mathf.Clamp(elevation, 0, 1);
            if (float.IsNaN(elevation)) elevation = 0;
            Gizmos.DrawWireSphere(point * (1 + elevation) * planet.radius, 1.0f);
        }
    }

    private void SetupScorerPoints()
    {
        List<Vector3> listPoints = new List<Vector3>();

        // An icosahedron has 12 vertices, and
        // since it's completely symmetrical the
        // formula for calculating them is kind of
        // symmetrical too:

        List<TriangleIndex> m_Polygons = new List<TriangleIndex>();

        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        listPoints.Add(new Vector3(-1, t, 0).normalized);
        listPoints.Add(new Vector3(1, t, 0).normalized);
        listPoints.Add(new Vector3(-1, -t, 0).normalized);
        listPoints.Add(new Vector3(1, -t, 0).normalized);
        listPoints.Add(new Vector3(0, -1, t).normalized);
        listPoints.Add(new Vector3(0, 1, t).normalized);
        listPoints.Add(new Vector3(0, -1, -t).normalized);
        listPoints.Add(new Vector3(0, 1, -t).normalized);
        listPoints.Add(new Vector3(t, 0, -1).normalized);
        listPoints.Add(new Vector3(t, 0, 1).normalized);
        listPoints.Add(new Vector3(-t, 0, -1).normalized);
        listPoints.Add(new Vector3(-t, 0, 1).normalized);

        // And here's the formula for the 20 sides,
        // referencing the 12 vertices we just created.
        m_Polygons.Add(new TriangleIndex(0, 11, 5));
        m_Polygons.Add(new TriangleIndex(0, 5, 1));
        m_Polygons.Add(new TriangleIndex(0, 1, 7));
        m_Polygons.Add(new TriangleIndex(0, 7, 10));
        m_Polygons.Add(new TriangleIndex(0, 10, 11));
        m_Polygons.Add(new TriangleIndex(1, 5, 9));
        m_Polygons.Add(new TriangleIndex(5, 11, 4));
        m_Polygons.Add(new TriangleIndex(11, 10, 2));
        m_Polygons.Add(new TriangleIndex(10, 7, 6));
        m_Polygons.Add(new TriangleIndex(7, 1, 8));
        m_Polygons.Add(new TriangleIndex(3, 9, 4));
        m_Polygons.Add(new TriangleIndex(3, 4, 2));
        m_Polygons.Add(new TriangleIndex(3, 2, 6));
        m_Polygons.Add(new TriangleIndex(3, 6, 8));
        m_Polygons.Add(new TriangleIndex(3, 8, 9));
        m_Polygons.Add(new TriangleIndex(4, 9, 5));
        m_Polygons.Add(new TriangleIndex(2, 4, 11));
        m_Polygons.Add(new TriangleIndex(6, 2, 10));
        m_Polygons.Add(new TriangleIndex(8, 6, 7));
        m_Polygons.Add(new TriangleIndex(9, 8, 1));

        var midPointCache = new Dictionary<int, int>();

        for (int i = 0; i < NbSubdivision; i++)
        {
            var newPolys = new List<TriangleIndex>();
            foreach (var poly in m_Polygons)
            {
                int a = poly.a;
                int b = poly.b;
                int c = poly.c;
                // Use GetMidPointIndex to either create a
                // new vertex between two old vertices, or
                // find the one that was already created.
                int ab = GetMidPointIndex(midPointCache, a, b, ref listPoints);
                int bc = GetMidPointIndex(midPointCache, b, c, ref listPoints);
                int ca = GetMidPointIndex(midPointCache, c, a, ref listPoints);
                // Create the four new polygons using our original
                // three vertices, and the three new midpoints.
                newPolys.Add(new TriangleIndex(a, ab, ca));
                newPolys.Add(new TriangleIndex(b, bc, ab));
                newPolys.Add(new TriangleIndex(c, ca, bc));
                newPolys.Add(new TriangleIndex(ab, bc, ca));
            }
            // Replace all our old polygons with the new set of
            // subdivided ones.
            m_Polygons = newPolys;
        }

        scorerPoints = listPoints.ToArray();
    }

    public int GetMidPointIndex(Dictionary<int, int> cache, int indexA, int indexB, ref List<Vector3> listPoints)
    {
        // We create a key out of the two original indices
        // by storing the smaller index in the upper two bytes
        // of an integer, and the larger index in the lower two
        // bytes. By sorting them according to whichever is smaller
        // we ensure that this function returns the same result
        // whether you call
        // GetMidPointIndex(cache, 5, 9)
        // or...
        // GetMidPointIndex(cache, 9, 5)
        int smallerIndex = Mathf.Min(indexA, indexB);
        int greaterIndex = Mathf.Max(indexA, indexB);
        int key = (smallerIndex << 16) + greaterIndex;
        // If a midpoint is already defined, just return it.
        int ret;
        if (cache.TryGetValue(key, out ret))
            return ret;
        // If we're here, it's because a midpoint for these two
        // vertices hasn't been created yet. Let's do that now!
        Vector3 p1 = listPoints[indexA];
        Vector3 p2 = listPoints[indexB];
        Vector3 middle = Vector3.Lerp(p1, p2, 0.5f).normalized;

        ret = listPoints.Count;
        listPoints.Add(middle);

        cache.Add(key, ret);
        return ret;
    }

    public static Vector3[] GetScorerPoints()
    {
        return scorerPoints;
    }

    public static bool IsNaN(float value)
    {
        return float.IsNaN(value);
    }

    public static bool IsInfinity(float value)
    {
        return float.IsInfinity(value);
    }
}

public class TriangleIndex
{
    public int a;
    public int b;
    public int c;
    public TriangleIndex(int a, int b, int c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }
}

public abstract class GeneticValue
{
    protected string name;

    public virtual void ApplyValue(PBSNoiseScript noiseScript)
    {
    }

    public virtual void SetValue(object v)
    {
    }

    public virtual object GetRandomValue()
    {
        return null;
    }
}

public class IntGenetic : GeneticValue
{
    public int value, min, max;

    public IntGenetic(int value, int min, int max, string name)
    {
        this.value = value;
        this.min = min;
        this.max = max;
        this.name = name;
    }

    public override void ApplyValue(PBSNoiseScript noiseScript)
    {
        noiseScript.GetType().GetField(name).SetValue(noiseScript, value);
    }

    public override void SetValue(object v)
    {
        value = (int)v;
    }

    public override object GetRandomValue()
    {
        return UnityEngine.Random.Range(min, max);
    }
}

public class FloatGenetic : GeneticValue
{
    public float value, min, max;

    public FloatGenetic(float value, float min, float max, string name)
    {
        this.value = value;
        this.min = min;
        this.max = max;
        this.name = name;
    }

    public override void ApplyValue(PBSNoiseScript noiseScript)
    {
        noiseScript.GetType().GetField(name).SetValue(noiseScript, value);
    }

    public override void SetValue(object v)
    {
        value = (float)v;
    }

    public override object GetRandomValue()
    {
        return UnityEngine.Random.Range(min, max);
    }
}

public class EnumGenetic : GeneticValue
{
    public int value, min, max;

    public EnumGenetic(int value, int min, int max, string name)
    {
        this.value = value;
        this.min = min;
        this.max = max;
        this.name = name;
    }

    public override void ApplyValue(PBSNoiseScript noiseScript)
    {
        noiseScript.GetType().GetField(name).SetValue(noiseScript, value);
    }

    public override void SetValue(object v)
    {
        value = (int)v;
    }

    public override object GetRandomValue()
    {
        return UnityEngine.Random.Range(min, max);
    }
}