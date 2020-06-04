using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RandomGenerator
{
    PoissonDisc
}

[Serializable]
public struct SpawnerConfig
{
    public RandomGenerator randomGenerator;
    public int seed;
    public float minDistance;
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
            PoissonDiscSampler poissonDisc = gameObject.AddComponent<PoissonDiscSampler>();
            poissonDisc.NewPointAdded += OnNewPointAdded;
            poissonDisc.SamplingFinished += OnSamplingFinished;

            poissonDisc.regionWidth = 100;
            poissonDisc.regionHeight = 100;
            poissonDisc.regionLength = 100;
            poissonDisc.minDistance = config.minDistance;
            poissonDisc.rejectionLimit = 30;
            poissonDisc.isSpherical = true;

            poissonDisc.StartSampling(config.seed);
        }
    }

    void OnNewPointAdded(Vector3 p)
    {
        //Debug.Log("Point: " + p + " is added!");
        GameObject go = GameObject.Instantiate(configs[0].obj);
        float theta = p.x;
        float phi = p.y;
        p.x = Mathf.Sin(theta) * Mathf.Cos(phi);
        p.y = Mathf.Sin(theta) * Mathf.Sin(phi);
        p.z = Mathf.Cos(theta);
        float elevation = planet.noiseGenerator.GetNoise3D(p.normalized);
        p = p.normalized * (1 + elevation) * planet.radius;
        go.transform.position = p;
        go.transform.LookAt(p.normalized);
    }

    void OnSamplingFinished()
    {
        Debug.LogWarning("Sampling finished!");

        //Debug.Log("Point count: " + sampler.Points.Length);

    }
}
