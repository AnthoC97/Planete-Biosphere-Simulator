using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum RandomGenerator
{
    //PoissonDisc,
    RandomLinear
}

[Serializable]
public struct SpawnerConfig
{
    public RandomGenerator randomGenerator;
    public int seed;
    public float minDistance;
    public float maxIteration;
    public GameObject obj;
}

public class SpawnerGenerator : MonoBehaviour
{
    [SerializeField]
    SpawnerConfig[] configs;

    Planet planet;

    // Start is called before the first frame update
    void Start()
    {
        planet = GetComponent<Planet>();

        foreach (SpawnerConfig config in configs)
        {
            switch(config.randomGenerator)
            {
            case RandomGenerator.RandomLinear:
                RandomLinear(config);
                break;
            }
        }
    }

    void RandomLinear(SpawnerConfig config)
    {
        Random.InitState(config.seed);
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < config.maxIteration; ++i)
        {
            Vector3 unitSphere = Random.onUnitSphere;
            float elevation = planet.noiseGenerator.GetNoise3D(unitSphere);
            Vector3 pos = unitSphere * (1 + elevation) * planet.radius;

            bool isOk = true;
            foreach(Vector3 opos in positions)
            {
                if (Vector3.Distance(pos, opos) < config.minDistance)
                {
                    isOk = false;
                    break;
                }
            }
            if(isOk)
            {
                positions.Add(pos);
            }
        }

        foreach(Vector3 pos in positions)
        {
            GameObject go = GameObject.Instantiate(config.obj);
            go.transform.position = pos;
            go.transform.transform.rotation = Quaternion.FromToRotation(Vector3.up, pos.normalized);
        }
    }
}
