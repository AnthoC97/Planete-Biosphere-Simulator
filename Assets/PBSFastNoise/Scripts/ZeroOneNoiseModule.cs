using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Clamp between 0 and 1
public class ZeroOneNoiseModule : PBSNoiseGenerator
{
    PBSNoiseGenerator noiseGenerator;
    public ZeroOneNoiseModule(PBSNoiseGenerator noiseGenerator)
    {
        this.noiseGenerator = noiseGenerator;
    }

    public override float GetNoise3D(Vector3 vector)
    {
        float elevation = noiseGenerator.GetNoise3D(vector);
        if (float.IsInfinity(elevation) || float.IsNaN(elevation))
            elevation = 1f;
        return Mathf.Clamp(elevation, 0, 1.0f);
    }
}
