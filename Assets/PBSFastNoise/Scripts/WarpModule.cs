using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WarpIterationsType
{
    One,
    Two
};

// Domain warping
public class WarpModule : PBSNoiseGenerator
{
    PBSNoiseGenerator inputModule;
    PBSNoiseGenerator warpModule;

    float Iteration1XOffset;
    float Iteration1YOffset;
    float Iteration1ZOffset;

    float Iteration2XOffset1;
    float Iteration2YOffset1;
    float Iteration2ZOffset1;
    float Iteration2XOffset2;
    float Iteration2YOffset2;
    float Iteration2ZOffset2;

    WarpIterationsType WarpIterations;

    float UnitSize;

    float Multiplier;

    public WarpModule(PBSNoiseGenerator inputModule, PBSNoiseGenerator warpModule, float multiplier, WarpIterationsType warpIterations)
    {
        this.inputModule = inputModule;
        this.warpModule = warpModule;
        this.Iteration1XOffset = 1.6f;
        this.Iteration1YOffset = 2.5f;
        this.Iteration2XOffset1 = 5.7f;
        this.Iteration2YOffset1 = 3.4f;
        this.Iteration2XOffset2 = 2.1f;
        this.Iteration2YOffset2 = 3.5f;
        this.Multiplier = multiplier;
        this.WarpIterations = warpIterations;
    }

    public override float GetNoise3D(Vector3 vector)
    {
        if (inputModule == null || warpModule == null)
        {
            return 0.0f;
        }

        Vector3 q = new Vector3(warpModule.GetNoise3D(vector), warpModule.GetNoise3D(vector + new Vector3(Iteration1XOffset, Iteration1YOffset, Iteration1ZOffset)), warpModule.GetNoise3D(vector + new Vector3(Iteration1XOffset + 0.5f, Iteration1YOffset + 0.5f, Iteration1ZOffset + 2.4f)));

        if (WarpIterations == WarpIterationsType.One)
        {
            return (inputModule.GetNoise3D(vector + new Vector3( (Multiplier * q.x), (Multiplier * q.y), (Multiplier * q.z))));
        }

        Vector3 r = new Vector3(warpModule.GetNoise3D(vector + new Vector3((Multiplier * q).x + Iteration2XOffset1, (Multiplier * q).y + Iteration2YOffset1, (Multiplier * q).z + Iteration2ZOffset1)),
            warpModule.GetNoise3D(vector + new Vector3((Multiplier * q).x + Iteration2XOffset2, (Multiplier * q).y + Iteration2YOffset2, (Multiplier * q).z + Iteration2ZOffset2)),
            warpModule.GetNoise3D(vector + new Vector3((Multiplier * q).x + 3.4f + Iteration2XOffset2, (Multiplier * q).y + Iteration2YOffset2 + 4.6f, (Multiplier * q).z + Iteration2ZOffset2 + 2.3f)));

        return (inputModule.GetNoise3D(vector + new Vector3(Multiplier * r.x, Multiplier * r.y, Multiplier * r.z)));
    }
}
