using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;

public class GeneticNoise : MonoBehaviour
{
    [Min(0.5f)]
    public float interval = 1;

    public int populationSize = 10;

    public bool debug = false;

    PBSNoiseScript noiseScript;
    List<FieldInfo> fields;

    List<List<IntGenetic>> solutions;
    List<float> notes;

    List<IntGenetic> bestSolution;
    float bestScore = Mathf.NegativeInfinity;

    public int sectorCount = 5;
    public int stackCount = 5;

    void Start()
    {
        noiseScript = GetComponent<CustomNoiseScript>();
        if (!noiseScript) return;

        //Recupération de l'objet System.Type représentant le type sous-jacent de l'objet
        Type objectType = noiseScript.GetType();

        solutions = new List<List<IntGenetic>>();
        notes = new List<float>();

        StartCoroutine(RunAlgoGenetic());
    }

    IEnumerator RunAlgoGenetic() {
        for(int i = 0; i < populationSize; ++i)
            solutions.Add(Generate());

        while (true)
        {
            yield return new WaitForSeconds(interval);
            for (int i = 0; i < solutions.Count; ++i)
            {
                notes.Add(Evaluate(i));
                yield return new WaitForSeconds(0.1f);
            }
            if (EndCriteria())
            {
                break;
            }
            else
            {
                List<List<IntGenetic>> selections = Selector();
                solutions.Clear();
                notes.Clear();
                foreach(List<IntGenetic> solution1 in selections)
                {
                    foreach(List<IntGenetic> solution2 in selections)
                    {
                        if (solution1 == solution2) continue;
                        solutions.Add(MutationOperator(CrossOperator(solution1, solution2)));
                    }
                }
            }
        }
    }

    List<IntGenetic> Generate()
    {
        List<IntGenetic> solution = new List<IntGenetic>();
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
        }
        return solution;
    }

    float Evaluate(int index)
    {
        List<IntGenetic> solution = solutions[index];
        float maxBelow = 0.2f;
        float percentage = 0.5f;
        List<float> below = new List<float>();
        List<float> above = new List<float>();

        // Apply solution
        foreach(IntGenetic intGenetic in solution)
        {
            intGenetic.ApplyValue(noiseScript);
        }

        float x, y, z, xy;                              // vertex position

        float sectorStep = 2 * Mathf.PI / sectorCount;
        float stackStep = Mathf.PI / stackCount;
        float sectorAngle, stackAngle;

        for (int i = 0; i <= stackCount; ++i)
        {
            stackAngle = Mathf.PI / 2 - i * stackStep;  // starting from pi/2 to -pi/2
            xy = Mathf.Cos(stackAngle);                 // cos(u)
            z = Mathf.Sin(stackAngle);                  // sin(u)

            // add (sectorCount+1) vertices per stack
            // the first and last vertices have same position and normal, but different tex coords
            for (int j = 0; j <= sectorCount; ++j)
            {
                sectorAngle = j * sectorStep;           // starting from 0 to 2pi

                // vertex position (x, y, z)
                x = xy * Mathf.Cos(sectorAngle);             // r * cos(u) * cos(v)
                y = xy * Mathf.Sin(sectorAngle);             // r * cos(u) * sin(v)

                Vector3 pos = new Vector3(x, z, y);

                float elevation = noiseScript.GetNoiseGenerator().GetNoise3D(pos);
                if (elevation <= maxBelow) below.Add(elevation);
                else above.Add(elevation);
            }
        }

        float percent = (float)below.Count/(below.Count + above.Count);
        float score = 1 - Mathf.Abs(percentage - percent);
        if(score > bestScore)
        {
            bestScore = score;
            bestSolution = solution;
            print("new best score: "+ bestScore);
            GetComponent<Planet>().UpdateNoiseGenerator(noiseScript.GetNoiseGenerator());
        }

        return score;
    }

    bool EndCriteria()
    {
        return bestScore >= 0.8;
    }

    List<List<IntGenetic>> Selector()
    {
        int nBest = 8;
        List<List<IntGenetic>> selections = new List<List<IntGenetic>>();

        for(int i=0; i<nBest; ++i)
        {
            int index = notes.IndexOf(notes.Max());
            selections.Add(solutions[index]);

            solutions.RemoveAt(index);
            notes.RemoveAt(index);
        }

        return selections;
    }

    List<IntGenetic> CrossOperator(List<IntGenetic> solution1, List<IntGenetic> solution2)
    {
        return solution1;
    }

    List<IntGenetic> MutationOperator(List<IntGenetic> solution)
    {
        float probaMutation = 0.2f;
        if (probaMutation > 0.2f) return solution;
        int randIndex = UnityEngine.Random.Range(0, solution.Count - 1);
        solution[randIndex].SetValue(UnityEngine.Random.Range(solution[randIndex].min, solution[randIndex].max));
        return solution;
    }

    private void OnDrawGizmos()
    {
        if (!debug) return;
        Gizmos.color = Color.green;
        float x, y, z, xy;                              // vertex position

        float sectorStep = 2 * Mathf.PI / sectorCount;
        float stackStep = Mathf.PI / stackCount;
        float sectorAngle, stackAngle;

        for (int i = 0; i <= stackCount; ++i)
        {
            stackAngle = Mathf.PI / 2 - i * stackStep;  // starting from pi/2 to -pi/2
            xy = Mathf.Cos(stackAngle);                 // cos(u)
            z = Mathf.Sin(stackAngle);                  // sin(u)

            // add (sectorCount+1) vertices per stack
            // the first and last vertices have same position and normal, but different tex coords
            for (int j = 0; j <= sectorCount; ++j)
            {
                sectorAngle = j * sectorStep;           // starting from 0 to 2pi

                // vertex position (x, y, z)
                x = xy * Mathf.Cos(sectorAngle);             // r * cos(u) * cos(v)
                y = xy * Mathf.Sin(sectorAngle);             // r * cos(u) * sin(v)
          
                Gizmos.DrawWireSphere(new Vector3(x, z, y),0.1f);
            }
        }
    }
}

public class IntGenetic
{
    public int value, min, max;
    string name;

    public IntGenetic(int value, int min, int max, string name)
    {
        this.value = value;
        this.min = min;
        this.max = max;
        this.name = name;
    }

    public void ApplyValue(PBSNoiseScript noiseScript)
    {
        noiseScript.GetType().GetField(name).SetValue(noiseScript, value);
    }

    public void SetValue(int v)
    {
        value = v;
    }
}