using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Multiplies/Adds values
/// </summary>
public class ScaleBiasNoiseModule : PBSNoiseGenerator
{
    PBSNoiseGenerator inputModule;
    float scale;
    float bias;

    public ScaleBiasNoiseModule(PBSNoiseGenerator inputModule, float scale, float bias)
    {
        this.inputModule = inputModule;
        this.scale = scale;
        this.bias = bias;
    }

    public override float GetNoise3D(Vector3 vector)
    {
        if (inputModule==null)
        {
            return 0.0f;
        }

        return (inputModule.GetNoise3D(vector) * scale) + bias;
    }
}