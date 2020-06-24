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

    Planet planet;
    PBSNoiseScript noiseScript;
    List<FieldInfo> fields;

    List<List<GeneticValue>> solutions;
    List<float> notes;

    PBSNoiseGenerator bestNoiseGenerator;
    List<GeneticValue> bestSolution;
    float bestScore = Mathf.NegativeInfinity;

    public int loopPointCount = 6;

    public float minElevation = 5000;
    public float maxElevation = -5000;

    List<Vector3> scorerPoints;

    private void OnEnable()
    {
        // Store points before to speed up
        SetupScorerPoints();
    }

    void Start()
    {
        planet = GetComponent<Planet>();
        noiseScript = GetComponent<PBSNoiseScript>();
        if (!noiseScript) return;

        //Recupération de l'objet System.Type représentant le type sous-jacent de l'objet
        Type objectType = noiseScript.GetType();

        solutions = new List<List<GeneticValue>>();
        notes = new List<float>();

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
                int max = fieldInfo.FieldType.GetEnumValues().Length-1;
                solution.Add(new EnumGenetic(UnityEngine.Random.Range(0, max), 0, max, fieldInfo.Name));
            }
        }
        return solution;
    }

    float Evaluate(int index)
    {
        List<GeneticValue> solution = solutions[index];
        float maxBelow = 0.2f;
        float percentage = 0.5f;
        List<float> below = new List<float>();
        List<float> above = new List<float>();
        float score = 0;

        // Apply solution
        foreach(GeneticValue geneticVal in solution)
        {
            geneticVal.ApplyValue(noiseScript);
        }

        Script script = new Script();
        script.Options.ScriptLoader = new MoonSharp.Interpreter.Loaders.FileSystemScriptLoader();
        ((MoonSharp.Interpreter.Loaders.ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = new string[] { Application.dataPath+"/?", Application.dataPath+"/?.lua" };
        if (File.Exists(Application.dataPath+"/../scorer.lua"))
        {
            script.DoFile("scorer.lua");

            foreach (Vector3 point in scorerPoints)
            {
                /*float elevation = noiseScript.GetNoiseGenerator().GetNoise3D(point);
                if (float.IsNaN(elevation) || float.IsInfinity(elevation)) score += -9999999;
                if (elevation == 0) ++score;*/
                Table tablePoint = new Table(script);
                tablePoint.Set("x", DynValue.NewNumber(point.x));
                tablePoint.Set("y", DynValue.NewNumber(point.y));
                tablePoint.Set("z", DynValue.NewNumber(point.z));
                script.Call(script.Globals["scorePerPoint"], tablePoint);
            }

            DynValue res = script.Call(script.Globals["getScore"]);
            Debug.Log(res.Number);
            score = (float)res.Number;
        }

        /*foreach (Vector3 point in scorerPoints)
        {
            float elevation = noiseScript.GetNoiseGenerator().GetNoise3D(point);
            if (float.IsNaN(elevation) || float.IsInfinity(elevation)) score += -9999999;
            /*if (maxElevation < elevation) maxElevation = elevation;
            if (minElevation > elevation) minElevation = elevation;
            if (elevation <= maxBelow) below.Add(elevation);
            else above.Add(elevation);*/
        /*if (elevation == 0) ++score;
      }*/

        /*float percent = (float)below.Count/(below.Count + above.Count);
        float score = 1 - Mathf.Abs(percentage - percent);*/
        //score = score/((loopPointCount + 1) * (loopPointCount + 1));
        if (score > bestScore)
        {
            bestScore = score;
            bestSolution = solution;
            bestNoiseGenerator = noiseScript.GetNoiseGenerator();
            print("new best score: "+ bestScore);
            planet.UpdateNoiseGenerator(bestNoiseGenerator);
        }

        return score;
    }

    bool EndCriteria()
    {
        bool End = true;
        Script script = new Script();
        script.Options.ScriptLoader = new MoonSharp.Interpreter.Loaders.FileSystemScriptLoader();
        ((MoonSharp.Interpreter.Loaders.ScriptLoaderBase)script.Options.ScriptLoader).ModulePaths = new string[] { Application.dataPath + "/?", Application.dataPath + "/?.lua" };
        if (File.Exists(Application.dataPath + "/../scorer.lua"))
        {
            script.DoFile("scorer.lua");

            script.Globals["solutions"] = solutions;
            DynValue res = script.Call(script.Globals["getIsEndCriteria"]);
            End = res.Boolean;
        }
        return End;
        //return bestScore >= 1;//0.95;
    }

    List<List<GeneticValue>> Selector()
    {
        int nBest = 8;
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
        return solution1;
    }

    List<GeneticValue> MutationOperator(List<GeneticValue> solution)
    {
        float probaMutation = 0.2f;
        if (probaMutation > UnityEngine.Random.value) return solution;
        int randIndex = UnityEngine.Random.Range(0, solution.Count - 1);
        solution[randIndex].SetValue(solution[randIndex].GetRandomValue());
        return solution;
    }

    private void OnDrawGizmos()
    {
        if (!debug) return;

        if (bestNoiseGenerator == null) return;

        Gizmos.color = Color.green;

        foreach (Vector3 point in scorerPoints)
        {
            float elevation = bestNoiseGenerator.GetNoise3D(point);
            elevation = Mathf.Clamp(elevation, 0, 1);
            if (float.IsNaN(elevation)) elevation = 0;
            Gizmos.DrawWireSphere(point * (1 + elevation) * planet.radius, 1.0f);
        }
    }

    private void SetupScorerPoints()
    {
        scorerPoints = new List<Vector3>();
        float x, y, z, xy;                              // vertex position

        float sectorStep = 2 * Mathf.PI / loopPointCount;
        float stackStep = Mathf.PI / loopPointCount;
        float sectorAngle, stackAngle;

        for (int i = 0; i <= loopPointCount; ++i)
        {
            stackAngle = Mathf.PI / 2 - i * stackStep;  // starting from pi/2 to -pi/2
            xy = Mathf.Cos(stackAngle);                 // cos(u)
            z = Mathf.Sin(stackAngle);                  // sin(u)

            // add (sectorCount+1) vertices per stack
            // the first and last vertices have same position and normal, but different tex coords
            for (int j = 0; j <= loopPointCount; ++j)
            {
                sectorAngle = j * sectorStep;           // starting from 0 to 2pi

                // vertex position (x, y, z)
                x = xy * Mathf.Cos(sectorAngle);             // r * cos(u) * cos(v)
                y = xy * Mathf.Sin(sectorAngle);             // r * cos(u) * sin(v)

                scorerPoints.Add(new Vector3(x, z, y));
            }
        }
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